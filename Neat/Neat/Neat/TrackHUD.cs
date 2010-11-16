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
using Neat.Graphics;
 

namespace Neat
{
    public class TrackHUD
    {
        public NeatGame game;
        
        public float alpha = 0f;
        public float alphaSpeed = 0.01f;


        public bool fade = true;
        public Vector2 drawPosition;

        public string fontName = "Calibri";

        Vector2 trackNameOffset = new Vector2(20, 10);
        Vector2 trackArtistOffset = new Vector2(20, 40);
        Vector2 trackAlbumOffset = new Vector2(20, 60);
        Vector2 artOffset = new Vector2(4, 4);

        string trackName = "",
             trackArtist = "",
             trackAlbum = "";

         Song ActiveSong;
         //Vector2 offset = new Vector2(11, 11);

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

        public void Initialize()
        {
            Refresh();
        }

        public void Update()
        {
            if (trackName.Trim().Length == 0) alpha = -0.1f;
            if (fade)
            {
                alpha += alphaSpeed;
                if (alpha < 0f) fade = false;
                if (alpha > 2f) { alphaSpeed *= -1; }
                
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

        public void Draw()
        {
            if (alpha > 0)
            {
                game.SpriteBatch.Draw(game.getTexture("mediaHUD"),
                    drawPosition, GraphicsHelper.GetColorWithAlpha(Color.White, alpha));

                //text
                try
                {
                    GraphicsHelper.DrawShadowedString(game.SpriteBatch,
                        game.GetFont(fontName),
                        trackName,
                       drawPosition + trackNameOffset,
                       GraphicsHelper.GetColorWithAlpha(Color.White, alpha),
                       GraphicsHelper.GetColorWithAlpha(Color.Black, alpha));

                    GraphicsHelper.DrawShadowedString(game.SpriteBatch, game.GetFont(fontName),
                         trackAlbum,
                        drawPosition + trackAlbumOffset,
                        GraphicsHelper.GetColorWithAlpha(Color.White, alpha),
                        GraphicsHelper.GetColorWithAlpha(Color.Black, alpha));

                    GraphicsHelper.DrawShadowedString(game.SpriteBatch, game.GetFont(fontName),
                         trackArtist,
                        drawPosition + trackArtistOffset,
                        GraphicsHelper.GetColorWithAlpha(Color.White, alpha),
                        GraphicsHelper.GetColorWithAlpha(Color.Black, alpha));
                }
                catch
                { alpha = 0; }
            }
        }
    }
}
