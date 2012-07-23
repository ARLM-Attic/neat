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
            string oldcmd = command;
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
                    else if (i == 6) { if (ctrl) command += GeometryHelper.Vector2String(game.MousePosition); else command += '^'; }
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

            if (SoundEffects && command != oldcmd) game.PlaySound("console_keystroke");

            if (IsPressed(Keys.PageUp))
            {
                mOffset--;
                if (SoundEffects) game.PlaySound("console_echo");
            }
            else if (IsPressed(Keys.PageDown))
            {
                mOffset++;
                if (SoundEffects) game.PlaySound("console_echo");
            }
            else if (IsPressed(Keys.Tab))
            {
                if (SoundEffects) game.PlaySound("console_space");

                var args = command.Split(' ');
                var lastArg = args.LastOrDefault();
                var oldLastArg = lastArg;
                var keys = ((args.Length <= 1) || args.Length > 1 && !Autocompletes.ContainsKey(args[0].ToLower())) ? Commands.Keys.ToList()
                    : Autocompletes[args[0].ToLower()](args);

                List<string> s = new List<string>();
                if (keys != null)
                    foreach (var item in keys)
                    {
                        if (item.StartsWith(lastArg.Trim())) s.Add(item);
                    }
                if (s.Count == 0)
                {
                    WriteLine("No commands found.");
                }
                else if (s.Count == 1)
                {
                    lastArg += s[0].Substring(lastArg.Trim().Length) + " ";
                }
                else
                {
                    //if (lastArg.Length >= 2)
                    {
                        var sample = s[0];
                        int cindex = lastArg.Trim().Length;
                        for (; cindex <= sample.Length; cindex++)
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

                        lastArg = sample.Substring(0, cindex - 1);
                        if (lastArg.Length < oldLastArg.Length) lastArg = oldLastArg;
                    }
                    for (int i = 0; i < s.Count; i++)
                    {
                        Write(s[i] + "   ");
                        if (i > 0 && i % 5 == 0) WriteLine("");
                    }
                    WriteLine("");   
                }

                if (lastArg != oldLastArg)
                {
                    command = "";
                    for (int i = 0; i < args.Length - 1; i++)
                        command += args[i] + " ";
                    command += lastArg;
                }
            }

            if (game.IsTapped(Keys.Enter, Buttons.A))
            {
                if (SoundEffects) game.PlaySound("console_return");
                commandsbuffer.Add(command);
                
                RunCommand();
                command = "";
            }
            else if (game.IsTapped(Keys.Escape, Buttons.Back)) command = "";
            
            if (commandsbuffer.Count > 0)
            {
                if (game.IsTapped(Keys.Down)) GetNextCommand();
                else if (game.IsTapped(Keys.Up)) GetPrevCommand();
            }
        }
    }
}