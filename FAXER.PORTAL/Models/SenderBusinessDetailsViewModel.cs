using FAXER.PORTAL.Attributes;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderBusinessDetailsViewModel
    {
        public const string BindProperty = "BusinessName,RegistrationNumber,BusinessType";

        [Required(ErrorMessage = "Enter Business Name")]
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "Enter Registration Number")]
        public string RegistrationNumber { get; set; }

        public SenderBusinessType BusinessType { get; set; }

    }
    public class SenderBusinessRegisteredViewModel
    {
        public const string BindProperty = "Country,City,AddressLine1,AddressLine2,Postal";

        [Required(ErrorMessage = "Enter Country")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Enter City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Enter Address Line 1")]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        [Required(ErrorMessage = "Enter Postal/Zip Code")]
        public string Postal { get; set; }
    }

    public class SenderBusinessOperatingViewModel
    {
        public const string BindProperty = "OperatingPostal,OperatingAddressLine1,OperatingAddressLine2,OperatingCity,IsSamePersonalAddress";

        [Required(ErrorMessage = "Enter Postal/Zip Code")]
        public string OperatingPostal { get; set; }
        [Required(ErrorMessage = "Enter Address Line 1")]
        public string OperatingAddressLine1 { get; set; }
        public string OperatingAddressLine2 { get; set; }
        [Required(ErrorMessage = "Enter City")]
        public string OperatingCity { get; set; }
        public bool IsSamePersonalAddress { get; set; }
    }
    public class SenderBusinessLoginInfoViewModel
    {
        public const string BindProperty = "EmailAddress,MobileNo,Country,MobileCode,Password,ConfirmPassword";

        [Required(ErrorMessage = "Enter email address ")]
        [EmailAddress(ErrorMessage = "Invalid email address ")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Enter a mobile number")]
        public string MobileNo { get; set; }
        public string Country { get; set; }
        public string MobileCode { get; set; }
        [Required(ErrorMessage = "Enter a password"),
         PasswordPolicyAttribute(ErrorMessage = "Password should be atleast 8 characters and contain a number")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Re-enter password!")]
        [Compare("Password", ErrorMessage = "Password doesn't match!")]
        public string ConfirmPassword { get; set; }
    }
}