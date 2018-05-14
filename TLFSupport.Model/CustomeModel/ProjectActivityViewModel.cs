using System;

namespace TLFSupport.Model.CustomeModel
{
    public class ProjectActivityViewModel
    {
        public ProjectActivityViewModel()
        {
        }

        public ProjectActivityViewModel(int Id)
        {
            ProjectId = Id;
        }

        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Nullable<int> ProjectActivityId { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedOn { get; set; }
        public string CreatedIPAddress { get; set; }
        public string ProjectActivityName { get; set; }
        public string UpdatedIPAddress { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}