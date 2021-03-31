using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class TotalPrefundingAmontViewModel
    {
        public int Id{ get; set; }
        public string SendingCountry{ get; set; }
        public string SendingCountryFlag{ get; set; }
        public int Year{ get; set; }

        public string Month { get; set; }
        public string AgentName { get; set; }
        public string AccountNo { get; set; }
        public string Currency { get; set; }
        public decimal CurrentFunds { get; set; }
    }
}