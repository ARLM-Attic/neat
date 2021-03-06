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
using Neat.Graphics;

namespace Neat.GUI
{
    public class CheckBox : Control 
    {
        public bool Checked = false;
        public string CheckedPicture = "checkedBox";
        public string UncheckedPicture = "uncheckedBox";
        public bool DrawShadow = true;
        public override void Initialize()
        {
            _size.Y = 32;
            Caption = "Checkbox";
            base.Initialize();
        }

        public override void Released(Vector2 pos = new Vector2())
        {
            Checked = !Checked;
            if (Checked)
            {
                if (OnCheckedRun != null) Game.Console.Run(OnCheckedRun);
                if (OnChecked != null) OnChecked(); 
            }
            else
            {
                if (OnUncheckedRun != null) Game.Console.Run(OnUncheckedRun);
                if (OnUnchecked != null) OnUnchecked(); }
        
            base.Released();
        }

        public string OnCheckedRun, OnUncheckedRun;

        public event XEventHandler OnChecked;
        public event XEventHandler OnUnchecked;

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Rectangle bounds = new Rectangle((int)(position.X), (int)(position.Y), (int)(size.X), (int)(size.Y));
            Texture2D t = Game.GetTexture(Checked ? CheckedPicture : UncheckedPicture);
            spriteBatch.Draw(t, 
                new Rectangle((int)(Position.X),(int)(Position.Y),32,32) , TintColor); // Draw Background

            Vector2 textsize = Game.GetFont(Font).MeasureString(Caption);

            if (DrawShadow)
            {
                GraphicsHelper.DrawShadowedString(spriteBatch, Game.GetFont(Font), Caption, Position + new Vector2(35, 0),
                    (IsMouseHold ? MouseHoldColor :
                    (IsMouseHovered ? MouseHoverColor :
                    ForeColor)),
                    Color.Black);
            }
            else
            {
                spriteBatch.DrawString(Game.GetFont(Font), Caption, Position + new Vector2(35, 0),
                    (IsMouseHold ? MouseHoldColor :
                    (IsMouseHovered ? MouseHoverColor :
                    ForeColor)));
            }
        }

        public override void AttachToConsole()
        {
            base.AttachToConsole();
            Game.Console.AddCommand("fc_checked", fc_checked);
            Game.Console.AddCommand("fc_drawshadow", fc_drawshadow);
            Game.Console.AddCommand("fc_checkedimg", fc_checkedimg);
            Game.Console.AddCommand("fc_uncheckedimg", fc_uncheckedimg);
            Game.Console.AddCommand("fc_onchecked", fc_onchecked);
            Game.Console.AddCommand("fc_onunchecked", fc_onunchecked);
        }

        void fc_onchecked(IList<string> args)
        {
            OnCheckedRun = Game.Console.Args2Str(args, 1);
        }

        void fc_onunchecked(IList<string> args)
        {
            OnUncheckedRun = Game.Console.Args2Str(args, 1);
        }

        void fc_checked(IList<string> args)
        {
            if (args.Count != 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [bool]");
                return;
            }
            Checked = bool.Parse(args[1]);
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

        void fc_checkedimg(IList<string> args)
        {
            if (args.Count != 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [string]");
                return;
            }
            CheckedPicture = args[1];
        }

        void fc_uncheckedimg(IList<string> args)
        {
            if (args.Count != 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [string]");
                return;
            }
            UncheckedPicture = args[1];
        }
    }
}
