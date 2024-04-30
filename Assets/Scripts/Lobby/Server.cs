using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using BackgammonNet.Core;
using System.Collections;

namespace BackgammonNet.Lobby
{
    // A class representing the functionality of a server dedicated to handling the multiplayer Backgammon game.
    // We program at the lowest level possible in the Unity environment, using the .NET Framework network classes (lower than LLAPI).
    // The server listens for incoming messages from users(clients) connected to it with each game frame.

    public class Server : MonoBehaviour
    {
        public int portNo = 6321;                           // A port from the public pool.

        private List<ServerClient> clients;                 // All clients connected to the server as players.
        private List<ServerClient> disconnectList;

        private TcpListener server;                         // A reference to the server instance.
        private bool serverStarted;

        private ServerClient transmitterClient;             // the client from which the currently received message comes from the server
        private List<ServerClient> observerClients;         // Clients waiting to restore the game.

        private bool isMainServer;

        //---- TCP server creation -------------------------------

        public void Init(bool isMainServer)
        {
            this.isMainServer = isMainServer;
            clients = new List<ServerClient>();
            disconnectList = new List<ServerClient>();
            observerClients = new List<ServerClient>();

            try
            {
                server = new TcpListener(IPAddress.Any, portNo);    // Everyone will be able to connect to our server.
                server.Start();                                     // We start the server work. From this point on, we should start listening for client connection requests.

                serverStarted = true;                               // Initiate a listening procedure for messages from connected clients.
                StartListening();                                   // Initiate the acceptance procedure for connecting the client to the server.
            }

            catch (Exception e)
            {
                Debug.Log("Socket error: " + e.Message);
            }
        }

        private void StartListening()
        {
            server.BeginAcceptTcpClient(AcceptTcpClient, server);   // After the end of the Handshake procedure, the customer is accepted.
        }

        private void AcceptTcpClient(IAsyncResult asyncResult)      // Method of handling the event of accepting a new client.
        {
            TcpListener listener = (TcpListener)asyncResult.AsyncState;
            ServerClient serverClient = new ServerClient(listener.EndAcceptTcpClient(asyncResult));
            serverClient.clientName = "";
            string allUsers = "";                                   // All clients so far connected to the host.

            if (isMainServer)
            {
                foreach (ServerClient client in clients)
                    allUsers += client.clientName + '|';

                clients.Add(serverClient);
                StartListening();                                   // We resume listening on the port.

                string allInfo = "";                                // All hosts so far connected to the main server.
                foreach (GameObject host in LobbyManager.Instance.hosts)
                {
                    string info = host.GetComponentInChildren<Text>().text;
                    allInfo += host.name + '|' + info + '|';
                }

                Broadcast("SIPA|" + allInfo, serverClient);         // The server asks the client for its IP address.
            }
            else
            {                
                int count = clients.Count;
                for (int i = 0; i < count; i++)
                    if (clients[count - 1 - i].clientName == "")
                        clients.Remove(clients[count - 1 - i]);     // Removing unnamed clients (those coming from the main server).

                foreach (ServerClient client in clients)
                    allUsers += client.clientName + '|';

                clients.Add(serverClient);
                StartListening();                                   // We resume listening on the port.

                Debug.Log("Somebody has connected!");               // SWHO (Server Who): The server asks the client for its name.
                Broadcast("SWHO|" + allUsers, serverClient);        // We send data to the last connected client.
            }
        }

        private void Broadcast(string data, ServerClient c)         // Sending from the server to one client.
        {
            List<ServerClient> serverClient = new List<ServerClient>() { c };
            Broadcast(data, serverClient);
        }

        private void Broadcast(string data, List<ServerClient> cl)              // Sending from the server to clients.
        {
            string[] aData = data.Split('|');

            foreach (ServerClient serverClient in cl)
            {
                if (aData[0] == "SWHO" || aData[0] == "SCONN" || aData[0] == "SOUT" || aData[0] == "SBTN" ||
                    aData[0] == "SIPA" || aData[0] == "SAMNT" || aData[0] == "SOBSRV" || aData[0] == "SRMV")  // to all clients
                {
                    Send(serverClient, data);
                }
                else
                {
                    if (serverClient != transmitterClient)    // Sending a message to customers, omitting the sender of this message.
                        Send(serverClient, data);
                }
            }
        }

        private void Send(ServerClient serverClient, string data)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(serverClient.tcp.GetStream());
                streamWriter.WriteLine(data);
                streamWriter.Flush();
            }

            catch (Exception e)
            {
                Debug.Log("Write error: " + e.Message);
            }
        }

        //---- listening for customer messages ------------------------

        private void Update()
        {
            if (!serverStarted)
                return;

            for (int i = 0; i < clients.Count; i++)             // We check the status of clients added to the list of clients.
            {                                                   // Is the user still connected?
                if (!IsConnected(clients[i].tcp))
                {
                    clients[i].tcp.Close();
                    disconnectList.Add(clients[i]);
                    continue;
                }
                else
                {
                    NetworkStream clientStream = clients[i].tcp.GetStream();

                    if (clientStream.DataAvailable)
                    {
                        StreamReader reader = new StreamReader(clientStream, true);
                        string data = reader.ReadLine();

                        if (data != null)
                            OnIncomingData(clients[i], data);
                    }
                }
            }

            for (int i = 0; i < disconnectList.Count; i++)
                clients.Remove(disconnectList[i]);

            if (disconnectList.Count > 0)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    string data = "";

                    for (int j = 0; j < disconnectList.Count; j++)
                        data += disconnectList[j].clientName + '|';

                    Broadcast("SOUT|" + data, clients[i]);                      // Inform our user that someone has disconnected.
                }

                disconnectList = new List<ServerClient>();

                if (clients.Count > 1)
                {
                    for (int i = 1; i < clients.Count; i++)
                    {
                        if (clients[i].clientName != "")
                        {
                            Debug.Log("SBTN: " + clients[i].clientName);
                            Broadcast("SBTN|", clients[i]);                     // Let the next customer on the list unlock the "New Game" button.
                            break;
                        }
                    }
                }
            }
        }

        private bool IsConnected(TcpClient tcpClient)                           // Checking if a given TCP client is connected.
        {
            try
            {
                if (tcpClient != null && tcpClient.Client != null && tcpClient.Client.Connected)
                {
                    if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                        return !(tcpClient.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                    return true;
                }
                else
                    return false;
            }

            catch
            {
                return false;
            }
        }

        private void OnIncomingData(ServerClient serverClient, string data)     // Reading the messages sent by the given client.
        {
            transmitterClient = serverClient;
            string[] aData = data.Split('|');

            switch (aData[0])
            {
                case "CIPA":                                                        // only sent by the host
                    LobbyManager.Instance.AddHost(aData[1], aData[2]);              // put the host names on the list of hosts shared with remote clients
                    LobbyManager.Instance.UpdateHost(aData[1], "1");    // to show the number of players from the beginning
                    break;
                
                case "CAMNT":                                                       // sent in LobbyManager in the InformMainServer method
                    LobbyManager.Instance.UpdateHost(aData[1], aData[2]);           // updates the host list with the number of sitting players
                    break;
                
                //-----------------------------------------------------------------------------------------------------------

                case "CWHO":                                                        // Client Who - The client sends his name.
                    serverClient.clientName = aData[1];
                    serverClient.isHost = (aData[2] == "0") ? false : true;
                    Broadcast("SCONN|" + serverClient.clientName, clients);         // SCONN - Server Connection.
                    break;                                                          // We inform all clients about the new client.

                case "CACCPT":
                    Broadcast("SACCPT|acceptance", clients);                        // confirmation of your willingness to start the game
                    break;

                case "CDCS":
                    Broadcast("SDCS|" + aData[1] + "|" + aData[2], clients);        // The values ​​rolled on the opponent's dice.
                    break;

                case "CMOV":
                    Broadcast("SMOV|" + aData[1] + "|" + aData[2] + "|" + aData[3] + "|" + aData[4], clients);    // the move of the opponent
                    break;

                case "CNG":
                    Broadcast("SNG|" + aData[1], clients);                          // the opponent confirmed his willingness to continue the game
                    break;

                case "CRMV":
                    Broadcast("SRMV|" + aData[1], clients);
                    break;

                case "COBSRV":                                                      // a received request from an observer to submit a save of the game
                    observerClients.Add(serverClient);
                    if(observerClients.Count == 1)
                        StartCoroutine(WaitForHostTurn());
                    break;
            }
        }

        private IEnumerator WaitForHostTurn()
        {
            while (true)
            {
                yield return null;

                if (GameController.Instance.newTurn && GameController.turn == 0)    // only when the host takes the turn
                {
                    GameController.Instance.newTurn = false;
                    
                    string info = "";

                    //---- time synchronization

                    info += (int)TimeController.Instance.timeLapse[0] + "|";
                    info += (int)TimeController.Instance.timeLapse[1] + "|";

                    //---- the amount of stones in the shelters

                    GameObject go = GameObject.Find("White House");

                    for (int i = 0; i < go.transform.childCount; i++)
                        if (go.transform.GetChild(i).gameObject.activeSelf == false)
                        {
                            info += i + "|";
                            break;
                        }

                    go = GameObject.Find("Red House");

                    for (int i = 0; i < go.transform.childCount; i++)
                        if (go.transform.GetChild(i).gameObject.activeSelf == false)
                        {
                            info += i + "|";
                            break;
                        }

                    //---- go through all slots (including the prison)

                    for (int i = 0; i < Slot.slots.Count; i++)
                    {
                        int height = Slot.slots[i].Height();

                        if (height > 0)
                        {
                            int color = Slot.slots[i].IsWhite();
                            info += i + "|" + height + "|" + color + "|";
                        }
                    }

                    Broadcast("SOBSRV|" + info, observerClients);

                    yield return null;
                    observerClients = new List<ServerClient>();
                    yield break;
                }
            }
        }

        public void Stop() => server.Stop();        // server deactivation
    }
}