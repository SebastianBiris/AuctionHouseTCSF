using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;


namespace AuctionHouseServer
{
    class ServerThreadMethods
    {
        private Socket socketToTheClient;
        private ServerMonitor monitor;
      

        


        public ServerThreadMethods(Socket socketToTheClient, ServerMonitor monitor)
        {
            this.socketToTheClient = socketToTheClient;
            this.monitor = monitor;
        }
        public void startGavel(TimeSpan timeNow)
        {
              string state1 = "First";
              string state2 = "Second";
              string state3 = "Third. Sold to " + Thread.CurrentThread.Name;
            Thread.Sleep(3000);
            if (timeNow == bidTime)
            {
                monitor.BroadcastGavel(state1);
                Thread.Sleep(3000);
                if (timeNow == bidTime)
                {
                    monitor.BroadcastGavel(state2);

                    Thread.Sleep(3000);
                    if (timeNow == bidTime)
                    { monitor.BroadcastGavel(state3); }
                    else
                    { bidTime = timeNow; }
                }
            }
           
         
        }

        public void HandleClient()
        {
            Thread gavelThread;
            string bid;
            NetworkStream networkStream;
            StreamReader streamreader;
            StreamWriter streamwriter;
            string clientIpAdressString;
            string clientName;
            

            //List<Item> items = new List<Item>();
            Item tv = new Item("TV Flatscreen", 1200, 1200);
            //  items.Add(tv);

            networkStream = new NetworkStream(this.socketToTheClient);
            streamreader = new StreamReader(networkStream);
            streamwriter = new StreamWriter(networkStream);

            IPAddress clientIpAdress = ((IPEndPoint)this.socketToTheClient.RemoteEndPoint).Address;
            clientIpAdressString = clientIpAdress.ToString();
            clientName = GetMachineNameFromIPAddress(clientIpAdressString);
            Thread.CurrentThread.Name = clientName;
            string state1 = "First";
            string state2 = "Second";
            string state3 = "Third. Sold to " + clientName;


            streamwriter.WriteLine(tv.Name);
            streamwriter.WriteLine(tv.StartPrice);
            streamwriter.WriteLine(tv.CurrPrice);
            this.monitor.AddClients(streamwriter);
            this.monitor.BroadcastBid(clientIpAdressString, "joined");

            while (true)
            {
                int currentBid = 0;
                bid = streamreader.ReadLine();
                TimeSpan actualTime;
                if (bid == null)
                    break;

                if (int.TryParse(bid, out currentBid))
                {
                    if (currentBid > tv.CurrPrice)
                    {
                        tv.CurrPrice = monitor.NewHighestBid(currentBid);
                        monitor.BroadcastBid(Thread.CurrentThread.Name, bid + " Kr.");
                        actualTime = DateTime.Now.TimeOfDay;
                        

                        startGavel(actualTime);
                       
                    }
                    else
                    {
                        streamwriter.WriteLine("Invalid Bid. Must be larger than the Highest bid");
                        streamwriter.Flush();
                    }
                }
                else
                {
                    streamwriter.WriteLine("Sever says: Invalid Bid. Must enter digits only");
                    streamwriter.Flush();
                }


                Console.WriteLine(Thread.CurrentThread.Name + "  " + bid);
            }

            monitor.RemoveClients(streamwriter);
            monitor.BroadcastBid(Thread.CurrentThread.Name, "Logged out");
            Console.WriteLine("Connection to client closing down...\n");
            streamwriter.Close();
            streamreader.Close();
            networkStream.Close();

        }
        private static string GetMachineNameFromIPAddress(string ipAdress)
        {
            string machineName = string.Empty;
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAdress);

                machineName = hostEntry.HostName;
            }
            catch (Exception ex)
            {
                // Machine not found...
            }
            return machineName;
        }

    }
}
