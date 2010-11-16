using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Microsoft.Xna.Framework.GamerServices;
namespace Neat.Console
{
    public partial class Console
    {
        public void ParseCommand()
        {
            command = command.Trim();
            if (command.Length < 1) return;

            if (command.Contains(eval_open)) // there's a dvar needs to be evaluated
            {
                //bruteforce
                int start = command.IndexOf(eval_open);
                int end = start;
                int level = 0;
                for (int i = start; i < command.Length; i++)
                {
                    if (command[i] == eval_open) level++;
                    else if (command[i] == eval_close) level--;
                    if (level == 0) { end = i; break; }
                }
                if (end == start) // error!
                {
                    WriteLine("Error evaluating expression - " + command);
                    return;
                }
                else //replace
                {
                    string left = command.Substring(0, start);
                    string right = command.Substring(end + 1);
                    string mid = command.Substring(start + 1, end - start - 1);
                    command = left + ram.GetValue(mid) + right;
                }
                ParseCommand(); //parse again!
                return;
            }
            List<string> args = new List<string>();
            string k = "";
            string cmd = command;
            for (int i = 0; i < cmd.Length; i++)
            {
                if (cmd[i] == ' ')
                {
                    cmd = (cmd.Substring(i)).Trim();
                    i = -1;
                    args.Add(k);
                    k = "";
                }
                else k += cmd[i];
            }
            args.Add(k);

            Run(args);
            //command = "";
        }

        //ParseColor(string) -> color 
        //returns new Color() on error
        //input methods:              
        //r,g,b                       
        //r,g,b,a 0f-1f               
        public Color ParseColor(string p)
        {
            Color cl = new Color();

            List<string> args = new List<string>();
            args.Add("");

            string k = "";
            string cmd = p;
            for (int i = 0; i < cmd.Length; i++)
            {
                if (cmd[i] == ' ')
                {
                    cmd = (cmd.Substring(i)).Trim();
                    i = -1;
                    args.Add(k);
                    k = "";
                }
                else k += cmd[i];
            }
            args.Add(k);

            try
            {
                if (args[1] == "black") cl = Color.Black;
                else if (args[1] == "blue") cl = Color.Blue;
                else if (args[1] == "red") cl = Color.Red;
                else if (args[1] == "green") cl = Color.Green;
                else if (args[1] == "yellow") cl = Color.Yellow;
                else if (args[1] == "purple") cl = Color.Purple;
                else if (args[1] == "brown") cl = Color.Brown;
                else if (args[1] == "white") cl = Color.White;
                else if (args[1] == "gray") cl = Color.Gray;
                else if (args.Count > 2) // custom color - byte, byte, byte [, byte(alpha) ]
                {
                    try
                    {
                        if (args.Count == 4)
                            cl = new Color(
                                new Vector3(float.Parse(args[1]),  //R
                                    float.Parse(args[2]), //G
                                    float.Parse(args[3])));
                        else if (args.Count == 5)
                            cl = new Color(
                                new Vector4(float.Parse(args[1]),  //R
                                    float.Parse(args[2]), //G
                                    float.Parse(args[3]), //B
                                    float.Parse(args[4]))); //A
                        else
                            WriteLine("Error in parameters");
                    }
                    catch { WriteLine("Error: Cannot create color"); }
                }
                else { WriteLine("Bad Color"); }
            }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
            return cl;
        }

        public string Args2Str(IList<string> args, int startIndex)
        {
            string result = "";
            for (int i = startIndex; i < args.Count; i++)
                result += args[i] + " ";
            return result.Trim();
        }
    }
}