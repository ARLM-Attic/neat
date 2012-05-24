using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neat.Mathematics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using Neat.Graphics;
using Neat.GUI;
using Microsoft.Xna.Framework.Media;

namespace Neat
{
    public class CreditsScreen : Screen
    {
        Transition trans; 

        public CreditsScreen(NeatGame Game)
            : base(Game)
        {
        }

        public override void HandleInput(GameTime gameTime)
        {
            if (game.IsTapped(Keys.Space, Keys.Enter, Keys.Escape) || game.IsMouseClicked())
                finish();
#if KINECT
            foreach (var item in game.Touch.TrackPoints)
            {
                if (item.Hold) finish();
            }
#endif
            base.HandleInput(gameTime);
        }

        void finish()
        {
            game.ActivateScreen("mainmenu", trans);
        }

        public override void Activate()
        {
            trans = game.Transition;
            base.Activate();
        }

        public override void Deactivate(string nextScreen)
        {
            base.Deactivate(nextScreen);
        }

        public override void Render(GameTime gameTime)
        {
            game.SpriteBatch.Draw(game.GetTexture("credits"),
                new Rectangle(0, 0, game.GameWidth, game.GameHeight),
                Color.White);
            game.RestartBatch();
            base.Render(gameTime);
        }
    }
}
