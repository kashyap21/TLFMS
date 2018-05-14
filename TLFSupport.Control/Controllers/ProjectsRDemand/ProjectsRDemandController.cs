using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TLFSupport.Common;
using TLFSupport.Model;
using TLFSupport.Model.CustomeModel;
using TLFSupport.Model.DatabaseModel;
using TLFSupport.Model.DatabaseModel.Repositories;

namespace TLFSupport.Control.Controllers.ProjectsRDemand
{
    public class ProjectsRDemandController : Controller
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
        private GenericRepository<ProjectResourceDemand> _objProjectDemand;

        #endregion Private members

        #region Public Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProjectsRDemandController()
        {
            this._dbContext = BaseContext.GetDbContext();
            this._objProjectDemand = new GenericRepository<ProjectResourceDemand>();
            this._objProject = new GenericRepository<Projects>();
        }

        #endregion Public Constructors

        /// <returns></returns>
        public ActionResult Index([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View("~/Views/ProjectsRDemand/Index.cshtml", new ProjectDemandViewModel(id));
        }

        #region ActionMethods

        ///// <summary>
        ///// ProjectTeam Details
        ///// </summary>
        ///// <returns>Details of Projects Team</returns>
        //public ActionResult Index([DataSourceRequest]DataSourceRequest request, int id)
        //{
        //    //{
        //    //    return Json(GetProjectTeamName(id).ToDataSourceResult(request));
        //    //}

        //    if (Session["user"] == null)
        //    {
        //        return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
        //    }
        //    return View(new ProjectDemandViewModel(id));
        //}

        /// <summary>
        /// Display Project Team data
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Json result of Project Team data</returns>
        public ActionResult ViewProjectsDemandDetails([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return Json(GetListOfProjectDemand(id).ToDataSourceResult(request));
        }

        // <summary>
        /// Deleting  for project Team
        /// </summary>
        /// <param name="request">Kendo data source request</param>
        /// <param name="sla">Instance of ProjectTeam model</param>
        /// <returns>Json for Project Team</returns>

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectDemandDestroy([DataSourceRequest] DataSourceRequest request, ProjectResourceDemand prjDemand)
        {
            if (prjDemand != null)
            {
                //Find SLA id
                var prjDemandId = (from data in _dbContext.ProjectResourceDemand
                                   where data.Id == prjDemand.Id
                                   select data.Id).FirstOrDefault();

                _objProjectDemand.DeleteById(prjDemandId);
            }
            return Json(new[] { prjDemand }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// Updating SLA for project
        /// </summary>
        /// <param name="request"></param>
        /// <param name="sla">Instance of slaview model</param>
        /// <returns>Updated Json data</returns>
        public ActionResult InlineEditProjectDemand([DataSourceRequest] DataSourceRequest request, ProjectResourceDemand prjDemandview)
        {
            ProjectResourceDemand prjdemand = new ProjectResourceDemand();
            prjdemand.Id = prjDemandview.Id;
            prjdemand.ProjectId = prjDemandview.ProjectId;
            prjdemand.ProjectRoleId = prjDemandview.ProjectRoleId;
            prjdemand.ExpectedStartWeek = prjDemandview.ExpectedStartWeek;
            prjdemand.ExpectedHoursPerWeek = prjDemandview.ExpectedHoursPerWeek;
            prjdemand.Year = _dbContext.ProjectResourceDemand.Where(y => y.Id == prjDemandview.Id).Select(c => c.Year).SingleOrDefault();

            _objProjectDemand.Update(prjdemand);

            return Json(new[] { prjDemandview }.ToDataSourceResult(request, ModelState));
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
        public JsonResult AddDemandbyAjax(int id, int esw, int ProjectRId, int ehw)
        {
            ProjectResourceDemand demand = new ProjectResourceDemand();

            demand.ProjectId = id;
            demand.ProjectRoleId = ProjectRId;
            demand.ExpectedStartWeek = esw;
            demand.ExpectedHoursPerWeek = ehw;
            demand.Year = DateTime.Now.Year;
            _objProjectDemand.Insert(demand);

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display ProjectRole data
        /// </summary>
        /// <returns>Json result of country data</returns>
        public JsonResult ViewProjectRole()
        {
            return this.Json(GetRoleName(), JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<ProjectDemandViewModel> GetRoleName()
        {
            return (from role in _dbContext.ProjectRole
                    where role.Id != 1 && role.Id != 2 && role.Id != 3
                    select new ProjectDemandViewModel
                    {
                        ProjectRoleId = role.Id,
                        ProjectRoleName = role.Name
                    }).ToList().OrderBy(order => order.ProjectRoleName);
        }

        /// <summary>
        /// Get sla data
        /// </summary>
        /// <returns>List of all sla detail</returns>
        public IEnumerable<ProjectDemandViewModel> GetListOfProjectDemand(int projId)
        {
            return (from data in _dbContext.ProjectResourceDemand
                    where data.ProjectId == projId
                    select new ProjectDemandViewModel
                    {
                        Id = data.Id,
                        ExpectedHoursPerWeek = data.ExpectedHoursPerWeek,
                        ExpectedStartWeek = data.ExpectedStartWeek,
                        ProjectId = data.ProjectId,
                        ProjectRoleId = data.ProjectRoleId,
                        ProjectRoleName = data.ProjectRole.Name,
                    }
                    ).ToList();
        }
    }
}