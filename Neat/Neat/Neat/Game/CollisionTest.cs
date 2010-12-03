using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if LIVE
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
#if WINDOWS

using Microsoft.Xna.Framework.Storage;
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
using Neat.EasyMenus;
using Neat.GUI;
using Neat.Mathematics;
using Neat.Graphics;

namespace Neat
{
    public class CollisionTest : Screen
    {
        public CollisionTest(NeatGame Game)
            : base(Game)
        {
        }

        Form form;
        Polygon poly1,poly2;
        LineBrush lb;

        public override void Initialize()
        {
            base.Initialize();
            form = new Form(game);
            form.MouseSpeed = 0;
            Reset();

            lb = new LineBrush(game.GraphicsDevice, 1);
        }

        void Reset()
        {
            poly1 = Polygon.BuildCircle(20, new Vector2(10), 70);
            poly1.AutoTriangulate = true;
            poly1.Triangulate();

            poly2 = new Polygon();
            poly2.AddVertex(0, 500);
            poly2.AddVertex(800, 500);
            poly2.AddVertex(800, 600);
            poly2.AddVertex(0, 600);
            poly2.Vertices = poly2.GetVerticesCounterClockwise();
            poly2.AutoTriangulate = true;
            poly2.Triangulate();
        }

        public override void Activate()
        {
            base.Activate();
        }
        Vector2 mouse;
        public override void Behave(GameTime gameTime)
        {
            mouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            
            form.Update(gameTime);
            base.Behave(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            if (game.IsPressed(Keys.Left)) poly1.Offset(new Vector2(-5,0));
            if (game.IsPressed(Keys.Right)) poly1.Offset(new Vector2(5, 0));
            if (game.IsPressed(Keys.Up)) poly1.Offset(new Vector2(0, -5));
            if (game.IsPressed(Keys.Down)) poly1.Offset(new Vector2(0, 5));
            if (game.IsPressed(Keys.R)) Reset();
            base.HandleInput(gameTime);
        }


        public override void Render(GameTime gameTime)
        {
            base.Render(gameTime);
            foreach (var item in poly1.Triangles)
            {
                item.Draw(game.SpriteBatch, lb, Color.White);
            }
            poly2.Draw(game.SpriteBatch, lb, Color.Red);
            Vector2 mtd = Vector2.Zero;
            var c = (Polygon.Collide(poly1, poly2, out mtd));
            game.Write(c.ToString(), new Vector2(100));
            if (c) game.Write("mtd="+GeometryHelper.Coords2String(mtd)+
            "\nmouse="+GeometryHelper.Coords2String(mouse), new Vector2(200));
            form.Draw(gameTime);
        }
    }
}
