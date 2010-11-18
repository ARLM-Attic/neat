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
    public class TypeWriter : Label
    {

        int frame = 0;
        public int Speed = 7;
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
            if (frame % Speed == 0) cursor++;
            if (cursor >= Caption.Length) cursor = Caption.Length;

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (DrawShadow)
                GraphicsHelper.DrawShadowedString(spriteBatch, game.GetFont(Font), Caption.Substring(0, cursor), Position,
                    (IsMouseHold ? MouseHoldColor :
                    (IsMouseHovered ? MouseHoverColor :
                    ForeColor)),
                    ShadowColor);
            else
            {
                spriteBatch.DrawString(game.GetFont(Font), Caption.Substring(0, cursor), Position, (IsMouseHold ? MouseHoldColor :
                    (IsMouseHovered ? MouseHoverColor :
                    ForeColor)));
            }
        }

        public override void AttachToConsole()
        {
            base.AttachToConsole();
            game.Console.AddCommand("fc_speed", fc_speed);
        }

        void fc_speed(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [int]");
                return;
            }
            Speed = int.Parse(args[1]);
        }
    }
}
