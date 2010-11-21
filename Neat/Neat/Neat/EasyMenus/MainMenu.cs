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

namespace Neat.EasyMenus
{
    class MainMenu : Menu 
    {
        public MainMenu(NeatGame G)
            : base(G)
        {
        }

        public override void CreateMenu()
        {
            System.AddItem("Play");
            System.AddItem("Options");
            System.AddItem("Quit");
            System.GetLastMenuItem().Forecolor = Color.Red;
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
                game.ActivateScreen("quitconfirm");
#if ZUNE
            if (game.IsTapped( Buttons.A))
#elif WINDOWS
            if (game.IsTapped(Keys.Enter))
#endif
            {
                switch (System.SelectedItem)
                {
                    case 0: //game
                        game.Console.Run("g_start");
                        break;
                    case 1: //opt
                        game.Console.Run("sh optionsmenu");
                        break;
                    case 2: //quit
                        game.Console.Run("sh quitconfirm");
                        break;
                }
            }
            base.HandleInput(gameTime);
        }

        public override void Render(GameTime gameTime)
        {
            game.SpriteBatch.Draw(game.GetTexture("menuBackground"), new Rectangle(0, 0, game.GameWidth, game.GameHeight), Color.White);
            base.Render(gameTime);
        } 
    }
}
