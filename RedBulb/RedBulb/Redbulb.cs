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
using RedBulb.EasyMenus;

namespace RedBulb
{
    public enum GamePartNames
    {
        MainMenu = 0,
        QuitConfirm = 1,
        Options = 2,
        InGame = 3,
        Game = 4
    }

    public partial class RedBulbGame : Microsoft.Xna.Framework.Game
    {
        public RedBulb.Console.Console console;
        public bool hasConsole = true;
        public Keys consoleKey = Keys.OemTilde;
        public bool IsWideScreen()
        {
            return (GraphicsDevice.DisplayMode.AspectRatio > 1.5);
        }
        public bool freezed = false;
        public bool forceSignIn = true;

        public RAM ram;

        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public Random randomGenerator;

#if XLIVE
        public NetworkHelper networkHelper;
#endif

        public Dictionary<string,GamePart> parts;
        public string activePart;

        public int 
            gameWidth = 1024,
            gameHeight=768;

        public bool fullscreen = false;

#if WINDOWS
        public virtual void InitializeGraphics()
        {
            GraphicsDevice.Reset();
            
            graphics.PreferredBackBufferWidth = gameWidth;
            graphics.PreferredBackBufferHeight = gameHeight;
            
            graphics.IsFullScreen = fullscreen;
            
            graphics.ApplyChanges();
        }

        public void ToggleFullScreen()
        {
            graphics.ToggleFullScreen();
        }
#endif
        //public GameWindow window { get { return Window; } }

        public RedBulbGame()
        {
            ram = new RAM(this);
            console = new RedBulb.Console.Console(this);
            sounds = new Dictionary<string, SoundEffect>() ;
            songs = new Dictionary<string, Song>();
            effects = new Dictionary<string, Effect>();
            fonts = new Dictionary<string, SpriteFont>();
            videos = new Dictionary<string, Video>();
            videoPlayers = new List<VideoPlayer>();

#if XLIVE
            Components.Add(new GamerServicesComponent(this));
#endif
            graphics = new GraphicsDeviceManager(this);
            randomGenerator = new Random();
            Content.RootDirectory = "Content";
        }
        public virtual void FirstTime()
        {
        }

        bool isFirstTime = true;
        public uint frame = 0;
        public const uint maxFrame = 16777214;

        public void ActivatePart(string  part)
        {
            parts[part].Activate();
            activePart = part;
        }

        public SpriteFont normalFont;

        #region Logic
        protected override void Initialize()
        {
            frame = 0;

#if XLIVE
            networkHelper = new NetworkHelper(this,gamestime);
#endif

            InitializeInput();
            InitializeMessages();

            SayMessage("ClientBounds: " + Window.ClientBounds.Width.ToString() + "x" + Window.ClientBounds.Height.ToString());
            SayMessage("Display Mode: " + graphics.GraphicsDevice.DisplayMode.Width.ToString() + "x" + graphics.GraphicsDevice.DisplayMode.Height.ToString());
            SayMessage("Aspect Ratio: " + graphics.GraphicsDevice.DisplayMode.AspectRatio.ToString());

            base.Initialize();
        }

        public virtual void CreateParts()
        {
            parts.Add("mainmenu", new MainMenu(this));
            parts.Add("quitconfirm", new QuitConfirmationMenu(this));
            parts.Add("optionsmenu", new OptionsMenu(this));
            parts.Add("ingamemenu", new InGameMenu(this));
        }

        public virtual void RestartGame()
        {
        }

        void InitializeParts()
        {
            foreach (var p in parts)
            {
                p.Value.Initialize();
            }
        }

        #endregion

        #region Debug Helpers
        public int GetCPS()
        {
            return (int)frame / (gamesTime.TotalGameTime.Seconds+1);
        }
        public float GetRealCPS()
        {
            return (float)frame / (float)gamesTime.TotalRealTime.TotalSeconds;
        }
        public void WriteCPS(Vector2 position)
        {
            Write("Update: @" + updateCPS.ToString() + "/ Draw:"+ 
                drawCPS.ToString() +" CPS"+(gamestime.IsRunningSlowly?" SLOW":"Normal"), position);
        }
        #endregion
    }


}
