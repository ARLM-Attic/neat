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
    public class CheckBox : FormObject 
    {
        public bool Checked = false;
        public string CheckedPicture = "checkedBox";
        public string UncheckedPicture = "uncheckedBox";
        public bool DrawShadow = true;
        public override void Initialize()
        {
            Size.Y = 32;
            Caption = "Checkbox";
            base.Initialize();
        }

        public override void Released()
        {
            Checked = !Checked;
            if (Checked)
            { if (OnChecked != null) OnChecked(); }
            else
            { if (OnUnchecked != null) OnUnchecked(); }

            base.Released();
        }

        public event XEventHandler OnChecked;
        public event XEventHandler OnUnchecked;

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Rectangle bounds = new Rectangle((int)(position.X), (int)(position.Y), (int)(size.X), (int)(size.Y));
            Texture2D t = game.getTexture(Checked ? CheckedPicture : UncheckedPicture);
            spriteBatch.Draw(t, 
                new Rectangle((int)(Position.X),(int)(Position.Y),32,32) , TintColor); // Draw Background

            Vector2 textsize = game.GetFont(Font).MeasureString(Caption);

            if (DrawShadow)
            {
                GraphicsHelper.DrawShadowedString(spriteBatch, game.GetFont(Font), Caption, Position + new Vector2(35, 0),
                    (IsMouseHold ? MouseHoldColor :
                    (IsMouseHovered ? MouseHoverColor :
                    ForeColor)),
                    Color.Black);
            }
            else
            {
                spriteBatch.DrawString(game.GetFont(Font), Caption, Position + new Vector2(35, 0),
                    (IsMouseHold ? MouseHoldColor :
                    (IsMouseHovered ? MouseHoverColor :
                    ForeColor)));
            }
        }

        public override void AttachToConsole()
        {
            base.AttachToConsole();
            game.Console.AddCommand("fo_checked", fo_checked);
            game.Console.AddCommand("fo_drawshadow", fo_drawshadow);
            game.Console.AddCommand("fo_checkedimg", fo_checkedimg);
            game.Console.AddCommand("fo_uncheckedimg", fo_uncheckedimg);
        }

        void fo_checked(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            Checked = bool.Parse(args[1]);
        }

        void fo_drawshadow(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            DrawShadow = bool.Parse(args[1]);
        }

        void fo_checkedimg(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [string]");
                return;
            }
            CheckedPicture = args[1];
        }

        void fo_uncheckedimg(IList<string> args)
        {
            if (args.Count != 2)
            {
                game.Console.WriteLine("syntax: " + args[0] + " [string]");
                return;
            }
            UncheckedPicture = args[1];
        }
    }
}
