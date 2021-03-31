using FAXER.PORTAL.Areas.Staff.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class StaffContactDetailsViewModel
    {
        public const string BindProperty = "Id , Address1 ,Address2 ,City , State ,PostalCode , Country, PhoneNumber ,CountryCode , BeenLivingSince ";

        public int Id { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }
        public BeenLivingSince BeenLivingSince { get; set; }
    }
}