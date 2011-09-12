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
using System.Diagnostics;
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

        public bool EnableCharacterByCharacterDrawing = true;

        public Dictionary<char, Color> ColorsTable = new Dictionary<char, Color>() {
        {'0', Color.Black},    
        {'1', Color.Blue},
        {'2', Color.Green},
        {'3', Color.Aqua},
        {'4', Color.DarkRed},
        {'5', Color.Purple},
        {'6', Color.Yellow},
        {'7', Color.WhiteSmoke},
        {'8', Color.Gray},
        {'9', Color.LightBlue},
        {'A', Color.LightGreen},
        {'B', Color.LightCyan},
        {'C', Color.Red},
        {'D', Color.LightPink},
        {'E', Color.LightYellow},
        {'F', Color.White}};

        public char ColorChangeSpecialCharacter = '@';

        public void Draw(int _hoffset, int _lines, bool showOnBottom)
        {
            Draw(game.SpriteBatch,_hoffset, _lines, showOnBottom);
        }

        public void Draw(SpriteBatch spriteBatch,int _hoffset, int _lines, bool showOnBottom)
        {
            fx_Update();
            if (/*standAlone && */!IsActive) return;
            if (yCurtain >= 0) yCurtain = 0;
            else yCurtain += CurtainSpeed;

            string messages = GetMessages(_lines, ref mOffset).Replace("\r\n", "\n"); 
            
            var font = standAlone ? StandAloneFont : game.GetFont(Font);
            Vector2 charSize = font.MeasureString("Z");

            int height = (int)(charSize.Y * (_lines + 1));// MeasureHeight(_lines);
            var width = standAlone ? spriteBatch.GraphicsDevice.DisplayMode.Width : game.GameWidth;

            //Draw Rectangle
            spriteBatch.Draw(
                standAlone ? StandAloneTexture : game.GetTexture(BackTexture),
                new Rectangle(0, _hoffset + (showOnBottom ? -yCurtain : yCurtain), 
                    width, height),
                BackColor);

            //Write Text
            spriteBatch.DrawString(
                font,
                "> " + command + ( ((int)(game.Frame / 20)) % 2 == 0 ? "_" : " "),
                new Vector2(0, _hoffset + (showOnBottom ? -yCurtain : yCurtain)),
                InputColor);
            try
            {
                //Limit text to _lines lines.
                int rowLength = (int)(width / charSize.X);
                int length = messages.Length;
                for (int i = 1; i < length / rowLength; i++) 
                    if (messages[i-1] == ColorChangeSpecialCharacter)
                        messages.Insert((i * rowLength)+1, "\n");
                    else
                        messages.Insert(i * rowLength, "\n");
                var texts = messages.Split('\n');
                Stack<string> reverseTexts = new Stack<string>();
                for (int i = texts.Length - 1;
                    i >= 0 && reverseTexts.Count < _lines; i--)
                    reverseTexts.Push(texts[i]);
                messages = "";
                while (reverseTexts.Count > 0) messages += reverseTexts.Pop() + "\n";

                if (EnableCharacterByCharacterDrawing)
                {
                    //Draw text with colors
                    int row = 0; int col = 0; Color c = TextColor;
                    Vector2 startPoint = new Vector2(0, _hoffset + (showOnBottom ? -yCurtain : yCurtain) + charSize.Y);

                    for (int i = 0; i < messages.Length; i++)
                    {
                        if (messages[i] == ColorChangeSpecialCharacter && messages.Length > i + 1 && ColorsTable.ContainsKey(messages[i + 1]))
                        {
                            //change color
                            c = ColorsTable[messages[i + 1]];
                            i++;
                        }
                        else if (messages[i] == '\n')
                        {
                            row++;
                            col = 0;
                        }
                        else
                        {
                            spriteBatch.DrawString(
                                font,
                                messages[i].ToString(),
                                new Vector2(col * charSize.X, row * charSize.Y) + startPoint,
                                c);
                            col++;
                        }
                        if (col > rowLength)
                        {
                            row++;
                            col = 0;
                        }
                    }
                }
                else
                {
                    //Draw text without colors
                    spriteBatch.DrawString(font,messages,
                        new Vector2(0, _hoffset + (showOnBottom ? -yCurtain : yCurtain) + charSize.Y),
                        TextColor);
                }
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
            Draw(showOnBottom ? game.GameHeight - MeasureHeight(LinesCount) : 0, LinesCount, showOnBottom);
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