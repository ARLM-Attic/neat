using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neat;
using Neat.Mathematics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Neat.Game.SideScroller
{
    public class Entity
    {
        protected NeatGame game;
        protected World2d world;
        public Body body;
        protected StateMachine states;
        protected Dictionary<string, string> textures;
        protected Dictionary<string, Action> actions;
        protected string texture;

        public bool RemoveInNextCycle = false;
        
        //TODO: Implement Constructors
        //TODO: Implement Entity.AttachToConsole with en_

        public virtual void Initialize()
        {
            textures = new Dictionary<string, string>();
            actions = new Dictionary<string, Action>();
            states = new StateMachine();
            states.Add("generic", new State(state_generic_activate));
            textures.Add("generic", "entity_generic");
        }

        void state_generic_activate()
        {
            texture = textures["generic"];
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Shot(Projectile p)
        {
        }

        public virtual void Touch(Entity e)
        {
        }

        public virtual void Blocked(Polygon w) //blocked by wall
        {
        }

        public virtual void Move(Vector2 direction)
        {
        }

        public virtual void Render(SpriteBatch spriteBatch, Vector2 offset)
        {
        }
    }
}
