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
using Microsoft.Xna.Framework.Media;
using RedBulb.GUI;
using System.IO;
using RedBulb;
using RedBulb.MenuSystem;
using RedBulb.EasyMenus;
#endregion

/// RedBulb Engine GamePart
/// By Saeed Afshari
/// www.saeedoo.com
namespace RedBulbStarter
{
    public class GamePart1 : GamePart
    {
        public GamePart1(RedBulbGame Game)
            : base(Game)
        {
        }

        Form form;
        public override void Initialize()
        {
            base.Initialize();
            form = new Form(game);
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Behave(GameTime gameTime)
        {
            form.Update(gameTime);
            base.Behave(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            if (game.IsTapped(Keys.Escape, Buttons.Back)) ;
            base.HandleInput(gameTime);
        }


        public override void Render(GameTime gameTime)
        {
            base.Render(gameTime);
            form.Draw(gameTime);
        }
    }
}
