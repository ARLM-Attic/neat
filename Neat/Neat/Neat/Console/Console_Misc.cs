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
        int lc = 0;
        Dictionary<string, int> labels= new Dictionary<string,int>();
        Stack<Dictionary<string, int>> labelsStack = new Stack<Dictionary<string, int>>();
        Stack<int> lcStack = new Stack<int>();
        Dictionary<string, string[]> bufferedScripts = new Dictionary<string, string[]>();
        bool batchEnd = false;
        public void LoadBatch(string path)
        {
            LoadBatch(path, Path.GetFileNameWithoutExtension(path), true);
        }

        public void LoadBatch(string path, string name, bool call)
        {
            if (call && (bufferedScripts.ContainsKey(path.ToLower())))
            {
                ExecuteBatch(bufferedScripts[path]);
                return;
            }

             if (!File.Exists(path))
            {
                WriteLine("File " + path + " does not exist.");
                return;
            }
            
            var batch = File.ReadAllLines(path);

            if (call) ExecuteBatch(batch);
            else bufferedScripts.Add(name.ToLower(), batch);
        }

        public void ExecuteBatch(string[] script)
        {
            labelsStack.Push(labels);
            labels = new Dictionary<string, int>();
            AddCommand("goto", b_goto);
            AddCommand("end", b_end);
            //PASS I: Find Addresses
            for (int i = 0; i < script.Length; i++)
                if (!string.IsNullOrWhiteSpace(script[i]) && script[i].Length > 0 &&
                    (script[i].Trim())[0] == ':') labels.Add(script[i].Trim().Substring(1), i);

            //PASS II: Interpret
            lcStack.Push(lc);
            for (lc = 0; lc < script.Length; lc++)
            {
                if (!String.IsNullOrWhiteSpace(script[lc]) && (script[lc].Trim())[0] != ':')
                {
                    try
                    {
                        command = script[lc];
                        RunCommand();
                        if (batchEnd)
                        {
                            batchEnd = false;
                            break;
                        }
                    }
                    catch
                    {
                        WriteLine("Error running " + command);
                    }
                }
            }

            if (lcStack.Count > 0) lc = lcStack.Pop();
            if (labelsStack.Count > 0) labels = labelsStack.Pop();
        }

        void b_goto(IList<string> args)
        {
            if (labels.ContainsKey(args[1]))
                lc = labels[args[1]];
            else WriteLine("Label " + args[1] + " not found.");
        }

        void b_end(IList<string> args)
        {
            batchEnd = true;
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
                (int)((standAlone ? StandAloneFont : game.GetFont(Font)).MeasureString(rulerHelper).Y) + 1;
        }
    }
}