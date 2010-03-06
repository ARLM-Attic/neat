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
    public class CheckBox : FormObject 
    {
        public bool isChecked = false;
        public string checkedPicture = "checkedBox";
        public string uncheckedPicture = "uncheckedBox";
        public override void Initialize()
        {
            size.Y = 32;
            caption = "Checkbox";
            base.Initialize();
        }

        public override void Released()
        {
            isChecked = !isChecked;
            if (isChecked)
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
            Texture2D t = game.getTexture(isChecked ? checkedPicture : uncheckedPicture);
            spriteBatch.Draw(t, 
                new Rectangle((int)(position.X),(int)(position.Y),32,32) , tintColor); // Draw Background

            Vector2 textsize = game.GetFont(font).MeasureString(caption);
            game.DrawShadowedString(game.GetFont(font), caption, position + new Vector2(35,0),
                (isMouseHold ? mouseHoldColor :
                (isMouseHovered ? mouseHoverColor :
                foreColor)),
                Color.Black);
        }
    }
}
