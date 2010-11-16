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
            System.AddItem("Return to Game");
            System.AddItem("Restart the Game");
            System.AddItem("", false);
            System.AddItem("Return to Main Menu");
            //system.AddItem("Exit to Windows");
            System.GetLastMenuItem().Forecolor = Color.PaleVioletRed;
            System.Enable();
            System.ItemsOffset = new Vector2(0, 100);
            Activate();
            base.CreateMenu();
        }

        public override void LoadContent()
        {
            game.LoadFont("menufont");
            Font = game.GetFont("menufont");
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
                game.Console.Run("ap game");
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
                    game.Console.Run("ap game");
                    break;
                case 1:
                    game.Console.Run("g_restart");
                    game.Console.Run("ap game");
                    break;
                case 2:
                    // Seperator
                    break;
                case 3:
                    game.Console.Run("ap mainmenu");
                    break;
                case 4:
                    game.Console.Run("quit");
                    break;
            }
        }

        public override void Render(GameTime gameTime)
        {
            game.SpriteBatch.Draw(game.getTexture("transparent"), new Rectangle(0, 0, game.GameWidth, game.GameHeight), Color.Black); 
            base.Render(gameTime);
        }
    }
}
