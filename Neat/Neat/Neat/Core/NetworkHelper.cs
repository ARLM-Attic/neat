using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
using Microsoft.Xna.Framework.Media;
using Neat;
using Neat.MenuSystem;

#if LIVE
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
#endif

namespace Neat
{
    public delegate void XEventHandler();
#if LIVE
    public class NetworkHelper
    {
        /// VARIABLES
        public NetworkSession Session = null;  //  The game session 
        public int maximumGamers = 8;  
        public int maximumLocalPlayers = 4; 

        public event XEventHandler OnRecieve;
        public event XEventHandler OnSend;
        public event XEventHandler OnCreate;
        public event XEventHandler OnJoin;

        public NeatGame Game; 
        public NetworkHelper(NeatGame g, GameTime t )
        {
            gamestime = t;
            Game = g;
            initializeMessages();
        }

        public virtual void CreateSession()
        {
            if (Session == null)
            {
                Session = NetworkSession.Create(NetworkSessionType.SystemLink,
                maximumLocalPlayers,
                maximumGamers);
            }

            // If the host goes out, another machine will assume as a new host
            Session.AllowHostMigration = true;
            // Allow players to join a game in progress
            Session.AllowJoinInProgress = true;

            Session.GamerJoined += new EventHandler<GamerJoinedEventArgs>(session_GamerJoined);
            Session.GamerLeft += new EventHandler<GamerLeftEventArgs>(session_GamerLeft);
            Session.GameStarted += new EventHandler<GameStartedEventArgs>(session_GameStarted);
            Session.GameEnded += new EventHandler<GameEndedEventArgs>(session_GameEnded);
            Session.SessionEnded += new EventHandler<NetworkSessionEndedEventArgs>(session_SessionEnded);
            Session.HostChanged += new EventHandler<HostChangedEventArgs>(session_HostChanged);

            if (OnCreate != null) OnCreate();
        }

        public virtual void FindSession()
        {
            // Search for sessions.
            using (AvailableNetworkSessionCollection availableSessions =
                        NetworkSession.Find(NetworkSessionType.SystemLink,
                                            maximumLocalPlayers, null))
            {
                if (availableSessions.Count == 0)
                {
                    sayMessage("No network sessions found.");
                    return;
                }
                else
                {
                    sayMessage("Found an available session at host " +
                availableSessions[0].HostGamertag);
                    Session = NetworkSession.Join(availableSessions[0]);
                }
            }

            if (OnJoin != null) OnJoin();
        }

        IAsyncResult AsyncSessionFind = null;
        public virtual void AsyncFindSession()
        {
            sayMessage("Asynchronous search started!");
            if (AsyncSessionFind == null)
            {
                AsyncSessionFind = NetworkSession.BeginFind(
                NetworkSessionType.SystemLink, 1, null,
                new AsyncCallback(session_SessionFound), null);
            }
        }

        public virtual void Update()
        {
            if (Session != null)
                Session.Update();
        }

        // Message regarding the session's current state 
        private String message = "Waiting for user command...";
        public String Message
        {
            get { return message; }
        }

        public virtual void StartGame()
        {
            Session.StartGame();
        }

        public int GetRecieveSpeed()
        {
            if (Session != null) return Session.BytesPerSecondReceived;
            else return 0;
        }
        public int GetSendSpeed()
        {
            if (Session != null) return Session.BytesPerSecondSent;
            else return 0;
        }

        public virtual void CloseSession()
        {
            Session.Dispose();
            Session = null;
        }

        public virtual void EndGame()
        {
            Session.EndGame();
        }

        public virtual void SignInGamer()
        {
#if WINDOWS
            if (!Guide.IsVisible)
            {
                Guide.ShowSignIn(1, false);
            }
#endif
        }
        public virtual void ShowChat()
        {
#if WINDOWS
            if (!Guide.IsVisible)
            {
                Guide.BeginShowKeyboardInput(PlayerIndex.One, "Chat", "Enter your message",
                    "", new AsyncCallback(keyboard_done), null);
            }
#endif
        }
        public virtual void keyboard_done(IAsyncResult r)
        {
#if WINDOWS
            string msg = Guide.EndShowKeyboardInput(r);
            sayMessage(msg);
            SendMessage("T" + msg);
#endif
        }

        public virtual void MessageBox(string title,string text,MessageBoxIcon icon)
        {
#if WINDOWS
            Game.Freezed = true;
           List<string> buttons = new List<string>();
            buttons.Add("OK");
            Guide.BeginShowMessageBox(PlayerIndex.One,title,text,buttons,0,icon,new AsyncCallback(message_done),null);
#endif
        }
        public virtual void message_done(IAsyncResult r)
        {
#if WINDOWS
            Game.Freezed = false;
#endif
        }

        public virtual void SetPlayerReady()
        {
            foreach (LocalNetworkGamer gamer in Session.LocalGamers)
                gamer.IsReady = true;
        }

        public virtual void SetPlayerNotReady()
        {
            foreach (LocalNetworkGamer gamer in Session.LocalGamers)
                gamer.IsReady = false;
        }

        void session_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            if (e.Gamer.IsHost)
            {
                sayMessage( "The Host started the session!");
            }
            else
            {
                sayMessage( "Gamer " + e.Gamer.Tag + " joined the session!");
                
                // Other played joined, start the game!
                StartGame();
            }
        }


        public virtual bool isHost()
        {
            try
            {
                return (Session.IsHost);
            }
            catch
            {
                sayMessage("Error in isHost(). returning false.");
                return false;
            }
        }

        public virtual void session_SessionFound(IAsyncResult result)
        {
            // all sessions found
            AvailableNetworkSessionCollection availableSessions;
            // the session we will join
            AvailableNetworkSession availableSession = null;
            if (AsyncSessionFind.IsCompleted)
            {
                availableSessions = NetworkSession.EndFind(result);
                // Look for a session with available gamer slots
                foreach (AvailableNetworkSession curSession in
                availableSessions)
                {
                    int TotalSessionSlots = curSession.OpenPublicGamerSlots +
                    curSession.OpenPrivateGamerSlots;
                    if (TotalSessionSlots > curSession.CurrentGamerCount)
                        availableSession = curSession;
                }
                // if a session was found, connect to it
                if (availableSession != null)
                {
                    sayMessage( "Found an available session at host" +
                    availableSession.HostGamertag);
                    Session = NetworkSession.Join(availableSession);
                }
                else
                    sayMessage( "No sessions found!");
                //  Reset the session finding result
                AsyncSessionFind = null;
            }
        }

        void session_GamerLeft(object sender, GamerLeftEventArgs e)
        {
            sayMessage( "Gamer " + e.Gamer.Tag + " left the session!");
        }
        void session_GameStarted(object sender, GameStartedEventArgs e)
        {
            sayMessage( "Game Started");
        }
        void session_HostChanged(object sender, HostChangedEventArgs e)
        {
            sayMessage( "Host changed from " + e.OldHost.Tag + " to " + e.NewHost.Tag);
        }
        void session_SessionEnded(object sender, NetworkSessionEndedEventArgs e)
        {
            sayMessage( "The session has ended");
        }
        void session_GameEnded(object sender, GameEndedEventArgs e)
        {
            sayMessage( "Game Over");
        }

        public virtual void JustFindSessions()
        {
            sayMessage("Calling JustFindSessions()");
            AvailableNetworkSessionCollection availableSessions;
            int maximumLocalPlayers = 1;
            availableSessions = NetworkSession.Find(
                NetworkSessionType.SystemLink, maximumLocalPlayers, null);
try
            {
            int sessionIndex = 0;
            AvailableNetworkSession availableSession = availableSessions[sessionIndex];

            
                string HostGamerTag = availableSession.HostGamertag;
                int GamersInSession = availableSession.CurrentGamerCount;
                int OpenPrivateGamerSlots = availableSession.OpenPrivateGamerSlots;
                int OpenPublicGamerSlots = availableSession.OpenPublicGamerSlots;
                string sessionInformation = "Session available from gamertag " + HostGamerTag +
                    "\n" + GamersInSession + " players already in this session. \n" +
                    +OpenPrivateGamerSlots + " open private player slots available. \n" +
                    +OpenPublicGamerSlots + " public player slots available.";
                JustFindSessionsResult = sessionInformation;
            }
            catch
            {
                sayMessage("Error in JustFindSessions()");
            }
        }
        public string JustFindSessionsResult = "Nothing.";

        public virtual NetworkSessionState SessionState
        {
        get
            {
                if (Session == null)
                    return NetworkSessionState.Ended;
                else
                    return Session.SessionState;
            }
        }


        /// Write / Read stuff
        public PacketWriter packetWriter = new PacketWriter();
        public virtual void SendMessage(string key)
        {

            foreach (LocalNetworkGamer localPlayer in Session.LocalGamers)
            {
                packetWriter.Write(key);
                localPlayer.SendData(packetWriter, SendDataOptions.ReliableInOrder);
                //sayMessage( "Sending message: " + key);
                sayMessage(key.Length.ToString() + " bytes sent.");

            }
        }

        public virtual void SendPackets()
        {
            foreach (var localPlayer in Session.LocalGamers)
            {
                localPlayer.SendData(
                    packetWriter,
                    SendDataOptions.ReliableInOrder);
            }
        }

        public virtual void SendMessage()
        {
            if (OnSend != null)
            foreach (LocalNetworkGamer localPlayer in Session.LocalGamers)
            {
                OnSend();
            }
        }
        
        public PacketReader packetReader = new PacketReader();
        public virtual bool ReceiveMessage()
        {
            bool result = false;
            if (OnRecieve != null)
            {
                NetworkGamer remotePlayer;  // The sender of the message
                foreach (LocalNetworkGamer localPlayer in Session.LocalGamers)
                {
                    //  While there is data available for us, keep reading
                    while (localPlayer.IsDataAvailable)
                    {
                        result = true;
                        localPlayer.ReceiveData(packetReader, out remotePlayer);
                        // Ignore input from local players
                        
                        if (!remotePlayer.IsLocal)
                        {
                            OnRecieve();
                        }
                    }
                }
            }
            return result;
        }


        public GameTime gamestime;

        // Helper funcs for debug n messaging
        string[] gameMessages;
        int gameMessagesCount = 2;
        void initializeMessages()
        {
            gameMessages = new string[gameMessagesCount];
            resetMessages();
            sayMessage("Network Initialized.");
        }
        public virtual void sayMessage(string msg)
        {
            Game.Console.WriteLine("NETWORK HELPER: " +msg);
            for (int i = 0; i < gameMessagesCount - 1; i++)
            {
                gameMessages[i] = gameMessages[i + 1];
            }
            gameMessages[gameMessagesCount - 1] = msg;
        }
        public virtual string getMessages()
        {
            string r = "";
            foreach (string m in gameMessages)
            {
                r += m + "\n";
            }
            return r;
        }
        public virtual void resetMessages()
        {
            for (int i = 0; i < gameMessagesCount; i++)
            {
                gameMessages[i] = "";
            }
        }
    }
#endif
}
