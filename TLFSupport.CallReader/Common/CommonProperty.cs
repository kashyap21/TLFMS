using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using TLFSupport.Common;
using TLFSupport.Model.DatabaseModel;
using TLFSupport.Model.DatabaseModel.Repositories;

namespace TLFSupport.CallReader.Common
{
    /// <summary>
    /// Holds static variables and paths of logs file
    /// </summary>
    public class CommonProperty
    {
        #region ENUM
        /// <summary>
        /// Enum for call action
        /// </summary>
        public enum _action { MISSED = 0, ATTENDED };
        #endregion ENUM

        #region Static Member
        /// <summary>
        /// Instance of generic repository
        /// </summary>
        private static GenericRepository<CS_CallLog> _callLog;

        /// <summary>
        /// Context of entity
        /// </summary>
        private static TLFCSEntities _dbContext = new TLFCSEntities();

        /// <summary>
        /// PBX IP addreess
        /// </summary>
        private static string PBX_IP = _dbContext.CS_Configuration.Select(i => i.PBX_IP).First();

        /// <summary>
        /// PBX port
        /// </summary>
        private static int PBX_Port = _dbContext.CS_Configuration.Select(i => i.PBX_Port).First();

        /// <summary>
        /// Instance of Call log
        /// </summary>
        public static CS_CallLog CallDetail = new CS_CallLog();

        /// <summary>
        /// Host address information
        /// </summary>
        public static IPHostEntry IpHost = Dns.GetHostEntry(Dns.GetHostName());

        /// <summary>
        /// IP address of local host
        /// </summary>
        public static IPAddress IpAddr = IpHost.AddressList[1];

        /// <summary>
        /// TCP client declaration, client communicates from provided IP and port
        /// </summary>
        public static TcpClient ClientSocket = new TcpClient(PBX_IP, 23);

        /// <summary>
        /// Port number to communicate incoming call logs
        /// </summary>
        public static int Port = PBX_Port;

        /// <summary>
        /// Get instance of generic repository
        /// </summary>
        public static GenericRepository<CS_CallLog> CallLog
        {
            get
            {
                if (_callLog == null)
                {
                    _callLog = new GenericRepository<CS_CallLog>();
                }
                return _callLog;
            }
        }
        #endregion Static Member

        #region Log Path
        /// <summary>
        /// Get current dictionary information
        /// </summary>
        public static DirectoryInfo info = new DirectoryInfo(".");

        /// <summary>
        /// Path to incoming call log file
        /// </summary>
        public static string IncomingCallFilePath = info.FullName + "\\IncomingCallLogs.txt";

        /// <summary>
        /// Path to error log file
        /// </summary>
        public static string ErrorLogFilePath = info.FullName + "\\CallErrorLogs.txt";
        #endregion Log Path
    }
}