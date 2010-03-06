#region References
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using RedBulb;
using RedBulb.MenuSystem;
//using UltimaScroll.IBLib;
#endregion

namespace RedBulb
{
    public class GamePart
    {
#region Fields
        protected SpriteBatch spriteBatch { get { return game.spriteBatch; } set { game.spriteBatch = value; } }
        public RedBulbGame game;
        public GameTime gamesTime;
        public uint frame { get { return game.frame; }}
#endregion

#region Initialize
        public GamePart(RedBulbGame Game)
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
