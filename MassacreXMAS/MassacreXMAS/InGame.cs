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

        MenuSystem inGameMenu;
        void InitializeInGameMenu()
        {
            inGameMenu = new MenuSystem(this, new Vector2(gameWidth / 2, gameHeight / 2), GetFont("menufont2"));
            inGameMenu.itemsOffset = new Vector2(0, 80);
            //inGameMenu.position.Y = 117;
            //inGameMenu.position.X = 213;
            inGameMenu.AddItem("Return to Game");
            inGameMenu.AddItem("Restart the Game");
            inGameMenu.AddItem("", false);
            inGameMenu.AddItem("Return to Main Menu");
            inGameMenu.AddItem("Exit");
            inGameMenu.GetLastMenuItem().forecolor = Color.PaleVioletRed ;
            inGameMenu.Enable();
        }
        #region Input
        void HandleInGameInput()
        {
            if (IsTapped(Keys.Escape, Buttons.Back)) gameState = GameState.Game;
            if (IsTapped(Keys.Enter,Buttons.A))
            {
                switch (inGameMenu.selectedItem)
                {
                    case 0:
                        gameState = GameState.Game;
                        break;
                    case 1:
                        gameState = GameState.Game;
                        GameOver();
                        break;
                    case 2:
                        // Seperator
                        break;
                    case 3:
                        EnableMainMenu();
                        break;
                    case 4:
                        this.Exit();
                        break;
                }
            }

            //if (IsPressed(Keys.Left)) inGameMenu.position.X--;
            //if (IsPressed(Keys.Right)) inGameMenu.position.X++;
            //if (IsPressed(Keys.Down)) inGameMenu.position.Y++;
            //if (IsPressed(Keys.Up)) inGameMenu.position.Y--;
        }
        #endregion

        #region Update
        void UpdateInGameMenu(GameTime gameTime)
        {
            inGameMenu.Update(gameTime);
            HandleInGameInput();
            
        }
        #endregion

        #region Draw
        void DrawInGameMenu(GameTime gameTime)
        {
            DrawGame(gameTime);
            Shade();
            inGameMenu.Draw(gameTime);
 //           spriteBatch.DrawString(calibri, inGameMenu.position.X.ToString() + "," + inGameMenu.position.Y.ToString(), Vector2.Zero, Color.White);
        }

        void Shade()
        {
            spriteBatch.Draw(getTexture("transparent"), new Rectangle(0, 0, gameWidth, gameHeight), Color.Black);
        }
        #endregion
    }
}