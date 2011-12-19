using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Neat
{
    public class EffectHandler
    {
        protected Effect Effect;
        protected RenderTarget2D Target;
        protected NeatGame Game;
        protected Color FinalTint;
        public EffectHandler()
        {
        }

        public virtual void Initialize(NeatGame game)
        {
            Game = game;
            FinalTint = Color.White;
            
            Target = new RenderTarget2D(Game.GraphicsDevice, Game.GameWidth, Game.GameHeight, 
                false, SurfaceFormat.Color, DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
        }

        public virtual void BeginDraw(GameTime gameTime)
        {
            Game.PushTarget(Target, false);
        }

        public virtual void EndDraw(GameTime gameTime)
        {
            Game.PopTarget(true);
            //Game.GraphicsDevice.Clear(Color.Black);
            Game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                null, DepthStencilState.None, RasterizerState.CullNone, Effect);
            Game.SpriteBatch.Draw(Target, Vector2.Zero, FinalTint);
            Game.SpriteBatch.End();
        }
    }
}
