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
using Microsoft.Xna.Framework.Media;
using RedBulb;
using RedBulb.MenuSystem;
//using UltimaScroll.IBLib;
#endregion

namespace RedBulb.GUI
{
    public class TypeWriter : Label 
    {

        int frame = 0;
        public int speed = 7;
        int cursor = 0;

        public void Reset()
        {
            cursor = 0;
            frame = 0;
        }


    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        frame++;
        if (frame % speed == 0) cursor++;
        if (cursor >= caption.Length) cursor = caption.Length;

    }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.DrawShadowedString(game.GetFont(font), caption.Substring(0,cursor), position,
                (isMouseHold ? mouseHoldColor :
                (isMouseHovered ? mouseHoverColor :
                foreColor)),
                shadowColor);

        }
    }
}
