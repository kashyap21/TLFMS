namespace TLFSupport.Common
{
    public class EmailContent
    {
        public static string EmployeeBody(string link)
        {
            return "New Ticket assigned to you  </br> Please click  <span style='font-family: Times New Roman;font-size:15;font-weight:bold;'><a href='" + "http://localhost:60622/" + "Ticket/" + "CheckMail/" + link + "'>here</a></span> to view more details</br>";
        }

        public static string EmployeeSubject = "Ticket Number:-##";
        public static string SLAbreechSubject = "Ticket Number:-##";
        public static string CustomerSubject = "[RequestID:##";

        public static string SLAbreechBody = "No Employee replied to above ticket </br>Somebody should take action immediately.";
        public static string CustomerBody = "Hello, </br> Your issue is under action.Priority & Status detail as below </br> Note : For further communication regarding same issue keep subject of mail same as this email</br>";
    }
}