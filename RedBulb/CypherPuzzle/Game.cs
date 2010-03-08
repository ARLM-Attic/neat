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
namespace CipherPuzzle
{
    public class GamePart1:GamePart 
    {
        public enum ShowTypes
        {
            cipherOnly,
            KeyOnly,
            Mixed
        }
        public enum GameSkills
        {
            Amateur,
            Normal,
            Pro
        }
        public GamePart1(RedBulbGame Game)
            : base(Game)
        {
            Initialize();
        }

        GameSkills skill;
        Form form;
        Cipher c;
        string s;
        Dictionary<char, char> key = new Dictionary<char, char>();
        ShowTypes st;
        bool showinfo = false;
        char selectedChar = ' ';
        TimeSpan t;
        string solvedText
        {
            get
            {
                string result = "";
                for (int i = 0; i < c.length; i++)
                {
                    if (key.ContainsKey(c.cipher[i]))
                        result += key[c.cipher[i]];
                    else result += c.cipher[i];
                }
                return result;
            }
        }
        public override void Initialize()
        {
            base.Initialize();
            form = new Form(game);
            Activate();
            form.NewObject("label1", new Label());
            form.GetObject("label1").caption = c.cipher;
            form.GetObject("label1").ToLabel().SetColor(Color.White);
            form.GetObject("label1").position = Vector2.Zero;
            form.GetObject("label1").visible = false;

            float btnsY = 10f ;
            form.NewObject("btncipherOnly", new Button());
            form.GetObject("btncipherOnly").caption = "Cipher (F1)";
            form.GetObject("btncipherOnly").position = new Vector2(game.gameWidth/2 - 400, btnsY);
            form.GetObject("btncipherOnly").OnRelease += new XEventHandler(btncipherOnly);
            
            form.NewObject("btnMixedView", new Button());
            form.GetObject("btnMixedView").caption = "Mixed (F2)";
            form.GetObject("btnMixedView").position = new Vector2(game.gameWidth / 2 - 200, btnsY);
            form.GetObject("btnMixedView").OnRelease += new XEventHandler(btnMixedView);
            
            form.NewObject("btnKeyOnly", new Button());
            form.GetObject("btnKeyOnly").caption = "Key (F3)";
            form.GetObject("btnKeyOnly").position = new Vector2(game.gameWidth / 2, btnsY);
            form.GetObject("btnKeyOnly").OnRelease += new XEventHandler(btnKeyOnly);

            form.NewObject("btnHint", new Button());
            form.GetObject("btnHint").caption = "Hint (F4)";
            form.GetObject("btnHint").enabled = skill != GameSkills.Pro;
            form.GetObject("btnHint").position = new Vector2(game.gameWidth / 2 +200, btnsY);
            form.GetObject("btnHint").OnRelease += new XEventHandler(btnHint);
            
        }

        void btnHint()
        {
            if (selectedChar != ' ' && !c.IsLetterCorrect(key[selectedChar], selectedChar))
            {
                key[selectedChar] = c.GetHint(selectedChar);
                selectedChar = ' ';
            }
            else
            {
                for (int i = 0; i < c.length; i++)
                {
                    if (key.ContainsKey(c.cipher[i]) && !c.IsLetterCorrect(key[c.cipher[i]], c.cipher[i]))
                    {
                        key[c.cipher[i]] = c.GetHint(c.cipher[i]);
                        break;
                    }
                }
            }
            t = t.Subtract(new TimeSpan(0, 0, 30));
        }

        void btnKeyOnly()
        {
            st = ShowTypes.KeyOnly;
        }

        void btnMixedView()
        {
            st = ShowTypes.Mixed;
        }

        void btncipherOnly()
        {
            st = ShowTypes.cipherOnly;
        }

        public override void Activate()
        {
            base.Activate();
            c = new Cipher(game.randomGenerator);
            StreamReader r = new StreamReader("l.cfg", System.Text.Encoding.ASCII);
            List<string> texts = new List<string>();
            while (!r.EndOfStream)
            {
                texts.Add(r.ReadLine().Trim().ToLower());
            }
            r.Close();
            c.Generatecipher(texts[game.randomGenerator.Next(texts.Count)]);
            key = new Dictionary<char, char>();
            for (char i = 'a'; i <= 'z'; i++) key.Add(i, '_');
            skill = GameSkills.Amateur;
            r = new StreamReader("s.cfg", System.Text.Encoding.ASCII);
            string q = r.ReadLine().Trim();
            if (q == "a") skill = GameSkills.Amateur;
            else if (q == "m") skill = GameSkills.Normal;
            else skill = GameSkills.Pro;
            r.Close();
            st = ShowTypes.Mixed;
            s = "";
            foreach (var item in c.key)
            {
                s += item.Key + "=" + item.Value + '\n';
            }
            try
            {
                form.GetObject("btnHint").enabled = skill != GameSkills.Pro;
                form.GetObject("btnHint").visible = skill != GameSkills.Pro;
            }
            catch
            { }
        }
        bool a = false;
        bool cw = false;
        public override void Behave(GameTime gameTime)
        {
            if (a == false) { a = true; t = gameTime.TotalRealTime; }
            
            cw = CheckWin();
            if (cw)
            {
                
                form.GetObject("label1").caption = "WON";
            }
            else
            {
                form.GetObject("btncipherOnly").foreColor = st == ShowTypes.cipherOnly ? Color.Tomato : Color.Yellow;
                form.GetObject("btnMixedView").foreColor = st == ShowTypes.Mixed ? Color.Tomato : Color.Yellow;
                form.GetObject("btnKeyOnly").foreColor = st == ShowTypes.KeyOnly ? Color.Tomato : Color.Yellow;
                if (!showinfo) form.Update(gameTime);
            }

            base.Behave(gameTime);
        }
       
        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);
            if (game.IsTapped(Keys.Escape)) game.ActivatePart("mainmenu");
            if (!cw)
            {
                if (game.IsMouseClicked()) SelectLetter();

                for (Keys k = Keys.A; k <= Keys.Z; k++)
                {
                    if (selectedChar != ' ')
                    {
                        if (game.IsTapped(k)) AssignLetter((char)((int)(k - Keys.A) + (int)'a'));
                    }
                    else if (game.IsTapped(k)) SelectLetter((char)((int)(k - Keys.A) + (int)'a'));
                }
                if (game.IsTapped(Keys.Back)) AssignLetter('_');
                
                if (game.IsTapped(Keys.F1)) btncipherOnly();
                if (game.IsTapped(Keys.F3)) btnKeyOnly() ;
                if (game.IsTapped(Keys.F2)) btnMixedView();
                if (game.IsTapped(Keys.F4) && skill != GameSkills.Pro) btnHint();
                showinfo = game.IsPressed(Keys.OemTilde);
            }
        }


        public override void Render(GameTime gameTime)
        {
            base.Render(gameTime);
            game.spriteBatch.Draw(
                game.getTexture("background"),
                new Vector2(game.gameWidth / 2 - 1280 / 2, game.gameHeight / 2 - 1024 / 2),
                Color.White);
            
            
            DrawCipher();
            DrawLetters();
            if (!cw)
            {
                form.Draw(gameTime);
                if (showinfo) DrawInfo(gameTime);
            }
            else
            {
                DrawWin();
            }
        }

        float winA = 0f, winAR = 0.1f;
        public void DrawWin()
        {
            const int lettersInLine = 20;
            const float characterSpacing = 40;
            Vector2 origin = new Vector2(
                game.gameWidth / 2 - lettersInLine * characterSpacing / 2,
                100);
            game.spriteBatch.Draw(
                game.getTexture("winBG"),
                new Vector2(game.gameWidth / 2 - game.getTexture("cipherBG").Width / 2, origin.Y - 20),
                game.GetColorWithAlpha( Color.White , winA));
            winA += winAR;
            if (winA > 1f) winA = 1f;

        }

        public void DrawCipher()
        {
            const int lettersInLine = 20;
            const float characterSpacing = 40;
            const float lineSpacing = 40;
            Vector2 origin = new Vector2(
                game.gameWidth / 2 - lettersInLine * characterSpacing / 2,
                100);
            game.spriteBatch.Draw(
                game.getTexture("cipherBG"),
                new Vector2( game.gameWidth /2 - game.getTexture("cipherBG").Width / 2 , origin.Y - 20 ),
                Color.White);
            int line = -1, ch = 0;
            for (int i = 0; i < c.length; i++)
            {
                if (i % lettersInLine == 0) { line++; ch=0;}
                if (st == ShowTypes.cipherOnly || !key.ContainsKey(c.cipher[i]))
                {
                    game.DrawShadowedString(game.GetFont("cipherFont"), "" + c.cipher[i],
                        origin + new Vector2(ch * characterSpacing, line * lineSpacing),
                        Color.White);
                }
                else if (st == ShowTypes.Mixed && key.ContainsKey(c.cipher[i]))
                {
                    if (key[c.cipher[i]] != '_')
                        game.DrawShadowedString(game.GetFont("cipherFont"), "" + key[c.cipher[i]],
                        origin + new Vector2(ch * characterSpacing, line * lineSpacing),
                        Color.Yellow);
                    else game.DrawShadowedString(game.GetFont("cipherFont"), "" + c.cipher[i],
                        origin + new Vector2(ch * characterSpacing, line * lineSpacing),
                        Color.White);
                } 
                else
                {
                    game.DrawShadowedString(game.GetFont("cipherFont"), "" + key[c.cipher[i]],
                        origin + new Vector2(ch * characterSpacing, line * lineSpacing),
                        Color.Yellow);
                }
                
                ch++;
            }
        }

        public bool CheckWin()
        {
            return (solvedText == c.mainText);
        }

        public void DrawLetters()
        {
            const int lettersInLine = 13;
            const float offset = 10;
            Vector2 letterOffset = new Vector2(10, 2);
            Texture2D t = game.getTexture("letterBG");
            Vector2 origin = new Vector2(game.gameWidth/2 - lettersInLine*(t.Width+offset)/2, 500);
            Vector2 pos = origin;
            int ch = 0, line = -1;
            for (char i = 'a'; i <= 'z'; i++)
            {
                if (ch % lettersInLine == 0) { ch = 0; line++; }
                pos.X = ch * (t.Width + offset);
                pos.Y = line*(t.Height+ offset);
                
                if (c.key.ContainsValue(i))
                {
                    if (selectedChar == i)
                        game.spriteBatch.Draw(t, origin + pos, Color.Yellow);
                    else
                        if (key[i] != '_' && skill == GameSkills.Amateur )
                            game.spriteBatch.Draw(t, origin + pos, c.IsLetterCorrect( key[i],i) ? Color.Lime: Color.Red);
                        else
                            game.spriteBatch.Draw(t, origin + pos, Color.White);
                    game.DrawShadowedString(game.GetFont("LetterFont"), "" + i, origin + pos + letterOffset, Color.White);
                    game.DrawShadowedString(game.GetFont("LetterFont"), "" + key[i], origin + pos + letterOffset + new Vector2(0, 37), Color.Gold);
                }
                else
                {
                    game.spriteBatch.Draw(t, origin + pos, Color.DarkBlue);
                    game.DrawShadowedString(game.GetFont("LetterFont"), "" + i, origin + pos + letterOffset, Color.Gray);
                }
                ch++;
            }
        }

        public void DrawInfo(GameTime gameTime)
        {
            game.spriteBatch.Draw(game.getTexture("transparent"), new Rectangle(0, 0, game.gameWidth, game.gameHeight), Color.Black);
            game.DrawShadowedString(game.GetFont("menufont"), "Letters Count:", new Vector2(100, 100), Color.Red);
            float f = 130; int l = -1, ll = 5,ch = 0;
            foreach (var item in key)
            {
                if (ch % ll == 0) { l++; ch = 0; }
                int count = 0;
                for (int i = 0; i < c.length; i++)
                {
                    if (item.Key == c.cipher[i]) count++;
                }
                if (count > 0)
                {
                    game.DrawShadowedString(game.GetFont("LetterFont"), item.Key.ToString() + " : " + count.ToString(), 
                        new Vector2(100+ch*130, f+80*l), Color.White);
                    ch++;
                }
            }
            game.DrawShadowedString(game.GetFont("menufont"), "Time:", new Vector2(100, 500), Color.Red);
            TimeSpan a = gameTime.TotalRealTime.Subtract(t);
            game.DrawShadowedString(game.GetFont("menufont"), a.Hours.ToString()+":"+a.Minutes.ToString()+":"+a.Seconds.ToString(), new Vector2(200, 500), Color.White);

        }

        public void SelectLetter()
        {
            Vector2 p = game.mousePosition;

            const int lettersInLine = 13;
            const float offset = 10;
            Vector2 letterOffset = new Vector2(10, 2);
            Texture2D t = game.getTexture("letterBG");
            Vector2 size = new Vector2((float)t.Width, (float)t.Height);
            Vector2 origin = new Vector2(game.gameWidth / 2 - lettersInLine * (t.Width + offset) / 2, 500);
            Vector2 pos = origin;
            int ch = 0, line = -1;
            for (char i = 'a'; i <= 'z'; i++)
            {
                if (ch % lettersInLine == 0) { ch = 0; line++; }
                pos.X = ch * (t.Width + offset);
                pos.Y = line * (t.Height + offset);

                if (c.key.ContainsValue(i))
                {
                    if (Geometry2D.IsVectorInRectangle(origin + pos, size, p))
                    {
                        if (c.IsLetterCorrect(key[i], i) && skill == GameSkills.Amateur) return;
                        selectedChar = i;
                        return;
                    }
                }
                ch++;
            }
           // selectedChar = ' ';
        }

        public void SelectLetter(char ch)
        {
            if (c.key.ContainsValue(ch))
            {
                if (c.IsLetterCorrect(key[ch], ch) && skill == GameSkills.Amateur) return;
                selectedChar = ch;
                return;
            }
        }

        public void AssignLetter(char c)
        {
            if (key.ContainsKey(selectedChar))
            {
                key[selectedChar] = c;
                selectedChar = ' ';
            }
        }
    }
}
