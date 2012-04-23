using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neat.Graphics;

namespace Neat.Effects
{
    public class ScaleEffect : EffectHandler
    {
        public float Scale = 1.0f;

        public override void EndDraw(GameTime gameTime)
        {
            Game.PopTarget(true);
            Game.GraphicsDevice.Clear(Color.Black);
            Game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                null, DepthStencilState.None, RasterizerState.CullNone, Effect);
            Game.SpriteBatch.Draw(
                Target, 
                new Vector2(Game.GameWidth, Game.GameHeight) / 2.0f, 
                null,
                FinalTint,
                0, 
                new Vector2(Target.Width, Target.Height) / 2.0f,
                Scale,
                SpriteEffects.None,
                0);
            Game.SpriteBatch.End();
        }
    }
}
