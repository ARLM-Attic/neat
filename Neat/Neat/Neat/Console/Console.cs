using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
namespace Neat.Components
{
    public partial class Console : GameComponent 
    {
        private const char eval_open = '(';
        private const char eval_close= ')';
        private List<string> commandsbuffer;
        NeatGame game;
        List<string> buffer;
        string command;
        RAM ram;

        public bool IsActive = false;
        public string Font = "consolefont";
        public Color TextColor = Color.White;
        public Color InputColor = Color.Yellow;
        public Color BackColor = Color.DarkBlue;
        public Keys ConsoleKey = Keys.OemTilde;
        public string BackTexture = "solid";
        public bool Echo = true;
        public Console(NeatGame _game) : base(_game)
        {
            game = _game;
            ram = game.Ram;
            Clear();
            InitCommands();
            command = "";
            commandsbuffer = new List<string>();
            WriteLine("Neat Console Initialized.");
        }

        public override void Update(GameTime gameTime)
        {
            UpdateMessages();
            if (!IsActive) return;
            HandleInput(gameTime);
            base.Update(gameTime);
        }
    }
}
