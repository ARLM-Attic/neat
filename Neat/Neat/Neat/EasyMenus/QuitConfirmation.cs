﻿using System;
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
    public class QuitConfirmationMenu : Menu
    {
        public QuitConfirmationMenu(NeatGame G)
            : base(G)
        {
        }

        public override void CreateMenu()
        {
            System.AddItem("Do you really want to quit?", false);
            
            System.AddItem("No! i wanna play a bit more", true, null, "sh mainmenu");
            System.GetLastMenuItem().Forecolor = Color.Green;
            System.AddItem("Yes. lemme out!", true, null, "quit");
                       
            System.GetLastMenuItem().Forecolor = Color.Red;
            System.DisabledItemForeground = Color.White;
            System.ItemsOffset = new Vector2(0, 100);
            System.SelectedItem = 1;
            //Activate();
            base.CreateMenu();
        }

        public override void Render(GameTime gameTime)
        {
            game.SpriteBatch.Draw(game.GetTexture("menuBackground"), new Rectangle(0, 0, game.GameWidth, game.GameHeight), Color.White);
            game.SpriteBatch.Draw(game.GetTexture("transparent"), new Rectangle(0, 0, game.GameWidth, game.GameHeight), Color.Black);
            base.Render(gameTime);
        }
    }
}
