using System;
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
    public class Body
    {
        public Action<object> Collide = null; 
        public Polygon Mesh;
        public bool IsStatic = false;
        public bool IsFree = false;
        //public bool Pushable = true;
        public bool AttachToGravity = true;
        public Vector2 Force;
        public Vector2 Acceleration;
        public Vector2 Velocity;
        public float InverseMass = 0;
        //public float Elasticity = 0; //[0=no elasticity, 1]
        //public float FrictionCoef = 0.1f; //[0=no friction, 1]
        public Vector2 MaxSpeed = new Vector2(-1);
        public Vector2 MaxAcceleration = new Vector2(-1);
        public Vector2 GravityNormal = new Vector2(0, -1);
        public bool PreventSlippingOnSlopes = true; //Hopefully this is just temporary and I'll find the right way to do this
        float _mass = 0;
        Neat.Components.Console console;
        public float Mass
        {
            get
            {
                return _mass;
            }
            set
            {
                if (value != 0) InverseMass = 1f / value;
                else
                {
                    InverseMass = float.MaxValue - 1f;
                }
                _mass = value;
            }
        }

        public object entity;
        public PhysicsSimulator simulator;
        public NeatGame game;

        public Body(PhysicsSimulator i_sim, NeatGame i_game,
            object i_entity, Polygon i_mesh, float i_mass)
        {
            simulator = i_sim;
            game = i_game;
            entity = i_entity;
            Mass = i_mass;
            Mesh = i_mesh;

            simulator.Bodies.Add(this);
            Stop();
        }

        public Body(PhysicsSimulator i_sim, NeatGame i_game,
            Polygon i_mesh, float i_mass)
        {
            simulator = i_sim;
            game = i_game;
            Mass = i_mass;
            Mesh = i_mesh;

            simulator.Bodies.Add(this);
            Stop();
        }

        public Body()
        {
        }

        public void Stop()
        {
            Force = Vector2.Zero;
            Acceleration = Vector2.Zero;
            Velocity = Vector2.Zero;
        }

        public bool Moves
        {
            get
            {
                return Velocity != Vector2.Zero;
            }
        }

        public void ApplyForce(Vector2 f)
        {
            Force += f;
        }

        public void ApplyForce(float x, float y)
        {
            Force.X += x;
            Force.Y += y;
        }

        public void ApplyImpact(Vector2 p)
        {
            Velocity += p / Mass;
        }

        #region Console Commands
        public void AttachToConsole()
        {
            if (game == null) return;
            console = game.Console;
            console.AddCommand("bd_static", bd_static);
            console.AddCommand("bd_free", bd_free);
            console.AddCommand("bd_gravity", bd_gravity);
            console.AddCommand("bd_mass", bd_mass);
            console.AddCommand("bd_selectmesh", bd_selectmesh);
            console.AddCommand("bd_velocity", bd_velocity);
            console.AddCommand("bd_impact", bd_impact);
            console.AddCommand("bd_detach", bd_detach);
            console.AddCommand("bd_attach", bd_attach);
            console.AddCommand("bd_gravitynormal", bd_gravitynormal);
        }

        void bd_static(IList<string> args)
        {
            if (args.Count > 1) IsStatic = bool.Parse(args[1]);
            else console.WriteLine(IsStatic.ToString());
        }

        void bd_free(IList<string> args)
        {
            if (args.Count > 1) IsFree = bool.Parse(args[1]);
            else console.WriteLine(IsFree.ToString());
        }

        void bd_gravity(IList<string> args)
        {
            if (args.Count > 1) AttachToGravity = bool.Parse(args[1]);
            else console.WriteLine(AttachToGravity.ToString());
        }

        void bd_mass(IList<string> args)
        {
            if (args.Count > 1) Mass = float.Parse(args[1]);
            else console.WriteLine(Mass.ToString());
        }

        void bd_selectmesh(IList<string> args)
        {
            //TODO: Implement bd_selectmesh
        }

        void bd_velocity(IList<string> args)
        {
            if (args.Count > 1) Velocity = GeometryHelper.String2Vector(args[1]);
            else console.WriteLine(GeometryHelper.Vector2String(Velocity));
        }

        void bd_gravitynormal(IList<string> args)
        {
            if (args.Count > 1) GravityNormal = GeometryHelper.String2Vector(args[1]);
            else console.WriteLine(GeometryHelper.Vector2String(GravityNormal));
        }

        void bd_impact(IList<string> args)
        {
            ApplyImpact(GeometryHelper.String2Vector(args[1]));
        }

        void bd_detach(IList<string> args)
        {
            simulator.Bodies.Remove(this);
        }

        void bd_attach(IList<string> args)
        {
            if (!simulator.Bodies.Contains(this)) simulator.Bodies.Add(this);
        }
        #endregion
    }

    public class PhysicsSimulator : GameComponent
    {
        const float _epsilon = 1f;
        const float _bigNumber = 100f;
        Vector2 negativeOne = new Vector2(-1);

        public float AllowedPenetrationDepth = -0.1f;
        public float DampingCoef = 0.95f;
        public Vector2 Gravity = new Vector2(0, 0.98f);
        public List<Body> Bodies = new List<Body>();
        public float SpeedCoef = 0.025f;
        public float StickCoef = 0.8f;
        
        float speed;
        public NeatGame game;

        Neat.Components.Console console { get { return game.Console; } set { game.Console = value; } }

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
            Vector2 a, v, p, r;
            speed = (time * SpeedCoef);
            for (int i = 0; i < Bodies.Count; i++)
            {
                var body = Bodies[i];
                if (body.IsStatic) continue;
                a = body.Force * body.InverseMass -
                    (body.AttachToGravity ? 
                    new Vector2(Gravity.X * body.GravityNormal.X, Gravity.Y * body.GravityNormal.Y) : 
                    Vector2.Zero);
                v = a * speed + body.Velocity;
                Vector2 position,size;
                body.Mesh.GetPositionAndSize(out position, out size);

                r = v * speed;
                //r = 0.5f * a * Speed * Speed + body.Velocity * Speed;
                p = r + position;
                //Polygon nm = new Polygon(body.Mesh);
                body.Mesh.AutoTriangulate = true;
                body.Mesh.Offset(r);

                if (!body.IsFree)
                {
                    for (int j = 0; j < Bodies.Count; j++)
                    {
                        var item = Bodies[j];

                        #region SAT
                        if (i == j) continue;
                        foreach (var tri1 in body.Mesh.Triangles)
                        {
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
                                    body.Mesh.Offset(pd);
                                    if (body.Collide != null) body.Collide(item.entity);
                                }
                            }
                        }
                        #endregion
                    }
                }

                var newP = body.Mesh.GetPosition();
                var movedDistance = newP-p;
                if (movedDistance.Length() > _bigNumber) //We don't want no weird teleports, do we?
                    body.Mesh.Offset(-movedDistance); //rollback
                else
                    v += movedDistance / speed;
                body.Velocity = v*DampingCoef;
                body.Acceleration = a;
                
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
        }

        public void Collide(Body A, Body B, Triangle tA, Triangle tB)
        {
            if (A.IsStatic && B.IsStatic) return;
            Vector2 MTD, N;
            float t = speed;

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

        public void Draw(SpriteBatch spriteBatch, LineBrush lb, Color color, Vector2 offset, bool showTriangles=false)
        {
            if (showTriangles)
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
            console.AddCommand("ph_gravity", ph_gravity);
            console.AddCommand("ph_speed", ph_speed);
            console.AddCommand("ph_selectbody", ph_selectbody);
            console.AddCommand("ph_removebody", ph_removebody);
            console.AddCommand("ph_newbody", ph_newbody);
            console.AddCommand("ph_update", ph_update);
            console.AddCommand("ph_stickiness", ph_stickiness);
            console.AddCommand("ph_penetration", ph_penetration);
            console.AddCommand("ph_damp", ph_damp);
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
        #endregion
    }
}
