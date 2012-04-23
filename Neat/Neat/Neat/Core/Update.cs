using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
using System.Diagnostics;

namespace Neat
{
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
        public bool InputEnabled = true;
        public bool ConsoleDisablesInput = true;
#if WINDOWS
        public bool HasMouse = true;
#endif
#if LIVE
        public bool NeedSignIn = true;
#endif
        public bool IsPaused = false;
        int secondCounter = 0;
        
        void UpdateGame(GameTime gameTime)
        {
            gamestime = gameTime;
#if LIVE
            networkHelper.Update();
#endif
            GetInputState();

            //Apparently MediaPlayer.GetState sucks as it's shown in
            //memory profiling. so let's check these conditions every
            //second instead of every frame.

            /*if (secondCounter != gameTime.TotalGameTime.Seconds)
            {
                if (MediaPlayer.State == MediaState.Playing && MuteAllSounds)
                {
                    MediaPlayer.Pause();
                }
                else if (MediaPlayer.State == MediaState.Paused && !MuteAllSounds)
                {
                    MediaPlayer.Resume();
                }
            }*/

#if WINDOWS
            if (HasConsole && IsTapped(ConsoleKey))
            {
                if (Console.SoundEffects) PlaySound("console_echo");
                Console.IsActive = !Console.IsActive;
            }
#endif
            if (!Freezed)
            {
                Behave(gameTime);
            }
            base.Update(gameTime);
            SaveInputState();

            secondCounter = gameTime.TotalGameTime.Seconds;
        }
        public void UpdateManually(GameTime gameTime)
        {
            Update(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            Frame++;
            if (Frame > MaxFrame) Frame = 0;
            if (!standAlone) return;
#if LIVE && FALSE
            if (!NeedSignIn && !Guide.IsVisible)
            {
                UpdateGame(gameTime);
            }
            else
            {
                if ((SignedInGamer.SignedInGamers.Count > 0 || !ForceSignIn) && !Guide.IsVisible)
                {
                    UpdateGame(gameTime);
                }
                else
                {
                    if (!Guide.IsVisible)
                    {
                        Guide.ShowSignIn(1, false);
                    }
                }
            }
#else
            UpdateGame(gameTime);
#endif
            
#if WINDOWS
                if (HasMouse)
                {
                    RealMousePosition.X = currentMouseState.X;
                    RealMousePosition.Y = currentMouseState.Y;

                    if (StretchMode == StretchModes.None)
                        MousePosition = RealMousePosition;
                    else  {
                        MousePosition = new Vector2(
                            (RealMousePosition.X / _vecSize.X) * (float)GameWidth,
                            (RealMousePosition.Y / _vecSize.Y) * (float)GameHeight);
                        MousePosition += new Vector2(
                            MousePosition.X / _vecSize.X, MousePosition.Y / _vecSize.Y) * _vecDest;
                    }
                }
#endif

            if (Transition != null && !Transition.Finished) Transition.Update(gameTime);
        }
#if ZUNE
        public Vector2 mouseSens = new Vector2(6f,6f);
        void MoveZuneMouse()
        {
            mousePosition.X += (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X *mouseSens.X );
            mousePosition.Y -= (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y*mouseSens.Y );
            /*
            if (IsPressed(Buttons.LeftThumbstickLeft))
            {
                mousePosition.X -= mouseSens;
                if (mousePosition.X < 0) mousePosition.X = 0;
            }
            if (IsPressed(Buttons.LeftThumbstickRight))
            {
                mousePosition.X += mouseSens;
                if (mousePosition.X > GameWidth) mousePosition.X = GameWidth;
            }
            if (IsPressed(Buttons.LeftThumbstickUp))
            {
                mousePosition.Y -= mouseSens;
                if (mousePosition.Y < 0) mousePosition.Y = 0;
            }
            if (IsPressed(Buttons.LeftThumbstickDown))
            {
                mousePosition.Y += mouseSens;
                if (mousePosition.Y > GameHeight) mousePosition.Y = GameHeight;
            }*/
        }
#endif
        public Vector2 MousePosition = Vector2.Zero;
        public Vector2 RealMousePosition = Vector2.Zero;
        protected virtual void Behave(GameTime gameTime)
        {
            if (!IsPaused && ActiveScreen != null && Screens.ContainsKey(ActiveScreen)) 
                Screens[ActiveScreen].Update(gameTime, InputEnabled && !(Console.IsActive && ConsoleDisablesInput));
        }
    }
}