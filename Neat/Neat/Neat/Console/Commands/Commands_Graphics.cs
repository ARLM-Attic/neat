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
        /* g_res [width] [height]
             * Changes the screen resolution
             */
        void g_res(IList<string> args)
        {
            try
            {
                int _w = int.Parse(args[1]);
                int _h = int.Parse(args[2]);
                if (game.StretchMode == NeatGame.StretchModes.None)
                {
                    game.GameWidth = _w;
                    game.GameHeight = _h;
                }
                else
                {
                    game.OutputResolution.X = _w;
                    game.OutputResolution.Y = _h;
                }
            }
            catch
            {
                WriteLine("Error in " + Args2Str(args, 0));
            }
        }

        /*g_size [width] [height]
         * Changes game's boundaries
         */
        void g_size(IList<string> args)
        {
            try
            {
                int _w = int.Parse(args[1]);
                int _h = int.Parse(args[2]);
                game.GameWidth = _w;
                game.GameHeight = _h;
            }
            catch
            {
                WriteLine("Error in " + Args2Str(args, 0));
            }
        }

        /* g_stretchmode {none|fit|stretch|center}
         * Changes game's stretch mode
         */
        void g_stretchmode(IList<string> args)
        {
            if (args.Count == 1)
            {
                WriteLine("Stretch Mode = " + game.StretchMode.ToString());
                return;
            }
            args[1] = args[1].ToLower();
            if (args[1] == "none") game.StretchMode = NeatGame.StretchModes.None;
            else if (args[1] == "fit") game.StretchMode = NeatGame.StretchModes.Fit;
            else if (args[1] == "stretch") game.StretchMode = NeatGame.StretchModes.Stretch;
            else if (args[1] == "center") game.StretchMode = NeatGame.StretchModes.Center;
            else WriteLine("Invalid Mode.");
        }

        /* g_reinit
             * Reinitializes graphics
             */
        void g_reinit(IList<string> args)
        {
            try { game.InitializeGraphics(); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* g_fullscreen [bool]
             * g_fullscreen
             * Changes fullscreen state
             */
        void g_fullscreen(IList<string> args)
        {
            if (args.Count == 1) //Toggle Fullscreen
                game.FullScreen = !game.FullScreen;
            else try //Has Parameters
                {
                    bool _state = bool.Parse(args[1]);
                    game.FullScreen = _state;
                }
                catch
                {
                    WriteLine("Error in " + Args2Str(args, 0));
                }
        }

        /* g_showmouse [bool]
         *  g_showmouse
         *  shows/hides the mouse pointer
         */
        void g_showmouse(IList<string> args)
        {
            if (args.Count == 1) //Toggle Fullscreen
                game.ShowMouse = !game.ShowMouse;
            else try //Has Parameters
                {
                    bool _state = bool.Parse(args[1]);
                    game.ShowMouse = _state;
                }
                catch
                {
                    WriteLine("Error in " + Args2Str(args, 0));
                }
        }

        /* g_autoclear [bool]
             * Changes autoclear state
             */
        void g_autoclear(IList<string> args)
        {
            if (args.Count == 1) //Toggle Fullscreen
                game.AutoClear = !game.AutoClear;
            else try //Has Parameters
                {
                    bool _state = bool.Parse(args[1]);
                    game.AutoClear = _state;
                }
                catch
                {
                    WriteLine("Error in " + Args2Str(args, 0));
                }
        }

        /* g_background [color]
             * Changes background color
             */
        void g_background(IList<string> args)
        {
            try { game.GameBackgroundColor = ParseColor(Args2Str(args, 1)); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* g_outputcolor [color] */
        void g_outputcolor(IList<string> args)
        {
            try { game.OutputBackgroundColor = ParseColor(Args2Str(args, 1)); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* g_drawsprite [floatx] [floaty] [texturename]
             * g_drawsprite [int left] [int top] [int w] [int h] [texturename]
             * Draws a sprite/texture
             */
        void g_drawsprite(IList<string> args)
        {
            //TODO: g_drawsprite - FIX DA BUG
            try
            {
                if (args.Count > 4)
                    game.SpriteBatch.Draw(game.GetTexture(Args2Str(args, 5)),
                        new Rectangle(int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4])), Color.White);
                else
                    game.SpriteBatch.Draw(game.GetTexture(args[3]), //game.getTexture(Args2Str(args, 3)),
                        new Vector2(float.Parse(args[1]), float.Parse(args[2])), Color.White);
            }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* g_saymessage [message]
         */
        void g_saymessage(IList<string> args)
        {
            game.TextEffects.Echo(Args2Str(args, 1));
        }

        /* g_messagecolor [color]
            */
        void g_messagecolor(IList<string> args)
        {
            try { game.TextEffects.ForeColor = ParseColor(Args2Str(args, 1)); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        void g_assigntexture(IList<string> args)
        {
            game.AssignTexture(args[1], args[2]);
        }
    }
}