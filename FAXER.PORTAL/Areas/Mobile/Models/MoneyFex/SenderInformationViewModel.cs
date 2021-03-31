using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex
{
    public class SenderInformationViewModel
    {
        public int SenderId { get; set; }
        public string MobileNo { get; set; }
        public string Email{ get; set; }
        public string SenderName{ get; set; }
        public string Country{ get; set; }
        public string CountryCode { get; set; }
        public string CurrencySymbol { get; set; }
        public string CountryPhoneCode { get; set; }
        public bool IsBusiness { get; set; }
        public decimal MonthlyTransactionLimitAmount { get;  set; }
        public string CurrencyCode { get;  set; }
    }
}