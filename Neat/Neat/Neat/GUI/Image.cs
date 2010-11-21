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
            Position.X = game.GameWidth / 2 - game.GetTexture(BackgroundImage).Width / 2;
        }
        public void CenterY()
        {
            Position.Y = game.GameHeight / 2 - game.GetTexture(BackgroundImage).Height / 2;
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
                spriteBatch.Draw(game.GetTexture(BackgroundImage),
                    Position,
                    TintColor);
            }
            else
            {
                spriteBatch.Draw(game.GetTexture(BackgroundImage),
                    new Rectangle((int)(Position.X), (int)(Position.Y), (int)(Size.X), (int)(Size.Y)),
                    TintColor);
            }
        }

        public override void AttachToConsole()
        {
            base.AttachToConsole();
            game.Console.AddCommand("fc_autosize", fc_autosize);
            game.Console.AddCommand("fc_color", fc_color);
            game.Console.AddCommand("fc_center", fc_center);
            game.Console.AddCommand("fc_stretch", fc_stretch);
        }

        void fc_autosize(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            AutoSize = bool.Parse(args[1]);
        }

        void fc_color(IList<string> args)
        {
            if (args.Count < 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            SetColor( game.Console.ParseColor(game.Console.Args2Str(args, 1)));
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
                game.Console.WriteLine("syntax: " + args[0] + " {x|y}");
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
