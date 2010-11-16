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
namespace Neat.Console
{
    public partial class Console
    {
        //////////////////////////////////////////////ENGINE
        /* e_activatepart [partname]
         * Activates a gamepart
         */
        void e_activatepart(IList<string> args)
        {
            try { game.ActivatePart(Args2Str(args, 1)); }
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
                game.freezed = !game.freezed;
                WriteLine("e_freeze is " + (game.freezed ? "ON" : "OFF"));
            }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

    }
}