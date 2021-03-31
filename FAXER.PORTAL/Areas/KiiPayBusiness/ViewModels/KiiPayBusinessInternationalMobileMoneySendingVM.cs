using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessInternationalMobileMoneySendingVM
    {
        public const string BindProperty = "Id ,Country ,CountryPhoneCode ,RecentNumber , NewNumber ,CurrencySymbol , AvailableBalanceDollar ,AvailableBalanceCent";

        public int Id { get; set; }
        public string Country { get; set; }
        public string CountryPhoneCode { get; set; }
        public string RecentNumber { get; set; }

        [MaxLength(10), MinLength(10)]
        public string NewNumber { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal AvailableBalanceDollar { get; set; }
        public decimal AvailableBalanceCent { get; set; }
    }
}