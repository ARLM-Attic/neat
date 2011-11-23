#if KINECT
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Neat;
using Neat.Mathematics;
using Neat.Graphics;
using Neat.GUI;
using Neat.Components;
using Microsoft.Research.Kinect.Nui;
namespace Neat
{
    public partial class CalibrateScreen : Screen
    {
        public static KinectEngine Kinect;
        Label tiltLabel;
        Image kinectImage;
        LineBrush lb;
        public CalibrateScreen(NeatGame game)
            : base(game)
        {
            
        }

        public override void LoadContent()
        {
            base.LoadContent();
            lb = new LineBrush(game.GraphicsDevice, 1);
        }

        public override void Initialize()
        {
            base.Initialize();
            tiltLabel = Form.NewControl("tiltlabel", new Label()).ToLabel();
            tiltLabel.Caption = "Tilt:";
            tiltLabel.Position = new Vector2(tiltLabel.Position.X, game.GameHeight / 2.0f - tiltLabel.Size.Y / 2.0f);

            kinectImage = Form.NewControl("kinectimage", new Image()).ToImage();
            kinectImage.BackgroundImage = "kinectrgb";

            game.Console.AddCommand("k_smooth", k_smooth);
        }

        void k_smooth(IList<string> args)
        {
            float value = float.Parse(args[2]);
            TransformSmoothParameters s = Kinect.Nui.SkeletonEngine.SmoothParameters;
            if (args[1][0] == 'c') s.Correction = value;
            else if (args[1][0] == 'j') s.JitterRadius = value;
            else if (args[1][0] == 'd') s.MaxDeviationRadius = value;
            else if (args[1][0] == 'p') s.Prediction = value;
            else if (args[1][0] == 's') s.Smoothing = value;
            else game.Console.WriteLine("Invalid value");
            Kinect.Nui.SkeletonEngine.SmoothParameters = s;
        }

        public override void Behave(GameTime gameTime)
        {
            if (Kinect == null) return; // this.Kinect = ((KineatGame)game).Kinect;
            tiltLabel.Caption = "Tilt: " + Kinect.Nui.NuiCamera.ElevationAngle;
            kinectImage.Center();
            
            base.Behave(gameTime);
        }

        public override void Render(GameTime gameTime)
        {
            if (Kinect == null) return;
            Vector2 offset = kinectImage.Position;
            Vector2 size = kinectImage.Size;

            base.Render(gameTime);

            game.Write(
                "Correction: " + Kinect.Nui.SkeletonEngine.SmoothParameters.Correction + "\n" +
                "Jitter Radius: " + Kinect.Nui.SkeletonEngine.SmoothParameters.JitterRadius + "\n" +
                "Max Deviation: " + Kinect.Nui.SkeletonEngine.SmoothParameters.MaxDeviationRadius + "\n" +
                "Prediction: " + Kinect.Nui.SkeletonEngine.SmoothParameters.Prediction + "\n" +
                "Smoothing: " + Kinect.Nui.SkeletonEngine.SmoothParameters.Smoothing, new Vector2(10, 100));
            if (Kinect.TrackedSkeletonsCount > 0)
            {
                var skeleton = Kinect.Skeletons[0];

                string predicted = "";
                string zs = "";
                for (JointID i = 0; i < JointID.Count; i++)
                {
                    var v = Kinect.ToVector3(i, new Vector3(640, 480, 256));
                    int r = 0;// (int)v.Z - ((int)(v.Z / 256) * 256);
                    int g = 0; // (int)(v.Z / 256) - ((int)(v.Z / 256) / 256) * 256;
                    int b = (int)(v.Z);// ((int)(v.Z / 256) / 256);
                    r = g = b;
                    var p = Polygon.BuildCircle(10, Kinect.ToVector2(i, new Vector2(v.X, v.Y)), 5);
                    //p.Draw(SpriteBatch, lb, offset+Vector2.One, Color.Black);
                    Color c = new Color(r, g, b);
                    p.Draw(SpriteBatch, lb, offset, skeleton.Joints[i].TrackingState == JointTrackingState.Inferred ? Color.Yellow : c);
                    if (skeleton.Joints[i].TrackingState != JointTrackingState.Tracked)
                        predicted += i.ToString() + "\n";
                    zs += i.ToString() + (int)v.Z + "\n";
                }

                game.Write(zs + "\n\n\n\n\n\n" + predicted, new Vector2(200, 100));
            }

            
        }
    }
}
#endif