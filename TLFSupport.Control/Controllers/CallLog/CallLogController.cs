using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TLFSupport.Common;
using TLFSupport.Model;
using TLFSupport.Model.CustomeModel;
using TLFSupport.Model.DatabaseModel;
using TLFSupport.Model.DatabaseModel.Repositories;

namespace TLFSupport.Control.Controllers.CallLog
{
    /// <summary>
    /// Controller for calllog actions (View,Insert,Update,Delete)
    /// </summary>
    public class CallLogController : Controller
    {
        #region Private members

        /// <summary>
        /// Instance of the ticket
        /// </summary>
        private GenericRepository<CS_CallLog> _objCallLog;

        private GenericRepository<ProjectTeam> _objProjectTeam;

        /// <summary>
        /// Database context
        /// </summary>
        private TLFCSEntities _dbContext = BaseContext.GetDbContext();

        #endregion Private members

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public CallLogController()
        {
            this._objCallLog = new GenericRepository<CS_CallLog>();
            this._objProjectTeam = new GenericRepository<ProjectTeam>();
        }

        #endregion Constructor

        #region Action methods

        /// <summary>
        /// Ticket Details
        /// </summary>
        /// <returns>Details of tickets</returns>
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View();
        }

        /// <summary>
        /// Display call log data
        /// </summary>
        /// <param name="request"> Kendo data source request</param>
        /// <returns>Json result of ticket data</returns>
        public ActionResult ViewCallLog([DataSourceRequest]DataSourceRequest request)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return Json(GetCallLog().ToDataSourceResult(request));
        }
              
        #endregion Action methods

        #region Public methods

        /// <summary>
        /// Get call log data
        /// </summary>
        /// <returns>List of ticket data</returns>
        public IEnumerable<CallLogViewModel> GetCallLog()
        {
            int[] ProjID = _objProjectTeam.GetAll().Where(c => c.EmployeeId == GetEmployeeId()).Select(v => v.ProjectId).ToArray();
            if (ProjID.Contains(1))
            {
                return (from calllog in _objCallLog.GetAll()

                        select new CallLogViewModel
                   {
                       CallLogId = calllog.CallLogId,
                       ExtensionNumber = GetCustomerNameByExtensionNumber(calllog.ExtensionNumber),
                       CallNumber = calllog.CallNumber,
                       Trunk = calllog.Trunk,
                       Date = calllog.Date,
                       Time = calllog.Time,
                       Duration = calllog.Duration,
                       Action = Convert.ToBoolean(calllog.Action) ? "Attended" : "Missed Call",
                       TicketId = calllog.TicketId,
                       TicketNumber = _dbContext.CS_Ticket.Where(x => x.TicketId == calllog.TicketId).Select(p => p.TicketNumber).SingleOrDefault(),
                       IsTicketGenerated = calllog.IsTicketGenerated
                   }).OrderByDescending(order => order.CallLogId).ToList();
            }
            else
            {
                // Find customer for given projects
                var CustExtension = _dbContext.Projects.Include("Customer").Where(c => ProjID.Contains(c.Id)).Select(c => c.Customer.CS_DedicatedNumber).ToList();

                var sa = (from calllog in _objCallLog.GetAll().Where(x => CustExtension.Contains(x.ExtensionNumber))
                          //where CustExtension.Contains(calllog.ExtensionNumber)
                          select new CallLogViewModel
                          {
                              CallLogId = calllog.CallLogId,
                              ExtensionNumber = GetCustomerNameByExtensionNumber(calllog.ExtensionNumber),
                              CallNumber = calllog.CallNumber,
                              Trunk = calllog.Trunk,
                              Date = calllog.Date,
                              Time = calllog.Time,
                              Duration = calllog.Duration,
                              Action = Convert.ToBoolean(calllog.Action) ? "Attended" : "Missed Call",
                              TicketId = calllog.TicketId,
                              TicketNumber = _dbContext.CS_Ticket.Where(x => x.TicketId == calllog.TicketId).Select(p => p.TicketNumber).SingleOrDefault(),
                              IsTicketGenerated = calllog.IsTicketGenerated
                          }).OrderByDescending(order => order.CallLogId).ToList();
                return sa;
            }
        }
        

        /// <summary>
        /// Method to get customer name form extension number
        /// </summary>
        /// <param name="ExtensionNumber">Extension number</param>
        /// <returns>Customer name</returns>        
        public string GetCustomerNameByExtensionNumber(string ExtensionNumber)
        {
            string CustName = null;
            var cust = (from u in _dbContext.Customer where u.CS_DedicatedNumber.Equals(ExtensionNumber) select u);
            foreach (var item in cust)
            {
                CustName = item.Name;
            }
            return CustName;
        }

        /// <summary>
        /// Get Employee Id from session
        /// </summary>
        /// <returns>EmployeeId</returns>
        public int GetEmployeeId()
        {
            return (Convert.ToInt32(Session["user"] as string));
        }

        #endregion Public methods
    }
}