using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
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
            fullscreen2 = game.FullScreen;
            System.Items[0].Caption = ("Fullscreen: " + (fullscreen2 ? "ON" : "OFF"));
            System.Items[1].Caption = ("Sounds: " + (game.muteAllSounds ? "OFF" : "ON"));

            resolutions = new List<Point>();
            resolutions.Add(new Point(800, 600));
            resolutions.Add(new Point(1024, 768));
            resolutions.Add(new Point(1280, 720));
            resolutions.Add(new Point(1280, 800));
            resolutions.Add(new Point(1280, 1024));
            resolutions.Add(new Point(1920, 1080));
            resolutions.Add(new Point(1920, 1200));

            Point currentRes = new Point(game.GameWidth, game.GameHeight);
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
            fullscreen2 = game.FullScreen;
            System.AddItem("Fullscreen: " + (fullscreen2 ? "ON" : "OFF"));
#if !WINDOWS
            System.GetLastMenuItem().Enabled = false;
#endif
            System.AddItem("Sounds: " + (game.muteAllSounds ? "OFF" : "ON"));
            System.AddItem("Resolution: ", true);
            resolutionItem = System.GetLastMenuItem();
            resolutionItem.Caption += game.GameWidth.ToString() + "x" + game.GameHeight.ToString();
            System.AddItem("Apply Settings", true);
            System.AddItem("Back");
#if !WINDOWS
            resolutionItem.Enabled = false;
#endif
            System.ItemsOffset = new Vector2(0, 100);
            Activate();
            base.CreateMenu();
        }

        public override void LoadContent()
        {
            Font = game.Content.Load<SpriteFont>("menuFont");
            base.LoadContent();
        }

        public override void HandleInput(GameTime gameTime)
        {
#if ZUNE
            if (game.IsTapped(Buttons.Back))
#elif WINDOWS
            if (game.IsTapped(Keys.Escape))
#endif
                game.ActivatePart("mainmenu");

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
                    game.muteAllSounds = !game.muteAllSounds;
                    System.Items[1].Caption=("Sounds: " + (game.muteAllSounds ? "OFF" : "ON"));
                    break;
                case 2:
                    selectedResolution = (selectedResolution + 1) % resolutions.Count;
                    resolutionItem.Caption = ("Resolution: " + resolutions[selectedResolution].X.ToString() + "x" + resolutions[selectedResolution].Y.ToString());
                    //optionsMenu.recalculateSizes();
                    break;
                case 3:
#if WINDOWS
                    game.GameWidth = resolutions[selectedResolution].X;
                    game.GameHeight = resolutions[selectedResolution].Y;
                    game.FullScreen = fullscreen2;
                    game.InitializeGraphics();
                    Reset();
#endif
                    break;
                case 4:
                    game.Console.Run("ap mainmenu");
                    break;
            }
        }

        public override void Render(GameTime gameTime)
        {
            game.SpriteBatch.Draw(game.getTexture("menuBackground"), new Rectangle(0, 0, game.GameWidth, game.GameHeight), Color.White);
            game.SpriteBatch.Draw(game.getTexture("transparent"), new Rectangle(0, 0, game.GameWidth, game.GameHeight), Color.Black);

            base.Render(gameTime);
        }
    }
}
