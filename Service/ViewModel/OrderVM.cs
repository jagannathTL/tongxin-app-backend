using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModel
{
    public class OrderVM
    {
        //public int MarketId { get; set; }
        public string MarketName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }

    public class MyOrderMarketVM
    {
        public int MarketId { get; set; }
        public string MarketName { get; set; }
    }
}
