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

namespace Neat.GUI
{
    public class Image:FormObject 
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
            Position.X = game.GameWidth / 2 - game.getTexture(BackgroundImage).Width / 2;
        }
        public void CenterY()
        {
            Position.Y = game.GameHeight / 2 - game.getTexture(BackgroundImage).Height / 2;
        }
        public void Center()
        {
            CenterX();
            CenterY();
        }
        public void StretchToScreen()
        {
            Position = Vector2.Zero;
            Size = new Vector2(((float)(game.GameWidth)), ((float)(game.GameHeight)));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (AutoSize)
            {
                spriteBatch.Draw(game.getTexture(BackgroundImage),
                    Position,
                    TintColor);
            }
            else
            {
                spriteBatch.Draw(game.getTexture(BackgroundImage),
                    new Rectangle((int)(Position.X), (int)(Position.Y), (int)(Size.X), (int)(Size.Y)),
                    TintColor);
            }
        }

        public override void AttachToConsole()
        {
            base.AttachToConsole();
            game.Console.AddCommand("fo_autosize", fo_autosize);
            game.Console.AddCommand("fo_color", fo_color);
            game.Console.AddCommand("fo_center", fo_center);
            game.Console.AddCommand("fo_stretch", fo_stretch);
        }

        void fo_autosize(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            AutoSize = bool.Parse(args[1]);
        }

        void fo_color(IList<string> args)
        {
            if (args.Count < 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            SetColor( game.Console.ParseColor(game.Console.Args2Str(args, 1)));
        }

        void fo_center(IList<string> args)
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
                game.Console.WriteLine("syntax: " + args[0] + " {x|y}");
                return;
            }
        }

        void fo_stretch(IList<string> args)
        {
            StretchToScreen();
            return;
        }
    }
}
