#if ZUNE
#else

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
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
        public void LoadEffect(string spath)
        {
            LoadEffect(getNameFromPath(spath), Content.Load<Effect>(spath));
        }
        public void LoadEffect(string spath, string sname)
        {
            LoadEffect(sname, Content.Load<Effect>(spath));
        }
        public void LoadEffect(string name, Effect data)
        {
            name = name.ToLower();
            effects.Add(name, data);
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
            spriteBatch.End();
            spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, GetEffect(effect));
        }
        public void UseEffect(Effect effect)
        {
            spriteBatch.End();
            spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null,effect);
        }
        public void RestartBatch()
        {
            spriteBatch.End();
            spriteBatch.Begin();
        }
#if TODO
        /***********************************************/
        public void TurnOnEffect(string effect)
        {
            TurnOnEffect(GetEffect(effect));
        }
        public void TurnOffEffect(string effect)
        {
            TurnOffEffect(GetEffect(effect));
        }
        public void TurnOnEffect(Effect effect)
        {
            UseEffect(effect);
            BeginEffectPass(effect, 0);
        }
        public void TurnOffEffect(Effect effect)
        {
            EndEffectPass(effect, 0);
            EndEffect(effect);
        }
        /***********************************************/
        public void UseEffect(string effect)
        {
            UseEffect(GetEffect(effect));
        }
        public void EndEffect(string effect)
        {
            EndEffect(GetEffect(effect));
        }
        public void UseEffect(Effect effect)
        {
            if (autoDraw) spriteBatch.End();
            spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend);
            effect.Begin();
        }
        public void EndEffect(Effect effect)
        {
            effect.End();
            spriteBatch.End();
            if (autoDraw) spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
        }
        /***********************************************/
        public void BeginEffectPass(string effect, int pass)
        {
            BeginEffectPass(GetEffect(effect), pass);
        }
        public void EndEffectPass(string effect, int pass)
        {
            EndEffectPass(GetEffect(effect), pass);
        }
        public void BeginEffectPass(Effect effect, int pass)
        {
            effect.CurrentTechnique.Passes[pass].Begin();
        }
        public void EndEffectPass(Effect effect, int pass)
        {
            effect.CurrentTechnique.Passes[pass].End();
        }
        /***********************************************/
#endif
    }
}

#endif