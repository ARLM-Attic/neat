using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Microsoft.Xna.Framework.GamerServices;
namespace RedBulb.Console
{
    public partial class Console
    {
        private const char eval_open = '(';
        private const char eval_close= ')';
        private List<string> commandsbuffer;
        RedBulbGame game;
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
        public Console(RedBulbGame _game)
        {
            game = _game;
            ram = game.ram;
            Clear();
            InitCommands();
            command = "";
            commandsbuffer = new List<string>();
            WriteLine("RedBulb Console Initialized.");
        }

        public void Update(GameTime gameTime)
        {
            UpdateMessages();
            if (!isActive) return;
            HandleInput(gameTime);
        }
    }
}
