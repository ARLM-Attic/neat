using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neat;
using Microsoft.Xna.Framework;

namespace Neat.Effects
{
    public class RippleEffect : EffectHandler
    {
        public float PhaseCoef = 1.0f;
        public float Amplitude = 1.0f;
        public Color Tint;
        public Vector2 UVShift = Vector2.Zero;

        float phase;

        public override void Initialize(NeatGame game)
        {
            base.Initialize(game);
            Effect = game.GetEffect("ripple");
            Tint = Color.White;
        }

        public override void Update(GameTime gameTime)
        {
            phase = MathHelper.ToDegrees(Game.Frame*0.0001f);
            base.Update(gameTime);
        }

        public override void BeginDraw(GameTime gameTime)
        {
            //Effect.CurrentTechnique = Effect.Techniques["Technique1"];
            Effect.Parameters["phase"].SetValue(PhaseCoef * phase);
            Effect.Parameters["amplitude"].SetValue(Amplitude);
            Effect.Parameters["tint"].SetValue(Tint.ToVector4());
            Effect.Parameters["uvShift"].SetValue(UVShift);
            base.BeginDraw(gameTime);
        }
    }
}
