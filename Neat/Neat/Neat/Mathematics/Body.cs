using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using Neat.Graphics;

namespace Neat.Mathematics
{
    public class Body
    {
        public Action<object, Vector2> Collide = null;
        public Polygon Mesh;
        public bool IsStatic = true;
        public bool IsFree = false;
        //public bool Pushable = true;
        public bool AttachToGravity = true;
        public Vector2 Force;
        public Vector2 Acceleration;
        public Vector2 Velocity;
        //public float Elasticity = 0; //[0=no elasticity, 1]
        //public float FrictionCoef = 0.1f; //[0=no friction, 1]
        public Vector2 MaxSpeed = new Vector2(-1);
        public Vector2 MaxAcceleration = new Vector2(10);
        public Vector2 GravityNormal = new Vector2(0, -1);
        public bool PreventSlippingOnSlopes = true; //Hopefully this is just temporary and I'll find the right way to do this
        public bool Convex = false;
        float _mass = 0, _inverseMass = 0;
        Neat.Components.Console console;
        public float Mass
        {
            get
            {
                return _mass;
            }
            set
            {
                if (value != 0) _inverseMass = 1f / value;
                else
                {
                    _inverseMass = float.MaxValue - 1f;
                }
                _mass = value;
            }
        }
        public float InverseMass
        {
            get
            {
                return _inverseMass;
            }
            set
            {
                if (value != 0) _mass = 1f / value;
                else
                {
                    _mass = float.MaxValue - 1f;
                }
                _inverseMass = value;
            }
        }
        public object Entity;
        public PhysicsSimulator Simulator;
        public NeatGame Game;

        public Body(PhysicsSimulator i_sim, NeatGame i_game,
            object i_entity, Polygon i_mesh, float i_mass)
        {
            Simulator = i_sim;
            Game = i_game;
            Entity = i_entity;
            Mass = i_mass;
            Mesh = i_mesh;

            Simulator.Bodies.Add(this);
            Stop();
            Convex = Mesh.IsConvex();
        }

        public Body(PhysicsSimulator i_sim, NeatGame i_game,
            Polygon i_mesh, float i_mass)
        {
            Simulator = i_sim;
            Game = i_game;
            Mass = i_mass;
            Mesh = i_mesh;

            Simulator.Bodies.Add(this);
            Stop();
            Convex = Mesh.IsConvex();
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
            if (float.IsNaN(Velocity.X)) Velocity.X = 0;
            if (float.IsNaN(Velocity.Y)) Velocity.Y = 0;
        }

        #region Console Commands
        public void AttachToConsole()
        {
            if (Game == null) return;
            console = Game.Console;
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
            Simulator.Bodies.Remove(this);
        }

        void bd_attach(IList<string> args)
        {
            if (!Simulator.Bodies.Contains(this)) Simulator.Bodies.Add(this);
        }
        #endregion
    }
}