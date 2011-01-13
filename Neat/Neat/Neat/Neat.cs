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
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
using System.IO;
 

namespace Neat
{
    public enum GameScreenNames
    {
        MainMenu = 0,
        QuitConfirm = 1,
        Options = 2,
        InGame = 3,
        Game = 4
    }

    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {

        public Neat.Components.Console Console;
        public bool HasConsole = true;
        public Keys ConsoleKey = Keys.OemTilde;
#if WINDOWS
        public bool IsWideScreen()
        {
            return (GraphicsDevice.DisplayMode.AspectRatio > 1.5);
        }
#endif

        public bool Freezed = false;

#if LIVE
        public bool ForceSignIn = true;
#endif
        public RAM Ram;

        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public Random RandomGenerator;

#if LIVE
        public NetworkHelper networkHelper;
#endif

        public Dictionary<string,Screen> Screens;
        public string ActiveScreen;

#if WINDOWS_PHONE
        public int GameWidth
        {
            get{return Window.ClientBounds.Width;}
            set{}
        }
        public int GameHeight
        {
            get{return Window.ClientBounds.Height;}
            set{}
        }
#else
        public int 
            GameWidth   =   1024,
            GameHeight  =   768;
#endif
        public bool FullScreen = false;

#if WINDOWS
        public virtual void InitializeGraphics()
        {
            GraphicsDevice.Reset();

            if (StretchMode == StretchModes.None)
            {
                Graphics.PreferredBackBufferWidth = GameWidth;
                Graphics.PreferredBackBufferHeight = GameHeight;
            }
            else
            {
                Graphics.PreferredBackBufferWidth = OutputResolution.X;
                Graphics.PreferredBackBufferHeight = OutputResolution.Y;
                ResetRenderTarget();
            }

            Graphics.IsFullScreen = FullScreen;
            Graphics.ApplyChanges();
        }

        public void ToggleFullScreen()
        {
            Graphics.ToggleFullScreen();
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
        bool standAlone = true;
        public NeatGame()
        {
            Ram = new RAM(this);
            
            videos = new Dictionary<string, Video>();
            videoPlayers = new List<VideoPlayer>();
            sounds = new Dictionary<string, SoundEffect>() ;
            songs = new Dictionary<string, Song>();
            effects = new Dictionary<string, Effect>();
            fonts = new Dictionary<string, SpriteFont>();

            Console = new Neat.Components.Console(this);
            Components.Add(Console);
            
#if LIVE
            Components.Add(new GamerServicesComponent(this));
#endif

            Graphics = new GraphicsDeviceManager(this);
            RandomGenerator = new Random();
            Content.RootDirectory = "Content";

            SetFrameRate(60);

            Screens = new Dictionary<string, Screen>();
            AddScreens();
        }

        public NeatGame(Game parent)
        {
            standAlone = false;
        }

        public void SetFrameRate(double f)
        {
            TargetElapsedTime = TimeSpan.FromSeconds(1 / f);
        }

        public virtual void FirstTime()
        {
        }

        public uint Frame = 0;
        public const uint MaxFrame = uint.MaxValue - 2;

        public void ActivateScreen(string screen)
        {
            Screens[screen].Activate();
            ActiveScreen = screen;
        }

        public SpriteFont NormalFont;

        protected override void BeginRun()
        {
            InitializeScreens();
            ActivateScreen("mainmenu");
            FirstTime();
            base.BeginRun();
        }

        protected override void Initialize()
        {
            Frame = 0;
            InitializeInput();
            if (standAlone)
            {
#if LIVE
            networkHelper = new NetworkHelper(this,gamestime);
#endif
                OutputResolution =
                    new Point(
                    Graphics.GraphicsDevice.DisplayMode.Width,
                    Graphics.GraphicsDevice.DisplayMode.Height);
                InitializeMessages();
                InitializeGraphics();

                SayMessage("ClientBounds: " + Window.ClientBounds.Width.ToString() + "x" + Window.ClientBounds.Height.ToString());
                SayMessage("Display Mode: " + Graphics.GraphicsDevice.DisplayMode.Width.ToString() + "x" + Graphics.GraphicsDevice.DisplayMode.Height.ToString());
                SayMessage("Aspect Ratio: " + Graphics.GraphicsDevice.DisplayMode.AspectRatio.ToString());

                base.Initialize();

                if (File.Exists("options.nsc")) Console.Run("call options.nsc");
            }
        }

        public virtual void InitializeScreens()
        {
            foreach (var p in Screens)
                p.Value.Initialize();
        }
    }
}
