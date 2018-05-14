using System;
using System.ComponentModel.DataAnnotations;

namespace TLFSupport.Model.CustomeModel
{
    public class SLAViewModel
    {
        [Required(ErrorMessage = "please select appropriate priority")]
        public int SLAId { get; set; }

        [Required(ErrorMessage = "please select appropriate project")]
        public int ProjId { get; set; }

        public string ProjName { get; set; }

        [Required(ErrorMessage = "please select appropriate customer")]
        public Nullable<int> CustId { get; set; }

        public string CustName { get; set; }
        public int SLAPriorityLevel { get; set; }
        public string PriorityName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModified { get; set; }

        [Required(ErrorMessage = "please enter valid description")]
        public string SLADescription { get; set; }

        [Display(Name = "Default Priority")]
        public bool IsDefaultPriority { get; set; }

        public int MaximumResponseTime { get; set; }
        public int MaximumResolveTime { get; set; }

        public int EscalationTime { get; set; }

        [Display(Name = "Maximum Response Time")]
        public string tempResponse { get; set; }

        [Display(Name = "Maximum Resolve Time")]
        public string tempResolve { get; set; }

        [Display(Name = "Escalation Time")]
        public string tempEscalationTime { get; set; }

        public bool IsActive { get; set; }
    }
}