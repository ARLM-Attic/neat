using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Neat;

namespace NeatStarter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class NeatStarterGame : NeatGame
    {
        public NeatStarterGame()
        {
            gameWidth = 800;
            gameHeight = 600;
            fullscreen = false;
        }

        protected override void Initialize()
        {
            base.Initialize();
            hasConsole = true;
            consoleKey = Keys.OemTilde;
            graphics.ApplyChanges();
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
                graphics.ToggleFullScreen();
            if (IsTapped(Keys.Pause))
                IsPaused = !IsPaused;
            base.Behave(gameTime);
        }

        protected override void Render(GameTime gameTime)
        {
            base.Render(gameTime);
        }

        public override void CreateParts()
        {
            base.CreateParts();
            parts["mainmenu"] = new StartScreen(this);
        }
    }
}
