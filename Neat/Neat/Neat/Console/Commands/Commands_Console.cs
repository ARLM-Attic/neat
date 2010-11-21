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
        /* c_textcolor, c_inputcolor, c_backcolor [ ]
             * changes console's colors
            */
        void c_textcolor(IList<string> args)
        {
            try { TextColor = ParseColor(Args2Str(args, 1)); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        void c_inputcolor(IList<string> args)
        {
            try { InputColor = ParseColor(Args2Str(args, 1)); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        void c_backcolor(IList<string> args)
        {
            try { BackColor = ParseColor(Args2Str(args, 1)); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* c_show/hide
             * shows/hides the console
             */
        void c_show(IList<string> args)
        {
            try { IsActive = true; }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        void c_hide(IList<string> args)
        {
            try { IsActive = false; }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* c_texture [texturename]
             * changes console's background texture
             */
        void c_texture(IList<string> args)
        {
            try { BackTexture = Args2Str(args, 1); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* c_font [fontname]
             * changes console's font
             */
        void c_font(IList<string> args)
        {
            try { Font = Args2Str(args, 1); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* clear
         * clears the console
         */
        void c_clear(IList<string> args)
        {
            Clear();
        }

        /* c_size [int]
             * changes console's size (in lines)
             */
        void c_size(IList<string> args)
        {
            //try { 
            //TODO: write c_size
        }
    }
}