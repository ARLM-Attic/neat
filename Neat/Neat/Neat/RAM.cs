using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Neat
{
    public struct DVar
    {
        public string value;

        public int ReturnInt() { return int.Parse(value); }
        public string ReturnString() { return value; }
        public float ReturnFloat() { return float.Parse(value); }
        public bool ReturnBool() { return bool.Parse(value); }
    };

    public class RAM : Dictionary<string,DVar>
    {
        NeatGame _game;
        public RAM(NeatGame game) : base()
        {
            _game = game;
        }
        public void Add(string key, string value)
        {
            key = key.Trim().ToLower();
            // check internal dvars
            try
            {
                // GRAPHICS
                if (key == "g_width") _game.gameWidth = int.Parse(value);
                else if (key == "g_height") _game.gameHeight = int.Parse(value);
                else if (key == "g_fullscreen") _game.fullscreen = bool.Parse(value);
                else if (key == "g_autoclear") _game.autoClear = bool.Parse(value);

                // SFX/ MUSIC
                else if (key == "a_mediavolume") MediaPlayer.Volume = float.Parse(value);
                else if (key == "a_mediamute") MediaPlayer.IsMuted = bool.Parse(value);
                else if (key == "a_mediashuffle") MediaPlayer.IsShuffled = bool.Parse(value);
                else if (key == "a_mediarepeat") MediaPlayer.IsRepeating = bool.Parse(value);
                else if (key == "a_mute") _game.muteAllSounds = bool.Parse(value);

                // ENGINE
                else if (key == "e_freeze") _game.freezed = bool.Parse(value);
            }
            catch { }
            DVar v = new DVar();
            v.value = value;
            if (ContainsKey(key)) this[key] = v;
            else this.Add(key, v);
        }
        public string GetValue(string key)
        {
            try
            {
                // GRAPHICS
                if (key == "g_width") return _game.gameWidth.ToString();
                else if (key == "g_height") return _game.gameHeight.ToString();
                else if (key == "g_fullscreen") return _game.fullscreen.ToString();
                else if (key == "g_autoclear") return _game.autoClear.ToString();

                // SFX/ MUSIC
                else if (key == "a_mediavolume") return MediaPlayer.Volume.ToString();
                else if (key == "a_mediamute") return MediaPlayer.IsMuted.ToString();
                else if (key == "a_mediashuffle") return MediaPlayer.IsShuffled.ToString();
                else if (key == "a_mediarepeat") return MediaPlayer.IsRepeating.ToString();
                else if (key == "a_mute") return _game.muteAllSounds.ToString();

                // ENGINE
                else if (key == "e_freeze") return _game.freezed.ToString();
            }
            catch { return "#ERROR"; }
            key = key.Trim().ToLower();
            if (ContainsKey(key))
                return this[key].ReturnString();
            else
                return "#ERROR";
        }
    }
}
