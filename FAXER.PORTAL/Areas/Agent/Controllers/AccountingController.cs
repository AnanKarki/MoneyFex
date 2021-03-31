using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AccountingController : Controller
    {
        AgentInformation agentInfo = null;
        AccountingServices _services = null;
        CommonServices _commonservices = null;
        public AccountingController()
        {
            agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            _services = new AccountingServices();
            _commonservices = new CommonServices();
        }
        // GET: Agent/Accounting
        public ActionResult Index(string ReceivingCountry = "", string date = "")
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            var Countries = _commonservices.GetCountries();
            ViewBag.ReceivingCountry = new SelectList(Countries, "Code", "Name", ReceivingCountry);
            List<AccountingViewModel> vm = new List<AccountingViewModel>();
            var PayingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            vm = _services.GetTransactionStatement(ReceivingCountry, date, PayingAgentStaffId);
            ViewBag.NumberOfDays = _services.GetNumberofdaysAgentWasCreated(PayingAgentStaffId);
            ViewBag.MonthlySales = _services.get30dayTotalSale(PayingAgentStaffId);
            ViewBag.MonthlyMargin = _services.get30dayTotalMargin(PayingAgentStaffId);
            ViewBag.MonthlyFee = _services.get30dayTotalFee(PayingAgentStaffId);
            ViewBag.AgenCountryCurrency = Common.Common.GetCountryCurrency(agentInfo.CountryCode);
            ViewBag.DateRange = date;
            
            return View(vm);
        }
    }
}