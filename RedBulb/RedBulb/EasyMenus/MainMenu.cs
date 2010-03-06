#region References
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
using RedBulb;
using RedBulb.MenuSystem;
//using UltimaScroll.IBLib;
#endregion

namespace RedBulb.EasyMenus
{
    class MainMenu : Menu 
    {
        public MainMenu(RedBulbGame G)
            : base(G)
        {
        }

        public override void CreateMenu()
        {
            system.AddItem("Play");
            system.AddItem("Options");
            system.AddItem("Quit");
            system.GetLastMenuItem().forecolor = Color.Red;
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
                game.ActivatePart("quitconfirm");
#if ZUNE
            if (game.IsTapped( Buttons.A))
#elif WINDOWS
            if (game.IsTapped(Keys.Enter))
#endif
            {
                switch (system.selectedItem)
                {
                    case 0: //game
                        game.ActivatePart("game" );
                        break;
                    case 1: //opt
                        game.ActivatePart("optionsmenu" );
                        break;
                    case 2: //quit
                        game.ActivatePart("quitconfirm");
                        break;
                }
            }
            base.HandleInput(gameTime);
        }

        public override void Render(GameTime gameTime)
        {
            game.spriteBatch.Draw(game.getTexture("menuBackground"), new Rectangle(0, 0, game.gameWidth, game.gameHeight), Color.White);
            base.Render(gameTime);
        } 
    }
}
