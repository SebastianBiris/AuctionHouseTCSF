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
    public delegate void ResetGavelDelegate();
    class ServerThreadMethods
    {
        private Socket socketToTheClient;
        public Thread gavelThread;
        private ServerMonitor monitor;
        ServerMain serverMain;
        public void Run()
        {
            gavelThread = new Thread(new ThreadStart(this.RunGavel));
            gavelThread.Start();
            
        }
        public void ResetGavel()
        {
            gavelThread = new Thread(new ThreadStart(this.ResetGavel));
            this.gavelThread.Abort();
            monitor.GavelNo = 0;
            monitor.SecondsSinceLastGavel = 0;
            gavelThread = new Thread(new ThreadStart(this.RunGavel));
            gavelThread.Start();
        }
        public void RunGavel()
        {
            Thread.Sleep(10000);
            monitor.GavelNo = 1;
            monitor.SecondsSinceLastGavel = 10;
           
                if (monitor.GavelNo == 1 || monitor.SecondsSinceLastGavel == 10)
                {
                    monitor.SecondsSinceLastGavel = 0;
                    monitor.BroadcastGavel("First");
                    Thread.Sleep(1000);
                    monitor.SecondsSinceLastGavel++;
                    Thread.Sleep(1000);
                    monitor.SecondsSinceLastGavel++;
                    Thread.Sleep(1000);
                    monitor.SecondsSinceLastGavel++;
                    Thread.Sleep(1000);
                    monitor.SecondsSinceLastGavel++;
                    Thread.Sleep(1000);
                    monitor.SecondsSinceLastGavel++;
                    monitor.GavelNo++;
                }
                if (monitor.GavelNo == 2 || monitor.SecondsSinceLastGavel == 5)
                {
                    monitor.BroadcastGavel("Second");
                    monitor.SecondsSinceLastGavel = 0;
                    Thread.Sleep(1000);
                    monitor.SecondsSinceLastGavel++;
                    Thread.Sleep(1000);
                    monitor.SecondsSinceLastGavel++;
                    Thread.Sleep(1000);
                    monitor.SecondsSinceLastGavel++;
                    monitor.GavelNo++;
                }
                if (monitor.GavelNo == 3 || monitor.SecondsSinceLastGavel == 3)
                {
                    monitor.BroadcastGavel("Sold to:" + Thread.CurrentThread.Name);
                    Thread.Sleep(1000);

                    monitor.SecondsSinceLastGavel++;
                }
            }


        


        public ServerThreadMethods(Socket socketToTheClient, ServerMonitor monitor)
        {
            this.socketToTheClient = socketToTheClient;
            this.monitor = monitor;
        }


        public void HandleClient()
        {
            
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
                        monitor.ResetGavelDelegate += new ResetGavelDelegate(this.ResetGavel);
                       RunGavel();

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
