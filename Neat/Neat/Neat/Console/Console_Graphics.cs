using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
using Neat.Graphics;
namespace Neat.Components
{
    public partial class Console : GameComponent
    {
        public int LinesCount = 10;
        int yCurtain = 0;
        public int CurtainSpeed = 10;
        int mOffset = 0;

        public Texture2D StandAloneTexture;
        public SpriteFont StandAloneFont;

        public void Draw(int _hoffset, int _lines, bool showOnBottom)
        {
            if (standAlone) return;
            else Draw( game.SpriteBatch,_hoffset, _lines, showOnBottom);
        }

        public void Draw(SpriteBatch spriteBatch,int _hoffset, int _lines, bool showOnBottom)
        {
            if (standAlone && !IsActive) return;
            fx_Update();

            if (yCurtain >= 0) yCurtain = 0;
            else yCurtain += CurtainSpeed;

            string messages = GetMessages(_lines, ref mOffset);

            int height = MeasureHeight(_lines);
            var width = standAlone ? spriteBatch.GraphicsDevice.DisplayMode.Width : game.GameWidth;
            //Draw Rectangle
            spriteBatch.Draw(
                standAlone ? StandAloneTexture : game.GetTexture(BackTexture),
                new Rectangle(0, _hoffset + (showOnBottom ? -yCurtain : yCurtain), 
                    width, height),
                BackColor);

            var font = standAlone ? StandAloneFont : game.GetFont(Font);
            //Write Text
            spriteBatch.DrawString(
                font,
                "> " + command + ( ((int)(game.Frame / 20)) % 2 == 0 ? "_" : " "),
                new Vector2(0, _hoffset + (showOnBottom ? -yCurtain : yCurtain)),
                InputColor);
            try
            {
                spriteBatch.DrawString(
                    font,
                    messages,
                    new Vector2(0, _hoffset + (showOnBottom ? -yCurtain : yCurtain) + font.MeasureString("Z").Y),
                    TextColor);
            }
            catch (Exception e)
            {
                Clear();
                WriteLine("Error: " + e.Message);
                WriteLine("Console cleared to fix the problem.");
            }
            if (lb == null)
            {
                lb = new LineBrush(spriteBatch.GraphicsDevice, 1);
            }
            else
            {
                lb.Draw(spriteBatch, new Vector2(0, _hoffset - 1 + (showOnBottom ? -yCurtain : yCurtain)), new Vector2(width, _hoffset - 1 + (showOnBottom ? -yCurtain : yCurtain)), InputColor);
                lb.Draw(spriteBatch, new Vector2(0, _hoffset + 1 + height + (showOnBottom ? -yCurtain : yCurtain)), new Vector2(width, _hoffset + height + (showOnBottom ? -yCurtain : yCurtain)), InputColor);
            }
        }

        public void Draw(bool showOnBottom)
        {
            Draw(showOnBottom ? game.GameHeight - MeasureHeight(LinesCount) : 0, LinesCount,showOnBottom);
        }

        public void Draw(SpriteBatch spriteBatch, bool showOnBottom)
        {
            Draw( spriteBatch,showOnBottom ? 
                (standAlone ? spriteBatch.GraphicsDevice.DisplayMode.Height : game.GameHeight)
                - MeasureHeight(LinesCount) : 0, LinesCount, showOnBottom);
        }
        #region Special Effects

        List<string> messages = new List<string>();
        string fx_text = "Get Ready!";
        bool fx_on, fx_end;
        float fx_scale, fx_scale_end, fx_scale_speed,
          fx_alpha, fx_alpha_speed, fx_alpha_end,
          fx_alpha2, fx_alpha2_speed, fx_alpha2_end;
        Vector2 fx_offset, fx_offset_speed;
        Color fx_Color = Color.White;


        public void ShoutText(string message)
        {
            messages.Add(message);
        }

        public void UpdateMessages()
        {
            if (messages.Count > 0)
            {
                if (!fx_on && !fx_end)
                {
                    fx_text = messages[0];
                    fx_Start();
                    messages.RemoveAt(0);
                }
            }
        }

        void fx_Start()
        {
            fx_on = true;
            fx_scale = 1.5f;
            fx_scale_end = 0.6f;
            fx_scale_speed = -0.015f;
            fx_alpha = 0f;
            fx_alpha_speed = 0.02f;
            fx_alpha_end = 1f;
            fx_end = false;
            fx_alpha2 = 1f;
            fx_alpha2_speed = -0.125f;
            fx_alpha2_end = 0f;
            fx_offset = new Vector2(0, 0 + game.GameHeight / 2 - 100);
            fx_offset_speed = new Vector2(0, 0.4f);
        }

        void fx_Update()
        {
            if (fx_on || fx_end)
            {
                Vector2 size = game.GetFont("fxFont").MeasureString(fx_text) * fx_scale;
                Vector2 position = new Vector2(
                    game.GameWidth / 2 - size.X / 2 + 0,
                    fx_offset.Y);
                fx_offset += fx_offset_speed;

                if (fx_on)
                {
                    game.SpriteBatch.DrawString(
                        game.GetFont("fxFont"),
                        fx_text,
                        position,
                        GraphicsHelper.GetColorWithAlpha(fx_Color, fx_alpha), 0,
                        Vector2.Zero, // size / 2,
                        fx_scale,
                        SpriteEffects.None, 0);
                    if (fx_scale > fx_scale_end) fx_scale += fx_scale_speed;
                    else { fx_on = false; fx_end = true; }
                    if (fx_alpha < fx_alpha_end) fx_alpha += fx_alpha_speed;
                }
                else
                {
                    game.SpriteBatch.DrawString(
                        game.GetFont("fxFont"),
                        fx_text,
                        position,
                        GraphicsHelper.GetColorWithAlpha(fx_Color, fx_alpha2), 0,
                        Vector2.Zero,
                        fx_scale,
                        SpriteEffects.None, 0);
                    if (fx_alpha2 > fx_alpha2_end) fx_alpha2 += fx_alpha2_speed;
                    else fx_end = false;
                }
            }
        }
        #endregion
    }
}