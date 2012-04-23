using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Neat
{
    public class Transition
    {
        public static NeatGame Game
        {
            get
            {
                return EffectHandler.Game;
            }
            set
            {
                EffectHandler.Game = value;
            }
        }

        protected RenderTarget2D TargetA, TargetB;
        protected Effect Effect;
        protected TimeSpan Time;
        public TimeSpan Length; 

        public bool Finished { get; private set; }

        protected float Phase;
        
        public Transition()
        {
        }

        public virtual void Initialize(GameTime gameTime)
        {
            if (Length == TimeSpan.Zero) Length = new TimeSpan(0, 0, 1);
            Debug.WriteLine("Transition Initialized");
            TargetA = new RenderTarget2D(Game.GraphicsDevice, Game.GameWidth, Game.GameHeight,
                false, SurfaceFormat.Color, DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
            TargetB = new RenderTarget2D(Game.GraphicsDevice, Game.GameWidth, Game.GameHeight,
                false, SurfaceFormat.Color, DepthFormat.None, 1, RenderTargetUsage.PreserveContents);

           Time = TimeSpan.Zero;
           Finished = false;
        }

        public virtual void Deinitialize(GameTime gameTime)
        {
            Finished = true;
        }

        protected virtual void Update() { }

        public void Update(GameTime gameTime)
        {
            Time += gameTime.ElapsedGameTime;
            Phase = MathHelper.ToRadians(Time.Milliseconds);
            if (Time > Length) Deinitialize(gameTime);
            else Update();
        }

        public void Draw(GameTime gameTime)
        {
            if (Game.PreviousScreen == null || !Game.Screens.ContainsKey(Game.PreviousScreen) || Finished)
            {
                Game.RenderScreen(Game.ActiveScreen, gameTime);
            }
            else
            {
                Game.PushTarget(TargetA);
                if (!Game.AutoDraw) Game.SpriteBatch.Begin();
                Game.RenderScreen(Game.PreviousScreen, gameTime);
                Game.SpriteBatch.End();
                Game.PopTarget(false);

                Game.PushTarget(TargetB);
                Game.SpriteBatch.Begin();
                Game.RenderScreen(Game.ActiveScreen, gameTime);
                Game.SpriteBatch.End();
                Game.PopTarget();

                Draw();
            }
        }

        protected virtual void Draw() { }
    }
}
