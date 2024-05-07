using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using BackgammonNet.Core;

namespace BackgammonNet.Lobby
{
    // A class representing the web client of the mutliplayer Backgammon game.
    // Here there are class method calls specific to the game scene.

    public class Client : MonoBehaviour
    {
        public static event Action<string> OnNewPlayer = delegate { };
        public static event Action<string> OnRemovePlayer = delegate { };

        public List<GameClient> players;                            // List of users (players) connected to the server.
        public string clientName;
        public bool isHost;                                         // Whether the client is local or remote.

        private bool socketReady;
        private TcpClient tcpClient;
        private NetworkStream networkStream;                        // A network stream that exists between the client and the server.
        private StreamWriter streamWriter;
        private StreamReader streamReader;

        public static bool isSobsrvMessage;
        private bool onceOnly;

        private void Start() => players = new List<GameClient>();
        
        //---- attempting to connect to the server ----------

        public bool ConnectToServer(string hostIP, int port)       // Called in the LobbyManager right after creating the remote client.
        {
            try
            {
                tcpClient = new TcpClient(hostIP, port);
                networkStream = tcpClient.GetStream();
                
                streamWriter = new StreamWriter(networkStream);
                streamReader = new StreamReader(networkStream);

                socketReady = true;                                 // Start listening for messages from the server.
            }

            catch (Exception e)
            {
                LobbyManager.monitContent = "Socket error: " + e.Message;
                return false;
            }

            return socketReady;
        }
        
        public bool ConnectToServerTest(string hostIP, int port)    // Used in test procedures: host detection.
        {
            try
            {
                TcpClient tcpTest = new TcpClient(hostIP, port);
            }

            catch (Exception e)
            {
                Debug.Log("Socket error: " + e.Message);
                return false;
            }

            return true;
        }

        //---- listening for server messages (commands) -------------------------------

        private void Update()                                       // Listening for incoming messages from the server when opening a socket.
        {
            if (socketReady)
            {
                if (networkStream.DataAvailable)
                {
                    string data = streamReader.ReadLine();

                    if (data != null)
                        OnIncomingData(data);
                }
            }
        }

        private bool IsUnicalName(string[] names, int start)        // Verification the uniqueness of the name.
        {
            bool unical = true;

            for (int i = start; i < names.Length; i++)
                if (names[i] == clientName)
                    unical = false;

            return unical;
        }

        private void OnIncomingData(string data)                    // Reading messages sent by the server.
        {
            string[] aData = data.Split('|');

            switch (aData[0])
            {
                case "SIPA":
                    if (isHost)
                    {
                        string[] names = new string[(aData.Length - 1) / 2];

                        if (aData[1] != "")
                            for (int i = 1; i < aData.Length - 1; i += 2)
                                if (aData[i] != "")
                                {
                                    string[] adat = aData[i + 1].Split(' ');
                                    names[(i - 1) / 2] = adat[0];
                                }
                        
                        if (IsUnicalName(names, 0))    // Check the uniqueness of the hostnames present on the master server.
                        {
                            Send("CIPA|" + LobbyManager.Instance.ourIpAddress + "|" + LobbyManager.clientName); // forward host address
                            LobbyManager.Instance.CreateHost();                                                 // creating the host
                            LobbyManager.Instance.SwitchMenuView(false, false, true, false, false);
                        }
                        else
                        {
                            LobbyManager.monitContent = "Choose another name (a host with that name already exists)!";
                            LobbyManager.Instance.GoToMainMenu();
                        }
                    }
                    else
                    {                               // we go through the successive addresses of the hosts received from the main server
                        if (aData[1] != "")
                            for (int i = 1; i < aData.Length - 1; i += 2)
                                if (aData[i] != "")
                                    LobbyManager.Instance.AddHost(aData[i], aData[i+1]);    // display host names on scroll view
                    }                    
                    break;

                //-----------------------------------------------------------------------------------------------------------

                case "SWHO":                                        // When the server allows our client to connect to itself, 
                    if (IsUnicalName(aData, 1))
                    {
                        if (aData[1] != "")                         // it transmits a list of currently connected clients.
                            for (int i = 1; i < aData.Length; i++)
                                if (aData[i] != "")
                                {
                                    Debug.Log("SWHO(" + i + "): " + aData[i]);
                                    UserConnected(aData[i]);                    // These clients are other clients.
                                }

                        Send("CWHO|" + clientName + '|' + (isHost ? 1 : 0).ToString());     // CWHO (Client Who): We send the name of our client to the server.
                    }
                    else
                    {
                        LobbyManager.monitContent = "Choose another name (a player with that name already exists)!";
                        LobbyManager.Instance.GoToMainMenu();
                    }
                    break;

                case "SCONN":                                       // The server informs about a new connected remote client.
                    Debug.Log("SCONN: " + aData[1]);
                    UserConnected(aData[1]);            // here even our name is transferred
                    break;

                case "SOBSRV":
                    if (!onceOnly)
                    {
                        if (aData[1] != "")
                        {
                            onceOnly = true;
                            isSobsrvMessage = true;

                            TimeController.Instance.timeLapse[0] = int.Parse(aData[1]);
                            TimeController.Instance.timeLapse[1] = int.Parse(aData[2]);

                            for (int i = 3; i < 5; i++)                 // stones in the shelter (the first two indexes in aData)
                                if (int.Parse(aData[i]) > 0)
                                {
                                    Board.Instance.PlaceInShelter(int.Parse(aData[i]), i - 3);
                                    Pawn.shelterSide[i - 3] = true;
                                }

                            for (int i = 5; i < aData.Length - 2; i += 3)   // stones on slots (including in a prison)
                                Board.Instance.CreateSlotPawns(int.Parse(aData[i]), int.Parse(aData[i + 1]), int.Parse(aData[i + 2]));
                        }
                    }
                    break;

                case "SOUT":
                    if (aData[1] != "")                             // it transmits a list of currently connected clients.
                        for (int i = 1; i < aData.Length; i++)
                            if (aData[i] != "")
                            {
                                OnRemovePlayer?.Invoke(aData[i]);
                                GameClient player = players.Find(p => p.name == aData[i]);
                                players.Remove(player);                                     // remove disconnected player
                                Board.Instance.ShowText("Removed player: " + player.name);
                            }
                    break;

                case "SACCPT":
                    Board.Instance.acceptance++;        // confirmation of the opponent's willingness to start the game
                    break;

                case "SDCS":
                    if (!Board.Instance.observer)
                        GameController.Instance.CheckIfTurnChange(int.Parse(aData[1]), int.Parse(aData[2]));
                    else if (isSobsrvMessage)
                        GameController.Instance.CheckIfTurnChange(int.Parse(aData[1]), int.Parse(aData[2]));
                    break;

                case "SMOV":                                        // SMOV (Server Move): Information about the player's movement.
                    if (!Board.Instance.observer)
                        Board.Instance.OpponentTryMove(int.Parse(aData[1]), int.Parse(aData[2]), bool.Parse(aData[3]), float.Parse(aData[4]));  // Make your opponent move.
                    else if (isSobsrvMessage)
                        Board.Instance.OpponentTryMove(int.Parse(aData[1]), int.Parse(aData[2]), bool.Parse(aData[3]), float.Parse(aData[4]));  // Make your opponent move.
                    break;
                    
                case "SNG":                                         // Whether the opponent pressed the "New Game" button.
                    GameController.Instance.sidesAgreed++;
                    if (isHost)                                     // Information about pressing the NewGame button by the opponent.
                        Board.Instance.ShowText("New Game Request");
                    break;
                    
                case "SBTN":
                    GameController.Instance.ActivateButtons(true);
                    break;

                case "SRMV":
                    if (clientName == aData[1])                     // you have been kicked from the game
                    {
                        LobbyManager.monitContent = "You have been kicked from the game table!";
                        GameController.Instance.GoToMainMenu();
                    }
                    break;
            }
        }

        private void UserConnected(string name)                     // Add the newly connected player to the player list.
        {
            GameClient c = new GameClient();
            c.name = name;

            players.Add(c);

            if (name != clientName)                                 // We don't put ourselves on the players list.
                OnNewPlayer?.Invoke(name);

            if (players.Count == 2)
                LobbyManager.Instance.StartGame();                  // Load a scene with a game of Backgammon.
        }

        //---- Data transfer from client to server.

        public bool Send(string data)
        {
            if (!socketReady)
                return false;

            try
            {
                streamWriter.WriteLine(data);
                streamWriter.Flush();
            }

            catch (Exception e)
            {
                Debug.Log("Socket error: " + e.Message);
                return false;
            }

            return true;
        }

        //---- closing a socket (connection) ------------------------------

        private void OnDisable() => CloseSocket();

        private void CloseSocket()
        {
            if (!socketReady)
                return;

            streamWriter.Close();
            streamReader.Close();
            tcpClient.Close();

            socketReady = false;
        }
    }
}