using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class CardUserLoginViewModel
    {
        public const string BindProperty = "EmailAddress ,Password "; 
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class PasswordResetViewModel
    {
        public const string BindProperty = "NewPassword ,ConfirmPassword ";
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage ="Password didn't match please try again")]
        public string ConfirmPassword { get; set; }

    }
}