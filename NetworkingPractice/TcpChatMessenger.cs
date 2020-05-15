



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
                if (!_isDisconnected(_client))
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
                Console.WriteLine($"Wasn't able to connect to the server at {endPoint}");
            }
        }


        public void SendMessages()
        {
            bool wasRunning = Running;

            while (Running)
            {
                // Poll user for input
                Console.Write($"{Name}>");
                string msg = Console.ReadLine();

                //Qut or send a message 
                if ((msg.ToLower() == "quit") || (msg.ToLower() == "exit"))
                {
                    // User wants to quit
                    Console.WriteLine("Diconnectiong...");
                    Running = false;
                }
                else if (msg != string.Empty)
                {
                    // Send the message
                    byte[] msgBuffer = Encoding.UTF8.GetBytes(msg);
                    _msgStream.Write(msgBuffer, 0, msgBuffer.Length);
                }

                // use less CPU
                Thread.Sleep(10);

                // Check if the server didnt disconnect us
                if (_isDisconnected(_client))
                {
                    Running = false;
                    Console.WriteLine("Server has disconnected from us \n:[");
                }
            }

            _cleanupNetworkResources();
            if (wasRunning)
                Console.WriteLine("Disconnected");
        }

        // Cleans any leftowver network resources
        private void _cleanupNetworkResources()
        {
            _msgStream?.Close();
            _msgStream = null;
            _client.Close();
        }

        //Checks if socket is disconnected
        // Adapted from...
        private bool _isDisconnected(TcpClient client)
        {
            try
            {
                Socket s = client.Client;
                return s.Poll(10 * 1000, SelectMode.SelectRead) && (s.Available == 0);
            }
            catch(SocketException se)
            {
                // We got a socket error, assume its disconnected
                return true;
            }

        }




        public static void StartMessenger(string args)
        {
            // Get a name
            Console.Write("Enter a name to use: ");
            string name = Console.ReadLine();

            // Setup the Messenger
            string host = "localhost"; //args[0].Trim()"
            int port = 6000; // int.Parse(args[1].Trim());
            TcpChatMessenger messenger = new TcpChatMessenger(host, port, name);

            // connect and send messages
            messenger.Connect();
            messenger.SendMessages();
        }
     
    }
}
