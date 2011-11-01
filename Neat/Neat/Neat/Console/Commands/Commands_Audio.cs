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
        /* a_sfx [sfxname]
             * plays a sound effect
             */
        void a_sfx(IList<string> args)
        {
            try { game.PlaySound(Args2Str(args, 1)); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* a_mutesounds [bool]
         * Toggles game.muteallsounds
         */
        void a_mutesounds(IList<string> args)
        {
            game.MuteAllSounds = bool.Parse(args[1]);
        }

        /* a_medianext
             * Plays next track
             */
        void a_medianext(IList<string> args)
        {
            try { MediaPlayer.MoveNext(); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* a_mediaprev
             * Plays prev track
             */
        void a_mediaprev(IList<string> args)
        {
            try { MediaPlayer.MovePrevious(); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* a_mediapause*/
        void a_mediapause(IList<string> args)
        {
            try { MediaPlayer.Pause(); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* a_mediastop*/
        void a_mediastop(IList<string> args)
        {
            try { MediaPlayer.Stop(); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* a_mediaresume*/
        void a_mediaresume(IList<string> args)
        {
            try { MediaPlayer.Resume(); }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* a_mediaplay*/
        void a_mediaplay(IList<string> args)
        {
            try
            {
                if (args.Count > 1 && args[1] == "library") MediaPlayer.Play(new MediaLibrary().Songs);
                else MediaPlayer.Play(game.GetSong(Args2Str(args, 1)));
            }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        /* a_songinfo */
        void a_songinfo(IList<string> args)
        {
            try
            {
                WriteLine("Song Name: " + MediaPlayer.Queue.ActiveSong.Name);
                WriteLine("Album: " + MediaPlayer.Queue.ActiveSong.Album);
                WriteLine("Artist: " + MediaPlayer.Queue.ActiveSong.Artist);
                WriteLine("Duration: " + MediaPlayer.Queue.ActiveSong.Duration);
            }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
        }

        void a_mediashuffle(IList<string> args)
        {
            if (args.Count == 1) WriteLine(MediaPlayer.IsShuffled.ToString());
            else MediaPlayer.IsShuffled = bool.Parse(args[1]);
        }

        void a_mediarepeat(IList<string> args)
        {
            if (args.Count == 1) WriteLine(MediaPlayer.IsRepeating.ToString());
            else MediaPlayer.IsRepeating = bool.Parse(args[1]);
        }
    }
}