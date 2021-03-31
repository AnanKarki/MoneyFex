using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.Common
{
    public class MobileUserInformationViewModel
    {
        public int Id { get; set; }
        public string MobileNo { get; set; }
        public string PassCode { get; set; }
        public string CompanyName { get; set; }
        public int KiiPayBusinessId { get; set; }
        public decimal CurrentBalance { get; set; }
        public string CurrencySymbol { get; set; }
        public string BusinessAddress { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
        public string CountryPhoneCode { get; set; }

    }
}