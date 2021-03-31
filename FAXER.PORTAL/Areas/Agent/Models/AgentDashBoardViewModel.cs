using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentDashBoardViewModel
    {
        public string AgentCurrencySymbol { get; set; }
        public string AgentCurrency{ get; set; }
        public decimal AUXAgentAccountBalance{ get; set; }

        public decimal Commission { get; set; }
        public List<AlertsViewModel> AlersViewModel { get; set; }
        public List<ExchangeRateSettingViewModel> ExchangeRate { get; set; }
    }
}