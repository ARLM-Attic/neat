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
using RedBulb;
using Microsoft.Xna.Framework.Media;
using RedBulb.MenuSystem;
namespace Tree
{
    enum GameState { Menu, InGameMenu, Game, OptionsMenu, Credits}
    public partial class Massacre : RedBulbGame
    {
        GameState gameState = GameState.Menu;
        List<Song> Songs;
        public Massacre()
        {
        }

        /// </summary>
        protected override void Initialize()
        {
#if ZUNE
            GameWidth = 240 ;
            GameHeight = 320;
#endif
            fullscreen = true;
            // TODO: Add your initialization logic here
            forceSignIn = false;
            
#if WINDOWS
            gameWidth = 1280;
            gameHeight = 960;
            if (IsWideScreen()) gameHeight = 800;
            InitializeGraphics();
#endif
            EnableMainMenu();
            showMouse = false;
            InitializeGame();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            // TODO: use this.Content to load your game content here
            LoadTexture("Sprites\\Game\\pL", "playerLeft");
            LoadTexture("Sprites\\Game\\pR", "playerRight");
            LoadTexture("Sprites\\Game\\pS", "playerStraight");
            LoadTexture("Sprites\\Game\\Tree", "Tree");
            LoadTexture("Sprites\\logo", "logo1");
            LoadTexture("Sprites\\Game\\keys", "keys");
            LoadTexture("Sprites\\xmas", "xmas");
            LoadTexture("Sprites\\creds", "credits");
            LoadTexture("Sprites\\background", "menuBackground");
            LoadTexture(@"Sprites\Game\Bars");
            LoadTexture(@"Sprites\Game\GameBG");
            LoadFont(@"Sprites\Fonts\gamefont");
            LoadTexture(@"Sprites\Game\zapper1","zapper");
            LoadFont(@"Sprites\Fonts\menuFont2");

            Songs = new List<Song>();
            Songs.Add(Content.Load<Song>(@"Sounds\Songs\jingle-bells"));
            InitializeMenus();

         
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        int activeSong = 0;
        protected override void Behave(GameTime gameTime)
        {
            gamesTime = gameTime;
            if (MediaPlayer.State == MediaState.Stopped)
            {
                MediaPlayer.Play(Songs[activeSong++]);
                if (activeSong >= Songs.Count) activeSong = 0;
            }
                switch (gameState)
                {
                    case GameState.Menu:
                        UpdateMenu(gameTime);
                        break;
                    case GameState.InGameMenu:
                        UpdateInGameMenu(gameTime);
                        break;
                    case GameState.Game:
                        UpdateGame(gameTime);
                        break;
                    case GameState.OptionsMenu:
                        UpdateOptionsMenu(gameTime);
                        break;
                    case GameState.Credits:
                        UpdateCredits(gameTime);
                        break;
                }
        }
        
        protected override void Render(GameTime gameTime)
        {
            if (gameState != GameState.Game || gameState != GameState.InGameMenu || gameState != GameState.Menu ||
                gameState != GameState.Credits)
            {
                spriteBatch.Draw(getTexture("menuBackground"), new Rectangle(0, 0, gameWidth, gameHeight), Color.White);
                spriteBatch.Draw(getTexture("logo1"), new Vector2(8, 8), Color.White);
                spriteBatch.Draw(getTexture("xmas"), new Vector2(gameWidth - 520, 60), Color.White);
            }
            switch (gameState)
            {
                case GameState.Menu:
                    DrawMenu(gameTime );
                    break;
                case GameState.InGameMenu:
                    DrawInGameMenu(gameTime);
                    break;
                case GameState.Game:
                    DrawGame(gameTime);
                    break;
                case GameState.OptionsMenu:
                    DrawOptionsMenu(gameTime);
                    break;
                case GameState.Credits:
                    DrawCredits(gameTime);
                    break;
            }
        }
    }


}
