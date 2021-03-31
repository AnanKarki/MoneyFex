using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class MFTCPaymentsByMyAgentViewModel
    {
        public string Year { get; set; }
        public Month Month { get; set; }
        public string TotalCountOfMonthlyMFTCPayments { get; set; }
        public string TotalMonthlyMFTCPayments { get; set; }
        public string TotalYearlyMFTCPayments { get; set; }
    }
}