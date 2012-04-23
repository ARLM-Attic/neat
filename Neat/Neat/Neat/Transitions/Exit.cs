using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neat.Graphics;

namespace Neat.Transitions
{
    public enum Directions { Left, Right, Up, Down, Count }
    public class Exit : Transition
    {
        public float Rate { get; set; }
        public Directions Direction;
        public bool Random = false;
        public bool Fade = true;

        public override void Initialize(GameTime gameTime)
        {
            base.Initialize(gameTime);
            if (Random)
            {
                Direction = (Directions)(Game.RandomGenerator.Next((int)Directions.Count));
            }
            if (Rate == 0) Rate = 1;
        }

        protected override void Draw()
        {
            float a = MathHelper.Clamp((float)((Rate * Time.TotalMilliseconds) / Length.TotalMilliseconds), 0, 1);
            Color c = Fade ? new Color(new Vector4(1-a)) : Color.White;
            Game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Game.SpriteBatch.Draw(TargetB, Vector2.Zero, Color.White);

            if (Direction == Directions.Left)
                Game.SpriteBatch.Draw(TargetA,
                    new Vector2(-a * (float)(TargetA.Width), 0),
                    c);
            else if (Direction == Directions.Right)
                Game.SpriteBatch.Draw(TargetA,
                    new Vector2(a * (float)(TargetA.Width), 0),
                    c);
            else if (Direction == Directions.Up)
                Game.SpriteBatch.Draw(TargetA,
                    new Vector2(0, -a * (float)(TargetA.Height)),
                    c);
            else if (Direction == Directions.Down)
                Game.SpriteBatch.Draw(TargetA,
                    new Vector2(0, a * (float)(TargetA.Height)),
                    c);
            Game.SpriteBatch.End();
            if (Game.AutoDraw) Game.SpriteBatch.Begin();
            base.Draw();
        }
    }
}
