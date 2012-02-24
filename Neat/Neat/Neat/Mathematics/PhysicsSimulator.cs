﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using Neat.Graphics;

namespace Neat.Mathematics
{
    public class PhysicsSimulator : GameComponent
    {
        const float _epsilon = 1f;
        const float _bigNumber = 100f;
        Vector2 negativeOne = new Vector2(-1);

        public float AllowedPenetrationDepth = 0;// -0.1f;
        public float DampingCoef = 0.95f;
        public Vector2 Gravity = new Vector2(0, 0.98f);
        public List<Body> Bodies = new List<Body>();
        public float SpeedCoef = 0.025f;
        public float StickCoef = 0.8f;
        public short MaxUpdatePerBody = 1;


        public float Speed { get; protected set; }
        public NeatGame game;
        short[] bodyUpdateCount;

        Neat.Components.Console console { get { if (game == null) return null; else return game.Console; } set { game.Console = value; } }

        public PhysicsSimulator(NeatGame i_game) : base(i_game)
        {
            game = i_game;
            Initialize();
        }

        public override void Initialize()
        {
            AttachToConsole();
        }


        public override void Update(GameTime gameTime)
        {
            Update((float)(gameTime.ElapsedGameTime.Milliseconds + 1));
        }

        public void Update(float time)
        {
            bodyUpdateCount = new short[Bodies.Count];
         
            Speed = (time * SpeedCoef);
            for (int i = 0; i < Bodies.Count; i++)
                UpdateBody(i);
        }

        void UpdateBody(int i)
        {
            if (MaxUpdatePerBody < ++bodyUpdateCount[i]) return;
            Debug.Assert(bodyUpdateCount[i] != 2);

            Vector2 a, v, p, r;
            var body = Bodies[i];
            //if (body.IsStatic) return;
            
            a = body.Force * body.InverseMass -
                (body.AttachToGravity ?
                new Vector2(Gravity.X * body.GravityNormal.X, Gravity.Y * body.GravityNormal.Y) :
                Vector2.Zero);
            v = a * Speed + body.Velocity;
            Vector2 position, size;
            body.Mesh.GetPositionAndSize(out position, out size);

            v.X = MathHelper.Clamp(v.X, -body.MaxSpeed.X, body.MaxSpeed.X);
            v.Y = MathHelper.Clamp(v.Y, -body.MaxSpeed.Y, body.MaxSpeed.Y);

            r = v * Speed;
            //r = 0.5f * a * Speed * Speed + body.Velocity * Speed;
            p = r + position;
            //Polygon nm = new Polygon(body.Mesh);
            body.Mesh.AutoTriangulate = true;
            if (body.Mesh.Triangles == null) body.Mesh.Triangulate();
            body.Mesh.Offset(r);
            
            if (body.IsStatic) return;

            if (!body.IsFree && !body.IsStatic)
            {
                for (int j = 0; j < Bodies.Count; j++)
                {
                    var item = Bodies[j];
                    if (i == j) continue;

                    var push = Vector2.Zero;
                    bool collide = false;
                    if (item.Convex && body.Convex)
                    {
                        if (Polygon.Collide(body.Mesh, item.Mesh, out push))
                        {
                            Vector2 pd = push;
                            pd.Normalize();
                            pd = push - AllowedPenetrationDepth * pd;
                            if (body.PreventSlippingOnSlopes)
                                if (Math.Abs(pd.X) < StickCoef) pd.X = 0;
                            if (GeometryHelper.IsNaN(pd)) pd = Vector2.Zero;
                            /*
                             * EXPERIMENT:
                             * Move both bodies based on their weight
                             */
                            else if (item.IsStatic)
                            {
                                if (!item.IsFree) body.Mesh.Offset(pd);
                                if (body.Collide != null) body.Collide(item.Entity, pd);
                            }
                            else
                            {
                                //Lerp between masses

                                var massSum = body.InverseMass + item.InverseMass;
                                var bodyW = body.InverseMass / massSum;
                                var itemW = bodyW - 1.0f;
                                var bodyPV = pd * bodyW;
                                var itemPV = pd * itemW;
                                
                                if (!item.IsFree)
                                {
                                    UpdateBody(j);
                                    body.Mesh.Offset(bodyPV);
                                }
                                if (body.Collide != null) body.Collide(item.Entity, bodyPV);
                                if (item.Collide != null) item.Collide(body.Entity, itemPV);
                            }
                        }
                    }
                    else
                    {
                    #region SAT With Triangles
                        foreach (var tri1 in body.Mesh.Triangles)
                        {
                            if (item.Mesh.Triangles == null)
                                item.Mesh.Triangulate();
                            foreach (var tri2 in item.Mesh.Triangles)
                            {
                                var p1 = tri1.ToPolygon();
                                var p2 = tri2.ToPolygon();
                                var push = Vector2.Zero;

                                if (Polygon.Collide(p1, p2, out push))
                                {
                                    Vector2 pd = push;
                                    pd.Normalize();
                                    pd = push - AllowedPenetrationDepth * pd;
                                    if (body.PreventSlippingOnSlopes)
                                        if (Math.Abs(pd.X) < StickCoef) pd.X = 0;
                                    /*
                                     * EXPERIMENT:
                                     * Move both bodies based on their weight
                                     */
                                    if (body.IsStatic) pd = Vector2.Zero;
                                    else if (item.IsStatic)
                                    {
                                        body.Mesh.Offset(pd);
                                        if (body.Collide != null) body.Collide(item.Entity, pd);
                                    }
                                    else
                                    {
                                        //Lerp between masses
                                        var massSum = body.InverseMass + item.InverseMass;
                                        var bodyW = body.InverseMass / massSum;
                                        var itemW = bodyW - 1.0f;
                                        var bodyPV = pd * bodyW;
                                        var itemPV = pd * itemW;
                                        body.Mesh.Offset(bodyPV);
                                        item.Mesh.Offset(itemPV);
                                        item.Collide(body.Entity, itemPV);
                                        body.Collide(item.Entity, bodyPV);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }

            var newP = body.Mesh.GetPosition();
            var movedDistance = newP - p;
            if (movedDistance.Length() > _bigNumber) //We don't want no weird teleports, do we?
                body.Mesh.Offset(-movedDistance); //rollback
            else
                v += movedDistance / Speed;
            body.Velocity = v * DampingCoef;
            body.Acceleration = a;

            /////////////////////
            // Check for NaNs generated by bugs
            /////////////////////
            if (GeometryHelper.IsNaN(body.Velocity)) body.Velocity = Vector2.Zero;
            if (GeometryHelper.IsNaN(body.Acceleration)) body.Acceleration = Vector2.Zero;

            if (body.MaxSpeed.X >= 0 && Math.Abs(body.Velocity.X) > body.MaxSpeed.X)
                body.Velocity.X = body.MaxSpeed.X * Math.Sign(body.Velocity.X);
            if (body.MaxSpeed.Y >= 0 && Math.Abs(body.Velocity.Y) > body.MaxSpeed.Y)
                body.Velocity.Y = body.MaxSpeed.Y * Math.Sign(body.Velocity.Y);

            if (body.MaxAcceleration.X >= 0 && Math.Abs(body.Acceleration.X) > body.MaxAcceleration.X)
                body.Acceleration.X = body.MaxAcceleration.X * Math.Sign(body.Acceleration.X);
            if (body.MaxAcceleration.Y >= 0 && Math.Abs(body.Velocity.Y) > body.MaxAcceleration.Y)
                body.Acceleration.Y = body.MaxAcceleration.Y * Math.Sign(body.Acceleration.Y);

            body.Force = Vector2.Zero;
        }

        public void Collide(Body A, Body B, Triangle tA, Triangle tB)
        {
            if (A.IsStatic && B.IsStatic) return;
            Vector2 MTD, N;
            float t = Speed;

            if (Polygon.Collide(tA.ToPolygon(), tB.ToPolygon(), A.Velocity, B.Velocity, out N, ref t))
            {
                if (t < 0.0f)
                {
                    MTD = N * -t;
                    //objects are overlapped, push them away.
                    if (A.IsStatic)
                    {
                        B.Mesh.Offset(-MTD);
                    }
                    else if (B.IsStatic)
                    {
                        A.Mesh.Offset(MTD);
                    }
                    else
                    {
                        //TODO: Implement Push
                        var aw = A.Mass / (A.Mass + B.Mass);
                        A.Mesh.Offset(MTD * aw);
                        B.Mesh.Offset(MTD * (1 - aw));
                    }

                    N = MTD;
                    N.Normalize();
                    t = 0.0f;
                }
                //process collision
                var V = A.Velocity - B.Velocity;
                var n = Vector2.Dot(V, N);
                Vector2 Dn = N * n;
                Vector2 Dt = V - Dn;

                if (n > 0.0f) Dn = Vector2.Zero;

                float dt = Dt.LengthSquared();

                float m = A.Mass + B.Mass;
                float r0 = A.Mass / m;
                float r1 = B.Mass / m;

                A.Velocity += V * r0;
                B.Velocity -= V * r1;
            }
        }

        Vector2 FindIntersectionPoint(Vector2 vNew, Vector2 vOld, Triangle tri)
        {
            var ip = GeometryHelper.GetIntersectionPoint(vNew, vOld, tri.A, tri.B);
            if (!float.IsNaN(ip.X)) return ip;

            ip = GeometryHelper.GetIntersectionPoint(vNew, vOld, tri.B, tri.C);
            if (!float.IsNaN(ip.X)) return ip;

            ip = GeometryHelper.GetIntersectionPoint(vNew, vOld, tri.C, tri.A);
            return ip;
        }

        bool TrianglesIntersect(Triangle a, Triangle b)
        {
            return a.IsInside(b.A) || a.IsInside(b.B) || a.IsInside(b.C) ||
                b.IsInside(a.A) || b.IsInside(a.B) || b.IsInside(a.C);
        }

        bool PolygonsIntersect(Polygon a, Polygon b)
        {
            foreach (var i in a.Triangles)
                foreach (var j in b.Triangles)
                    if (TrianglesIntersect(i, j)) return true;
            return false;
        }

        public bool ShowTriangles;
        public void Draw(SpriteBatch spriteBatch, LineBrush lb, Color color, Vector2 offset, bool? showTriangles=null)
        {
            if ((showTriangles.HasValue && showTriangles.Value) || ShowTriangles)
            {
                foreach (var item in Bodies)
                    if (item.Mesh.Triangles != null)
                        foreach (var tri in item.Mesh.Triangles)
                            tri.Draw(spriteBatch, lb, offset, color);
            }
            else
                foreach (var item in Bodies)
                    item.Mesh.Draw(spriteBatch, lb, offset, color);
        }

        #region Console Commands
        public void AttachToConsole()
        {
            if (console == null) return;
            console.AddCommand("ph_gravity", ph_gravity);
            console.AddCommand("ph_speed", ph_speed);
            console.AddCommand("ph_selectbody", ph_selectbody);
            console.AddCommand("ph_removebody", ph_removebody);
            console.AddCommand("ph_newbody", ph_newbody);
            console.AddCommand("ph_update", ph_update);
            console.AddCommand("ph_stickiness", ph_stickiness);
            console.AddCommand("ph_penetration", ph_penetration);
            console.AddCommand("ph_damp", ph_damp);
            console.AddCommand("ph_showtriangles", ph_showtriangles);
        }

        void ph_gravity(IList<string> args)
        {
            if (args.Count > 1) Gravity.Y = float.Parse(args[1]);
            else console.WriteLine(Gravity.Y.ToString());
        }

        void ph_speed(IList<string> args)
        {
            if (args.Count > 1) SpeedCoef = float.Parse(args[1]);
            else console.WriteLine(SpeedCoef.ToString());
        }

        void ph_penetration(IList<string> args)
        {
            if (args.Count > 1) AllowedPenetrationDepth = float.Parse(args[1]);
            else console.WriteLine(AllowedPenetrationDepth.ToString());
        }

        void ph_damp(IList<string> args)
        {
            if (args.Count > 1) DampingCoef = float.Parse(args[1]);
            else console.WriteLine(DampingCoef.ToString());
        }

        void ph_stickiness(IList<string> args)
        {
            if (args.Count > 1) StickCoef = float.Parse(args[1]);
            else console.WriteLine(StickCoef.ToString());
        }

        void ph_selectbody(IList<string> args)
        {
            Bodies[int.Parse(args[1])].AttachToConsole();
        }

        void ph_removebody(IList<string> args)
        {
            Bodies.RemoveAt(int.Parse(args[1]));
        }

        void ph_newbody(IList<string> args)
        {
            List<Vector2> points = new List<Vector2>();
            for (int i = 1; i < args.Count; i++)
                points.Add(GeometryHelper.String2Vector(args[i]));
            Body b = new Body(this, game, new Polygon(points), 100);
            b.Mesh.AutoTriangulate = true;
            b.Mesh.Triangulate();
            b.IsFree = true;
            b.AttachToConsole();
        }

        void ph_update(IList<string> args)
        {
            if (args.Count > 1) Update(float.Parse(args[1]));
            else Update(16);
        }

        void ph_showtriangles(IList<string> args)
        {
            if (args.Count > 1) ShowTriangles = bool.Parse(args[1]);
            else console.WriteLine(ShowTriangles.ToString());
        }
        #endregion
    }
}
