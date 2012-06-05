using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neat.Graphics;
using Neat.Mathematics;
using Microsoft.Xna.Framework.Input;

namespace Neat.GUI
{
    class Window : Container
    {
        public string TopLeftSprite = "window_tl";
        public string TopMidSprite = "window_tm";
        public string TopRightSprite = "window_tr";
        public string VBarSprite = "window_v";
        public string HBarSprite = "window_h";
        public string BottomLeftSprite = "window_bl";
        public string BottomRightSprite = "window_br";
        public bool Movable = true;
        public bool Resizable = true;

        public override void Initialize()
        {
            base.Initialize();
            BackgroundColor = Color.White;
            Size = new Vector2(640, 480);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawTarget(gameTime, spriteBatch);
            var tl = Game.GetSlice(TopLeftSprite);
            var tm = Game.GetSlice(TopMidSprite);
            var tr = Game.GetSlice(TopRightSprite);
            var v = Game.GetSlice(VBarSprite);
            var h = Game.GetSlice(HBarSprite);
            var bl = Game.GetSlice(BottomLeftSprite);
            var br = Game.GetSlice(BottomRightSprite);

            spriteBatch.Draw(Target, GeometryHelper.Vectors2Rectangle(
                    Position + new Vector2(v.Crop.Value.Width, tl.Crop.Value.Height),
                    Size - new Vector2(v.Crop.Value.Width * 2.0f, tm.Crop.Value.Height + h.Crop.Value.Height)), 
                GeometryHelper.Vectors2Rectangle(
                    Vector2.Zero,
                    Size - new Vector2(v.Crop.Value.Width*2.0f, tm.Crop.Value.Height + h.Crop.Value.Height)), 
                TintColor);

            spriteBatch.Draw(tl.Texture, Position, tl.Crop, TintColor);
            spriteBatch.Draw(tm.Texture,
                GeometryHelper.Vectors2Rectangle(
                    Position + new Vector2(tl.Crop.Value.Width, 0),
                    new Vector2(Size.X - (tl.Crop.Value.Width + tr.Crop.Value.Width), tm.Crop.Value.Height)),
                tm.Crop, TintColor);
            spriteBatch.Draw(tr.Texture,
                    Position + new Vector2(Size.X - tr.Crop.Value.Width, 0),
                    tr.Crop, TintColor);

            spriteBatch.Draw(v.Texture,
                GeometryHelper.Vectors2Rectangle(
                    Position + new Vector2(0, tl.Crop.Value.Height),
                    new Vector2(v.Crop.Value.Width, Size.Y - (tl.Crop.Value.Height + bl.Crop.Value.Height))),
                v.Crop, TintColor);
            spriteBatch.Draw(v.Texture,
                GeometryHelper.Vectors2Rectangle(
                    Position + new Vector2(Size.X - v.Crop.Value.Width, tr.Crop.Value.Height),
                    new Vector2(v.Crop.Value.Width, Size.Y - (tr.Crop.Value.Height + br.Crop.Value.Height))),
                v.Crop, TintColor);

            spriteBatch.Draw(bl.Texture,
                Position + new Vector2(0, Size.Y - bl.Crop.Value.Height),
                bl.Crop, TintColor);
            spriteBatch.Draw(h.Texture,
                GeometryHelper.Vectors2Rectangle(
                    Position + new Vector2(bl.Crop.Value.Width, Size.Y - h.Crop.Value.Height),
                    new Vector2(Size.X - (bl.Crop.Value.Width + br.Crop.Value.Width), h.Crop.Value.Height)),
                    h.Crop, TintColor);
            spriteBatch.Draw(br.Texture,
                Position + Size - new Vector2(br.Crop.Value.Width, br.Crop.Value.Height),
                br.Crop, TintColor);

            var fnt = Game.GetFont(Font);
            Vector2 txtSize = fnt.MeasureString(Caption);
            spriteBatch.DrawString(Game.GetFont(Font), Caption,
                Position + new Vector2(
                    tl.Crop.Value.Width + (Size.X - (tl.Crop.Value.Width + tr.Crop.Value.Width) - txtSize.X) / 2.0f,
                    (tm.Crop.Value.Height - txtSize.Y) / 2.0f),
                ForeColor);
        }

        Vector2 lastPosition, lastSize;
        Vector2 mouseRelPosition;
        bool moving = false, resizing = false;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            if (Enabled)
            {
                if (resizing || moving) Parent.ClickHandled = true;
                if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    moving = false;
                    resizing = false;
                    lastPosition = Position;
                    lastSize = Size;
                    mouseRelPosition = Parent.MousePosition;
                }
                else if (Mouse.GetState().LeftButton == ButtonState.Pressed && !Parent.ClickHandled)
                {
                    Held(Parent.MousePosition);
                }

                
            }

            base.HandleInput(gameTime);
        }

        public override void Held(Vector2 pos = new Vector2())
        {
            var br = Game.GetSlice(BottomRightSprite);
            var vbr = new Vector2(br.Crop.Value.Width, br.Crop.Value.Height) * 3.0f;
            if (GeometryHelper.IsVectorInRectangle(Position, Size, pos))
            {
                Parent.BringControlToFront(this);
                Parent.ClickHandled = true;
                if (!resizing && GeometryHelper.IsVectorInRectangle(Position, new Vector2(Size.X, Game.GetSlice(TopMidSprite).Crop.Value.Height), pos)) { moving = true; }
                else if (!moving && GeometryHelper.IsVectorInRectangle(Position + Size - vbr, vbr, pos)) resizing = true;
                if (moving && !Movable) moving = false;
                if (resizing && !Resizable) resizing = false;
            }

            if (moving)
            {
                Position = lastPosition + (pos - mouseRelPosition);
            }
            else if (resizing)
            {
                var size = lastSize + (pos - mouseRelPosition);
                if (size.X > MinSize.X) _size.X = size.X;
                if (size.Y > MinSize.Y) _size.Y = size.Y;
            }
            base.Held();
        }

        public override void Move(Vector2 newPos)
        {
            base.Move(newPos);
            Form.MouseOffset = Parent.MouseOffset - (newPos + new Vector2(
                Game.GetSlice(VBarSprite).Crop.Value.Width,
                Game.GetSlice(TopLeftSprite).Crop.Value.Height));
        }

        #region Console
        public override void AttachToConsole()
        {
            base.AttachToConsole();
            Game.Console.AddCommand("fc_movable", fc_movable);
            Game.Console.AddCommand("fc_resizable", fc_resizable);
        }

        void fc_movable(IList<string> args)
        {
            if (args.Count < 2) Game.Console.WriteLine(Movable.ToString());
            else Movable = bool.Parse(args[1]);
        }

        void fc_resizable(IList<string> args)
        {
            if (args.Count < 2) Game.Console.WriteLine(Resizable.ToString());
            else Resizable = bool.Parse(args[1]);
        }
        #endregion
    }
}
