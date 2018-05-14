using System.ServiceProcess;

namespace TLFSupport.EmailReader
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
            ServiceBase[] ServicesToRun;    //Base class for service as part of service application
            ServicesToRun = new ServiceBase[] 
            { 
                new EmailTicketGeneration() 
            };
            //Starts service
            ServiceBase.Run(ServicesToRun);
        }
        #endregion
    }
}
