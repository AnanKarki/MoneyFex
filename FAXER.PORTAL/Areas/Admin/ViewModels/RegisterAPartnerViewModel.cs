using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class RegisterAPartnerViewModel
    {
        public const string BindProperty = "Id , Name , LicenseNo ,ContactPersonName , PartnerType,Address1 , Address2, State,PostalCode ,City ,Country ,CountryPhoneCode ,PhoneNumber" +
            " ,EmailAddress ,Website ,PartnerLogoUrl";

        public int Id { get; set; }
        public string Name { get; set; }
        public string LicenseNo { get; set; }
        public string ContactPersonName { get; set; }
        public string PartnerType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountryPhoneCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Website { get; set; }
        public string PartnerLogoUrl { get; set; }
    }
}