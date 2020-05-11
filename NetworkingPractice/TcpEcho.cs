using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace NetworkingPractice
{
    class TcpEcho
    {
        public static void Run()
        {

            Thread serverThread = new Thread((ThreadStart)delegate
            {
                TcpListener listener = new TcpListener(IPAddress.Loopback, 1234);

                listener.Start();

                using (TcpClient client = listener.AcceptTcpClient())

                using (NetworkStream stream = client.GetStream())

                using (StreamReader reader = new StreamReader(stream)
    )
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.AutoFlush = true;

                    string inputLine;

                    while ((inputLine = reader.ReadLine()) != null)
                    {
                        writer.WriteLine($"Echoing string: {inputLine}");
                    }

                    Console.WriteLine("Server saw disconnect from client");

                }

            });

            serverThread.Start();

            string[] linesToSend = new string[] { "foo", "bar", "ack" };

            using (TcpClient client = new TcpClient("127.0.0.1", 1234))

            using (NetworkStream stream = client.GetStream())

            using (StreamReader reader = new StreamReader(stream))

            using (StreamWriter writer = new StreamWriter(stream))

            {
                writer.AutoFlush = true;
                
                foreach (string lineToSend in linesToSend)
                {
                    Console.WriteLine($"Sending to server: {lineToSend}");

                    writer.WriteLine(lineToSend);

                    string lineWeRead = reader.ReadLine();

                    Console.WriteLine($"Recieved from server {lineWeRead}");

                    Thread.Sleep(2000); // just for effect

                }

                Console.WriteLine("Client is disconnecting from server");
            }

            // wait for the server to exix before Main
            // this way, we make sure to keep Console
            // around since the server writes to it

            serverThread.Join();


        }
    }
}
