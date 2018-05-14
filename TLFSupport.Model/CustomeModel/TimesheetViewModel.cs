using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TLFSupport.Model.CustomeModel
{
    /// <summary>
    /// Contains all Information about timesheet
    /// </summary>
    public class TimesheetViewModel
    {

        public int TimesheetId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        
        public int PhaseId { get; set; }
        public string PhaseName { get; set; }
        
        public string ReferenceIssueNo { get; set; }
        public decimal ActualHrs { get; set; }
        public decimal EstimatedHrs { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        //public string Date = System.DateTime.Today.ToString();
        public DateTime Date { get; set; }
        public DateTime currentTempDate { get; set; }
    }
}
