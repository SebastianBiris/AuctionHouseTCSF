using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace AuctionHouseServer
{
    class ServerMain
    {
        
        public event ResetGavelDelegate ResetGavelEvent;
        private const int PORT = 5000;
        static void Main(string[] args)
        {
            
            IPEndPoint port;
            Thread clientThread;
            ServerThreadMethods serverThreadMethod;
            Socket socketToTheClient;
            ServerMonitor monitor;

            port = new IPEndPoint(IPAddress.Any, PORT);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(port);
            listener.Listen(10);

            monitor = new ServerMonitor();

            while (true)
            {
                Console.WriteLine("Waiting for a client .....");
                socketToTheClient = listener.Accept();
                Console.WriteLine("New Client Joined "+Thread.CurrentThread.Name);
                serverThreadMethod = new ServerThreadMethods(socketToTheClient, monitor);
                clientThread = new Thread(new ThreadStart(serverThreadMethod.HandleClient));
                clientThread.Start();

            }
        }
    }
}
