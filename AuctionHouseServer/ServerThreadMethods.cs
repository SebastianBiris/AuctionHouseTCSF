using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace AuctionHouseServer
{
    class ServerThreadMethods
    {
        private Socket socketToTheClient;
        private ServerMonitor monitor;

        public ServerThreadMethods(Socket socketToTheClient,ServerMonitor monitor)
        {
            this.socketToTheClient = socketToTheClient;
            this.monitor = monitor;
        }
        public void HandleClient()
        {
            string bid ;
            NetworkStream networkStream;
            StreamReader streamreader;
            StreamWriter streamwriter;
            string clientIpAdressString;

            networkStream = new NetworkStream(this.socketToTheClient);
            streamreader = new StreamReader(networkStream);
            streamwriter = new StreamWriter(networkStream);

            IPAddress clientIpAdress = ((IPEndPoint)this.socketToTheClient.RemoteEndPoint).Address;
            clientIpAdressString = clientIpAdress.ToString();
            IPHostEntry ip = Dns.GetHostEntry(clientIpAdressString);
            Thread.CurrentThread.Name = ip.HostName;

            this.monitor.AddClients(streamwriter);
           // this.monitor.BroadcastBid(clientIpAdressString, "joined");

            while (true)
            {
                bid = streamreader.ReadLine();
                if (bid == null)
                {
                    break;
                }
                monitor.BroadcastBid(clientIpAdressString, bid);
            }

            monitor.RemoveClients(streamwriter);
          //  monitor.BroadcastBid(clientIpAdressString, "Logged out");
            streamwriter.Close();
            streamreader.Close();
            networkStream.Close();


        }
        
    }
}
