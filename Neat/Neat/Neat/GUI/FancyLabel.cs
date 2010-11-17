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
    public class FancyLabel : Label 
    {
        public int Speed = 10;
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
            alpha += Speed * 0.01f;
            if (alpha >= 1) { alpha = 0f; cursor++; lastText = Text; }
            if (cursor >= Caption.Length) cursor = Caption.Length;
            Text = Caption.Substring(0, cursor);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (DrawShadow)
            {
                if (lastText != Caption)
                {
                    GraphicsHelper.DrawShadowedString(spriteBatch, game.GetFont(Font), Text, Position,
                        (IsMouseHold ? MouseHoldColor :
                        (IsMouseHovered ? MouseHoverColor :
                        GraphicsHelper.GetColorWithAlpha(ForeColor, alpha))),
                        GraphicsHelper.GetColorWithAlpha(ShadowColor, alpha));

                    GraphicsHelper.DrawShadowedString(spriteBatch, game.GetFont(Font), lastText, Position,
                        (IsMouseHold ? MouseHoldColor :
                        (IsMouseHovered ? MouseHoverColor :
                        ForeColor)),
                        ShadowColor);
                }
                else
                {
                    GraphicsHelper.DrawShadowedString(spriteBatch, game.GetFont(Font), Caption, Position,
                        (IsMouseHold ? MouseHoldColor :
                        (IsMouseHovered ? MouseHoverColor :
                        ForeColor)),
                        ShadowColor);
                }
            }
            else
            {
                if (lastText != Caption)
                {
                    spriteBatch.DrawString(game.GetFont(Font), Text, Position,
                        (IsMouseHold ? MouseHoldColor :
                        (IsMouseHovered ? MouseHoverColor :
                        GraphicsHelper.GetColorWithAlpha(ForeColor, alpha))));
                    spriteBatch.DrawString(game.GetFont(Font), lastText, Position,
                        (IsMouseHold ? MouseHoldColor :
                        (IsMouseHovered ? MouseHoverColor :
                        ForeColor)));
                }
                else
                {
                    spriteBatch.DrawString(game.GetFont(Font), Caption, Position,
                        (IsMouseHold ? MouseHoldColor :
                        (IsMouseHovered ? MouseHoverColor :
                        ForeColor)));
                }
            }
        }

        public override void AttachToConsole()
        {
            base.AttachToConsole();
            game.Console.AddCommand("fo_speed", fo_speed);
        }

        void fo_speed(IList<string> args)
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
