namespace TLFSupport.Common
{
    /// <summary>
    /// List of actions
    /// </summary>
    public static class ActionName
    {
        #region Public variable

        /// <summary>
        /// Customer index
        /// </summary>
        public const string CustomerMgtIndex = "Index";

        /// <summary>
        /// Customer create
        /// </summary>
        public const string CustomerMgtCreate = "Create";

        /// <summary>
        /// Customer index
        /// </summary>
        public const string ProjectMgtIndex = "Index";

        /// <summary>
        /// Customer index
        /// </summary>
        public const string ProjectMgtView = "ViewProjects";

        /// <summary>
        /// Customer create
        /// </summary>
        public const string ProjectMgtCreate = "Create";

        /// <summary>
        /// Customer create
        /// </summary>
        public const string ProjectMgtEdit = "Edit";

        /// <summary>
        /// CallLog index
        /// </summary>
        public const string CallLogIndex = "Index";

        /// <summary>
        /// call log view
        /// </summary>
        public const string CallLogView = "ViewCallLog";

        /// <summary>
        /// Ticket index
        /// </summary>
        public const string TicketIndex = "Index";

        /// <summary>
        /// Ticket create
        /// </summary>
        public const string TicketCreate = "Create";

        /// <summary>
        /// Generate ticket
        /// </summary>
        public const string TicketGenerate = "Generate";

        /// <summary>
        /// Configuration window
        /// </summary>
        public const string ConfigurationIndex = "Index";

        /// <summary>
        /// Ticket view
        /// </summary>
        public const string TicketView = "ViewTicket";

        /// <summary>
        /// Ticket edit
        /// </summary>
        public const string TicketEdit = "Edit";

        /// <summary>
        /// NLEmployee List
        /// </summary>
        public const string GetAllNLEmployee = "GetAllNLEmployee";

        /// <summary>
        /// FMSSLA Create
        /// </summary>
        public const string FMSSLACreate = "Create";

        /// <summary>
        /// FMSSLA Index
        /// </summary>
        public const string FMSSLAIndex = "Index";

        /// <summary>
        /// FMSSLA edit
        /// </summary>
        public const string FMSSLAEdit = "Edit";

        /// <summary>
        /// SLA Detail view
        /// </summary>
        public const string SlaDetail = "SLADetail";

        /// <summary>
        /// SLA Detail window view
        /// </summary>
        public const string SlaDetailView = "GetListOfSLA";

        /// <summary>
        /// Authentication Login
        /// </summary>
        public const string AuthenticationLogin = "Login";

        /// <summary>
        /// Get Customer
        /// </summary>
        public const string FMSSLAGetCustomer = "GetCustomer";

        /// <summary>
        /// Get projects
        /// </summary>
        public const string FMSSLAGetCascadeProjects = "GetCascadeProjects";

        /// <summary>
        /// Get sla by project id
        /// </summary>
        public const string FMSSLAGetSLAByProjectId = "GetSLAByProjectId";

        /// <summary>
        /// FMSSLA Inline create
        /// </summary>
        public const string FMSSLAInlineCreate = "FmsSlaInlineCreate";

        /// <summary>
        /// FMSSLA Inline Update
        /// </summary>
        public const string FMSSLAInlineUpdate = "FmsSlaInlineUpdate";

        /// <summary>
        /// FMSSLA Inline Delete
        /// </summary>
        public const string FMSSLAInlineDelete = "FmsSlaInlineDestroy";

        /// <summary>
        /// FMSSLA ViewSLA
        /// </summary>
        public const string FMSSLAViewSLA = "ViewSLADetail";

        /// <summary>
        /// FMSSLA Customer Filtering
        /// </summary>
        public const string FMSSLAFilterByCustomerName = "GetCustomerNameForFiltering";

        /// <summary>
        /// FMSSLA Action Filtering
        /// </summary>
        public const string CallLogFilterByActionName = "GetActionNameForFiltering";

        /// <summary>
        /// Ticket Priority Filtering
        /// </summary>
        public const string TicketFilterByPriority = "GetPriorityForFiltering";

        /// <summary>
        /// Ticket Status Filtering
        /// </summary>
        public const string TicketFilterByStauts = "GetStatusForFiltering";

        /// <summary>
        /// Get Customer
        /// </summary>
        public const string TicketGetCustomer = "GetCustomer";

        /// <summary>
        /// Get projects
        /// </summary>
        public const string TicketGetCascadeProjects = "GetCascadeProjects";

        /// <summary>
        /// Get priority
        /// </summary>
        public const string TicketGetPriority = "GetPriorityOfProject";

        /// <summary>
        /// Get employee
        /// </summary>
        public const string TicketGetEmployeeForProject = "ViewEmployeeByProjectId";

        /// <summary>
        /// Get priority
        /// </summary>
        public const string TicketGetPriorityForProject = "ViewPriorityByProjectId";

        /// <summary>
        /// Get projects
        /// </summary>
        public const string TicketFilterByProject = "GetProjectsForFiltering";

        /// <summary>
        /// Get Employee
        /// </summary>
        public const string TicketFilterByEmployee = "GetEmployeeForFiltering";

        /// <summary>
        /// Get Employee
        /// </summary>
        public const string TicketFilterByTicketTitle = "GetTicketTitleForFiltering";

        /// <summary>
        /// Get status
        /// </summary>
        public const string TicketGetStatus = "ViewStatus";

        /// <summary>
        /// Get list of project for given employee
        /// </summary>
        public const string GetProjectForEmployee = "GetProjectForEmployee";

        /// <summary>
        /// Get List of all employee
        /// </summary>
        public const string GetAllEmployee = "GetAllEmployee";

        public const string GetAllEmployeeForIndia = "GetAllEmployeeForIndia";

        /// <summary>
        /// Get list of all phases
        /// </summary>
        public const string GetAllPhase = "GetAllPhase";

        /// <summary>
        /// Get List of Activity based on project
        /// </summary>
        public const string GetActivityByProjectId = "GetActivityByProjectId";

        /// <summary>
        /// Get Grid of timesheet information
        /// </summary>
        public const string TimesheetDetailView = "TimesheetDetailView";

        /// <summary>
        /// Fetch list of project for Employee in timesheet
        /// </summary>
        public const string GetProjectForEmployeeForTimesheet = "GetProjectForEmployeeForTimesheet";

        /// <summary>
        /// Fetch list of project for Employee in timesheet
        /// </summary>
        public const string GetProjectForNLEmployee = "GetProjectForNLEmployee";

        /// <summary>
        /// Get timesheet grid 
        /// </summary>
        public const string GetTimesheetSummeryGrid = "GetTimesheetSummeryGrid";

        public const string EditTimesheetIndia = "EditTimesheetIndia";
        #endregion Public variable
    }
}