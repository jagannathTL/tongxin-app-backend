using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModel
{
    public class SupplyVM
    {
        public int Id { get; set; }
        public bool SupplyType { get; set; }
        public string Product { get; set; }
        public int CatalogId { get; set; }
        public string Quantity { get; set; }
        public string Mobile { get; set; }
        public string Contact { get; set; }
        public string Description { get; set; }
        public bool DeliveryType { get; set; }
        public string Provice { get; set; }
        public string City { get; set; }
        public string CreateDate { get; set; }
        public string Creater { get; set; }
        public string Price { get; set; }
        public List<string> Images { get; set; }
    }

    public class SupplyViewVM
    {
        public int Id { get; set; }
        public bool SupplyType { get; set; }
        public bool? isChecked { get; set; }
        public int CatalogId { get; set; }
        public string Avatar { get; set; }
        public string Product { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string Date { get; set; }
        public string Price { get; set; }
    }

    public class SupplyDetailVM
    {
        public int Id { get; set; }
        public bool SupplyType { get; set; }
        public List<string> Avatar { get; set; }
        public string Product { get; set; }
        public string Quantity { get; set; }
        public string Mobile { get; set; }
        public string Contact { get; set; }
        public string Description { get; set; }
        public bool DeliveryType { get; set; }
        public string Address { get; set; }
        public string Date { get; set; }
        public string Price { get; set; }
        public string ErrorCode { get; set; }
    }
}
