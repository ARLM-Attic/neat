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
#if !XNA_4_FINAL
    public class Video
    {
    }
    public class VideoPlayer
    {
    }
#endif
#if !WINDOWS_PHONE
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
        Dictionary<string, Video> videos;
        public List<VideoPlayer> videoPlayers;

        public void LoadVideo(string path)
        {
#if XNA_4_FINAL
            LoadVideo(getNameFromPath(path), Content.Load<Video>(path));
#endif
        }
        public void LoadVideo(string path, string name)
        {
#if XNA_4_FINAL
            LoadVideo(name, Content.Load<Video>(path));
#endif
        }
        public void LoadVideo(string name, Video data)
        {
            name = name.ToLower();
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
                return GetVideo("error");
            }
        }
#if XNA_4_FINAL
        public Texture2D GetVideoTexture(int playerID)
        {
            return videoPlayers[playerID].GetTexture();
        }
#endif
    }
#endif
}