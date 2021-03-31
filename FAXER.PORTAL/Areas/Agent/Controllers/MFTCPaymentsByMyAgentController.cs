using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class MFTCPaymentsByMyAgentController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        // GET: Agent/MFTCPaymentsByMyAgent
        public ActionResult Index(string year = "", int monthId = 0)
        {
            //Session.Remove("FirstLogin");
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            int agentId = agentInfo.Id;
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            if (Common.AgentSession.AgentStaffLogin.AgentStaff.AgentStaffType == StaffType.Transaction)
            {
                return RedirectToAction("Index", "AgentDashboard");
            }
            MFTCPaymentsByMyAgentViewModel vm = new MFTCPaymentsByMyAgentViewModel();
            vm.TotalCountOfMonthlyMFTCPayments = "0";
            vm.TotalMonthlyMFTCPayments = "0.00";
            vm.TotalYearlyMFTCPayments = "0.00";
            vm.Year = year;
            vm.Month = (Month)monthId;

            var AgentCurrency = Common.Common.GetCurrencySymbol(agentInfo.CountryCode);
            if (!string.IsNullOrEmpty(year))
            {
                int yearParam = int.Parse(year);
                var cardWithDrawlYearwise = dbContext.UserCardWithdrawl.Where(x => x.TransactionDate.Year == yearParam && x.AgentInformationId == agentId);
                if (cardWithDrawlYearwise.Count() > 0)
                {
                    vm.TotalYearlyMFTCPayments =AgentCurrency + " " +  Math.Round(cardWithDrawlYearwise.Sum(x => x.TransactionAmount) , 2).ToString();
                }
                if (monthId != 0)
                {
                    var cardWithDrawlMontwise = dbContext.UserCardWithdrawl.Where(x => x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId && x.AgentInformationId ==agentId);
                    if (cardWithDrawlMontwise.Count() > 0)
                    {
                        vm.TotalCountOfMonthlyMFTCPayments = cardWithDrawlMontwise.Count().ToString();
                        vm.TotalMonthlyMFTCPayments = AgentCurrency + " " + Math.Round(cardWithDrawlMontwise.Sum(x => x.TransactionAmount) , 2).ToString();
                    }
                }
            }
            return View(vm);
        }
    }
}