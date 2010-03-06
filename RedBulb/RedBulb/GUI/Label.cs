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
    public class Label : FormObject 
    {
        public override void Initialize()
        {
            SetColor(Color.White);
            caption = "Label";
            selectable = false;
            pushSound = "mute";
            base.Initialize();
        }

        public Color shadowColor = Color.Black;
        public void SetColor(Color color)
        {
            foreColor = color;
            mouseHoldColor = color;
            mouseHoverColor = color;
        }

        public void Center()
        {
            position.X =
                game.Window.ClientBounds.Width / 2 -
                game.GetFont(font).MeasureString(caption).X / 2;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.DrawShadowedString(game.GetFont(font), caption, position,
                (isMouseHold ? mouseHoldColor :
                (isMouseHovered ? mouseHoverColor :
                foreColor)),
                shadowColor);

        }

        public TypeWriter ToTypeWriter()
        {
            return ((TypeWriter)this);
        }

        public FancyLabel ToFancyLabel()
        {
            return ((FancyLabel)this);
        }
    }
}
