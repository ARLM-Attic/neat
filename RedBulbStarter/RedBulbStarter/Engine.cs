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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using RedBulb;

namespace RedBulbStarter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Engine : RedBulbGame
    {
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            showMouse = false;
            fullscreen = false;
            gameWidth = 800;
            gameHeight = 600;
            base.Initialize();

            console.Commands.Add("greet", e_greet);
        }

        void e_greet(IList<string> args)
        {
            console.WriteLine("Hello " + args[1]);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            InitializeGraphics();
        }

        public override void CreateParts()
        {
            parts["mainmenu"] = new GamePart1(this);
        }    

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Behave(GameTime gameTime)
        {
            base.Behave(gameTime);

            if (IsReleased(Keys.F11)) ToggleFullScreen();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Render(GameTime gameTime)
        {
            base.Render(gameTime);
        }
    }
}
