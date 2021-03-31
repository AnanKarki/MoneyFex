using FAXER.PORTAL.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models.SignUp
{
    public class SenderPersonalLoginInformationVM
    {
        public const string BindProperty = "CountryCode,CountryPhoneCode,EmailAddress,MobileNo,Password,ConfirmPassword";

        [Required(ErrorMessage ="Select country ")]
        public string CountryCode { get; set; }
        public string CountryPhoneCode { get; set; }

        [Required(ErrorMessage = "Enter email address ")]
        [EmailAddress(ErrorMessage ="Invalid email address ")]
        public string EmailAddress { get; set; }


        [Required(ErrorMessage = "Enter a mobile number")]
        public string MobileNo { get; set; }



        [Required(ErrorMessage = "Enter a password"),
         PasswordPolicyAttribute(ErrorMessage = "Password should be atleast 8 characters and contain a number")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Re-enter password!")]
        [Compare("Password", ErrorMessage = "Password doesn't match!")]
        public string ConfirmPassword { get; set; }
    }
    public class SenderPersonalDetailVM
    {

        public const string BindProperty = "UserPhotoURL,FirstName,MiddleName,LastName,Day,Month,Year,Gender";

        public string UserPhotoURL { get; set; }

        [Required(ErrorMessage ="Enter your first name")]

        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Enter your last name")]
        public string LastName { get; set; }

        [Range(1 , 32 , ErrorMessage = "Enter a valid day") ]
        [Required(ErrorMessage ="Enter day")]
        public int Day { get; set; }

        [Range(1, 32, ErrorMessage = "Select Month")]
        public Month Month { get; set; }

        [Range( 1000, int.MaxValue, ErrorMessage = "Enter a valid year")]
        [Required(ErrorMessage = "Enter year")]
        public int Year { get; set; }
        public Gender Gender { get; set; }

    }
    public class SenderPersonalAddressVM
    {
        public const string BindProperty = "PostCode,AddressLine1,AddressLine2,City";


        [Required(ErrorMessage ="Enter a postcode")]
        public string PostCode { get; set; }


        [Required(ErrorMessage = "Please enter adddress")]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "Please enter city")]
        public string City { get; set; }



    }

    public class SenderKiiPayWalletEnableVM
    {

        public const string BindProperty = "EnableKiiPayWallet";

        public bool EnableKiiPayWallet { get; set; }

    }
}