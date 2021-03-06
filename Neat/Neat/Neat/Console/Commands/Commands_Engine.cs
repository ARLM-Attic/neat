﻿using System;
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
        //////////////////////////////////////////////ENGINE
        /* e_show [screenname]
         * Activates a Screen
         */
        void e_show(IList<string> args)
        {
            try { game.ActivateScreen(Args2Str(args, 1)); }
            catch (Exception e)
            {
                WriteLine("Error in " + Args2Str(args, 0));
                WriteLine(e.Source + ": " + e.Message);
            }
        }

#if LIVE
        /* e_xliveguide
             * Shows Games for Windows - LIVE Guide
             */
        void e_xliveguide(IList<string> args)
        {
            try { Guide.ShowGamerCard(PlayerIndex.One, (Gamer)Gamer.SignedInGamers[0]); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }
#endif

        /* e_freeze
         * freezes the game
         */
        void e_freeze(IList<string> args)
        {
            try
            {
                game.Freezed = !game.Freezed;
                WriteLine("e_freeze is " + (game.Freezed ? "ON" : "OFF"));
            }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* e_framerate [integer]
         * changes the frame rate. tested values are 30 and 60
         */
        void e_framerate(IList<string> args)
        {
            try
            {
                game.SetFrameRate(double.Parse(args[1]));
            }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* e_title [string]
         * changes window's title
         */
        void e_title(IList<string> args)
        {
            game.Window.Title = Args2Str(args, 1);
        }

        void e_lockfps(IList<string> args)
        {
            if (args.Count > 1)
                game.IsFixedTimeStep = bool.Parse(args[1]);
            else
                WriteLine(game.IsFixedTimeStep.ToString());
        }
    }
}