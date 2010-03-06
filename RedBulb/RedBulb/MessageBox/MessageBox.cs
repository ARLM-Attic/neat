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
    
    public enum MessageBoxType { TextBox, YesNo, Ok };
    public class MessageBox
    {
        public event XEventHandler OnShow;
        public event XEventHandler OnHide;
        public event XEventHandler OnSubmit;
        public event XEventHandler InteractiveChange;
     
        public RedBulbGame game;
        public MessageBoxType type;

        public float alpha = 0f;
        public float alphaSpeed = 0.2f;

        public bool fade = true;
        public Vector2 drawPosition;
        public Vector2 titlePosition;
        public Rectangle bounds;
        public string backgroundTexture="MessageBoxWindow";
        public string buttonTexture = "MessageBoxWindow_Button";
        public string buttonFocusedTexture = "MessageBoxWindow_ButtonFocused";
        public Vector2 yesButtonPos = new Vector2(110, 60);
        public Vector2 noButtonPos = new Vector2(260, 60);
        public Color titleColor;
        public Color textColor;
        public string titleFont = "messageBoxTitleFont";
        public string textFont = "messageBoxTextFont";
        public Vector2 titlePos = new Vector2(96, 0);
        public Vector2 textPos = new Vector2(34, 57);
        public bool boolResult = false;
        public string textResult = "";

        string _title = "Message";
        public string title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                try
                {
                    titlePos.X = (float)game.getTexture(backgroundTexture).Width / 2 - game.GetFont(titleFont).MeasureString(title).X / 2;
                }
                catch { }
            }
        }

        public MessageBox(RedBulbGame _game, MessageBoxType _type, string _title)
        {
            type = _type;
            title = _title;
            game = _game;
            titleColor = new Color(1f, (float)153 / 255, 0f);
            textColor = Color.White;
        }

        public void Initialize()
        {
            game.SayMessage("MessageBox Initialized.");
            if (type == MessageBoxType.TextBox) backgroundTexture = "MessageBoxWindow_TextBox";
            else if (type == MessageBoxType.YesNo) backgroundTexture = "MessageBoxWindow_YesNo";
            Center();
            titlePos.X = (float)game.getTexture(backgroundTexture).Width / 2 - game.GetFont(titleFont).MeasureString(title).X / 2;
        }

        public void Center()
        {
            Texture2D bg = game.getTexture(backgroundTexture);
            drawPosition.X = (int)(game.gameWidth / 2 - bg.Width / 2);
            drawPosition.Y = (int)(game.gameHeight / 2 - bg.Height / 2);
        }

        public void Update()
        {
            if (fade)
            {   
                alpha += alphaSpeed;
                if (alpha < 0f) fade = false;
                if (alpha > 1f) { fade = false; if (OnShow != null) OnShow();}
            }
            else if (alpha > 0)
            {
                if (type == MessageBoxType.TextBox) UpdateTextBox();
                else if (type == MessageBoxType.YesNo) UpdateYesNo();
            }
        }

        public void UpdateTextBox()
        {
            string lastTextResult = textResult;
            bool shift = (game.IsPressed(Keys.LeftShift) || game.IsPressed(Keys.RightShift));
            if (textResult.Length < 30)
            {
                for (int i = 0; i < 26; i++)
                {
                    if (game.IsTapped(Keys.A + i))
                        if (shift) textResult += (char)('A' + i);
                        else       textResult += (char)('a' + i);
                    
                    if (i <= 9 && (game.IsTapped(Keys.D0 + i) || game.IsTapped(Keys.NumPad0 + i)))
                        textResult += (char)('0' + i);
                }
                if (game.IsTapped(Keys.Space)) textResult += ' ';
            }

            if (game.IsTapped(Keys.Back, Buttons.Back)) textResult =
                (textResult.Length != 0 ? textResult.Substring(0, textResult.Length - 1) : "");

            if (textResult != lastTextResult && InteractiveChange != null) InteractiveChange();

            else if (game.IsTapped(Keys.Enter, Buttons.A)) Submit(true);
            else if (game.IsTapped(Keys.Escape, Buttons.Back)) Submit(false);
        }
        public void UpdateYesNo()
        {
            bool lastBoolResult = boolResult;
            if (game.IsTapped(Keys.Right) || game.IsTapped(Keys.Left)) boolResult = !boolResult;
            if (lastBoolResult != boolResult && InteractiveChange != null) InteractiveChange();
            if (game.IsTapped(Keys.Enter, Buttons.A)) Submit(boolResult);
        }

        public void Submit(bool state)
        {
            boolResult = state;
            if (OnSubmit != null) OnSubmit();
            alphaSpeed *= -1;
            fade = true;
            if (OnHide != null) OnHide();
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
                game.spriteBatch.Draw(game.getTexture(backgroundTexture),
                    drawPosition, game.GetColorWithAlpha(Color.White, alpha));
                
                game.spriteBatch.DrawString(
                game.GetFont(titleFont),
                title ,
                drawPosition + titlePos,
                game.GetColorWithAlpha(titleColor,alpha));
                if (type == MessageBoxType.TextBox) DrawTextBox();
                else if (type == MessageBoxType.YesNo) DrawYesNo();
            }
        }

        public void DrawTextBox()
        {
            game.spriteBatch.DrawString(
                game.GetFont(textFont),
                (textResult.Length<30 ? textResult+"_" : textResult),
                drawPosition+textPos,
                game.GetColorWithAlpha(textColor,alpha));
        }
        public void DrawYesNo()
        {
            Texture2D t = game.getTexture( buttonTexture );
            game.spriteBatch.Draw(
                t,
                yesButtonPos + drawPosition,
                game.GetColorWithAlpha(Color.White, alpha));
            game.spriteBatch.Draw(
                t,
                noButtonPos + drawPosition,
                game.GetColorWithAlpha(Color.White, alpha));
            game.spriteBatch.Draw(
                game.getTexture(buttonFocusedTexture),
                (boolResult ? yesButtonPos : noButtonPos) + drawPosition,
                game.GetColorWithAlpha(Color.White,alpha));
            SpriteFont font = game.GetFont(textFont);
            
            game.spriteBatch.DrawString(
                font, "Yes", 
                drawPosition+ yesButtonPos+ new Vector2(t.Width / 2,t.Height/2+2) - font.MeasureString("Yes") / 2,
                game.GetColorWithAlpha( textColor, alpha) );
            game.spriteBatch.DrawString(
                font, "No",
                drawPosition + noButtonPos + new Vector2(t.Width / 2, t.Height / 2 + 2) - font.MeasureString("No") / 2,
                game.GetColorWithAlpha( textColor, alpha) );
        }
    }
}
