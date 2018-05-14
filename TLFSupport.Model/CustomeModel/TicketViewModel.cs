using System;
using System.ComponentModel.DataAnnotations;

namespace TLFSupport.Model.CustomeModel
{
    /// <summary>
    /// Display all ticket details
    /// </summary>
    public class TicketViewModel
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; }

        [Required(ErrorMessage = "please select appropriate customer")]
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        [Required(ErrorMessage = "please select appropriate project")]
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        [Required(ErrorMessage = "please select appropriate priority")]
        public int IDSLA { get; set; }

        public string Priority { get; set; }

        [Required(ErrorMessage = "please select appropriate employee")]
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }
        public Nullable<int> ForwardTo { get; set; }
        public string ForwardEmployeeName { get; set; }
        public int PriorityLevel { get; set; }

        [Required(ErrorMessage = "please enter valid title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "please enter valid description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "please enter valid comment")]
        public string Comment { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required(ErrorMessage = "please select appropriate status")]
        public int StatusId { get; set; }

        public string Status { get; set; }
        public string MaximumResponseTime { get; set; }
        public string MaximumResolveTime { get; set; }
        //public IEnumerable<Employee> Employees { get; set; }
        //public IEnumerable<CS_SLA> PriorityList { get; set; }
    }
}