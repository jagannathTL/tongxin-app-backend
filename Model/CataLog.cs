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
    
    public partial class CataLog
    {
        public int ID { get; set; }
        public string KeyName { get; set; }
        public string Description { get; set; }
        public int ChannelId { get; set; }
    
        public virtual Channel Channel { get; set; }
    }
}
