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
 
 
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
using Neat.GUI;
 

namespace Neat
{
    public class Screen
    {
#region Fields
        protected SpriteBatch SpriteBatch { get { return game.SpriteBatch; } set { game.SpriteBatch = value; } }
        protected NeatGame game;
        public GameTime gamesTime;
        public uint Frame { get { return game.Frame; }}
        public Form Form;
        public List<GameComponent> Components = new List<GameComponent>();
        public EffectChain EffectChain;
        public bool RenderFormWithEffects = true;
        public bool InputEnabled = true;
#endregion

#region Initialize
        public Screen(NeatGame Game)
        {
            game = Game;
        }

        public virtual void Initialize()
        {
            Form = new Form(game);
            if (EffectChain == null) EffectChain = new EffectChain(game);
            foreach (var item in Components)
            {
                item.Initialize();
            }

            EffectHandler.Game = game;
        }

        public virtual void Activate()
        {
            if (game.HasConsole && Form != null) Form.AttachToConsole();
#if KINECT
            game.Touch.Reset();
#endif
        }

        public virtual void Deactivate(string nextScreen)
        {
        }
#endregion

#region Files
        public virtual void LoadContent()
        {
            if (EffectChain == null) EffectChain = new EffectChain(game);
        }
#endregion 

#region Loop
        public virtual void Update(GameTime gameTime, bool ForceInputEnable=true)
        {
            if (InputEnabled && ForceInputEnable) HandleInput(gameTime);
            Behave(gameTime);
            foreach (var item in Components)
            {
                item.Update(gameTime);
            }
            foreach (var item in EffectChain)
            {
                item.Update(gameTime);
            }
            Form.Update(gameTime);
        }

        public virtual void Behave(GameTime gameTime)
        {
        }
#endregion

#region Render
        public virtual void BeforeRender(GameTime gameTime)
        {
            if (EffectChain != null) EffectChain.Begin(gameTime);
        }

        public virtual void Render(GameTime gameTime)
        {
        }

        public virtual void AfterRender(GameTime gameTime)
        {
            if (EffectChain != null)
            {
                if (RenderFormWithEffects) Form.Draw(gameTime);
                EffectChain.End(gameTime);
                if (!RenderFormWithEffects) Form.Draw(gameTime);
            }
            else Form.Draw(gameTime);
        }
#endregion

#region Input
        public virtual void HandleInput(GameTime gameTime)
        {
        }
#endregion

    }
}
