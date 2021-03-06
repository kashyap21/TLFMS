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
    
    public partial class CS_Ticket
    {
        public CS_Ticket()
        {
            this.CS_Log = new HashSet<CS_Log>();
            this.CS_CallLog = new HashSet<CS_CallLog>();
        }
    
        public int TicketId { get; set; }
        public string TicketNumber { get; set; }
        public byte PriorityLevel { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CustomerId { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public int StatusId { get; set; }
        public int SLAId { get; set; }
        public string LastComment { get; set; }
        public Nullable<int> ForwardToEmployee { get; set; }
    
        public virtual CS_Status CS_Status { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Projects Projects { get; set; }
        public virtual ICollection<CS_Log> CS_Log { get; set; }
        public virtual Employee Employee1 { get; set; }
        public virtual CS_EmailLink CS_EmailLink { get; set; }
        public virtual ICollection<CS_CallLog> CS_CallLog { get; set; }
        public virtual CS_SLA CS_SLA { get; set; }
    }
}
