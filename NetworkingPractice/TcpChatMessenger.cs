



using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetworkingPractice
{
    class TcpChatMessenger
    {
        // Connection objects
        public readonly string ServerAddress;
        public readonly int Port;
        private TcpClient _client;
        public bool Running { get; private set; }

        // buffer & messaging
        public readonly int BufferSize = 2 * 1024; // 2 KB
        private NetworkStream _msgStream = null;

        // Personal data
        public readonly string Name;

        public TcpChatMessenger(string serverAddress, int port, string name)
        {
            // Create a non-connected TcpClient
            _client = new TcpClient();
            _client.SendBufferSize = BufferSize;
            Running = false;

            // Set the other things
            ServerAddress = serverAddress;
            Port = port;
            Name = name;
        }

        public void Connect()
        {
            // Try to connect
            _client.Connect(ServerAddress, Port);   //Will resolve DNS for us; blocks
            EndPoint endPoint = _client.Client.RemoteEndPoint;

            // Make sure we're connected 
            if (_client.Connected)
            {
                // Got in!
                Console.WriteLine($"Connected to the server at {endPoint}.");

                // Tell them we're a messenger
                _msgStream = _client.GetStream();
                byte[] msgBuffer = Encoding.UTF8.GetBytes(String.Format($"Name {Name}"));
                _msgStream.Write(msgBuffer, 0, msgBuffer.Length); // Blocks

                // If we're still connceted after sending our name, that means the server accepts us
                if (!isDisconnected(_client))
                    Running = true;
                else
                {
                    // name was probably taken...
                    _cleanupNetworkResources();
                    Console.WriteLine($"The server rejected us \"{Name}\" is probably in use");
                }
            }
            else
            {
                _cleanupNetworkResources();
                Console.WriteLine($"Wasn't able to connect to the server at {EndPoint}");
            }
        }


        public void SendMessages()
        {

        }
        private void _cleanupNetworkResources()
        {
            throw new NotImplementedException();
        }

        private bool isDisconnected(TcpClient client)
        {
            throw new NotImplementedException();
        }
    }
}
