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

namespace TLFSupport.Control.Controllers.ProjectsTeam
{
    public class ProjectsTeamController : Controller
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
        public ProjectsTeamController()
        {
            this._dbContext = BaseContext.GetDbContext();
            this._objProjectTeam = new GenericRepository<ProjectTeam>();
            this._objProject = new GenericRepository<Projects>();
        }

        #endregion Public Constructors

        #region ActionMethods

        /// <summary>
        /// ProjectTeam Details
        /// </summary>
        /// <returns>Details of Projects Team</returns>
        public ActionResult Index([DataSourceRequest]DataSourceRequest request, int id)
        {
            //{
            //    return Json(GetProjectTeamName(id).ToDataSourceResult(request));
            //}

            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View(new ProjectTeamsViewModel(id));
        }

        /// <summary>
        /// Display Project Team data
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Json result of Project Team data</returns>
        public ActionResult ViewProjectsTeamDetails([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return Json(GetListOfProjectTeam(id).ToDataSourceResult(request));
        }

        // <summary>
        /// Deleting  for project Team
        /// </summary>
        /// <param name="request">Kendo data source request</param>
        /// <param name="sla">Instance of ProjectTeam model</param>
        /// <returns>Json for Project Team</returns>

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTeamDestroy([DataSourceRequest] DataSourceRequest request, ProjectTeamsViewModel prjview)
        {
            if (prjview != null)
            {
                //Find SLA id
                var prjTeamId = (from data in _dbContext.ProjectTeam
                                 where data.Id == prjview.Id
                                 select data.Id).FirstOrDefault();

                _objProjectTeam.DeleteById(prjTeamId);
            }
            return Json(new[] { prjview }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// Updating SLA for project
        /// </summary>
        /// <param name="request"></param>
        /// <param name="sla">Instance of slaview model</param>
        /// <returns>Updated Json data</returns>
        public ActionResult InlineEditProjectTeam([DataSourceRequest] DataSourceRequest request, ProjectTeamsViewModel prjview)
        {
            ProjectTeam prjteam = new ProjectTeam();
            prjteam.Id = prjview.Id;
            prjteam.IsActive = prjview.IsActive;
            prjteam.IsCreate = prjview.IsCreate;
            prjteam.ProjectId = prjview.ProjectId;
            prjteam.ProjectRoleId = prjview.ProjectRoleId;

            prjteam.CS_Level = prjview.CS_Level;
            prjteam.EmployeeId = prjview.EmployeeId;

            _objProjectTeam.Update(prjteam);

            //_objSla.Update(sla);
            return Json(new[] { prjview }.ToDataSourceResult(request, ModelState));
        }

        #endregion ActionMethods

        /// <summary>
        /// ProjectTeam TabStrip ProjectTeam Add
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <param name="empid">EmployeeId</param>
        /// <param name="ProjectRId">ProjectTeam Member Role Id</param>
        /// <param name="empLevel">Level Of Employee in Team</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddTeambyAjax(int id, int empid, int ProjectRId, int empLevel)
        {
            ProjectTeam team = new ProjectTeam();
            team.ProjectId = id;
            team.EmployeeId = empid;
            team.ProjectRoleId = ProjectRId;
            team.CS_Level = Convert.ToByte(empLevel);
            team.IsActive = true;
            team.IsCreate = false;

            _objProjectTeam.Insert(team);

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChnageStatusbyAjax(int id)
        {
            ProjectTeam Active = _objProjectTeam.FindById(id);
            if (Active.IsActive == true)
            {
                Active.IsActive = false;
            }
            else
            {
                Active.IsActive = true;
            }
            _objProjectTeam.Update(Active);
            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display Project employee
        /// </summary>
        /// <returns></returns>
        public JsonResult ViewEmployee(int prjId)
        {
            return this.Json(GetEmployeeName(prjId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display ProjectRole data
        /// </summary>
        /// <returns>Json result of country data</returns>
        public JsonResult ViewProjectRole()
        {
            return this.Json(GetRoleName(), JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<ProjectTeamsViewModel> GetEmployeeName(int prjId)
        {
            int[] userid = _dbContext.EmployeeDesignation.Where(p => p.DesignationId != 12).Select(p => p.EmployeeId).ToArray();
            int[] AlreadyEmp = _dbContext.ProjectTeam.Where(p => p.ProjectId == prjId).Select(p => p.EmployeeId).ToArray();

            return (from prj in _dbContext.Employee
                    where userid.Contains(prj.Id) && prj.IsActive == true && !AlreadyEmp.Contains(prj.Id)
                    select new ProjectTeamsViewModel
                    {
                        EmployeeId = prj.Id,
                        EmployeeName = prj.FiistName + " " + prj.LastName
                    }).ToList().OrderBy(order => order.EmployeeName);
        }

        public IEnumerable<ProjectTeamsViewModel> GetRoleName()
        {
            return (from role in _dbContext.ProjectRole
                    where role.Id != 1 && role.Id != 2 && role.Id != 3
                    select new ProjectTeamsViewModel
                    {
                        ProjectRoleId = role.Id,
                        ProjectRoleName = role.Name
                    }).ToList().OrderBy(order => order.ProjectRoleName);
        }

        /// <summary>
        /// Get sla data
        /// </summary>
        /// <returns>List of all sla detail</returns>
        public IEnumerable<ProjectTeamsViewModel> GetListOfProjectTeam(int projId)
        {
            return (from data in _dbContext.ProjectTeam
                    where data.ProjectId == projId
                    select new ProjectTeamsViewModel
                    {
                        CS_Level = data.CS_Level,
                        EmployeeId = data.EmployeeId,
                        EmployeeName = data.Employee.LastName + " " + data.Employee.FiistName,
                        Id = data.Id,
                        IsActive = data.IsActive,
                        IsCreate = data.IsCreate,
                        ProjectId = data.ProjectId,
                        ProjectRoleId = data.ProjectRoleId,
                        ProjectRoleName = data.ProjectRole.Name,
                    }
                    ).ToList();
        }
    }
}