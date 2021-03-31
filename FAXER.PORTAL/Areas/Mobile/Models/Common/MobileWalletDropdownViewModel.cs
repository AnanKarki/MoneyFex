using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.Common
{
    public class MobileWalletDropdownViewModel
    {
        public int Id { get; set; }
        public int KiiPayBusinessInformationId { get; set; }

        public string FullName { get; set; }
        public System.DateTime DOB { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string IdCardType { get; set; }
        public string IdCardNumber { get; set; }
        public DateTime IdExpiryDate { get; set; }
        public string IdIssuingCountry { get; set; }
    }
}