using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLFSupport.Model.CustomeModel
{
    public class ProjectDeadlineViewModel
    {
        public ProjectDeadlineViewModel()
        {
        }

        public ProjectDeadlineViewModel(int id)
        {
            ProjectId = id;
        }
        public int Id { get; set; }
        public System.DateTime RevisedDate { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public System.DateTime UpdatedOn { get; set; }
        public string Reason { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}
