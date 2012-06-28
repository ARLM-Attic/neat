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
using Microsoft.Kinect;
using System.Diagnostics;
using Console = Neat.Components.Console;

namespace Neat
{
    public partial class CalibrateScreen : Screen
    {
        KinectEngine Kinect { get { return game.Kinect; } }
        Label tiltLabel;
        Image kinectImage;
        LineBrush lb;

        Button tiltUp, tiltDown;
        Button seated, standing;
        Button back;

        Trackbar trackBar1;

        public static bool ShowModeButtons = true;

        public CalibrateScreen(NeatGame game)
            : base(game)
        {
            
        }

        bool oldTouch, oldDraw;
        public override void Activate()
        {
            if (game.Kinect.ColorStream == null) game.Kinect.OpenColorStream();

            oldTouch = game.Touch.Enabled;
            game.Touch.Enabled = true;

            oldDraw = game.Kinect.Draw;
            game.Kinect.Draw = true;

            base.Activate();
        }

        public override void Deactivate(string nextScreen)
        {
            base.Deactivate(nextScreen);

            game.Touch.Enabled = oldTouch;
            game.Kinect.Draw = oldDraw;
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
            tiltLabel.Visible = false;

            kinectImage = Form.NewControl("kinectimage", new Image()).ToImage();
            kinectImage.BackgroundImage = "kinectcolor";
            kinectImage.Size = new Vector2(320,240) * 1.5f;
            kinectImage.AutoSize = false;
            game.Console.AddCommand("kc_showdepth", o => kinectImage.BackgroundImage = bool.Parse(o[1]) ? "kinectdepth" : "kinectcolor");
            
            Form.NewControl("tiltup", tiltUp = new Button());
            tiltUp.BackgroundImage = "tilt_up";
            tiltUp.Size = new Vector2(64);
            tiltUp.Caption = "";
            tiltUp.OnPress = () =>
                {
                    try
                    {
                        Kinect.Sensor.ElevationAngle += 5;
                    }
                    catch
                    {
                    }
                    //game.Touch.Reset(new TimeSpan(0, 0, 0, 2));
                };

            Form.NewControl("tiltdown", tiltDown = new Button());
            tiltDown.BackgroundImage = "tilt_down";
            tiltDown.Size = new Vector2(64);
            tiltDown.Caption = "";
            tiltDown.OnPress = () => 
                {
                    try
                    {
                        Kinect.Sensor.ElevationAngle -= 5;
                    }
                    catch { }
                    //game.Touch.Reset(new TimeSpan(0, 0, 0, 2));
                };

            Form.NewControl("seated", seated = new Button());
            seated.BackgroundImage = "kinect_seated";
            seated.Size = new Vector2(64);
            seated.Caption = "";
            seated.OnPress = () => Kinect.SeatedMode = true;

            Form.NewControl("standing", standing = new Button());
            standing.BackgroundImage = "kinect_standing";
            standing.Size = new Vector2(64);
            standing.Caption = "";
            standing.OnPress = () => Kinect.SeatedMode = false;

            Form.NewControl("back", back = new Button());
            back.Caption = "Back";
            back.Size = new Vector2(64 + 64 + 10, 64);
            back.OnPress = () => game.ActivateScreen(game.PreviousScreen);
            back.TintColor = new Color(42, 171, 225);

            Form.NewControl("trackbar1", trackBar1 = new Trackbar());
            trackBar1.Size = new Vector2(500, 32);
            trackBar1.Position = new Vector2(32);
            trackBar1.OnRelease = () => Debug.WriteLine(trackBar1.Percent);
            trackBar1.Visible = false;
        }
        /*
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
        }*/

        public override void Behave(GameTime gameTime)
        {
            if (Kinect == null) return; // this.Kinect = ((KineatGame)game).Kinect;
            tiltLabel.Caption = "Tilt: " + Kinect.Sensor.ElevationAngle;
            //kinectImage.Center();
            //skeletonImage.Center();

            if (ShowModeButtons && !standing.Visible) standing.Visible = seated.Visible = true;
            else if (!ShowModeButtons && standing.Visible) standing.Visible = seated.Visible = false;

            var gc = game.GameCenter;
            var kic = kinectImage.Size / 2.0f;
            const float margin = 16;

            kinectImage.Position = gc - kic;
            tiltUp.Position = new Vector2(kinectImage.Position.X, gc.Y - (kic.Y + tiltUp.Size.Y + margin));
            tiltDown.Position = new Vector2(kinectImage.Position.X, gc.Y + kic.Y + margin);
            seated.Position = new Vector2(kinectImage.Position.X + kinectImage.Size.X, tiltUp.Position.Y);
            standing.Position = seated.Position - new Vector2(standing.Size.X + 10, 0);
            back.Position = new Vector2(standing.Position.X, tiltDown.Position.Y);

            base.Behave(gameTime);
        }

        public override void Render(GameTime gameTime)
        {
            if (Kinect == null) return;
            base.Render(gameTime);
        }
    }
}
#endif