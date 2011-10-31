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

namespace Neat.GUI
{
    public class Label : Control 
    {
        public override void Initialize()
        {
            SetColor(Color.White);
            Caption = "Label";
            Selectable = false;
            PushSound = "mute";
            base.Initialize();
        }

        public override void CaptionChanged(string newCaption)
        {
            base.CaptionChanged(newCaption);
            Size = Game.GetFont(Font).MeasureString(newCaption);
        }

        public override void FontChanged(string newFont)
        {
            base.FontChanged(newFont);
            Caption = Caption.ToString();
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
            _position.X =
                Game.Window.ClientBounds.Width / 2 -
                Game.GetFont(Font).MeasureString(Caption).X / 2;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (DrawShadow)
                GraphicsHelper.DrawShadowedString(spriteBatch, Game.GetFont(Font), Caption, Position,
                    (IsMouseHold ? MouseHoldColor :
                    (IsMouseHovered ? MouseHoverColor :
                    ForeColor)),
                    ShadowColor);
            else
                spriteBatch.DrawString(Game.GetFont(Font), Caption, Position, (IsMouseHold ? MouseHoldColor :
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

        public override void AttachToConsole()
        {
            base.AttachToConsole();
            Game.Console.AddCommand("fc_center", fc_center);
            Game.Console.AddCommand("fc_color", fc_color);
            Game.Console.AddCommand("fc_drawshadow", fc_drawshadow);
            Game.Console.AddCommand("fc_shadowcolor", fc_shadowcolor);
        }

        void fc_center(IList<string> args)
        {
            Center();
        }

        void fc_color(IList<string> args)
        {
            if (args.Count < 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            SetColor(Game.Console.ParseColor(Game.Console.Args2Str(args, 1)));
        }

        void fc_drawshadow(IList<string> args)
        {
            if (args.Count != 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            DrawShadow = bool.Parse(args[1]);
        }

        void fc_shadowcolor(IList<string> args)
        {
            if (args.Count < 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            ShadowColor = Game.Console.ParseColor(Game.Console.Args2Str(args, 1));
        }
    }
}
