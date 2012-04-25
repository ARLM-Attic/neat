using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
using Neat.EasyMenus;
using System.IO;

namespace Neat.EasyMenus
{
    public class OptionsMenu : Menu
    {
        int gameWidth2, gameHeight2;
        bool fullscreen2;
        MenuItem resolutionItem;
        List<Point> resolutions;
        int selectedResolution = 0;

        public OptionsMenu(NeatGame G)
            : base(G)
        {
        }

        public override void Activate()
        {
            OpenOptionsMenu();
            base.Activate();
        }

        public override void CreateMenu()
        {
            gameWidth2 = game.GameWidth;
            gameHeight2 = game.GameHeight;

            if (game.StretchMode != NeatGame.StretchModes.None)
            {
                gameWidth2 = game.OutputResolution.X;
                gameHeight2 = game.OutputResolution.Y;
            }
            fullscreen2 = game.FullScreen;

            var fullscreenItem = System.AddItem("Fullscreen: " + (fullscreen2 ? "ON" : "OFF"));
            fullscreenItem.OnSelect = o =>
            {
                fullscreen2 = !fullscreen2;
                o.Caption = ("Fullscreen: " + (fullscreen2 ? "ON  " : "OFF  "));
            };
#if !WINDOWS
            fullscreenItem.Enabled = false;
#endif

            var soundsItem = System.AddItem("Sounds: " + (game.MuteAllSounds ? "OFF" : "ON"));
            soundsItem.OnSelect = o =>
            {
                game.MuteAllSounds = !game.MuteAllSounds;
                o.Caption = ("Sounds: " + (game.MuteAllSounds ? "OFF" : "ON"));
            };

            resolutionItem = System.AddItem("Resolution: ", true);
            resolutionItem.Caption += gameWidth2.ToString() + "x" + gameHeight2.ToString();
            resolutionItem.OnSelect = o =>
            {
                selectedResolution = (selectedResolution + 1) % resolutions.Count;
                resolutionItem.Caption = ("Resolution: " + resolutions[selectedResolution].X.ToString() + "x" + resolutions[selectedResolution].Y.ToString());
            };

            var applyItem = System.AddItem("Apply Settings", true);
            applyItem.OnSelect = o => ApplySettings();
            System.AddItem("Back", true, null, "sh mainmenu");
#if !WINDOWS
            resolutionItem.Enabled = false;
#endif

            System.ItemsOffset = new Vector2(0, 100);
            //Activate();
            base.CreateMenu();
        }

        public override void HandleInput(GameTime gameTime)
        {
            if (game.IsTapped(Keys.Escape) || game.IsTapped(Buttons.Back))
                game.Console.Run("sh mainmenu");

            base.HandleInput(gameTime);
        }

        void OpenOptionsMenu()
        {
            gameWidth2 = game.GameWidth;
            gameHeight2 = game.GameHeight;
            if (game.StretchMode != NeatGame.StretchModes.None)
            {
                gameWidth2 = game.OutputResolution.X;
                gameHeight2 = game.OutputResolution.Y;
            }
            fullscreen2 = game.FullScreen;
            System.Items[0].Caption = ("Fullscreen: " + (fullscreen2 ? "ON" : "OFF"));
            System.Items[1].Caption = ("Sounds: " + (game.MuteAllSounds ? "OFF" : "ON"));

            resolutions = new List<Point>();
            resolutions.Add(new Point(800, 600));
            resolutions.Add(new Point(1024, 600));
            resolutions.Add(new Point(1024, 768));
            resolutions.Add(new Point(1280, 720));
            resolutions.Add(new Point(1280, 800));
            resolutions.Add(new Point(1280, 1024));
            resolutions.Add(new Point(1920, 1080));
            resolutions.Add(new Point(1920, 1200));

            Point currentRes = new Point(gameWidth2, gameHeight2);
            bool found = false;
            for (int i = 0; i < resolutions.Count; i++)
            {
                var item = resolutions[i];
                if (item == currentRes)
                {
                    found = true;
                    selectedResolution = i;
                    break;
                }
            }
            if (!found)
            {
                resolutions.Add(currentRes);
                selectedResolution = resolutions.Count - 1;
            }
        }

        public Action<StreamWriter> OptionalSave = null;
        void ApplySettings()
        {
            StreamWriter sw = new StreamWriter("options.nsc");

            if (OptionalSave != null) OptionalSave(sw);

            sw.WriteLine("g_res " + resolutions[selectedResolution].X.ToString() + " " + resolutions[selectedResolution].Y.ToString());
            sw.WriteLine("g_fullscreen " + fullscreen2.ToString());
            sw.WriteLine("a_mutesounds " + game.MuteAllSounds.ToString());
            sw.WriteLine("g_reinit");

            sw.Close();
            if (game.StretchMode == NeatGame.StretchModes.None)
            {
                game.GameWidth = resolutions[selectedResolution].X;
                game.GameHeight = resolutions[selectedResolution].Y;
            }
            else
            {
                game.OutputResolution = resolutions[selectedResolution];
            }
            game.FullScreen = fullscreen2;
            game.InitializeGraphics();
            Reset();
            game.Console.Run("sh optionsmenu");
        }

        public override void Render(GameTime gameTime)
        {
            game.SpriteBatch.Draw(game.GetTexture("menuBackground"), new Rectangle(0, 0, game.GameWidth, game.GameHeight), Color.White);
            game.SpriteBatch.Draw(game.GetTexture("transparent"), new Rectangle(0, 0, game.GameWidth, game.GameHeight), Color.Black);

            base.Render(gameTime);
        }
    }
}
