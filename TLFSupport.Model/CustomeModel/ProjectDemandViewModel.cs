using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLFSupport.Model.CustomeModel
{
    public class ProjectDemandViewModel
    {
        public ProjectDemandViewModel()
        {
        }

        public ProjectDemandViewModel(int id)
        {
            // TODO: Complete member initialization
            ProjectId = id;
        }

        public int ExpectedStartWeek { get; set; }
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ProjectRoleId { get; set; }
        public string ProjectRoleName { get; set; }
        public int Year { get; set; }
        public Nullable<int> ExpectedHoursPerWeek { get; set; }
        public Nullable<int> NoOfWeeks { get; set; }
        public string Remark { get; set; }
    }
}