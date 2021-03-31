using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using Microsoft.Office.Interop.Excel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.Rest.Trunking.V1;


namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AUXAgentRecentTransactionController : Controller
    {
        // GET: Admin/AUXAgentRecentTransaction

        CommonServices _commonServices = null;
        AUXAgentRecentTransactionServices _aUXAgentRecentTransactionServices = null;
        public AUXAgentRecentTransactionController()
        {
            _commonServices = new CommonServices();
            _aUXAgentRecentTransactionServices = new AUXAgentRecentTransactionServices();
        }
        public ActionResult Index(string SendingCountry = "", string ReceivingCountry = "", string Date = "", string Status = "",
            int AgentId = 0,
            string Sender = "", string Receiver = "", string Identifier = "",
            int? page = null, int pageSize = 10, int CurrentpageCount = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var sendingcountries = _commonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var receivingCountries = _commonServices.GetCountries();
            ViewBag.ReceivingCountries = new SelectList(receivingCountries, "Code", "Name");
           
            int pageNumber = (page ?? 1);

            var agents = _commonServices.GetAuxAgents();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName" , AgentId);
            int payingStaffId = Common.Common.GetPayingIdByAgentId(AgentId);
            SenderTransactionSearchParamVm searchParamVm = new SenderTransactionSearchParamVm()
            {
                SendingCountry = SendingCountry,
                ReceivingCountry = ReceivingCountry,
                DateRange = Date,
                Status = Status,
                senderId = payingStaffId,
                SenderName = Sender,
                ReceiverName = Receiver,
                searchString = Identifier,
                PageNum = pageNumber,
                PageSize = pageSize
            };
            ViewBag.PageSize = pageSize;
            ViewBag.DateRange = Date;
            ViewBag.CurrentpageCount = CurrentpageCount;
            ViewBag.NumberOfPage = page ?? 1;
            ViewBag.PageNumber = page ?? 1;
            ViewBag.ButtonCount = 0;

            List<AUXAgentRecentTransactionViewModel> result = _aUXAgentRecentTransactionServices.getAuxAgentRecentTransactionList(searchParamVm);
            if (result.Count != 0)
            {
                var TotalCount = result.FirstOrDefault().TotalCount;
                int NumberOfPage = Common.Common.GetNumberOfPage(TotalCount, pageSize);
                ViewBag.NumberOfPage = NumberOfPage;
                var numberofbuttonshown = NumberOfPage - CurrentpageCount;
                ViewBag.ButtonCount = numberofbuttonshown;
            }
            ViewBag.Sender = Sender;
            ViewBag.Receiver = Receiver;
            ViewBag.Identifier = Identifier;
            ViewBag.Status = Status;

            return View(result);

        }


        public ActionResult TransactionDetails(int id, TransactionType transactionService, Agent.Models.Type Type, int AgentId)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            DailyTransactionStatementServices _dailyTransactionStatementServices = new DailyTransactionStatementServices();
            //int agentStaffId = _dailyTransactionStatementServices.GetPayingStaffId(AgentId);
            AgentTransactionHistoryViewModel vm = _dailyTransactionStatementServices.GetTransactionHistories(transactionService, AgentId);
            vm.TransactionHistoryList = vm.TransactionHistoryList.Where(x => x.Id == id).ToList();
            vm.FilterKey = transactionService;
            vm.FilterType = Type;
            string fullName = vm.TransactionHistoryList.Where(x => x.Id == id).Select(x => x.ReceiverName).FirstOrDefault();
            string firstName = "";
            if (fullName != null)
            {
                var names = fullName.Split(' ');
                firstName = names[0];
            }

            ViewBag.ReceiverFirstName = firstName;
            return View(vm);
        }
    }
}