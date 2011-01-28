using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
namespace Neat.Components
{
    public partial class Console : GameComponent
    {   
        void s_rem(IList<string> args)
        {
            //eh?
        }

        void s_set(IList<string> args)
        {
            for (int i = 1; i < args.Count; i++)
                ram.Add(args[i], "True");
        }

        void s_free(IList<string> args)
        {
            for (int i = 1; i < args.Count; i++)
                if (ram.ContainsKey(args[i])) ram.Remove(args[i]);
        }

        void s_on(IList<string> args)
        {
            bool b;
            float i;
            if (args[1].ToLower() == bool.TrueString.ToLower() || (bool.TryParse(args[1], out b) && b)) Run(Args2Str(args, 2));
            else if (float.TryParse(args[1], out i)) { if (i != 0) Run(Args2Str(args, 2)); }
            else if (args[1].ToLower() == bool.FalseString.ToLower()) ; // do nothing
            else throw new Exception("Invalid Boolean Expression");
        }

        void s_inc(IList<string> args)
        {
            if (args.Count == 2) ram.Add(args[1], (float.Parse(ram.GetValue(args[1])) + 1).ToString());
            else if (args.Count == 3) ram.Add(args[1], (float.Parse(ram.GetValue(args[1])) + float.Parse(args[2])).ToString());
            else throw new Exception("Error in ~inc");
        }

        void s_quit(IList<string> args)
        {
            game.Exit();
        }

        /* dvar [dvar_name] [value]
             * declares a dvar
             * stores a value to dvar
             */
        void s_dvar(IList<string> args)
        {
            try { ram.Add(args[1], Args2Str(args, 2)); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* echo [message]
             * echo off
             * echo on
             * echo .
             * prints a message
             */
        void s_echo(IList<string> args)
        {
            try
            {
                if (args.Count == 1) WriteLine("");
                else if (args[1] == "on") Echo = true;
                else if (args[1] == "off") Echo = false;
                else if (args[1] == ".") WriteLine("");
                else WriteLine(Args2Str(args, 1));
            }
            catch
            {
                WriteLine("Error in " + Args2Str(args, 0));
            }
        }

        /* call [batchfile]
             * runs a batch file
             */
        void s_call(IList<string> args)
        {
            try { LoadBatch(Args2Str(args, 1)); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* log
             * saves a log to console.log
             */
        void s_log(IList<string> args)
        {
            try { SaveLog("console.log"); }
            catch { WriteLine("Error in log"); }
        }

        void s_dir(IList<string> args)
        {
            var c = 1;
            foreach (var item in Directory.GetFiles(".\\", args[1]))
            {
                if (c++ % 5 == 0) WriteLine("");
                Write(item);
            }
        }

        void s_type(IList<string> args)
        {
            var filename = Args2Str(args, 1);
            if (!File.Exists(filename))
            {
                WriteLine("Error: File " + filename + " does not exist");
                return;
            }
            var f = File.ReadAllLines(filename);
            foreach (var item in f)
            {
                WriteLine(item);
            }
        }

        ////////Logics
        /* loop [count] [command]
         */
        void l_loop(IList<string> args)
        {
            try
            {
                int count = int.Parse(args[1]);
                string command = Args2Str(args, 2);
                for (int i = 0; i < count; i++) Run(command);
            }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }

        }
    }
}