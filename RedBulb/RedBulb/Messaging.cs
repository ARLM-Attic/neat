#region References
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using RedBulb;
using RedBulb.MenuSystem;
//using UltimaScroll.IBLib;
#endregion
namespace RedBulb
{
    public partial class RedBulbGame : Microsoft.Xna.Framework.Game
    {
        GameTime gamestime;
        // Helper funcs for debug n messaging
        string[] gameMessages;
        public int gameMessagesCount = 7;

        public Vector2 messagesPosition = Vector2.Zero;

        void InitializeMessages()
        {
            ResetMessages();
        }

        public void SayMessage(string msg)
        {
            try
            {
                msg = gamestime.TotalGameTime.Hours.ToString() + ":" +
                    gamestime.TotalGameTime.Minutes.ToString() + ":" +
                    gamestime.TotalGameTime.Seconds.ToString() + "," +
                    gamestime.TotalGameTime.Milliseconds.ToString() + " ->" +
                    msg;
            }
            catch
            { 
                // :-) Do nothing.
            }
            for (int i = 0; i < gameMessagesCount - 1; i++)
            {
                gameMessages[i] = gameMessages[i + 1];
            }
            gameMessages[gameMessagesCount - 1] = msg;
        }

        public string GetMessages()
        {
            string r = "";
            foreach (string m in gameMessages)
            {
                r += m + "\n";
            }
            return r;
        }

        public void ResetMessages()
        {
            gameMessages = new string[gameMessagesCount];
            for (int i = 0; i < gameMessagesCount; i++)
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