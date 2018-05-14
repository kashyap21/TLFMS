using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using TLFSupport.Common;
using TLFSupport.Model;
using TLFSupport.Model.CustomeModel;
using TLFSupport.Model.DatabaseModel;
using TLFSupport.Model.DatabaseModel.Repositories;

namespace TLFSupport.Control.Controllers.Ticket
{
    /// <summary>
    /// Controller for ticket actions (View,Insert,Update,Delete)
    /// </summary>
    public class TicketController : BaseController
    {
        #region Private members

        /// <summary>
        /// Database context
        /// </summary>
        private TLFCSEntities _dbContext;

        /// <summary>
        /// Instance of the ticket
        /// </summary>
        private GenericRepository<CS_Ticket> _objTicket;

        /// <summary>
        /// Instance of customer
        /// </summary>
        private GenericRepository<Customer> _objCustomer;

        /// <summary>
        /// Instance of Project
        /// </summary>
        private GenericRepository<Projects> _objProject;

        /// <summary>
        /// Instance Of SLA
        /// </summary>
        private GenericRepository<CS_SLA> _objSLA;

        /// <summary>
        /// Instance of CallLog
        /// </summary>
        private GenericRepository<CS_CallLog> _objCallLog;

        /// <summary>
        /// Instance of Priority
        /// </summary>
        private GenericRepository<CS_Priority> _objPriority;

        /// <summary>
        /// Instance of Employee
        /// </summary>
        private GenericRepository<Employee> _objEmployee;

        /// <summary>
        /// Instance of ProjectTeam
        /// </summary>
        private GenericRepository<ProjectTeam> _objProjectTeam;

        /// <summary>
        /// Instance of Status
        /// </summary>
        private GenericRepository<CS_Status> _objStatus;

        /// <summary>
        /// Instance of Log table
        /// </summary>
        private GenericRepository<CS_Log> _objLog;

        /// <summary>
        /// Instance of Log table
        /// </summary>
        private GenericRepository<CS_EmailLink> _objLink;

        #endregion Private members

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TicketController()
        {
            this._dbContext = BaseContext.GetDbContext();
            this._objLog = new GenericRepository<CS_Log>();
            this._objStatus = new GenericRepository<CS_Status>();
            this._objProjectTeam = new GenericRepository<ProjectTeam>();
            this._objProject = new GenericRepository<Projects>();
            this._objCustomer = new GenericRepository<Customer>();
            this._objTicket = new GenericRepository<CS_Ticket>();
            this._objSLA = new GenericRepository<CS_SLA>();
            this._objCallLog = new GenericRepository<CS_CallLog>();
            this._objPriority = new GenericRepository<CS_Priority>();
            this._objEmployee = new GenericRepository<Employee>();
            this._objLink = new GenericRepository<CS_EmailLink>();
        }

        #endregion Constructor

        #region Action methods

        /// <summary>
        /// Generate ticket page
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Generate()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            // Store current Url
            string currentURL = Request.Url.PathAndQuery;
            // Find Call Log id from page uri
            string dirName = Path.GetFileName(currentURL.TrimEnd(Path.DirectorySeparatorChar));
            // Store CallLog Id
            int callLogId;
            bool result = (dirName.All(Char.IsDigit) && dirName.Length > 0);

            // Redirect From CallLog page
            if (result)
            {
                // Fetching CallLogid from url
                callLogId = Convert.ToInt32(dirName);
                // Find row from call Log Id
                var temp = _objCallLog.FindById(callLogId);
                // Store customerid
                int custId = 0;

                // Find customerid from extension number
                foreach (var item in _objCustomer.GetAll())
                {
                    // On assumption that each customer has Dedicated Telephone Line
                    if (item.CS_DedicatedNumber == temp.ExtensionNumber)
                    {
                        custId = item.Id;
                    }
                }
                ViewData["CustId"] = new SelectList(_objCustomer.GetAll().Where(c => c.Id == custId), "Id", "Name");
            }
            return View(ViewName.TicketGenerate);
        }

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
            return View(ViewName.TicketIndex);
        }

        /// <summary>
        /// Generate new ticket for customer
        /// </summary>
        /// <param name="ticketViewModel">Instance of ticket view model</param>
        /// <returns>Redirect to action page</returns> 
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Generate(TicketViewModel ticketViewModel)
        {
            CS_Ticket ticketObj = new CS_Ticket();
            CS_Log logObj = new CS_Log();
            CS_EmailLink linkObj = new CS_EmailLink();

            ticketObj.CreatedOn = DateTime.Now;
            ticketObj.EmployeeId = GetEmployeeId();
            ticketObj.Description = ticketViewModel.Description;
            ticketObj.CustomerId = ticketViewModel.CustomerId;
            ticketObj.SLAId = ticketViewModel.IDSLA;
            ticketObj.Title = ticketViewModel.Title;
            ticketObj.ProjectId = ticketViewModel.ProjectId;
            ticketObj.StatusId = 1;
            ticketObj.LastComment = "";
            _objTicket.Insert(ticketObj);

            // To add entry in Log table
            int ticketid = _dbContext.CS_Ticket.Max(item => item.TicketId);
            logObj.TicketId = ticketid;
            logObj.Title = ticketViewModel.Title;
            logObj.Comment = "Ticket Created";
            logObj.CreatedOn = ticketObj.CreatedOn;
            logObj.EmployeeId = ticketObj.EmployeeId;
            logObj.CustumerId = ticketObj.CustomerId;
            logObj.StatusId = 1;
            logObj.PriorityName = FindPriorityBySLAId(ticketObj.SLAId);

            _objLog.Insert(logObj);
            string ReadUrl = ReadURL();
            bool result = (ReadUrl.All(Char.IsDigit) && ReadUrl.Length > 0);

            // Update Ticket generated In calllog table
            if (result == true)
            {
                int callLogid = Convert.ToInt32(ReadUrl);
                //// CS_CallLog callObj = new CS_CallLog();
                //var row = (from u in _dbContext.CS_CallLog
                //           where u.CallLogId == callLogid
                //           select u);

                //foreach (var entity in row)
                //{
                //    callObj = entity;
                //    callObj.IsTicketGenerated = true;
                //}
                CS_CallLog callObj = _objCallLog.FindById(callLogid);

                callObj.IsTicketGenerated = true;
                callObj.TicketId = ticketObj.TicketId;
                _objCallLog.Update(callObj);
            }

            //Fetch customer email id
            string emailFrom = _dbContext.Customer.Where(e => e.Id == ticketObj.CustomerId).Select(a => a.Email).SingleOrDefault();
            //Fetch status name
            string status = _dbContext.CS_Status.Where(s => s.StatusId == ticketObj.StatusId).Select(a => a.Status).SingleOrDefault();
            //Fetch priority name
            string priority = _dbContext.CS_SLA.Where(p => p.SLAId == ticketObj.SLAId).Select(a => a.PriorityName).SingleOrDefault();
            //Fetch ticket number
            string ticketNumber = _dbContext.CS_Ticket.Where(t => t.TicketId == ticketObj.TicketId).Select(a => a.TicketNumber).SingleOrDefault();
            //Fectch employee email
            string employeeEmail = _dbContext.Employee.Where(e => e.Id == ticketObj.EmployeeId).Select(a => a.Email_Official).SingleOrDefault();

            linkObj.TicketId = ticketObj.TicketId;
            linkObj.Link = Utility.GenerateLink(linkObj.TicketId);
            _objLink.Insert(linkObj);
            string employeeBody = EmailContent.EmployeeBody(linkObj.Link);

            Thread AcknowledgeCustomer = new Thread(() => SendMail.SendAcknowledgement(emailFrom, EmailContent.CustomerSubject, EmailContent.CustomerBody, ticketNumber, priority, status));
            Thread NotifyEmployee = new Thread(() => SendMail.SendAcknowledgement(employeeEmail, EmailContent.EmployeeSubject, employeeBody, ticketNumber, priority, status));
            AcknowledgeCustomer.Start();
            NotifyEmployee.Start();

            return RedirectToAction(ActionName.TicketIndex, ControllerName.Ticket);
        }

        /// <summary>
        /// Display ticket data
        /// </summary>
        /// <param name="request">Kendo data source request</param>
        /// <returns>Json result of ticket data</returns>
        public ActionResult ViewTicket([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }

            return Json(GetTicket(id).ToDataSourceResult(request));
        }

        ///<summary>
        /// Redirect to edit view
        /// </summary>
        /// <param name="id">Id of selected entity</param>
        /// <returns>Display view of the selected ticket</returns>
        public ActionResult Edit(int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            CS_Ticket csTicket = _objTicket.FindById(id);
            TicketViewModel ticketViewModel = new TicketViewModel();

            ticketViewModel.TicketId = id;
            ticketViewModel.TicketNumber = csTicket.TicketNumber;
            ticketViewModel.PriorityLevel = csTicket.PriorityLevel;
            ticketViewModel.Title = csTicket.Title;
            ticketViewModel.Description = csTicket.Description;
            ticketViewModel.CreatedOn = csTicket.CreatedOn;
            ticketViewModel.CustomerId = csTicket.CustomerId;
            ticketViewModel.EmployeeId = csTicket.EmployeeId;
            ticketViewModel.ForwardTo = csTicket.ForwardToEmployee;
            ticketViewModel.ProjectId = csTicket.ProjectId;
            ticketViewModel.StatusId = csTicket.StatusId;
            ticketViewModel.IDSLA = csTicket.SLAId;
            ticketViewModel.Comment = csTicket.LastComment;
            // Fetch customer name
            ticketViewModel.CustomerName = _dbContext.CS_Ticket.Include("Customer")
                                            .Where(c => c.TicketId == ticketViewModel.TicketId
                                                && c.CustomerId == ticketViewModel.CustomerId)
                                            .Select(s => s.Customer.Name).SingleOrDefault();
            // Fetch project name
            ticketViewModel.ProjectName = _dbContext.CS_Ticket.Include("Projects")
                                            .Where(p => p.ProjectId == ticketViewModel.ProjectId
                                                && p.TicketId == ticketViewModel.TicketId)
                                            .Select(a => a.Projects.ProjectName).SingleOrDefault();

            ticketViewModel.EmployeeName = _dbContext.CS_Ticket.Include("Employee")
                                            .Where(c => c.TicketId == ticketViewModel.TicketId
                                                && c.EmployeeId == ticketViewModel.EmployeeId)
                                            .Select(s => s.Employee.FiistName + " " + s.Employee.LastName).SingleOrDefault();

            return View(ticketViewModel);
        }

        /// <summary>
        /// Edit ticket in database
        /// Insert entry in Log table
        /// </summary>
        /// <param name="ticketViewModel">Instanc of ticket view model</param>
        /// <returns>Redirect to index page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TicketViewModel ticketViewModel)
        {
            CS_Ticket ticketObj = new CS_Ticket();
            CS_Log logObj = new CS_Log();
            CS_EmailLink linkObj = new CS_EmailLink();

            int statusId = _dbContext.CS_Ticket.Where(s => s.TicketId == ticketViewModel.TicketId).Select(a => a.StatusId).Single();
            int slaId = _dbContext.CS_Ticket.Where(s => s.TicketId == ticketViewModel.TicketId).Select(a => a.SLAId).Single();
            int employId = _dbContext.CS_Ticket.Where(s => s.TicketId == ticketViewModel.TicketId).Select(a => a.EmployeeId).Single();
            string ticketNumber = _dbContext.CS_Ticket.Where(t => t.TicketId == ticketViewModel.TicketId).Select(a => a.TicketNumber).Single();
            string priority = _dbContext.CS_SLA.Where(p => p.SLAId == ticketViewModel.IDSLA).Select(a => a.PriorityName).Single();
            string status = _dbContext.CS_Status.Where(s => s.StatusId == ticketViewModel.StatusId).Select(a => a.Status).Single();

            if (ticketViewModel.ForwardTo != null)
            {
                if (ticketViewModel.ProjectId != 1)
                {
                    linkObj.TicketId = ticketViewModel.TicketId;
                    linkObj.Link = Utility.GenerateLink(linkObj.TicketId);
                    if ((_dbContext.CS_EmailLink.Where(p => p.TicketId == linkObj.TicketId).Count()) == 1)
                    {
                        _objLink.Update(linkObj);
                        string employeeBody = EmailContent.EmployeeBody(linkObj.Link);

                        string employeeEmail = _dbContext.Employee.Where(e => e.Id == ticketViewModel.ForwardTo).Select(a => a.Email_Official).SingleOrDefault();
                        Thread NotifyEmployee = new Thread(() => SendMail.SendAcknowledgement(employeeEmail, EmailContent.EmployeeSubject, employeeBody, ticketNumber, priority, status));
                        NotifyEmployee.Start();
                    }
                }
            }

            if (statusId != ticketViewModel.StatusId
                || slaId != ticketViewModel.IDSLA)
            {
                //if (ticketViewModel.StatusId == 6)
                //{
                //    linkObj.TicketId = _dbContext.CS_EmailLink.Where(t => t.TicketId == ticketViewModel.TicketId).Select(a => a.TicketId).Single();
                //    _objLink.DeleteById(linkObj.TicketId);
                //}
                string emailFrom = _dbContext.Customer.Where(e => e.Id == ticketViewModel.CustomerId).Select(a => a.Email).Single();
                Thread AcknowledgeCustomer = new Thread(() => SendMail.SendAcknowledgement(emailFrom, EmailContent.CustomerSubject, EmailContent.CustomerBody, ticketNumber, priority, status));
                AcknowledgeCustomer.Start();
            }
            // Find priority level from SLAId
            Int16 prioritylevel = 0;
            foreach (var item in _objSLA.GetAll())
            {
                if (item.SLAId == ticketViewModel.IDSLA)
                {
                    prioritylevel = item.PriorityLevel;
                    break;
                }
            }

            // Create ticket object with edited value for update
            ticketObj.TicketId = ticketViewModel.TicketId;
            ticketObj.Title = ticketViewModel.Title;
            ticketObj.CreatedOn = ticketViewModel.CreatedOn;
            ticketObj.PriorityLevel = Convert.ToByte(prioritylevel);
            ticketObj.Description = _dbContext.CS_Ticket.Where(x => x.TicketId == ticketViewModel.TicketId).Select(d => d.Description).SingleOrDefault();
            ticketObj.CustomerId = ticketViewModel.CustomerId;
            ticketObj.EmployeeId = GetEmployeeId();
            ticketObj.ForwardToEmployee = ticketViewModel.ForwardTo;
            ticketObj.SLAId = ticketViewModel.IDSLA;
            ticketObj.StatusId = ticketViewModel.StatusId;
            ticketObj.ProjectId = ticketViewModel.ProjectId;
            ticketObj.TicketNumber = ticketViewModel.TicketNumber;
            ticketObj.LastComment = ticketViewModel.Comment;
            // New object for log table to insert entry
            logObj.Title = ticketViewModel.Title;
            logObj.Comment = ticketViewModel.Comment;
            logObj.CreatedOn = DateTime.Now;
            logObj.ForwardToEmployee = ticketViewModel.ForwardTo;
            logObj.EmployeeId = GetEmployeeId();
            logObj.StatusId = ticketViewModel.StatusId;
            logObj.TicketId = ticketViewModel.TicketId;
            logObj.CustumerId = ticketViewModel.CustomerId;
            logObj.PriorityName = FindPriorityBySLAId(ticketViewModel.IDSLA);

            // Methods call update ticket table
            _objTicket.Update(ticketObj);
            // Method call to insert in log table
            _objLog.Insert(logObj);
            return RedirectToAction(ActionName.TicketIndex);
        }

        /// <summary>
        /// Action Method for Invalid Link from Email
        /// </summary>
        /// <returns>View</returns>
        public ActionResult InvalidLink()
        {
            return View("InvalidLink");
        }

        /// <summary>
        /// To CheckMailLink if Link is true Then Redirect To Edit Page
        /// else Redirect to authentication Page
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckMail()
        {
            string dirName = ReadURL();
            int TicketNo = 0;
            TicketNo = _dbContext.CS_EmailLink.Where(c => c.Link == dirName).Select(c => c.TicketId).SingleOrDefault();

            if (TicketNo == 0)
                return RedirectToAction("InvalidLink");
            else
            {
                //int EmployeeId = 100;
                int CurrentEmployee = _dbContext.CS_Ticket.Where(c => c.TicketId == TicketNo).Select(c => c.EmployeeId).SingleOrDefault();

                //if(EmployeeId == CurrentEmployee)
                //{
                Session["user"] = Convert.ToString(CurrentEmployee);
                //}
                return RedirectToAction("Edit", "Ticket", new { id = TicketNo });
            }
        }

        #endregion Action methods

        #region Public methods

        /// <summary>
        /// Search ticket from database
        /// </summary>
        /// <param name="id">Ticket Number</param>
        /// <returns>Ticket Id</returns>
        public JsonResult SearchTicket(string id)
        {
            int result = SearchValidate(id);
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Search ticket id from ticket number
        /// </summary>
        /// <param name="ticketNumber">ticket number</param>
        /// <returns>Ticket Id</returns>
        public int SearchValidate(string ticketNumber)
        {
            int data = 0;
            int temp = 0;
            temp = _objProjectTeam.GetAll().Where(c => c.EmployeeId == GetEmployeeId() && c.ProjectId == 1).Select(c => c.ProjectId).SingleOrDefault();

            if (temp == 1)
                data = _objTicket.GetAll().Where(c => c.TicketNumber == ticketNumber).Select(c => c.TicketId).SingleOrDefault();
            else
                data = _objTicket.GetAll().Where(c => c.TicketNumber == ticketNumber && c.EmployeeId == GetEmployeeId()).Select(c => c.TicketId).SingleOrDefault();

            return data;
        }

        /// <summary>
        /// Display employee data
        /// </summary>
        /// <param name="id">ProjectId</param>
        /// <returns>Json result of employee data</returns>
        public JsonResult ViewEmployeeByProjectId(int id)
        {
            return this.Json(GetListOfEmployee(id), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display Priority Name
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <returns>Json result of sla priority</returns>
        public JsonResult ViewPriorityByProjectId(int id, int tid)
        {
            return this.Json(GetListOfPriority(id,tid ), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display Status Name
        /// </summary>
        /// <returns>Json result of status</returns>
        public JsonResult ViewStatus()
        {
            return this.Json(GetListOfStatus(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// View All ticket on checkbox click
        /// </summary>
        /// <param name="id">Checkbox checked(0) or unchecked id(1)</param>
        /// <returns>Ticket Data of selected id</returns>
        public JsonResult ViewTicketByAjax(int id)
        {
            return this.Json(GetTicket(id), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get ticket data
        /// </summary>
        /// <returns>List of ticket data</returns>
        public IEnumerable<TicketViewModel> GetTicket(int statusId)
        {
            int[] DefaultsTeamId = _objProjectTeam.GetAll().Where(c => c.ProjectId == 1).Select(v => v.EmployeeId).ToArray();

            var id = GetEmployeeId();
            var query = (from data in _dbContext.CS_Ticket
                        .Include("Customer")
                        .Include("Projects")
                        .Include("Employee")
                        .Include("CS_SLA")
                        .Include("CS_Status")
                         select new TicketViewModel
                         {
                             TicketId = data.TicketId,
                             TicketNumber = data.TicketNumber,
                             CustomerId = data.CustomerId,
                             CustomerName = data.Customer.Name,
                             EmployeeId = data.EmployeeId,
                             EmployeeName = data.Employee.FiistName + " " + data.Employee.LastName,
                             ProjectId = data.ProjectId,
                             ProjectName = data.Projects.ProjectName,
                             IDSLA = data.CS_SLA.SLAId,
                             Priority = data.CS_SLA.PriorityName,
                             Title = data.Title,
                             Description = data.Description,
                             PriorityLevel = data.PriorityLevel,
                             CreatedOn = data.CreatedOn,
                             StatusId = data.StatusId,
                             ForwardTo = data.ForwardToEmployee,
                             ForwardEmployeeName = data.ForwardToEmployee == null
                             ? "  ---  " : _dbContext.Employee
                            .Where(c1 => c1.Id == data.ForwardToEmployee)
                            .Select(s => s.FiistName + " " + s.LastName).FirstOrDefault(),
                             Status = data.CS_Status.Status,
                         }).ToList().OrderByDescending(order => order.TicketId);
            if (DefaultsTeamId.Contains(id))
            {
                if (statusId == 1)
                {
                    return query.Where(p => p.Status == "OPEN");
                }
                return query;
            }
            else
            {
                if (statusId == 1)
                {
                    return query.Where(p => p.ForwardTo == id)
                   .Concat(query.Where(p => p.EmployeeId == id
                                         && p.Status == "OPEN"));
                }
                else
                {
                    int[] ProjID = _objProjectTeam.GetAll().Where(c => c.EmployeeId == GetEmployeeId()).Select(v => v.ProjectId).ToArray();
                    // query.Where(p => p.ForwardEmployeeName == GetForwardEmployeeName(p.ForwardTo));
                    return query.Where(p => p.ForwardTo == id)
                   .Concat(query.Where(p => ProjID.Contains(p.ProjectId) && p.ForwardTo != id));
                }
            }
        }

        /// <summary>
        /// Get employee data
        /// </summary>
        /// <param name="pid">Project id</param>
        /// <returns>List of employee</returns>
        public IEnumerable<TicketViewModel> GetListOfEmployee(int pid)
        {
            var employeeId = GetEmployeeId();
            return (from emp in _dbContext.ProjectTeam.Include("Employee")
                    where emp.ProjectId == pid && emp.EmployeeId != employeeId
                    select new TicketViewModel
                    {
                        ForwardTo = emp.EmployeeId,
                        ForwardEmployeeName = emp.Employee.FiistName + " " + emp.Employee.LastName,
                    }).ToList().OrderBy(o => o.ForwardEmployeeName);
        }

        /// <summary>
        /// Get prioriy name for SLA data
        /// </summary>
        /// <param name="id">ProjectId</param>
        /// <returns>List of priority name</returns>
        public IEnumerable<TicketViewModel> GetListOfPriority(int id, int tid)
        {

            Nullable<bool>  a = _dbContext.CS_Ticket.Where(t => t.TicketId == tid).Select(s => s.CS_SLA.IsActive).SingleOrDefault();

            var query = (from sla in _dbContext.CS_SLA
                    where sla.ProjectId == id
                    && sla.IsActive == true
                    select new TicketViewModel
                    {
                        IDSLA = sla.SLAId,
                        Priority = sla.PriorityName
                    }).ToList();


            if (a == false)
            {
                var Data = from d in _dbContext.CS_Ticket.Include("CS_SLA")
                             where d.TicketId == tid
                             select new { IDSLA = d.CS_SLA.SLAId, Priority = d.CS_SLA.PriorityName };

                foreach (var priority in Data)
                {
                    query.Add(new TicketViewModel { IDSLA = priority.IDSLA, Priority = priority.Priority });
                }
            }
            return query.OrderBy(o => o.Priority);
        }

        /// <summary>
        /// Get Status data
        /// </summary>
        /// <returns>List of status name</returns>
        public IEnumerable<CS_Status> GetListOfStatus()
        {
            return (from status in _objStatus.GetAll() select new CS_Status { StatusId = status.StatusId, Status = status.Status }).ToList();
        }

        /// <summary>
        /// Get Employee Id from session
        /// </summary>
        /// <returns>EmployeeId</returns>
        public int GetEmployeeId()
        {
            return (Convert.ToInt32(Session["user"] as string));
        }

        /// <summary>
        /// Get customer data
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCustomer()
        {
            return this.Json(GetListOfCustomer(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get list of customer id and name
        /// </summary>
        /// <returns>Json result of customer</returns>
        public IEnumerable<Customer> GetListOfCustomer()
        {
            // Store all project id for given employee id

            int[] ProjID = _objProjectTeam.GetAll().Where(c => c.EmployeeId == GetEmployeeId()).Select(v => v.ProjectId).ToArray();
            if (ProjID.Contains(1))
            {
                return (from customer in _objCustomer.GetAll() select new Customer { Id = customer.Id, Name = customer.Name }).ToList().OrderBy(o => o.Name);
            }
            else
            {
                // Find customer for given projects
                int?[] CustId = _objProject.GetAll().Where(c => ProjID.Contains(c.Id)).Select(c => c.CustomerId).ToArray();

                return (from customer in _objCustomer.GetAll().Where(x => CustId.Contains(x.Id)) select new Customer { Id = customer.Id, Name = customer.Name }).ToList().OrderBy(o => o.Name);
            }
        }

        /// <summary>
        /// Get Project detail
        /// </summary>
        /// <param name="categories">Id of selected customer</param>
        /// <returns>Json result of project</returns>
        public JsonResult GetCascadeProjects(int? categories)
        {
            return this.Json(GetListofProjectsById(categories).Select(p => new { Id = p.Id, ProjectName = p.ProjectName }).OrderBy(o => o.ProjectName), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get list of project detail
        /// </summary>
        /// <param name="customer">Id of customer</param>
        /// <returns>List of projects id and name </returns>
        public IEnumerable<Projects> GetListofProjectsById(int? customer)
        {
            int[] ProjID = _objProjectTeam.GetAll().Where(c => c.EmployeeId == GetEmployeeId()).Select(v => v.ProjectId).ToArray();
            var product = _objProject.GetAll().AsQueryable();
            if (customer != null && ProjID.Contains(1))
            {
                // Fetch product for given customer
                product = product.Where(p => p.CustomerId == customer);
            }
            else
            {
                product = product.Where(p => p.CustomerId == customer);
                product = product.Where(x => ProjID.Contains(x.Id));
            }
            return (from project in product select new Projects { Id = project.Id, ProjectName = project.ProjectName }).ToList().OrderBy(o => o.ProjectName);
        }

        /// <summary>
        /// Get priority data
        /// </summary>
        /// <param name="proj">Id of project</param>
        /// <returns>Json result of project</returns>
        public JsonResult GetPriorityOfProject(int proj)
        {
            return this.Json(GetListofProjectsprio(proj).Select(p => new { SLAId = p.SLAId, PriorityName = p.PriorityName }).OrderBy(o => o.PriorityName), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get list of priority
        /// </summary>
        /// <param name="proj">Id of project</param>
        /// <returns>List of priority id and name</returns>
        public IEnumerable<CS_SLA> GetListofProjectsprio(int? proj)
        {
            var product = _objSLA.GetAll().Where(a => a.IsActive == true).AsQueryable();
            if (proj != null)
            {
                product = product.Where(p => p.ProjectId == proj);
            }
            return (from priority in product select new CS_SLA { SLAId = priority.SLAId, PriorityName = priority.PriorityName }).ToList().OrderBy(o => o.PriorityName);
        }

        /// <summary>
        /// Method to find SLA id based on ProjectId and PriorityName
        /// </summary>
        /// <param name="PriorityName">Priority Name</param>
        /// <param name="projid">Project Id</param>
        /// <returns>SLA ID</returns>
        public int FindSLAIdByPriorityName(string PriorityName, int projid)
        {
            int slaid = 0;
            // Find SLA Id from Project Id
            foreach (var item in _objSLA.GetAll().Where(p => p.ProjectId == projid && p.PriorityName == PriorityName))
            {
                slaid = item.SLAId;
                break;
            }
            return slaid;
        }

        /// <summary>
        /// Method to find Priority from SLAID
        /// </summary>
        /// <param name="slaid">SLA id</param>
        /// <returns>priorityname</returns>
        public string FindPriorityBySLAId(int slaid)
        {
            // Find priority name from SLA id
            string priorityName = (from u in _dbContext.CS_SLA
                                   where u.SLAId == slaid
                                   select u.PriorityName).SingleOrDefault();

            return priorityName;
        }

        /// <summary>
        /// Display ticket data
        /// </summary>
        /// <param name="id">Ticket id</param>
        /// <returns>Json result of ticket log</returns>
        public JsonResult ViewTicketLogById(int id)
        {
            return this.Json(GetTicketLogById(id), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get ticket data
        /// </summary>
        /// <param name="ticketId">Ticket id</param>
        /// <returns>List of ticket data</returns>
        public IEnumerable<TicketLogViewModel> GetTicketLogById(int ticketId)
        {
            return (from ticketLog in _dbContext.CS_Log
                        .Include("Customer")
                        .Include("Employee")
                        .Include("CS_Ticket")
                        .Include("CS_Status")
                    where ticketLog.TicketId == ticketId
                    select new TicketLogViewModel
                    {
                        TicketId = ticketLog.TicketId,
                        TicketNumber = ticketLog.CS_Ticket.TicketNumber,
                        EmployeeId = ticketLog.EmployeeId,
                        EmployeeName = ticketLog.Employee.FiistName + " " + ticketLog.Employee.LastName,
                        ForwardTo = ticketLog.ForwardToEmployee,
                        ForwardEmployeeName = ticketLog.ForwardToEmployee == null
                        ? "  ---  " : _dbContext.Employee
                       .Where(c1 => c1.Id == ticketLog.ForwardToEmployee)
                       .Select(s => s.FiistName + " " + s.LastName).FirstOrDefault(),
                        CustomerId = ticketLog.CustumerId,
                        CustomerName = ticketLog.Customer.Name,
                        ProjectId = ticketLog.CS_Ticket.ProjectId,
                        ProjectName = ticketLog.CS_Ticket.Projects.ProjectName,
                        PriorityName = ticketLog.PriorityName,
                        StatusId = ticketLog.StatusId,
                        Status = ticketLog.CS_Status.Status,
                        LogId = ticketLog.LogId,
                        LogTitle = ticketLog.Title,
                        LogComment = ticketLog.Comment,
                        LogCreatedOn = ticketLog.CreatedOn
                    }).ToList().OrderByDescending(order => order.LogCreatedOn);
        }

        /// <summary>
        /// ReadURL based on Function
        /// </summary>
        /// <returns>Last directory of url</returns>
        public string ReadURL()
        {
            // Get url
            string url = Request.Url.AbsolutePath;
            // Find last directory or file or Id
            string dirName = Path.GetFileName(url.TrimEnd(Path.DirectorySeparatorChar));
            return dirName;
        }

        /// <summary>
        /// Display Status Name
        /// </summary>
        /// <returns>Json result of status</returns>
        public JsonResult AcceptUpdate(int Ticketid, int FEmpId)
        {
            CS_Ticket csTicket = _objTicket.FindById(Ticketid);

            csTicket.ForwardToEmployee = null;
            csTicket.EmployeeId = FEmpId;

            return this.Json(FrowardEdit(csTicket), JsonRequestBehavior.AllowGet);
        }

        private bool FrowardEdit(CS_Ticket UpdateTicket)
        {
            CS_Log logObj = new CS_Log();
            _objTicket.Update(UpdateTicket);
            logObj.Title = UpdateTicket.Title;
            logObj.Comment = "ACCEPTED";
            logObj.CreatedOn = DateTime.Now;
            logObj.ForwardToEmployee = null;
            logObj.EmployeeId = GetEmployeeId();
            logObj.StatusId = UpdateTicket.StatusId;
            logObj.TicketId = UpdateTicket.TicketId;
            logObj.CustumerId = UpdateTicket.CustomerId;
            logObj.PriorityName = FindPriorityBySLAId(UpdateTicket.SLAId);
            // Method call to insert in log table
            _objLog.Insert(logObj);
            return true;
        }

        public JsonResult GetAllCustomer()
        {
            return this.Json(GetListOfAllCustomer(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult StatusBar(int selstatus)
        {
            return this.Json(GetStatusProgresbarOpen(selstatus), JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<Customer> GetListOfAllCustomer()
        {
            return (from customer in _objCustomer.GetAll() select new Customer { Id = customer.Id, Name = customer.Name }).ToList();
        }

        public double GetStatusProgresbarOpen(int selstatus)
        {
            var totalcount = _objTicket.GetAll().Where(x => x.EmployeeId == GetEmployeeId()).Count();
            int count = _objTicket.GetAll().Where(status => status.StatusId == selstatus).Where(x => x.EmployeeId == GetEmployeeId()).Count();
            double a = ((float)(count * 100) / totalcount);

            return a;
        }

        #endregion Public methods
    }
}