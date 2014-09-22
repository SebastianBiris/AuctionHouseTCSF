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
        public event ResetGavelDelegate ResetGavelDelegate;
        private List<StreamWriter> streamWriters;
        private int highestBid;
        private object door;
        private int secondsSinceLastGavel;
        int gavelNo;


        public ServerMonitor()
        {
            streamWriters = new List<StreamWriter>();
            highestBid = 0;
            this.gavelNo = 0;
            this.secondsSinceLastGavel = 0;
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
                        streamWriter.WriteLine("(" + clientName + "): " + bid);
                        streamWriter.Flush();

                    }
                }
                catch (Exception ex)
                { Console.WriteLine(ex.ToString()); }
            }
        }
        public void BroadcastGavel(string state )
        { 
            lock (this)
            {
                try
                {
                    foreach (StreamWriter streamWriter in streamWriters)
                    {
                        streamWriter.WriteLine( state  );
                        streamWriter.Flush();

                    }
                }
                catch (Exception ex)
                { Console.WriteLine(ex.ToString()); }
            }
        }

        public int NewHighestBid(int currentBid)
        {
            //int currentBid = 0;
            //int.TryParse(bid, out currentBid);
            //highestBid = currentBid;
            lock (this)
            {
                if (currentBid == highestBid)
                {
                    Monitor.Wait(this.door, 8000);
                }
                if (currentBid < highestBid)
                {
                    Monitor.PulseAll(this.door);
                }
                return currentBid;
                
            }
        }

    public int GavelNo
        {
            get { return gavelNo; }
            set { gavelNo = value; }
        } 
        public int SecondsSinceLastGavel
        {
            get { return secondsSinceLastGavel; }
            set { secondsSinceLastGavel = value; }
        }
    }
}

