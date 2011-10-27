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
 
namespace Neat
{
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
        protected GameTime gamesTime;
        Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        
        public void LoadTexture(string path, string name=null, bool viaContentManager=true)
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
                        return;
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
            }
            catch
            {
                if (viaContentManager && File.Exists(path)) LoadTexture(path, name, false);
                SayMessage("Cannot load {" + name + "}");
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
                else
                    return;
            }
            List<Sprite.Slice> frames = new List<Sprite.Slice>();
            int f = -1;
            foreach (string path in paths)
            {
                f++;
                string frameName = getNameFromPath(path);
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
        
        public Sprite.Slice GetSlice(string name)
        {
            try
            {
                name = name.ToLower();
                if (sprites.ContainsKey(name))
                    return sprites[name].GetSlice(Frame);
                else
                    return sprites["error"].GetSlice(Frame);
            }
            catch
            {
                return sprites["error"].GetSlice(Frame);
            }
        } //Texture using default time
        public Sprite.Slice GetSlice(string name, GameTime gt)
        {
            try
            {
                name = name.ToLower();
                if (sprites.ContainsKey(name))
                    return sprites[name].GetSlice(gt);
                else
                    return sprites["error"].GetSlice(gt);
            }
            catch
            {
                return sprites["error"].GetSlice(gt);
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
    }
}