﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
using Neat.EasyMenus;
using Neat.GUI;
using Neat.Mathematics;
using Neat.Graphics;

namespace NeatStarter
{
    public class StartScreen : Screen
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
                game.Console.Run("sh " + NextScreen);
            base.Behave(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);
        }
        
        public override void Render(GameTime gameTime)
        {
            var t = game.GetTexture("neatlogo");
            Vector2 p = new Vector2( (game.GameWidth - t.Width) / 2f, (game.GameHeight - t.Height) / 2f);
            game.GetEffect("ColorFilter").Parameters["color"].SetValue(new Vector4(MathHelper.Clamp(alpha, 0.0f, 1.0f)));
            game.UseEffect("ColorFilter");
            game.SpriteBatch.Draw(t, p, Color.White);
            game.RestartBatch();
            base.Render(gameTime);
        }
    }
}
