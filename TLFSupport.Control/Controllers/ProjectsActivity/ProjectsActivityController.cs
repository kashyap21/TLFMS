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

namespace TLFSupport.Control.Controllers.ProjectsActivity
{
    public class ProjectsActivityController : Controller
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
        private GenericRepository<ProjectActivityLink> _objProjectActivity;

        /// <summary>
        /// Host address information
        /// </summary>
        public static IPHostEntry IpHost = Dns.GetHostEntry(Dns.GetHostName());

        /// <summary>
        /// IP address of local host
        /// </summary>
        public static IPAddress IpAddr = IpHost.AddressList[1];

        #endregion Private members

        #region Public Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProjectsActivityController()
        {
            this._dbContext = BaseContext.GetDbContext();
            this._objProjectActivity = new GenericRepository<ProjectActivityLink>();
            this._objProject = new GenericRepository<Projects>();
        }

        #endregion Public Constructors

        /// <summary>
        /// ProjectTeam Details
        /// </summary>
        /// <returns>Details of Projects Team</returns>
        public ActionResult Index([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View(new ProjectActivityViewModel(id));
        }

        /// <summary>
        /// Display Project Team data
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Json result of Project Team data</returns>
        public ActionResult ViewProjectsActivityDetails([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return Json(GetListOfProjectActivity(id).ToDataSourceResult(request));
        }

        /// Deleting  for project phase
        /// </summary>
        /// <param name="request">Kendo data source request</param>
        /// <param name="sla">Instance of ProjectTeam model</param>
        /// <returns>Json for Project phase</returns>

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectActivityDestroy([DataSourceRequest] DataSourceRequest request, ProjectActivityViewModel prjActview)
        {
            if (prjActview != null)
            {
                var prjActivityId = (from data in _dbContext.ProjectActivityLink
                                     where data.Id == prjActview.Id
                                     select data.Id).FirstOrDefault();

                _objProjectActivity.DeleteById(prjActivityId);
            }
            return Json(new[] { prjActview }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <param name="activityname">Project Phase Name</param>
        /// <param name="isActive"></param>
        /// <param name="isCurrent"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddActivitybyAjax(int id, string activityname, bool isActive)
        {
            ProjectActivityLink activity = new ProjectActivityLink();
            activity.ProjectActivityName = activityname;
            activity.IsActive = isActive;
            activity.CreatedOn = DateTime.Now;
            activity.ProjectId = id;
            activity.CreatedBy = Convert.ToInt32(Session["userId"] as string);
            _objProjectActivity.Insert(activity);

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <param name="activityname">Project Phase Name</param>
        /// <param name="isActive"></param>
        /// <param name="isCurrent"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateActivitybyAjax(int id, string activityname, bool isActive, int UId)
        {
            ProjectActivityLink phase = new ProjectActivityLink();

            phase.Id = UId;
            phase.ProjectActivityName = activityname;
            phase.IsActive = isActive;
            phase.CreatedBy = _dbContext.ProjectActivityLink.Where(c => c.Id == UId).Select(x => x.CreatedBy).SingleOrDefault();
            phase.CreatedOn = _dbContext.ProjectActivityLink.Where(c => c.Id == UId).Select(x => x.CreatedOn).SingleOrDefault();
            phase.UpdatedOn = DateTime.Now;
            phase.ProjectId = id;
            phase.UpdatedBy = Convert.ToInt32(Session["userId"] as string);
            phase.UpdatedIPAddress = Convert.ToString(IpAddr);

            _objProjectActivity.Update(phase);

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get sla data
        /// </summary>
        /// <returns>List of all sla detail</returns>
        public IEnumerable<ProjectActivityViewModel> GetListOfProjectActivity(int projId)
        {
            return (from data in _dbContext.ProjectActivityLink
                    where data.ProjectId == projId
                    select new ProjectActivityViewModel
                    {
                        Id = data.Id,
                        ProjectActivityName = data.ProjectActivityName,
                        IsActive = data.IsActive,
                    }
                    ).ToList();
        }
    }
}