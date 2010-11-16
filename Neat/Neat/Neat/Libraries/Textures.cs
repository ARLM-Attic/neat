using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
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
        Dictionary<String, IBTexture> textures = new Dictionary<string, IBTexture>();

        public virtual void LoadTexture(string path)
        {
            string textureName = getNameFromPath(path).ToLower();
            try
            {
                textures.Add(textureName,
                    new IBTexture(textureName,
                    Content.Load<Texture2D>(path)));
            }
            catch
            {
                SayMessage("Cannot Load {" + path + "}");
            }
        }
        public virtual void LoadTexture(string path, string name)
        {
            try
            {
                textures.Add(name.ToLower(),
                    new IBTexture(name,
                    Content.Load<Texture2D>(path)));
            }
            catch
            {
                SayMessage("Cannot load {" + name + "}");
            }
        }

        public virtual void AssignTexture(string Sourcename, string Destname)
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
        public virtual void LoadTexture(string name, double frameRate, params string[] paths)
        {
            List<Texture2D> frames = new List<Texture2D>();
            foreach (string path in paths)
            {
                frames.Add(
                    Content.Load<Texture2D>(path));
            }
            textures.Add(name.ToLower(),
                new IBTexture(name,
                frames, frameRate));
        }
        
        public virtual Texture2D getTexture(string name)
        {
            try
            {
                return textures[name.ToLower()].GetTexture(frame);
                //return textures[name.ToLower()].getTexture(gamesTime);
            }
            catch
            {
                return getTexture("error");
            }
        } //Texture using default time
        public virtual Texture2D getTexture(string name, GameTime gt)
        {
            try
            {
                return textures[name.ToLower()].GetTexture(gt);
            }
            catch
            {
                //try to load the file
                try
                {
                    LoadTexture(name);
                    return getTexture(name, gt);
                }
                catch
                {
                    return getTexture("error");
                }
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