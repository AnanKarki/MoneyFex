using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class ForgotPasswordViewModel
    {
        public const string BindProperty = " Id ,EmailAddress";
        public int Id { get; set; }
        [Required]
        public string EmailAddress { get; set; }
    }
}