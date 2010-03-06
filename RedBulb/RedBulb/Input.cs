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
using RedBulb;
using RedBulb.MenuSystem;
//using UltimaScroll.IBLib;
#endregion
namespace RedBulb
{
    public partial class RedBulbGame : Microsoft.Xna.Framework.Game
    {
#if ZUNE
       // KeyboardState currentKeyboardState, lastKeyboardState;
        GamePadState currentGamePadState, lastGamePadState;
       // MouseState currentMouseState, lastMouseState;

        protected void InitializeInput()
        {
          //  currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
           // currentMouseState = Mouse.GetState();
          //  lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;
         //   lastMouseState = currentMouseState;
        }

        protected void SaveInputState()
        {

         //   lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;
        //    lastMouseState = currentMouseState;
        }
        protected void GetInputState()
        {
        //    currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
         //   currentMouseState = Mouse.GetState();
        }
        

        public bool IsTapped( Buttons button)
        {
            return 
                (currentGamePadState.IsButtonUp(button) && lastGamePadState.IsButtonDown(button));
        }

        public bool IsPressed( Buttons button)
        {
            return 
                (currentGamePadState.IsButtonDown(button));
        }
#endif
#if WINDOWS
        KeyboardState currentKeyboardState, lastKeyboardState;
        GamePadState currentGamePadState, lastGamePadState;
        MouseState currentMouseState, lastMouseState;

        protected void InitializeInput()
        {
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentMouseState = Mouse.GetState();
            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;
            lastMouseState = currentMouseState;
        }

        protected void SaveInputState()
        {
            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;
            lastMouseState = currentMouseState;
        }
        protected void GetInputState()
        {
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentMouseState = Mouse.GetState();
        }

        public bool IsMouseClicked()
        {
            return currentMouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsMousePressed()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsRightMouseClicked()
        {
            return currentMouseState.RightButton == ButtonState.Released && lastMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsRightMousePressed()
        {
            return currentMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsMidMouseClicked()
        {
            return currentMouseState.MiddleButton == ButtonState.Released && lastMouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool IsMidMousePressed()
        {
            return currentMouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool IsTapped(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key);
        }
        public bool IsTapped(Buttons button)
        {
            return currentGamePadState.IsButtonDown(button) && lastGamePadState.IsButtonUp(button);
        }
        public bool IsPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }
        public bool IsPressed(Buttons button)
        {
            return currentGamePadState.IsButtonDown(button);
        }
        public bool IsTapped(Keys key, Buttons button)
        {
            return IsTapped(key) || IsTapped(button);
        }
        public bool IsPressed(Keys key, Buttons button)
        {
            return IsPressed(key) || IsPressed(button);
        }
        public bool IsReleased(Keys key)
        {
            return currentKeyboardState.IsKeyUp(key) && lastKeyboardState.IsKeyDown(key);
        }
        public bool IsReleased(Buttons button)
        {
            return currentGamePadState.IsButtonUp(button) && lastGamePadState.IsButtonDown(button);
        }
        public bool IsReleased(Keys key, Buttons button)
        {
            return IsReleased(key) || IsReleased(button);
        }
#endif
    }
}