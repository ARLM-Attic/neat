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

using RedBulb;
using RedBulb.MenuSystem;
using RedBulb.EasyMenus;
#endregion
using RedBulb.GUI;
using System.IO;
/// RedBulb Engine Sample Game
/// By Saeed Afshari
/// www.saeedoo.com
/// 
namespace CipherPuzzle
{
    public class Options:GamePart 
    {
        public Options(RedBulbGame Game)
            : base(Game)
        {
        }

        Form form;
        Rectangle bounds = Rectangle.Empty;
        public override void Initialize()
        {
            game.SayMessage("Options Initialized.");
            base.Initialize();

            bounds.Width = 1280;
            bounds.Height = 800;
            
            bounds.Y = (game.gameWidth / 2 - game.gameHeight / 2);
            Vector2 offset = new Vector2((float)bounds.X, (float)bounds.Y);
            form = new Form(game);
            
            Vector2 b1 = new Vector2(790, 120);
            Vector2 b2 = new Vector2(0, 90);

            form.NewObject("statusbar", new TypeWriter());
            form.GetObject("statusbar").position = new Vector2(5, 759) + offset;
            form.GetObject("statusbar").caption = "Options";

            form.NewObject("btnAmateur", new MainMenuButton(form.GetObject("statusbar")));
            form.GetObject("btnAmateur").position = offset + b1;
            form.GetObject("btnAmateur").OnRelease += new XEventHandler(_amateur);
            form.GetObject("btnAmateur").caption = "Amateur";
            ((MainMenuButton)(form.GetObject("btnAmateur"))).statusBarText = "Amateur : Solve the puzzle with lots of hints!";

            form.NewObject("btnMedium", new MainMenuButton(form.GetObject("statusbar")));
            form.GetObject("btnMedium").position = offset + b1 + b2 * 2;
            form.GetObject("btnMedium").OnRelease += new XEventHandler(_medium);
            form.GetObject("btnMedium").caption = "Medium";
            ((MainMenuButton)(form.GetObject("btnMedium"))).statusBarText = "Medium : Solve the puzzle on your own, you can use hints whenever you want";

            form.NewObject("btnPro", new MainMenuButton(form.GetObject("statusbar")));
            form.GetObject("btnPro").position = offset + b1 + b2 * 2;
            form.GetObject("btnPro").OnRelease += new XEventHandler(_pro);
            form.GetObject("btnPro").caption = "Pro";
            ((MainMenuButton)(form.GetObject("btnPro"))).statusBarText = "Pro : Solve the puzzle on your own without any hints";

            form.NewObject("btnQuit", new MainMenuButton(form.GetObject("statusbar")));
            form.GetObject("btnQuit").position = offset + b1+b2*3;
            form.GetObject("btnQuit").OnRelease+=new XEventHandler(_back);
            form.GetObject("btnQuit").caption = "Back";
            ((MainMenuButton)(form.GetObject("btnQuit"))).statusBarText = "Back : Return to main menu";
           
        }
        void _amateur()
        {
            StreamWriter s = new StreamWriter("s.cfg", false, System.Text.Encoding.ASCII);
            s.WriteLine("a");
            s.Close();
            _back();
        }
        void _medium()
        {
            StreamWriter s = new StreamWriter("s.cfg", false, System.Text.Encoding.ASCII);
            s.WriteLine("m");
            s.Close();
            _back();
        }
        void _pro()
        {
            StreamWriter s = new StreamWriter("s.cfg", false, System.Text.Encoding.ASCII);
            s.WriteLine("p");
            s.Close();
            _back();
        }
        void _back()
        {
            game.ActivatePart("mainmenu");
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Behave(GameTime gameTime)
        {
            Vector2 b1 = new Vector2(game.gameWidth - 450, 100);
            Vector2 b2 = new Vector2(0, 90);
            if (game.gameWidth < 1280) b1.X = game.gameWidth - 400;
            form.GetObject("statusbar").position =  new Vector2(5, game.gameHeight - 36) ;
            form.GetObject("btnAmateur").position = b1;
            form.GetObject("btnMedium").position = b1 + b2 * 1;
            form.GetObject("btnPro").position = b1 + b2 * 2;
            form.GetObject("btnQuit").position =  b1 + b2 * 3;
            base.Behave(gameTime);
            form.Update(gameTime);
        }
       
        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);
        }


        public override void Render(GameTime gameTime)
        {
            base.Render(gameTime);
            Texture2D t = game.getTexture("mainmenu");
            if (game.gameWidth < 1280) t = game.getTexture("mainmenu43");
            else if (game.gameHeight == 800) t = game.getTexture("mainmenuw");
            game.spriteBatch.Draw(t, 
                new Vector2(game.gameWidth / 2 - t.Width / 2, game.gameHeight / 2 - t.Height / 2),
                Color.White);
            form.Draw(gameTime) ;
        }
    }
}
