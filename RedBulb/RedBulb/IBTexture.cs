using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RedBulb
{
    class IBTexture
    {
        List<Texture2D> textures;
        int frames;
        int playingFrame;
        string name;
        double frameRate;

        public IBTexture(string textureName, List<Texture2D> t, double rate)
        {
            textures = t;
            frames = textures.Count;
            name = textureName;
            frameRate = rate;
            playingFrame = 0;
        } //Animated Texture

        public IBTexture(string textureName, Texture2D t)
        {
            textures = new List<Texture2D>();
            textures.Add( t) ;
            frames = textures.Count;
            name = textureName;
            frameRate = 0;
            playingFrame = 0;
        } //Still Texture

        public Texture2D GetTexture(GameTime gameTime)
        {
            if (frameRate > 0)
            {
                playingFrame = (int)(gameTime.TotalGameTime.TotalMilliseconds / frameRate) % frames;
                /*if (gameTime.TotalGameTime.Milliseconds % frameRate == 0)
                {
                    playingFrame++;
                    if (playingFrame >= frames) playingFrame = 0;
                }*/
            }
            return textures[playingFrame];
        }

        public Texture2D GetTexture(uint frame)
        {
            if (frameRate > 0)
            {
                playingFrame = (int)(frame / frameRate) % frames;
                /*if (gameTime.TotalGameTime.Milliseconds % frameRate == 0)
                {
                    playingFrame++;
                    if (playingFrame >= frames) playingFrame = 0;
                }*/
            }
            return textures[playingFrame];
        }

        public Texture2D GetTexture()
        {
            return textures[playingFrame];
        }
    }
}
