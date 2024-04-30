using System.Net.Sockets;

namespace BackgammonNet.Lobby
{
    // A class representing the user connected to the server.

    public class ServerClient
    {
        public string clientName;
        public TcpClient tcp;
        public bool isHost;

        public ServerClient(TcpClient tcp) => this.tcp = tcp;
    }
}