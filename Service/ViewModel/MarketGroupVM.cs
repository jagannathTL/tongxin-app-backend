using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModel
{
    public class MarketGroupVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string inBucket { get; set; }
        public List<MarketVM> Market { get; set; }

    }
}
