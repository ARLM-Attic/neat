using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
#if WINDOWS
using Microsoft.Xna.Framework.Net;
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
    public class CollisionResolution : Screen
    {
        public CollisionResolution(NeatGame Game)
            : base(Game)
        {
        }

        Form form;
        Polygon poly;
        LineBrush lb;
        PhysicsSimulator sim;
        List<Body> bodies;
        List<Color> palette;
        bool paused = false;
        public override void Initialize()
        {
            base.Initialize();
            form = new Form(game);
            form.MouseSpeed = 0;
            Reset();
        }

        void Reset()
        {
            sim = new PhysicsSimulator(game);

            lb = new LineBrush(game.GraphicsDevice, 1);

            poly = new Polygon();
            poly.AddVertex(0, 0);
            poly.AddVertex(0, 150);
            poly.AddVertex(100, 150);
            poly.AddVertex(100, 0);/*
            poly.AddVertex(50, 100);
            poly.AddVertex(50, 50);
            poly.AddVertex(100, 50);
            poly.AddVertex(100, 0);*/
            poly.Triangulate();

            var poly2 = new Polygon();
            poly2.AddVertex(0, 500);
            poly2.AddVertex(500, 400);
            poly2.AddVertex(800, 500);
            poly2.AddVertex(800, 600);
            poly2.AddVertex(0, 600);
            poly2.Triangulate();

            var poly3 = new Polygon();
            poly3.AddVertex(700, 0);
            poly3.AddVertex(800, 0);
            poly3.AddVertex(800, 450);
            poly3.AddVertex(700, 450);
            poly3.Triangulate();

            var circle = Polygon.BuildCircle(20, new Vector2(300), 90);
            circle.Triangulate();

            bodies = new List<Body>();
            bodies.Add(new Body(sim, game, poly, 10));
            bodies.Add(new Body(sim, game, poly2, 10));
            bodies.Add(new Body(sim, game, poly3, 10));
            bodies.Add(new Body(sim, game, circle, 10));
            //body1.AttachToGravity = false;
            bodies[1].IsStatic = true;
            bodies[2].IsStatic = true;
            bodies[3].IsStatic = true;
            //body2.IsFree = true;

            palette = new List<Color>();
            for (int i = 0; i < 10; i++)
            {
                palette.Add(GraphicsHelper.GetRandomColor());
            }
        }

        public override void Activate()
        {
            base.Activate();
        }

        bool showFill = false;
        public override void Behave(GameTime gameTime)
        {
            showFill = false;
            Vector2 mouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            foreach (var item in bodies[0].Mesh.Triangles)
            {
                if (item.IsInside(mouse))
                {
                    showFill = true;
                    break;
                }
            }

            
            form.Update(gameTime);
            base.Behave(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            bodies[0].Velocity.X = 0f;
            if (game.IsPressed(Keys.Left))
            {
                //bodies[0].Mesh.Offset(new Vector2(-10, -0.1f));
                //bodies[0].Mesh.Triangulate();
                bodies[0].ApplyForce(new Vector2(-100, 0));
            }
                //body1.ApplyForce(new Vector2(-10, 0));
            if (game.IsPressed(Keys.Right))
            {
                //bodies[0].Mesh.Offset(new Vector2(10, -0.1f));
                //bodies[0].Mesh.Triangulate();
                bodies[0].ApplyForce(new Vector2(100, 0));
            }
                //body1.ApplyForce(new Vector2(10, 0));
            if (game.IsPressed(Keys.Up))
            {
                //bodies[0].Mesh.Offset(new Vector2(0, -5f));
                bodies[0].ApplyForce(new Vector2(0, -100));
            }
            if (game.IsPressed(Keys.Down))
            {
                //bodies[0].Mesh.Offset(new Vector2(0, 5f));
                bodies[0].ApplyForce(new Vector2(0, 100));
            }
            if (game.IsTapped(Keys.Space)) paused = !paused;
            if (game.IsTapped(Keys.R)) Reset();

            if (!paused)
            {
                sim.Update();
            }
            base.HandleInput(gameTime);
        }


        public override void Render(GameTime gameTime)
        {
            int q = 0;
            base.Render(gameTime);
            game.Write(showFill.ToString() + GeometryHelper.Coords2String(bodies[0].Mesh.GetPosition()), new Vector2(100));

            foreach (var item in bodies)
            {
                q++;
                var c = palette[q % palette.Count];
                
                foreach (var tri in item.Mesh.Triangles)
                {
                    tri.Draw(game.SpriteBatch, lb, c);
                }
            }

            form.Draw(gameTime);
        }
    }
}
