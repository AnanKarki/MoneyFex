using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class TranscationDetailsController : Controller
    {
        Agent.AgentServices.DailyTransactionStatementServices _dailyTransactionStatementServices = null;

        public TranscationDetailsController()
        {
            _dailyTransactionStatementServices = new Agent.AgentServices.DailyTransactionStatementServices();
        }

        // GET: Admin/TranscationDetails
        public ActionResult Index(int id = 0, TransactionType transactionService = 0, int AgentId = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int agentStaffId = _dailyTransactionStatementServices.GetPayingStaffId(AgentId);
            AgentTransactionHistoryViewModel vm = _dailyTransactionStatementServices.GetTransactionHistories(transactionService, agentStaffId);
            vm.TransactionHistoryList = vm.TransactionHistoryList.Where(x => x.Id == id).ToList();
            vm.FilterKey = transactionService;
            ViewBag.AgentId = AgentId;
            FAXER.PORTAL.Areas.Admin.Services.CommonServices _commonServices = new Services.CommonServices();
            ViewBag.AgentName = _commonServices.GetAgentInformation(AgentId).Name;
            ViewBag.AccountNo = _commonServices.GetAgentAccNo(AgentId);
            ViewBag.AgentCountry = Common.Common.GetCountryName(_commonServices.GetAgentInformation(AgentId).CountryCode);
            return View(vm);
        }
        public ActionResult CashPickUpDetails(int id = 0, TransactionType transactionService = 0, int AgentId = 0)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int agentStaffId = _dailyTransactionStatementServices.GetPayingStaffId(AgentId);
            AgentTransactionHistoryViewModel vm = _dailyTransactionStatementServices.GetTransactionHistories(transactionService, agentStaffId);
            vm.TransactionHistoryList = vm.TransactionHistoryList.Where(x => x.Id == id).ToList();
            vm.FilterKey = transactionService;
            ViewBag.AgentId = AgentId;
            return View(vm);
        }
        public ActionResult MobileWalletDetails(int id = 0, TransactionType transactionService = 0, int AgentId = 0)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int agentStaffId = _dailyTransactionStatementServices.GetPayingStaffId(AgentId);
            AgentTransactionHistoryViewModel vm = _dailyTransactionStatementServices.GetTransactionHistories(transactionService, agentStaffId);
            vm.TransactionHistoryList = vm.TransactionHistoryList.Where(x => x.Id == id).ToList();
            vm.FilterKey = transactionService;
            ViewBag.AgentId = AgentId;
            return View(vm);
        }
        public ActionResult KiiPayDetails(int id = 0, TransactionType transactionService = 0, int AgentId = 0)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int agentStaffId = _dailyTransactionStatementServices.GetPayingStaffId(AgentId);
            AgentTransactionHistoryViewModel vm = _dailyTransactionStatementServices.GetTransactionHistories(transactionService, agentStaffId);
            vm.TransactionHistoryList = vm.TransactionHistoryList.Where(x => x.Id == id).ToList();
            vm.FilterKey = transactionService;
            ViewBag.AgentId = AgentId;
            return View(vm);
        }


        public ActionResult BankDepositDetails(int id = 0, TransactionType transactionService = 0, int AgentId = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int agentStaffId = _dailyTransactionStatementServices.GetPayingStaffId(AgentId);
            AgentTransactionHistoryViewModel vm = _dailyTransactionStatementServices.GetTransactionHistories(transactionService, agentStaffId);
            vm.TransactionHistoryList = vm.TransactionHistoryList.Where(x => x.Id == id).ToList();
            vm.FilterKey = transactionService;
            ViewBag.AgentId = AgentId;
            return View(vm);
        }

    }
}