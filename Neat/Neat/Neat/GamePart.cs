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
    public class GamePart
    {
#region Fields
        protected SpriteBatch spriteBatch { get { return game.spriteBatch; } set { game.spriteBatch = value; } }
        public NeatGame game;
        public GameTime gamesTime;
        public uint frame { get { return game.frame; }}
#endregion

#region Initialize
        public GamePart(NeatGame Game)
        {
            game = Game;
            //spriteBatch = game.spriteBatch;
        }
        public virtual void Initialize()
        {
            //frame = game.frame;
        }
        public virtual void Activate()
        {
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
        }

        public virtual void Behave(GameTime gameTime)
        {
        }
#endregion

#region Render
        public virtual void Render(GameTime gameTime)
        {
            
        }
#endregion

#region Input
        public virtual void HandleInput(GameTime gameTime)
        {
        }
#endregion

    }
}
