using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class KiiPayPersonalSignUpSessionViewModel
    {
        public int UserId { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string PhotoUrl { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int BirthdateDay { get; set; }
        public int BirthdateMonth { get; set; }
        public int BirthdateYear { get; set; }
        public Gender Gender { get; set; }
        public string PersonalAddressCountry { get; set; }
        public string PersonalAddressCity { get; set; }
        public string PersonalAddressAddress1 { get; set; }
        public string PersonalAddressAddress2 { get; set; }
        public string PersonalAddressPostalCode { get; set; }

    }
}