﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if LIVE
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
#if WINDOWS

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
    public class StartScreen : Screen
    {
        float alpha ;
        float fadeRate = 0.01f;
        bool fade = true;
        bool finished = false;
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
            if (fade)
                alpha += fadeRate;

            if (alpha > 2.0f)
            {
                if (!finished)
                {
                    fade = false;
                    alpha = 0f;
                    game.Console.Run("sh (nextscreen)");
                    finished = true;
                }
            }
            base.Behave(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);
        }

        public override void Render(GameTime gameTime)
        {
            if (fade)
            {
                var t = game.GetTexture("neatlogo");
                Vector2 p = new Vector2((game.GameWidth - t.Width) / 2f, (game.GameHeight - t.Height) / 2f);
                game.GetEffect("ColorFilter").Parameters["color"].SetValue(new Vector4(MathHelper.Clamp(alpha, 0.0f, 1.0f)));
                game.UseEffect("ColorFilter");
                game.SpriteBatch.Draw(t, p, Color.White);
                game.RestartBatch();
            }
            game.Write("Press ~ to open the console. \nPress Alt-F4 to quit.", new Vector2(0, game.GameHeight - 40));
            base.Render(gameTime);
        }
    }
}
