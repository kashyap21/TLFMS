using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLFSupport.Model.CustomeModel
{
    public class ProjectPhaseViewModel
    {
        public ProjectPhaseViewModel()
        {
        }

        public ProjectPhaseViewModel(int Id)
        {
            ProjectId = Id;
        }

        public bool IsCurrent { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> PhaseId { get; set; }
        public Nullable<int> SeqNr { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string CreatedIPAddress { get; set; }
        public string PhaseName { get; set; }
        public string UpdatedIPAddress { get; set; }
    }
}