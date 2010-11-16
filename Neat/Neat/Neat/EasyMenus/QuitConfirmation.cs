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
    class QuitConfirmationMenu : Menu
    {
        public QuitConfirmationMenu(NeatGame G)
            : base(G)
        {
        }

        public override void CreateMenu()
        {
            system.AddItem("Do you really want to quit?", false);
            
            system.AddItem("No! i wanna play a bit more");
            system.GetLastMenuItem().forecolor = Color.Green;
            system.AddItem("Yes. lemme out!");
                       
            system.GetLastMenuItem().forecolor = Color.Red;
            system.disabledItemForeground = Color.White;
            system.itemsOffset = new Vector2(0, 100);
            system.selectedItem = 1;
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
            //if (game.IsTapped(Keys.Escape)) game.Exit();
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
                case 1: //back
                    game.ActivatePart("mainmenu");
                    break;
                case 2: //quit
                    game.Exit();
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
