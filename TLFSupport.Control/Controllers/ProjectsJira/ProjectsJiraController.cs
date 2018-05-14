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

namespace TLFSupport.Control.Controllers.ProjectsJira
{
    public class ProjectsJiraController : Controller
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
        /// Instance of ProjectJira
        /// </summary>
        private GenericRepository<TlfmsJiraLink> _objProjectJira;

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
        public ProjectsJiraController()
        {
            this._dbContext = BaseContext.GetDbContext();
            this._objProjectJira = new GenericRepository<TlfmsJiraLink>();
            this._objProject = new GenericRepository<Projects>();
        }

        #endregion Public Constructors

        /// <summary>
        /// Index for the Project Jira
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View(new ProjectJiraViewModel(id));
        }

        /// <summary>
        /// Display Project Jira data
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Json result of Project deadline data</returns>
        public ActionResult ViewProjectsJiraDetails([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return Json(GetListOfProjectJira(id).ToDataSourceResult(request));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="prjJira"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectJiraDestroy([DataSourceRequest] DataSourceRequest request, ProjectJiraViewModel prjJira)
        {
            if (prjJira != null)
            {
                var jiraId = (from data in _dbContext.TlfmsJiraLink
                              where data.Id == prjJira.Id
                              select data.Id).FirstOrDefault();

                _objProjectJira.DeleteById(jiraId);
            }
            return Json(new[] { prjJira }.ToDataSourceResult(request, ModelState));
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
        public JsonResult UpdateJirabyAjax(int id, string name, string key, string version, int UId)
        {
            TlfmsJiraLink jiratbl = new TlfmsJiraLink();
            jiratbl.Id = UId;
            jiratbl.TlfmsProjectId = id;
            jiratbl.JiraProjectName = name;
            jiratbl.JiraProjectVerison = version;
            jiratbl.JiraProjectKey = key;
            jiratbl.CreatedBy = _dbContext.TlfmsJiraLink.Where(i => i.Id == UId).Select(create => create.CreatedBy).SingleOrDefault();
            jiratbl.CreatedOn = _dbContext.TlfmsJiraLink.Where(i => i.Id == UId).Select(create => create.CreatedOn).SingleOrDefault();
            jiratbl.UpdatedBy = Convert.ToInt32(Session["userId"] as string);
            jiratbl.UpdatedOn = DateTime.Now;

            _objProjectJira.Update(jiratbl);

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Jira data
        /// </summary>
        /// <returns>List of all sla detail</returns>
        public IEnumerable<ProjectJiraViewModel> GetListOfProjectJira(int projId)
        {
            return (from data in _dbContext.TlfmsJiraLink
                    where data.TlfmsProjectId == projId
                    select new ProjectJiraViewModel
                    {
                        Id = data.Id,
                        JiraProjectName = data.JiraProjectName,
                        JiraProjectKey = data.JiraProjectVerison,
                        JiraProjectVerison = data.JiraProjectVerison,
                    }
                    ).ToList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="revisedate"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddJirabyAjax(int id, string name, string key, string version)
        {
            TlfmsJiraLink jiraAdd = new TlfmsJiraLink();
            jiraAdd.TlfmsProjectId = id;
            jiraAdd.JiraProjectName = name;
            jiraAdd.JiraProjectKey = key;
            jiraAdd.JiraProjectVerison = version;
            jiraAdd.CreatedBy = Convert.ToInt32(Session["userId"] as string);
            jiraAdd.CreatedOn = DateTime.Now;

            _objProjectJira.Insert(jiraAdd);

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}