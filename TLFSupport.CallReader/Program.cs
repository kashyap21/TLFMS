using System.ServiceProcess;

namespace TLFSupport.CallReader
{
    /// <summary>
    /// Program execution starts
    /// </summary>
    public static class Program
    {
        #region Main Method
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            //Provides base class for service that exist as part of service application
            ServiceBase[] ServicesToRun;

            //Base class instantiation
            ServicesToRun = new ServiceBase[] 
            { 
                new CallLogs() 
            };

            //Register executable for service
            ServiceBase.Run(ServicesToRun);
        }
        #endregion
    }
}
