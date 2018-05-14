using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TLFSupport.Model.CustomeModel
{
    public class TimesheetReportViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }


        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string Type { get; set; }
        public int TypeId { get; set; }
        
        public bool IncludeExtra { get; set; }
    }
}
