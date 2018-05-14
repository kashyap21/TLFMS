using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLFSupport.Model.CustomeModel
{
    public class CallLogViewModel
    {
        public int CallLogId { get; set; }
        public string ExtensionNumber { get; set; }
        public string Trunk { get; set; }
        public string CallNumber { get; set; }
        public System.DateTime Date { get; set; }
        public System.TimeSpan Time { get; set; }
        public int Duration { get; set; }
        public string Action { get; set; }
        public bool IsTicketGenerated { get; set; }
        public Nullable<int> TicketId { get; set; }
        public string TicketNumber { get; set; }
    }
}