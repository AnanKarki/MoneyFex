using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class ResetPasswordViewModel
    {
        public const string BindProperty = " Id ,SecurityCode";

        public int Id { get; set; }
        [Required]
        public string SecurityCode { get; set; }
    }

    public class EnterNewPasswordViewModel
    {
        public const string BindProperty = " Id ,Password ,ConfirmPassword ";
        public int Id { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}