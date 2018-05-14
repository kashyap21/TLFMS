using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TLFSupport.Model.DatabaseModel;
using TLFSupport.Model.DatabaseModel.Repositories;

namespace TLFSupport.EmailReader.Common
{
    /// <summary>
    /// Holds static variables and paths of logs file
    /// </summary>
    public static class CommonProperty
    {
        #region Static Member
        /// <summary>
        /// Instance of generic repository
        /// </summary>
        private static GenericRepository<CS_Ticket> _ticket;

        /// <summary>
        /// Instance of generic repository
        /// </summary>
        private static GenericRepository<CS_Log> _log;

        /// <summary>
        /// Instance of generic repository
        /// </summary>
        private static GenericRepository<CS_EmailLink> _link;

        /// <summary>
        /// Instance of generic repository
        /// </summary>
        private static GenericRepository<CS_EmailTicketLog> _existTicket;

        /// <summary>
        /// Keeps link detail
        /// </summary>
        public static CS_EmailLink LinkDetail = new CS_EmailLink();

        /// <summary>
        /// Keeps new ticket detail
        /// </summary>
        public static CS_Ticket TicketDetail = new CS_Ticket();

        /// <summary>
        /// Keeps log detail
        /// </summary>
        public static CS_Log LogDetail = new CS_Log();

        /// <summary>
        /// Existing ticket detail
        /// </summary>
        public static CS_EmailTicketLog ExistTicketDetail = new CS_EmailTicketLog();

        /// <summary>
        /// Database context
        /// </summary>
        public static TLFCSEntities dbContext = new TLFCSEntities();

        /// <summary>
        /// List for ticket escalation
        /// </summary>
        public static List<Escalation> EscalationList = new List<Escalation>();
        
        /// <summary>
        /// Temporary list to keep entries to be deleted
        /// </summary>
        public static List<Escalation> TempList = new List<Escalation>();

        /// <summary>
        /// Number of mail in inbox
        /// </summary>
        public static int GlobalEmailCount = 0;

        /// <summary>
        /// MIME host
        /// </summary>
        public static string MIMEHost = dbContext.CS_Configuration.Select(i => i.MIMEHost).First();

        /// <summary>
        /// MIME Port
        /// </summary>
        public static int MIMEPort = dbContext.CS_Configuration.Select(i => i.MIMEPort).First();

        /// <summary>
        /// Email address for support
        /// </summary>
        public static string SupportEMailAddress = dbContext.CS_Configuration.Select(i => i.SupportEmailAddress).First();

        /// <summary>
        /// Password for support address
        /// </summary>
        public static string Password = dbContext.CS_Configuration.Select(i => i.Password).First();

        /// <summary>
        /// Email address for support
        /// </summary>
        public static string SLABreachEmailAddress = dbContext.CS_Configuration.Select(i => i.SLABreachEmailAddress).First();

        /// <summary>
        /// Get instance of generic repository
        /// </summary>
        public static GenericRepository<CS_EmailLink> Link
        {
            get
            {
                if (_link == null)
                {
                    _link = new GenericRepository<CS_EmailLink>();
                }
                return _link;
            }
        }

        /// <summary>
        /// Get instance of generic repository
        /// </summary>
        public static GenericRepository<CS_Ticket> Ticket
        {
            get
            {
                if (_ticket == null)
                {
                    _ticket = new GenericRepository<CS_Ticket>();
                }
                return _ticket;
            }
        }

        /// <summary>
        /// Get instance of generic repository
        /// </summary>
        public static GenericRepository<CS_Log> Log
        {
            get
            {
                if (_log == null)
                {
                    _log = new GenericRepository<CS_Log>();
                }
                return _log;
            }
        }

        /// <summary>
        /// Get instance of generic repository
        /// </summary>
        public static GenericRepository<CS_EmailTicketLog> ExistTicket
        {
            get
            {
                if (_existTicket == null)
                {
                    _existTicket = new GenericRepository<CS_EmailTicketLog>();
                }
                return _existTicket;
            }
        }
        /// <summary>
        /// Get current dictionary information
        /// </summary>
        public static DirectoryInfo info = new DirectoryInfo(".");

        /// <summary>
        /// Path to store email count on stopping service
        /// </summary>
        public static string LastEmailCountPath = info.FullName + "\\LastEmailCount.txt";

        /// <summary>
        /// Path to incoming call log file
        /// </summary>
        public static string IncomingEmailFilePath = info.FullName + "\\IncomingEmailLogs.txt";

        /// <summary>
        /// Path to email error log file
        /// </summary>
        public static string EmailErrorLogFilePath = info.FullName + "\\EmailErrorLogs.txt";

        /// <summary>
        /// Path to escalation error log file
        /// </summary>
        public static string EscalationErrorFilePath = info.FullName + "\\EscalationErrorLogs.txt";
        #endregion
    }

    /// <summary>
    /// Represents fields required for escalation list
    /// </summary>
    public class Escalation
    {
        #region Fields
        public CS_Ticket EscTicketDetail = new CS_Ticket(); //Ticket object
        public DateTime EscalationExpiryTime = new DateTime();  //Level expiry time
        public double EscalationTime;   //Time of escalation that need to be added
        public int EscalationLevel; //Total level of escalation
        //public int CurrentLevel;    //Current level of ticket
        #endregion
    }
}
