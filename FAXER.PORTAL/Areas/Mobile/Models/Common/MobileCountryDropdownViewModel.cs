using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.Common
{
    public class MobileCountryDropdownViewModel
    {
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string CountryCurrency { get; set; }
        public string FlagCode { get; set; }
        public string CountryPhoneCode { get; set; }
        public string CurrencySymbol { get; set; }
        public string CountryWithCurrency { get; set; }
    }
}