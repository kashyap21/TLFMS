using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TLFSupport.Model;
using TLFSupport.Model.DatabaseModel;

namespace TLFSupport.Control.Controllers.Common
{
    /// <summary>
    /// Helper class for list of common methods
    /// </summary>
    public class CommonController : BaseController
    {
        #region Private members

        /// <summary>
        /// Database context
        /// </summary>
        private TLFCSEntities _dbContext;

        #endregion Private members

        #region Contructor

        /// <summary>
        /// Default Contructor
        /// </summary>
        public CommonController()
        {
            _dbContext = BaseContext.GetDbContext();
        }

        #endregion Contructor

        #region Action methods

        /// <summary>
        /// Display status data
        /// </summary>
        /// <returns>Json result of status</returns>
        public ActionResult GetStatusForFiltering()
        {
            return Json(_dbContext.CS_Status.Select(s => s.Status), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display Projects data
        /// </summary>
        /// <returns>Json result of Project</returns>
        public ActionResult GetProjectsForFiltering()
        {
            return Json(_dbContext.Projects.Select(p => p.ProjectName).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display Employee data
        /// </summary>
        /// <returns>Json result of employee</returns>
        public ActionResult GetEmployeeForFiltering()
        {
            return Json(_dbContext.Employee.Select(e => e.FiistName + " " + e.LastName).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display Ticket number
        /// </summary>
        /// <returns>Json result of ticket number</returns>
        public ActionResult GetTicketnumberForFilter()
        {
            return Json(_dbContext.CS_Ticket.Select(t => t.TicketNumber).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display Ticket title
        /// </summary>
        /// <returns>Json result of ticket title</returns>
        public ActionResult GetTicketTitleForFiltering()
        {
            return Json(_dbContext.CS_Ticket.Select(t => t.Title).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display Priority data
        /// </summary>
        /// <returns>Json result of Priority</returns>
        public ActionResult GetPriorityForFiltering()
        {
            return Json(_dbContext.CS_SLA.Select(s => s.PriorityName).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display Customer data
        /// </summary>
        /// <returns>Json result of customer name</returns>
        public ActionResult GetCustomerNameForFiltering()
        {
            return Json(_dbContext.Customer.Select(c => c.Name).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display status data
        /// </summary>
        /// <returns>Json result of status</returns>
        public ActionResult prjProjectStatus()
        {
            return Json(_dbContext.ProjectStatus.Select(s => s.Name), JsonRequestBehavior.AllowGet);
        }

        public ActionResult prjProjectManager()
        {
            return Json(_dbContext.Employee.Select(s => s.FiistName + " " + s.LastName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult prjProjectSLead()
        {
            return Json(_dbContext.Employee.Select(s => s.FiistName + " " + s.LastName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult prjProjectCustomer()
        {
            return Json(_dbContext.Customer.Select(s => s.Name), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportExcel(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        #endregion Action methods


    }
}