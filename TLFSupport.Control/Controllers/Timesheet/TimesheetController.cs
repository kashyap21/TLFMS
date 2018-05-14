using System;
using System.Collections.Generic;
using System.Linq;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Web.Mvc;
using TLFSupport.Common;
using TLFSupport.Model;
using TLFSupport.Model.CustomeModel;
using TLFSupport.Model.DatabaseModel;
using TLFSupport.Model.DatabaseModel.Repositories;
using System.Globalization;
using System.Data.Entity;

namespace TLFSupport.Control.Controllers.Timesheet
{
    public class TimesheetController : BaseController
    {
        #region Private members
        /// <summary>
        /// Database context
        /// </summary>
        private TLFCSEntities _dbContext;

        /// <summary>
        /// Instance of Employee
        /// </summary>
        private GenericRepository<Employee> _objEmployee;

        /// <summary>
        /// Instance of project team
        /// </summary>
        private GenericRepository<ProjectTeam> _objProjectTeam;

        /// <summary>
        /// Instance of Timesheet
        /// </summary>
        private GenericRepository<TimeSheet> _objTimesheet;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public TimesheetController()
        {
            this._dbContext = BaseContext.GetDbContext();
            this._objTimesheet = new GenericRepository<TimeSheet>();
            this._objEmployee = new GenericRepository<Employee>();
            this._objProjectTeam = new GenericRepository<ProjectTeam>();
        }
        #endregion

        #region Action Methods

        /// <summary>
        /// Action method for timesheet report summery
        /// </summary>
        /// <returns>View</returns>
        public ActionResult TimesheetReportSummery()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View();
        }

        /// <summary>
        /// Action method for Indexpage of Timesheet
        /// </summary>
        /// <returns>View of that page</returns>
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View();
        }

        /// <summary>
        /// Action method for Index page of NLTimesheet
        /// </summary>
        /// <returns>View of NL Timesheet</returns>
        public ActionResult NLIndex()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View();
        }

        /// <summary>
        /// Inserts timesheet value
        /// </summary>
        /// <param name="timesheetdata">Form value</param>
        /// <returns>View of index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NLIndex(NLTimesheetViewModel timesheetdata)
        {
            if (timesheetdata.TimesheetId != 0)
            {
                NLUpdate(timesheetdata);
            }
            else
            {
                TimeSheet timesheet = new TimeSheet();

                for (int i = 0; i < 7; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if (timesheetdata.MonActualHrs <= 0)
                            {
                                continue;
                            }
                            else
                            {
                                timesheet.Date = YearWeekDayToDateTime(DateTime.Now.Year, DayOfWeek.Monday, timesheetdata.weekno);
                                timesheet.TimeSpent = timesheetdata.MonActualHrs;
                            }
                            break;

                        case 1:
                            if (timesheetdata.TueActualHrs <= 0)
                            {
                                continue;
                            }
                            else
                            {
                                timesheet.Date = YearWeekDayToDateTime(DateTime.Now.Year, DayOfWeek.Tuesday, timesheetdata.weekno);
                                timesheet.TimeSpent = timesheetdata.TueActualHrs;
                            }
                            break;

                        case 2:
                            if (timesheetdata.WedActualHrs <= 0)
                            {
                                continue;
                            }
                            else
                            {
                                timesheet.Date = YearWeekDayToDateTime(DateTime.Now.Year, DayOfWeek.Wednesday, timesheetdata.weekno);
                                timesheet.TimeSpent = timesheetdata.WedActualHrs;
                            }
                            break;

                        case 3:
                            if (timesheetdata.ThuActualHrs <= 0)
                            {
                                continue;
                            }
                            else
                            {
                                timesheet.Date = YearWeekDayToDateTime(DateTime.Now.Year, DayOfWeek.Thursday, timesheetdata.weekno);
                                timesheet.TimeSpent = timesheetdata.ThuActualHrs;
                            }
                            break;

                        case 4:
                            if (timesheetdata.FriActualHrs <= 0)
                            {
                                continue;
                            }
                            else
                            {
                                timesheet.Date = YearWeekDayToDateTime(DateTime.Now.Year, DayOfWeek.Friday, timesheetdata.weekno);
                                timesheet.TimeSpent = timesheetdata.FriActualHrs;
                            }
                            break;

                        case 5:
                            if (timesheetdata.SatActualHrs <= 0)
                            {
                                continue;
                            }
                            else
                            {
                                timesheet.Date = YearWeekDayToDateTime(DateTime.Now.Year, DayOfWeek.Saturday, timesheetdata.weekno);
                                timesheet.TimeSpent = timesheetdata.SatActualHrs;
                            }
                            break;

                        case 6:
                            if (timesheetdata.SunActualHrs <= 0)
                            {
                                continue;
                            }
                            else
                            {
                                timesheet.Date = YearWeekDayToDateTime(DateTime.Now.Year, DayOfWeek.Sunday, timesheetdata.weekno);
                                timesheet.TimeSpent = timesheetdata.SunActualHrs;
                            }
                            break;

                        default:
                            break;
                    }
                    timesheet.ProjectId = timesheetdata.ProjectId;
                    timesheet.WeekNo = timesheetdata.weekno;
                    if (timesheetdata.EmployeeId == 0)
                    {
                        timesheet.EmployeeId = Convert.ToInt32(Session["user"] as string);
                    }
                    else
                    {
                        timesheet.EmployeeId = timesheetdata.EmployeeId;
                    }
                    timesheet.ProjectActivityLinkId = timesheetdata.ActivityId;
                    timesheet.Remarks = timesheetdata.Remark;
                    timesheet.ProjectPhaseId = timesheetdata.PhaseId;
                    timesheet.CreatedOn = DateTime.Now;
                    timesheet.UpdatedOn = DateTime.Now;
                    timesheet.Year = Convert.ToString(DateTime.Now.Year);
                    _objTimesheet.Insert(timesheet);
                }
            }
            return View();
        }

        /// <summary>
        /// Update NL timesheet
        /// </summary>
        /// <param name="timesheetdata">Form value</param>
        private void NLUpdate(NLTimesheetViewModel timesheetdata)
        {
            var rawDate = (from d in _dbContext.TimeSheet
                                 where d.Id == timesheetdata.TimesheetId
                                 select d.Date).Single();
            DateTime date = Convert.ToDateTime(rawDate);
            int day = (int)date.DayOfWeek;
            int flag = 0;

            TimeSheet timesheet = new TimeSheet();
            for (int i = 0; i < 7; i++)
            {
                switch (i)
                {
                    case 0:
                        if (timesheetdata.SunActualHrs <= 0 || day == i)
                        {
                            if (day == i)
                            {
                                timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Sunday, timesheetdata.weekno);
                                timesheet.Id = timesheetdata.TimesheetId;
                                flag = 1;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Sunday, timesheetdata.weekno);
                            timesheet.TimeSpent = timesheetdata.SunActualHrs;
                            timesheet.CreatedOn = DateTime.Now;
                            flag = 0;
                        }
                        break;
                    
                    case 1:
                        if (timesheetdata.MonActualHrs <= 0 || day == i)
                        {
                            if (day == i)
                            {
                                timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Monday, timesheetdata.weekno);
                                timesheet.Id = timesheetdata.TimesheetId;
                                timesheet.TimeSpent = timesheetdata.MonActualHrs;
                                flag = 1;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Monday, timesheetdata.weekno);
                            timesheet.TimeSpent = timesheetdata.MonActualHrs;
                            timesheet.CreatedOn = DateTime.Now;
                            flag = 0;
                        }
                        break;

                    case 2:
                        if (timesheetdata.TueActualHrs <= 0 || day == i)
                        {
                            if (day == i)
                            {
                                timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Tuesday, timesheetdata.weekno);
                                timesheet.Id = timesheetdata.TimesheetId;
                                timesheet.TimeSpent = timesheetdata.TueActualHrs;
                                flag = 1;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Tuesday, timesheetdata.weekno);
                            timesheet.TimeSpent = timesheetdata.TueActualHrs;
                            timesheet.CreatedOn = DateTime.Now;
                            flag = 0;
                        }
                        break;

                    case 3:
                        if (timesheetdata.WedActualHrs <= 0 || day == i)
                        {
                            if (day == i)
                            {
                                timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Wednesday, timesheetdata.weekno);
                                timesheet.Id = timesheetdata.TimesheetId;
                                timesheet.TimeSpent = timesheetdata.WedActualHrs;
                                flag = 1;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Wednesday, timesheetdata.weekno);
                            timesheet.TimeSpent = timesheetdata.WedActualHrs;
                            timesheet.CreatedOn = DateTime.Now;
                            flag = 0;
                        }
                        break;

                    case 4:
                        if (timesheetdata.ThuActualHrs <= 0 || day == i)
                        {
                            if (day == i)
                            {
                                timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Thursday, timesheetdata.weekno);
                                timesheet.Id = timesheetdata.TimesheetId;
                                timesheet.TimeSpent = timesheetdata.ThuActualHrs;
                                flag = 1;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Thursday, timesheetdata.weekno);
                            timesheet.CreatedOn = DateTime.Now;
                            timesheet.TimeSpent = timesheetdata.ThuActualHrs;
                            flag = 0;
                        }
                        break;

                    case 5:
                        if (timesheetdata.FriActualHrs <= 0 || day == i)
                        {
                            if (day == i)
                            {
                                timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Friday, timesheetdata.weekno);
                                timesheet.Id = timesheetdata.TimesheetId;
                                timesheet.TimeSpent = timesheetdata.FriActualHrs;
                                flag = 1;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Friday, timesheetdata.weekno);
                            timesheet.TimeSpent = timesheetdata.FriActualHrs;
                            timesheet.CreatedOn = DateTime.Now;
                            flag = 0;
                        }
                        break;

                    case 6:
                        if (timesheetdata.SatActualHrs <= 0 || day == i)
                        {
                            if (day == i)
                            {
                                timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Saturday, timesheetdata.weekno);                                
                                timesheet.Id = timesheetdata.TimesheetId;
                                timesheet.TimeSpent = timesheetdata.SatActualHrs;                                
                                flag = 1;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            timesheet.Date = YearWeekDayToDateTime(date.Year, DayOfWeek.Saturday, timesheetdata.weekno);
                            timesheet.TimeSpent = timesheetdata.SatActualHrs;
                            timesheet.CreatedOn = DateTime.Now;
                            flag = 0;
                        }
                        break;

                    default:
                        break;
                }
                
                timesheet.WeekNo = timesheetdata.weekno;
                timesheet.ProjectId = timesheetdata.ProjectId;
                timesheet.EmployeeId = timesheetdata.EmployeeId;
                timesheet.ProjectActivityLinkId = timesheetdata.ActivityId;
                timesheet.Remarks = timesheetdata.Remark;
                timesheet.ProjectPhaseId = timesheetdata.PhaseId;
                timesheet.UpdatedOn = DateTime.Now;

                if (flag == 0)
                {
                    _objTimesheet.Insert(timesheet);
                }
                else
                {
                    _objTimesheet.Update(timesheet);
                }
            }
        }

        /// <summary>
        /// Calculate Date time for week number provided for any year
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="day">Day of week</param>
        /// <param name="week">Week of year</param>
        /// <returns>Date</returns>
        private static DateTime YearWeekDayToDateTime(int year, DayOfWeek day, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3).AddDays((int)day - 1);
            
            /*DateTime startOfYear = new DateTime(year, 1, 1);

            // The +7 and %7 stuff is to avoid negative numbers etc.
            int daysToFirstCorrectDay = (((int)day - (int)startOfYear.DayOfWeek) + 7) % 7;

            return startOfYear.AddDays(7 * (week - 1) + daysToFirstCorrectDay);*/
        }

        /// <summary>
        /// submit timesheet form for india
        /// Insert in database or Update database if it already exist
        /// Insert or Edit
        /// </summary>
        /// <param name="timesheetdata"></param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(TimesheetViewModel timesheetdata)
        {
            if (timesheetdata.TimesheetId == 0)
            {
                TimeSheet timesheet = new TimeSheet();
                timesheet.Date = DateTime.Today;
                timesheet.ProjectId = timesheetdata.ProjectId;
                if (timesheetdata.EmployeeId == 0)
                    timesheet.EmployeeId = Convert.ToInt32(Session["user"] as string);
                else
                    timesheet.EmployeeId = timesheetdata.EmployeeId;
                timesheet.WeekNo = GetWeekNo();
                timesheet.ReferenceIssueNo = timesheetdata.ReferenceIssueNo;
                //  timesheet.ActivityId = timesheetdata.ActivityId;
                timesheet.ProjectActivityLinkId = timesheetdata.ActivityId;
                timesheet.TimeSpent = timesheetdata.ActualHrs;
                //timesheet.TimeBillable =
                timesheet.Remarks = timesheetdata.Remarks;
                timesheet.ProjectPhaseId = timesheetdata.PhaseId;
                timesheet.EstimatedTime = timesheetdata.EstimatedHrs;
                timesheet.IsCompleted = true;
                timesheet.IsExtraHours = true;
                timesheet.Description = timesheetdata.Description;
                timesheet.CreatedOn = DateTime.Now;
                timesheet.UpdatedOn = DateTime.Now;
                timesheet.Year = DateTime.Now.Year.ToString();

                _objTimesheet.Insert(timesheet);
            }
            else
            {
                TimeSheet timesheet = new TimeSheet();
                timesheet = _objTimesheet.FindById(timesheetdata.TimesheetId);
                timesheet.ProjectId = timesheetdata.ProjectId;
                if (timesheetdata.EmployeeId == 0)
                    timesheet.EmployeeId = Convert.ToInt32(Session["user"] as string);
                else
                    timesheet.EmployeeId = timesheetdata.EmployeeId;
                timesheet.ReferenceIssueNo = timesheetdata.ReferenceIssueNo;
                timesheet.ProjectActivityLinkId = timesheetdata.ActivityId;
                timesheet.TimeSpent = timesheetdata.ActualHrs;
                //timesheet.TimeBillable =
                timesheet.Remarks = timesheetdata.Remarks;
                timesheet.ProjectPhaseId = timesheetdata.PhaseId;
                timesheet.EstimatedTime = timesheetdata.EstimatedHrs;
                timesheet.IsCompleted = false;
                timesheet.IsExtraHours = false;
                timesheet.Description = timesheetdata.Description;
                timesheet.UpdatedOn = DateTime.Now;
                timesheet.Year = DateTime.Now.Year.ToString();

                _objTimesheet.Update(timesheet);
            }
            return View();
        }    
        #endregion

        #region Public Methods
        /// <summary>
        /// Json method to list all Employee
        /// </summary>
        /// <returns>List of all Employee</returns>
        public JsonResult GetAllEmployeeForIndia()
        {
            return this.Json(GettListOfEmployeeForIndia(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Public method to mark as extrahour
        /// </summary>
        /// <param name="id">Timesheet Id</param>
        public void MarkAsExtrahour(int id)
        {
            MarkedAsExtrahour(id);
        }

        /// <summary>
        /// Method to Mark as extrahour
        /// </summary>
        /// <param name="id">Timesheetid</param>
        public void MarkedAsExtrahour(int id)
        {
            TimeSheet model = new TimeSheet();
            model = _objTimesheet.FindById(id);
            model.IsExtraHours = true;
            _objTimesheet.Update(model);
        }

        /// <summary>
        /// Method to get list of all employees
        /// </summary>
        /// <returns>List of all employees</returns>
        public IEnumerable<TimesheetViewModel> GettListOfEmployeeForIndia()
        {
            var query = (from data in _dbContext.Employee
                    select new TimesheetViewModel
                    {
                        ActivityId = data.PresentCountryId ?? 0,
                        EmployeeId = data.Id,
                        EmployeeName = data.FiistName + " " + data.LastName
                    }).ToList();

            return (query.Where(p => p.ActivityId == 1));
        }

        /// <summary>
        /// Method to Get List of Employee
        /// </summary>
        /// <returns>List of employee</returns>
        public JsonResult GetAllEmployee()
        {
            return this.Json(GettListOfEmployee(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to get list of all employees
        /// </summary>
        /// <returns>List of all employees</returns>
        public IEnumerable<TimesheetViewModel> GettListOfEmployee()
        {
            var query = (from data in _dbContext.Employee
                         select new TimesheetViewModel
                         {
                             EmployeeId = data.Id,
                             EmployeeName = data.FiistName + " " + data.LastName
                         }).ToList();

            return query;
        }

        /// <summary>
        /// Json method to list all Employee
        /// </summary>
        /// <returns>List of all Employee</returns>
        public JsonResult GetAllNLEmployee()
        {
            return this.Json(GettListOfNLEmployee(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to get list of all employees
        /// </summary>
        /// <returns>List of all employees</returns>
        public IEnumerable<NLTimesheetViewModel> GettListOfNLEmployee()
        {
            var query = (from data in _dbContext.Employee
                         select new NLTimesheetViewModel
                         {
                             ActivityId = data.PresentCountryId ?? 0,
                             EmployeeId = data.Id,
                             EmployeeName = data.FiistName + " " + data.LastName
                         }).ToList();

            return (query.Where(p => p.ActivityId == 2));
        }

        /// <summary>
        /// Public method to Get week number
        /// </summary>
        /// <returns>week no of todays date</returns>
        public short GetWeekNo()
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            int weekNum = cul.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return Convert.ToInt16(weekNum);
        }

        /// <summary>
        /// Get week number form any date
        /// </summary>
        /// <param name="date">date</param>
        /// <returns>week number</returns>
        public short GetWeek(string date)
        {
            DateTime d = Convert.ToDateTime(date);
            CultureInfo cul = CultureInfo.CurrentCulture;
            int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return Convert.ToInt16(weekNum);
        }

        /// <summary>
        /// Get week number for NL employees
        /// </summary>
        /// <param name="date"></param>
        /// <returns>week number</returns>
        public JsonResult NLGetWeekNo(string date)
        {
            return this.Json(GetWeek(date), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to get Projects from employee id
        /// </summary>
        /// <param name="employeeid">employeeid</param>
        /// <returns></returns>
        public JsonResult GetProjectForEmployee(int employeeid)
        {
            return this.Json(GetListOfProjectFromEmployee(employeeid), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get project data
        /// </summary>
        /// <param name="employeeid">Employee id</param>
        /// <returns>List of Projects for given employee</returns>
        public IEnumerable<TimesheetViewModel> GetListOfProjectFromEmployee(int employeeid)
        {
            //Contains all project id for given employee
            int[] ProjectI = _objProjectTeam.GetAll().Where(c => c.EmployeeId == employeeid).Select(v => v.ProjectId).ToArray();

            var query = (from data in _dbContext.Projects
                         select new TimesheetViewModel
                         {
                             ProjectId = data.Id,
                             ProjectName = data.ProjectName

                         }).ToList();
            return query.Where(p => ProjectI.Contains(p.ProjectId));
        }

        /// <summary>
        /// Get List of valid projects for given employee
        /// </summary>
        /// <param name="employeeid">Employee id</param>
        /// <returns>List of projects</returns>
        public IEnumerable<TimesheetViewModel> GetListOfProjectFromEmployeeWithOutLeave(int employeeid)
        {
            int[] ProjectI = _objProjectTeam.GetAll().Where(c => c.EmployeeId == employeeid && c.ProjectId != 200).Select(v => v.ProjectId).ToArray();

            var query = (from data in _dbContext.Projects
                         select new TimesheetViewModel
                         {
                             ProjectId = data.Id,
                             ProjectName = data.ProjectName

                         }).ToList();
            return query.Where(p => ProjectI.Contains(p.ProjectId));
        }

        /// <summary>
        /// Get project Employee for timesheet dropdown
        /// </summary>
        /// <returns>List of projects</returns>
        public JsonResult GetProjectForEmployeeForTimesheet()
        {
            int employeeid = Convert.ToInt32(Session["user"] as string);
            return this.Json(GetListOfProjectFromEmployeeWithOutLeave(employeeid), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectForNLEmployee()
        {
            int employeeid = Convert.ToInt32(Session["user"] as string);
            return this.Json(GetListOfProjectFromNLEmployee(employeeid), JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<NLTimesheetViewModel> GetListOfProjectFromNLEmployee(int employeeid)
        {
            int[] ProjectI = _objProjectTeam.GetAll().Where(c => c.EmployeeId == employeeid).Select(v => v.ProjectId).ToArray();

            var query = (from data in _dbContext.Projects
                         select new NLTimesheetViewModel
                         {
                             ProjectId = data.Id,
                             ProjectName = data.ProjectName

                         }).ToList();
            return query.Where(p => ProjectI.Contains(p.ProjectId));
        }

        /// <summary>
        /// Get all phases of given project
        /// </summary>
        /// <param name="projid">project</param>
        /// <returns>list of projects</returns>
        public JsonResult GetAllPhase(int projid)
        {
            return this.Json(GetProjectPhases(projid), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Phase data from projectid
        /// </summary>
        /// <param name="projid">projectid</param>
        /// <returns>List of phase id and name for given project name</returns>
        public IEnumerable<TimesheetViewModel> GetProjectPhases(int projid)
        {
            var query = (from data in _dbContext.ProjectPhases
                         select new TimesheetViewModel
                         {
                             ProjectId = data.ProjectId,
                             PhaseId = data.Id,
                             PhaseName = data.PhaseName
                         }).ToList();
            return query.Where(c => c.ProjectId == projid);
        }

        /// <summary>
        /// Return List of Activities for given project
        /// </summary>
        /// <param name="projid">projectid</param>
        /// <returns> Json data of All activites for given project</returns>
        public JsonResult GetActivityByProjectId(int projid)
        {
            return this.Json(GetActivitiesByProjectId(projid), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns list of Activity name and id for given project
        /// </summary>
        /// <param name="projid">Project Id</param>
        /// <returns>List of Activity Id and Activity name</returns>
        public IEnumerable<TimesheetViewModel> GetActivitiesByProjectId(int projid)
        {
            var query = (from data in _dbContext.ProjectActivityLink
                         select new TimesheetViewModel
                         {
                             ProjectId = data.ProjectId,
                             ActivityId = data.Id,
                             ActivityName = data.ProjectActivityName
                         }).ToList();
            return query.Where(c => c.ProjectId == projid);
        }

        public JsonResult NLEdit(int id, decimal Mon, decimal Tue, decimal Wed, decimal Thu, decimal Fri, decimal Sat, decimal Sun, decimal Sum)
        {
            return this.Json(EditGetData(id,Mon,Tue,Wed,Thu,Fri,Sat,Sun,Sum), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get ticket data
        /// </summary>
        /// <returns>List of ticket data</returns>
        public IEnumerable<NLTimesheetViewModel> EditGetData(int id, decimal Mon, decimal Tue, decimal Wed, decimal Thu, decimal Fri, decimal Sat, decimal Sun, decimal Sum)
        {
            var query = (from data in _dbContext.TimeSheet
                         where data.Id == id
                         select new NLTimesheetViewModel
                         {
                             TimesheetId = data.Id,
                             EmployeeId = data.EmployeeId,
                             ProjectId = data.ProjectId ?? 0,
                             ActivityId = data.ProjectActivityLinkId ?? 0,
                             ActivityName = data.ProjectActivityLink.ProjectActivityName,
                             Date = data.Date,
                             PhaseId = data.ProjectPhaseId ?? 0,
                             MonActualHrs = Mon,
                             TueActualHrs = Tue,
                             WedActualHrs = Wed,
                             ThuActualHrs = Thu,
                             FriActualHrs = Fri,
                             SatActualHrs = Sat,
                             SunActualHrs = Sun,
                             TotalHrs = Sum,
                             Remark = data.Remarks
                         }
                         ).ToList();
            return query;
        }

        /// <summary>
        /// Method to Edit timesheet
        /// </summary>
        /// <param name="request"></param>
        /// <param name="timesheet">form input</param>
        /// <returns>View</returns>
        public ActionResult EditTimesheet([DataSourceRequest] DataSourceRequest request, TimesheetViewModel timesheet)
        {

            TimeSheet objTimesheet = new TimeSheet();

            objTimesheet = _objTimesheet.FindById(timesheet.TimesheetId);

            objTimesheet.Id = timesheet.TimesheetId;
            objTimesheet.ReferenceIssueNo = timesheet.ReferenceIssueNo;
            objTimesheet.TimeSpent = timesheet.ActualHrs;
            objTimesheet.Remarks = timesheet.Remarks;
            objTimesheet.IsCompleted = false;
            objTimesheet.IsExtraHours = false;
            objTimesheet.Description = timesheet.Description;

            _objTimesheet.Update(objTimesheet);

            return Json(productServiceRead().ToDataSourceResult(request));
        }

        public ActionResult DeleteTimesheet([DataSourceRequest] DataSourceRequest request, TimesheetViewModel timesheet)
        {
            _objTimesheet.DeleteById(timesheet.TimesheetId);
            return Json(productServiceRead().ToDataSourceResult(request));
        }
        
        public IEnumerable<TimesheetViewModel> productServiceRead()
        {
            var query = (from data in _dbContext.ProjectActivityLink
                         select new TimesheetViewModel
                         {
                             ProjectId = data.ProjectId,
                             ActivityId = data.Id,
                             ActivityName = data.ProjectActivityName
                         }).ToList();
            return query.ToList();
        }

        /// <summary>
        /// Deleting row from timesheet
        /// </summary>
        /// <param name="request">Kendo data source request</param>
        /// <param name="sla">Instance of slaview model</param>
        /// <returns>Json for Project SLA</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NLInlineDestroy([DataSourceRequest] DataSourceRequest request, NLTimesheetViewModel NLView)
        {
            if (NLView != null)
            {
                _objTimesheet.DeleteById(NLView.TimesheetId);
            }
            return Json(new[] { NLView }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult GetTimesheet([DataSourceRequest]DataSourceRequest request, string date, int id)
        {
            return Json(TimesheetDetailView(date, id).ToDataSourceResult(request));
        }

        public ActionResult GetTimesheetForNonAdmin([DataSourceRequest]DataSourceRequest request, string date)
        {
            int id = Convert.ToInt32(Session["user"] as string);
            return Json(TimesheetDetailView(date, id).ToDataSourceResult(request));
        }

        public IEnumerable<TimesheetViewModel> TimesheetDetailView(string date, int id)
        {

            DateTime d = Convert.ToDateTime(date);
            var query = (from data in _dbContext.TimeSheet
                             //.Include("Projects")
                             //.Include("ProjectActivityLink")
                             //.Include("ProjectPhases")
                         where data.Date.Day == d.Day
                         && data.Date.Month == d.Month
                         && data.Date.Year == d.Year
                         && data.EmployeeId == id
                         select new TimesheetViewModel
                         {
                             ProjectName = data.Projects.ProjectName,
                             PhaseName = data.ProjectPhases.Phases.Name,
                             ActivityName = data.ProjectActivityLink.ProjectActivityName,
                             ReferenceIssueNo = data.ReferenceIssueNo,
                             EstimatedHrs = data.EstimatedTime ?? 0,
                             ActualHrs = data.TimeSpent,
                             Remarks = data.Remarks,
                             Date = data.Date,
                             TimesheetId = data.Id
                         }).ToList();
            //return query.Where(c => c.Date == date);
            return query;
            //return (from data in _dbContext.TimeSheet
            //        .Include("Projects")
            //        .Include("ProjectActivityLink")
            //        .Include("ProjectPhases")
            //        select new TimesheetViewModel
            //        {
            //            ProjectName = data.Projects.ProjectName,
            //            PhaseName = data.ProjectPhases.PhaseName,
            //            ActivityName = data.ProjectActivityLink.ProjectActivityName,
            //            ReferenceIssueNo = data.ReferenceIssueNo,
            //            EstimatedHrs = data.EstimatedTime ?? 0,
            //            ActualHrs = data.TimeSpent,
            //            Remarks = data.Remarks
            //        }).Where(x => x.Date == date).Select(grp => grp.FirstOrDefault()).ToList();
            //        

        }

        public ActionResult NLGetTimeSheet([DataSourceRequest]DataSourceRequest request, string date, int id)
        {
            return Json(NLTimesheetDetailView(date, id).ToDataSourceResult(request));
        }

        public IEnumerable<NLTimesheetViewModel> NLTimesheetDetailView(string date, int id)
        {
            List<NLTimesheetViewModel> _timeData = new List<NLTimesheetViewModel>();

            int week = GetWeek(date);
            DateTime d = Convert.ToDateTime(date);
            DateTime selectedDateMon = YearWeekDayToDateTime(d.Year, DayOfWeek.Monday, week);
            DateTime weekEndDate = selectedDateMon.AddDays(6);

            var timeSheets = (from t in _dbContext.TimeSheet.Include("Projects")
                              where t.Employee.Id == id && DbFunctions.TruncateTime(t.Date) >= selectedDateMon && DbFunctions.TruncateTime(t.Date) <= weekEndDate
                              orderby t.Projects.ProjectName, t.ProjectActivityLink.ProjectActivityName
                              select new { TimeSheetId = t.Id, EmployeeId = t.EmployeeId, ProjectId = t.ProjectId, PhaseId = t.ProjectPhaseId, ActivityId = t.ProjectActivityLinkId, ProjectCode = t.Projects.ProjectCode, ProjectName = t.Projects.ProjectName, ActivityName = t.ProjectActivityLink.ProjectActivityName, Date = t.Date, TimeSpent = t.TimeSpent, Remark = t.Remarks, ProjectPhase = (t.ProjectPhases == null ? "" : t.ProjectPhases.PhaseName) });
            _timeData.Clear();
            NLTimesheetViewModel timeData;


            foreach (var timesheet in timeSheets)
            {
                timeData = new NLTimesheetViewModel();
                timeData.TimesheetId = timesheet.TimeSheetId;
                timeData.EmployeeId = timesheet.EmployeeId;
                timeData.ProjectId = timesheet.ProjectId ?? 0;
                timeData.PhaseId = timesheet.PhaseId ?? 0;
                timeData.ActivityId = timesheet.ActivityId ?? 0;
                timeData.ProjectCode = timesheet.ProjectCode;
                timeData.ProjectName = timesheet.ProjectName;
                timeData.ActivityName = timesheet.ActivityName;
                timeData.Remark = timesheet.Remark;
                timeData.PhaseName = timesheet.ProjectPhase;

                if (timeData != null)
                {
                    timeData.WeekFirstDate = selectedDateMon;

                    switch (timesheet.Date.DayOfWeek)
                    {
                        case DayOfWeek.Friday:
                            timeData.FriActualHrs += timesheet.TimeSpent;
                            break;
                        case DayOfWeek.Monday:
                            timeData.MonActualHrs += timesheet.TimeSpent;
                            break;
                        case DayOfWeek.Saturday:
                            timeData.SatActualHrs += timesheet.TimeSpent;
                            break;
                        case DayOfWeek.Sunday:
                            timeData.SunActualHrs += timesheet.TimeSpent;
                            break;
                        case DayOfWeek.Thursday:
                            timeData.ThuActualHrs += timesheet.TimeSpent;
                            break;
                        case DayOfWeek.Tuesday:
                            timeData.TueActualHrs += timesheet.TimeSpent;
                            break;
                        case DayOfWeek.Wednesday:
                            timeData.WedActualHrs += timesheet.TimeSpent;
                            break;
                        default:
                            break;
                    }
                    timeData.Remark = timesheet.Remark;
                    timeData.TotalHrs += timesheet.TimeSpent;
                }
                _timeData.Add(timeData);
            }
            return _timeData;
        }
        
        public ActionResult GetTimesheetSummeryGrid([DataSourceRequest] DataSourceRequest request, int week, int EmployeeId, int ProjectId)
        {
            return Json(TimesheetSummeryGridView(week, EmployeeId, ProjectId).ToDataSourceResult(request));
        }

        public IEnumerable<TimesheetSummaryReportViewModel> TimesheetSummeryGridView(int week, int EmployeeId, int ProjectId)
        {
            // Create List of view model
            List<TimesheetSummaryReportViewModel> data = new List<TimesheetSummaryReportViewModel>();
            // Counter
            int cnt = 0;

            // condition if user select Employee and project both
            if (EmployeeId != 0 && ProjectId != 0)
            {
                // fetch all row that satisfied condition
                var query = (from row in _dbContext.TimeSheet
                             where row.WeekNo == week && EmployeeId == row.EmployeeId && ProjectId == row.ProjectId
                             select new TimesheetSummaryReportViewModel
                             {
                                 EmployeeId = row.EmployeeId,
                                 EmployeeName = row.Employee.FiistName + " " + row.Employee.LastName ,
                                 WeekNo = row.WeekNo,
                                 Date = row.Date,
                                 shortcode = row.Remarks,
                                 TotalHour = row.TimeSpent
                             });

                data.Clear();

                // Instance for View model
                TimesheetSummaryReportViewModel Timedata;
                // To check whether it's first element of loop or not
                int i = 0;
                // Stores previous date
                DateTime PrevDate = DateTime.Now;
                // Store number of hour employee spend on given project at given day
                decimal dayhour = 0;

                Timedata = new TimesheetSummaryReportViewModel();

                foreach (var timesheettemp in query)
                {
                    cnt++;

                    // Enters fist time in loop
                    if (i == 0)
                    {
                        PrevDate = timesheettemp.Date;
                        dayhour = timesheettemp.TotalHour + dayhour;
                        i++;
                    }
                    else
                    {
                        i = 2;
                    }

                    // if have 2 entries for same project on given day
                    if (PrevDate == timesheettemp.Date && i == 2)
                    {
                        dayhour = timesheettemp.TotalHour + dayhour;
                    }

                    // Day change
                    // Reset all temparary variable
                    else if (PrevDate != timesheettemp.Date && i != 1)
                    {
                        PrevDate = timesheettemp.Date;
                        Timedata.TotalHour = dayhour;
                        dayhour = 0;
                        data.Add(Timedata);
                        dayhour = timesheettemp.TotalHour;
                    }
                    Timedata = timesheettemp;
                } // Foreach ends
                Timedata.TotalHour = dayhour;

                if (cnt > 0)
                    data.Add(Timedata);

                return data;
            }

            // condition if user select only employee
            else if (EmployeeId != 0)
            {
                // fetch all row that week for given employee
                var query = (from row in _dbContext.TimeSheet
                             where row.WeekNo == week && row.EmployeeId == EmployeeId
                             orderby row.ProjectId
                             select new TimesheetSummaryReportViewModel
                             {
                                 EmployeeId = row.EmployeeId,
                                 EmployeeName = row.Employee.FiistName + " " + row.Employee.LastName ,
                                 WeekNo = row.WeekNo,
                                 Date = row.Date,
                                 shortcode = row.Remarks,
                                 TotalHour = row.TimeSpent,
                                 ProjectName = row.Projects.ShortCode,
                                 ProjectId = row.Projects.Id
                             });

                data.Clear();

                // Instance for View model
                TimesheetSummaryReportViewModel Timedata;
                // To check whether it's first element of loop or not
                int i = 0;
                // Stores previous date
                DateTime PrevDate = DateTime.Now;
                // Store number of hour employee spend on given project at given day
                decimal dayhour = 0;
                int prevProjectId = 0;

                Timedata = new TimesheetSummaryReportViewModel();

                foreach (var timesheettemp in query)
                {
                    cnt++;

                    // Enters fist time in loop
                    if (i == 0)
                    {
                        prevProjectId = timesheettemp.ProjectId;
                        PrevDate = timesheettemp.Date;
                        dayhour = timesheettemp.TotalHour + dayhour;
                        i++;
                    }
                    else
                    {
                        i = 2;
                    }

                    // if have same day & project as previous row
                    if (PrevDate == timesheettemp.Date && prevProjectId == timesheettemp.ProjectId && i == 2)
                    {
                        dayhour = timesheettemp.TotalHour + dayhour;
                    }

                    // If Date or Employee chagne in given row
                    // Reset all temparary variable
                    else if ((PrevDate != timesheettemp.Date || prevProjectId != timesheettemp.ProjectId) && i != 1)
                    {
                        PrevDate = timesheettemp.Date;
                        prevProjectId = timesheettemp.ProjectId;
                        Timedata.TotalHour = dayhour;
                        dayhour = 0;
                        data.Add(Timedata);
                        dayhour = timesheettemp.TotalHour;
                    }
                    Timedata = timesheettemp;
                } // For each ends 
                Timedata.TotalHour = dayhour;

                if (cnt > 0)
                    data.Add(Timedata);

                return data;
            }

            // condition if user select none
            // Display weekly report of all employees
            else
            {
                // fetch all row of that week
                var query = (from row in _dbContext.TimeSheet
                             where row.WeekNo == week
                             orderby row.EmployeeId, row.ProjectId
                             select new TimesheetSummaryReportViewModel
                             {
                                 EmployeeId = row.EmployeeId,
                                 EmployeeName = row.Employee.FiistName + " " + row.Employee.LastName,
                                 WeekNo = row.WeekNo,
                                 Date = row.Date,
                                 shortcode = row.Remarks,
                                 TotalHour = row.TimeSpent
                             });

                data.Clear();

                // Instance for View model
                TimesheetSummaryReportViewModel Timedata;
                // To check whether it's first element of loop or not
                int i = 0;
                // Stores previous date
                DateTime PrevDate = DateTime.Now;
                // Store number of hour employee spend on given project at given day
                decimal dayhour = 0;
                // Store ProjectId of previous row
                int prevProjectId = 0;
                // Store employeeid of previous row
                int prevEmployeeId = 0;
                Timedata = new TimesheetSummaryReportViewModel();

                foreach (var timesheettemp in query)
                {
                    cnt++;
                    // Enters fist time in loop
                    if (i == 0)
                    {
                        prevEmployeeId = timesheettemp.EmployeeId;
                        prevProjectId = timesheettemp.ProjectId;
                        PrevDate = timesheettemp.Date;
                        dayhour = timesheettemp.TotalHour + dayhour;
                        i++;
                    }
                    else
                    {
                        i = 2;
                    }

                    // True if 2 entries are there for given employee and project on same day
                    if (PrevDate == timesheettemp.Date && prevProjectId == timesheettemp.ProjectId && prevEmployeeId == timesheettemp.EmployeeId && i == 2)
                    {
                        dayhour = timesheettemp.TotalHour + dayhour;
                    }

                    // Project or Employee or day change
                    // Reset all data
                    else if ((PrevDate != timesheettemp.Date || prevProjectId != timesheettemp.ProjectId || prevEmployeeId != timesheettemp.EmployeeId) && i != 1)
                    {
                        PrevDate = timesheettemp.Date;
                        prevProjectId = timesheettemp.ProjectId;
                        prevEmployeeId = timesheettemp.EmployeeId;
                        Timedata.TotalHour = dayhour;
                        dayhour = 0;
                        data.Add(Timedata);
                        dayhour = timesheettemp.TotalHour;
                    }
                    Timedata = timesheettemp;
                } //foreach ends 

                Timedata.TotalHour = dayhour;

                if (cnt > 0)
                    data.Add(Timedata);//insert last row in list

                return data;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteEntry([DataSourceRequest] DataSourceRequest request, TimesheetViewModel ViewModel)
        {

            if (ViewModel != null)
            {
                //Find by timesheet id
                var TimesheetDelete = (from data in _dbContext.TimeSheet
                                       where data.Id == ViewModel.TimesheetId
                                       select data.Id).FirstOrDefault();
                _objTimesheet.DeleteById(TimesheetDelete);
            }

            return Json(new[] { ViewModel }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult EditTimesheetIndia(int id)
        {
            return this.Json(GetEditTimesheetIndia(id), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get ticket data
        /// </summary>
        /// <returns>List of ticket data</returns>
        public IEnumerable<TimesheetViewModel> GetEditTimesheetIndia(int id)
        {
            DateTime cureentDate = DateTime.Now.Date;
            var query = (from data in _dbContext.TimeSheet
                         where data.Id == id && data.ProjectActivity.Id != 34
                         select new TimesheetViewModel
                         {
                             TimesheetId = data.Id,
                             EmployeeId = data.EmployeeId,
                             EmployeeName = data.Employee.FiistName + " " + data.Employee.LastName,
                             ProjectId = data.ProjectId ?? 0,
                             ProjectName = data.Projects.ProjectName,
                             ActivityId = data.ProjectActivityLinkId ?? 0,
                             ActivityName = data.ProjectActivityLink.ProjectActivityName,
                             Date = data.Date,
                             PhaseId = data.ProjectPhaseId ?? 0,
                             PhaseName = data.ProjectPhases.PhaseName,
                             ReferenceIssueNo = data.ReferenceIssueNo,
                             EstimatedHrs = data.EstimatedTime ?? 0,
                             ActualHrs = data.TimeSpent,
                             Description = data.Description,
                             Remarks = data.Remarks,
                             currentTempDate = cureentDate
                         }
                         ).ToList();
            return query;
        }

        #endregion


        /****** Code for Timesheet Report
         * begin from here
         * *******/

        #region Action Methods
        /// <summary>
        /// Action method for Timesheet Report page
        /// </summary>
        /// <returns>View of that page</returns>
        public ActionResult TimesheetReport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TimesheetReport (TimesheetReportViewModel forminput)
        {
            return View();
        }
        #endregion

    }
}
