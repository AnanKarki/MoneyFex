using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ManualBankDepositController : Controller
    {
        SSenderBankAccountDeposit _bankdeposit = null;
        DailyTransactionStatementServices _dailyTransactionStatementServices = null;
        CommonServices _CommonServices = null;

        public ManualBankDepositController()
        {
            _bankdeposit = new SSenderBankAccountDeposit();
            _dailyTransactionStatementServices = new DailyTransactionStatementServices();
            _CommonServices = new CommonServices();
        }

        // GET: Admin/ManualBankDeposit
        public ActionResult Index(int AgentId = 0, int Year = 0, int Month = 0, int Day = 0, string ReceiverName = "", string Status = ""
            , string SendingCountry = "", int? page = null)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var agentInformation = _bankdeposit.AgentInformation(AgentId);
            var agentLoginInformation = _bankdeposit.AgentLoginInformation(AgentId);
            ViewBag.AgentName = agentInformation.Name;
            ViewBag.AgentAccountNo = agentInformation.AccountNo;
            ViewBag.AgentLoginCode = agentLoginInformation.LoginCode;
            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            ViewBag.Month = Month;
            ViewBag.Day = Day;
            ViewBag.AgentId = AgentId;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.Status = Status;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            IPagedList<ManualBankDepositViewModel> vm = _bankdeposit.GetAgentManualBankDeposit(AgentId, Year, Month, Day).Data.ToPagedList(pageNumber, pageSize);

            if (!string.IsNullOrEmpty(ReceiverName))
            {
                ReceiverName = ReceiverName.Trim();
                vm = vm.Where(x => x.ReceiverName.ToLower().Contains(ReceiverName.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(Status))
            {
                Status = Status.Trim();
                vm = vm.Where(x => x.StatusName.ToLower().Contains(Status.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(SendingCountry))
            {
                vm = vm.Where(x => x.SendingCountry == SendingCountry).ToPagedList(pageNumber, pageSize);
            }
            return View(vm);
        }

        public ActionResult ManualBankDepositDetails(int id = 0, TransactionType transactionService = 0, int AgentId = 0)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int agentStaffId = _dailyTransactionStatementServices.GetPayingStaffId(AgentId);
            AgentTransactionHistoryViewModel vm = _dailyTransactionStatementServices.GetTransactionHistories(transactionService, agentStaffId);
            vm.TransactionHistoryList = _dailyTransactionStatementServices.GetManualBankAccountDepositTrasactionDetails(id).ToList();
            //vm.TransactionHistoryList = vm.TransactionHistoryList.Where(x => x.Id == id).ToList();
            vm.FilterKey = transactionService;
            ViewBag.AgentId = AgentId;
            return View(vm);
        }


        public ActionResult ViewAgents()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var viewmodel = new List<ViewModels.ViewRegisteredAgentsViewModel>();
            string CountryCode = "";
            string City = "";
            Services.ViewRegisteredAgentsServies AgentInformation = new Services.ViewRegisteredAgentsServies();
            viewmodel = AgentInformation.getFilterAgentList(CountryCode, City);
            //viewmodel = AgentInformation.getAgentInformationList();
            ViewBag.Country = "";

            return View(viewmodel);

        }

        public ActionResult HoldUnhold(int id, int AgentId = 0)
        {
            if (id != 0)
            {
                var data = _bankdeposit.List().Data.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {

                    if (data.Status == BankDepositStatus.Held)
                    {
                        data.Status = BankDepositStatus.UnHold;
                    }
                    else if (data.Status == BankDepositStatus.UnHold)
                    {
                        data.Status = BankDepositStatus.Held;
                    }
                    else if (data.Status == BankDepositStatus.Incomplete)
                    {
                        data.Status = BankDepositStatus.Held;
                    }
                    else
                    {
                        data.Status = data.Status;
                    }

                }

                _bankdeposit.Update(data);

            }
            return RedirectToAction("Index", "ManualBankDeposit", new { @AgentId = AgentId });
        }

        public ActionResult Cancel(int id, int AgentId)
        {
            if (id != 0)
            {
                var data = _bankdeposit.List().Data.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {
                    data.Status = BankDepositStatus.Cancel;
                }

                _bankdeposit.Update(data);

            }
            return RedirectToAction("Index", "ManualBankDeposit", new { @AgentId = AgentId });
        }


    }
}