using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
            if (fonts.ContainsKey(name))
            {
                if (ContentDuplicateBehavior == ContentDuplicateBehaviors.Replace)
                    fonts.Remove(name);
                else
                    return fonts[name];
            }
            
            fonts[name] = data;
            return data;
        }

        public SpriteFont GetFont(string name)
        {
            name = name.ToLower();
            try
            {
                if (!fonts.ContainsKey(name)) return fonts["normal"];
                return fonts[name];
            }
            catch
            {
                return fonts["normal"];
            }
        }

        public string[] FontsKeys { get { return fonts.Keys.ToArray(); } }
    }
}