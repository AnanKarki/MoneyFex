using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class NonNFTCPaymentsByMyAgentController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        // GET: Agent/NonNFTCPaymentsByMyAgent
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
            NonMFTCPaymentsByMyAgent vm = new NonMFTCPaymentsByMyAgent();
            vm.TotalCountOfMonthlyNonMFTCPayments = "0";
            vm.TotalMonthlyNonMFTCPayments = "0.00";
            vm.TotalYearlyNonMFTCPayments = "0.00";
            vm.Year = year;
            vm.Month = (Month)monthId;
            var AgentCurrency = Common.Common.GetCurrencySymbol(agentInfo.CountryCode);

            if (!string.IsNullOrEmpty(year))
            {
                int yearParam = int.Parse(year);
                var noncardReceiveYearwise = dbContext.ReceiverNonCardWithdrawl.Where(x => x.TransactionDate.Year == yearParam && x.AgentId == agentId);
                if (noncardReceiveYearwise.Count() > 0)
                {
                    vm.TotalYearlyNonMFTCPayments = AgentCurrency + " " + Math.Round(noncardReceiveYearwise.Sum(x => x.TransactionAmount) , 2).ToString();
                }
                if (monthId != 0)
                {
                    var noncardReceiveMontwise = dbContext.ReceiverNonCardWithdrawl.Where(x => x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId && x.AgentId == agentId);
                    if (noncardReceiveMontwise.Count() > 0)
                    {
                        vm.TotalCountOfMonthlyNonMFTCPayments = noncardReceiveMontwise.Count().ToString();
                        vm.TotalMonthlyNonMFTCPayments = AgentCurrency + " " + Math.Round(noncardReceiveMontwise.Sum(x => x.TransactionAmount) , 2).ToString();
                    }
                }
            }
            return View(vm);
        }
    }
}