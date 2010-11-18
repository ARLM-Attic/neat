using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Neat.Mathematics
{
    public class Body
    {
        public bool IsStatic = false;
        public bool IsFree = false;
        public bool Pushable = true;
        public bool AttachToGravity = true;
        public Vector2 Force;
        public Vector2 Acceleration;
        public Vector2 Velocity;
        public float InverseMass = 0;
        public float Elasticity = 0; //[0=no elasticity, 1]
        public float FrictionCoef = 0.1f; //[0=no friction, 1]
        float _mass = 0;
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

        public Polygon Mesh;

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

        
    }

    public class PhysicsSimulator
    {
        const float _epsilon = 1f;
        const float _allowedPenetrationDepth = -0.1f;
        const float _bigNumber = 100f;
        const float _dampingCoEf = 0.95f;
        public Vector2 Gravity = new Vector2(0, -0.98f);
        public List<Body> Bodies = new List<Body>();
        public float Speed = 0.7f;

        public NeatGame game;

        Neat.Components.Console console { get { return game.Console; } set { game.Console = value; } }

        public PhysicsSimulator(NeatGame i_game)
        {
            game = i_game;

            Initialize();
        }

        public void Initialize()
        {
        }

        public void Update()
        {
            Vector2 a, v, p, r;
            for (int i = 0; i < Bodies.Count; i++)
            {
                var body = Bodies[i];
                if (body.IsStatic) continue;
                a = body.Force * body.InverseMass -
                    (body.AttachToGravity ? Gravity : Vector2.Zero);
                v = a * Speed + body.Velocity;
                Vector2 position,size;
                body.Mesh.GetPositionAndSize(out position, out size);

                r = v * Speed;
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
                                    body.Mesh.Offset(push-_allowedPenetrationDepth*pd);
                                }
                            }
                        }
                        #endregion
                    }
                }

                var newP = body.Mesh.GetPosition();
                var movedDistance = newP-p;
                if (movedDistance.Length() > _bigNumber)
                    body.Mesh.Offset(-movedDistance); //rollback
                v += movedDistance / Speed;
                body.Velocity = v*_dampingCoEf;
                body.Acceleration = a;
                body.Force = Vector2.Zero;
            }
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
    }
}
