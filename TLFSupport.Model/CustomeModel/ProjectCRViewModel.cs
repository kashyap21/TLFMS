using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLFSupport.Model.CustomeModel
{
    public class ProjectCRViewModel
    {
        public ProjectCRViewModel()
        {
        }

        public ProjectCRViewModel(int id)
        {
            ProjectId = id;
        }

        public int CRId { get; set; }
        public int ProjectId { get; set; }
        public string CRNo { get; set; }
        public System.DateTime CRDate { get; set; }
        public string Description { get; set; }
        public decimal EstimatedHrs { get; set; }
        public bool ApprovedByClient { get; set; }
        public int ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string Status { get; set; }
    }
}