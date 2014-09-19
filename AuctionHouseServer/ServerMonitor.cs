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
        TimeSpan bidTime;


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

       

        //public string Gavel(int currBid)
        //{

        //    lock (this.door)
        //    {

        //        if (highestBid == currBid)
        //        {

        //            Monitor.Wait(this.door, 3000);
                    
        //        }
        //        if (currBid > highestBid)
        //        {
        //            Monitor.PulseAll(this.door);
        //        }

        //    }
        //     return "First";
        //}
        //public string Gavel2(int currBid)
        //{

        //    lock (this.door)
        //    {

        //        if (highestBid == currBid)
        //        {

        //            Monitor.Wait(this.door, 3000);
                    
        //        }
        //        if (currBid > highestBid)
        //        {
        //            Monitor.PulseAll(this.door);
        //        }
        //       return "Second";
        //    }
        //}
        //public string Gavel3(int currBid)
        //{

        //    lock (this)
        //    {

        //       if (highestBid == currBid)
        //        {

        //            Monitor.Wait(this, 3000);                   
        //        }
        //        if (currBid > highestBid)
        //        {
        //            Monitor.PulseAll(this);

        //        }
        //        return "Third.Sold to:" + Thread.CurrentThread.Name + " for " + currBid;
        //    }
        //}
    }
}

