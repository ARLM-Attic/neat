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
    public class Image:FormObject 
    {
        public override void Initialize()
        {
            selectable = false;
            SetColor(Color.White);
            pushSound = "mute";
            base.Initialize();
        }

        public bool autoSize = true;
        public void SetColor(Color color)
        {
            foreColor = color;
            mouseHoldColor = color;
            mouseHoverColor = color;
        }

        public void CenterX()
        {
            position.X = game.gameWidth / 2 - game.getTexture(backgroundImage).Width / 2;
        }
        public void CenterY()
        {
            position.Y = game.gameHeight / 2 - game.getTexture(backgroundImage).Height / 2;
        }
        public void Center()
        {
            CenterX();
            CenterY();
        }

        public void StretchToScreen()
        {
            position = Vector2.Zero;
            size = new Vector2(((float)(game.gameWidth)), ((float)(game.gameHeight)));
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (autoSize)
            {
                spriteBatch.Draw(game.getTexture(backgroundImage),
                    position,
                    tintColor);
            }
            else
            {
                spriteBatch.Draw(game.getTexture(backgroundImage),
                    new Rectangle((int)(position.X), (int)(position.Y), (int)(size.X), (int)(size.Y)),
                    tintColor);
            }
        }
    }
}
