using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLFSupport.Model.CustomeModel
{
    /// <summary>
    /// Properties  of Customer
    /// </summary>
    public class CustomerViewModel
    {
        public int Id { get; set; }
         [Required(ErrorMessage = "Please enter valid customer name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter valid customer short name")]
        public string ShortName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required(ErrorMessage = "Please enter valid city")]
        public string City { get; set; }
        [Required(ErrorMessage = "Please enter valid state")]
        public string State { get; set; }
        [Required(ErrorMessage = "Please select appropriate country")]
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string PinCode { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        [Required(ErrorMessage = "Please select appropriate owner")]
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        [Required(ErrorMessage = "Please enter valid contact person name")]
        public string ContactPersonName { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Please enter valid customer dedicated number")]
        public string CS_DedicatedNumber { get; set; }
        [Required(ErrorMessage = "Please enter valid customer domain")]
        public string CS_DomainName { get; set; }

    }
}
