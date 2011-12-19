using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
 
namespace Neat
{
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
#if ZUNE || WINDOWS_PHONE
#if WINDOWS_PHONE
        KeyboardState currentKeyboardState, lastKeyboardState;
        public TouchCollection currentTouchState, lastTouchState;
#endif
        GamePadState currentGamePadState, lastGamePadState;
       // MouseState currentMouseState, lastMouseState;

        protected void InitializeInput()
        {
#if WINDOWS_PHONE
            currentKeyboardState = Keyboard.GetState();
            lastKeyboardState = currentKeyboardState;
            currentTouchState = TouchPanel.GetState();
            lastTouchState = currentTouchState;
#endif
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
           // currentMouseState = Mouse.GetState();
            lastGamePadState = currentGamePadState;
         //   lastMouseState = currentMouseState;
        }

        protected void SaveInputState()
        {
#if WINDOWS_PHONE
            lastKeyboardState = currentKeyboardState;
            lastTouchState = currentTouchState;
#endif
            lastGamePadState = currentGamePadState;
        //    lastMouseState = currentMouseState;
        }
        protected void GetInputState()
        {
#if WINDOWS_PHONE
            currentKeyboardState = Keyboard.GetState();
            currentTouchState = TouchPanel.GetState();
#endif
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
         //   currentMouseState = Mouse.GetState();
        }
        
#if ZUNE
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
#endif


#if WINDOWS_PHONE
        public List<Vector2> GetTouchedLocations()
        {
            List<Vector2> v = new List<Vector2>();
            foreach (var item in currentTouchState)
            {
                v.Add(item.Position);
            }
            return v;
        }

        public bool IsTouched()
        {
            return currentTouchState.Count > 0;
        }

        public bool IsTouched(Rectangle r)
        {
            return IsTouched(r, currentTouchState);
        }

        bool IsTouched(Rectangle r, TouchCollection state)
        {
            foreach (var item in state)
            {
                if (Geometry2D.IsVectorInRectangle(r, item.Position)) return true;
            }
            return false;
        }

        public bool IsTapped(Rectangle r)
        {
            return !IsTouched(r, lastTouchState) && IsTouched(r);
        }

        public Vector2 GetHardestTouchPosition()
        {
            if (!IsTouched()) return new Vector2(-1);
            Vector2 v = currentTouchState[0].Position;
            float p = currentTouchState[0].Pressure;
            foreach (var item in currentTouchState)
            {
                if (item.Pressure > p)
                {
                    p = item.Pressure;
                    v = item.Position;
                }
            }
            return v;
        }

        public int GetTouchesCount()
        {
            return currentTouchState.Count;
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

        public void SaveInputState()
        {
            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;
            lastMouseState = currentMouseState;
        }
        public void GetInputState()
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
#endif

#if WINDOWS || WINDOWS_PHONE
        public bool IsTapped(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key);
        }
        public bool IsTapped(params Keys[] keys)
        {
            foreach (var item in keys)
            {
                if (IsTapped(item)) return true;
            }
            return false;
        }
        public bool IsTapped(Buttons button)
        {
            return currentGamePadState.IsButtonDown(button) && lastGamePadState.IsButtonUp(button);
        }
        public bool IsTapped(params Buttons[] buttons)
        {
            foreach (var item in buttons)
            {
                if (IsTapped(item)) return true;
            }
            return false;
        }
        public bool IsPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }
        public bool IsPressed(params Keys[] keys)
        {
            foreach (var item in keys)
            {
                if (IsPressed(item)) return true;
            }
            return false;
        }
        public bool IsPressed(Buttons button)
        {
            return currentGamePadState.IsButtonDown(button);
        }
        public bool IsPressed(params Buttons[] buttons)
        {
            foreach (var item in buttons)
            {
                if (IsPressed(item)) return true;
            }
            return false;
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
        public bool IsReleased(params Keys[] keys)
        {
            foreach (var item in keys)
            {
                if (IsReleased(item))
                    return true;
            }
            return false;
        }
        public bool IsReleased(Buttons button)
        {
            return currentGamePadState.IsButtonUp(button) && lastGamePadState.IsButtonDown(button);
        }
        public bool IsReleased(params Buttons[] buttons)
        {
            foreach (var item in buttons)
            {
                if (IsReleased(item))
                    return true;
            }
            return false;
        }
        public bool IsReleased(Keys key, Buttons button)
        {
            return IsReleased(key) || IsReleased(button);
        }
#endif
    }
}