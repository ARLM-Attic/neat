using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
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
 

namespace Neat
{
    public enum GamePartNames
    {
        MainMenu = 0,
        QuitConfirm = 1,
        Options = 2,
        InGame = 3,
        Game = 4
    }

    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {

        public Neat.Console.Console console;
        public bool hasConsole = true;
        public Keys consoleKey = Keys.OemTilde;
#if WINDOWS
        public bool IsWideScreen()
        {
            return (GraphicsDevice.DisplayMode.AspectRatio > 1.5);
        }
#endif

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

#if WINDOWS_PHONE
        public int gameWidth
        {
            get{return Window.ClientBounds.Width;}
            set{}
        }
        public int gameHeight
        {
            get{return Window.ClientBounds.Height;}
            set{}
        }
#else

        public int 
            gameWidth = 1024,
            gameHeight=768;
#endif
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
#if WINDOWS_PHONE
        public virtual void InitializeGraphics()
        {
            gameWidth = graphics.PreferredBackBufferWidth;
            gameHeight = graphics.PreferredBackBufferHeight;

            fullscreen = graphics.IsFullScreen;
        }
#endif
        //public GameWindow window { get { return Window; } }

        public NeatGame()
        {
            ram = new RAM(this);
#if !WINDOWS_PHONE
            
            videos = new Dictionary<string, Video>();
            videoPlayers = new List<VideoPlayer>();
#endif
            sounds = new Dictionary<string, SoundEffect>() ;
            songs = new Dictionary<string, Song>();
            effects = new Dictionary<string, Effect>();
            fonts = new Dictionary<string, SpriteFont>();

            console = new Neat.Console.Console(this);
#if XLIVE
            Components.Add(new GamerServicesComponent(this));
#endif
            graphics = new GraphicsDeviceManager(this);
            randomGenerator = new Random();
            Content.RootDirectory = "Content";


            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);
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
            InitializeGraphics();

            SayMessage("ClientBounds: " + Window.ClientBounds.Width.ToString() + "x" + Window.ClientBounds.Height.ToString());
            SayMessage("Display Mode: " + graphics.GraphicsDevice.DisplayMode.Width.ToString() + "x" + graphics.GraphicsDevice.DisplayMode.Height.ToString());
            SayMessage("Aspect Ratio: " + graphics.GraphicsDevice.DisplayMode.AspectRatio.ToString());

            base.Initialize();
        }

        public virtual void CreateParts()
        {
            parts.Add("mainmenu", new EasyMenus.MainMenu(this));
            parts.Add("quitconfirm", new EasyMenus.QuitConfirmationMenu(this));
            parts.Add("optionsmenu", new EasyMenus.OptionsMenu(this));
            parts.Add("ingamemenu", new EasyMenus.InGameMenu(this));
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

        #endregion
    }


}
