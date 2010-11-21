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
 

namespace Neat
{
    //TODO: Uncomment whenever LIVE can actually be used.
    public delegate void XEventHandler();
#if LIVE && TODO
    public class NetworkHelper
    {
        /// VARIABLES
        public NetworkSession session = null;  //  The game session 
        public int maximumGamers = 8;  // Only 2 will play 
        public int maximumLocalPlayers = 4;  //  no split-screen, only remote players

        public event XEventHandler OnRecieve;
        public event XEventHandler OnSend;
        public event XEventHandler OnCreate;
        public event XEventHandler OnJoin;

        public NeatGame parent; 
        public NetworkHelper(NeatGame g, GameTime t )
        {
            gamestime = t;
            parent = g;
            initializeMessages();
        }

        /// <summary>
        /// CREATE/JOIN METHODS
        /// </summary>
        public virtual void CreateSession()
        {
            if (session == null)
            {
                session = NetworkSession.Create(NetworkSessionType.SystemLink,
                maximumLocalPlayers,
                maximumGamers);
            }

            // If the host goes out, another machine will assume as a new host
            session.AllowHostMigration = true;
            // Allow players to join a game in progress
            session.AllowJoinInProgress = true;

            #region BindEvents
            session.GamerJoined +=
            new EventHandler<GamerJoinedEventArgs>(session_GamerJoined);
            session.GamerLeft +=
            new EventHandler<GamerLeftEventArgs>(session_GamerLeft);
            session.GameStarted +=
            new EventHandler<GameStartedEventArgs>(session_GameStarted);
            session.GameEnded +=
            new EventHandler<GameEndedEventArgs>(session_GameEnded);
            session.SessionEnded +=
            new EventHandler<NetworkSessionEndedEventArgs>(session_SessionEnded);
            session.HostChanged +=
            new EventHandler<HostChangedEventArgs>(session_HostChanged);
            #endregion

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
                    session = NetworkSession.Join(availableSessions[0]);
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
            if (session != null)
                session.Update();
        }

        // Message regarding the session's current state 
        private String message = "Waiting for user command...";
        public String Message
        {
            get { return message; }
        }

        public virtual void StartGame()
        {
            session.StartGame();
        }

        public int GetRecieveSpeed()
        {
            if (session != null) return session.BytesPerSecondReceived;
            else return 0;
        }
        public int GetSendSpeed()
        {
            if (session != null) return session.BytesPerSecondSent;
            else return 0;
        }

        public virtual void CloseSession()
        {
            session.Dispose();
            session = null;
        }

        public virtual void EndGame()
        {
            session.EndGame();
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
            parent.freezed = true;
           List<string> buttons = new List<string>();
            buttons.Add("OK");
            Guide.BeginShowMessageBox(PlayerIndex.One,title,text,buttons,0,icon,new AsyncCallback(message_done),null);
#endif
        }
        public virtual void message_done(IAsyncResult r)
        {
#if WINDOWS
            parent.freezed = false;
#endif
        }

        public virtual void SetPlayerReady()
        {
            foreach (LocalNetworkGamer gamer in session.LocalGamers)
                gamer.IsReady = true;
        }

        public virtual void SetPlayerNotReady()
        {
            foreach (LocalNetworkGamer gamer in session.LocalGamers)
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
                return (session.IsHost);
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
                    session = NetworkSession.Join(availableSession);
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
                if (session == null)
                    return NetworkSessionState.Ended;
                else
                    return session.SessionState;
            }
        }


        /// Write / Read stuff
        public PacketWriter packetWriter = new PacketWriter();
        public virtual void SendMessage(string key)
        {

            foreach (LocalNetworkGamer localPlayer in session.LocalGamers)
            {
                packetWriter.Write(key);
                localPlayer.SendData(packetWriter, SendDataOptions.ReliableInOrder);
                //sayMessage( "Sending message: " + key);
                sayMessage(key.Length.ToString() + " bytes sent.");

            }
        }

        public virtual void SendPackets()
        {
            foreach (var localPlayer in session.LocalGamers)
            {
                localPlayer.SendData(
                    packetWriter,
                    SendDataOptions.ReliableInOrder);
            }
        }

        public virtual void SendMessage()
        {
            if (OnSend != null)
            foreach (LocalNetworkGamer localPlayer in session.LocalGamers)
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
                foreach (LocalNetworkGamer localPlayer in session.LocalGamers)
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
            parent.console.WriteLine("NETWORK HELPER: " +msg);
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
