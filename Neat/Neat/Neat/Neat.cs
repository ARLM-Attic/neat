﻿using System;
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
        public NeatGame(string[] args = null, GraphicsDevice device=null, ContentManager content=null)
        {
            Debug.WriteLine("NeatGame Constructed.");
            _graphicsDevice = device;
            _content = content;
            Create();
        }

        GraphicsDevice _graphicsDevice = null;
        ContentManager _content = null;
        public new GraphicsDevice GraphicsDevice
        {
            get { if (_graphicsDevice != null) return _graphicsDevice; else return base.GraphicsDevice; }
        }
        public new ContentManager Content
        {
            get { if (_content != null) return _content; else return base.Content; }
        }
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
        bool standAlone = true;
        
        public void Create()
        {
            Ram = new RAM(this);

            videos = new Dictionary<string, Video>();
            videoPlayers = new List<VideoPlayer>();
            sounds = new Dictionary<string, SoundEffect>();
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

        public uint Frame = 0;
        public const uint MaxFrame = uint.MaxValue - 2;

        public void ActivateScreen(string screen)
        {
            if (Screens.ContainsKey(screen))
            {
                Screens[screen].Activate();
                ActiveScreen = screen;
            }
        }

        public SpriteFont NormalFont;

        protected override void BeginRun()
        {
            base.BeginRun();
            InitializeScreens();
            ActivateScreen("mainmenu");
            FirstTime();

            if (File.Exists("autoexec.nsc")) Console.Run("call autoexec.nsc");
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
                p.Value.Initialize();
        }
    }
}
