using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLFSupport.Model.CustomeModel
{
    public class ProjectTeamsViewModel
    {
        public ProjectTeamsViewModel(int id)
        {
            ProjectId = id;
        }

        public ProjectTeamsViewModel()
        {
        }

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ProjectRoleId { get; set; }
        public string ProjectRoleName { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public bool IsActive { get; set; }
        public bool IsCreate { get; set; }
        public Nullable<byte> CS_Level { get; set; }
    }
}