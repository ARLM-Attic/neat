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
        MenuSystem menu;
        #endregion 

        bool MainMenuCinematics = true;
        void InitializeMenus()
        {

            LoadTexture("Sprites\\Menu\\logo1");
            LoadTexture("Sprites\\Menu\\menuBackground");

            InitializeInGameMenu();
            InitializeOptionMenu();
              
            menu = new MenuSystem(this, new Vector2(gameWidth / 2, gameHeight / 2), GetFont("menufont2"));
            menu.defaultItemColor = Color.YellowGreen 
 ;
            menu.itemsOffset = new Vector2(0, 80);
            menu.AddItem("Go Skiing");
            menu.AddItem("Options");
            menu.AddItem("Credits");
            menu.AddItem("Quit Tree Massacre");
            menu.Enable();
            EnableMainMenu();

        }
        void EnableMainMenu()
        {
            MainMenuCinematics = true;
            alpha = 0;
            
            gameState = GameState.Menu;
        }
        #region Input

        #endregion

        #region Update
        void UpdateMenu(GameTime gameTime)
        {
           // if (MainMenuCinematics) return;
            if (IsPressed(Keys.Escape,Buttons.Back))
                this.Exit();
            if (IsTapped(Keys.Enter,Buttons.A))
            {
                switch (menu.selectedItem)
                {
                    case 0:
                        InitializeLogics();
                        gameState = GameState.Game;
                        break;
                    case 1:
                        OpenOptionsMenu();
                        break;
                    case 2:
                        OpenCredits();
                        break;
                    case 3:
                        this.Exit();
                        break;
                }
            }
            menu.Update(gameTime);
        }
        #endregion

        #region Draw
        void DrawMenu(GameTime gameTime)
        {
            if (MainMenuCinematics) DrawMainMenuCinematics(gameTime);
            else
            {
                //graphics.GraphicsDevice.Clear(Color.Black);
                spriteBatch.Draw(getTexture("menuBackground"), new Rectangle(0, 0, gameWidth, gameHeight), Color.White);
                spriteBatch.Draw(getTexture("logo1"), new Vector2(8,8), Color.White);
                spriteBatch.Draw(getTexture("xmas"), new Vector2(gameWidth-520, 60), Color.White);
                menu.Draw(gameTime);
            }
        }
        float alpha = 0;
        void DrawMainMenuCinematics(GameTime gameTime)
        {
            if (alpha > 1f) MainMenuCinematics = false;
            
            spriteBatch.Draw(getTexture("logo1"), new Vector2(8, 8),GetColorWithAlpha(Color.White, alpha));
            spriteBatch.Draw(getTexture("xmas"), new Vector2(gameWidth - 520, 60), GetColorWithAlpha(Color.White, alpha));
            alpha+=0.02f;
        }

        Color getTransparentColor(Color color, float alpha)
        {
            return new Color( new Vector4(color.R, color.G, color.B, alpha) );
        }
        Color CreateColor(float red,float green, float blue, float alpha)
        {
            return new Color(new Vector4(red, green, blue, alpha));
        }
        #endregion
    }
}