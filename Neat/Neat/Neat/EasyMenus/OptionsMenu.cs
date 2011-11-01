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
#if WINDOWS
 
 
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
        public OptionsMenu(NeatGame G)
            : base(G)
        {
        }


        int gameWidth2, gameHeight2;
        bool fullscreen2;
        MenuItem resolutionItem;
        List<Point> resolutions;
        int selectedResolution = 0;
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
            System.AddItem("Fullscreen: " + (fullscreen2 ? "ON" : "OFF"));
#if !WINDOWS
            System.GetLastMenuItem().Enabled = false;
#endif
            System.AddItem("Sounds: " + (game.MuteAllSounds ? "OFF" : "ON"));
            System.AddItem("Resolution: ", true);
            resolutionItem = System.GetLastMenuItem();
            resolutionItem.Caption += gameWidth2.ToString() + "x" + gameHeight2.ToString();
            System.AddItem("Apply Settings", true);
            System.AddItem("Back");
#if !WINDOWS
            resolutionItem.Enabled = false;
#endif
            System.ItemsOffset = new Vector2(0, 100);
            Activate();
            base.CreateMenu();
        }

        public override void HandleInput(GameTime gameTime)
        {
#if ZUNE
            if (game.IsTapped(Buttons.Back))
#elif WINDOWS
            if (game.IsTapped(Keys.Escape))
#endif
                game.Console.Run("sh mainmenu");

#if ZUNE
            if (game.IsTapped(Buttons.A))
#elif WINDOWS
            if (game.IsTapped(Keys.Enter))
#endif
            {
                SelectItem();
            }
            base.HandleInput(gameTime);
        }

        void SelectItem()
        {
            switch (System.SelectedItem)
            {
                case 0:
                    fullscreen2 = !fullscreen2;
                    System.Items[0].Caption = ("Fullscreen: " + (fullscreen2 ? "ON  " : "OFF  "));
#if WINDOWS
                    //game.ToggleFullScreen();
#endif
                    //optionsMenu.recalculateSizes();
                    break;
                case 1:
                    game.MuteAllSounds = !game.MuteAllSounds;
                    System.Items[1].Caption=("Sounds: " + (game.MuteAllSounds ? "OFF" : "ON"));
                    break;
                case 2:
                    selectedResolution = (selectedResolution + 1) % resolutions.Count;
                    resolutionItem.Caption = ("Resolution: " + resolutions[selectedResolution].X.ToString() + "x" + resolutions[selectedResolution].Y.ToString());
                    //optionsMenu.recalculateSizes();
                    break;
                case 3:
#if WINDOWS
                    StreamWriter sw = new StreamWriter("options.nsc");
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
#endif
                    break;
                case 4:
                    game.Console.Run("sh mainmenu");
                    break;
            }
        }

        public override void Render(GameTime gameTime)
        {
            game.SpriteBatch.Draw(game.GetTexture("menuBackground"), new Rectangle(0, 0, game.GameWidth, game.GameHeight), Color.White);
            game.SpriteBatch.Draw(game.GetTexture("transparent"), new Rectangle(0, 0, game.GameWidth, game.GameHeight), Color.Black);

            base.Render(gameTime);
        }
    }
}
