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
    public class Menu : GamePart
    {
        public Menu(NeatGame G)
            : base(G)
        {
        }
        public MenuSystem.MenuSystem system;
        public SpriteFont font;

        public override void Initialize()
        {
            system = new Neat.MenuSystem.MenuSystem(
                game,
                new Vector2(game.gameWidth / 2, game.gameHeight / 2 - 100),
                font);
            CreateMenu();
            base.Initialize();
        }
        public virtual void CreateMenu()
        {
        }
        public override void Activate()
        {
            system.Enable();
            base.Activate();
        }
        public override void Behave(GameTime gameTime)
        {
            system.Update(gameTime);
            base.Behave(gameTime);
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Render(GameTime gameTime)
        {
            system.Draw(gameTime);
            base.Render(gameTime);
        }
    }
}
