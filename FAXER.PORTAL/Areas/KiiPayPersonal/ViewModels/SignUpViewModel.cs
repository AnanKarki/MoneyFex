using FAXER.PORTAL.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class SignUpViewModel
    {
        public const string BindProperty = "Id ,CountryCode ,CountryPhoneCode , MobileNumber , EmailAddress ,Password , ConfirmPassword";
        public int Id { get; set; }
        [Required]
        public string CountryCode { get; set; }
        public string CountryPhoneCode { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        [PasswordPolicy(ErrorMessage ="Password must contain at least one uppercase, one lowercase and one special character !")]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}