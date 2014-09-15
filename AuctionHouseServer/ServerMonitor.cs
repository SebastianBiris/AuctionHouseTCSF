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

        public ServerMonitor()
        {
            streamWriters = new List<StreamWriter>();
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
        public void BroadcastChat(string clientIpAdress, long bid)
        {
            string clientName = Thread.CurrentThread.Name;
            lock (this)
            {
                try
                {
                    foreach (StreamWriter streamWriter in streamWriters)
                    {
                        streamWriter.WriteLine(clientIpAdress + "(" + clientName + "): " + bid);
                        streamWriter.Flush();
                        //long max=0;
                        // if(bid>max)
                        // { max =bid;}
                        // else
                        // {Console.WriteLine}
                    }
                }
                catch (Exception ex)
                { Console.WriteLine(ex.ToString()); }
            }
        }
    }
}
