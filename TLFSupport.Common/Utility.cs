using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using TLFSupport.Model.DatabaseModel;

namespace TLFSupport.Common
{
    public class Utility
    {
        private static byte[] keybytes = ASCIIEncoding.ASCII.GetBytes("TLFMS_01");

        #region ENUM

        /// <summary>
        /// Enum for call action
        /// </summary>
        public enum _action { Status = 1, PM, PL, Customer };

        #endregion ENUM

        /// <summary>
        /// Converts string data of D:00 H:00 M:00 of form to int minute
        /// </summary>
        /// <param name="value">String data</param>
        /// <returns>Minute</returns>
        public static int StringToMinute(string value)
        {
            value = Regex.Replace(value, "[^0-9_]", "");
            value = Regex.Replace(value, "_", "0");
            int valueRawInt = Convert.ToInt32(value);
            int min = valueRawInt % 100;
            valueRawInt = valueRawInt / 100;
            int hour = valueRawInt % 100;
            valueRawInt = valueRawInt / 100;
            int day = valueRawInt % 100;
            return (day * 1440 + hour * 60 + min);
        }

        /// <summary>
        /// total status counter for Progress bar in ticket view
        /// </summary>
        public static int totoalstatus = 6;

        /// <summary>
        /// Converts int minute to string data of D:00 H:00 M:00
        /// </summary>
        /// <param name="min">Minute</param>
        /// <returns>string data of form D:00 H:00 M:00</returns>
        public static string MinuteToString(int min)
        {
            int day = min / 1440;
            min = min % 1440;
            int hour = min / 60;
            min = min % 60;
            string hourString = hour.ToString();
            string dayString = day.ToString();
            string minString = min.ToString();
            if (hourString.Length == 1)
            {
                hourString = "0" + hourString;
            }
            if (dayString.Length == 1)
            {
                dayString = "0" + dayString;
            }
            if (minString.Length == 1)
            {
                minString = "0" + minString;
            }

            string value = "D:" + dayString + " H:" + hourString + " M:" + minString;
            return value;
        }

        public static string GenerateLink(int TicketId)
        {
            TLFCSEntities _dbContext = new TLFCSEntities();
            string Link = RandomNumber(9, 900000);

            int EmployeeId = _dbContext.CS_Ticket.Where(c => c.TicketId == TicketId).Select(c => c.EmployeeId).SingleOrDefault();
            string strEmployee = EmployeeId.ToString();
            if (strEmployee.Length < 2)
            {
                strEmployee = "000" + strEmployee;
            }
            else if (strEmployee.Length < 3)
            {
                strEmployee = "00" + strEmployee;
            }
            else if (strEmployee.Length < 4)
            {
                strEmployee = "0" + strEmployee;
            }

            Link = Link + strEmployee;
            Link = Encrypt(Link);
            Link = Link.Replace((char)47, 'D');
            return Link;
        }

        public static string Encrypt(string originalString)
        {
            if (String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException
                       ("The string which needs to be encrypted can not be null.");
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                        cryptoProvider.CreateEncryptor(keybytes, keybytes),
                                                        CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        public static string RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max).ToString();
        }
    }
}