using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AUXAccountingController : Controller
    {
        // GET: Admin/AUXAccounting

        AccountingServices _services = null;
        CommonServices _commonservices = null;
        public AUXAccountingController()
        {

            _services = new AccountingServices();
            _commonservices = new CommonServices();
        }
        public ActionResult Index(string ReceivingCountry = "", string date = "", int AgentId = 0,
            string Sender = "", string Receiver = "", string Identifier = "",
            int? page = null)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _commonservices.GetCountries();
            ViewBag.ReceivingCountry = new SelectList(Countries, "Code", "Name", ReceivingCountry);
            var agents = _commonservices.GetAuxAgents();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.MonthlySales = 0;
            ViewBag.MonthlyMargin = 0;
            ViewBag.MonthlyFee = 0;
            IPagedList<AccountingViewModel> vm = _services.GetTransactionStatement(ReceivingCountry, date, 0, AgentId, true).ToPagedList(pageNumber, pageSize);
            if (!string.IsNullOrEmpty(Sender))
            {
                Sender = Sender.Trim();
                vm = vm.Where(x => x.Sender.ToLower().Contains(Sender.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(Receiver))
            {
                Receiver = Receiver.Trim();
                vm = vm.Where(x => x.Receiver.ToLower().Contains(Receiver.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(Identifier))
            {
                Identifier = Identifier.Trim();
                vm = vm.Where(x => x.Identifier.ToLower().Contains(Identifier.ToLower())).ToPagedList(pageNumber, pageSize);
            }


            ViewBag.Sender = Sender;
            ViewBag.Receiver = Receiver;
            ViewBag.Identifier = Identifier;

            ViewBag.MonthlySales = _services.get30dayTotalSaleOfAllAuxAgent();
            ViewBag.MonthlyMargin = _services.get30dayTotalMarginOfAllAuxAgent();
            ViewBag.MonthlyFee = _services.get30dayTotalFeeForAllAuxAgent();

            var countrycode = Common.StaffSession.LoggedStaff.Country;
            ViewBag.AgenCountryCurrency = Common.Common.GetCountryCurrency(countrycode);
            ViewBag.DateRange = date;
            return View(vm);

        }
    }
}