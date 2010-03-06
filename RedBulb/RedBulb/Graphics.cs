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
#endregion
namespace RedBulb
{
    public partial class RedBulbGame : Microsoft.Xna.Framework.Game
    {
        public Color backGroundColor = Color.Black;

        public bool autoDraw = true;
        public bool autoClear = true;
        public bool showMouse = true;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void Clear(Color c)
        {
            graphics.GraphicsDevice.Clear(c);
        }

        protected override void Draw(GameTime gameTime)
        {
            _dcps++;
            if (gameTime.TotalRealTime.Seconds != lastDrawTimeSpan.Seconds)
            {
                lastDrawTimeSpan = gameTime.TotalRealTime;
                drawCPS = _dcps;
                _dcps = 0;
            }

            gamestime = gameTime;
            
            if (autoClear)
                graphics.GraphicsDevice.Clear(backGroundColor);

            if (autoDraw)
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

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
                DrawMouse(mousePosition);
            }
            if (hasConsole)
            {
                if (!autoDraw) spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                console.Draw(0, 10);
                if (!autoDraw) spriteBatch.End();
            }
            if (autoDraw)
                spriteBatch.End();

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
        public void Write(string text, Vector2 position)
        {
            DrawShadowedString(normalFont, text, position, Color.White);
        }
        
        public void DrawShadowedString(SpriteFont spriteFont, string text, Vector2 position, Color foreColor, Color backColor)
        {
            spriteBatch.DrawString(spriteFont, text, position + (new Vector2(1, 1)), backColor);
            spriteBatch.DrawString(spriteFont, text, position, foreColor);
        }
        public void DrawShadowedString(SpriteFont spriteFont, string text, Vector2 position, Vector2 shadowOffset, Color foreColor, Color backColor)
        {
            spriteBatch.DrawString(spriteFont, text, position + shadowOffset, backColor);
            spriteBatch.DrawString(spriteFont, text, position, foreColor);
        }
        public void DrawShadowedString(SpriteFont spriteFont, string text, Vector2 position, Color foreColor)
        {
            spriteBatch.DrawString(spriteFont, text, position + (new Vector2(1, 1)), Color.Black);
            spriteBatch.DrawString(spriteFont, text, position, foreColor);
        }
        public void DrawShadowedString(SpriteFont spriteFont, string text, Vector2 position, Vector2 shadowOffset, Color foreColor)
        {
            spriteBatch.DrawString(spriteFont, text, position + shadowOffset, Color.Black);
            spriteBatch.DrawString(spriteFont, text, position, foreColor);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public Color GetShadowColorFromAlpha(float alpha)
        {
            Vector3 c = Color.Black.ToVector3();
            return new Color(new Vector4(c, alpha));
        }
        public Color GetColorWithAlpha(Color col, float alpha)
        {
            Vector3 c = col.ToVector3();
            return new Color(new Vector4(c, alpha));
        }
        public Color GetRandomColor()
        {
            return new Color((byte)randomGenerator.Next(0, 255),
                    (byte)randomGenerator.Next(0, 255),
                    (byte)randomGenerator.Next(0, 255));
        }
    }
}