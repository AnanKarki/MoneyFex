using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex.SignUp
{
    public class MobilePersonalSignUpModel
    {
        public const string BindProperty = "CountryName ,CountryPhoneCode ,EmailAddress ,Password ,ConfirmPassword ,CountryCode ,LegalFirstName , LegalLastName," +
            "MobileNumber ,DateOfBirth ,City , AddressLineOne,AddressLineTwo , PostcodeOrZipCode,CurrencyCode ,CurrencySymbol ,CurrentBalance , SenderId  ";
        public string CountryName { get; set; }
        public string CountryPhoneCode { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string CountryCode { get; set; }


        public string LegalFirstName { get; set; }
        public string LegalLastName { get; set; }
        public string MobileNumber { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string City { get; set; }
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string PostcodeOrZipCode { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal CurrentBalance { get; set; }
        public int SenderId { get; set; }
        public int Gender { get; set; }
    }
}