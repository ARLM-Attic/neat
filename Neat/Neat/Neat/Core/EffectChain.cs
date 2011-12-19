using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Neat
{
    public class EffectChain : List<EffectHandler>
    {
        protected NeatGame Game;

        public EffectChain(NeatGame game)
        {
            Game = game;
        }

        public new void Add(EffectHandler effect)
        {
            base.Add(effect);
            effect.Initialize(Game);
        }

        public void Begin(GameTime gameTime)
        {
            if (Count > 0)
            {
                if (Game.AutoDraw) Game.SpriteBatch.End();
                for (int i = 0; i < Count; i++)
                {
                    this[i].BeginDraw(gameTime);
                }
                Game.GraphicsDevice.SetRenderTarget(Game.CurrentTarget);
                if (Game.AutoDraw) Game.SpriteBatch.Begin();
            }
        }

        public void End(GameTime gameTime)
        {
            if (Count > 0)
            {
                if (Game.AutoDraw) Game.SpriteBatch.End();
                for (int i = Count - 1; i >= 0; i--)
                {
                    this[i].EndDraw(gameTime);
                }
                if (Game.AutoDraw) Game.SpriteBatch.Begin();
            }
        }
    }
}
