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
using Microsoft.Research.Kinect;
using Neat.Components;
using Neat;
using Neat.Mathematics;
using Microsoft.Research.Kinect.Nui;
using System.Diagnostics;

namespace Neat.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class KinectEngine : Microsoft.Xna.Framework.GameComponent
    {
        Console Console;
        Runtime nui;
        public bool ReceiveVideo = false;
        public Texture2D KinectRGB;
        int xMax = 640;
        int yMax = 480;
        public SkeletonData[] Skeletons = new SkeletonData[2];
        int trackedSkeletonsCount = 0;
        public int TrackedSkeletonsCount { get { return trackedSkeletonsCount; } }

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
            // TODO: Add your initialization code here
            Console.AddCommand("k_tilt", k_tilt);
            Console.AddCommand("k_init", k_init);
            Console.AddCommand("k_uninit", k_uninit);
            Console.AddCommand("k_video", k_video);

            try
            {
                nui = Runtime.Kinects[0];
                nui.Initialize(RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
            }
            catch
            {
                Debug.WriteLine("Error while initializing kinect. Aborting.");
                throw;
            }
            NeatGame game = ((NeatGame)this.Game);
            nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);
            nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            KinectRGB = new Texture2D(game.GraphicsDevice, xMax, yMax);
            game.AssignTexture(new Sprite(KinectRGB), "kinectrgb");
            Console.WriteLine("Kinect initialized successfully!");

            base.Initialize();
        }

        public void OpenVideoStream()
        {
            nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            ReceiveVideo = true;
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            var trackedSkeletons = from s in e.SkeletonFrame.Skeletons
                                   where
                                       s.TrackingState == SkeletonTrackingState.Tracked
                                   select s;
            trackedSkeletonsCount = trackedSkeletons.Count();
            for (int i = 0; i < trackedSkeletonsCount; i++)
            {
                Skeletons[i] = trackedSkeletons.ElementAt(i);
            }
        }

        void nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            if (!ReceiveVideo) return;
            PlanarImage image = e.ImageFrame.Image;

            int offset = 0;
            Color[] bitmap = new Color[xMax * yMax];
            for (int y = 0; y < yMax; y++)
            {
                for (int x = 0; x < xMax; x++)
                {
                    bitmap[y * xMax + x] = new Color(image.Bits[offset + 2], image.Bits[offset + 1], image.Bits[offset], 255);
                    offset += 4;
                }
            }
            KinectRGB.SetData(bitmap);
        }

        public void Uninitialize()
        {
            try
            {
                nui.Uninitialize();
                Debug.WriteLine("Kinect uninitialized.");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to uninitialize kinect!");
                Debug.WriteLine(e.Message);
            }
        }

        public void Tilt(int degrees)
        {
            nui.NuiCamera.ElevationAngle = degrees;
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

        #region Console
        void k_tilt(IList<string> args)
        {
            if (args.Count == 1) Console.WriteLine(nui.NuiCamera.ElevationAngle.ToString());
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
        #endregion
    }
}
#endif