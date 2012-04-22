using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Neat.Mathematics;

namespace Neat
{
    public class Sprite
    {
        public enum AnimationModes
        {
            Illegal,
            Normal,
            Repeat,
            PingPong,
            Freeze
        }

        public class Slice
        {
            public Texture2D Texture;
            public Rectangle? Crop;

            public Slice(Texture2D tex, Rectangle? crop = null)
            {
                create(tex, crop);
            }

            public Slice(Texture2D tex, int x, int y, int width, int height)
            {
                create(tex, new Rectangle(x, y, width, height));
            }

            void create(Texture2D tex, Rectangle? crop = null)
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


            public Vector2 Center
            {
                get
                {
                    if (Crop.HasValue)
                        return new Vector2(Crop.Value.Width / 2.0f, Crop.Value.Height / 2.0f);
                    return new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);
                }
            }

            public Vector2 Size
            {
                get
                {
                    if (Crop.HasValue)
                        return new Vector2(Crop.Value.Width, Crop.Value.Height);
                    return new Vector2(Texture.Width, Texture.Height);
                }
            }
        }

        List<Slice> textures;
        public List<Slice> Textures { get { return textures; } }
        int frames;
        int playingFrame;
        public double FrameRate;
        public AnimationModes AnimationMode = AnimationModes.Repeat;
        TimeSpan startTime;
        uint startGameFrame = 0;
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
            startTime = new TimeSpan(0);
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

        void fixAnimation()
        {
            if (AnimationMode == AnimationModes.Repeat)
            {
                playingFrame %= frames;
            }
            else if (AnimationMode == AnimationModes.Normal)
            {
                if (playingFrame >= frames) playingFrame = frames - 1;
                else if (playingFrame < 0) playingFrame = 0;
            }
            else if (AnimationMode == AnimationModes.PingPong)
            {
                playingFrame %= (frames * 2);
            }
        }

        public void Rewind(GameTime gameTime, uint gameFrame)
        {
            startTime = gameTime.TotalGameTime;
            startGameFrame = gameFrame;
            playingFrame = 0;
        }

        public Slice GetSlice(GameTime gameTime)
        {
            if (AnimationMode != AnimationModes.Freeze)
            if (FrameRate > 0)
            {
                playingFrame = (int)((gameTime.TotalGameTime-startTime).TotalMilliseconds / FrameRate);
            }
            fixAnimation();

            return textures[playingFrame >= frames ? 2*frames - playingFrame - 1 : playingFrame];
        }

        public Slice GetSlice(uint frame)
        {
            if (AnimationMode != AnimationModes.Freeze)
            if (FrameRate > 0)
            {
                playingFrame = (int)((frame-startGameFrame) / FrameRate);
            }
            fixAnimation();
            return textures[playingFrame >= frames ? 2 * frames - playingFrame - 1 : playingFrame];
        }

        public Slice GetSlice()
        {
            return textures[playingFrame];
        }
    }
}
