using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModel
{
    public class MarketVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductPriceVM> NewPrices { get; set; }

        public int Order { get; set; }
    }
}
