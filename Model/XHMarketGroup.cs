//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class XHMarketGroup
    {
        public XHMarketGroup()
        {
            this.MyAppGroups = new HashSet<MyAppGroup>();
        }
    
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public Nullable<int> Flag { get; set; }
        public bool IsForApp { get; set; }
        public Nullable<bool> IsDefault { get; set; }
    
        public virtual ICollection<MyAppGroup> MyAppGroups { get; set; }
    }
}
