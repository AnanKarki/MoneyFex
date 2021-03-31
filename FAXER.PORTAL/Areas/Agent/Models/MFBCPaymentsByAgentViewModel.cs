using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class MFBCPaymentsByAgentViewModel
    {
        public string Year { get; set; }
        public Month Month { get; set; }
        public string TotalCountOfMonthlyMFBCPayments { get; set; }
        public string TotalMonthlyMFBCPayments { get; set; }
        public string TotalYearlyMFBCPayments { get; set; }
    }
}