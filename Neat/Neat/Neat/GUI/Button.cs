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
    public class Button : FormObject 
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
        public override void Update(GameTime gameTime)
        {
            if (IsMouseHovered) TintColor = HoverTintColor ;
            else TintColor = NormalTintColor;
            base.Update(gameTime);
        }
    }
}
