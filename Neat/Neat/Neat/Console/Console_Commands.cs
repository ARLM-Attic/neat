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
        public virtual void Run(string c)
        {
            var t = command;
            command = c;
            ParseCommand();
            command = t;
        }
        public Dictionary<string, Action<IList<string>>> Commands;

        public void AddCommand(string key, Action<IList<string>> act)
        {
            if (Commands.ContainsKey(key)) Commands.Remove(key);
            Commands.Add(key, act);
        }
        void InitCommands()
        {
            if (standAlone)
            {
                Commands = new Dictionary<string, Action<IList<string>>>
                {
                {"rem"                  ,s_rem},
                {"clear"                ,c_clear},
                {"dvar"                 ,s_dvar},
                {"dv"                   ,s_dvar},
                {"echo"                 ,s_echo},
                {"call"                 ,s_call},
                {"log"                  ,s_log},
                {"loop"                 ,l_loop},
                {"set"                  ,s_set},
                {"free"                 ,s_free},
                {"on"                   ,s_on},
                {"if"                   ,s_on},
                {"inc"                  ,s_inc},
                {"type"                 ,s_type},
                {"c_textcolor"          ,c_textcolor},
                {"c_inputcolor"         ,c_inputcolor},
                {"c_backcolor"          ,c_backcolor},
                {"c_show"               ,c_show},
                {"c_hide"               ,c_hide},
                {"c_clear"              ,c_clear},
                {"c_lines"              ,c_lines},
                {"c_openspeed"          ,c_curtainspeed},
                {"c_keydelay"           ,c_keydelay}};
            }
            else
            {
                Commands = new Dictionary<string, Action<IList<string>>>
            {
                //system
                {"rem"                  ,s_rem},
                {"clear"                ,c_clear},
                {"quit"                 ,s_quit},
                {"dvar"                 ,s_dvar},
                {"dv"                   ,s_dvar},
                {"echo"                 ,s_echo},
                {"call"                 ,s_call},
                {"log"                  ,s_log},
                {"loop"                 ,l_loop},
                {"set"                  ,s_set},
                {"free"                 ,s_free},
                {"on"                   ,s_on},
                {"if"                   ,s_on},
                {"inc"                  ,s_inc},
                {"type"                 ,s_type},
                {"dir"                  ,s_dir},

                //graphics
                {"g_res"                ,g_res},
                {"g_size"               ,g_size},
                {"g_stretchmode"        ,g_stretchmode},
                {"g_backcolor"          ,g_background},
                {"g_outputcolor"        ,g_outputcolor},
                {"g_reinit"             ,g_reinit},
                {"g_fullscreen"         ,g_fullscreen},
                {"g_autoclear"          ,g_autoclear},
                {"g_showmouse"          ,g_showmouse},
                {"g_saymessage"         ,g_saymessage},
                {"g_messagecolor"       ,g_messagecolor},
                {"g_assigntexture"      ,g_assigntexture},

                //console
                {"c_textcolor"          ,c_textcolor},
                {"c_inputcolor"         ,c_inputcolor},
                {"c_backcolor"          ,c_backcolor},
                {"cfx_color"            ,cfx_color},
                {"c_allowcolors"        ,c_allowcolors},
                {"c_setcolorcode"       ,c_setcolorcode},
                {"c_removecolorcode"    ,c_removecolorcode},
                {"c_setcolorchar"       ,c_setcolorchar},
                {"c_show"               ,c_show},
                {"c_hide"               ,c_hide},
                {"c_texture"            ,c_texture},
                {"c_font"               ,c_font},
                {"c_clear"              ,c_clear},
                {"c_lines"              ,c_lines},
                {"c_showonbottom"       ,c_showonbottom},
                {"c_openspeed"          ,c_curtainspeed},
                {"c_keydelay"           ,c_keydelay},

                //audio
                {"a_sfx"                ,a_sfx},
                {"a_medianext"          ,a_medianext},
                {"a_mediaprev"          ,a_mediaprev},
                {"a_mediapause"         ,a_mediapause},
                {"a_mediastop"          ,a_mediastop},
                {"a_mediaresume"        ,a_mediaresume},
                {"a_mediaplay"          ,a_mediaplay},
                {"a_songinfo"           ,a_songinfo},
                {"a_mutesounds"         ,a_mutesounds},

                //engine
                {"e_show"               ,e_show},
                {"e_framerate"          ,e_framerate},
                {"sh"                   ,e_show},
                {"e_title"              ,e_title},
#if LIVE
                {"e_xliveguide"         ,e_xliveguide},
#endif
                {"e_freeze"             ,e_freeze},
                {"fr"                   ,e_freeze},

                //files
                {"load"                 ,f_load},

                //network
#if LIVE
                {"net_create"           ,net_create},
                {"net_join"             ,net_join},
                {"net_find"             ,net_find},
                {"net_stats"            ,net_stats},
                {"net_startgame"        ,net_startgame},
                {"net_ready"            ,net_ready},
                {"net_unready"          ,net_unready},
                {"net_showchat"         ,net_showchat},
                {"net_signin"           ,net_signin},
                {"net_endgame"          ,net_endgame},
                {"net_close"            ,net_disconnect},
                {"disconnect"           ,net_disconnect}
#endif

            };
            }
        }
        public virtual void Run(List<string> args)
        {
            if (args[0] == "rem") return;
            else if (Commands.ContainsKey(args[0]))
            {
                try
                {
                    Commands[args[0]](args);
                }
                catch (Exception e)
                {
                    WriteLine("#ERROR IN "+Args2Str(args,0)+":" + e.Message);
                }
            }
            else // no match
            {
                if (bufferedScripts.ContainsKey(args[0].ToLower()))
                    ExecuteBatch(bufferedScripts[args[0].ToLower()]);
                else
                    WriteLine("Error: Bad Command.");
            }
        }
    }
}