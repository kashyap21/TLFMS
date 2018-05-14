using System.ServiceProcess;
using System.Threading;
using TLFSupport.CallReader.BusinessLogic;

namespace TLFSupport.CallReader
{
    /// <summary>
    /// Implements service for call reader
    /// </summary>
    public partial class CallLogs : ServiceBase
    {
        #region Private Member
        /// <summary>
        /// Calls class object
        /// </summary>
        private Calls _calls;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public CallLogs()
        {
            //Instance creation of calls object
            _calls = new Calls();
            InitializeComponent();
        }
        #endregion

        #region Service Methods
        /// <summary>
        /// On stopping service, this function is executed
        /// </summary>
        protected override void OnStart(string[] args)
        {
            //Thread for call reading
            Thread CallReader = new Thread(_calls.IncomingCallReader);

            //Start threads
            CallReader.Start();
        }

        /// <summary>
        /// On stopping service, this function is executed
        /// </summary>
        protected override void OnStop()
        {
        }
        #endregion
    }
}
