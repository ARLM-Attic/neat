using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Neat.Effects
{
    public class CropEffect : EffectHandler
    {
        public Rectangle Bounds;

        public override void Initialize(NeatGame game)
        {
            base.Initialize(game);
            Effect = game.GetEffect("crop");
        }

        public override void BeginDraw(GameTime gameTime)
        {
            //Effect.CurrentTechnique = Effect.Techniques["Technique1"];
            Effect.Parameters["top"].SetValue(Bounds.Top);
            Effect.Parameters["down"].SetValue(Bounds.Bottom);
            Effect.Parameters["left"].SetValue(Bounds.Left);
            Effect.Parameters["right"].SetValue(Bounds.Right);
            base.BeginDraw(gameTime);
        }
    }
}
