using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
#if WINDOWS
 
 
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;
using System.Diagnostics;
 
namespace Neat
{
    public partial class NeatGame : Microsoft.Xna.Framework.Game
    {
        GameTime gamestime;
        // Helper funcs for debug n messaging
        // It's better to use Console.
        string[] gameMessages;
        public int GameMessagesCount = 7;

        public Vector2 messagesPosition = Vector2.Zero;

        void InitializeMessages()
        {
            ResetMessages();
        }

        public void SayMessage(string msg, bool fail = false)
        {
            Debug.WriteLine(msg);
            if (gamestime != null)
            {
                msg = gamestime.TotalGameTime.Hours.ToString() + ":" +
                    gamestime.TotalGameTime.Minutes.ToString() + ":" +
                    gamestime.TotalGameTime.Seconds.ToString() + "," +
                    gamestime.TotalGameTime.Milliseconds.ToString() + " ->" +
                    msg;
            }
            for (int i = 0; i < GameMessagesCount - 1; i++)
            {
                gameMessages[i] = gameMessages[i + 1];
            }
            gameMessages[GameMessagesCount - 1] = msg;
            
            Debug.Assert(!fail, msg);
        }

        public string GetMessages()
        {
            string r = "";
            foreach (string m in gameMessages)
            {
                r += m + "\n";
            }
            return r.Remove(0, r.LastIndexOf('\n', GameMessagesCount)+1);
        }

        public void ResetMessages()
        {
            gameMessages = new string[GameMessagesCount];
            for (int i = 0; i < GameMessagesCount; i++)
            {
                gameMessages[i] = "";
            }
        }

        public void DrawMessages()
        {
            Write(GetMessages(), messagesPosition);
        }
    }
}