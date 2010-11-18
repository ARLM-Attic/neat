using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Microsoft.Xna.Framework.GamerServices;
namespace Neat.Components
{
    public partial class Console : GameComponent
    {
#if XLIVE
        void net_create(IList<string> args)
        {
            game.networkHelper.CreateSession();
        }
        void net_join(IList<string> args)
        {
            game.networkHelper.FindSession();
        }
        void net_find(IList<string> args)
        {
            game.networkHelper.JustFindSessions();
            try
            {
                WriteLine("JustFindSessionsResult: " + game.networkHelper.JustFindSessionsResult);
            }
            catch
            {
                WriteLine("JustFindSessionsResult: ERROR");
            }
        }
        void net_stats(IList<string> args)
        {
            WriteLine("NETWORK STATUS:");
            WriteLine("isHost: " + game.networkHelper.isHost().ToString());
            WriteLine("Bandwidth: " + game.networkHelper.GetRecieveSpeed().ToString() + "-recieve   " +
                game.networkHelper.GetSendSpeed().ToString() + "-send");
            foreach (var player in game.networkHelper.session.AllGamers)
            {
                Write(player.Id.ToString() + ": ");
                Write(player.Gamertag + " - ");
                Write("PING:" + player.RoundtripTime.TotalSeconds.ToString() + " - ");
                Write("READY:" + player.IsReady.ToString());
                WriteLine("");
            }
        }
        void net_startgame(IList<string> args)
        {
            game.networkHelper.StartGame();
        }
        void net_unready(IList<string> args)
        {
            game.networkHelper.SetPlayerNotReady();
        }
        void net_ready(IList<string> args)
        {
            game.networkHelper.SetPlayerReady();
        }
        void net_showchat(IList<string> args)
        {
            game.networkHelper.ShowChat();
        }
        void net_signin(IList<string> args)
        {
            game.networkHelper.SignInGamer();
        }
        void net_endgame(IList<string> args)
        {
            game.networkHelper.EndGame();
        }
        void net_disconnect(IList<string> args)
        {
            game.networkHelper.CloseSession();
        }
#endif
    }
}