using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using RedBulb;
using RedBulb.MenuSystem;
namespace Tree
{
    public partial class Massacre : RedBulb.RedBulbGame
    {
        #region Fields
        MenuSystem optionsMenu;
        #endregion

        int gameWidth2, gameHeight2;
        bool fullscreen2;
        void InitializeOptionMenu()
        {
            gameWidth2 = gameWidth;
            gameHeight2 = gameHeight;
            fullscreen2 = fullscreen;
            optionsMenu = new MenuSystem(this, new Vector2(gameWidth / 2, gameHeight / 2), GetFont("menufont2"));
            optionsMenu.itemsOffset = new Vector2(0, 80);
            optionsMenu.AddItem("Fullscreen: " + (fullscreen2 ? "ON" : "OFF"));
            optionsMenu.AddItem("Toggle Sound");
            optionsMenu.AddItem("Resolution: 1024x1024",false);
            optionsMenu.AddItem("Apply Settings",false);
            optionsMenu.AddItem("Back");
            optionsMenu.items[2].caption = ("Resolution: " + gameWidth2.ToString() + "x" + gameHeight2.ToString());
            optionsMenu.Enable();

        }
        void OpenOptionsMenu()
        {
            gameState = GameState.OptionsMenu;
            gameWidth2 = gameWidth;
            gameHeight2 = gameHeight;
            fullscreen2 = fullscreen;
            optionsMenu.items[0].caption = ("Fullscreen: " + (fullscreen2 ? "ON" : "OFF"));
            optionsMenu.items[2].caption = ("Resolution: " + gameWidth2.ToString() + "x" + gameHeight2.ToString());

        }
        #region Input

        #endregion

        #region Update
        void UpdateOptionsMenu(GameTime gameTime)
        {
            if (IsTapped(Keys.Escape,Buttons.Back))
                gameState = GameState.Menu;
            if (IsTapped(Keys.Enter,Buttons.A))
            {
                switch (optionsMenu.selectedItem)
                {
                    case 0:
                        fullscreen2 = !fullscreen2;
                        optionsMenu.items[0].caption=("Fullscreen: " + (fullscreen2 ? "ON" : "OFF"));
                        graphics.ToggleFullScreen();
                        //optionsMenu.recalculateSizes();
                        break;
                    case 1:
                        MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
                        muteAllSounds = !muteAllSounds;
                        break;
                    case 2:
                        if (gameWidth2 == 1024)
                        {
                            gameWidth2 = 1280; gameHeight2 = 960;
                        }
                        else if (gameWidth2 == 1280 && gameHeight2 == 960)
                        {
                            gameWidth2 = 1280; gameHeight2 = 1024;
                        }
                        else if (gameWidth2 == 1280 && gameHeight2 == 1024)
                        {
                            gameWidth2 = 1024; gameHeight2 = 768;
                        }
                        optionsMenu.items[2].caption = ("Resolution: " + gameWidth2.ToString() + "x" + gameHeight2.ToString());
                        //optionsMenu.recalculateSizes();
                        break;
                    case 3:
#if WINDOWS
                        gameWidth = gameWidth2;
                        gameHeight = gameHeight2 ;
                        fullscreen = fullscreen2;
                        InitializeGraphics();
#endif
                        break;
                    case 4:
                        gameState = GameState.Menu;
                        break;
                }
            }
            optionsMenu.Update(gameTime);
        }
        #endregion

        #region Draw
        void DrawOptionsMenu(GameTime gameTime)
        {
            //graphics.GraphicsDevice.Clear(Color.Black);
            
            optionsMenu.Draw(gameTime);
        }
        #endregion
    }
}