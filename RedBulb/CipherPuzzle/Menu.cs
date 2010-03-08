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
/// RedBulb Engine Sample Game
/// By Saeed Afshari
/// www.saeedoo.com
/// 
namespace CipherPuzzle
{
    public class MainMenu:GamePart 
    {
        public MainMenu(RedBulbGame Game)
            : base(Game)
        {
        }

        Form form;
        Rectangle bounds = Rectangle.Empty;
        public override void Initialize()
        {
            game.SayMessage("MainMenu Initialized.");
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
            form.GetObject("statusbar").caption = "Welcome to Cipher Puzzle!";

            form.NewObject("btnStart", new MainMenuButton(form.GetObject("statusbar")));
            form.GetObject("btnStart").position = offset + b1;
            form.GetObject("btnStart").OnRelease+=new XEventHandler(_newGame);
            form.GetObject("btnStart").caption = "New Game";
            ((MainMenuButton)(form.GetObject("btnStart"))).statusBarText = "New Game : Begins a new puzzle";

            form.NewObject("btnOptions", new MainMenuButton(form.GetObject("statusbar")));
            form.GetObject("btnOptions").position = offset + b1+b2*2;
            form.GetObject("btnOptions").OnRelease+=new XEventHandler(_options);
            form.GetObject("btnOptions").caption = "Options";
            ((MainMenuButton)(form.GetObject("btnOptions"))).statusBarText = "Options : Game Tunings";

            form.NewObject("btnCredits", new MainMenuButton(form.GetObject("statusbar")));
            form.GetObject("btnCredits").position = offset + b1 + b2 * 2;
            form.GetObject("btnCredits").caption = "About";
            form.GetObject("btnCredits").visible = false;
            ((MainMenuButton)(form.GetObject("btnCredits"))).statusBarText = "About : Game Credits";

            form.NewObject("btnQuit", new MainMenuButton(form.GetObject("statusbar")));
            form.GetObject("btnQuit").position = offset + b1+b2*3;
            form.GetObject("btnQuit").OnRelease+=new XEventHandler(_quit);
            form.GetObject("btnQuit").caption = "Quit";
            ((MainMenuButton)(form.GetObject("btnQuit"))).statusBarText = "Quit : Exits the Game";
           
        }
        void _newGame()
        {
            game.ActivatePart("game");
        }
        void _options()
        {
            game.ActivatePart("options");
        }
        
        void _quit()
        {
            game.Exit();
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
            form.GetObject("btnStart").position =  b1;
            form.GetObject("btnOptions").position = b1 + b2 * 1;
            form.GetObject("btnCredits").position =  b1 + b2 * 2;
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

    public class MainMenuButton : Button
    {
        public MainMenuButton(FormObject l)
        {
            backgroundImage = "button_2";
            caption = "";
            size = new Vector2(380, 60);
            mouseHoverColor = Color.Yellow;
            font = "menufont";
            foreColor = Color.LightBlue;
            statusBar = (TypeWriter)l;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!isH && isMouseHovered)
            {
                statusBar.caption = statusBarText;
                statusBar.Reset();
            }
            isH = isMouseHovered;
        }

        public string statusBarText = "";
        public TypeWriter statusBar;
        bool isH = false;
    }
}
