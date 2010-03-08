using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CipherPuzzle
{
    public class Cipher
    {
        Random _randomGenerator;

        public string mainText;
        public string cipher;
        public Dictionary<char, char> key;
        
        public int length
        {
            get { return mainText.Length; }
        }

        public Cipher(Random r)
        {
            _randomGenerator = r;
        }

        public void Generatecipher(string text)
        {
            List<char> staticChars = new List<char>();
            staticChars.Add(' ');
            staticChars.Add('.');
            staticChars.Add('\'');
            staticChars.Add(',');
            staticChars.Add('/');
            staticChars.Add('"');
            staticChars.Add('\n');
            staticChars.Add('-');
            staticChars.Add('!');
            staticChars.Add(';');
            staticChars.Add(':');
            for (char i = '0'; i <= '9'; i++) staticChars.Add(i);
            this.mainText = text.ToLower();
            List<char> letters = new List<char>();
            for (int i = 0; i < length; i++)  if (!letters.Contains(mainText[i])) letters.Add(mainText[i]);
            letters.Sort();
            foreach (var item in staticChars) if (letters.Contains(item)) letters.RemoveAt(letters.IndexOf(item));
            char l = (char)((int)'a' + letters.Count);
            key = new Dictionary<char, char>();
            foreach (var item in staticChars) key.Add(item, item);
            for (char i = 'a'; i < l; i++)
            {
                int k = _randomGenerator.Next(letters.Count);
                key.Add( letters[k],i);
                letters.RemoveAt(k);
            }
            cipher = "";
            
            for (int i = 0; i < length; i++)
            {
                cipher += key[mainText[i]];
            }
        }

        public List<char> StringToList(string text)
        {
            List<char> r = new List<char>();
            for (int i = 0; i < length; i++)
            {
                r.Add(text[i]);
            }
            return r;
        }

        public bool IsLetterCorrect(char k, char input)
        {
            foreach (var item in key)
            {
                if (item.Value == input && item.Key == k) return true;
            }
            return false;
        }

        public char GetHint(char input)
        {
            foreach (var item in key)
            {
                if (item.Value == input) return item.Key;
            }
            return '_';
        }
    }
}
