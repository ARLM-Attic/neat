using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Neat
{
    public class Sprite
    {
        public class Slice
        {
            public Texture2D Texture;
            public Rectangle? Crop;

            public Slice(Texture2D tex, Rectangle? crop=null)
            {
                create(tex, crop);
            }

            public Slice(Texture2D tex, int x, int y, int width, int height)
            {
                create(tex, new Rectangle(x, y, width, height));
            }

            void create(Texture2D tex, Rectangle? crop=null)
            {
                Texture = tex;
                Crop = crop;
            }

            public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
            {
                spriteBatch.Draw(
                    Texture,
                    position,
                    Crop,
                    color);
            }
        }

        List<Slice> textures;
        int frames;
        int playingFrame;
        public double FrameRate;

        public Sprite(double frameRate, List<Slice> slices)
        {
            textures = slices;
            FrameRate = frameRate;
            create();
        }

        public Sprite(double frameRate, params Slice[] slices)
        {
            textures = new List<Slice>(slices);
            FrameRate = frameRate;
            create();
        } //Animated Texture

        public Sprite(Texture2D t, Rectangle? rect=null)
        {
            textures = new List<Slice> { new Slice(t,rect) };
            FrameRate = 0;
            create();
        } //Still Texture

        public Sprite(Slice t)
        {
            textures = new List<Slice>{t};
            FrameRate = 0;
            create();
        } //Still Texture

        void create()
        {
            frames = textures.Count;
            playingFrame = 0;
        }

        public Texture2D GetTexture(GameTime gameTime)
        {
            return GetSlice(gameTime).Texture;
        }

        public Texture2D GetTexture(uint frame)
        {
            return GetSlice(frame).Texture;
        }

        public Texture2D GetTexture()
        {
            return textures[playingFrame].Texture;
        }

        public Slice GetSlice(GameTime gameTime)
        {
            if (FrameRate > 0)
            {
                playingFrame = (int)(gameTime.TotalGameTime.TotalMilliseconds / FrameRate) % frames;
            }
            return textures[playingFrame];
        }

        public Slice GetSlice(uint frame)
        {
            if (FrameRate > 0)
            {
                playingFrame = (int)(frame / FrameRate) % frames;
            }
            return textures[playingFrame];
        }

        public Slice GetSlice()
        {
            return textures[playingFrame];
        }
    }
}
