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
    
    public partial class ProjectPhases
    {
        public ProjectPhases()
        {
            this.TimeSheet = new HashSet<TimeSheet>();
        }
    
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Nullable<int> PhaseId { get; set; }
        public string PhaseName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string CreatedIPAddress { get; set; }
        public string UpdatedIPAddress { get; set; }
        public Nullable<int> SeqNr { get; set; }
        public bool IsCurrent { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        public virtual Phases Phases { get; set; }
        public virtual ICollection<TimeSheet> TimeSheet { get; set; }
        public virtual Projects Projects { get; set; }
    }
}