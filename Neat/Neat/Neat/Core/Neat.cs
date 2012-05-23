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
using System.Diagnostics;

#if KINECT
    using Microsoft.Kinect;
#endif

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
        public static List<NeatGame> Games = new List<NeatGame>();

        public uint Frame = 0;
        public const uint MaxFrame = uint.MaxValue - 2;

        GraphicsDevice _graphicsDevice = null;
        ContentManager _content = null;
        public Components.TextEffects TextEffects = null;
        public Components.ElegantTextEngine ElegantTextEngine = null;

        public Neat.Components.Console Console;
        public bool HasConsole = true;
        public Keys ConsoleKey = Keys.OemTilde;

#if KINECT
        public KinectEngine Kinect;
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

        public Dictionary<string, Screen> Screens;
        public string ActiveScreen, PreviousScreen = null;
        
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
        int _gameWidth = 1024,
            _gameHeight = 768;

        public int GameWidth
        {
            get
            {
                return _gameWidth;
            }
            set
            {
                _gameWidth = value;
                ResetRenderTarget();
            }
        }
        public int GameHeight
        {
            get
            {
                return _gameHeight;
            }
            set
            {
                _gameHeight = value;
                ResetRenderTarget();
            }
        }


#endif
        public bool FullScreen = false;
        bool standAlone = true;
        public SpriteFont NormalFont;
        public Transition DefaultTransition = null;

        public NeatGame(string[] args = null, GraphicsDevice device=null, ContentManager content=null)
        {
            Debug.WriteLine("NeatGame Constructed.");
            _graphicsDevice = device;
            _content = content;
            Create();
            Games.Add(this);
        }

        public new GraphicsDevice GraphicsDevice
        {
            get { if (_graphicsDevice != null) return _graphicsDevice; else return base.GraphicsDevice; }
        }

        public new ContentManager Content
        {
            get { if (_content != null) return _content; else return base.Content; }
        }
#if WINDOWS
        public bool IsWideScreen()
        {
            return (GraphicsDevice.DisplayMode.AspectRatio > 1.5);
        }
#endif

#if WINDOWS
        public virtual void InitializeGraphics()
        {
            if (_graphicsDevice != null)
            {
                return;
            }

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
            }

            ResetRenderTarget();
            Graphics.IsFullScreen = FullScreen;
            Graphics.ApplyChanges();
        }

        public void ToggleFullScreen()
        {
            Graphics.ToggleFullScreen();
        }

        public void ResizeToScreen()
        {
            OutputResolution.X = GraphicsDevice.DisplayMode.Width;
            OutputResolution.Y = GraphicsDevice.DisplayMode.Height;
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
        public void Create()
        {
            Ram = new RAM(this);

            videos = new Dictionary<string, Video>();
            videoPlayers = new List<VideoPlayer>();
            sounds = new Dictionary<string, SFXList>();
            songs = new Dictionary<string, Song>();
            effects = new Dictionary<string, Effect>();
            fonts = new Dictionary<string, SpriteFont>();

            Console = new Neat.Components.Console(this);
            TextEffects = new Neat.Components.TextEffects(this);
            ElegantTextEngine = new Neat.Components.ElegantTextEngine(this);
            Event.Engine = this;

            Components.Add(Console);
            Components.Add(TextEffects);
            Components.Add(ElegantTextEngine);

#if KINECT
            Components.Add(
                Kinect = new KinectEngine(this, 
                    ColorImageFormat.RgbResolution640x480Fps30, 
                    DepthImageFormat.Resolution640x480Fps30));
#endif

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

        public static NeatGame NumbNeatGame
        {
            get
            {
                return new NeatGame(new Game());
            }
        }

        public void SetFrameRate(double f)
        {
            TargetElapsedTime = TimeSpan.FromSeconds(1 / f);
        }

        public virtual void FirstTime()
        {
        }

        public void ActivateScreen(string screen, Transition trans = null)
        {
            Debug.WriteLine("ActivateScreen(" + screen + ")");
            if (Screens.ContainsKey(screen))
            {
                if (ActiveScreen != null && Screens.ContainsKey(ActiveScreen))
                {
                    Screens[ActiveScreen].Deactivate(screen);
                    PreviousScreen = ActiveScreen;
                }
                Screens[screen].Activate();
                ActiveScreen = screen;
            }

            Transition = trans;
            if (trans == null) Transition = DefaultTransition;

            if (Transition != null)
            {
                EffectHandler.Game = this;
                Transition.Initialize(gamestime);
            }
        }

        protected override void BeginRun()
        {
            Debug.WriteLine("BeginRun()");
            base.BeginRun();
            InitializeScreens();
            ActivateScreen("mainmenu");
            FirstTime();

            if (File.Exists("autoexec.nsc")) Console.Run("call autoexec.nsc");
#if DEBUG
            Console.Run("et_echo Engine Initialization Complete.$_" +
                "et_echo This is Neat.");
#endif
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
                    GraphicsDevice.DisplayMode.Width,
                    GraphicsDevice.DisplayMode.Height);
                InitializeMessages();
                InitializeGraphics();

                SayMessage("ClientBounds: " + Window.ClientBounds.Width.ToString() + "x" + Window.ClientBounds.Height.ToString());
                SayMessage("Display Mode: " + GraphicsDevice.DisplayMode.Width.ToString() + "x" + GraphicsDevice.DisplayMode.Height.ToString());
                SayMessage("Aspect Ratio: " + GraphicsDevice.DisplayMode.AspectRatio.ToString());

                base.Initialize();

                if (File.Exists("options.nsc")) Console.Run("call options.nsc");
            }
        }

        public virtual void InitializeScreens()
        {
            foreach (var p in Screens)
            {
                Debug.WriteLine("InitializeScreens(" + p.Key + ")");
                p.Value.Initialize();
            }
        }

        public bool GetRandomBool()
        {
            return RandomGenerator.Next(RandomGenerator.Next()) % 2 == 0;
        }
    }
}
