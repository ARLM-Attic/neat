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
using Neat.Mathematics;
namespace Neat.Components
{
    public partial class Console : GameComponent
    {
        Keys lastkey = Keys.None;
        int lastKeyTime = 0;
        public int KeyboardRepeatDelay = 15;
        bool IsPressed(Keys key)
        {
            if ((lastKeyTime > KeyboardRepeatDelay && lastkey == key && game.IsPressed(key)) || game.IsTapped(key))
            {
                lastKeyTime = 0;
                lastkey = key;
                return true;
            }
            else return false;
        }

        void HandleInput(GameTime gameTime)
        {
            lastKeyTime++;
            bool shift = (game.IsPressed(Keys.LeftShift) || game.IsPressed(Keys.RightShift));
            bool ctrl = (game.IsPressed(Keys.LeftControl) || game.IsPressed(Keys.RightControl));
            //Alphabet, Numbers, Space
            for (int i = 0; i < 26; i++)
            {
                if (IsPressed(Keys.A + i))
                    if (shift) command += (char)('A' + i);
                    else command += (char)('a' + i);

                if (i <= 9 && (IsPressed(Keys.D0 + i) || IsPressed(Keys.NumPad0 + i)))
                {
                    if (!shift)
                        command += (char)('0' + i);
                    else if (i == 1) command += '!';
                    else if (i == 2) command += '@';
                    else if (i == 3) command += '#';
                    else if (i == 4) command += '$';
                    else if (i == 5) command += '%';
                    else if (i == 6) { if (ctrl) command += GeometryHelper.Vector2String(game.mousePosition); else command += '^'; }
                    else if (i == 7) command += '&';
                    else if (i == 8) command += '*';
                    else if (i == 9) command += '(';
                    else if (i == 0) command += ')';
                }
            }
            if (IsPressed(Keys.OemBackslash)) command += '\\';
            if (IsPressed(Keys.Subtract) || game.IsTapped(Keys.OemMinus)) command += shift ? '_' : '-';
            if (IsPressed(Keys.OemPipe)) command += shift ? '|' : '\\';
            if (IsPressed(Keys.OemPlus)) command += shift ? '+' : '=';
            if (IsPressed(Keys.Divide)) command += '/';
            if (IsPressed(Keys.Space)) command += ' ';
            if (IsPressed(Keys.OemPeriod)) if (shift) command += '>'; else command += '.';
            if (IsPressed(Keys.OemComma)) if (shift) command += '<'; else command += ',';

            //Control Keys
            if (IsPressed(Keys.Back)) command = (command.Length != 0 
                ? command.Substring(0, command.Length - 1) 
                : "");
            
            if (IsPressed(Keys.PageUp))
            {
                mOffset--;
            }
            else if (IsPressed(Keys.PageDown))
            {
                mOffset++;
            }
            else if (IsPressed(Keys.Tab))
            {
                List<string> s = new List<string>();
                foreach (var item in Commands.Keys)
                {
                    if (item.StartsWith(command.Trim())) s.Add(item);
                }
                if (s.Count == 0)
                {
                    WriteLine("No commands found.");
                }
                else if (s.Count == 1)
                {
                    command += s[0].Substring(command.Trim().Length)+" ";
                }
                else
                {
                    if (command.Length >= 2)
                    {
                        var sample = s[0];
                        int cindex = command.Trim().Length;
                        for (; cindex < sample.Length; cindex++)
                        {
                            bool done = false;
                            for (int i = 1; i < s.Count; i++)
                            {
                                if (!s[i].StartsWith(sample.Substring(0,cindex)))
                                {
                                    done = true;
                                    break;
                                }
                            }
                            if (done) break;
                        }
                        command = sample.Substring(0,cindex-1);
                    }
                    for (int i = 0; i < s.Count; i++)
                    {
                        Write(s[i] + "   ");
                        if (i > 0 && i % 5 == 0) WriteLine("");
                    }
                    WriteLine("");
                }
            }

            if (game.IsTapped(Keys.Enter, Buttons.A))
            {
                commandsbuffer.Add(command);
                if (Echo) WriteLine("  > " + command);
                RunCommand();
                command = "";
            }
            else if (game.IsTapped(Keys.Escape, Buttons.Back)) command = "";
            
            if (commandsbuffer.Count > 0)
            {
                if (game.IsTapped(Keys.Up)) GetNextCommand();
                else if (game.IsTapped(Keys.Down)) GetPrevCommand();
            }
        }
    }
}