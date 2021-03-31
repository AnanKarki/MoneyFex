using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class FaxerMoneyFaxMerchantPaymentsViewModel
    {
        public string MerchantName { get; set; }
        public string MerchantAccountNumber { get; set; }
        public string MerchantCountry { get; set; }
        public string MerchantCity { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentRefrence { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string CurrencySymbol { get; set; }
    }
}