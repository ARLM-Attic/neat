using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neat.Graphics;

namespace Neat.Transitions
{
    public class Push : Transition
    {
        public float Rate { get; set; }
        public bool Horizontal = true;
        public bool Reverse = false;
        public bool Random = false;

        public override void Initialize(GameTime gameTime)
        {
            base.Initialize(gameTime);
            if (Random)
            {
                Horizontal = Game.GetRandomBool();
                Reverse = Game.GetRandomBool();
            }
            if (Rate == 0) Rate = 1;
        }

        protected override void Draw()
        {
            float a = MathHelper.Clamp((float)((Rate * Time.TotalMilliseconds) / Length.TotalMilliseconds), 0, 1);

            Game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (Reverse)
            {
                if (Horizontal)
                {
                    Game.SpriteBatch.Draw(TargetA,
                        new Vector2(a * (float)(TargetA.Width), 0),
                        Color.White);
                    Game.SpriteBatch.Draw(TargetB,
                        new Vector2((a - 1) * (float)(TargetB.Width), 0),
                        Color.White);
                }
                else
                {
                    Game.SpriteBatch.Draw(TargetA,
                        new Vector2(0, a * (float)(TargetA.Height)),
                        Color.White);
                    Game.SpriteBatch.Draw(TargetB,
                        new Vector2(0, (a - 1) * (float)(TargetB.Height)),
                        Color.White);
                }
            }
            else
            {
                if (Horizontal)
                {
                    Game.SpriteBatch.Draw(TargetA,
                        new Vector2(-a * (float)(TargetA.Width), 0),
                        Color.White);
                    Game.SpriteBatch.Draw(TargetB,
                        new Vector2((-a + 1) * (float)(TargetB.Width), 0),
                        Color.White);
                }
                else
                {
                    Game.SpriteBatch.Draw(TargetA,
                        new Vector2(0, -a * (float)(TargetA.Height)),
                        Color.White);
                    Game.SpriteBatch.Draw(TargetB,
                        new Vector2(0, (-a + 1) * (float)(TargetB.Height)),
                        Color.White);
                }
            }
            Game.SpriteBatch.End();
            if (Game.AutoDraw) Game.SpriteBatch.Begin();
            base.Draw();
        }
    }
}
