using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neat.Graphics;

namespace Neat.Transitions
{
    public class Fader : Transition
    {
        public float Rate { get; set; }

        public override void Initialize(GameTime gameTime)
        {
            if (Rate == 0) Rate = 1;
            base.Initialize(gameTime);
        }

        protected override void Draw()
        {
            float a = MathHelper.Clamp((float)((Rate * Time.TotalMilliseconds) / Length.TotalMilliseconds), 0, 1);
            Vector4 cB = new Vector4(a);
            Vector4 cA = Vector4.One - cB;

            Game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Game.SpriteBatch.Draw(TargetA, Vector2.Zero, Color.White);
            Game.SpriteBatch.Draw(TargetB, Vector2.Zero, new Color(cB));
            Game.SpriteBatch.End();
            if (Game.AutoDraw) Game.SpriteBatch.Begin();
            base.Draw();
        }
    }
}
