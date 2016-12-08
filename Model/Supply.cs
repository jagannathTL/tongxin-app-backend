//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Supply
    {
        public Supply()
        {
            this.Images = new HashSet<Image>();
        }
    
        public int ID { get; set; }
        public Nullable<bool> SupplyType { get; set; }
        public Nullable<int> CatalogID { get; set; }
        public string Product { get; set; }
        public string Quantity { get; set; }
        public string Mobile { get; set; }
        public string Contact { get; set; }
        public string Description { get; set; }
        public Nullable<bool> DeliveryType { get; set; }
        public Nullable<int> ProviceID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<bool> isChecked { get; set; }
        public string Creater { get; set; }
        public string Price { get; set; }
        public string buissnes { get; set; }
        public string provinceName { get; set; }
        public string cityName { get; set; }
        public Nullable<int> DocumentType { get; set; }
    
        public virtual ICollection<Image> Images { get; set; }
        public virtual Province Province { get; set; }
    }
}
