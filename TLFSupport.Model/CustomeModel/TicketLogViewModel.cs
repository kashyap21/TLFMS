using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLFSupport.Model.CustomeModel
{
    public class TicketLogViewModel
    {
        public int LogId { get; set; }
        public string LogTitle { get; set; }
        public string LogComment { get; set; }
        public DateTime LogCreatedOn { get; set; }
        public int TicketId { get; set; }
        public string TicketNumber { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        //public int IDSLA { get; set; }
        public string PriorityName { get; set; }
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }
        public Nullable<int> ForwardTo { get; set; }
        public string ForwardEmployeeName { get; set; }

        //public int PriorityLevel { get; set; }
        //public string Title { get; set; }
        //public string Description { get; set; }
        //public string Comment { get; set; }
        //public DateTime CreatedOn { get; set; }
        public int StatusId { get; set; }

        public string Status { get; set; }
    }
}