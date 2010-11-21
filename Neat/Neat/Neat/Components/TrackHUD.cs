using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
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
using Neat.Graphics;
 

namespace Neat.Components
{
    public class TrackHUD : DrawableGameComponent
    {
        NeatGame game;
        
        float alpha = 0f;
        float alphaSpeed = 0.01f;

        bool fade = true;
        public Vector2 DrawPosition;

        public string FontName = "Calibri";

        Vector2 trackNameOffset = new Vector2(20, 10);
        Vector2 trackArtistOffset = new Vector2(20, 40);
        Vector2 trackAlbumOffset = new Vector2(20, 60);
        Vector2 artOffset = new Vector2(4, 4);

        string trackName = "",
             trackArtist = "",
             trackAlbum = "";

        Song ActiveSong;

        public TrackHUD(NeatGame _game)
            : base(_game)
        {
            game = _game;
        }

        public void Refresh()
        {
            try
            {
                ActiveSong = MediaPlayer.Queue.ActiveSong;
                trackName = ActiveSong.Name;
                trackArtist = ActiveSong.Artist.Name;
                trackAlbum = ActiveSong.Album.Name;
            }
            catch
            {
                alpha = 0f;
            }
        }

        public override void Initialize()
        {
            Refresh();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (trackName.Trim().Length == 0) alpha = -0.1f;
            if (fade)
            {
                alpha += alphaSpeed;
                if (alpha < 0f) fade = false;
                if (alpha > 2f) { alphaSpeed *= -1; }

            }
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            MediaPlayer.ActiveSongChanged += new EventHandler<EventArgs>(MediaPlayer_ActiveSongChanged);
            base.LoadContent();
        }

        void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            try
            {
                game.Console.WriteLine("Song Changed to " + MediaPlayer.Queue.ActiveSong.Name + " | Album: " + MediaPlayer.Queue.ActiveSong.Album);
                DrawPosition = new Vector2(0, (game.Window.ClientBounds.Height) - 100);

                Refresh();
                FadeIn();
            }
            catch
            {
                game.SayMessage("Error changing the song");
            }
        }

        public void FadeIn()
        {
            alpha = 0f;
            alphaSpeed = Math.Abs(alphaSpeed);
            fade = true;
        }

        public void FadeOut()
        {
            alphaSpeed = 2f;
            alphaSpeed = -Math.Abs(alphaSpeed);
            fade = true;
        }

        public override void Draw(GameTime gameTime)
        {
            if (alpha > 0)
            {
                game.SpriteBatch.Begin();
                game.SpriteBatch.Draw(game.GetTexture("mediaHUD"),
                    DrawPosition, GraphicsHelper.GetColorWithAlpha(Color.White, alpha));

                //text
                try
                {
                    GraphicsHelper.DrawShadowedString(game.SpriteBatch,
                        game.GetFont(FontName),
                        trackName,
                       DrawPosition + trackNameOffset,
                       GraphicsHelper.GetColorWithAlpha(Color.White, alpha),
                       GraphicsHelper.GetColorWithAlpha(Color.Black, alpha));

                    GraphicsHelper.DrawShadowedString(game.SpriteBatch, game.GetFont(FontName),
                         trackAlbum,
                        DrawPosition + trackAlbumOffset,
                        GraphicsHelper.GetColorWithAlpha(Color.White, alpha),
                        GraphicsHelper.GetColorWithAlpha(Color.Black, alpha));

                    GraphicsHelper.DrawShadowedString(game.SpriteBatch, game.GetFont(FontName),
                         trackArtist,
                        DrawPosition + trackArtistOffset,
                        GraphicsHelper.GetColorWithAlpha(Color.White, alpha),
                        GraphicsHelper.GetColorWithAlpha(Color.Black, alpha));
                    
                }
                catch
                { alpha = 0; }
                game.SpriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
