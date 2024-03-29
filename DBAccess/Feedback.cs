//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DBAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class Feedback
    {
        public int ID { get; set; }
        public Nullable<int> IDLifeInCity { get; set; }
        public Nullable<int> IDTransportation { get; set; }
        public Nullable<int> IDAccommodation { get; set; }
        public int IDUser { get; set; }
        public System.DateTime FeedbackDate { get; set; }
        public string Comment { get; set; }
        public Nullable<int> Rating { get; set; }
        public string Heading { get; set; }
    
        public virtual Accommodation Accommodation { get; set; }
        public virtual CTEmployee CTEmployee { get; set; }
        public virtual LifeInCity LifeInCity { get; set; }
        public virtual Transportation Transportation { get; set; }
    }
}
