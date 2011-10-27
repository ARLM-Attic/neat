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

namespace Neat.GUI
{
    public class Image:Control 
    {
        public override void Initialize()
        {
            Selectable = false;
            SetColor(Color.White);
            PushSound = "mute";
            base.Initialize();
        }

        public bool AutoSize = true;
        public void SetColor(Color color)
        {
            ForeColor = color;
            MouseHoldColor = color;
            MouseHoverColor = color;
        }

        public void CenterX()
        {
            _position.X = Game.GameWidth / 2 - Game.GetTexture(BackgroundImage).Width / 2;
        }
        public void CenterY()
        {
            _position.Y = Game.GameHeight / 2 - Game.GetTexture(BackgroundImage).Height / 2;
        }
        public void Center()
        {
            CenterX();
            CenterY();
        }
        public void StretchToScreen()
        {
            Position = Vector2.Zero;
            Size = new Vector2(((float)(Game.GameWidth)), ((float)(Game.GameHeight)));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (AutoSize)
            {
                spriteBatch.Draw(Game.GetTexture(BackgroundImage),
                    Position,
                    TintColor);
            }
            else
            {
                spriteBatch.Draw(Game.GetTexture(BackgroundImage),
                    new Rectangle((int)(Position.X), (int)(Position.Y), (int)(Size.X), (int)(Size.Y)),
                    TintColor);
            }
        }

        public override void AttachToConsole()
        {
            base.AttachToConsole();
            Game.Console.AddCommand("fc_autosize", fc_autosize);
            Game.Console.AddCommand("fc_color", fc_color);
            Game.Console.AddCommand("fc_center", fc_center);
            Game.Console.AddCommand("fc_stretch", fc_stretch);
        }

        void fc_autosize(IList<string> args)
        {
            if (args.Count != 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            AutoSize = bool.Parse(args[1]);
        }

        void fc_color(IList<string> args)
        {
            if (args.Count < 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            SetColor( Game.Console.ParseColor(Game.Console.Args2Str(args, 1)));
        }

        void fc_center(IList<string> args)
        {
            if (args.Count == 1)
            {
                Center();
                return;
            }
            else if (args[1].ToLower()[0] == 'x')
            {
                CenterX();
                return;
            }
            else if (args[1].ToLower()[0] == 'y')
            {
                CenterY();
                return;
            }
            else
            {
                Game.Console.WriteLine("syntax: " + args[0] + " {x|y}");
                return;
            }
        }

        void fc_stretch(IList<string> args)
        {
            StretchToScreen();
            return;
        }
    }
}
