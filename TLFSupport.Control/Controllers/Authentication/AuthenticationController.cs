using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TLFSupport.Common;
using TLFSupport.Model.DatabaseModel;

namespace TLFSupport.Control.Controllers.Authentication
{
    /// <summary>
    /// Controller for Authentication
    /// </summary>
    public class AuthenticationController : Controller
    {
        #region Private membber

        /// <summary>
        /// string for encoding
        /// </summary>
        private static byte[] keybytes = ASCIIEncoding.ASCII.GetBytes("TLFMS_01");

        #endregion Private membber

        #region Action Methods

        /// <summary>
        /// Action method for Login page
        /// </summary>
        /// <returns>View of Login Page</returns>
        [AllowAnonymous]
        public ActionResult Login()
        {
            //TLFCSEntities db = new TLFCSEntities();
            //var data = (from d in db.ProjectTeam
            //            where d.ProjectId == 213
            //            select d.CS_Level).Distinct().Count();
            return View();
        }

        /// <summary>
        /// Controller to check authentication
        /// </summary>
        /// <param name="name"> Username</param>
        /// <param name="password"> Password</param>
        /// <returns>if username and password are true then redirect to Ticket/Index</returns>
        [HttpPost]
        public ActionResult Login(string name, string password, string rememberme)
        {
            if (rememberme == "on")
            {
                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(30);
                Response.Cookies["Password"].Expires = DateTime.Now.AddDays(30);
            }
            else
            {
                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);
            }
            Response.Cookies["UserName"].Value = name.Trim();
            Response.Cookies["Password"].Value = password.Trim();

            // Encrypt password
            password = Utility.Encrypt(password);

            // Database context
            TLFCSEntities dbcontext = new TLFCSEntities();

            //Fetch username and password
            var user = (from u in dbcontext.Users where u.Name.Equals(name) && u.Password.Equals(password) select u);
            foreach (var item in user)
            {
                // Fetch EmployeeId from employee based on userid
                var emp = (from v in dbcontext.Employee where v.User_Id == item.Id select v);
                foreach (var item1 in emp)
                {
                    Session["user"] = Convert.ToString(item1.Id);
                    Session["userName"] = item.Name;
                    Session["userId"] = Convert.ToString(item1.Id);

                    int[] roleId = (from w in dbcontext.UserRole where w.Users_Id == item1.User_Id select w.Roles_Id).ToArray();
                    Session["userRole"] = roleId;
                    return RedirectToAction(ActionName.TicketIndex, ControllerName.Ticket);
                }
            }
            return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication, "1");
        }

        /// <summary>
        /// Action for Logout
        /// </summary>
        /// <returns>clear session and redirect to login page</returns>
        public ActionResult Logout()
        {
            Session.Clear();
            Response.ClearHeaders();
            Response.AddHeader("Cache-Control", "no-cache, no-store, max-age=0, must-revalidate");
            Response.AddHeader("Pragma", "no-cache");

            if (Request.Cookies["UserName"] != null)
            {
                var c = new HttpCookie("UserName");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }

            if (Request.Cookies["Password"] != null)
            {
                var c = new HttpCookie("Password");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }

            // Logout doesn't show data but going back on page
            //Session.Abandon();
            //Session.Clear();
            //Session.RemoveAll();
            //System.Web.Security.FormsAuthentication.SignOut();
            //Response.Redirect("Login", false);

            return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
        }

        #endregion Action Methods

        #region Utility

        /// <summary>
        /// To Decrypt Encripted string
        /// </summary>
        /// <param name="cryptedString">Encrypted string</param>
        /// <returns>Decrypted string</returns>
        public static string Decrypt(string cryptedString)
        {
            if (String.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException
                   ("The string which needs to be decrypted can not be null.");
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream
                    (Convert.FromBase64String(cryptedString));
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateDecryptor(keybytes, keybytes), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }

        #endregion Utility
    }
}