#region References
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using RedBulb;
using RedBulb.MenuSystem;
//using UltimaScroll.IBLib;
#endregion

namespace RedBulb.GUI
{
    public class Button : FormObject 
    {
        public Button() : base()
        {
        }
        public Color hoverTintColor = Color.Purple;
        public Color normalTintColor = Color.White;
        public override void Initialize()
        {
            backgroundImage = "buttonBG";
            caption = "Button";
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            if (isMouseHovered) tintColor = hoverTintColor ;
            else tintColor = normalTintColor;
            base.Update(gameTime);
        }
    }
}
