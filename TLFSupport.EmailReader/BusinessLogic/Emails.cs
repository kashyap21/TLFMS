using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using TLFSupport.Common;
using TLFSupport.EmailReader.Common;

namespace TLFSupport.EmailReader.BusinessLogic
{
    /// <summary>
    /// Implements email reading, ticket generation
    /// </summary>
    public class Emails
    {
        #region Public Methods

        /// <summary>
        /// Read new incoming mail
        /// </summary>
        public void ReadIncomingEmail()
        {
            while (true)
            {
                // The client disconnects from the server when being disposed
                using (Pop3Client clientTest = new Pop3Client())
                {
                    Authenticate(clientTest);   //Authenticate POP client

                    int NewEmailCount = 0;  //Number of new emails
                    int CurrentEmailCount = clientTest.GetMessageCount();   //Current number of emails in inbox
                    string EmailFrom = string.Empty;    //Sender of mail

                    //New mail arrived
                    if (CommonProperty.GlobalEmailCount < CurrentEmailCount)
                    {
                        //Number of new emails
                        NewEmailCount = CurrentEmailCount - CommonProperty.GlobalEmailCount;

                        //Stores all new mails
                        List<Message> allMessages = new List<Message>(NewEmailCount);

                        //Fetch details of all new mail
                        for (int i = CommonProperty.GlobalEmailCount + 1; i <= CurrentEmailCount; i++)
                        {
                            allMessages.Add(clientTest.GetMessage(i));
                        }

                        try
                        {
                            foreach (var mail in allMessages)
                            {
                                CommonProperty.TicketDetail.CreatedOn = DateTime.Now;   //Date of receiving email
                                EmailFrom = mail.Headers.From.Address;  //Sender of mail
                                CommonProperty.TicketDetail.Title = mail.Headers.Subject; //Subject of mail

                                //Body contains plain text
                                if (mail.FindFirstPlainTextVersion() != null)
                                {
                                    CommonProperty.TicketDetail.Description = mail.FindFirstPlainTextVersion().GetBodyAsText();
                                }
                                //Body contains HTML
                                else
                                {
                                    CommonProperty.TicketDetail.Description = mail.FindFirstHtmlVersion().GetBodyAsText();
                                }
                                //Checks subject of mail. If ticket is generated no new ticket generation
                                if (CommonProperty.TicketDetail.Title.Contains("RequestID:##"))
                                {
                                    ExistTicketEntry(EmailFrom, CommonProperty.TicketDetail.Title); //Keeps log of communication for ticket
                                }
                                else
                                {
                                    GenerateTicket(EmailFrom);  //New ticket generation
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            StreamWriter sw = new StreamWriter(CommonProperty.EmailErrorLogFilePath, true);
                            sw.WriteLine(ex.ToString());
                            sw.Flush();
                            sw.Close();
                        }
                        //CommonProperty.GlobalEmailCount = CurrentEmailCount;     //Update global mail count to latest value
                        allMessages.Clear();    //Clears list
                    }
                    Thread.Sleep(2000); //Wait for 2 sec
                    CommonProperty.GlobalEmailCount = clientTest.GetMessageCount(); //Update global mail count to latest value
                }
            }
        }

        /// <summary>
        /// Performs escalation of ticket
        /// </summary>
        public void PerformEscalation()
        {
            while (true)
            {
                try
                {
                    //Iterates through all entries of list
                    foreach (var listItem in CommonProperty.EscalationList)
                    {
                        //Finds current status of ticket
                        int StatusId = (from s in CommonProperty.dbContext.CS_Ticket
                                        where s.TicketId == listItem.EscTicketDetail.TicketId
                                        select s.StatusId).Single();
                        //Escalates to next level
                        if (listItem.EscalationExpiryTime < DateTime.Now &&
                            StatusId == 1)
                        {
                            listItem.EscalationExpiryTime = listItem.EscalationExpiryTime.AddMinutes(listItem.EscalationTime);  //Changes expiry time to next
                            //Ticket not reached to highest level
                            string priority = CommonProperty.dbContext.CS_SLA.Where(p => p.SLAId == listItem.EscTicketDetail.SLAId).Select(a => a.PriorityName).Single();
                            string status = CommonProperty.dbContext.CS_Status.Where(s => s.StatusId == listItem.EscTicketDetail.StatusId).Select(a => a.Status).Single();
                            if (listItem.EscalationLevel > 1)
                            {
                                listItem.EscalationLevel -= 1; //Increments next level
                                //Finds employee of project with next level
                                listItem.EscTicketDetail.EmployeeId = (from e in CommonProperty.dbContext.ProjectTeam
                                                                       where e.ProjectId == listItem.EscTicketDetail.ProjectId
                                                                       && e.CS_Level == listItem.EscalationLevel
                                                                       select e.EmployeeId).FirstOrDefault();

                                //Updates ticket entry
                                CommonProperty.Ticket.Update(listItem.EscTicketDetail);
                                //Store log details
                                CommonProperty.LogDetail.CreatedOn = DateTime.Now;
                                CommonProperty.LogDetail.EmployeeId = listItem.EscTicketDetail.EmployeeId;
                                CommonProperty.LogDetail.Title = listItem.EscTicketDetail.Title;
                                CommonProperty.LogDetail.Comment = "Ticket Escalted";
                                CommonProperty.LogDetail.StatusId = StatusId;
                                CommonProperty.LogDetail.TicketId = listItem.EscTicketDetail.TicketId;
                                CommonProperty.LogDetail.CustumerId = listItem.EscTicketDetail.CustomerId;
                                CommonProperty.LogDetail.PriorityName = (from p in CommonProperty.dbContext.CS_SLA
                                                                         where p.SLAId == listItem.EscTicketDetail.SLAId
                                                                         select p.PriorityName).SingleOrDefault();
                                //Insert log details
                                CommonProperty.Log.Insert(CommonProperty.LogDetail);

                                int[] arr = CommonProperty.dbContext.ProjectTeam.Where(e => e.ProjectId == listItem.EscTicketDetail.ProjectId && e.CS_Level == listItem.EscalationLevel).Select(a => a.EmployeeId).ToArray();
                                var employeeEmail = CommonProperty.dbContext.Employee.Where(p => arr.Contains(p.Id)).Select(a => a.Email_Official).ToList();

                                CommonProperty.LinkDetail.TicketId = CommonProperty.dbContext.CS_EmailLink.Where(t => t.TicketId == listItem.EscTicketDetail.TicketId).Select(a => a.TicketId).Single();
                                CommonProperty.LinkDetail.Link = Utility.GenerateLink(CommonProperty.LinkDetail.TicketId);
                                CommonProperty.Link.Update(CommonProperty.LinkDetail);

                                string employeeBody = EmailContent.EmployeeBody(CommonProperty.LinkDetail.Link);

                                SendMail.SendAcknowledgement(employeeEmail, EmailContent.EmployeeSubject, employeeBody, listItem.EscTicketDetail.TicketNumber, CommonProperty.LogDetail.PriorityName, "OPEN");

                                //string employeeEmail = CommonProperty.dbContext.Employee.Where(e => e.Id == listItem.EscTicketDetail.EmployeeId).Select(a => a.Email_Official).Single();
                                //SendMail.MailToEmployee(employeeEmail,listItem.EscTicketDetail.TicketNumber);
                            }
                            else
                            {
                                CommonProperty.TempList.Add(listItem);  //All escalation level is finished
                                //Sends mail to authority person
                                // SendMail.SLABreach(listItem.EscTicketDetail.TicketNumber);
                                SendMail.SendAcknowledgement(CommonProperty.SLABreachEmailAddress, EmailContent.SLAbreechSubject, EmailContent.SLAbreechBody, listItem.EscTicketDetail.TicketNumber, CommonProperty.LogDetail.PriorityName, "OPEN");
                            }
                        }
                        else
                        {
                            //Status changed from open
                            if (StatusId != 1)
                            {
                                CommonProperty.TempList.Add(listItem);
                            }
                        }
                    }
                    //Removes unnecessary entries
                    foreach (var item in CommonProperty.TempList)
                    {
                        CommonProperty.EscalationList.Remove(item);
                    }
                }
                catch (Exception ex)
                {
                    StreamWriter sw = new StreamWriter(CommonProperty.EscalationErrorFilePath, true);
                    sw.WriteLine(ex.ToString());
                    sw.Flush();
                    sw.Close();
                }
                Thread.Sleep(5000);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Authenticates POP client
        /// </summary>
        /// <param name="clientTest">POP3Client object</param>
        private void Authenticate(Pop3Client clientTest)
        {
            //Connect to the server
            clientTest.Connect(CommonProperty.MIMEHost,CommonProperty.MIMEPort,true);

            // Authenticate ourselves towards the server
            clientTest.Authenticate(CommonProperty.SupportEMailAddress,CommonProperty.Password);
        }

        /// <summary>
        /// Stores conversation for existing ticket
        /// </summary>
        /// <param name="EmailFrom">Sender of email</param>
        /// <param name="Subject">Subject  of email</param>
        private void ExistTicketEntry(string EmailFrom, string Subject)
        {
            CommonProperty.ExistTicketDetail.Date = CommonProperty.TicketDetail.CreatedOn;  //Date of email arrival
            CommonProperty.ExistTicketDetail.EmailFrom = EmailFrom; //Sender of email

            //Separates ticket number from subject
            string ticketNumber = Subject.Substring(Subject.IndexOf('#') + 2);
            ticketNumber = ticketNumber.Substring(0, ticketNumber.IndexOf('#'));
            CommonProperty.ExistTicketDetail.TicketNumber = Convert.ToInt32(ticketNumber);  //Ticket number

            var ticketExist = from t in CommonProperty.dbContext.CS_Ticket
                              where t.TicketNumber == ticketNumber
                              select t.TicketId;
            //Ticket number is found
            if (ticketExist != null)
            {
                CommonProperty.ExistTicketDetail.Description = CommonProperty.TicketDetail.Description; //Body of mail
                CommonProperty.ExistTicket.Insert(CommonProperty.ExistTicketDetail);    //Insert in database
            }
            else
            {
                GenerateTicket(EmailFrom);  //New ticket generation
            }
        }

        /// <summary>
        /// Generates ticket
        /// </summary>
        /// <param name="EmailFrom">Sender of email</param>
        private void GenerateTicket(string EmailFrom)
        {
            MailAddress address = new MailAddress(EmailFrom);   //Represents address of mail sender or recipient

            string DomainName = "@" + address.Host; //Domain name of sender
            //Count customer with given domain name
            var customer = (from c in CommonProperty.dbContext.Customer
                            where c.CS_DomainName == DomainName
                            select c.Id);

            var custFromEmail = (from e in CommonProperty.dbContext.CS_CustomerEmployeeInfo
                                 where e.EmailId == EmailFrom
                                 select e.CustomerId);

            //Customer identification. CustomerCount = 0 => Not identified
            if (customer.Count() == 0 && custFromEmail.Count() == 0)
            {
                GenerateDefaultTicket(1);   //Insert values for default customer
            }
            else
            {
                if (customer != null)
                {
                    //Finds customer ID
                    CommonProperty.TicketDetail.CustomerId = customer.SingleOrDefault();
                }
                else
                {
                    CommonProperty.TicketDetail.CustomerId = custFromEmail.Single();
                }
                //Count projects of customer
                int ProjectCount = (from p in CommonProperty.dbContext.Projects
                                    where p.CustomerId == CommonProperty.TicketDetail.CustomerId
                                    select p.Id).Count();
                //For more than 1 customer project- insert to default customer
                if (ProjectCount > 1)
                {
                    GenerateDefaultTicket(CommonProperty.TicketDetail.CustomerId);  //Insert values for default customer
                }
                else
                {
                    //Finds project ID
                    CommonProperty.TicketDetail.ProjectId = (from p in CommonProperty.dbContext.Projects
                                                             where p.CustomerId == CommonProperty.TicketDetail.CustomerId
                                                             select p.Id).SingleOrDefault();

                    //Finds SLA ID
                    var SlaIDCount = from s in CommonProperty.dbContext.CS_SLA
                                     where s.ProjectId == CommonProperty.TicketDetail.ProjectId
                                     && s.IsDefault == true
                                     select s.SLAId;
                    if (SlaIDCount != null)
                    {
                        CommonProperty.TicketDetail.SLAId = SlaIDCount.SingleOrDefault();

                        var priorityLevel = (from p in CommonProperty.dbContext.CS_SLA
                                             where p.SLAId == CommonProperty.TicketDetail.SLAId
                                             select p.PriorityLevel).SingleOrDefault();
                        CommonProperty.TicketDetail.PriorityLevel = Convert.ToByte(priorityLevel);
                    }
                    else
                    {
                        var SlaID = (from s in CommonProperty.dbContext.CS_SLA
                                     where s.ProjectId == CommonProperty.TicketDetail.ProjectId
                                     select s.SLAId).FirstOrDefault();
                        //Assign default priority
                        var Priority = (from p in CommonProperty.dbContext.CS_Priority
                                        select p.PriorityLevel).FirstOrDefault();
                        CommonProperty.TicketDetail.PriorityLevel = Convert.ToByte(Priority);
                    }
                    int level = (from e in CommonProperty.dbContext.ProjectTeam
                                 where e.ProjectId == CommonProperty.TicketDetail.ProjectId
                                 select e.CS_Level).Distinct().Count();
                    //Finds employee Id which is in first level of project team
                    CommonProperty.TicketDetail.EmployeeId = (from e in CommonProperty.dbContext.ProjectTeam
                                                              where e.ProjectId == CommonProperty.TicketDetail.ProjectId
                                                              && e.CS_Level == level
                                                              select e.EmployeeId).FirstOrDefault();
                    //Status becomes open
                    CommonProperty.TicketDetail.StatusId = 1;
                    //Insert ticket in database
                    CommonProperty.Ticket.Insert(CommonProperty.TicketDetail);
                }
            }

            CommonProperty.LogDetail.CreatedOn = DateTime.Now;
            CommonProperty.LogDetail.EmployeeId = CommonProperty.TicketDetail.EmployeeId;
            CommonProperty.LogDetail.Title = CommonProperty.TicketDetail.Title;
            CommonProperty.LogDetail.Comment = "Ticket created";
            CommonProperty.LogDetail.StatusId = CommonProperty.TicketDetail.StatusId;
            CommonProperty.LogDetail.TicketId = CommonProperty.TicketDetail.TicketId;
            CommonProperty.LogDetail.CustumerId = CommonProperty.TicketDetail.CustomerId;
            CommonProperty.LogDetail.PriorityName = (from p in CommonProperty.dbContext.CS_SLA
                                                     where p.SLAId == CommonProperty.TicketDetail.SLAId
                                                     select p.PriorityName).SingleOrDefault();
            //Insert log details
            CommonProperty.Log.Insert(CommonProperty.LogDetail);

            CommonProperty.TicketDetail.TicketNumber = (from e in CommonProperty.dbContext.CS_Ticket
                                                        where e.TicketId == CommonProperty.TicketDetail.TicketId
                                                        select e.TicketNumber).SingleOrDefault();

            string priority = CommonProperty.dbContext.CS_SLA.Where(p => p.SLAId == CommonProperty.TicketDetail.SLAId).Select(a => a.PriorityName).SingleOrDefault();
            string status = CommonProperty.dbContext.CS_Status.Where(s => s.StatusId == CommonProperty.TicketDetail.StatusId).Select(a => a.Status).SingleOrDefault();

            int employeeCount = (from e in CommonProperty.dbContext.ProjectTeam
                                 where e.ProjectId == CommonProperty.TicketDetail.ProjectId
                                 select e.CS_Level).Distinct().Count();

            int incrementTime = (from t in CommonProperty.dbContext.CS_SLA
                                 where t.SLAId == CommonProperty.TicketDetail.SLAId
                                 select t.EscalationTime).SingleOrDefault();

            int[] arr = CommonProperty.dbContext.ProjectTeam.Where(e => e.ProjectId == CommonProperty.TicketDetail.ProjectId && e.CS_Level == employeeCount).Select(a => a.EmployeeId).ToArray();
            var employeeEmail = CommonProperty.dbContext.Employee.Where(p => arr.Contains(p.Id)).Select(a => a.Email_Official).ToList();

            CommonProperty.LinkDetail.TicketId = CommonProperty.TicketDetail.TicketId;
            CommonProperty.LinkDetail.Link = Utility.GenerateLink(CommonProperty.LinkDetail.TicketId);
            CommonProperty.Link.Insert(CommonProperty.LinkDetail);
            string employeeBody = EmailContent.EmployeeBody(CommonProperty.LinkDetail.Link);

            CommonProperty.EscalationList.
                            Add(new Escalation()
                            {
                                EscTicketDetail = CommonProperty.TicketDetail,
                                EscalationExpiryTime = CommonProperty.TicketDetail.CreatedOn.AddMinutes(incrementTime),
                                EscalationLevel = employeeCount,
                                EscalationTime = incrementTime,
                                //CurrentLevel = 1
                            });
            SendMail.SendAcknowledgement(EmailFrom, EmailContent.CustomerSubject, EmailContent.CustomerBody, CommonProperty.TicketDetail.TicketNumber, priority, status);
            SendMail.SendAcknowledgement(employeeEmail, EmailContent.EmployeeSubject, employeeBody, CommonProperty.TicketDetail.TicketNumber, priority, status);
        }

        /// <summary>
        /// Insert values for default customer in case of unidentified customer or more than one project
        /// </summary>
        /// <param name="CustId">Default customer ID - 1, Respective Id for more than 1 project</param>
        private void GenerateDefaultTicket(int CustId)
        {
            int DefaultPrjId = 1;   //Default project Id
            int DefaultSlaId = 1;   //Default sla Id
            CommonProperty.TicketDetail.CustomerId = CustId;    //Customer ID
            CommonProperty.TicketDetail.ProjectId = DefaultPrjId;   //Default project ID
            CommonProperty.TicketDetail.SLAId = DefaultSlaId;   //Default sla ID
            CommonProperty.TicketDetail.StatusId = 1;   //Status - open


            int level = (from e in CommonProperty.dbContext.ProjectTeam
                         where e.ProjectId == DefaultPrjId
                         select e.CS_Level).Distinct().Count();
            var employ = (from e in CommonProperty.dbContext.ProjectTeam
                          where e.ProjectId == 1
                          && e.CS_Level == level
                          select e.EmployeeId).FirstOrDefault();
            CommonProperty.TicketDetail.EmployeeId = employ;    //Default employee ID
            //Assign default priority
            var Priority = (from p in CommonProperty.dbContext.CS_Priority
                            select p.PriorityLevel).First();
            CommonProperty.TicketDetail.PriorityLevel = Convert.ToByte(Priority);

            CommonProperty.Ticket.Insert(CommonProperty.TicketDetail);  //Insert to database.
        }
        #endregion Private Methods
    }
}