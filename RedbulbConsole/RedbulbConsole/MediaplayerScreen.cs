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
namespace RedbulbConsole
{
    public class MediaplayerScreen : GamePart
    {
        MenuSystem system;
        public SpriteFont font;
        int width = 100;
        Texture2D line;

        public MediaplayerScreen(RedBulbGame Game)
            : base(Game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            
            font = game.GetFont("menufont");
            system = new MenuSystem(
                game,
                new Vector2(game.gameWidth / 2, 10),
                font);

            system.AddItem("Play Library");
            system.AddItem("Next Track");
            system.AddItem("Previous Track");
            system.AddItem("Stop");
            system.AddItem("Toggle Shuffle");
            system.AddItem("Toggle Repeat");
            system.itemsOffset = new Vector2(0, 100);

            line = game.getTexture("solid");
        }

        public override void Activate()
        {
            game.backGroundColor = Color.Black;
            base.Activate();
            
            CalculateCoords();
            system.Enable();
        }

        void CalculateCoords()
        {
            system.position = new Vector2(game.gameWidth / 2, 100);
        }

        VisualizationData vd = new VisualizationData();
        public override void Behave(GameTime gameTime)
        {
            system.Update(gameTime);
            CalculateCoords();
            MediaPlayer.GetVisualizationData(vd);
            
            base.Behave(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            if (game.IsTapped(Keys.Escape, Buttons.Back)) ;

#if ZUNE
            if (game.IsTapped( Buttons.A))
#elif WINDOWS
            if (game.IsTapped(Keys.Enter))
#endif
            {
                switch (system.selectedItem)
                {
                    case 0: //play
                        game.console.Run("a_mediaplay library");
                        break;
                    case 1: //next
                        game.console.Run("a_medianext");
                        break;
                    case 2: //prev
                        game.console.Run("a_mediaprev");
                        break;
                    case 3: //stop
                        game.console.Run("a_mediastop");
                        break;
                    case 4: //shuf
                        MediaPlayer.IsShuffled = !MediaPlayer.IsShuffled;
                        break;
                    case 5: //repe
                        MediaPlayer.IsRepeating = !MediaPlayer.IsRepeating;
                        break;

                }
            }
            base.HandleInput(gameTime);
        }

        
        public override void Render(GameTime gameTime)
        {
            base.Render(gameTime);
            system.Draw(gameTime);
            game.DrawShadowedString(game.GetFont("cambria"), "REPEAT: " + (MediaPlayer.IsRepeating ? "ON" : "OFF"),
                new Vector2(game.gameWidth - 300, (game.gameHeight - 70)),Color.White);
            game.DrawShadowedString(game.GetFont("cambria"), "SHUFFLE: " + (MediaPlayer.IsShuffled ? "ON" : "OFF"),
                new Vector2(game.gameWidth - 300, (game.gameHeight - 140)),Color.White);

            for (int y = 0; y < 255; y++)
            {
                // Draw frequency spectrum display.
                
                game.spriteBatch.Draw(
                    line,
                    new Rectangle(100+(int)((-1.0 + vd.Frequencies[y]) * width + 1), y+200,2,2),null,
                    Color.White,MathHelper.PiOver2,Vector2.Zero,SpriteEffects.None,0);
                
            }
        }
    }
}
