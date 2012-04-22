#if KINECT
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;
using Neat.Components;
using Neat;
using Neat.Mathematics;
using System.Diagnostics;
using Neat.Graphics;

namespace Neat.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class KinectEngine : Microsoft.Xna.Framework.GameComponent
    {
        #region FIELDS
        Console Console;
        public KinectSensor Nui;
        public bool ReceiveVideo = false;
        public bool ReceiveDepth = false;
        public Texture2D KinectRGB;
        public Texture2D KinectSkeletons;
        public int[,] KinectDepths;
        int xMax = 640;
        int yMax = 480;
        public Skeleton[] Skeletons;
        public int[] TrackTime = new int[2];
        int trackedSkeletonsCount = 0;
        public int TrackedSkeletonsCount { get { return trackedSkeletonsCount; } }
        #endregion

        #region Initialize
        public KinectEngine(NeatGame game)
            : base(game)
        {
            this.Console = game.Console;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            Initialize(false);
        }

        public virtual void Initialize(bool useDepth)
        {
            // TODO: Add your initialization code here
            Console.AddCommand("k_tilt", k_tilt);
            Console.AddCommand("k_init", k_init);
            Console.AddCommand("k_uninit", k_uninit);
            Console.AddCommand("k_video", k_video);

            if (useDepth)
                Console.AddCommand("k_depth", k_depth);

            try
            {
                Nui = KinectSensor.KinectSensors[0];
                Nui.SkeletonStream.Enable(new TransformSmoothParameters()
                {
                    Correction = 0.5f,
                    Smoothing = 1.0f
                });
                Nui.Start();
            }
            catch
            {
                Debug.WriteLine("Error while initializing kinect. Aborting.");
                throw;
            }
            NeatGame game = ((NeatGame)this.Game);
            Nui.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(nui_VideoFrameReady);
            Nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            Nui.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(Nui_DepthFrameReady);
            KinectRGB = new Texture2D(game.GraphicsDevice, xMax, yMax);
            game.AssignTexture(new Sprite(KinectRGB), "kinectrgb");

            KinectSkeletons = new Texture2D(game.GraphicsDevice, xMax, yMax);
            game.AssignTexture(new Sprite(KinectSkeletons), "kinectskeletons");
            
            Console.WriteLine("Kinect initialized successfully!");

            base.Initialize();
        }


        #endregion

        #region Events
        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool receivedData = false;
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if (Skeletons == null) //allocate the first time
                    {
                        Skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }
                    receivedData = true;
                }
                else
                {
                    // apps processing of skeleton data took too long; it got more than 2 frames behind.
                    // the data is no longer avabilable.
                }
            }
 
            if (receivedData)
            {
                
            }
 

            var frame = e.OpenSkeletonFrame();
            var trackedSkeletons = from s in frame.Skeletons
                                    where
                                        s.TrackingState == SkeletonTrackingState.Tracked
                                    select s;
            trackedSkeletonsCount = trackedSkeletons.Count();
            if (trackedSkeletonsCount == 1) TrackTime[1] = 0;
            if (trackedSkeletonsCount == 0) TrackTime[0] = 0;
            for (int i = 0; i < trackedSkeletonsCount; i++)
            {
                Skeletons[i] = trackedSkeletons.ElementAt(i);
                TrackTime[i]++;
            }
        }

        void Nui_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            if (!ReceiveDepth) return;
            PlanarImage image = e.ImageFrame.Image;

            int depthIndex = 0;
            Color[] bitmap = new Color[xMax * yMax];
            KinectDepths = new int[image.Width, image.Height];
            for (int y = 0; y < image.Height; y++)
            {
                var heightOffset = y * image.Width;
                for (int x = 0; x < image.Width; x++)
                {
                    var index = ((image.Width - x - 1) + heightOffset) * 4;
                    var distance = GetDistanceWithPlayerIndex(image.Bits[depthIndex], image.Bits[depthIndex + 1]);
                    KinectDepths[x, y] = distance;
                    Color c = Color.Transparent;
                    if (GetPlayerIndex(image.Bits[depthIndex]) > 1) c = Color.Blue;
                    else if (GetPlayerIndex(image.Bits[depthIndex]) > 0) c = Color.Red;
                    bitmap[2*y * xMax + 2*x] = c;
                    bitmap[2*y * xMax + (2*x + 1)] = c;
                    bitmap[(2*y + 1) * xMax + 2*x] = c;
                    bitmap[(2*y + 1) * xMax + (2*x + 1)] = c;
                    depthIndex += 2;
                }
            }
            KinectSkeletons.SetData(bitmap);
        }

        void nui_VideoFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            if (!ReceiveVideo) return;
            PlanarImage image = e.ImageFrame.Image;

            int offset = 0;
            Color[] bitmap = new Color[image.Width * image.Height]; //new Color[xMax * yMax];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    bitmap[y * image.Width + x] = new Color(image.Bits[offset + 2], image.Bits[offset + 1], image.Bits[offset], 255);
                    offset += 4;
                }
            }
            KinectRGB.SetData(bitmap);
        }
        #endregion

        #region Uninitialize
        public void Uninitialize()
        {
            try
            {
                Nui.Stop();
                Debug.WriteLine("Kinect uninitialized.");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to uninitialize kinect!");
                Debug.WriteLine(e.Message);
            }
        }
        #endregion

        #region Functions
        public void OpenVideoStream()
        {
            Nui.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            ReceiveVideo = true;
        }
        public void OpenDepthStream()
        {
            Nui.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            ReceiveDepth = true;

        }
        public void Tilt(int degrees)
        {
            Nui.ElevationAngle = degrees;
        }
        #endregion

        #region Helpers

        int GetDistanceWithPlayerIndex(byte firstFrame, byte secondFrame)
        {
            return (int)(firstFrame >> 3 | secondFrame << 5);
        }

        int GetPlayerIndex(byte firstFrame)
        {
            return (int)firstFrame & 7;
        }

        public Vector3 ToVector3(JointID joint, int skeletonId = 0)
        {
            var p = Skeletons[skeletonId].Joints[joint].Position;
            return new Vector3(p.X, p.Y, p.Z);
        }

        public Vector2 ToVector2(JointID joint, int skeletonId = 0)
        {
            var p = ToVector3(joint, skeletonId);
            return new Vector2(p.X, p.Y);
        }

        public static Vector2 ToVector2(Microsoft.Research.Kinect.Nui.Vector p, Vector2 scale)
        {
            var half = scale / 2.0f;
            return (new Vector2(p.X, p.Y) * half * new Vector2(1, -1)) + half;
        }

        public Vector2 ToVector2(JointID joint, Vector2 scale, int skeletonId = 0)
        {
            return ToVector2(Skeletons[skeletonId].Joints[joint].Position, scale);
        }

        public static Vector3 ToVector3(Microsoft.Research.Kinect.Nui.Vector p, Vector3 scale)
        {
            var half = scale / 2.0f;
            return (new Vector3(p.X, p.Y,p.Z) * half * new Vector3(1, -1,1)) + half;
        }

        public Vector3 ToVector3(JointID joint, Vector3 scale, int skeletonId = 0)
        {
            return ToVector3(Skeletons[skeletonId].Joints[joint].Position, scale);
        }

        public int InferredJointsCount(int skeletonId = 0)
        {
            if (trackedSkeletonsCount <= skeletonId || Skeletons[skeletonId] == null) return (int)JointID.Count;
            int r = 0;
            for (JointID i = 0; i < JointID.Count; i++)
            {
                if (Skeletons[skeletonId].Joints[i].TrackingState != JointTrackingState.Tracked) r++;
            }
            return r;
        }
        #endregion

        #region Render
public void DrawSkeleton(SpriteBatch spriteBatch, LineBrush lb, Vector2 position, Vector2 size, Color color, int skeletonId = 0)
{
    if (Skeletons.Length <= skeletonId || Skeletons[skeletonId] == null)
    {
        //Skeleton not found. Draw an X
        lb.Draw(spriteBatch, position, position + size, color);
        lb.Draw(spriteBatch, new LineSegment(position.X+size.X, position.Y, position.X, position.Y + size.Y), color);
        return;
    }

    //Right Hand
    lb.Draw(spriteBatch, ToVector2(JointID.HandRight, size, skeletonId), 
        ToVector2(JointID.WristRight, size, skeletonId), color, position);
    lb.Draw(spriteBatch, ToVector2(JointID.WristRight, size, skeletonId), 
        ToVector2(JointID.ElbowRight, size, skeletonId), color, position);
    lb.Draw(spriteBatch, ToVector2(JointID.ElbowRight, size, skeletonId), 
        ToVector2(JointID.ShoulderRight, size, skeletonId), color, position);

    //Head & Shoulders
    lb.Draw(spriteBatch, ToVector2(JointID.ShoulderRight, size, skeletonId), 
        ToVector2(JointID.ShoulderCenter, size, skeletonId), color, position);
    lb.Draw(spriteBatch, ToVector2(JointID.Head, size, skeletonId), 
        ToVector2(JointID.ShoulderCenter, size, skeletonId), color, position);
    lb.Draw(spriteBatch, ToVector2(JointID.ShoulderCenter, size, skeletonId), 
        ToVector2(JointID.ShoulderLeft, size, skeletonId), color, position);
            
    //Left Hand
    lb.Draw(spriteBatch, ToVector2(JointID.HandLeft, size, skeletonId), 
        ToVector2(JointID.WristLeft, size, skeletonId), color, position);
    lb.Draw(spriteBatch, ToVector2(JointID.WristLeft, size, skeletonId), 
        ToVector2(JointID.ElbowLeft, size, skeletonId), color, position);
    lb.Draw(spriteBatch, ToVector2(JointID.ElbowLeft, size, skeletonId), 
        ToVector2(JointID.ShoulderLeft, size, skeletonId), color, position);

    //Hips & Spine
    lb.Draw(spriteBatch, ToVector2(JointID.HipLeft, size, skeletonId), 
        ToVector2(JointID.HipCenter, size, skeletonId), color, position);
    lb.Draw(spriteBatch, ToVector2(JointID.HipRight, size, skeletonId), 
        ToVector2(JointID.HipCenter, size, skeletonId), color, position);
    lb.Draw(spriteBatch, ToVector2(JointID.Spine, size, skeletonId), 
        ToVector2(JointID.HipCenter, size, skeletonId), color, position);
    lb.Draw(spriteBatch, ToVector2(JointID.Spine, size, skeletonId), 
        ToVector2(JointID.ShoulderCenter, size, skeletonId), color, position);

    //Left foot
    lb.Draw(spriteBatch, ToVector2(JointID.HipLeft, size, skeletonId), 
        ToVector2(JointID.KneeLeft, size, skeletonId), color, position);
    lb.Draw(spriteBatch, ToVector2(JointID.KneeLeft, size, skeletonId), 
        ToVector2(JointID.AnkleLeft, size, skeletonId), color, position);
    lb.Draw(spriteBatch, ToVector2(JointID.AnkleLeft, size, skeletonId), 
        ToVector2(JointID.FootLeft, size, skeletonId), color, position);

    //Right foot
    lb.Draw(spriteBatch, ToVector2(JointID.HipRight, size, skeletonId), 
        ToVector2(JointID.KneeRight, size, skeletonId), color, position);
    lb.Draw(spriteBatch, ToVector2(JointID.KneeRight, size, skeletonId), 
        ToVector2(JointID.AnkleRight, size, skeletonId), color, position);
    lb.Draw(spriteBatch, ToVector2(JointID.AnkleRight, size, skeletonId), 
        ToVector2(JointID.FootRight, size, skeletonId), color, position);
}
        #endregion

        #region Console
        void k_tilt(IList<string> args)
        {
            if (args.Count == 1) Console.WriteLine(Nui.NuiCamera.ElevationAngle.ToString());
            else
            {
                int d = int.Parse(args[1]);
                if (Math.Abs(d) > 27) Console.WriteLine("Invalid angle. must be between -27 and 27.");
                else Tilt(d);
            }
        }

        void k_init(IList<string> args)
        {
            Initialize();
        }

        void k_uninit(IList<string> args)
        {
            Uninitialize();
        }

        void k_video(IList<string> args)
        {
            if (args.Count == 1)
            {
                Console.WriteLine(ReceiveVideo.ToString());
            }
            else if (args[1].ToLower() == "open") OpenVideoStream();
            else ReceiveVideo = bool.Parse(args[1]);
        }

        void k_depth(IList<string> args)
        {
            if (args.Count == 1)
            {
                Console.WriteLine(ReceiveVideo.ToString());
            }
            else if (args[1].ToLower() == "open") OpenDepthStream();
            else ReceiveVideo = bool.Parse(args[1]);
        }
        #endregion
    }
}
#endif