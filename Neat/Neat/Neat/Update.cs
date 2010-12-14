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
#if WINDOWS
 
 
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;

namespace Neat
{
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
#if LIVE
        public bool needSignIn = true;
#endif
        public bool IsPaused = false;
        
        void UpdateGame(GameTime gameTime)
        {
            gamestime = gameTime;

#if LIVE
            networkHelper.Update();
#endif
            GetInputState();

            if (MediaPlayer.State == MediaState.Playing && muteAllSounds)
            {
                MediaPlayer.Pause();
            }
            else if (MediaPlayer.State == MediaState.Paused && !muteAllSounds)
            {
                MediaPlayer.Resume();
            }

#if WINDOWS
            if (HasConsole && IsTapped(ConsoleKey))
            {
                Console.IsActive = !Console.IsActive;
            }
#endif
            if (!Freezed)
            {
                Behave(gameTime);
#if !WINDOWS_PHONE
#if ZUNE
            if (IsTapped(Buttons.Back))
#elif WINDOWS
                if (IsTapped(Keys.Escape))
#endif
                    GetSound("bleep6").Play();
#endif
            }
            base.Update(gameTime);
            SaveInputState();
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
#if LIVE
            if (!needSignIn && !Guide.IsVisible)
            {
                UpdateGame(gameTime);
            }
            else
            {
                if ((SignedInGamer.SignedInGamers.Count > 0 || !forceSignIn) && !Guide.IsVisible)
                {
                    UpdateGame(gameTime);
                }
                else
                {
                    if (SignedInGamer.SignedInGamers.Count <= 0 && !Guide.IsVisible && forceSignIn)
                    {
                        Guide.ShowSignIn(1, false);
                    }
                }
            }
#else
            UpdateGame(gameTime);
#endif
            
            if (ShowMouse)
            {
#if WINDOWS
                mousePosition = new Vector2((float)Mouse.GetState().X, (float)Mouse.GetState().Y);
#elif ZUNE
                MoveZuneMouse();
                
#endif
            }
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
        public Vector2 mousePosition = Vector2.Zero;
        protected virtual void Behave(GameTime gameTime)
        {
            if (!IsPaused && ActiveScreen != null) Screens[ActiveScreen].Update(gameTime);
        }
    }
}