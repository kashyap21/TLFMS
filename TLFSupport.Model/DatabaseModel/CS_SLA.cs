//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TLFSupport.Model.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class CS_SLA
    {
        public CS_SLA()
        {
            this.CS_Ticket = new HashSet<CS_Ticket>();
        }
    
        public int SLAId { get; set; }
        public byte PriorityLevel { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime LastModified { get; set; }
        public string PriorityName { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public int MaximumResponseTime { get; set; }
        public int MaximumResolveTime { get; set; }
        public int EscalationTime { get; set; }
        public Nullable<bool> IsActive { get; set; }
    
        public virtual Projects Projects { get; set; }
        public virtual ICollection<CS_Ticket> CS_Ticket { get; set; }
    }
}