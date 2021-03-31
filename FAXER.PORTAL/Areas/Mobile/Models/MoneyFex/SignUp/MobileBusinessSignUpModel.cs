using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex.SignUp
{
    
    public class MobileBusinessSignUpModel
    {
        public const string BindProperty = " CountryName, CountryPhoneCode , EmailAddress ,MobileNumber ,Password ,ConfirmPassword ,LegalFirstName , LegalLastName," +
            "DateOfBirth,Gender , YourRole,PersonalAddressLineOne ,PersonalAddressLineTwo ,PersonalCity , PersonalPostcodeOrZipCode, BusinessRegistrationNumber," +
            "BusinessLeagalName ,BusinessCity ,BusinessTypeName ,BusinessType,OperatingCity,OperatingAddressLineOne, OperatingAddressLineTwo, OperatingPostcodeOrZipCode ,CountryCode," +
            "CurrencyCode ,CurrencySymbol , CurrentBalance, SenderId";
        public string CountryName { get; set; }
        public string CountryPhoneCode { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string LegalFirstName { get; set; }
        public string LegalLastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string YourRole { get; set; }

        public string PersonalAddressLineOne { get; set; }
        public string PersonalAddressLineTwo { get; set; }
        public string PersonalCity { get; set; }
        public string PersonalPostcodeOrZipCode { get; set; }

        public string BusinessRegistrationNumber { get; set; }
        public string BusinessLeagalName { get; set; }
        public string BusinessCity { get; set; }
        public string BusinessTypeName { get; set; }
        public SenderBusinessType BusinessType{ get; set; }

        public string OperatingCity { get; set; }
        public string OperatingAddressLineOne { get; set; }
        public string OperatingAddressLineTwo { get; set; }
        public string OperatingPostcodeOrZipCode { get; set; }

        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal CurrentBalance { get; set; }
        public int SenderId { get; set; }
      
    }
}