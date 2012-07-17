using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Neat
{
    public class DVar
    {
        public string Value = "";

        public int ReturnInt() { return int.Parse(Value); }
        public string ReturnString() { return Value; }
        public float ReturnFloat() { return float.Parse(Value); }
        public bool ReturnBool() { return bool.Parse(Value); }

        //TODO: Implement Get/Set for the value: _value = Set(input); return Get(input)
        public Action<object> SetValue;
        public Func<string> GetValue;
        public DVar()
        {
            SetValue = new Action<object>(_setValue);
            GetValue = new Func<string>(() => {return Value;});
        }

        void _setValue(object o)
        {
            Value = o.ToString();
        }
    };

    public class RAM : Dictionary<string,DVar>
    {
        NeatGame _game;
        bool standAlone = false;
        public RAM(NeatGame game) : base()
        {
            _game = game;
        }
        public RAM()
            : base()
        {
            standAlone = true;
        }
        public void Add(string key, string value)
        {
            key = key.Trim().ToLower();
            if (!standAlone)
            {
                // check internal dvars
                try
                {
                    // GRAPHICS
                    if (key == "g_width") _game.GameWidth = int.Parse(value);
                    else if (key == "g_height") _game.GameHeight = int.Parse(value);
                    else if (key == "g_fullscreen") _game.FullScreen = bool.Parse(value);
                    else if (key == "g_autoclear") _game.AutoClear = bool.Parse(value);

                    // SFX/ MUSIC
                    else if (key == "a_mediavolume") MediaPlayer.Volume = float.Parse(value);
                    else if (key == "a_mediamute") MediaPlayer.IsMuted = bool.Parse(value);
                    else if (key == "a_mediashuffle") MediaPlayer.IsShuffled = bool.Parse(value);
                    else if (key == "a_mediarepeat") MediaPlayer.IsRepeating = bool.Parse(value);
                    else if (key == "a_mute") _game.MuteAllSounds = bool.Parse(value);

                    // ENGINE
                    else if (key == "e_freeze") _game.Freezed = bool.Parse(value);
                }
                catch { }
            }
            DVar v = new DVar();
            v.Value = value;
            if (ContainsKey(key)) this[key] = v;
            else this.Add(key, v);
        }
        public string GetValue(string key)
        {
            if (!standAlone)
            {
                try
                {
                    // GRAPHICS
                    if (key == "g_width") return _game.GameWidth.ToString();
                    else if (key == "g_height") return _game.GameHeight.ToString();
                    else if (key == "g_fullscreen") return _game.FullScreen.ToString();
                    else if (key == "g_autoclear") return _game.AutoClear.ToString();

                    // SFX/ MUSIC
                    else if (key == "a_mediavolume") return MediaPlayer.Volume.ToString();
                    else if (key == "a_mediamute") return MediaPlayer.IsMuted.ToString();
                    else if (key == "a_mediashuffle") return MediaPlayer.IsShuffled.ToString();
                    else if (key == "a_mediarepeat") return MediaPlayer.IsRepeating.ToString();
                    else if (key == "a_mute") return _game.MuteAllSounds.ToString();

                    // ENGINE
                    else if (key == "e_freeze") return _game.Freezed.ToString();
                }
                catch { return "#ERROR"; }
            }
            key = key.Trim().ToLower();
            if (ContainsKey(key))
                return this[key].ReturnString();
            else
                return key; // "False"; // "#ERROR";
        }
    }
}
