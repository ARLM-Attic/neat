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
    public class MainMenu : Menu 
    {
        public MainMenu(NeatGame G)
            : base(G)
        {
        }

        public override void CreateMenu()
        {
            System.AddItem("Play", true, null, "g_start");
            System.AddItem("Options", true, null, "sh optionsmenu");
            System.AddItem("Quit", true, null, "sh quitconfirm");
            System.GetLastMenuItem().Forecolor = Color.Red;
            System.ItemsOffset = new Vector2(0, 100);
            //Activate();
            base.CreateMenu();
        }

        public override void Render(GameTime gameTime)
        {
            game.SpriteBatch.Draw(game.GetTexture("menuBackground"), new Rectangle(0, 0, game.GameWidth, game.GameHeight), Color.White);
            base.Render(gameTime);
        }

        public void RenderBase(GameTime gameTime)
        {
            base.Render(gameTime);
        }
    }
}
