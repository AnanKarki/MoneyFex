using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class MoneyFaxedByAgentViewModel
    {
        public string Year { get; set; }
        public Month Month { get; set; }
        public string TotalCountOfMonthlyMoneyFaxed { get; set; }
        public string TotalMonthlyMoneyFaxed { get; set; }
        public string TotalYearlyMoneyFaxed { get; set; }

  
    }
}