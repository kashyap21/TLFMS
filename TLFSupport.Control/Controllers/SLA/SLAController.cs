using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TLFSupport.Common;
using TLFSupport.Model;
using TLFSupport.Model.CustomeModel;
using TLFSupport.Model.DatabaseModel;
using TLFSupport.Model.DatabaseModel.Repositories;

namespace TLFSupport.Control.Controllers.SLA
{
    /// <summary>
    /// Handle sla request for given project id
    /// </summary>
    public class SLAController : Controller
    {
        #region Private members
        /// <summary>
        /// Database context
        /// </summary>
        private TLFCSEntities _dbContext = BaseContext.GetDbContext();

        /// <summary>
        ///  Generic repository instance of the SLA
        /// </summary>
        private GenericRepository<CS_SLA> _objSLA;

        /// <summary>
        /// Instance of Project
        /// </summary>
        private GenericRepository<Projects> _objProject;
        #endregion Private members

        #region Constuctor
        /// <summary>
        /// Default constructor
        /// </summary>
        public SLAController()
        {
            this._objProject = new GenericRepository<Projects>();
            this._objSLA = new GenericRepository<CS_SLA>();
        }
        #endregion

        #region Action methods
        /// <summary>
        /// Display SLA data of selected customer
        /// </summary>
        /// <param name="request">Kendo data source</param>
        /// <param name="id">Project id</param>
        /// <returns>Json result of ticket data</returns>
        public ActionResult GetListOfSLA([DataSourceRequest]DataSourceRequest request, int id)
        {
            return Json(GetSLADetails(id).ToDataSourceResult(request));
        }

        /// <summary>
        /// SLA detail
        /// </summary>
        /// <param name="id">Project id</param>
        /// <returns>View of sla data</returns>
        public ActionResult SLADetail(int id)
        {            
            Projects project = _objProject.FindById(id);
            
            TicketViewModel ticketViewModel = new TicketViewModel();
            ticketViewModel.ProjectId = id;
            ticketViewModel.ProjectName = project.ProjectName;
            return View(ticketViewModel);
        }
        #endregion Action methods

        #region Public methods
        /// <summary>
        /// Display Project data
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Json result of project</returns>       
        public JsonResult ViewProject()
        {
            return this.Json(GetListOfProject(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get sla data
        /// </summary>
        /// <param name="id">Project id</param>
        /// <returns>List of sla details</returns>
        public IEnumerable<TicketViewModel> GetSLADetails(int pid)
        {
            return (from sla in _dbContext.CS_SLA.ToList()
                          where sla.ProjectId == pid
                          && sla.IsActive == true
                          select new TicketViewModel
                          {
                              ProjectId = sla.ProjectId,
                              Description = sla.Description,
                              Priority = sla.PriorityName,
                              PriorityLevel = sla.PriorityLevel,
                              IDSLA = sla.SLAId,
                              MaximumResolveTime = Utility.MinuteToString(sla.MaximumResolveTime),
                              MaximumResponseTime = Utility.MinuteToString(sla.MaximumResponseTime)
                          }).ToList().OrderBy(o => o.Priority);           
        }

        /// <summary>
        /// Get project data
        /// </summary>        
        /// <returns>List of ProjectName and its id</returns>
        public IEnumerable<Projects> GetListOfProject()
        {
            return (from project in _objProject.GetAll() select new Projects { Id = project.Id, ProjectName = project.ProjectName}).ToList();
        }
        #endregion Public methods
    }
}