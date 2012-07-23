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
#if !WINDOWS_PHONE
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
        Dictionary<string, Video> videos;
        public List<VideoPlayer> videoPlayers;

        public void LoadVideo(string path)
        {
            LoadVideo(getNameFromPath(path), Content.Load<Video>(path));
        }
        public void LoadVideo(string path, string name)
        {
            LoadVideo(name, Content.Load<Video>(path));
        }
        public void LoadVideo(string name, Video data)
        {
            name = name.ToLower();
            if (videos.ContainsKey(name))
            {
                if (ContentDuplicateBehavior == ContentDuplicateBehaviors.Replace)
                    videos.Remove(name);
                else
                    return;
            }
            videos.Add(name, data);
        }

        public Video GetVideo(string name)
        {
            name = name.ToLower();
            try
            {
                return videos[name];
            }
            catch
            {
                return videos["error"];
            }
        }

        public string[] VideosKeys { get { return videos.Keys.ToArray(); } }
    }
#endif
}