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

namespace TLFSupport.Control.Controllers.ProjectsCRs
{
    public class ProjectsCRsController : Controller
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
        private GenericRepository<ProjectCR> _objProjectCRs;

        #endregion Private members

        #region Public Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProjectsCRsController()
        {
            this._dbContext = BaseContext.GetDbContext();
            this._objProjectCRs = new GenericRepository<ProjectCR>();
            this._objProject = new GenericRepository<Projects>();
        }

        #endregion Public Constructors

        /// <summary>
        /// Index for the Project CRs
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View(new ProjectCRViewModel(id));
        }

        /// <summary>
        /// Display Project Jira data
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Json result of Project deadline data</returns>
        public ActionResult ViewProjectsCRsDetails([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return Json(GetListOfProjectCRs(id).ToDataSourceResult(request));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="prjJcr"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectCRsDestroy([DataSourceRequest] DataSourceRequest request, ProjectJiraViewModel prjJcr)
        {
            if (prjJcr != null)
            {
                var crId = (from data in _dbContext.ProjectCR
                            where data.CRId == prjJcr.Id
                            select data.CRId).FirstOrDefault();

                _objProjectCRs.DeleteById(crId);
            }
            return Json(new[] { prjJcr }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// Display Project Approval data
        /// </summary>
        /// <returns></returns>
        public JsonResult ViewProjectApprover(int Id)
        {
            return this.Json(GetProjectApprover(Id), JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<ProjectCRViewModel> GetProjectApprover(int id)
        {
            Projects apr = _objProject.FindById(id);

            return (from approve in _dbContext.Employee
                    where approve.Id == apr.ProjectOwner
                    select new ProjectCRViewModel
                    {
                        ApprovedBy = approve.Id,
                        ApprovedByName = approve.FiistName + " " + approve.LastName,
                    }).ToList();
        }

        /// <summary>
        /// Get CR data
        /// </summary>
        /// <returns>List of all sla detail</returns>
        public IEnumerable<ProjectCRViewModel> GetListOfProjectCRs(int projId)
        {
            return (from data in _dbContext.ProjectCR
                    where data.ProjectId == projId
                    select new ProjectCRViewModel
                    {
                        CRId = data.CRId,
                        CRNo = data.CRNo,
                        Description = data.Description,
                        EstimatedHrs = data.EstimatedHrs,
                        CRDate = data.CRDate,
                        ApprovedBy = data.ApprovedBy,
                        Status = data.Status,
                    }
                    ).ToList();
        }
    }
}