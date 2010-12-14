﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Neat.Graphics
{
    public class LineBrush
    {
        Texture2D texture;
        int _thickness;
        Vector2 x = new Vector2(1, 0);

        public LineBrush(GraphicsDevice device, int thickness)
        {
            Create(device, thickness);
        }

        public void Create(GraphicsDevice device, int thickness)
        {
            _thickness = thickness;
            texture = new Texture2D(device, 2, thickness * 2);
            int size = texture.Width * texture.Height;
            Color[] pixmap = new Color[size];
            for (int i = 0; i < size; i++)
                pixmap[i] = Color.White;
            texture.SetData(pixmap);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color)
        {
            /*if (point1.X > point2.X)
            {
                var temp = point1;
                point1 = point2;
                point2 = temp;
            }*/
            Vector2 difference, normalizedDifference, scale;
            float theta, rotation;

            Vector2.Subtract(ref point2, ref point1, out difference);

            Vector2.Normalize(ref difference, out normalizedDifference);
            Vector2.Dot(ref x, ref normalizedDifference, out theta);

            theta = (float)Math.Acos(theta);
            if (difference.Y < 0)
            {
                theta = -theta;
            }
            rotation = theta;

            float desiredLength = difference.Length();
            scale.X = desiredLength / texture.Width;
            scale.Y = 1;
            /*
            Vector2 distance = point2-point1;
            
            Vector2 normalizedDistance = Vector2.Normalize(distance);

            float length = distance.Length();


            if (length < 1) return;
            
            float dot = Vector2.Dot(normalizedDistance, x);
            float theta = (float)Math.Acos(dot) * Math.Sign(distance.Y);

            Vector2 scale = new Vector2(distance.Length() / texture.Width, 1);
            */
            spriteBatch.Draw(
                texture,
                point1,
                null,
                color,
                rotation, //theta,
                new Vector2(0, _thickness / 2f + 1), //Vector2.Zero,
                scale,
                SpriteEffects.None,
                0);
        }

        
    }
}