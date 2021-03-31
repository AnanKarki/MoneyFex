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
    public class RefundPaidByAgentController : Controller
    {
        DB.FAXEREntities dbContext = new FAXEREntities();
        // GET: Agent/RefundPaidByAgent
        public ActionResult RefundPaidByAgent(string year ="" , int monthId = 0)
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
            Models.RefundPaidByAgentViewModel vm = new Models.RefundPaidByAgentViewModel();
            vm.TotalCountOfMonthlyRefundPaid = "0";
            vm.TotalMonthlyRefundPaid = "0.00";
            vm.TotalYearlyRefundPaid = "0.00";
            vm.Year = year;
            vm.Month = (Month)monthId;

            var AgentCurrency = Common.Common.GetCurrencySymbol(agentInfo.CountryCode);
            if (!string.IsNullOrEmpty(year))
            {
                int yearParam = int.Parse(year);

                //var noncardFaxedYearwise = dbContext.RefundNonCardFaxMoneyByAgent.Where(x => x.RefundedDate.Year == yearParam && x.Agent_id == agentId);
                var noncardFaxedYearwise = from c in dbContext.FaxingNonCardTransaction.Where(x => x.TransactionDate.Year == yearParam && x.FaxingStatus == FaxingStatus.Refund)
                                           join d in dbContext.RefundNonCardFaxMoneyByAgent on c.Id equals d.NonCardTransaction_id
                                           where d.Agent_id == agentId
                                           select c;

                if (noncardFaxedYearwise.Count() > 0)
                {
                   vm.TotalYearlyRefundPaid = AgentCurrency + " " +  Math.Round(noncardFaxedYearwise.Sum(x => x.FaxingAmount) , 2).ToString();
                   
                }
                if (monthId != 0)
                {
                    //var noncardFaxedMontwise = dbContext.RefundNonCardFaxMoneyByAgent.Where(x => x.RefundedDate.Year == yearParam && x.RefundedDate.Month == monthId && x.Agent_id == agentId);
                    var noncardFaxedMontwise = from c in dbContext.FaxingNonCardTransaction.Where(x => x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId && x.FaxingStatus == FaxingStatus.Refund)
                                               join d in dbContext.RefundNonCardFaxMoneyByAgent on c.Id equals d.NonCardTransaction_id
                                               where d.Agent_id == agentId
                                               select c;
                    if (noncardFaxedMontwise.Count() > 0)
                    {
                        vm.TotalCountOfMonthlyRefundPaid = noncardFaxedMontwise.Count().ToString();
                        vm.TotalMonthlyRefundPaid = AgentCurrency + " " +  Math.Round(noncardFaxedMontwise.Sum(x => x.FaxingAmount) , 2).ToString();
                    }
                }
            }

            return View(vm);
        }
    }
}