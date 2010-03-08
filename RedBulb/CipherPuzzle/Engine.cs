using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using RedBulb;
using RedBulb.MenuSystem;
namespace CipherPuzzle
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class Game1 : RedBulbGame 
    {
        protected override void Initialize()
        {
            fullscreen = true;
            backGroundColor = Color.Black;
            if (GraphicsDevice.DisplayMode.Width < 1280)
            {
                gameWidth = 1024;
                gameHeight = 768;
            }
            else
            {
                gameWidth = 1280;
                if (IsWideScreen()) gameHeight = 800;
                else { gameWidth = 1024; gameHeight = 768; }
            }
            InitializeGraphics();
            base.Initialize();
        }
        public override void CreateParts()
        {
            parts = new Dictionary<string, GamePart>();
            parts.Add("mainmenu", new MainMenu(this));
            parts.Add("options", new Options(this));
            parts.Add("game", new GamePart1(this)); //0
        }    

        protected override void Behave(GameTime gameTime)
        {
            base.Behave(gameTime);
        }        

        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadFont(@"Sprites\Fonts\cipherFont");
            LoadFont(@"Sprites\Fonts\LetterFont");
            LoadFont(@"Sprites\Fonts\menuFont");
            LoadTexture(@"Sprites\winBG");
            LoadTexture(@"Sprites\background");
            LoadTexture(@"Sprites\cipherBG");
            LoadTexture(@"Sprites\letterBG");
            LoadTexture(@"Sprites\border");
            LoadTexture(@"Sprites\mainmenu"); LoadTexture(@"Sprites\mainmenu43"); LoadTexture(@"Sprites\mainmenuw");
            LoadTexture(@"Sprites\menubg");
            LoadTexture(@"Sprites\button_2");
        }

        protected override void Render(GameTime gameTime)
        {
            base.Render(gameTime);
        }
    }
}
