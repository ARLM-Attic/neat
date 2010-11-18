using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Microsoft.Xna.Framework.GamerServices;
namespace Neat.Components
{
    public partial class Console : GameComponent
    {
        void f_load(IList<string> args)
        {
            try
            {
                if (args[2] == "as") //load assettype as assetname assetpath
                {
                    string assetType = args[1];
                    string assetName = args[3];
                    string assetPath = Args2Str(args, 4);
                    if (assetType == "texture") game.LoadTexture(assetPath, assetName);
                    else if (assetType == "sfx") game.LoadSound(assetPath, assetName);
                    else if (assetType == "fx") game.LoadEffect(assetPath, assetName);
                    else if (assetType == "font") game.LoadFont(assetPath, assetName);
                    else if (assetType == "song") game.LoadSong(assetPath, assetName);
#if WINDOWS
                    else if (assetType == "video") game.LoadVideo(assetPath, assetName);
#endif
                    else { WriteLine("Error in Load (as)"); }
                }
                else //load assettype assetpath
                {
                    string assetType = args[1];
                    string assetPath = args[2];
                    if (assetType == "texture") game.LoadTexture(assetPath);
                    else if (assetType == "sfx") game.LoadSound(assetPath);
                    else if (assetType == "fx") game.LoadEffect(assetPath);
                    else if (assetType == "font") game.LoadFont(assetPath);
                    else if (assetType == "song") game.LoadSong(assetPath);
#if WINDOWS
                    else if (assetType == "video") game.LoadVideo(assetPath);
#endif

                    else { WriteLine("Error in Load"); }
                }
            }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }
    }
}