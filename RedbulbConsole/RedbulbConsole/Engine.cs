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
using RedBulb;
using RedBulb.MenuSystem;

namespace RedbulbConsole
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class Game1 : RedBulbGame 
    {

        TrackHUD trackHUD = new TrackHUD();

        protected override void Initialize()
        {
            showMouse = false;
            fullscreen = false;
            gameWidth = 800;
            gameHeight = 600;
            trackHUD.game = this;
            base.Initialize();

            console.Commands.Add("greet", e_greet);
        }

        void e_greet(IList<string> args)
        {
            console.WriteLine("Hello " + args[1]);
        }

        public override void CreateParts()
        {
            parts = new Dictionary<string, GamePart>();
            parts.Add("mainmenu", new MediaplayerScreen(this));
        }    

        protected override void Behave(GameTime gameTime)
        {
            base.Behave(gameTime);

            if (IsReleased(Keys.F11)) ToggleFullScreen();
            trackHUD.Update();
        }

        void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            try
            {
                console.WriteLine("Song Changed to " + MediaPlayer.Queue.ActiveSong.Name + " | Album: " + MediaPlayer.Queue.ActiveSong.Album);
                trackHUD.drawPosition = new Vector2(0, (Window.ClientBounds.Height) - 100);

                trackHUD.Refresh();
                trackHUD.FadeIn();
            }
            catch
            {
                SayMessage("Error changing the song");
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            MediaPlayer.ActiveSongChanged += new EventHandler(MediaPlayer_ActiveSongChanged);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            InitializeGraphics();
        }

        protected override void Render(GameTime gameTime)
        {
            base.Render(gameTime);
            trackHUD.Draw();
        }
    }
}
