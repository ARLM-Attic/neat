using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
namespace Neat.Components
{
    public partial class Console : GameComponent
    {
        public IList<string> AutocompleteColors(IList<string> args)
        {
            return new string[] {
                "black",
                "blue",
                "red",
                "green",
                "yellow",
                "purple",
                "brown",
                "white",
                "gray",
                "transparent" };
        }

        public IList<string> AutocompleteSprites(IList<string> args)
        {
            return game.Sprites.Keys.ToArray();
        }

        public IList<string> AutocompleteEffects(IList<string> args)
        {
            return game.EffectsKeys;
        }

        public IList<string> AutocompleteFonts(IList<string> args)
        {
            return game.FontsKeys;
        }

        public IList<string> AutocompleteSounds(IList<string> args)
        {
            return game.SoundsKeys;
        }

        public IList<string> AutocompleteSongs(IList<string> args)
        {
            return game.SongsKeys;
        }

        public IList<string> AutocompleteVideos(IList<string> args)
        {
            return game.VideosKeys;
        }

        public IList<string> AutocompleteDvars(IList<string> args)
        {
            return Ram.Keys.ToList();
        }

        public IList<string> AutocompleteFiles(IList<string> args)
        {
            var files = Directory.GetFiles(".\\", "*.*");
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].ToLower().Replace(".\\", "");
            }
            return files;
        }

        public IList<string> AutocompleteContentFiles(IList<string> args)
        {
            var files = Directory.GetFiles(game.Content.RootDirectory, "*.xnb", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].ToLower().Replace(".xnb", "");
            }
            return files;
        }

        public IList<string> AutocompleteBoolean(IList<string> args)
        {
            return new string[] { "true", "false" };
        }

        void InitAutocompletes()
        {
            Autocompletes.Add("dvar", AutocompleteDvars);
            Autocompletes.Add("dv", AutocompleteDvars);
            Autocompletes.Add("call", AutocompleteFiles);
            Autocompletes.Add("free", AutocompleteDvars);
            Autocompletes.Add("inc", AutocompleteDvars);
            Autocompletes.Add("type", AutocompleteFiles);
            Autocompletes.Add("c_textcolor", AutocompleteColors);
            Autocompletes.Add("c_inputcolor", AutocompleteColors);
            Autocompletes.Add("c_backcolor", AutocompleteColors);

            Autocompletes.Add("g_stretchmode", o =>
            {
                return new string[] { "none", "fit", "stretch", "center" };
            });
            Autocompletes.Add("g_backcolor", AutocompleteColors);
            Autocompletes.Add("g_outputcolor", AutocompleteColors);
            Autocompletes.Add("g_fullscreen", AutocompleteBoolean);
            Autocompletes.Add("g_autoclear", AutocompleteBoolean);
            Autocompletes.Add("g_showmouse", AutocompleteBoolean);
            Autocompletes.Add("g_messagecolor", AutocompleteBoolean);
            Autocompletes.Add("g_assigntexture", AutocompleteSprites);
            //Autocompletes.Add("c_textcolor", AutocompleteColors);
            //Autocompletes.Add("c_inputcolor", AutocompleteColors);
            //Autocompletes.Add("c_backcolor", AutocompleteColors);
            Autocompletes.Add("cfx_color", AutocompleteColors);
            Autocompletes.Add("c_allowcolors", AutocompleteBoolean);
            Autocompletes.Add("c_setcolorcode", o =>
            {
                return o.Count < 3 ? new string[] {} : AutocompleteColors(o);
            });
            Autocompletes.Add("c_removecolorcode", o =>
            {
                List<string> s = new List<string>();
                foreach (var item in ColorsTable)
                    s.Add(item.Key.ToString());
                return s;
            });
            Autocompletes.Add("c_texture", AutocompleteSprites);
            Autocompletes.Add("c_font", AutocompleteFonts);
            Autocompletes.Add("c_showonbottom", AutocompleteBoolean);
            Autocompletes.Add("c_sfx", AutocompleteSounds);
            Autocompletes.Add("a_sfx", AutocompleteSounds);
            Autocompletes.Add("a_mediaplay", o =>
                {
                    var l = AutocompleteSongs(o);
                    l.Add("library");
                    return l;
                });
            Autocompletes.Add("a_mutesounds", AutocompleteSounds);
            Autocompletes.Add("a_mutesongs", AutocompleteSongs);
            Autocompletes.Add("a_mediashuffle", AutocompleteBoolean);
            Autocompletes.Add("a_mediarepeat", AutocompleteBoolean);
            Autocompletes.Add("e_lockfps", AutocompleteBoolean);
            Autocompletes.Add("load", o =>
                {
                    if (o.Count == 2)
                    {
                        return new string[] {
                            "texture", "directtexture", "sfx", "fx", "font", "song", "script",
                            "video" };
                    }
                    else if (o.Count == 3 || o.Count == 5)
                    {
                        var lastarg = o[1];
                        var l = new List<string> { "as" };
                        if (lastarg == "script") return l;
                        return AutocompleteContentFiles(o).Union(l).ToList();
                    }
                    else return new string[] { };
                });
        }
    }
}