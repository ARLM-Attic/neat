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
#if XLIVE
        public bool ForceSignIn = true;
#endif
        public RAM Ram;

        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public Random RandomGenerator;

#if XLIVE
        public NetworkHelper networkHelper;
#endif

        public Dictionary<string,GamePart> Parts;
        public string ActivePart;

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
            GameWidth = 1024,
            GameHeight=768;
#endif
        public bool FullScreen = false;

#if WINDOWS
        public virtual void InitializeGraphics()
        {
            GraphicsDevice.Reset();
            
            Graphics.PreferredBackBufferWidth = GameWidth;
            Graphics.PreferredBackBufferHeight = GameHeight;
            
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
        //public GameWindow window { get { return Window; } }

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

#if XLIVE
            Components.Add(new GamerServicesComponent(this));
#endif

            Graphics = new GraphicsDeviceManager(this);
            RandomGenerator = new Random();
            Content.RootDirectory = "Content";

            SetFrameRate(60);

            Parts = new Dictionary<string, GamePart>();
            AddParts();
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

        public void ActivatePart(string part)
        {
            Parts[part].Activate();
            ActivePart = part;
        }

        public SpriteFont NormalFont;

        protected override void BeginRun()
        {
            
            InitializeParts();
            ActivatePart("mainmenu");
            FirstTime();
            base.BeginRun();
        }

        protected override void Initialize()
        {
            Frame = 0;

#if XLIVE
            networkHelper = new NetworkHelper(this,gamestime);
#endif

            InitializeInput();
            InitializeMessages();
            InitializeGraphics();

            SayMessage("ClientBounds: " + Window.ClientBounds.Width.ToString() + "x" + Window.ClientBounds.Height.ToString());
            SayMessage("Display Mode: " + Graphics.GraphicsDevice.DisplayMode.Width.ToString() + "x" + Graphics.GraphicsDevice.DisplayMode.Height.ToString());
            SayMessage("Aspect Ratio: " + Graphics.GraphicsDevice.DisplayMode.AspectRatio.ToString());
            
            base.Initialize();

        }

        public virtual void InitializeParts()
        {
            foreach (var p in Parts)
                p.Value.Initialize();
        }
    }


}
