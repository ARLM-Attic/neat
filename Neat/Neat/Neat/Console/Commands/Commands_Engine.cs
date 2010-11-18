using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Microsoft.Xna.Framework.GamerServices;
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
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* e_xliveguide
             * Shows Games for Windows - LIVE Guide
             */
        void e_xliveguide(IList<string> args)
        {
            try { Guide.ShowGamerCard(PlayerIndex.One, (Gamer)Gamer.SignedInGamers[0]); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

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
    }
}