using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class NonMFTCPaymentsByMyAgent
    {
        public string Year { get; set; }
        public Month Month { get; set; }
        public string TotalCountOfMonthlyNonMFTCPayments { get; set; }
        public string TotalMonthlyNonMFTCPayments { get; set; }
        public string TotalYearlyNonMFTCPayments { get; set; }
    }
    
}