using System;
using System.ComponentModel.DataAnnotations;

namespace TLFSupport.Model.CustomeModel
{
    /// <summary>
    /// Contains all Information about timesheet
    /// </summary>
    public class NLTimesheetViewModel
    {
        public int TimesheetId { get; set; }
        public string ProjectCode { get; set; }

        [Required(ErrorMessage = "Project is required")]
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        [Required(ErrorMessage = "Activity is required")]
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }

        [Required(ErrorMessage = "Employ is required")]
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Phase is required")]
        public int PhaseId { get; set; }
        public string PhaseName { get; set; }

        [Display(Name = "Mon")]
        public decimal MonActualHrs { get; set; }
        [Display(Name = "Tue")]
        public decimal TueActualHrs { get; set; }
        [Display(Name = "Wed")]
        public decimal WedActualHrs { get; set; }
        [Display(Name = "Thu")]
        public decimal ThuActualHrs { get; set; }
        [Display(Name = "Fri")]
        public decimal FriActualHrs { get; set; }
        [Display(Name = "Sat")]
        public decimal SatActualHrs { get; set; }
        [Display(Name = "Sun")]
        public decimal SunActualHrs { get; set; }
        [Display(Name = "Total")]
        public decimal TotalHrs { get; set; }

        public DateTime WeekFirstDate { get; set; }
        public string Remark { get; set; }
        public short weekno { get; set; }
        public DateTime Date { get; set; }
    }
}