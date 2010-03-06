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
    public class FancyLabel : Label 
    {
        public int speed = 10;
        int cursor = 0;
        float alpha = 0f;
        string lastText="";
        string Text="";
     
        public void Reset()
        {
            cursor = 0;
            alpha = 0;
        }

        public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        alpha += speed * 0.01f;
        if (alpha >= 1) { alpha = 0f; cursor++; lastText = Text; }
        if (cursor >= caption.Length) cursor = caption.Length;
        Text = caption.Substring(0, cursor);
    }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.DrawShadowedString(game.GetFont(font), Text, position,
                (isMouseHold ? mouseHoldColor :
                (isMouseHovered ? mouseHoverColor :
                game.GetColorWithAlpha(foreColor, alpha))),
                game.GetColorWithAlpha(shadowColor, alpha));

            game.DrawShadowedString(game.GetFont(font), lastText, position,
                (isMouseHold ? mouseHoldColor :
                (isMouseHovered ? mouseHoverColor :
                foreColor)),
                shadowColor);


        }
    }
}
