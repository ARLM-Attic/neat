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
using Neat.GUI;
 

namespace Neat
{
    public class Screen
    {
#region Fields
        protected SpriteBatch SpriteBatch { get { return game.SpriteBatch; } set { game.SpriteBatch = value; } }
        protected NeatGame game;
        public GameTime gamesTime;
        public uint Frame { get { return game.Frame; }}
        public Form Form;
#endregion

#region Initialize
        public Screen(NeatGame Game)
        {
            game = Game;
        }

        public virtual void Initialize()
        {
            Form = new Form(game);
        }

        public virtual void Activate()
        {
            if (game.HasConsole && Form != null) Form.AttachToConsole();
        }
#endregion

#region Files
        public virtual void LoadContent()
        {
        }
#endregion 

#region Loop
        public virtual void Update(GameTime gameTime)
        {
            HandleInput(gameTime);
            Behave(gameTime);
            Form.Update(gameTime);
        }

        public virtual void Behave(GameTime gameTime)
        {
        }
#endregion

#region Render
        public virtual void Render(GameTime gameTime)
        {
            Form.Draw(gameTime);
        }
#endregion

#region Input
        public virtual void HandleInput(GameTime gameTime)
        {
        }
#endregion

    }
}
