#if ZUNE
#else

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
        Dictionary<string, Effect> effects = new Dictionary<string,Effect>();
        public Effect LoadEffect(string spath)
        {
            return LoadEffect(getNameFromPath(spath), Content.Load<Effect>(spath));
        }
        public Effect LoadEffect(string spath, string sname)
        {
            return LoadEffect(sname, Content.Load<Effect>(spath));
        }
        public Effect LoadEffect(string name, Effect data)
        {
            name = name.ToLower();
            if (effects.ContainsKey(name))
            {
                if (ContentDuplicateBehavior == ContentDuplicateBehaviors.Replace)
                    effects.Remove(name);
                else
                    return effects[name];
            }
            effects.Add(name, data);
            return data;
        }
        /***********************************************/
        public Effect GetEffect(string name)
        {
            try
            {
                name = name.ToLower();
                return effects[name];
            }
            catch
            {
                SayMessage("GetEffect(string): Couldn't find effect " + name);
                return effects["none"];
            }
        }
        public int GetEffectPassesCount(string name)
        {
            return (GetEffectPassesCount(GetEffect(name)));
        }
        public int GetEffectPassesCount(Effect name)
        {
            return name.CurrentTechnique.Passes.Count;
        }
        public void UseEffect(string effect)
        {
            SpriteBatch.End();
            SpriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, GetEffect(effect));
        }
        public void UseEffect(Effect effect)
        {
            SpriteBatch.End();
            SpriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null,effect);
        }
        public void RestartBatch()
        {
            SpriteBatch.End();
            SpriteBatch.Begin();
        }
    }
}

#endif