using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            name = name.ToLower();
            return effects[name];
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
        public string[] EffectsKeys { get { return effects.Keys.ToArray(); } }
    }
}
