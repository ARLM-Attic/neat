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
namespace RedBulb
{
    public partial class RedBulbGame : Microsoft.Xna.Framework.Game
    {
        Dictionary<string, SpriteFont> fonts;
        public void LoadFont(string spath)
        {
            LoadFont(getNameFromPath(spath), Content.Load<SpriteFont>(spath));
        }
        public void LoadFont(string spath, string sname)
        {
            LoadFont(sname, Content.Load<SpriteFont>(spath));
        }
        public void LoadFont(string name, SpriteFont data)
        {
            name = name.ToLower();
            if (fonts.ContainsKey(name)) fonts.Remove(name);
            fonts.Add(name, data);
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
                return GetFont("Normal");
            }
        }
    }
}