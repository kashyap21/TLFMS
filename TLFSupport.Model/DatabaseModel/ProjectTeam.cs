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
    
    public partial class ProjectTeam
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ProjectRoleId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsActive { get; set; }
        public bool IsCreate { get; set; }
        public Nullable<byte> CS_Level { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Projects Projects { get; set; }
        public virtual ProjectRole ProjectRole { get; set; }
    }
}