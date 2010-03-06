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
using Microsoft.Xna.Framework.Media;
using RedBulb;
using RedBulb.MenuSystem;
#endregion
namespace RedBulb
{
    public partial class RedBulbGame : Microsoft.Xna.Framework.Game
    {
        #region SFX
        Dictionary<string, SoundEffect> sounds;
        public bool muteAllSounds = false;
        public void LoadSound(string spath)
        {
            LoadSound(getNameFromPath(spath), Content.Load<SoundEffect>(spath));
        }
        public void LoadSound(string spath, string sname)
        {
            LoadSound(sname, Content.Load<SoundEffect>(spath));
        }
        public void LoadSound(string name, SoundEffect data)
        {
            name = name.ToLower();
            sounds.Add(name, data);
        }

        public void PlaySound(string name)
        {
            if (name == null) return;
            GetSound(name).Play();
        }
        public void PlaySound(string name, float volume)
        {
            if (name == null) return;
            GetSound(name).Play(volume,0f,0f);
        }
        public SoundEffect GetSound(string name)
        {
            if (muteAllSounds)
            {
                return sounds["mute"];
            }

            
            try
            {
                name = name.ToLower();
                return sounds[name];
            }
            catch
            {
                return sounds["mute"];
            }
        }
        #endregion
        #region Songs
        Dictionary<string, Song> songs;
        public void LoadSong(string spath)
        {
            LoadSong(getNameFromPath(spath), Content.Load<Song>(spath));
        }
        public void LoadSong(string spath, string sname)
        {
            LoadSong(sname, Content.Load<Song>(spath));
        }
        public void LoadSong(string name, Song data)
        {
            name = name.ToLower();
            songs.Add(name, data);
        }

        public Song GetSong(string name)
        {
            try
            {
                name = name.ToLower();
                return songs[name];
            }
            catch
            {
                return songs["blank"];
            }
        }
        #endregion
    }
}