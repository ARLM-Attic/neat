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
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
using System.Diagnostics;
using System.IO;
using Neat.Mathematics;
 
namespace Neat
{
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
        public TimeSpan TotalTime { get { return gamestime == null ? new TimeSpan(0) : gamestime.TotalGameTime; } }
        protected GameTime gamesTime;
        Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        public Dictionary<string, Sprite> Sprites { get { return sprites; } }

        public Texture2D LoadTexture(string path, string name=null, bool viaContentManager=true)
        {
            try
            {
                if (name == null)
                    name = getNameFromPath(path).ToLower();
                else
                    name = name.ToLower();

                if (sprites.ContainsKey(name))
                {
                    if (ContentDuplicateBehavior == ContentDuplicateBehaviors.Replace)
                        sprites.Remove(name);
                    else
                        return sprites[name].Textures[0].Texture;
                }

                if (viaContentManager)
                    sprites.Add(name, new Sprite(Content.Load<Texture2D>(path)));
                else
                {
                    using (Stream titleStream = TitleContainer.OpenStream(path))
                    {
                        sprites.Add(name, new Sprite(Texture2D.FromStream(GraphicsDevice, titleStream)));
                    }
                }

                return sprites[name].Textures[0].Texture;
            }
            catch
            {
                if (viaContentManager && File.Exists(path)) LoadTexture(path, name, false);
                SayMessage("Cannot load {" + name + "}");

                return null;
            }
        }

        public void AssignTexture(string sourcename, string destname)
        {
            try
            {
                sprites[destname.ToLower()] = sprites[destname.ToLower()];
            }
            catch
            {
                SayMessage("Cannot assign {" + destname + "} to {" + sourcename + "}");
            }
        }

        public void AssignTexture(Sprite source, string Destname)
        {
            try
            {
                sprites[Destname.ToLower()] = source;
            }
            catch
            {
                SayMessage("Cannot assign {" + Destname + "}");
            }
        }

        public void AssignTexture(Sprite.Slice source, string Destname)
        {
            try
            {
                sprites[Destname.ToLower()] = new Sprite(source);
            }
            catch
            {
                SayMessage("Cannot assign {" + Destname + "}");
            }
        }

        public void CreateSprite(string name, string spritesheet, Vector2 sliceSize, double framerate, bool horizontal)
        {
            if (!sprites.ContainsKey(spritesheet = spritesheet.ToLower()))
            {
                SayMessage("Sprite " + spritesheet + " not found!");
                return;
            }
            var tex = GetTexture(spritesheet);
            List<Sprite.Slice> frames = new List<Sprite.Slice>();
            Point count = new Point((int)(tex.Width / sliceSize.X), (int)(tex.Height / sliceSize.Y));

            for (int i = 0; i < count.X * count.Y; i++)
            {
                Vector2 pos = new Vector2(
                    horizontal ? (i % count.X) * sliceSize.X : (i / count.X) * sliceSize.X,
                    horizontal ? (i / count.Y) * sliceSize.Y : (i % count.Y) * sliceSize.Y);
                frames.Add(new Sprite.Slice(tex, GeometryHelper.Vectors2Rectangle(pos, sliceSize)));
            }

            sprites[name] = new Sprite(framerate, frames);
        }

        public void CreateSprite(string name, string spritesheet, Rectangle slice, double framerate = 0, int count=1, bool horizontal=true)
        {
            if (!sprites.ContainsKey(spritesheet = spritesheet.ToLower()))
            {
                SayMessage("Sprite " + spritesheet + " not found!");
                return;
            }

            name = name.ToLower();
            if (sprites.ContainsKey(name))
            {
                if (ContentDuplicateBehavior != ContentDuplicateBehaviors.Replace)
                    return;
            }

            Texture2D tex = GetTexture(spritesheet);
            List<Sprite.Slice> frames = new List<Sprite.Slice>();
            for (int i = 0; i < count; i++)
            {
                if (slice.X + slice.Width > tex.Width ||
                    slice.Y + slice.Height > tex.Height ||
                    slice.X < 0 || slice.Y < 0)
                {
                    SayMessage("Slice is outside texture boundaries ("+name+")", true);
                    return;
                }
                frames.Add(new Sprite.Slice(tex, slice));
                if (horizontal) slice.X += slice.Width;
                else slice.Y += slice.Height;
            }

            sprites[name] = new Sprite(framerate, frames);
        }

        public void CreateSprite(string name, string spritesheet, Vector2 size, double frameRate,
            params Vector2[] positions)
        {
            if (positions.Length == 0) return;
            name = name.ToLower();
            if (sprites.ContainsKey(name))
            {
                if (ContentDuplicateBehavior != ContentDuplicateBehaviors.Replace)
                    return;
            }

            List<Sprite.Slice> frames = new List<Sprite.Slice>();
            Texture2D tex = GetTexture(spritesheet);
            for (int i = 0; i < positions.Length; i++)
            {
                if (positions[i].X + size.X > tex.Width ||
                    positions[i].Y + size.Y > tex.Height ||
                    positions[i].X < 0 || positions[i].Y < 0)
                {
                    SayMessage("Slice is outside texture boundaries (" + name + ")", true);
                    return;
                }
                frames.Add(new Sprite.Slice(tex, GeometryHelper.Vectors2Rectangle(positions[i], size)));
            }

            sprites[name] = new Sprite(frameRate, frames);
        }

        public void CreateSprite(string name, double frameRate, KeyValuePair<string, Rectangle>[] slices)
        {
            name = name.ToLower();
            if (sprites.ContainsKey(name))
            {
                if (ContentDuplicateBehavior != ContentDuplicateBehaviors.Replace)
                    return;
            }

            List<Sprite.Slice> frames = new List<Sprite.Slice>();
            for (int i = 0; i < slices.Length; i++)
            {
                Texture2D tex = GetTexture(slices[i].Key);
                if (slices[i].Value.X + slices[i].Value.Width > tex.Width ||
                    slices[i].Value.Y + slices[i].Value.Height > tex.Height ||
                    slices[i].Value.X < 0 || slices[i].Value.Y < 0)
                {
                    SayMessage("Slice is outside texture boundaries ("+name+")", true);
                    return;
                }
                frames.Add(new Sprite.Slice(tex, slices[i].Value));
            }

            sprites[name] = new Sprite(frameRate, frames);
        }

        public void LoadTexture(string name, double frameRate, bool viaContentManager, params string[] paths)
        {
            name = name.ToLower();
            if (sprites.ContainsKey(name))
            {
                if (ContentDuplicateBehavior == ContentDuplicateBehaviors.Replace)
                    sprites.Remove(name);
                else return;
            }
            List<Sprite.Slice> frames = new List<Sprite.Slice>();
            int f = -1;
            foreach (string path in paths)
            {
                f++;
                string frameName = getNameFromPath(path);
                while (sprites.ContainsKey(frameName += "0") || frameName == name) ;
                
                Texture2D tex;
                if (sprites.ContainsKey(frameName)) tex = GetTexture(frameName);
                else
                {
                    LoadTexture(path, frameName, viaContentManager);
                    tex = GetTexture(frameName);
                }
                frames.Add(new Sprite.Slice(tex, 0, 0, tex.Width, tex.Height));
            }
            sprites.Add(name, new Sprite(frameRate, frames));
        }

        public void LoadTexture(string name, double frameRate, params string[] paths)
        {
            LoadTexture(name, frameRate, true, paths);
        }

        uint _freezedFrame;
        bool _freezeAnimations;
        GameTime _freezedGameTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero, false);
        public bool FreezeAnimations
        {
            get
            {
                return _freezeAnimations;
            }
            set
            {
                if (value) _freezedFrame = Frame;
                _freezeAnimations = value;
            }
        }

        public Sprite.Slice GetSlice(string name)
        {
            try
            {
                name = name.ToLower();
                if (sprites.ContainsKey(name))
                    return sprites[name].GetSlice(FreezeAnimations ? _freezedFrame : Frame);
                else
                    return sprites["error"].GetSlice(FreezeAnimations ? _freezedFrame : Frame);
            }
            catch
            {
                return sprites["error"].GetSlice(FreezeAnimations ? _freezedFrame : Frame);
            }
        } //Texture using default time
        public Sprite.Slice GetSlice(string name, GameTime gt)
        {
            try
            {
                name = name.ToLower();
                if (sprites.ContainsKey(name))
                    return sprites[name].GetSlice(FreezeAnimations ? _freezedGameTime : gt);
                else
                    return sprites["error"].GetSlice(FreezeAnimations ? _freezedGameTime : gt);
            }
            catch
            {
                return sprites["error"].GetSlice(FreezeAnimations ? _freezedGameTime : gt);
            }
        }

        public Texture2D GetTexture(string name, GameTime gt = null)
        {
            if (gt == null)
                return GetSlice(name).Texture;
            else
                return GetSlice(name, gt).Texture;
        }

        public Sprite GetSprite(string name)
        {
            try
            {
                name = name.ToLower();
                if (sprites.ContainsKey(name))
                    return sprites[name];
                else return sprites["error"];
            }
            catch
            {
                return sprites["error"];
            }
        }

        public Sprite.AnimationModes GetAnimationMode(string name)
        {
            if (sprites.ContainsKey(name = name.ToLower()))
                return sprites[name].AnimationMode;
            return Sprite.AnimationModes.Illegal;
        }

        public bool SetAnimationMode(string name, Sprite.AnimationModes mode)
        {
            if (sprites.ContainsKey(name = name.ToLower()))
            {
                sprites[name].AnimationMode = mode;
                return true;
            }
            return false;
        }

        public bool RewindAnimation(string name)
        {
            if (sprites.ContainsKey(name = name.ToLower()))
            {
                sprites[name].Rewind(gamesTime,Frame);
                return true;
            }
            return false;
        }
    }
}