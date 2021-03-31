using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class CreditDebitCardUsageViewModel
    {
        public int Id{ get; set; }
        public int SenderId{ get; set; }
        public string SenderName { get; set; }
        public string CardNumber { get; set; }
        public string CardIssuingCountry { get; set; }
        public string CardIssuingCountryFlag { get; set; }
        public string OpStatus { get; set; }
        public string Amount { get; set; }
        public string AuthCount { get; set; }
        public string DateTime { get; set; }
    }
}