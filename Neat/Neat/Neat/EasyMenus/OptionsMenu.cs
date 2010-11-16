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

         void OpenOptionsMenu()
        {
            gameWidth2 = game.gameWidth;
            gameHeight2 = game.gameHeight;
            fullscreen2 = game.fullscreen;
            system.items[0].caption = ("Fullscreen: " + (fullscreen2 ? "ON" : "OFF"));
            system.items[1].caption = ("Sounds: " + (game.muteAllSounds ? "OFF" : "ON"));
        }

         public override void Activate()
         {
             OpenOptionsMenu();
             base.Activate();
         }

        public override void CreateMenu()
        {
            gameWidth2 = game.gameWidth;
            gameHeight2 = game.gameHeight;
            fullscreen2 = game.fullscreen;
            system.AddItem("Fullscreen: " + (fullscreen2 ? "ON" : "OFF"));
#if !WINDOWS
            system.GetLastMenuItem().enabled = false;
#endif
            system.AddItem("Sounds: " + (game.muteAllSounds ? "OFF" : "ON"));
            system.AddItem("Resolution: 1024x1024", false);
            //system.AddItem("Apply Settings", false);
            system.AddItem("Back");
            system.items[1].caption = ("Resolution: " + gameWidth2.ToString() + "x" + gameHeight2.ToString());
            system.itemsOffset = new Vector2(0, 100);
            Activate();
            base.CreateMenu();
        }

        public override void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("menuFont");
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
            switch (system.selectedItem)
            {
                case 0:
                    fullscreen2 = !fullscreen2;
                    system.items[0].caption = ("Fullscreen: " + (fullscreen2 ? "ON  " : "OFF  "));
#if WINDOWS
                    game.ToggleFullScreen();
#endif
                    //optionsMenu.recalculateSizes();
                    break;
                case 1:
                    game.muteAllSounds = !game.muteAllSounds;
                    system.items[1].caption=("Sounds: " + (game.muteAllSounds ? "OFF" : "ON"));
                    break;
                case 100000:
                    if (gameWidth2 == 1024)
                    {
                        gameWidth2 = 1280; gameHeight2 = 960;
                    }
                    else if (gameWidth2 == 1280 && gameHeight2 == 960)
                    {
                        gameWidth2 = 1280; gameHeight2 = 1024;
                    }
                    else if (gameWidth2 == 1280 && gameHeight2 == 1024)
                    {
                        gameWidth2 = 1024; gameHeight2 = 768;
                    }
                    system.items[1].caption = ("Resolution: " + gameWidth2.ToString() + "x" + gameHeight2.ToString());
                    //optionsMenu.recalculateSizes();
                    break;
                case 2:
#if WINDOWS
                    game.gameWidth = gameWidth2;
                    game.gameHeight = gameHeight2;
                    game.fullscreen = fullscreen2;
                    game.InitializeGraphics();
#endif
                    break;
                case 3:
                    game.ActivatePart("mainmenu");
                    break;
            }
        }

        public override void Render(GameTime gameTime)
        {
            game.spriteBatch.Draw(game.getTexture("menuBackground"), new Rectangle(0, 0, game.gameWidth, game.gameHeight), Color.White);
            game.spriteBatch.Draw(game.getTexture("transparent"), new Rectangle(0, 0, game.gameWidth, game.gameHeight), Color.Black);

            base.Render(gameTime);
        }
    }
}
