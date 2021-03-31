using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class SendingMoneyAbroadMobileViewModel
    {
        public const string BindProperty = " Id , CountryCode ,CountryPhoneCode , MobileNumber,RecentlyPaidMobileNumber";
        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string CountryPhoneCode { get; set; }
        public string MobileNumber { get; set; }
        public string RecentlyPaidMobileNumber { get; set; }
    }
}