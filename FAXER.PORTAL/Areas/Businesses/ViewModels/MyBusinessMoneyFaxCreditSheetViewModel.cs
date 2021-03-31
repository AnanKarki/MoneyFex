using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MyBusinessMoneyFaxCreditSheetViewModel
    {
        public string Year { get; set; }
        public Month Month { get; set; }
        public string TotalMonthlySalesCount { get; set; }
        public string TotalMonthlyCreditedAmount { get; set; }
        public string TotalYearlyCreditedAmount { get; set; }
        public string OutstandingCredit { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }
    }
}