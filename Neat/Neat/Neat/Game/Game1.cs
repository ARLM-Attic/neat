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
using Microsoft.Xna.Framework.Media;
using Neat;

namespace Neat
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class NeatDefault : NeatGame
    {
        public NeatDefault()
        {
            GameWidth = 800;
            GameHeight = 600;
            FullScreen = false;
        }

        protected override void Initialize()
        {
            base.Initialize();
            HasConsole = true;
            ConsoleKey = Keys.OemTilde;
            Graphics.ApplyChanges();
            Window.Title = "THIS IS NEAT.";
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Behave(GameTime gameTime)
        {
            if (IsTapped(Keys.F11))
                Graphics.ToggleFullScreen();
            if (IsTapped(Keys.Pause))
                IsPaused = !IsPaused;
            base.Behave(gameTime);
        }

        protected override void Render(GameTime gameTime)
        {
            base.Render(gameTime);
        }

        public override void AddScreens()
        {
            base.AddScreens();
            Screens["mainmenu"] = new StartScreen(this);
        }
    }
}
