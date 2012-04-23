using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neat.Graphics;

namespace Neat.Transitions
{
    public class Enter : Exit
    {
        protected override void Draw()
        {
            float a = MathHelper.Clamp((float)((Rate * Time.TotalMilliseconds) / Length.TotalMilliseconds), 0, 1);
            Color c = Fade ? new Color(new Vector4(a)) : Color.White;

            Game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Game.SpriteBatch.Draw(TargetA, Vector2.Zero, Color.White);

            if (Direction == Directions.Left)
                Game.SpriteBatch.Draw(TargetB,
                    new Vector2((a-1) * (float)(TargetB.Width), 0),
                    c);
            else if (Direction == Directions.Right)
                Game.SpriteBatch.Draw(TargetB,
                    new Vector2((1-a) * (float)(TargetB.Width), 0),
                    c);
            else if (Direction == Directions.Up)
                Game.SpriteBatch.Draw(TargetB,
                    new Vector2(0, (1-a) * (float)(TargetB.Height)),
                    c);
            else if (Direction == Directions.Down)
                Game.SpriteBatch.Draw(TargetB,
                    new Vector2(0, (a-1) * (float)(TargetB.Height)),
                    c);
            Game.SpriteBatch.End();
            if (Game.AutoDraw) Game.SpriteBatch.Begin();
        }
    }
}
