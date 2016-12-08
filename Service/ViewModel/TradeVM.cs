using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModel
{
    public class TradeVM
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string Contact { get; set; }
        public string Mobile { get; set; }
        public string ContactTel { get; set; }//联系方式
        public string Province { get; set; }
        public string City { get; set; }
        public string Buissnes { get; set; }//所属行业
        public string Description { get; set; }
        public int DocumentType { get; set; }//0:供应,1:采购,2:机械
        public string Date { get; set; }
        public bool IsChecked { get; set; }
        public List<string> Pics { get; set; }
    }

    public class TradeImageVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TradeId { get; set; }
    }
}
