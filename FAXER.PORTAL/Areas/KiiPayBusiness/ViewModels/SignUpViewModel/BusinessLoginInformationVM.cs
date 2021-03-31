//using FAXER.PORTAL.Attributes;
using FAXER.PORTAL.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels.SignUpViewModel
{
    public class BusinessLoginInformationVM
    {
        public const string BindProperty = " Country,CountryPhoneCode ,MobileNo , EmailAddress, Password, ConfirmPassword";
        [Required (ErrorMessage ="Please select your country!")]
        public string Country { get; set; }

        public string CountryPhoneCode { get; set; }


        [Required(ErrorMessage = "Please select your mobile no!")]
        public string MobileNo { get; set; }
        [Required(ErrorMessage = "Please select your email address!") 
        , EmailAddress(ErrorMessage ="Please enter valid email address!")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage ="Please enter the password"),
         PasswordPolicyAttribute(ErrorMessage = "Your password should be at least 8 characters, and include 1 upper case letter, 1 lower case letter and 1 number")]
        public string Password { get; set; }
        [Required(ErrorMessage ="Please re-enter your password!")]
        [Compare("Password" , ErrorMessage ="the password did not match!")]
        public string ConfirmPassword { get; set; }

    }
}