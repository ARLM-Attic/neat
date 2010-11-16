using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Neat
{
    class Sprite
    {
        List<Texture2D> textures;
        int frames;
        int playingFrame;
        string name;
        public double FrameRate;

        public Sprite(string textureName, List<Texture2D> t, double rate)
        {
            textures = t;
            frames = textures.Count;
            name = textureName;
            FrameRate = rate;
            playingFrame = 0;
        } //Animated Texture

        public Sprite(string textureName, Texture2D t)
        {
            textures = new List<Texture2D>();
            textures.Add( t) ;
            frames = textures.Count;
            name = textureName;
            FrameRate = 0;
            playingFrame = 0;
        } //Still Texture

        public Texture2D GetTexture(GameTime gameTime)
        {
            if (FrameRate > 0)
            {
                playingFrame = (int)(gameTime.TotalGameTime.TotalMilliseconds / FrameRate) % frames;
            }
            return textures[playingFrame];
        }

        public Texture2D GetTexture(uint frame)
        {
            if (FrameRate > 0)
            {
                playingFrame = (int)(frame / FrameRate) % frames;
            }
            return textures[playingFrame];
        }

        public Texture2D GetTexture()
        {
            return textures[playingFrame];
        }
    }
}
