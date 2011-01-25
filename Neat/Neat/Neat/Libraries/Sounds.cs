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
        #region SFX
        Dictionary<string, SoundEffect> sounds;
        public bool muteAllSounds = false;
        public SoundEffect LoadSound(string spath)
        {
            return LoadSound(getNameFromPath(spath), Content.Load<SoundEffect>(spath));
        }
        public SoundEffect LoadSound(string spath, string sname)
        {
            return LoadSound(sname, Content.Load<SoundEffect>(spath));
        }
        public SoundEffect LoadSound(string name, SoundEffect data)
        {
            if (sounds.ContainsKey(name))
            {
                if (ContentDuplicateBehavior == ContentDuplicateBehaviors.Replace)
                    sounds.Remove(name);
                else
                    return sounds[name];
            }
            name = name.ToLower();
            sounds.Add(name, data);
            return data;
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
        public Song LoadSong(string spath)
        {
            return LoadSong(getNameFromPath(spath), Content.Load<Song>(spath));
        }
        public Song LoadSong(string spath, string sname)
        {
            return LoadSong(sname, Content.Load<Song>(spath));
        }
        public Song LoadSong(string name, Song data)
        {
            name = name.ToLower();
            if (songs.ContainsKey(name))
            {
                if (ContentDuplicateBehavior == ContentDuplicateBehaviors.Replace)
                    songs.Remove(name);
                else
                    return songs[name];
            }
            
            songs.Add(name, data);
            return data;
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