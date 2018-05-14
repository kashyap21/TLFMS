using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using TLFSupport.Model.DatabaseModel;

namespace TLFSupport.Common
{
    /// <summary>
    /// Sends acknowledgment mail to customer
    /// </summary>
    public static class SendMail
    {
        #region Private Member
        /// <summary>
        /// Database context
        /// </summary>
        private static TLFCSEntities _dbContext = new TLFCSEntities();

        /// <summary>
        /// Password of support address
        /// </summary>
        private static string Password = _dbContext.CS_Configuration.Select(i => i.Password).First();

        /// <summary>
        /// Support email address
        /// </summary>
        private static string SupportEmailAddress = _dbContext.CS_Configuration.Select(i => i.SupportEmailAddress).First();

        /// <summary>
        /// SMTP host
        /// </summary>
        private static string SMTPHost = _dbContext.CS_Configuration.Select(i => i.SMTPHost).First();
        #endregion

        #region Public Method
        public static void SendAcknowledgement(string EmailFrom, string subject, string body, string tNumber, string priority, string status)
        {
            try
            {
                MailMessage mail = new MailMessage();   //Represents an email message
                mail.To.Add(EmailFrom);                 //Recipients email address

                mail.From = new System.Net.Mail.MailAddress(SupportEmailAddress); //Senders email address

                mail.Subject = subject + tNumber + "##]" + " issue status - " + status;  //Subject of email

                mail.Body = body
                           + "</br><b>Priority:</b> " + priority
                           + "</br><b>Current status:</b> " + status; //Body of email

                mail.IsBodyHtml = true; //Mail body is set to HTML

                SmtpClient smtp = new SmtpClient();   //Allows application to send email using SMTP
                smtp.Host = SMTPHost;   //Sets IP address of host

                //Authenticate sender
                smtp.Credentials = new System.Net.NetworkCredential(SupportEmailAddress, Password);
                smtp.EnableSsl = true;  //Enable secure socket layer connection

                smtp.Send(mail);    //Sends mail
            }
            catch (Exception ex)
            {
            }
            
        }

        public static void SendAcknowledgement(List<string> employeeEmail, string subject, string body, string tNumber, string priority, string status)
        {
            foreach (var address in employeeEmail)
            {
                SendAcknowledgement(address, subject, body, tNumber, priority, status);
            }
        }

        #endregion
    }
}