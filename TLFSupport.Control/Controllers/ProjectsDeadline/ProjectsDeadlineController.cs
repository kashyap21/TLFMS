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

namespace TLFSupport.Control.Controllers.ProjectsDeadline
{
    public class ProjectsDeadlineController : Controller
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
        /// Instance of ProjectDeadline
        /// </summary>
        private GenericRepository<ProjectDeadline> _objProjectDeadline;

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
        public ProjectsDeadlineController()
        {
            this._dbContext = BaseContext.GetDbContext();
            this._objProjectDeadline = new GenericRepository<ProjectDeadline>();
            this._objProject = new GenericRepository<Projects>();
        }

        #endregion Public Constructors

        /// <summary>
        /// Project deadline details
        /// </summary>
        /// <returns>Details of Projects deadline</returns>
        public ActionResult Index([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View(new ProjectDeadlineViewModel(id));
        }

        /// <summary>
        /// Display Project deadline data
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Json result of Project deadline data</returns>
        public ActionResult ViewProjectsDeadlineDetails([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return Json(GetListOfProjectDeadline(id).ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectDeadlineDestroy([DataSourceRequest] DataSourceRequest request, ProjectDeadlineViewModel prjDeadlineview)
        {
            if (prjDeadlineview != null)
            {
                var prjDeadlineId = (from data in _dbContext.ProjectDeadline
                                     where data.Id == prjDeadlineview.Id
                                     select data.Id).FirstOrDefault();

                _objProjectDeadline.DeleteById(prjDeadlineId);
            }
            return Json(new[] { prjDeadlineview }.ToDataSourceResult(request, ModelState));
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
        public JsonResult UpdateDeadLinebyAjax(int id, DateTime revisedate, string reason, int UId)
        {
            ProjectDeadline deadline = new ProjectDeadline();
            deadline.Id = UId;
            deadline.ProjectId = id;
            deadline.Reason = reason;
            deadline.RevisedDate = revisedate;
            deadline.CreatedBy = Convert.ToInt32(Session["userId"] as string);
            deadline.UpdatedOn = DateTime.Now;

            _objProjectDeadline.Update(deadline);

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get sla data
        /// </summary>
        /// <returns>List of all sla detail</returns>
        public IEnumerable<ProjectDeadlineViewModel> GetListOfProjectDeadline(int projId)
        {
            return (from data in _dbContext.ProjectDeadline
                    where data.ProjectId == projId
                    select new ProjectDeadlineViewModel
                    {
                        Id = data.Id,
                        RevisedDate = data.RevisedDate,
                        CreatedBy = data.CreatedBy,
                        CreatedByName = data.Employee.FiistName + " " + data.Employee.LastName,
                        UpdatedOn = data.UpdatedOn,
                        Reason = data.Reason,
                        ProjectId = data.ProjectId,
                        ProjectName = data.Projects.ProjectName,
                    }
                    ).ToList();
        }

        [HttpPost]
        public JsonResult AddDeadlinebyAjax(int id, DateTime revisedate, string reason)
        {
            ProjectDeadline deadline = new ProjectDeadline();
            deadline.ProjectId = id;
            deadline.Reason = reason;
            deadline.RevisedDate = revisedate;
            deadline.CreatedBy = Convert.ToInt32(Session["userId"] as string);
            deadline.UpdatedOn = DateTime.Now;

            _objProjectDeadline.Insert(deadline);

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}