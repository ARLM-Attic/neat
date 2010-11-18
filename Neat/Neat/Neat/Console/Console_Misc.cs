using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Microsoft.Xna.Framework.GamerServices;
namespace Neat.Components
{
    public partial class Console : GameComponent
    {
        public void LoadBatch(string path)
        {
            try
            {
                StreamReader r = new StreamReader(path);
                while (!r.EndOfStream)
                {
                    command = r.ReadLine();
                    try
                    {
                        ParseCommand();
                    }
                    catch
                    {
                        WriteLine("Error running " + command);
                    }
                }
                r.Close();
            }
            catch
            {
                WriteLine("Error running batch file " + path);
            }
        }

        public void SaveLog(string path)
        {
            try
            {
                StreamWriter s = new StreamWriter(path, false, System.Text.Encoding.Unicode);
                foreach (var item in buffer) s.WriteLine(item);
                s.Close();
                WriteLine("log saved to " + path);
            }
            catch
            {
                WriteLine("error saving log.");
            }
        }

        void GetNextCommand()
        {
            string v = command;
            command = commandsbuffer[0];
            for (int i = 1; i < commandsbuffer.Count; i++) commandsbuffer[i - 1] = commandsbuffer[i];
            commandsbuffer.RemoveAt(commandsbuffer.Count - 1);
            if (v.Length > 0) commandsbuffer.Add(v);
        }

        void GetPrevCommand()
        {
            string v = command;
            command = commandsbuffer[commandsbuffer.Count - 1];
            for (int i = commandsbuffer.Count - 2; i >= 0; i--) commandsbuffer[i + 1] = commandsbuffer[i];
            if (v.Length > 0) commandsbuffer[0] = v;
            else commandsbuffer.RemoveAt(0);
        }

        public int MeasureHeight(int _lines)
        {
            string rulerHelper = "Z";
            for (int i = 0; i < _lines; i++) rulerHelper += "\n";
            rulerHelper += "Z";

            return
                (int)(game.GetFont(Font).MeasureString(rulerHelper).Y) + 1;
        }
    }
}