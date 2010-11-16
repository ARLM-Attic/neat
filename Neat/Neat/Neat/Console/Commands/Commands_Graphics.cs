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
        /* g_res [width] [height]
             * Changes the screen boundaries
             */
        void g_res(IList<string> args)
        {
            try
            {
                int _w = int.Parse(args[1]);
                int _h = int.Parse(args[2]);
                game.gameWidth = _w;
                game.gameHeight = _h;
            }
            catch
            {
                WriteLine("Error in " + Args2Str(args, 0));
            }
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
                game.fullscreen = !game.fullscreen;
            else try //Has Parameters
                {
                    bool _state = bool.Parse(args[1]);
                    game.fullscreen = _state;
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
                game.showMouse = !game.showMouse;
            else try //Has Parameters
                {
                    bool _state = bool.Parse(args[1]);
                    game.showMouse = _state;
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
                game.autoClear = !game.autoClear;
            else try //Has Parameters
                {
                    bool _state = bool.Parse(args[1]);
                    game.autoClear = _state;
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
            try { game.backGroundColor = ParseColor(Args2Str(args, 1)); }
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
                    game.spriteBatch.Draw(game.getTexture(Args2Str(args, 5)),
                        new Rectangle(int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4])), Color.White);
                else
                    game.spriteBatch.Draw(game.getTexture(args[3]), //game.getTexture(Args2Str(args, 3)),
                        new Vector2(float.Parse(args[1]), float.Parse(args[2])), Color.White);
            }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* g_saymessage [message]
         */
        void g_saymessage(IList<string> args)
        {
            SayMessage(Args2Str(args, 1));
        }

        /* g_messagecolor [color]
            */
        void g_messagecolor(IList<string> args)
        {
            try { fx_Color = ParseColor(Args2Str(args, 1)); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }
    }
}