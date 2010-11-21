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