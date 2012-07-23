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
using System.Runtime.Serialization;

namespace Neat.Mathematics
{
    [Serializable]
    public class Body : ISerializable
    {
        [NonSerialized]
        public Action<object, Vector2> Collide = null;
        public Polygon Mesh { get; set; }
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
        public object Entity;

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

        public HashSet<Body> NotCollideWith = new HashSet<Body>();

        [NonSerialized]
        public PhysicsSimulator Simulator;
        public NeatGame Game { get { return PhysicsSimulator.Game; } }
        Neat.Components.Console console { get { return Game.Console; } }

        protected Body(SerializationInfo info, StreamingContext context)
        {
            //Collide = (Action<object, Vector2>)info.GetValue("Collide", typeof(Action<object, Vector2>));
            Mesh = (Polygon)info.GetValue("Mesh", typeof(Polygon));
            IsStatic = info.GetBoolean("IsStatic");
            IsFree = info.GetBoolean("IsFree");
            AttachToGravity = info.GetBoolean("AttachToGravity");
            Force = (Vector2)info.GetValue("Force", typeof(Vector2));
            Acceleration = (Vector2)info.GetValue("Acceleration", typeof(Vector2));
            Velocity = (Vector2)info.GetValue("Velocity", typeof(Vector2));
            MaxSpeed = (Vector2)info.GetValue("MaxSpeed", typeof(Vector2));
            MaxAcceleration = (Vector2)info.GetValue("MaxAcceleration", typeof(Vector2));
            GravityNormal = (Vector2)info.GetValue("GravityNormal", typeof(Vector2));
            PreventSlippingOnSlopes = info.GetBoolean("PreventSlippingOnSlopes");
            Convex = info.GetBoolean("Convex");
            Entity = info.GetValue("Entity", typeof(object));
            _mass = info.GetSingle("_mass");
            _inverseMass = info.GetSingle("_inverseMass");
            Body[] ncwArray = (Body[])info.GetValue("NotCollideWith", typeof(Body[]));
            NotCollideWith = new HashSet<Body>(ncwArray);
            //NotCollideWith = (HashSet<Body>)info.GetValue("NotCollideWith", typeof(HashSet<Body>));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue("Collide", Collide, Collide.GetType());
            info.AddValue("Mesh", Mesh, Mesh.GetType());
            info.AddValue("IsStatic", IsStatic);
            info.AddValue("IsFree", IsFree);
            info.AddValue("AttachToGravity", AttachToGravity);
            info.AddValue("Force", Force, Force.GetType());
            info.AddValue("Acceleration", Acceleration, Acceleration.GetType());
            info.AddValue("Velocity", Velocity, Velocity.GetType());
            info.AddValue("MaxSpeed", MaxSpeed, MaxSpeed.GetType());
            info.AddValue("MaxAcceleration", MaxAcceleration, MaxAcceleration.GetType());
            info.AddValue("GravityNormal", GravityNormal, GravityNormal.GetType());
            info.AddValue("PreventSlippingOnSlopes", PreventSlippingOnSlopes);
            info.AddValue("Convex", Convex);
            info.AddValue("Entity", Entity, typeof(object));
            info.AddValue("_mass", _mass);
            info.AddValue("_inverseMass", _inverseMass);
            info.AddValue("NotCollideWith", NotCollideWith.ToArray(), typeof(Body[]));
            //info.AddValue("NotCollideWith", NotCollideWith, NotCollideWith.GetType());
        }

        public Body()
        {
        }

        public Body(PhysicsSimulator i_sim, 
            object i_entity, Polygon i_mesh, float i_mass)
        {
            Simulator = i_sim;
            Entity = i_entity;
            Mass = i_mass;
            Mesh = i_mesh;

            Simulator.Bodies.Add(this);
            Stop();
            Convex = Mesh.IsConvex();
        }

        public Body(PhysicsSimulator i_sim, 
            Polygon i_mesh, float i_mass)
        {
            Simulator = i_sim;
            Mass = i_mass;
            Mesh = i_mesh;

            Simulator.Bodies.Add(this);
            Stop();
            Convex = Mesh.IsConvex();
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
            console.AddCommand("bd_rect", o => console.WriteLine(Mesh.IsRectangle().ToString()));
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