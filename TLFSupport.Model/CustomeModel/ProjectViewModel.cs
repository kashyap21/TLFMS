using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLFSupport.Model.CustomeModel
{
    public class ProjectViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select appropriate project type")]
        public int ProjectTypeId { get; set; }

        public string ProjectTypeName { get; set; }

        [Required(ErrorMessage = "Please select appropriate project category")]
        public int ProjectCategoryId { get; set; }

        public string ProjectCategoryName { get; set; }

        [Required(ErrorMessage = "Please enter valid project name")]
        public string ProjectName { get; set; }

        public string ProjectCode { get; set; }

        [Required(ErrorMessage = "Please enter valid short code")]
        public string ShortCode { get; set; }

        public Nullable<int> TechnologyId { get; set; }
        public string TechnologyName { get; set; }

        [Required(ErrorMessage = "Please select appropriate project owner")]
        public int ProjectOwner { get; set; }

        [Required(ErrorMessage = "Please select appropriate project manager")]
        public Nullable<int> ProjectManager { get; set; }

        public string ProjectManagerName { get; set; }

        [Required(ErrorMessage = "Please select appropriate project lead")]
        public Nullable<int> ProjectLead { get; set; }

        public string ProjectLeadName { get; set; }

        // [Required(ErrorMessage = "Please enter valid project description")]
        public string ProjectBrief { get; set; }

        public string CustomerName { get; set; }
        public string QuintiqProjectCode { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerLocation { get; set; }

        [Required(ErrorMessage = "Please enter valid estimated hours")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Count must be a natural number")]
        public Nullable<decimal> InitialEstimatedHrs { get; set; }

        public Nullable<decimal> ModelingEstimatedHrs { get; set; }
        public Nullable<int> ProjectStatusId { get; set; }
        public string ProjectStatus { get; set; }
        public bool IsNotification { get; set; }
        public bool IsProspect { get; set; }

        [Required(ErrorMessage = "Please select appropriate customer")]
        public Nullable<int> CustomerId { get; set; }

        [Required(ErrorMessage = "Please select appropriate project group")]
        public Nullable<int> ProjectGroupId { get; set; }

        public string ProjectGroupName { get; set; }

        //  [Required(ErrorMessage = "Please select valid project deadline")]
        public Nullable<System.DateTime> InitialDeadline { get; set; }

        public Nullable<System.DateTime> CurrentDeadline { get; set; }
    }
}