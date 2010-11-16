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
    public class Image:FormObject 
    {
        public override void Initialize()
        {
            Selectable = false;
            SetColor(Color.White);
            PushSound = "mute";
            base.Initialize();
        }

        public bool AutoSize = true;
        public void SetColor(Color color)
        {
            ForeColor = color;
            MouseHoldColor = color;
            MouseHoverColor = color;
        }

        public void CenterX()
        {
            Position.X = game.gameWidth / 2 - game.getTexture(BackgroundImage).Width / 2;
        }
        public void CenterY()
        {
            Position.Y = game.gameHeight / 2 - game.getTexture(BackgroundImage).Height / 2;
        }
        public void Center()
        {
            CenterX();
            CenterY();
        }

        public void StretchToScreen()
        {
            Position = Vector2.Zero;
            Size = new Vector2(((float)(game.gameWidth)), ((float)(game.gameHeight)));
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (AutoSize)
            {
                spriteBatch.Draw(game.getTexture(BackgroundImage),
                    Position,
                    TintColor);
            }
            else
            {
                spriteBatch.Draw(game.getTexture(BackgroundImage),
                    new Rectangle((int)(Position.X), (int)(Position.Y), (int)(Size.X), (int)(Size.Y)),
                    TintColor);
            }
        }
    }
}
