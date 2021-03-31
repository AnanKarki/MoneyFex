using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class OnlineTransferOperationSummaryController : Controller
    {

        CommonServices _commonServices = new CommonServices();
        // GET: Admin/OnlineTransferOperationSummary
        public ActionResult Index(string SendingCountry="",string ReceivingCountry="", int transferMethod=0,string Date="")
        {
            List<TransferOperationSummaryViewModel> vm = new List<TransferOperationSummaryViewModel>();

            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            return View(vm);
        }

        public ActionResult AgentTransferOperationSummary(string SendingCountry = "", string ReceivingCountry = "", int transferMethod = 0, string Date = "",int AgentId=0)
        {
            List<TransferOperationSummaryViewModel> vm = new List<TransferOperationSummaryViewModel>();
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            var Agent = _commonServices.GetAgent();
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");
            return View(vm);
        }
        public ActionResult PartnerTransferOperationSummary(string SendingCountry = "", string ReceivingCountry = "", int transferMethod = 0, string Date = "", int PartnerId = 0)
        {
            List<TransferOperationSummaryViewModel> vm = new List<TransferOperationSummaryViewModel>();
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            //assume agent as Partner
            var Agent = _commonServices.GetAgent();
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");

            return View(vm);
        }

    }
}