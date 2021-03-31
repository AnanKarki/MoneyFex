using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessLoginVM
    {
        public const string BindProperty = " UserName,Password ";
        [Required(ErrorMessage ="please enter the Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "please enter the password")]
        public string Password { get; set; }

    }
}