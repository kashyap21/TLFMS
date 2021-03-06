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
    
    public partial class Projects
    {
        public Projects()
        {
            this.ProjectTeam = new HashSet<ProjectTeam>();
            this.CS_Ticket = new HashSet<CS_Ticket>();
            this.TimeSheet = new HashSet<TimeSheet>();
            this.ProjectActivityLink = new HashSet<ProjectActivityLink>();
            this.ProjectPhases = new HashSet<ProjectPhases>();
            this.ProjectCR = new HashSet<ProjectCR>();
            this.ProjectCR1 = new HashSet<ProjectCR>();
            this.ProjectDeadline = new HashSet<ProjectDeadline>();
            this.ProjectResourceDemand = new HashSet<ProjectResourceDemand>();
            this.TlfmsJiraLink = new HashSet<TlfmsJiraLink>();
            this.CS_SLA = new HashSet<CS_SLA>();
        }
    
        public int Id { get; set; }
        public int ProjectTypeId { get; set; }
        public int ProjectCategoryId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public string ShortCode { get; set; }
        public Nullable<int> TechnologyId { get; set; }
        public int ProjectOwner { get; set; }
        public Nullable<int> ProjectManager { get; set; }
        public Nullable<int> ProjectLead { get; set; }
        public string ProjectBrief { get; set; }
        public string CustomerName { get; set; }
        public string QuintiqProjectCode { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerLocation { get; set; }
        public Nullable<decimal> InitialEstimatedHrs { get; set; }
        public Nullable<decimal> ModelingEstimatedHrs { get; set; }
        public Nullable<int> ProjectStatusId { get; set; }
        public bool IsNotification { get; set; }
        public bool IsProspect { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> ProjectGroupId { get; set; }
        public Nullable<System.DateTime> InitialDeadline { get; set; }
        public Nullable<System.DateTime> CurrentDeadline { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        public virtual Employee Employee2 { get; set; }
        public virtual ICollection<ProjectTeam> ProjectTeam { get; set; }
        public virtual ICollection<CS_Ticket> CS_Ticket { get; set; }
        public virtual ProjectGroup ProjectGroup { get; set; }
        public virtual ProjectStatus ProjectStatus { get; set; }
        public virtual Technology Technology { get; set; }
        public virtual ICollection<TimeSheet> TimeSheet { get; set; }
        public virtual ICollection<ProjectActivityLink> ProjectActivityLink { get; set; }
        public virtual ICollection<ProjectPhases> ProjectPhases { get; set; }
        public virtual ProjectType ProjectType { get; set; }
        public virtual ProjectCategory ProjectCategory { get; set; }
        public virtual ICollection<ProjectCR> ProjectCR { get; set; }
        public virtual ICollection<ProjectCR> ProjectCR1 { get; set; }
        public virtual ICollection<ProjectDeadline> ProjectDeadline { get; set; }
        public virtual ICollection<ProjectResourceDemand> ProjectResourceDemand { get; set; }
        public virtual ICollection<TlfmsJiraLink> TlfmsJiraLink { get; set; }
        public virtual ICollection<CS_SLA> CS_SLA { get; set; }
    }
}
