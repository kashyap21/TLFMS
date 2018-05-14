using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using TLFSupport.CallReader.Common;

namespace TLFSupport.CallReader.BusinessLogic
{
    /// <summary>
    /// Implements PBX call logs reading and related functions
    /// </summary>
    public class Calls
    {      
        #region Public Methods
        /// <summary>
        /// Defines port, file path for logs and write logs for incoming calls
        /// </summary>
        public void IncomingCallReader()
        {
            //Declare TCP listener socket
            TcpListener ServerSocket = new TcpListener(CommonProperty.IpAddr, CommonProperty.Port);

            //Listens TCP connection
            ServerSocket.Start();

            //Accept pending request connection
            CommonProperty.ClientSocket = ServerSocket.AcceptTcpClient();

            while(true)
            {
                try
                {
                    //Stream writer to write in file
                    StreamWriter LogStreamWrite = new StreamWriter(CommonProperty.IncomingCallFilePath, true);

                    //Provides underlying stream of data for network access
                    NetworkStream NetworkStream = CommonProperty.ClientSocket.GetStream();

                    //Buffer to store data from client
                    byte[] BytesFromStream = new byte[9999];

                    //Read data from network stream
                    NetworkStream.Read(BytesFromStream, 0, (int)CommonProperty.ClientSocket.ReceiveBufferSize);

                    //Data in ASCII received from client
                    string DataFromClient = System.Text.Encoding.ASCII.GetString(BytesFromStream).Trim(new[] { '\0' });

                    //Write in file
                    LogStreamWrite.WriteLine(DataFromClient);

                    //Clear buffer
                    LogStreamWrite.Flush();

                    //Close stream writer object
                    LogStreamWrite.Close();

                    InsertToDatabase(DataFromClient);   //Insert log to database
                }
                catch (Exception ex)
                {
                    //Stream writer to write in file
                    StreamWriter ErrorStreamWrite = new StreamWriter(CommonProperty.ErrorLogFilePath, true);

                    //Write in file
                    ErrorStreamWrite.WriteLine(ex.ToString());

                    //Clear buffer
                    ErrorStreamWrite.Flush();

                    //Close stream writer object
                    ErrorStreamWrite.Close();
                }
            }

            //Request underlying TCP connection to close
            CommonProperty.ClientSocket.Close();

            //Close listener
            ServerSocket.Stop();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Insert Incoming Call log to database
        /// </summary>
        /// <param name="DataFromClient">Data in ASCII received from client</param>
        private void InsertToDatabase(string DataFromClient)
        {
            string Pattern = "dd-MM-yy";    //Format for date
            string Date;    //Date as string

            //Stores column value of logs
            string[] CallLogColumn = DataFromClient.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);

            CommonProperty.CallDetail.ExtensionNumber = CallLogColumn[1]; //Extension number
            CommonProperty.CallDetail.CallNumber = CallLogColumn[3];  //Number which customer called
            
            Date = CallLogColumn[4];    //Date as string
            DateTime ParsedDate;    //Instant of date time
            DateTime.TryParseExact(Date, Pattern, null, DateTimeStyles.None, out ParsedDate);   //Parsing date to proper format
            CommonProperty.CallDetail.Date = ParsedDate;   //Date of call

            TimeSpan time = TimeSpan.Parse(CallLogColumn[5]);   //String to time conversion
            CommonProperty.CallDetail.Time = (time);    //Time of call

            CommonProperty.CallDetail.Duration = Convert.ToInt16(CallLogColumn[6]);    //Duration of call

            //Action on call,Unanswered=U,Answer=N(Normal)
            if (Convert.ToChar(CallLogColumn[7]) == 'U')
            {
                CommonProperty.CallDetail.Action = Convert.ToByte(CommonProperty._action.MISSED);
            }
            else
            {
                CommonProperty.CallDetail.Action = Convert.ToByte(CommonProperty._action.ATTENDED);
            }
            //CommonProperty.CallDetail.IsTicketGenerated = false;
            CommonProperty.CallLog.Insert(CommonProperty.CallDetail);  //Insert to database
        }
        #endregion
    }
}
