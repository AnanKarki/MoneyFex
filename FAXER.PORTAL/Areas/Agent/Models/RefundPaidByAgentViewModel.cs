using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class RefundPaidByAgentViewModel
    {
        public string Year { get; set; }
        public Month Month { get; set; }
        public string TotalCountOfMonthlyRefundPaid { get; set; }
        public string TotalMonthlyRefundPaid { get; set; }
        public string TotalYearlyRefundPaid { get; set; }
    }
}