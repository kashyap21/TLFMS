//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TLFSupport.Model.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class CS_CustomerEmployeeInfo
    {
        public int CustomerId { get; set; }
        public string EmployeeName { get; set; }
        public string EmailId { get; set; }
        public string ContactNumber { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}