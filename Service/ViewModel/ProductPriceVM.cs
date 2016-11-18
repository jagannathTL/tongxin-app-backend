using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModel
{
    public class ProductPriceVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Date { get; set; }
        public string LPrice { get; set; }
        public string HPrice { get; set; }
        public string Change { get; set; }
        public string IsOrder { get; set; }
        public DateTime AddDate { get; set; }
        public string ParentName { get; set; }
        public string Spec { get; set; }
        public string Comment { get; set; }
    }
}
