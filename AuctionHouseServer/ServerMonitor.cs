using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace AuctionHouseServer
{
    class ServerMonitor
    {
        private List<StreamWriter> streamWriters;
        private int highestBid;
        private object door;
        int count = 0;
        public ServerMonitor()
        {
            streamWriters = new List<StreamWriter>();
            highestBid = 0;
        }

        public void AddClients(StreamWriter streamWriter)
        {
            lock (this)
            {
                streamWriters.Add(streamWriter);
            }
        }
        public void RemoveClients(StreamWriter streamWriter)
        {
            lock (this)
            {
                streamWriters.Remove(streamWriter);
            }
        }
        public void BroadcastBid(string clientIpAdress, string bid)
        {
            string clientName = Thread.CurrentThread.Name;
            lock (this)
            {
                try
                {
                    foreach (StreamWriter streamWriter in streamWriters)
                    {
                        streamWriter.WriteLine( "(" + clientName + "): " + bid );
                        streamWriter.Flush();
                     
                    }
                }
                catch (Exception ex)
                { Console.WriteLine(ex.ToString()); }
            }
        }

        public int NewHighestBid(int currentBid)
        {
            lock (this)
            {
                highestBid = currentBid;
            }
            return highestBid;
        }
        public void Gavel(int currBid)
        {

            lock (this)
            {
              
                    try
                    {
                        foreach (StreamWriter streamWriter in streamWriters)
                        {
                            Thread.Sleep(3000);
                            streamWriter.WriteLine("First");
                            streamWriter.Flush();
                            Thread.Sleep(3000);
                            streamWriter.WriteLine("Second");
                            streamWriter.Flush();
                            Thread.Sleep(3000);
                            streamWriter.WriteLine("Third");
                            streamWriter.WriteLine("Sold to: " + Thread.CurrentThread.Name+ " for "+currBid);
                            streamWriter.Flush();
                        }
                    }

                    catch (Exception ex)
                    { Console.WriteLine(ex); }

                
              
            }
        }
    }
}
