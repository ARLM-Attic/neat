using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neat.Graphics;

namespace Neat.Transitions
{
    public class Crop : Transition
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
                Game.SpriteBatch.Draw(TargetB, Vector2.Zero, Color.White);
                if (Horizontal)
                    Game.SpriteBatch.Draw(TargetA, Vector2.Zero,
                        new Rectangle(0, 0, (int)((1-a) * (float)(TargetA.Width)), TargetA.Height),
                        Color.White);
                else
                    Game.SpriteBatch.Draw(TargetA, Vector2.Zero,
                        new Rectangle(0, 0, TargetA.Width, (int)((1-a) * (float)TargetA.Height)),
                        Color.White);
            }
            else
            {
                Game.SpriteBatch.Draw(TargetA, Vector2.Zero, Color.White);
                if (Horizontal)
                    Game.SpriteBatch.Draw(TargetB, Vector2.Zero,
                        new Rectangle(0, 0, (int)(a * (float)(TargetB.Width)), TargetB.Height),
                        Color.White);
                else
                    Game.SpriteBatch.Draw(TargetB, Vector2.Zero,
                        new Rectangle(0, 0, TargetB.Width, (int)(a * (float)TargetB.Height)),
                        Color.White);
            }
            Game.SpriteBatch.End();
            if (Game.AutoDraw) Game.SpriteBatch.Begin();
            base.Draw();
        }
    }
}
