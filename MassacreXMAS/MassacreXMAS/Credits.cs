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
using RedBulb;
using RedBulb.MenuSystem;
namespace Tree
{
    public partial class Massacre : RedBulb.RedBulbGame
    {
        #region Fields
        #endregion

        void InitializeCredits()
        {
            

        }
        void OpenCredits()
        {
            gameState = GameState.Credits;
        }
        #region Input

        #endregion

        #region Update
        void UpdateCredits(GameTime gameTime)
        {
            if (IsTapped(Keys.Escape,Buttons.Back) || IsTapped(Keys.Enter,Buttons.A))
                gameState = GameState.Menu;
        
        }
        #endregion

        #region Draw
        void DrawCredits(GameTime gameTime)
        {
            spriteBatch.Draw(getTexture("Solid"), new Rectangle(0, 0, gameWidth, gameHeight), Color.White);
            spriteBatch.Draw(getTexture("credits"), new Vector2(0,-10), Color.White);
            DrawShadowedString(GetFont("menufont2"), "< press escape", new Vector2(10, gameHeight - 100), Color.DarkGreen, Color.White);
        }
        #endregion
    }
}