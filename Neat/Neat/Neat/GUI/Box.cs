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
    public class Box:Control 
    {
        public override void Initialize()
        {
            Selectable = false;
            SetColor(GetColorWithAlpha(Color.White,0.2f));
            TintColor = GetColorWithAlpha(Color.White, 0.5f);
            PushSound = "mute";
            base.Initialize();
        }

        public void SetColor(Color color)
        {
            ForeColor = color;
            MouseHoldColor = color;
            MouseHoverColor = color;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game.GetTexture("solid"),
                new Rectangle((int)(Position.X), (int)(Position.Y), (int)(Size.X), (int)(Size.Y)),
                TintColor);
        }
    }
}
