using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLFSupport.Model.CustomeModel
{
    public class ConfigurationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "PBX IP is required")]
        [Display(Name = "PBX IP")]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Enter valid IP address")]
        public string PBX_IP { get; set; }

        [Required(ErrorMessage = "PBX Port is required")]
        [Display(Name = "PBX Port")]
        public int PBX_Port { get; set; }

        [Required(ErrorMessage = "Support email is required")]
        [Display(Name = "Support Email Id")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string SupportEmailAddress { get; set; }

        [Required(ErrorMessage = "Password")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "SMTP Host")]
        public string SMTPHost { get; set; }

        [Display(Name = "SMTP Port")]
        public int SMTPPort { get; set; }

        [Display(Name = "MIME Host")]
        public string MIMEHost { get; set; }

        [Display(Name = "MIME Port")]
        public int MIMEPort { get; set; }

        [Required(ErrorMessage = "SLA breach email address is required")]
        [Display(Name = "SLA Breach Email Id")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string SLABreachEmailAddress { get; set; }
    }
}
