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
        public Color backGroundColor = Color.Black;

        public bool autoDraw = true;
        public bool autoClear = true;
        public bool showMouse = true;

        public RasterizerState rasterizerState = RasterizerState.CullNone;
        bool _landscape = false;

        RenderTarget2D _renderTarget;
        Vector2 _vecOrigin, _vecDest;  

        public bool landscape
        {
            get
            {
                return _landscape;
            }
            set
            {
                if (value)
                {
                    _renderTarget = new RenderTarget2D(GraphicsDevice, gameWidth, gameHeight);
                    _vecOrigin = new Vector2(gameWidth/2f, gameHeight/2f);
                    _vecDest = new Vector2(_vecOrigin.Y, _vecOrigin.X);
                }
                _landscape = value;
            }
        }
        
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void Clear(Color c)
        {
            graphics.GraphicsDevice.Clear(c);
        }

        protected override void Draw(GameTime gameTime)
        {
            gamestime = gameTime;

            if (landscape)
            {
                GraphicsDevice.SetRenderTarget(_renderTarget);
            }

            if (autoClear)
                graphics.GraphicsDevice.Clear(backGroundColor);


            if (autoDraw)
                spriteBatch.Begin();

#if XLIVE
            if (SignedInGamer.SignedInGamers.Count > 0 || !needSignIn || !forceSignIn)
#endif
            {
                Render(gameTime);
            }

            if (showMouse
#if XLIVE
                && !Guide.IsVisible
#endif
                )
            {
#if WINDOWS
                DrawMouse(mousePosition);
#endif
            }
#if WINDOWS
            if (hasConsole)
            {
                if (!autoDraw) spriteBatch.Begin();
                console.Draw(0, 10);
                if (!autoDraw) spriteBatch.End();
            }
#endif
            if (autoDraw)
                spriteBatch.End();

            if (landscape)
            {
                GraphicsDevice.SetRenderTarget(null);

                spriteBatch.Begin();
                spriteBatch.Draw(
                    _renderTarget, 
                    _vecDest, 
                    null, 
                    Color.White, 
                    MathHelper.PiOver2, 
                    _vecOrigin, 
                    1f, 
                    SpriteEffects.None, 
                    0);

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
        protected virtual void DrawMouse(Vector2 pos)
        {
            spriteBatch.Draw(getTexture("mousePointer"),
                pos, Color.White);
        }
        protected virtual void Render(GameTime gameTime)
        {
            parts[activePart].Render(gameTime);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //deprecated methods
        #region Depricated Methods
        public void Write(string text, Vector2 position)
        {
            GraphicsHelper.DrawShadowedString(spriteBatch, normalFont, text, position, Color.White);
        }
        
        public void DrawShadowedString(SpriteFont spriteFont, string text, Vector2 position, Color foreColor, Color backColor)
        {
            GraphicsHelper.DrawShadowedString(spriteBatch, spriteFont, text, position, foreColor, backColor);
        }
        public void DrawShadowedString(SpriteFont spriteFont, string text, Vector2 position, Vector2 shadowOffset, Color foreColor, Color backColor)
        {
            GraphicsHelper.DrawShadowedString(spriteBatch, spriteFont, text, position, shadowOffset, foreColor, backColor);
        }
        public void DrawShadowedString(SpriteFont spriteFont, string text, Vector2 position, Color foreColor)
        {
            GraphicsHelper.DrawShadowedString(spriteBatch, spriteFont, text, position, foreColor);
        }
        public void DrawShadowedString(SpriteFont spriteFont, string text, Vector2 position, Vector2 shadowOffset, Color foreColor)
        {
            GraphicsHelper.DrawShadowedString(spriteBatch, spriteFont, text, position,
                shadowOffset, foreColor);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public Color GetShadowColorFromAlpha(float alpha)
        {
            return GraphicsHelper.GetShadowColorFromAlpha(alpha);
        }
        public Color GetColorWithAlpha(Color col, float alpha)
        {
            return GraphicsHelper.GetColorWithAlpha(col, alpha);
        }
        public Color GetRandomColor()
        {
            return GraphicsHelper.GetRandomColor();
        }
        #endregion
    }
}