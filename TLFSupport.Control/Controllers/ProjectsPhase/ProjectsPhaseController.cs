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

namespace TLFSupport.Control.Controllers.ProjectsPhase
{
    public class ProjectsPhaseController : Controller
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
        private GenericRepository<ProjectPhases> _objProjectPhase;

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
        public ProjectsPhaseController()
        {
            this._dbContext = BaseContext.GetDbContext();
            this._objProjectPhase = new GenericRepository<ProjectPhases>();
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
            return View(new ProjectPhaseViewModel(id));
        }

        /// <summary>
        /// Display Project Team data
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Json result of Project Team data</returns>
        public ActionResult ViewProjectsPhaseDetails([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return Json(GetListOfProjectPhase(id).ToDataSourceResult(request));
        }

        /// Deleting  for project phase
        /// </summary>
        /// <param name="request">Kendo data source request</param>
        /// <param name="sla">Instance of ProjectTeam model</param>
        /// <returns>Json for Project phase</returns>

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectPhaseDestroy([DataSourceRequest] DataSourceRequest request, ProjectPhaseViewModel phaseview)
        {
            if (phaseview != null)
            {
                var prjTeamId = (from data in _dbContext.ProjectPhases
                                 where data.Id == phaseview.Id
                                 select data.Id).FirstOrDefault();

                _objProjectPhase.DeleteById(prjTeamId);
            }
            return Json(new[] { phaseview }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <param name="phasename">Project Phase Name</param>
        /// <param name="isActive"></param>
        /// <param name="isCurrent"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddPhasebyAjax(int id, string phasename, bool isActive, bool isCurrent)
        {
            ProjectPhases phase = new ProjectPhases();
            phase.PhaseName = phasename;
            phase.IsActive = isActive;
            phase.IsCurrent = isCurrent;
            phase.CreatedOn = DateTime.Now;
            phase.ProjectId = id;
            phase.CreatedBy = Convert.ToInt32(Session["userId"] as string);
            _objProjectPhase.Insert(phase);

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <param name="phasename">Project Phase Name</param>
        /// <param name="isActive"></param>
        /// <param name="isCurrent"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdatePhasebyAjax(int id, string phasename, bool isActive, bool isCurrent, int UId)
        {
            ProjectPhases phase = new ProjectPhases();

            phase.Id = UId;
            phase.PhaseName = phasename;
            phase.IsActive = isActive;
            phase.IsCurrent = isCurrent;
            phase.CreatedBy = _dbContext.ProjectPhases.Where(c => c.Id == UId).Select(x => x.CreatedBy).SingleOrDefault();
            phase.CreatedOn = _dbContext.ProjectPhases.Where(c => c.Id == UId).Select(x => x.CreatedOn).SingleOrDefault();
            phase.UpdatedOn = DateTime.Now;
            phase.ProjectId = id;
            phase.UpdateBy = Convert.ToInt32(Session["userId"] as string);
            phase.UpdatedIPAddress = Convert.ToString(IpAddr);

            _objProjectPhase.Update(phase);

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChnageStatusbyAjax(int id)
        {
            ProjectPhases Current = _objProjectPhase.FindById(id);
            if (Current.IsCurrent == true)
            {
                Current.IsCurrent = false;
            }
            else
            {
                Current.IsCurrent = true;
            }
            _objProjectPhase.Update(Current);
            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get sla data
        /// </summary>
        /// <returns>List of all sla detail</returns>
        public IEnumerable<ProjectPhaseViewModel> GetListOfProjectPhase(int projId)
        {
            return (from data in _dbContext.ProjectPhases
                    where data.ProjectId == projId
                    select new ProjectPhaseViewModel
                    {
                        Id = data.Id,
                        PhaseId = data.PhaseId,
                        PhaseName = data.PhaseName,
                        IsActive = (bool)data.IsActive,
                        IsCurrent = data.IsCurrent,
                    }
                    ).ToList();
        }
    }
}