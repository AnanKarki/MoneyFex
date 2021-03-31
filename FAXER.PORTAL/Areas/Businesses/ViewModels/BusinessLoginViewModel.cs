using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class BusinessLoginViewModel
    {
        public const string BindProperty = " EmailAddress ,LoginCode , password,ConfirmPassword ";
        public string EmailAddress { get; set; }
      
        public string LoginCode { get; set; }

        
        public string password { get; set; }

        public string ConfirmPassword { get; set; }

    }
    public class ConfirmPasswordResetViewModel
    {
        public const string BindProperty = " NewPassword ,ConfirmPassword ";
        [Required, DisplayName("New Password")]
        public string NewPassword { get; set; }
        [Required, DisplayName("Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
    }

    public class LoggedBusinessMerchant
    {

        public int KiiPayBusinessInformationId { get; set; }

        public string BusinessName { get; set; }

        public string FullName { get; set; }


        public string MiddleName { get; set; }

        public string BusinessEmailAddress { get; set; }
        public string LoginCode { get; set; }

        public string BusinessMobileNo { get; set; }

        public string CountryCode { get; set; }

        public string MFBCCardNo { get; set; }

        public string CardUserName { get; set; }

        public decimal CurrentBalanceOnCard { get; set; }



    }
}