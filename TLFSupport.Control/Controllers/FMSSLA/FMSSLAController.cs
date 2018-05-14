using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TLFSupport.Common;
using TLFSupport.Model;
using TLFSupport.Model.CustomeModel;
using TLFSupport.Model.DatabaseModel;
using TLFSupport.Model.DatabaseModel.Repositories;

namespace TLFSupport.Control.Controllers.FMSSLA
{
    /// <summary>
    /// Controller for sla actions (View,Insert,Edit)
    /// </summary>
    public class FMSSLAController : Controller
    {
        #region Private members

        /// <summary>
        /// Database context
        /// </summary>
        private TLFCSEntities _dbContext = BaseContext.GetDbContext();

        /// <summary>
        /// Instance of customer
        /// </summary>
        private GenericRepository<Customer> _objCustomer;

        /// <summary>
        /// Instance of Priority
        /// </summary>
        private GenericRepository<CS_Priority> _objPriority;

        /// <summary>
        /// Instance of Project
        /// </summary>
        private GenericRepository<Projects> _objProject;

        /// <summary>
        /// Instance of Sla
        /// </summary>
        private GenericRepository<CS_SLA> _objSla;

        #endregion Private members

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public FMSSLAController()
        {
            this._objCustomer = new GenericRepository<Customer>();
            this._objPriority = new GenericRepository<CS_Priority>();
            this._objProject = new GenericRepository<Projects>();
            this._objSla = new GenericRepository<CS_SLA>();
        }

        #endregion Constructor

        #region Action method

        /// <summary>
        /// SLA creation
        /// </summary>
        /// <returns>create view of sla</returns>
        public ActionResult Create()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View();
        }

        /// <summary>
        /// SLA create for particular project
        /// </summary>
        /// <param name="ViewSLA">Instance of custom slaviewmodel</param>
        /// <returns>SLA created</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SLAViewModel ViewSLA)
        {
            CS_SLA sla = new CS_SLA();

            int count = 0;
            // All priority from priority table
            var priority = _objPriority.GetAll();
            foreach (var item in priority)
            {
                sla.CreatedOn = DateTime.Now;
                sla.LastModified = DateTime.Now;
                sla.MaximumResponseTime = 0;
                sla.MaximumResolveTime = 0;
                sla.PriorityName = item.Name;
                sla.ProjectId = ViewSLA.ProjId;
                sla.PriorityLevel = item.PriorityLevel;
                sla.Description = item.Description;
                sla.EscalationTime = 0;
                sla.IsActive = true;
                if (count == 0)
                {
                    sla.IsDefault = true;
                    count++;
                }
                else
                {
                    sla.IsDefault = false;
                }
                _objSla.Insert(sla);
            }
            return RedirectToAction(ActionName.FMSSLAEdit, ControllerName.FMSSLA, new { id = ViewSLA.ProjId });
        }

        /// <summary>
        /// Update sla data
        /// </summary>
        /// <param name="id">SLA Id</param>
        /// <returns>Redirect to EDIT SLA page</returns>
        public ActionResult Edit(int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            SLAViewModel slaViewModel = new SLAViewModel();

            slaViewModel.ProjId = id;
            //Find project name from ProjectId
            slaViewModel.ProjName = _dbContext.Projects.Where(project => project.Id == slaViewModel.ProjId).Select(projectName => projectName.ProjectName).SingleOrDefault();
            //Find customer id from project table
            slaViewModel.CustId = _dbContext.Projects.Where(project => project.Id == slaViewModel.ProjId).Select(customerId => customerId.CustomerId).SingleOrDefault();
            //Find customer Name from customer table
            slaViewModel.CustName = _dbContext.Customer.Where(customer => customer.Id == slaViewModel.CustId).Select(customerName => customerName.Name).SingleOrDefault();

            return View(slaViewModel);
        }

        [HttpPost]
        public JsonResult ChnageStatusbyAjax(int id)
        {
            CS_SLA Current = _objSla.FindById(id);
            if (Current.IsActive == true)
            {
                Current.IsActive = false;
            }
            else
            {
                Current.IsActive = true;
            }
            _objSla.Update(Current);
            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display Sla data by project id
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Json result of GetSLAByProjectId data</returns>
        public ActionResult GetSLAByProjectId([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return Json(GetListOfSLAById(id).ToDataSourceResult(request));
        }

        /// <summary>
        /// SLA detail
        /// </summary>
        /// <returns>Index view</returns>
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View();
        }

        /// <summary>
        /// Display Sla data
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Json result of sla data</returns>
        public ActionResult ViewSLADetail([DataSourceRequest]DataSourceRequest request)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return Json(GetListOfSLA().ToDataSourceResult(request));
        }

        #endregion Action method

        #region Public methods

        public JsonResult GetCustomer(string filter)
        {
            return this.Json(GetListOfCustomer(filter), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get list of customer id and name
        /// </summary>
        /// <returns>Json result of customer</returns>
        public IEnumerable<Customer> GetListOfCustomer(string filter)
        {
            /////running code
            // return (from customer in _objCustomer.GetAll() select new Customer { Id = customer.Id, Name = customer.Name }).ToList();
            var projid = _dbContext.CS_SLA.Select(x => x.ProjectId).Distinct();
            var cust = _dbContext.Projects.Where(p => !projid.Contains(p.Id)).Where(x => x.CustomerId != null).Select(s => s.CustomerId).ToList();
            var customerData = (from custo in _objCustomer.GetAll() select new Customer { Id = custo.Id, Name = custo.Name }).Where(p => cust.Contains(p.Id)).OrderBy(order => order.Name).AsEnumerable();

            if (!string.IsNullOrEmpty(filter))
            {
                customerData = customerData.Where(x => x.Name.Contains(filter));
            }

            return customerData;
        }

        /// <summary>
        /// Get Project detail
        /// </summary>
        /// <param name="customer">Id of selected customer</param>
        /// <returns>Json result of project</returns>
        public JsonResult GetCascadeProjects(int? customer)
        {
            return this.Json(GetListofProjectsById(customer).Select(p => new { Id = p.Id, ProjectName = p.ProjectName }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get list of project detail
        /// </summary>
        /// <param name="customer">Id of customer</param>
        /// <returns>List of projects id and name </returns>
        public IEnumerable<Projects> GetListofProjectsById(int? customer)
        {
            //var product = _objProject.GetAll().AsQueryable();
            //product = product.Where(p => p.CustomerId == customer);
            //return (from project in product select new Projects { Id = project.Id, ProjectName = project.ProjectName }).ToList();
            var projid = _dbContext.CS_SLA.Select(x => x.ProjectId).Distinct();
            var cust = _dbContext.Projects.Where(p => !projid.Contains(p.Id)).Where(x => x.CustomerId == customer).Select(s => s.Id).ToList();
            return (from proj in _objProject.GetAll() select new Projects { Id = proj.Id, ProjectName = proj.ProjectName }).Where(p => cust.Contains(p.Id)).ToList();
        }

        /// <summary>
        /// Get sla data
        /// </summary>
        /// <returns>List of all sla detail</returns>
        public IEnumerable<SLAViewModel> GetListOfSLA()
        {
            var a = (from data in _dbContext.CS_SLA.Include("Projects")
                     select new SLAViewModel
                     {
                         ProjId = data.ProjectId,
                         PriorityName = data.PriorityName,
                         ProjName = data.Projects.ProjectName,
                         CustId = data.Projects.CustomerId,
                         CustName = data.Projects.Customer.Name,
                         SLAPriorityLevel = data.PriorityLevel,
                         CreatedOn = data.CreatedOn,
                         LastModified = data.LastModified,
                         SLAId = data.SLAId,
                         SLADescription = data.Description,
                         MaximumResolveTime = data.MaximumResolveTime,
                         MaximumResponseTime = data.MaximumResponseTime
                     }).GroupBy(test => test.ProjId).Select(grp => grp.FirstOrDefault()).OrderBy(prj => prj.CustName).ToList();
            return a;
        }

        /// <summary>
        /// Get SLA data
        /// </summary>
        /// <param name="projId">Project Id</param>
        /// <returns>List of SLA details</returns>
        public IEnumerable<SLAViewModel> GetListOfSLAById(int projId)
        {
            var result = (from sla in _dbContext.CS_SLA.ToList()
                          where sla.ProjectId == projId
                          select new SLAViewModel
                          {
                              ProjId = sla.ProjectId,
                              PriorityName = sla.PriorityName,
                              SLADescription = sla.Description,
                              SLAPriorityLevel = sla.PriorityLevel,
                              SLAId = sla.SLAId,
                              CreatedOn = sla.CreatedOn,
                              IsDefaultPriority = sla.IsDefault ?? false,
                              IsActive = sla.IsActive ?? true,
                              tempResolve = Utility.MinuteToString(sla.MaximumResolveTime),
                              tempResponse = Utility.MinuteToString(sla.MaximumResponseTime),
                              tempEscalationTime = Utility.MinuteToString(sla.EscalationTime)
                          }).OrderBy(pri => pri.SLAPriorityLevel).ToList(); ;
            return result;
        }

        /// <summary>
        /// Display SLA data
        /// </summary>
        /// <param name="projId">Project id</param>
        /// <returns>Json result of sla</returns>
        [HttpPost]
        public JsonResult ViewSLAById(int projId)
        {
            return this.Json(GetListOfSLAById(projId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SLASave(int slaId, int prjid, int pl, string pn, string res, string resolved, string escalation, bool isdefault, bool Isactive, string description)
        {
            int a = 0;

            if (isdefaultsExist(prjid, isdefault, slaId))
            {
                a = 1;
            }
            else if (isPriorityNmExist(prjid, slaId, pn))
            {
                a = 2;
            }
            else if (isLevelExist(prjid, slaId, pl))
            {
                a = 3;
            }
            else if (IsValidTime(res))
            {
                a = 4;
            }
            else if (IsValidTime(resolved))
            {
                a = 5;
            }
            else if (IsValidTime(escalation))
            {
                a = 6;
            }
            else
            {
                CS_SLA sla = new CS_SLA();

                sla.LastModified = DateTime.Today;
                sla.MaximumResponseTime = Utility.StringToMinute(res);
                sla.MaximumResolveTime = Utility.StringToMinute(resolved);
                sla.EscalationTime = Utility.StringToMinute(escalation);
                sla.PriorityName = pn;
                sla.IsDefault = isdefault;
                sla.Description = description;
                sla.ProjectId = prjid;
                sla.IsActive = Isactive;
                sla.PriorityLevel = Convert.ToByte(pl);
                if (slaId == 0)
                {
                    sla.CreatedOn = DateTime.Now;

                    _objSla.Insert(sla);
                }
                else
                {
                    sla.SLAId = slaId;
                    sla.CreatedOn = _dbContext.CS_SLA.Where(w => w.SLAId == slaId).Select(s => s.CreatedOn).SingleOrDefault();

                    _objSla.Update(sla);
                }
            }
            return this.Json(a, JsonRequestBehavior.AllowGet);
        }

        #endregion Public methods

        #region Private Method

        /// <summary>
        /// Is default prority already exist or not
        /// </summary>
        /// <param name="id">project id</param>
        /// <param name="sig">Is default priority</param>
        /// <param name="slaid">SlA id id</param>
        /// <returns>True (if default priority already exist ) otherwise false</returns>
        private bool isdefaultsExist(int id, bool sig, int slaid)
        {
            int TrueSLAId = 0;
            if (sig == true)
            {
                if (slaid == 0)
                {
                    TrueSLAId = (from i in _dbContext.CS_SLA
                                 where i.ProjectId == id
                                    && i.IsDefault == true
                                 select i.SLAId).Count();
                }
                else
                {
                    //Find default slaid for given project
                    TrueSLAId = (from i in _dbContext.CS_SLA
                                 where i.ProjectId == id
                                    && i.IsDefault == true
                                    && slaid != i.SLAId
                                 select i.SLAId).Count();
                }
                // Validation fro single default priority
            }
            if (TrueSLAId > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Is prority level already exist or not
        /// </summary>
        /// <param name="id">project id</param>
        /// <param name="slaid">SlA id id</param>
        /// <param name="plevel">Priority level</param>
        /// <returns>>True (if priority level already exist )</returns>
        private bool isLevelExist(int id, int slaid, int plevel)
        {
            bool level = true;
            if (slaid == 0)
            {
                level = (from n in _dbContext.CS_SLA
                         where n.ProjectId == id
                         select n.PriorityLevel).ToList().Contains(Convert.ToByte(plevel));
            }
            else
            {
                level = (from n in _dbContext.CS_SLA
                         where n.ProjectId == id
                         && n.SLAId != slaid
                         select n.PriorityLevel).ToList().Contains(Convert.ToByte(plevel));
            }
            return level;
        }

        /// <summary>
        /// Is prority level already exist or not
        /// </summary>
        /// <param name="id">project id</param>
        /// <param name="slaid">SlA id id</param>
        /// <param name="pn">Priority name</param>
        /// <returns>>True (if priority name already exist )</returns>
        private bool isPriorityNmExist(int id, int slaid, string pn)
        {
            bool a = true;
            if (slaid == 0)
            {
                a = (from n in _dbContext.CS_SLA
                     where n.ProjectId == id
                     select
                           n.PriorityName.ToLower()).ToList().Contains(pn.ToLower());
            }
            else
            {
                a = (from n in _dbContext.CS_SLA
                     where n.ProjectId == id
                        && n.SLAId != slaid
                     select
                            n.PriorityName.ToLower()).ToList().Contains(pn.ToLower());
            }
            return a;
        }

        /// <summary>
        /// Checks whether time is valid or not
        /// </summary>
        /// <param name="time">Time in form D:H:M:</param>
        /// <returns></returns>
        private bool IsValidTime(string time)
        {
            time = Regex.Replace(time, "[^0-9_]", "");
            time = Regex.Replace(time, "_", "0");

            int hour = Convert.ToInt32(time.Substring(2, 2));
            int minute = Convert.ToInt32(time.Substring(4, 2));

            if (hour > 23
                || minute > 59)
            {
                return true;
            }
            return false;
        }

        #endregion Private Method
    }
}