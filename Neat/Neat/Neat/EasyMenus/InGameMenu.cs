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
using Neat.Graphics;

namespace Neat.EasyMenus
{
    public class InGameMenu : Menu
    {
        public InGameMenu(NeatGame G)
            : base(G)
        {
        }

        public override void CreateMenu()
        {
            ShadeColor = GraphicsHelper.GetColorWithAlpha(Color.Black, 0.4f);
            System.AddItem("Return to Game");
            System.AddItem("Restart the Game");
            System.AddItem("", false);
            System.AddItem("Return to Main Menu");
            //system.AddItem("Exit to Windows");
            System.GetLastMenuItem().Forecolor = Color.PaleVioletRed;
            System.Enable();
            System.ItemsOffset = new Vector2(0, 100);
            Activate();
            game.Console.AddCommand("m_gamescreen", m_gamescreen);
            base.CreateMenu();
        }

        void m_gamescreen(IList<string> args)
        {
            BackgroundScreen = args[1];
        }

        public override void HandleInput(GameTime gameTime)
        {
#if ZUNE
            if (game.IsTapped(Buttons.Back )) 
#elif WINDOWS
            if (game.IsTapped(Keys.Escape))
#endif
                game.Console.Run("sh game");
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
                    game.Console.Run("sh game");
                    break;
                case 1:
                    game.Console.Run("g_start");
                    break;
                case 2:
                    // Seperator
                    break;
                case 3:
                    game.Console.Run("sh mainmenu");
                    break;
                case 4:
                    game.Console.Run("quit");
                    break;
            }
        }

    }
}
