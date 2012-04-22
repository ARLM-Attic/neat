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
using Microsoft.Xna.Framework.Media;
using Neat;

namespace Neat.Graphics
{
    public static class GraphicsHelper
    {
        static Random randomGenerator = new Random();

        public static void DrawShadowedString(SpriteBatch spriteBatch,SpriteFont spriteFont, string text, Vector2 position, Color foreColor, Color backColor)
        {
            spriteBatch.DrawString(spriteFont, text, position + (new Vector2(1)), backColor);
            spriteBatch.DrawString(spriteFont, text, position, foreColor);
        }
        public static void DrawShadowedString(SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Vector2 shadowOffset, Color foreColor, Color backColor)
        {
            spriteBatch.DrawString(spriteFont, text, position + shadowOffset, backColor);
            spriteBatch.DrawString(spriteFont, text, position, foreColor);
        }
        public static void DrawShadowedString(SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color foreColor)
        {
            spriteBatch.DrawString(spriteFont, text, position + (new Vector2(1)), Color.Black);
            spriteBatch.DrawString(spriteFont, text, position, foreColor);
        }
        public static void DrawShadowedString(SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Vector2 shadowOffset, Color foreColor)
        {
            spriteBatch.DrawString(spriteFont, text, position + shadowOffset, Color.Black);
            spriteBatch.DrawString(spriteFont, text, position, foreColor);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static Color GetColorWithAlpha(Color col, float alpha)
        {
            var c = col.ToVector4();
            return new Color(c * new Vector4(Vector3.One, alpha));
        }
        public static Color GetRandomColor(int min=0, int max=255)
        {
            return new Color((byte)randomGenerator.Next(min, max),
                    (byte)randomGenerator.Next(min, max),
                    (byte)randomGenerator.Next(min, max));
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        
    }
}
