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
namespace Neat.Components
{
    public partial class Console : GameComponent
    {
        public void Clear()
        {
            buffer = new List<string>();
        }

        public void Write(string text)
        {
            if (buffer.Count > 0) buffer[buffer.Count - 1] += text;
            else buffer.Add(text);
        }

        public void WriteLine(string text)
        {
            Write(text);
            buffer.Add("");
        }

        public string GetMessages(int n)
        {
            int o = 0;
            return GetMessages(n, ref o);
        }

        public string GetMessages(int count, ref int offset)
        {
            if (offset > 0) offset = 0;
            else if (buffer.Count - 1 <= offset) 
                offset = -(buffer.Count - 1);
            if (buffer.Count == 0) return "";
            string result = "";
            for (int i = buffer.Count - 1 + offset, a = 0; i >= 0 && a < count; i--, a++)
            {
                result = (offset != 0 ? "  "+(buffer.Count - i).ToString()+"~ ":"")+ buffer[i] + '\n' + result;
            }
            return result.Trim();
        }
    }
}