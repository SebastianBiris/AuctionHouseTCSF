using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouseServer
{
    class Item
    {
        private string name;
        private long startPrice;
        private long currPrice;

        //constructor
        public Item(string name, long startPrice, long currPrice)
        {
            this.name = name;
            this.startPrice = startPrice;
            this.currPrice = currPrice;
        }

        #region Properties
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public long StartPrice
        {
            get { return startPrice; }
            set { startPrice = value; }
        }

        public long CurrPrice
        {
            get { return currPrice; }
            set { currPrice = value; }
        }
        #endregion
    }
}
