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
using RedBulb;
using RedBulb.MenuSystem;
//using UltimaScroll.IBLib;
#endregion

namespace RedBulb
{
#if XLIVE
    public class NetworkHUD
    {
        public NetworkHelper networkHelper;
        public RedBulbGame game;
        Texture2D displayPic;
        public float alpha = 0f;
        public float alphaSpeed = 0.04f;


        public bool fade = true;
        public Vector2 drawPosition;
        
         Vector2 mainTextOffset = new Vector2(20, 20);
         Vector2 gamerTagOffset = new Vector2(280, 13);
         //Vector2 offset = new Vector2(11, 11);
        public void Initialize()
        {
            try
            {
                titleText = SignedInGamer.SignedInGamers[0].Gamertag;
                mainText = "Player " + SignedInGamer.SignedInGamers[0].PlayerIndex.ToString();
            }
            catch
            {
            }
            try
            {
                displayPic = SignedInGamer.SignedInGamers[0].GetProfile().GamerPicture;
                isFetchedDisplayPicture = true;
            }
            catch
            {
                displayPic = game.getTexture("solid");
                
                game.sayMessage("NetworkHUD.Initialize(): Error in fetching GamerPicture");
            }
        }
        bool isFetchedDisplayPicture = false;

        public void SetPicture(Texture2D t)
        {
            isFetchedDisplayPicture = true;
            displayPic = t;
        }
        public void Update()
        {
            if (!isFetchedDisplayPicture)
            {
                try
                {
                    displayPic = SignedInGamer.SignedInGamers[0].GetProfile().GamerPicture;
                    isFetchedDisplayPicture = true;
                }
                catch
                {
                    displayPic = game.getTexture("solid");
                    game.sayMessage("NetworkHUD.Initialize(): Error in fetching GamerPicture");
                }
            }
            if (fade)
            {
                alpha += alphaSpeed;
                if (alpha < 0f || alpha > 1f) fade = false;
                
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
            alphaSpeed = 1f;
            alphaSpeed = -Math.Abs(alphaSpeed);
            fade = true;
        }

        public void Draw()
        {
            if (alpha > 0)
            {
                game.spriteBatch.Draw(game.getTexture("gamerCardHUD"),
                    drawPosition, game.getColorWithAlpha(Color.White, alpha));

                //text
                game.DrawShadowedString(game.GetFont("Calibri"),
                    (titleText!=""? titleText:SignedInGamer.SignedInGamers[0].Gamertag) ,
                   drawPosition + mainTextOffset,
                   game.getColorWithAlpha(Color.MidnightBlue, alpha),
                   game.getColorWithAlpha(Color.WhiteSmoke, alpha));

                game.DrawShadowedString(game.GetFont("Calibri"),
                    (mainText!=""?mainText: SignedInGamer.SignedInGamers[0].PlayerIndex.ToString()),
                    drawPosition + mainTextOffset + new Vector2(2, 20),
                    game.getColorWithAlpha(Color.Black, alpha),
                    game.getColorWithAlpha(Color.WhiteSmoke, alpha));

                //pic 
                Vector2 v = drawPosition + gamerTagOffset;
                game.spriteBatch.Draw(
                    displayPic,
                    new Rectangle(
                        (int)(v.X), (int)(v.Y), 62, 62),
                         game.getColorWithAlpha(Color.White, alpha));
            }
        }

        public string titleText = "";
        public string mainText = "";
    }
#endif
}
