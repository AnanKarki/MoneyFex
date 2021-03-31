using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.KiiPayBusiness
{
    public class KiiPayBusinessMobileSignUpModel
    {
        public const string BindProperty = "MobileNumber ,CountryPhoneCode ,CountryCode ,RegisterCode , PassCode , ConfirmPassCode" +
                ", PerosnalFirstName , PerosnalLastName ,DateOfBirth ,BusinessName , BusinessRegistrationNumber, BusinessType,BusinessCity , BusinessAddress, BusinessAddressOptional" +
               ", BusinessPostZipCode,IsBillIssued , EmailAddress , Gender ";
        public string MobileNumber { get; set; }
        public string CountryPhoneCode { get; set; }
        public string CountryCode { get; set; }
        public string RegisterCode { get; set; }
        public string PassCode { get; set; }
        public string ConfirmPassCode { get; set; }
        public string PerosnalFirstName { get; set; }
        public string PerosnalLastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string BusinessName { get; set; }
        public string BusinessRegistrationNumber { get; set; }
        public BusinessType BusinessType { get; set; }
        public string BusinessCity { get; set; }
        public string BusinessAddress { get; set; }
        public string BusinessAddressOptional { get; set; }
        public string BusinessPostZipCode { get; set; }
        public bool IsBillIssued { get; set; }
        public string EmailAddress { get; set; }
        public Gender Gender { get; set; }
    }
}