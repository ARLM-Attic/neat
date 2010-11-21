﻿using System;
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
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
 
namespace Neat
{
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
        Dictionary<string, SpriteFont> fonts;
        public SpriteFont LoadFont(string spath)
        {
            return LoadFont(getNameFromPath(spath), Content.Load<SpriteFont>(spath));
        }
        public SpriteFont LoadFont(string spath, string sname)
        {
            return LoadFont(sname, Content.Load<SpriteFont>(spath));
        }
        public SpriteFont LoadFont(string name, SpriteFont data)
        {
            name = name.ToLower();
            fonts.Add(name, data);
            return data;
        }

        public SpriteFont GetFont(string name)
        {
            name = name.ToLower();
            try
            {
                return fonts[name];
            }
            catch
            {
                return fonts["normal"];
            }
        }
    }
}