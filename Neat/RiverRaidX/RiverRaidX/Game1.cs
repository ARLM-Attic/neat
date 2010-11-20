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
using Neat.Components;

namespace RiverRaidX
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : NeatGame
    {
        public Game1() : base()
        {
            GameWidth = 1280;
            GameHeight = 720;
            FullScreen = false;
            Window.Title = "River Raid X by Saeed Afshari";
            //Console.Run("call startup.nsc");
        }

        protected override void Initialize()
        {
            base.Initialize();
            HasConsole = true;
            SetFrameRate(30.0);
            ConsoleKey = Keys.OemTilde;
            
            Graphics.ApplyChanges();
            Console.AddCommand("g_start", NewGame);
        }

        void NewGame( IList<string> args)
        {
            ((RiverRaidGame)Screens["game"]).Reset();
            ActivateScreen("game");
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
            Screens["game"] = new RiverRaidGame(this); 
        }
    }
}
