using System;

namespace TLFSupport.Model.CustomeModel
{
    public class ProjectJiraViewModel
    {
        public ProjectJiraViewModel(int id)
        {
            TlfmsProjectId = id;
        }

        public ProjectJiraViewModel()
        {
        }

        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int Id { get; set; }
        public int TlfmsProjectId { get; set; }
        public Nullable<DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string JiraProjectKey { get; set; }
        public string JiraProjectName { get; set; }
        public string JiraProjectVerison { get; set; }
    }
}