﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Microsoft.Xna.Framework.GamerServices;
namespace Neat.Console
{
    public partial class Console
    {
        private const char eval_open = '(';
        private const char eval_close= ')';
        private List<string> commandsbuffer;
        NeatGame game;
        List<string> buffer;
        string command;
        RAM ram;
        public bool isActive = false;
        public string font = "consolefont";
        public Color textColor = Color.White;
        public Color inputColor = Color.Yellow;
        public Color backColor = Color.DarkBlue;
        public Keys consoleKey = Keys.OemTilde;
        public string backTexture = "solid";
        public bool echo = true;
        public Console(NeatGame _game)
        {
            game = _game;
            ram = game.Ram;
            Clear();
            InitCommands();
            command = "";
            commandsbuffer = new List<string>();
            WriteLine("Neat Console Initialized.");
        }

        public void Update(GameTime gameTime)
        {
            UpdateMessages();
            if (!isActive) return;
            HandleInput(gameTime);
        }
    }
}
