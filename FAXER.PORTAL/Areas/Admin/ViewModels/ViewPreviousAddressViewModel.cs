using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewPreviousAddressViewModel
    {
        public int Id { get; set; }
        public int staffId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string CountryFlag { get; set; }
        public string PostalCode { get; set; }
        public string LivedFor { get; set; }
        public string PhoneNumber { get;  set; }
    }
}