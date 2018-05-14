using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TLFSupport.Common;
using TLFSupport.Model;
using TLFSupport.Model.CustomeModel;
using TLFSupport.Model.DatabaseModel;
using TLFSupport.Model.DatabaseModel.Repositories;

namespace TLFSupport.Control.Controllers.Project
{
    public class ProjectController : Controller
    {
        #region Private members

        /// <summary>
        /// Database context
        /// </summary>
        private TLFCSEntities _dbContext;

        /// <summary>
        /// Instance of Project
        /// </summary>
        private GenericRepository<Projects> _objProject;

        /// <summary>
        /// Instance of ProjectTeam
        /// </summary>
        private GenericRepository<ProjectTeam> _objProjectTeam;

        #endregion Private members

        #region Public Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProjectController()
        {
            this._dbContext = BaseContext.GetDbContext();
            this._objProjectTeam = new GenericRepository<ProjectTeam>();
            this._objProject = new GenericRepository<Projects>();
        }

        #endregion Public Constructors

        #region Action Methods

        /// <summary>
        /// Project Details
        /// </summary>
        /// <returns>Details of Projects</returns>
        public ActionResult Create()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View(ViewName.ProjectMgtCreate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProjectViewModel pview)
        {
            Projects project = new Projects();
            ProjectTeam team = new ProjectTeam();

            project.ProjectName = pview.ProjectName;
            project.ProjectTypeId = pview.ProjectTypeId;
            project.ProjectCategoryId = pview.ProjectCategoryId;
            project.TechnologyId = pview.TechnologyId;
            project.ProjectOwner = pview.ProjectOwner;
            project.ProjectManager = pview.ProjectManager;
            project.ProjectLead = pview.ProjectLead;
            project.InitialEstimatedHrs = pview.InitialEstimatedHrs;
            project.InitialDeadline = pview.InitialDeadline;
            project.ShortCode = pview.ShortCode;
            project.CustomerId = pview.CustomerId;
            project.CustomerName = _dbContext.Customer.Where(x => x.Id == pview.CustomerId).Select(name => name.Name).SingleOrDefault();
            //  project.CustomerEmail = _dbContext.Customer.Where(x => x.Id == pview.CustomerId).Select(email => email.Email).SingleOrDefault();
            //  project.CustomerLocation = _dbContext.Customer.Where(x => x.Id == pview.CustomerId).Select(location => location.Country.Name).SingleOrDefault();

            project.ProjectGroupId = pview.ProjectGroupId;
            project.ProjectBrief = pview.ProjectBrief;
            project.ProjectStatusId = 5;
            _objProject.Insert(project);

            //ProjectTeam DefaultEntry

            //Owner
            team.ProjectId = project.Id;
            team.EmployeeId = pview.ProjectOwner;
            team.ProjectRoleId = 1;
            team.CS_Level = 1;
            team.IsActive = true;
            team.IsCreate = false;
            _objProjectTeam.Insert(team);

            //Manager
            team.ProjectId = project.Id;
            team.EmployeeId = pview.ProjectManager ?? 0;
            team.ProjectRoleId = 2;
            team.CS_Level = 2;
            team.IsActive = true;
            team.IsCreate = false;
            _objProjectTeam.Insert(team);

            //Leader
            team.ProjectId = project.Id;
            team.EmployeeId = pview.ProjectLead ?? 0;
            team.ProjectRoleId = 3;
            team.CS_Level = 3;
            team.IsActive = true;
            team.IsCreate = false;
            _objProjectTeam.Insert(team);

            return RedirectToAction(ActionName.ProjectMgtIndex, ControllerName.Project);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            ProjectViewModel prjviewmodel = new ProjectViewModel();
            Projects prj = _objProject.FindById(id);

            prjviewmodel.Id = id;
            prjviewmodel.ProjectName = prj.ProjectName;
            prjviewmodel.TechnologyId = prj.TechnologyId;
            prjviewmodel.ProjectTypeId = prj.ProjectTypeId;
            prjviewmodel.ProjectCategoryId = prj.ProjectCategoryId;
            prjviewmodel.ProjectOwner = prj.ProjectOwner;
            prjviewmodel.ProjectGroupId = prj.ProjectGroupId;
            prjviewmodel.InitialEstimatedHrs = prj.InitialEstimatedHrs;
            prjviewmodel.ProjectStatusId = prj.ProjectStatusId;
            prjviewmodel.ProjectManager = prj.ProjectManager;
            prjviewmodel.ProjectLead = prj.ProjectLead;
            prjviewmodel.ProjectCode = prj.ProjectCode;
            prjviewmodel.ShortCode = prj.ShortCode;
            prjviewmodel.CustomerId = prj.CustomerId;
            prjviewmodel.CustomerName = prj.CustomerName;
            prjviewmodel.InitialDeadline = prj.InitialDeadline;
            prjviewmodel.CurrentDeadline = prj.CurrentDeadline;
            prjviewmodel.ProjectCode = prj.ProjectCode;
            prjviewmodel.ProjectBrief = prj.ProjectBrief;
            prjviewmodel.IsNotification = prj.IsNotification;
            prjviewmodel.ModelingEstimatedHrs = prj.ModelingEstimatedHrs;
            prjviewmodel.QuintiqProjectCode = prj.QuintiqProjectCode;
            return View(prjviewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectViewModel prj)
        {
            Projects project = new Projects();
            ProjectTeam team = new ProjectTeam();

            project.Id = prj.Id;
            project.ProjectName = prj.ProjectName;
            project.TechnologyId = prj.TechnologyId;
            project.ProjectTypeId = prj.ProjectTypeId;
            project.ProjectCategoryId = prj.ProjectCategoryId;
            project.ProjectOwner = prj.ProjectOwner;
            project.ProjectGroupId = prj.ProjectGroupId;
            project.InitialEstimatedHrs = prj.InitialEstimatedHrs;
            project.ModelingEstimatedHrs = prj.ModelingEstimatedHrs;
            project.QuintiqProjectCode = prj.QuintiqProjectCode;
            project.ProjectStatusId = prj.ProjectStatusId;
            project.ProjectManager = prj.ProjectManager;
            project.ProjectLead = prj.ProjectLead;
            project.ProjectCode = prj.ProjectCode;
            project.ShortCode = prj.ShortCode;
            project.CustomerId = prj.CustomerId;
            project.CustomerName = prj.CustomerName;
            project.ProjectBrief = prj.ProjectBrief;
            project.InitialDeadline = prj.InitialDeadline;
            project.CurrentDeadline = prj.CurrentDeadline;
            project.IsNotification = prj.IsNotification;
            project.IsProspect = false;

            _objProject.Update(project);

            //ProjectTeam DefaultEntry

            //Owner
            if (_dbContext.ProjectTeam.Where(x => x.ProjectId == prj.Id && x.ProjectRoleId == 1).Count() >= 1)
            {
                int id = _dbContext.ProjectTeam.Where(x => x.ProjectId == prj.Id && x.ProjectRoleId == 1).Select(x => x.Id).SingleOrDefault();
                team.Id = id;
                team.ProjectId = prj.Id;
                team.EmployeeId = prj.ProjectOwner;
                team.ProjectRoleId = 1;
                team.CS_Level = 1;
                team.IsActive = true;
                team.IsCreate = false;
                _objProjectTeam.Update(team);
            }
            if (_dbContext.ProjectTeam.Where(x => x.ProjectId == prj.Id && x.ProjectRoleId == 2).Count() >= 1)
            {
                //Manager
                int id = _dbContext.ProjectTeam.Where(x => x.ProjectId == prj.Id && x.ProjectRoleId == 2).Select(x => x.Id).SingleOrDefault();
                team.Id = id;
                team.ProjectId = prj.Id;
                team.EmployeeId = prj.ProjectManager ?? 0;
                team.ProjectRoleId = 2;
                team.CS_Level = 2;
                team.IsActive = true;
                team.IsCreate = false;
                _objProjectTeam.Update(team);
            }

            if (_dbContext.ProjectTeam.Where(x => x.ProjectId == prj.Id && x.ProjectRoleId == 3).Count() >= 1)
            {
                //Leader
                int id = _dbContext.ProjectTeam.Where(x => x.ProjectId == prj.Id && x.ProjectRoleId == 3).Select(x => x.Id).SingleOrDefault();
                team.Id = id;
                team.ProjectId = prj.Id;
                team.EmployeeId = prj.ProjectLead ?? 0;
                team.ProjectRoleId = 3;
                team.CS_Level = 3;
                team.IsActive = true;
                team.IsCreate = false;
                _objProjectTeam.Update(team);
            }

            return RedirectToAction(ActionName.ProjectMgtEdit, ControllerName.Project);
        }

        /// <summary>
        /// Project Details
        /// </summary>
        /// <returns>Details of Projects</returns>
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View(ViewName.ProjectMgtIndex);
        }

        /// <summary>
        /// Jira detail(Add, Update, Delete ,View)
        /// </summary>
        /// <returns>Jira  view </returns>
        public ActionResult Jira()
        {
            return View(ViewName.ProjectTabForJira);
        }

        /// <summary>
        /// Project CRs detail(Add,View)
        /// </summary>
        /// <returns>Project CRs view </returns>
        public ActionResult ProjectCRs()
        {
            return View(ViewName.ProjectTabForProjectCRs);
        }

        /// <summary>
        /// Project deadline detail(Add, Update, Delete ,View)
        /// </summary>
        /// <returns>Project deadline view </returns>
        public ActionResult ProjectDeadline()
        {
            return View(ViewName.ProjectTabForProjectDeadline);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult ViewProjects([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }

            return Json(GetProjects(id).ToDataSourceResult(request));
        }

        #endregion Action Methods

        #region Public Methods

        public JsonResult GetAllCustomer(string filter)
        {
            return this.Json(GetCustomerName(filter), JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<ProjectViewModel> GetAllProjectCategories()
        {
            return (from prj in _dbContext.ProjectCategory
                    select new ProjectViewModel
                    {
                        ProjectCategoryId = prj.Id,
                        ProjectCategoryName = prj.Name
                    }).ToList();
        }

        public JsonResult GetAllProjectCategory()
        {
            return this.Json(GetAllProjectCategories(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllProjectLeader(string filter)
        {
            return this.Json(GetLeaderName(filter), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllProjectManager(string filter)
        {
            return this.Json(GetManagerName(filter), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllStatus()
        {
            return this.Json(GetAllStatusList(), JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<ProjectViewModel> GetAllStatusList()
        {
            return (from prj in _dbContext.ProjectStatus
                    select new ProjectViewModel
                    {
                        ProjectStatusId = prj.Id,
                        ProjectStatus = prj.Name
                    }).ToList();
        }

        public JsonResult GetAllTechnology()
        {
            return this.Json(GetAllTechnologyName(), JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<ProjectViewModel> GetAllTechnologyName()
        {
            return (from prj in _dbContext.Technology
                    select new ProjectViewModel
                    {
                        TechnologyId = prj.Id,
                        TechnologyName = prj.Name
                    }).ToList();
        }

        public IEnumerable<ProjectViewModel> GetCustomerName(string filter)
        {
            var customerData = (from cust in _dbContext.Customer
                    where cust.IsActive == true
                    select new ProjectViewModel
                    {
                        CustomerId = cust.Id,
                        CustomerName = cust.Name
                    }).ToList().OrderBy(order => order.CustomerName).AsEnumerable();

            if (!string.IsNullOrEmpty(filter))
            {
                customerData = customerData.Where(x => x.CustomerName.Contains(filter));
            }

            return customerData;

        }

        public IEnumerable<ProjectViewModel> GetLeaderName(string filter)
        {
            int[] userid = _dbContext.EmployeeDesignation.Where(p => p.DesignationId != 12).Select(p => p.EmployeeId).ToArray();

            //var query = _dbContext.Employee.Where(p => userid.Contains(p.User_Id ?? 0)).Select(p => p.FiistName).ToList();
            //return (_dbContext.Employee.Where(p => userid.Contains(p.User_Id ?? 0)).Select(p => p.FiistName).ToList());

            var leaderData = (from prj in _dbContext.Employee
                    where userid.Contains(prj.Id)
                    select new ProjectViewModel
                    {
                        ProjectLead = prj.Id,
                        ProjectLeadName = prj.FiistName + " " + prj.LastName
                    }).ToList().OrderBy(order => order.ProjectLeadName).AsEnumerable();

            if (!string.IsNullOrEmpty(filter))
            {
                leaderData = leaderData.Where(x => x.ProjectLeadName.Contains(filter));
            }

            return leaderData;

        }

        public IEnumerable<ProjectViewModel> GetManagerName(string filter)
        {
            int[] userid = _dbContext.EmployeeDesignation.Where(p => p.DesignationId != 12).Select(p => p.EmployeeId).ToArray();

            //var query = _dbContext.Employee.Where(p => userid.Contains(p.User_Id ?? 0)).Select(p => p.FiistName).ToList();
            //return (_dbContext.Employee.Where(p => userid.Contains(p.User_Id ?? 0)).Select(p => p.FiistName).ToList());

            var managerData = (from prj in _dbContext.Employee
                               where userid.Contains(prj.Id)
                               select new ProjectViewModel
                               {
                                   ProjectManager = prj.Id,
                                   ProjectManagerName = prj.FiistName + " " + prj.LastName
                               }).ToList().OrderBy(order => order.ProjectManagerName).AsEnumerable();

            if (!string.IsNullOrEmpty(filter))
            {
                managerData = managerData.Where(x => x.ProjectManagerName.Contains(filter));
            }

            return managerData;
        }

        public JsonResult GetProjectGroup()
        {
            return this.Json(GetProjectGroupName(), JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<ProjectViewModel> GetProjectGroupName()
        {
            return (from groups in _dbContext.ProjectGroup

                    select new ProjectViewModel
                    {
                        ProjectGroupId = groups.Id,
                        ProjectGroupName = groups.Name
                    }).ToList().OrderBy(order => order.ProjectGroupName);
        }

        public JsonResult GetProjectOwner(string filter)
        {
            return this.Json(GetProjectOwnerName(filter), JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<ProjectViewModel> GetProjectOwnerName(string filter)
        {
           
            int[] userid = _dbContext.UserRole.Where(p => p.Roles_Id == 1 || p.Roles_Id == 2 || p.Roles_Id == 9).Select(p => p.Users_Id).ToArray();

            //var query = _dbContext.Employee.Where(p => userid.Contains(p.User_Id ?? 0)).Select(p => p.FiistName).ToList();
            //return (_dbContext.Employee.Where(p => userid.Contains(p.User_Id ?? 0)).Select(p => p.FiistName).ToList());

            var ownerData = (from prj in _dbContext.Employee
                    where userid.Contains(prj.User_Id ?? 0)
                    select new ProjectViewModel
                    {
                        ProjectOwner = prj.Id,
                        ProjectManagerName = prj.FiistName + " " + prj.LastName
                    }).ToList().OrderBy(order => order.ProjectManagerName).AsEnumerable();

            if (!string.IsNullOrEmpty(filter))
            {
                ownerData = ownerData.Where(x => x.ProjectManagerName.Contains(filter));
            }
            return ownerData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProjectViewModel> GetProjects(int id)
        {
            var ProjectDetail = (from prj in _dbContext.Projects
                      .Include("ProjectStatus")
                      .Include("ProjectGroup")
                      .Include("Employee")
                      .Include("Technology")
                                 select new ProjectViewModel
                                {
                                    Id = prj.Id,
                                    ProjectName = prj.ProjectName,
                                    ProjectCode = prj.ProjectCode,
                                    QuintiqProjectCode = prj.QuintiqProjectCode,
                                    CustomerName = prj.CustomerName,
                                    InitialEstimatedHrs = prj.InitialEstimatedHrs,
                                    ProjectStatusId = prj.ProjectStatusId,
                                    ProjectStatus = prj.ProjectStatus.Name,
                                    TechnologyId = prj.TechnologyId,
                                    TechnologyName = prj.Technology.Name,
                                    ProjectManager = prj.ProjectManager,
                                    ProjectManagerName = prj.Employee.FiistName + " " + prj.Employee.LastName,
                                    ProjectLead = prj.ProjectLead,
                                    ProjectLeadName = prj.Employee.FiistName + " " + prj.Employee.LastName,
                                    ProjectGroupId = prj.ProjectGroupId,
                                    ProjectGroupName = prj.ProjectGroup.Name,
                                }).ToList();
            if (id == 0)
            {
                return ProjectDetail.Where(x => x.ProjectStatus == "Running" || x.ProjectStatus == "New").OrderBy(order => order.ProjectName);
            }
            return ProjectDetail.OrderBy(order => order.ProjectName);
        }

        public IEnumerable<ProjectViewModel> GetProjectsType()
        {
            return (from prj in _dbContext.ProjectType
                    select new ProjectViewModel
                    {
                        ProjectTypeId = prj.Id,
                        ProjectTypeName = prj.Name
                    }).ToList();
        }

        //
        //          Methods for Edit Project Page
        //
        public JsonResult GetProjectType()
        {
            return this.Json(GetProjectsType(), JsonRequestBehavior.AllowGet);
        }

        public short GetWeekNo()
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            int weekNum = cul.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return Convert.ToInt16(weekNum);
        }

        public JsonResult ViewProjectsByFilter(int id)
        {
            return this.Json(GetProjects(id), JsonRequestBehavior.AllowGet);
        }

        #endregion Public Methods
    }
}