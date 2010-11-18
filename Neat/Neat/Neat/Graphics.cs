using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.Graphics;
using Neat.MenuSystem;
 

namespace Neat
{
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
        public Color BackGroundColor = Color.Black;

        public bool AutoDraw = true;
        public bool AutoClear = true;
        public bool ShowMouse = false;

        bool _landscape = false;

        RenderTarget2D _renderTarget;
        Vector2 _vecOrigin, _vecDest;  

        public bool Landscape
        {
            get
            {
                return _landscape;
            }
            set
            {
                if (value)
                {
                    _renderTarget = new RenderTarget2D(GraphicsDevice, GameWidth, GameHeight);
                    _vecOrigin = new Vector2(GameWidth/2f, GameHeight/2f);
                    _vecDest = new Vector2(_vecOrigin.Y, _vecOrigin.X);
                }
                _landscape = value;
            }
        }
        
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void Clear(Color c)
        {
            Graphics.GraphicsDevice.Clear(c);
        }

        protected override void Draw(GameTime gameTime)
        {
            gamestime = gameTime;

            if (Landscape)
                GraphicsDevice.SetRenderTarget(_renderTarget);

            if (AutoClear)
                Graphics.GraphicsDevice.Clear(BackGroundColor);


            if (AutoDraw)
                SpriteBatch.Begin();

#if XLIVE
            if (SignedInGamer.SignedInGamers.Count > 0 || !needSignIn || !forceSignIn)
#endif
            Render(gameTime);

            if (ShowMouse
#if XLIVE
                && !Guide.IsVisible
#endif
                )
            {
#if WINDOWS
                DrawMouse(mousePosition);
#endif
            }
            
            if (AutoDraw)
                SpriteBatch.End();
#if WINDOWS
            if (HasConsole && Console.IsActive)
            {
                SpriteBatch.Begin();
                Console.Draw(0, 10);
                SpriteBatch.End();
            }
#endif
            
            if (Landscape)
            {
                GraphicsDevice.SetRenderTarget(null);

                SpriteBatch.Begin();
                SpriteBatch.Draw(
                    _renderTarget, 
                    _vecDest, 
                    null, 
                    Color.White, 
                    MathHelper.PiOver2, 
                    _vecOrigin, 
                    1f, 
                    SpriteEffects.None, 
                    0);

                SpriteBatch.End();
            }

            base.Draw(gameTime);
        }
        protected virtual void DrawMouse(Vector2 pos)
        {
            SpriteBatch.Draw(GetTexture("mousePointer"),
                pos, Color.White);
        }
        protected virtual void Render(GameTime gameTime)
        {
            Screens[ActiveScreen].Render(gameTime);
        }
        
        public void Write(string text, Vector2 position)
        {
            GraphicsHelper.DrawShadowedString(SpriteBatch, NormalFont, text, position, Color.White);
        }
    }
}