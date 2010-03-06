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
using Microsoft.Xna.Framework.Media;
namespace RedBulb
{
    public partial class RedBulbGame : Microsoft.Xna.Framework.Game
    {
#if XLIVE
        public bool needSignIn = true;
#endif
        public int updateCPS = 0;
        public int drawCPS = 0;
        int _ucps = 0;
        int _dcps = 0;
        TimeSpan lastUpdateTimeSpan = new TimeSpan();
        TimeSpan lastDrawTimeSpan = new TimeSpan();

        void UpdateGame(GameTime gameTime)
        {
            _ucps++;
            if (gameTime.TotalRealTime.Seconds != lastUpdateTimeSpan.Seconds)
            {
                lastUpdateTimeSpan = gameTime.TotalRealTime;
                updateCPS = _ucps;
                _ucps = 0;
            }

            if (MediaPlayer.State == MediaState.Playing && muteAllSounds)
            {
                MediaPlayer.Pause();
            }
            else if (MediaPlayer.State == MediaState.Paused && !muteAllSounds)
            {
                MediaPlayer.Resume();
            }
            gamestime = gameTime;
#if XLIVE
            networkHelper.Update();
#endif
            GetInputState();

            if (isFirstTime) { FirstTime(); isFirstTime = false; }
            if (hasConsole)
            {
                if (IsTapped(Keys.OemTilde)) console.isActive =! console.isActive;
                console.Update(gameTime);
            }
            if (!freezed)
            {
                Behave(gameTime);
#if ZUNE
            if (IsTapped(Buttons.Back))
#elif WINDOWS
                if (IsTapped(Keys.Escape))
#endif
                    GetSound("bleep6").Play();
            }
            SaveInputState();
        }
        protected override void Update(GameTime gameTime)
        {
#if XLIVE
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
            frame++;
            if (frame > maxFrame) frame = 0;
            if (showMouse)
            {
#if WINDOWS
                mousePosition = new Vector2((float)Mouse.GetState().X, (float)Mouse.GetState().Y);
#elif ZUNE
                MoveZuneMouse();
                
#endif
            }
            base.Update(gameTime);
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
            parts[activePart].Update(gameTime);
        }
    }
}