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
    
    public partial class Gp
    {
        public int ID { get; set; }
        public string Tel { get; set; }
        public Nullable<int> RoleID { get; set; }
        public Nullable<int> MarketID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<int> ProductNum { get; set; }
        public Nullable<decimal> ProductSum { get; set; }
        public Nullable<int> ProductKind { get; set; }
        public string SendTime { get; set; }
    
        public virtual Market Market { get; set; }
    }
}