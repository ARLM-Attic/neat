using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.Graphics;
using Neat.Mathematics;

namespace Neat.GUI
{
    class Trackbar : Control
    {
        float 
            _min = 0, 
            _max = 100, 
            _value = 50;

        public float Percent { get; private set; }

        public Action OnValuesChanged = null;

        public void ValuesChanged()
        {
            Percent = (Value - Min) / (Max - Min);

            if (OnValuesChanged != null) OnValuesChanged();
        }

        public float Min { get { return _min; } set { _min = value; ValuesChanged(); } }
        public float Max { get { return _max; } set { _max = value; ValuesChanged(); } }
        public float Value { get { return _value; } set { _value = value; ValuesChanged(); } }

        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);
        }

        public override void Pressed(Vector2 pos = new Vector2())
        {
            if (!Enabled) return;
            var hitX = (pos.X - Position.X) / Size.X;
            var lu = Game.GetSlice("trackbar_lu");
            var ru = Game.GetSlice("trackbar_ru");
            float midW = Size.X - lu.Size.X - ru.Size.X;

            if (hitX >= 0 && hitX <= 1)
            {
                if (hitX <= 0.15) hitX = 0;
                else if (hitX >= 0.85) hitX = 1;

                Value = hitX * (Max - Min) + Min;
            }

            Parent.ClickHandled = true;
            base.Pressed(pos);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            var lu = Game.GetSlice("trackbar_lu");
            var lc = Game.GetSlice("trackbar_lc");
            var lb = Game.GetSlice("trackbar_lb");
            var ct = Game.GetSlice("trackbar_ct");
            var cc = Game.GetSlice("trackbar_cc");
            var cb = Game.GetSlice("trackbar_cb");
            var ru = Game.GetSlice("trackbar_ru");
            var rc = Game.GetSlice("trackbar_rc");
            var rb = Game.GetSlice("trackbar_rb");
            var gu = Game.GetSlice("trackbar_gu");
            var gc = Game.GetSlice("trackbar_gc");
            var gb = Game.GetSlice("trackbar_gb");

            float midW = Size.X - lu.Size.X - ru.Size.X;
            float midH = Size.Y - lu.Size.Y - lb.Size.Y;

            var col = Color.White;
            var gcol = IsMouseHold ? MouseHoldColor : IsMouseHovered ? MouseHoverColor : TintColor;

            spriteBatch.Draw(lu.Texture, Position, lu.Crop, col);
            spriteBatch.Draw(ct.Texture, new Rectangle((int)(Position.X + lu.Size.X), (int)Position.Y, (int)midW, (int)ct.Size.Y), ct.Crop, col);
            spriteBatch.Draw(ru.Texture, Position + new Vector2(lu.Size.X + midW, 0), ru.Crop, col);

            spriteBatch.Draw(lc.Texture, new Rectangle((int)(Position.X), (int)(Position.Y + lu.Size.Y), (int)(lc.Size.X), (int)midH), lc.Crop, col);
            spriteBatch.Draw(cc.Texture, new Rectangle((int)(Position.X + lu.Size.X), (int)(Position.Y + lu.Size.Y), (int)midW, (int)midH), cc.Crop, col);
            spriteBatch.Draw(rc.Texture, new Rectangle((int)(Position.X + lu.Size.X + midW), (int)(Position.Y + lu.Size.Y), (int)(rc.Size.X), (int)midH), rc.Crop, col);
            
            spriteBatch.Draw(lb.Texture, Position + new Vector2(0, lu.Size.Y + midH), lb.Crop, col);
            spriteBatch.Draw(cb.Texture, new Rectangle((int)(Position.X + lu.Size.X), (int)(Position.Y + lu.Size.Y + midH), (int)midW, (int)cb.Size.Y), cb.Crop, col);
            spriteBatch.Draw(rb.Texture, Position + new Vector2(lu.Size.X + midW, lu.Size.Y + midH), rb.Crop, col);

            var gx = Percent * (midW + lu.Size.X) + Position.X;
            var gch = Size.Y - (gu.Size.Y + gb.Size.X);

            spriteBatch.Draw(gu.Texture, new Vector2(gx, Position.Y), gu.Crop, gcol);
            spriteBatch.Draw(gc.Texture, new Rectangle((int)gx, (int)(Position.Y + gu.Size.Y), (int)gc.Size.X, (int)gch), gc.Crop, gcol);
            spriteBatch.Draw(gb.Texture, new Vector2(gx, Position.Y + gch + gu.Size.Y), gb.Crop, gcol);
        }
    }
}
