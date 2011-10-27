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
    public class Button : Control 
    {
        public Button() : base()
        {
        }
        public Color HoverTintColor = Color.Purple;
        public Color NormalTintColor = Color.White;
        public override void Initialize()
        {
            BackgroundImage = "buttonBG";
            Caption = "Button";
            base.Initialize();
        }
        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);
            if (IsMouseHovered) TintColor = HoverTintColor ;
            else TintColor = NormalTintColor;
        }

        public override void AttachToConsole()
        {
            base.AttachToConsole();
            Game.Console.AddCommand("fc_hovertintcolor", fc_hovertintcolor);
            Game.Console.AddCommand("fc_tintcolor", fc_tintcolor);
        }

        void fc_hovertintcolor(IList<string> args)
        {
            if (args.Count < 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            HoverTintColor = Game.Console.ParseColor(Game.Console.Args2Str(args, 1));
        }

        void fc_tintcolor(IList<string> args)
        {
            if (args.Count < 2)
            {
                Game.Console.WriteLine("syntax: " + args[0] + " [color]");
                return;
            }
            NormalTintColor = Game.Console.ParseColor(Game.Console.Args2Str(args, 1));
        }
    }
}
