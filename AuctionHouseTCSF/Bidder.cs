﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseTCSF
{
    class Bidder
    {
        private string name;
        private long bid;

        public long Bid
        {
            get { return bid; }
            set { bid = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
       
        public Bidder(string name, long bid)
        {
            this.name = name;
            this.bid = bid;
        
        }
    }
}
