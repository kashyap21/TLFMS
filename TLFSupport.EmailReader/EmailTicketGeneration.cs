using OpenPop.Pop3;
using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using TLFSupport.EmailReader.BusinessLogic;
using TLFSupport.EmailReader.Common;

namespace TLFSupport.EmailReader
{
    /// <summary>
    /// Implements service for email reading
    /// </summary>
    public partial class EmailTicketGeneration : ServiceBase
    {
        #region Private Member

        /// <summary>
        /// Emails class object
        /// </summary>
        private Emails _email;

        /// <summary>
        /// Pop3Client class object
        /// </summary>
        private Pop3Client _client;

        #endregion Private Member

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public EmailTicketGeneration()
        {
            //Instance creation of calls object
            _email = new Emails();
            //Instance of Pop client
            _client = new Pop3Client();

            //Connect to the server
            _client.Connect(CommonProperty.MIMEHost,CommonProperty.MIMEPort,true);

            //Authenticate ourselves towards the server
            _client.Authenticate(CommonProperty.SupportEMailAddress,CommonProperty.Password);

            //Number of mail in inbox
            CommonProperty.GlobalEmailCount = _client.GetMessageCount();

            //If service stopped in between last email count takes care of missed emails
            if (File.Exists(CommonProperty.LastEmailCountPath))
            {
                StreamReader sr = new StreamReader(CommonProperty.LastEmailCountPath);
                int testCount = Convert.ToInt32(sr.ReadLine());
                if (testCount != CommonProperty.GlobalEmailCount)
                {
                    CommonProperty.GlobalEmailCount = testCount;
                }
            }

            InitializeComponent();
        }

        #endregion Constructor

        #region Service Methods

        /// <summary>
        /// On starting service this method is executed
        /// </summary>
        /// <param name="args">Default arguments</param>
        protected override void OnStart(string[] args)
        {
            //Thread for email
            Thread EmailReader = new Thread(_email.ReadIncomingEmail);
            //Thread for escalation
            Thread Escalation = new Thread(_email.PerformEscalation);

            //Starts all threads
            EmailReader.Start();
            Escalation.Start();
        }

        /// <summary>
        /// On stopping service this method is executed
        /// </summary>
        protected override void OnStop()
        {
            StreamWriter sw = new StreamWriter(CommonProperty.LastEmailCountPath);
            sw.WriteLine(CommonProperty.GlobalEmailCount);
            sw.Flush();
            sw.Close();
        }

        #endregion Service Methods
    }
}