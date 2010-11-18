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
        public MenuSystem.MenuSystem System;
        public SpriteFont Font;

        public override void Initialize()
        {
            Reset();
            base.Initialize();
        }
        public virtual void Reset()
        {
            System = new Neat.MenuSystem.MenuSystem(
                game,
                new Vector2(game.GameWidth / 2, game.GameHeight / 2 - 100),
                Font);
            CreateMenu();
        }
        public virtual void CreateMenu()
        {
        }
        public override void Activate()
        {
            base.Activate();
            System.Enable();
            System.Position = new Vector2(game.GameWidth / 2, game.GameHeight / 2 - 100);
        }
        public override void Behave(GameTime gameTime)
        {
            System.Update(gameTime);
            base.Behave(gameTime);
        }

        public override void LoadContent()
        {
            Font = game.GetFont("menuFont");
            base.LoadContent();
        }

        public override void Render(GameTime gameTime)
        {
            System.Draw(gameTime);
            base.Render(gameTime);
        }
    }
}
