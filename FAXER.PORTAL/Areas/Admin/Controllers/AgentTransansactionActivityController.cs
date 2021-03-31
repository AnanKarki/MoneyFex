using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AgentTransansactionActivityController : Controller
    {
        AgentTransansactionActivityServices _agentTransansactionActivityServices = null;
        CommonServices _CommonService = null;
        public AgentTransansactionActivityController()
        {
            _agentTransansactionActivityServices = new AgentTransansactionActivityServices();
            _CommonService = new CommonServices();
        }
        // GET: Admin/AgentTransansactionActivity
        public ActionResult Index(int AgentId = 0, int transactionServiceType = 0, int year = 0, int month = 0, int Day = 0,
            string Idenifier = "", string StaffName = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            ViewBag.TransferMethod = transactionServiceType;
            ViewBag.Month = month;
            ViewBag.AgentId = AgentId;
            var LoginInfo = _agentTransansactionActivityServices.getLoginInfo(AgentId);
            ViewBag.AccountNuber = LoginInfo.AgentInformation.AccountNo;
            ViewBag.LoginCode = LoginInfo.LoginCode;
            ViewBag.AgentName = LoginInfo.AgentInformation.Name;
            ViewBag.Idenifier = Idenifier;
            ViewBag.StaffName = StaffName;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<AgentTransctionActivityVm> result = _agentTransansactionActivityServices.GetAgentTransactionHistory(AgentId, (Models.TransactionServiceType)transactionServiceType, year, month, Day).ToPagedList(pageNumber, pageSize);
            if (!string.IsNullOrEmpty(Idenifier))
            {
                Idenifier = Idenifier.Trim();
                result = result.Where(x => x.identifier.ToLower().Contains(Idenifier.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(StaffName))
            {
                StaffName = StaffName.Trim();
                result = result.Where(x => x.StaffName.ToLower().Contains(StaffName.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            return View(result);
        }

        public ActionResult AgentRecentTransactionStatememt(string date = "", string SendingCountry = "", string ReceivingCountry = "", string Status = "",
            string ReceiverName = "", string Identifier = "", int? page = null)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var Countries = _CommonService.GetCountries();
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.Status = Status;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.Identifier = Identifier;
            ViewBag.DateRange = date;
            IPagedList<DailyTransactionStatementListVm> vm = _agentTransansactionActivityServices.getRecentTransactionStatementList().ToPagedList(pageNumber, pageSize);
            if (!string.IsNullOrEmpty(date))
            {

                var Date = date.Split('-');
                string[] startDate = Date[0].Split('/');
                string[] endDate = Date[1].Split('/');
                var FromDate = new DateTime(int.Parse(startDate[2]), int.Parse(startDate[0]), int.Parse(startDate[1]));
                var ToDate = new DateTime(int.Parse(endDate[2]), int.Parse(endDate[0]), int.Parse(endDate[1]));// Convert.ToDateTime(Date[1]);
                vm = vm.Where(x => x.DateAndTime >= FromDate && x.DateAndTime <= ToDate).ToPagedList(pageNumber, pageSize);
            }

            if (!string.IsNullOrEmpty(SendingCountry))
            {
                vm = vm.Where(c => c.SendingCountry == SendingCountry).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(ReceivingCountry))
            {
                vm = vm.Where(c => c.ReceivingCountry == ReceivingCountry).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(Status))
            {
                Status = Status.Trim();
                vm = vm.Where(x => x.StatusName.ToLower().Contains(Status.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(ReceiverName))
            {
                ReceiverName = ReceiverName.Trim();
                vm = vm.Where(x => x.ReceiverName.ToLower().Contains(ReceiverName.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(Identifier))
            {
                Identifier = Identifier.Trim();
                vm = vm.Where(x => x.TransactionIdentifier.ToLower().Contains(Identifier.ToLower())).ToPagedList(pageNumber, pageSize);

            }

            return View(vm);
        }


    }
}