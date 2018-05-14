using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TLFSupport.Model.CustomeModel
{
    public class TimesheetSummaryReportViewModel
    {
        public int EmployeeId { get; set; }


        [Range(1, 53, ErrorMessage = "Invalid Week No")]
        public short? WeekNo { get; set; }

        public string EmployeeName { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalHour { get; set; }
        public string shortcode { get; set; }
    }
}
