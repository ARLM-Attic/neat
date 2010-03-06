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
    public class Box:FormObject 
    {
        public override void Initialize()
        {
            selectable = false;
            SetColor(GetColorWithAlpha(Color.White,0.2f));
            tintColor = GetColorWithAlpha(Color.White, 0.5f);
            pushSound = "mute";
            base.Initialize();
        }

        public void SetColor(Color color)
        {
            foreColor = color;
            mouseHoldColor = color;
            mouseHoverColor = color;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(game.getTexture("solid"),
                new Rectangle((int)(position.X), (int)(position.Y), (int)(size.X), (int)(size.Y)),
                tintColor);
        }
    }
}
