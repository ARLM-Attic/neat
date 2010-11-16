using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Microsoft.Xna.Framework.GamerServices;
using Neat.Mathematics;
namespace Neat.Console
{
    public partial class Console
    {
        Keys lastkey = Keys.None;
        bool IsPressed(Keys key)
        {
            if ((game.Frame % 20 == 0 && lastkey == key && game.IsPressed(key)) || game.IsReleased(key))
            {
                lastkey = key;
                return true;
            }
            else return false;
        }

        void HandleInput(GameTime gameTime)
        {
            //if (game.frame % 10 ==0)
            {
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
                if (IsPressed(Keys.OemPeriod)) command += '.';
                if (IsPressed(Keys.OemPipe)) command += shift ? '|' : '\\';
                if (IsPressed(Keys.OemPlus)) command += shift ? '+' : '=';
                if (IsPressed(Keys.Divide)) command += '/';
                if (IsPressed(Keys.Space)) command += ' ';
                if (IsPressed(Keys.OemComma)) command += ',';

                //Control Keys
                if (IsPressed(Keys.Back)) command =
                    (command.Length != 0 ? command.Substring(0, command.Length - 1) : "");
            }
            if (game.IsTapped(Keys.Enter, Buttons.A))
            {
                commandsbuffer.Add(command);
                if (echo) WriteLine("  > " + command);
                ParseCommand();
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