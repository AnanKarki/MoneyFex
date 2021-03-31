using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AgentStaffInfoAndLoginViewModel
    {
        public const string BindProperty = "Country , AgentId,AgenAccountNo ,Address ";
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Select Agent")]
        public int AgentId { get; set; }
        [Required(ErrorMessage = "Enter Account Number")]
        public string AgenAccountNo { get; set; }
        [Required(ErrorMessage = "Enter Address")]
        public string Address { get; set; }
    }
}