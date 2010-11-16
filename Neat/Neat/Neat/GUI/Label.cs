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
using Neat.Graphics;

namespace Neat.GUI
{
    public class Label : FormObject 
    {
        public override void Initialize()
        {
            SetColor(Color.White);
            Caption = "Label";
            Selectable = false;
            PushSound = "mute";
            base.Initialize();
        }

        public Color ShadowColor = Color.Black;
        public bool DrawShadow = true;
        public void SetColor(Color color)
        {
            ForeColor = color;
            MouseHoldColor = color;
            MouseHoverColor = color;
        }

        public void Center()
        {
            Position.X =
                game.Window.ClientBounds.Width / 2 -
                game.GetFont(Font).MeasureString(Caption).X / 2;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (DrawShadow)
                GraphicsHelper.DrawShadowedString(spriteBatch, game.GetFont(Font), Caption, Position,
                    (IsMouseHold ? MouseHoldColor :
                    (IsMouseHovered ? MouseHoverColor :
                    ForeColor)),
                    ShadowColor);
            else
                spriteBatch.DrawString(game.GetFont(Font), Caption, Position, (IsMouseHold ? MouseHoldColor :
                    (IsMouseHovered ? MouseHoverColor :
                    ForeColor)));
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
