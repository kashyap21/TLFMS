using System;
using System.ComponentModel.DataAnnotations;

namespace TLFSupport.Model.CustomeModel
{
    public class PopupSLAViewModel
    {
        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "please select appropriate priority")]
        public int SLAId { get; set; }

        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "please select appropriate project")]
        public int ProjId { get; set; }

        [ScaffoldColumn(false)]
        public string ProjName { get; set; }

        [ScaffoldColumn(false)]
        [Required(ErrorMessage = "please select appropriate customers")]
        public Nullable<int> CustId { get; set; }

        [ScaffoldColumn(false)]
        public string CustName { get; set; }

         [Required(ErrorMessage = "please enter valid priority level")]
         [Display(Name = "Priority Level")]
        [UIHint("Integer")]  
        public int SLAPriorityLevel { get; set; }

        [Required(ErrorMessage = "please enter valid priority name")]
        [Display(Name = "Priority Name")]
        [UIHint("String")]
        public string PriorityName { get; set; }

        [ScaffoldColumn(false)]
        public DateTime CreatedOn { get; set; }

        [ScaffoldColumn(false)]
        public DateTime LastModified { get; set; }

        [Required(ErrorMessage = "please enter valid description")]
        [Display(Name = "SLA Description")]
        [DataType(DataType.MultilineText)]
        public string SLADescription { get; set; }
       
        [ScaffoldColumn(false)]
        public int MaximumResponseTime { get; set; }

        [ScaffoldColumn(false)]
        public int MaximumResolveTime { get; set; }

        [ScaffoldColumn(false)]
        public int EscalationTime { get; set; }

        [Display(Name = "Maximum Response Time")]
        [Required(ErrorMessage = "please enter valid maximum response time")]
        [UIHint("MaskedTextBoxEditor")]
        public string tempResponse { get; set; }

        [Display(Name = "Maximum Resolve Time")]
        [Required(ErrorMessage = "please enter valid maximum resolve time")]
        [UIHint("MaskedTextBoxEditor")]
        public string tempResolve { get; set; }

        [Display(Name = "Escalation Time")]
        [Required(ErrorMessage = "please enter valid escalation time")]
        [UIHint("MaskedTextBoxEditor")]
        public string tempEscalationTime { get; set; }

        [Display(Name = "Default Priority")]
        public bool IsDefaultPriority { get; set; }

        public bool IsActive { get; set; }
    }
}