using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetworkingPractice
{
    class TcpChatServer
    {

        // What listens in 
        private TcpListener _listener;

        // Types of clients connected
        private List<TcpClient> _viewers = new List<TcpClient>();
        private List<TcpClient> _messengers = new List<TcpClient>();

        // Names that are taken by other messengers
        private Dictionary<TcpClient, string> _names = new Dictionary<TcpClient, string>();

        private Queue<string> _messageQueue = new Queue<string>();

        // Extra fun data
        public readonly string ChatName;
        public readonly int Port;
        public bool Running { get; private set; }

        // Buffer 
        public readonly int BufferSize = 2 * 1024; //KB

        public TcpChatServer(string chatName, int port)
        {
            // Set the basic data
            ChatName = chatName;
            Port = port;
            Running = false;

            // make listener listen for connections on any network device
            _listener = new TcpListener(IPAddress.Any, Port);
        }

        // If the server is running, this will shutdown the server

        public void Shutdown()
        {
            Running = false;
            Console.WriteLine("Shutting downt he server");
        }

        //  Start runnign the server. Will; stop when Shutdown() has been called

        public void Run()
        {
            // some info
            Console.WriteLine($"Starting the \"{ChatName} TCP Chat Server\"  on port {Port}");
            Console.WriteLine("Press Ctrl-C to shutdown the server at any time.");

            // Make the server run
            _listener.Start();
            Running = true;

            // Main server loop
            while (Running)
            {
                // Check for new clients
                if (_listener.Pending())
                    _handleNewConnection();

                // Do the rest
                _checkForDisconnects();
                _checkForNewMessages();
                _sendMessages();

                // Use less CPU
                Thread.Sleep(10);
            }

            foreach (TcpClient v in _viewers)
                _cleanupClient(v);
            foreach (var  m in _messengers)
                _cleanupClient(m);
            _listener.Stop();

            //Some info 
            Console.WriteLine("Sever is shutdown");
            
        }

        private void _handleNewConnection()
        {
            // There is (at least) one, see what they want
            bool good = false;
            TcpClient newClient = _listener.AcceptTcpClient(); // Blocks
            NetworkStream netStream = newClient.GetStream();

            // Modify the default buffer sizes
            newClient.SendBufferSize = BufferSize;
            newClient.ReceiveBufferSize = BufferSize;

            // Print some info
            EndPoint endPoint = newClient.Client.RemoteEndPoint;
            Console.WriteLine($"Handling a new client from {endPoint}...");

            // Let them identify themselves
            byte[] msgBuffer = new byte[BufferSize];
            int bytesRead = netStream.Read(msgBuffer, 0, msgBuffer.Length); // Blocks
            // Console.WriteLine($"Got {bytesRead} bytes.");
            if (bytesRead > 0)
            {
                string msg = Encoding.UTF8.GetString(msgBuffer, 0, bytesRead);

                if(msg == "viewer")
                {
                    // they just want to watch
                    good = true;
                    _viewers.Add(newClient);
                }
                else if (msg.StartsWith("name:"))
                {
                    string name = msg.Substring(msg.IndexOf (':') + 1);
                }

                if((name != string.Empty) && (!_names.ContainsValue(name)))
                {
                    good = true;
                    _names.Add(newClient, name);
                    
                }
            }
        }

        private void _cleanupClient(TcpClient v)
        {
            throw new NotImplementedException();
        }

        private void _sendMessages()
        {
            throw new NotImplementedException();
        }

        private void _checkForNewMessages()
        {
            throw new NotImplementedException();
        }

        private void _checkForDisconnects()
        {
            throw new NotImplementedException();
        }

    
    }
}
