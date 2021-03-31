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
    public class MoneyFaxedByAgentController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        // GET: Agent/MoneyFaxedByAgent
        public ActionResult Index(string year ="" , int monthId = 0)
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
            Models.MoneyFaxedByAgentViewModel vm = new Models.MoneyFaxedByAgentViewModel();
            vm.TotalCountOfMonthlyMoneyFaxed = "0";
            vm.TotalMonthlyMoneyFaxed = "0.00";
            vm.TotalYearlyMoneyFaxed = "0.00";
            vm.Year = year;
            vm.Month = (Month)monthId;

            var AgentCurrency = Common.Common.GetCurrencySymbol(agentInfo.CountryCode);
            if (!string.IsNullOrEmpty(year))
            {
                int yearParam = int.Parse(year);

                var noncardFaxedYearwise = from c in dbContext.FaxingNonCardTransaction.Where(x => x.TransactionDate.Year == yearParam)
                                           join d in dbContext.AgentFaxMoneyInformation on c.Id equals d.NonCardTransactionId
                                           where d.AgentId == agentId
                                           select c ;




                    if (noncardFaxedYearwise.Count() > 0)
                    {
                    vm.TotalYearlyMoneyFaxed =AgentCurrency + " " +  Math.Round(noncardFaxedYearwise.Sum(x => x.FaxingAmount), 2).ToString();
                    }
                    if (monthId != 0)
                    {
                    
                    var noncardFaxedMontwise = from c in dbContext.FaxingNonCardTransaction.Where(x => x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId)
                                               join d in dbContext.AgentFaxMoneyInformation on c.Id equals d.NonCardTransactionId
                                               where d.AgentId == agentId
                                               select c;
                    if (noncardFaxedMontwise.Count() > 0)
                        {
                            vm.TotalCountOfMonthlyMoneyFaxed = noncardFaxedMontwise.Count().ToString();
                            vm.TotalMonthlyMoneyFaxed = AgentCurrency + " " +  Math.Round(noncardFaxedMontwise.Sum(x => x.FaxingAmount) , 2).ToString();
                        }
                    }
                }

            return View(vm);
            
        }
    }
}