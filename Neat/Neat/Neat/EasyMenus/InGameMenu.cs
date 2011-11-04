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
            System.AddItem("Return to Game", true, null, "sh game");
            System.AddItem("Restart the Game", true, null, "g_start");
            System.AddItem("", false);
            System.AddItem("Return to Main Menu", true, null, "sh_mainmenu");
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
            if (game.IsTapped(Keys.Escape) || game.IsTapped(Buttons.Back))
                game.Console.Run("sh game");
            base.HandleInput(gameTime);
        }
    }
}
