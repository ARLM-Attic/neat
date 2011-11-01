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
        public class SFXList : List<SoundEffect>
        {
            static Random RandomGenerator = new Random();
            public int CurrentIndex = 0;
            public bool Shuffle = true;

            public SoundEffect GetNext()
            {
                if (Shuffle)
                    CurrentIndex = RandomGenerator.Next(Count);
                else
                    CurrentIndex = (CurrentIndex + 1) % Count;

                return this[CurrentIndex];
            }

            public SoundEffect GetPrevious()
            {
                if (Shuffle)
                    CurrentIndex = RandomGenerator.Next(Count);
                else
                {
                    CurrentIndex--;
                    if (CurrentIndex < 0) CurrentIndex = Count - 1;
                }

                return this[CurrentIndex];
            }
        }

        Dictionary<string, SFXList> sounds;
        public bool muteAllSounds = false;
        public SFXList LoadSound(string spath)
        {
            return LoadSound(getNameFromPath(spath), Content.Load<SoundEffect>(spath));
        }
        public SFXList LoadSound(string spath, string sname)
        {
            return LoadSound(sname, Content.Load<SoundEffect>(spath));
        }
        public SFXList LoadSound(string name, SoundEffect data)
        {
            name = name.ToLower();
            if (sounds.ContainsKey(name))
            {
                if (ContentDuplicateBehavior == ContentDuplicateBehaviors.Replace)
                    sounds.Remove(name);
                else
                    return sounds[name];
            }
            var sfxlist = new SFXList { data };
            sounds.Add(name, sfxlist);
            return sfxlist;
        }

        public SFXList LoadSounds(string name, params string[] paths)
        {
            SFXList sfxlist = new SFXList();
            
            foreach (var item in paths)
            {
                sfxlist.AddRange(LoadSound(item));
            }
            if (sounds.ContainsKey(name))
            {
                if (ContentDuplicateBehavior == ContentDuplicateBehaviors.Replace)
                    sounds.Remove(name);
                else
                    return sounds[name];
            }
            sounds.Add(name, sfxlist);
            return sfxlist;
        }

        public void PlaySound(string name, float volume=1.0f, float pitch=0.0f, float pan=0.0f)
        {
            if (name == null || muteAllSounds || name == "mute") return;
            GetSound(name).Play(volume,pitch,pan);
        }
        public SoundEffect GetSound(string name)
        {
            /*if (muteAllSounds)
            {
                return sounds["mute"].GetNext();
            }*/

            return GetSFXList(name).GetNext();
        }

        public SFXList GetSFXList(string name)
        {
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