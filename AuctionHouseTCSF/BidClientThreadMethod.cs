using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AuctionHouseTCSF
{
    class BidClientThreadMethod
    {
        public event BidReceived ReceivedBidEvent;

        public void ReceivedBidInfo(object streamReaderObj)
        {
            string bid;
            StreamReader streamReader = (StreamReader)streamReaderObj;

            while (true)
            {
                try
                { 
                    bid = streamReader.ReadLine(); 
                }
                catch (IOException)
                { 
                    break;
                }
                ReceivedBidEvent(bid);

            }


        }
    }
}
