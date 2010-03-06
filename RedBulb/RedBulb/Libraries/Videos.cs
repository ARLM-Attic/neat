#region References
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using RedBulb;
using RedBulb.MenuSystem;
//using UltimaScroll.IBLib;
#endregion
using Microsoft.Xna.Framework.Media;
namespace RedBulb
{
    public partial class RedBulbGame : Microsoft.Xna.Framework.Game
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

        public Texture2D GetVideoTexture(int playerID)
        {
            return videoPlayers[playerID].GetTexture();
        }
    }
}