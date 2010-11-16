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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
using Neat.EasyMenus;
using Neat.GUI;
using Neat.Mathematics;
using Neat.Graphics;

namespace Neat
{
    public class StartScreen : GamePart
    {
        float alpha ;
        float fadeRate = 0.01f;
        public string NextScreen = "";

        public StartScreen(NeatGame Game)
            : base(Game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Activate()
        {
            alpha = -0.8f;
            base.Activate();
        }

        public override void Behave(GameTime gameTime)
        {
            alpha += fadeRate;
            if (alpha > 2.0f && NextScreen.Trim().Length > 0)
                game.console.Run("ap " + NextScreen);
            base.Behave(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);
        }


        public override void Render(GameTime gameTime)
        {
            var t = game.getTexture("neatlogo");
            Vector2 p = new Vector2( (game.gameWidth - t.Width) / 2f, (game.gameHeight - t.Height) / 2f);
            game.GetEffect("ColorFilter").Parameters["mulColor"].SetValue(new Vector4(MathHelper.Clamp(alpha,0.0f,1.0f)));
            game.UseEffect("ColorFilter");
            game.spriteBatch.Draw(t, p, Color.White);
            base.Render(gameTime);
        }
    }
}
