﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Neat.Graphics;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
namespace Neat.Components
{
    public partial class Console : GameComponent 
    {
        public static List<Console> Consoles = new List<Console>();
        private const char eval_open = '(';
        private const char eval_close= ')';
        private List<string> commandsbuffer;
        NeatGame game;
        List<string> buffer;
        string command;
        public RAM Ram;
        LineBrush lb;
        bool _isActive = false;
        bool _soundEffects = true;

        public bool SoundEffects { get { if (game == null || !_soundEffects) return false; return true; } set { _soundEffects = value; } }
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                try
                {
                    yCurtain = -MeasureHeight(LinesCount);
                }
                catch { }
            }
        }
        public string Font = "consolefont";
        public Color TextColor = Color.White;
        public Color InputColor = Color.Orange;
        public Color BackColor = Color.DarkSlateGray;
        public Keys ConsoleKey = Keys.OemTilde;
        public string BackTexture = "solid";
        public bool Echo = true;

        bool standAlone = false;

        public Console(NeatGame _game) : base(_game)
        {
            game = _game;
            Ram = game.Ram;
            Consoles.Add(this);
            //Initialize();
        }

        public Console(Game _game)
            : base(_game)
        {
            game = new NeatGame(game);
            Ram = new RAM();
            standAlone = true;
            Consoles.Add(this);
        }

        public override void Initialize()
        {
            Clear();
            InitCommands();
            InitExpressionEvaluation();
            command = "";
            commandsbuffer = new List<string>();

            WriteLine("Neat " + (standAlone ? "stand alone " : "") +"Console Initialized. [http://neat.codeplex.com]");


            StandAloneTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            Color[] pixmap = new Color[1];
            pixmap[0] = Color.White;
            StandAloneTexture.SetData(pixmap);
            base.Initialize();
        }

        
        public override void Update(GameTime gameTime)
        {
            if (standAlone)
            {
                game.UpdateManually(gameTime);
                game.GetInputState();
                if (game.IsTapped(ConsoleKey))
                {
                    IsActive = !IsActive;
                }
                if (IsActive) HandleInput(gameTime);
                game.SaveInputState();
            }
            else
            {
                if (IsActive) HandleInput(gameTime);
            }
            base.Update(gameTime);
        }
    }
}
