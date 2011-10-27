using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neat.Graphics;
using Neat.Mathematics;

namespace Neat.GUI
{
    public class Container : Control
    {
        public Form Form;
        public RenderTarget2D Target = null;
        public Color BackgroundColor = Color.Transparent;
        public Color BorderColor = Color.Gold;
        public Vector2 MinSize = new Vector2(128, 64);
        LineBrush lb;
        public Container() : base()
        {
        }

        public override void Initialize()
        {
            Form = new Form(Game);
            Form.MainForm = false;
            lb = new LineBrush(Game.GraphicsDevice, 1);
            Resize(_size);
        }

        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                Form.ClickHandled = Parent.ClickHandled;
                Form.Update(gameTime);
            }
        }

        public virtual void DrawTarget(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Form == null) return;
            if (Target == null)
            {
                Resize(Size);
                return;
            }

            spriteBatch.End();
            
            Game.GraphicsDevice.SetRenderTarget(Target);
            Game.GraphicsDevice.Clear(BackgroundColor);
            spriteBatch.Begin();//SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, DepthStencilState.None, null);
            Form.Draw(gameTime);
            spriteBatch.End();
            
            Game.GraphicsDevice.SetRenderTarget(Game.DefaultTarget);
            spriteBatch.Begin();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawTarget(gameTime, spriteBatch);
            Rectangle bounds = GeometryHelper.Vectors2Rectangle(Position, Size);
            spriteBatch.Draw(Target, bounds, TintColor);
            //lb.DrawRectangle(spriteBatch, bounds, BorderColor);
        }

        public override void Resize(Vector2 newSize)
        {
            newSize.X = Math.Max(newSize.X, MinSize.X);
            newSize.Y = Math.Max(newSize.Y, MinSize.Y);
            Target = new RenderTarget2D(Game.GraphicsDevice, (int)(newSize.X), (int)(newSize.Y), false, SurfaceFormat.Color, DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
            Form.Size = newSize;
            base.Resize(newSize);
        }

        public override void Move(Vector2 newPos)
        {
            Form.MouseOffset = Parent.MouseOffset - newPos;
            base.Move(newPos);
        }

        public override void AttachToConsole()
        {
            Game.Console.AddCommand("fc_selform", fc_attachform);
            base.AttachToConsole();
        }

        void fc_attachform(IList<string> args)
        {
            Form.AttachToConsole();
        }
    }
}
