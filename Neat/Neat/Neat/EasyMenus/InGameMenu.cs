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
    class InGameMenu : Menu
    {
        public InGameMenu(NeatGame G)
            : base(G)
        {
        }

        public override void CreateMenu()
        {
            system.AddItem("Return to Game");
            system.AddItem("Restart the Game");
            system.AddItem("", false);
            system.AddItem("Return to Main Menu");
            //system.AddItem("Exit to Windows");
            system.GetLastMenuItem().forecolor = Color.PaleVioletRed;
            system.Enable();
            system.itemsOffset = new Vector2(0, 100);
            Activate();
            base.CreateMenu();
        }

        public override void LoadContent()
        {
            game.LoadFont("menufont");
            font = game.GetFont("menufont");
            //font = game.Content.Load<SpriteFont>("menuFont");
            base.LoadContent();
        }

        public override void HandleInput(GameTime gameTime)
        {
#if ZUNE
            if (game.IsTapped(Buttons.Back )) 
#elif WINDOWS
            if (game.IsTapped(Keys.Escape))
#endif
                game.ActivatePart("game");
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
                    game.ActivatePart("game");
                    break;
                case 1:
                    game.RestartGame();
                    game.ActivatePart("game");
                    break;
                case 2:
                    // Seperator
                    break;
                case 3:
                    game.ActivatePart("mainmenu" );
                    break;
                case 4:
                    game.Exit();
                    break;
            }
        }

        public override void Render(GameTime gameTime)
        {
            game.spriteBatch.Draw(game.getTexture("transparent"), new Rectangle(0, 0, game.gameWidth, game.gameHeight), Color.Black); 
            base.Render(gameTime);
        }
    }
}
