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
    public class SystemSummaryCreditController : Controller
    {
        SystemSummaryCreditServices _services = null;
        public SystemSummaryCreditController()
        {
            _services = new SystemSummaryCreditServices();
        }
        // GET: Agent/SystemSummaryCredit
        public ActionResult Index()
        {
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }

            SystemSummaryCreditViewModel vm = _services.GetSummaryDetails();
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = SystemSummaryCreditViewModel.BindProperty)] SystemSummaryCreditViewModel vm)
        {
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }

            if (vm.PrefundingAmount == 0)
            {
                ModelState.AddModelError("PrefundingAmount", "Enter Amount");
                vm = _services.GetSummaryDetails();
                return View(vm);
            }
            else
            {
                _services.AddPreFund(vm);

            }
            vm = _services.GetSummaryDetails();
            return View(vm);
        }

        public ActionResult PrefundingDetails(int id = 0, string ReceiptNo = "")
        {
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            AgentTransactionHistoryList vm = _services.PrefundingDetails(id, ReceiptNo);
            return View(vm);
        }
        public ActionResult MoneyfexWithdrawalDetails(int id = 0, string ReceiptNo = "")
        {
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            AgentTransactionHistoryList vm = _services.WithDrawalDetails(id, ReceiptNo);
            return View(vm);
        }

    }
}