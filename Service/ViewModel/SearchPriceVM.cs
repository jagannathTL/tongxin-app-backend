using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModel
{
    public class SearchPriceVM
    {
        public int MarketId { get; set; }
        public string MarketName { get; set; }
        public List<ProductPriceVM> prices { get; set; }
    }
}
