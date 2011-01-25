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
#if WINDOWS
 
 
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
 
namespace Neat
{
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
        protected GameTime gamesTime;
        Dictionary<String, Sprite> textures = new Dictionary<string, Sprite>();

        public void LoadTexture(string path)
        {
            string textureName = getNameFromPath(path).ToLower();
            try
            {
                if (textures.ContainsKey(textureName))
                {
                    if (ContentDuplicateBehavior == ContentDuplicateBehaviors.Replace)
                        textures.Remove(textureName);
                    else
                        return;
                }
                textures.Add(textureName,
                    new Sprite(textureName,
                    Content.Load<Texture2D>(path)));
            }
            catch
            {
                SayMessage("Cannot Load {" + path + "}");
            }
        }
        public void LoadTexture(string path, string name)
        {

            try
            {
                name = name.ToLower();
                if (textures.ContainsKey(name))
                {
                    if (ContentDuplicateBehavior == ContentDuplicateBehaviors.Replace)
                        textures.Remove(name);
                    else
                        return;
                }
                textures.Add(name,
                    new Sprite(name,
                    Content.Load<Texture2D>(path)));
            }
            catch
            {
                SayMessage("Cannot load {" + name + "}");
            }
        }

        public void AssignTexture(string Sourcename, string Destname)
        {
            try
            {
                textures.Add(Destname.ToLower(),
                    textures[Destname.ToLower()]);
            }
            catch
            {
                SayMessage("Cannot assign {" + Destname + "} to {" + Sourcename + "}");
            }
        }
        public void LoadTexture(string name, double frameRate, params string[] paths)
        {
            name = name.ToLower();
            List<Texture2D> frames = new List<Texture2D>();
            foreach (string path in paths)
            {
                frames.Add(
                    Content.Load<Texture2D>(path));
            }
            if (textures.ContainsKey(name))
            {
                if (ContentDuplicateBehavior == ContentDuplicateBehaviors.Replace)
                    textures.Remove(name);
                else
                    return;
            }
            textures.Add(name,
                new Sprite(name,
                frames, frameRate));
        }
        
        public Texture2D GetTexture(string name)
        {
            try
            {
                return textures[name.ToLower()].GetTexture(Frame);
            }
            catch
            {
                return textures["error"].GetTexture(Frame);
            }
        } //Texture using default time
        public Texture2D GetTexture(string name, GameTime gt)
        {
            try
            {
                return textures[name.ToLower()].GetTexture(gt);
            }
            catch
            {
                return textures["error"].GetTexture(gt);
            }
        }

        public Sprite GetSprite(string name)
        {
            try
            {
                return textures[name.ToLower()];
            }
            catch
            {
                return textures["error"];
            }
        }

        protected string getNameFromPath(string path)
        {
            int bs = 0;
            string fname = "";
            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == '\\')
                {
                    bs = i;
                    break;
                }
            }
            if (bs == 0) return fname;
            for (int i = bs + 1; i < path.Length; i++)
            {
                fname += path[i].ToString();
            }
            return fname;
        }
        

    }
}