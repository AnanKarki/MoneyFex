using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.CardUsers.Services;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class ExchangeRateController : Controller
    {
        AgentInformation agentInfo = null;
        ExchangeRateSercives _services = null;
        CommonServices _commonServices = null;
        public ExchangeRateController()
        {
            agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            _services = new ExchangeRateSercives();
            _commonServices = new CommonServices();
        }
        // GET: Agent/ExchangeRate
        public ActionResult Index(string sendingCountry = "", string receivingCountry = "", int transferType = 0, int transferMethod = 0)
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }

            var Countries = _commonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name", sendingCountry);
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name", receivingCountry);
            List<ExchangeRateSettingViewModel> vm = new List<ExchangeRateSettingViewModel>();
            vm = _services.GetExchangeRateList(sendingCountry, receivingCountry, transferType, transferMethod);
            return View(vm);
        }

        public ActionResult SetExchanegRate(int Id = 0)
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            SetCountryViewBag();
            SetCurrenciesViewBag();
            ExchangeRateSettingViewModel vm = new ExchangeRateSettingViewModel();
            if (Id > 0)
            {
                vm = _services.GetExchangeRateByCurrency(Id);

            }
            vm.SourceCountryCode = agentInfo.CountryCode;
            vm.SourceCurrencyCode = Common.Common.GetCountryCurrency(agentInfo.CountryCode);
            vm.TransferType = TransactionTransferType.AuxAgent;
            return View(vm);
        }
        [HttpPost]
        public ActionResult SetExchanegRate([Bind(Include = ExchangeRateSettingViewModel.BindProperty)]ExchangeRateSettingViewModel vm)
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }

            SetCountryViewBag();
            SetCurrenciesViewBag();
            vm.AgentId = agentInfo.Id;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.DestinationCurrencyCode))
                {
                    ModelState.AddModelError("DestinationCurrencyCode", "Select Currency");
                    return View(vm);
                }
                if (string.IsNullOrEmpty(vm.Range) == true)
                {
                    ModelState.AddModelError("Range", "Select Range");
                    return View(vm);

                }
                //var HasExceedRateLimit = _services.HasExceedRateLimit(vm);
                //if (HasExceedRateLimit)
                //{
                //    ModelState.AddModelError("HasExceedRateLimit", "Exchange Rate exceeded limit");
                //    return View(vm);
                //}
                if (vm.Id > 0)
                {
                    _services.UpdateAuxAgentExchangeRate(vm);
                }
                else
                {
                    _services.AddAuxAgentExchangeRate(vm);
                }
                return RedirectToAction("Index", "ExchangeRate");
            }

            return View(vm);
        }

        [HttpGet]
        public JsonResult GetPreviousRate(string ReceivingCurrency, string ReceivingCountry, int TransferMethod, string Range)
        {
            string sendingCurrency = Common.Common.GetCountryCurrency(agentInfo.CountryCode);
            string SendingCountry = agentInfo.CountryCode;
            int AgentId = agentInfo.Id;
            var data = _services.GetRates(SendingCountry, ReceivingCountry, TransferMethod, Range, sendingCurrency, ReceivingCurrency, AgentId);

            if (data != null)
            {
                return Json(new
                {
                    Id = data.Id,
                    ExchangeRate = data.Rate,
               
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {

                ExchangeRate = 0,
            }, JsonRequestBehavior.AllowGet);
        }

        public void SetCountryViewBag()
        {
            var Countries = _commonServices.GetCountries();
            Countries.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");

        }
        public void SetCurrenciesViewBag()
        {
            var Currency = _commonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(Currency, "Code", "Name");
            ViewBag.ReceivingCurrencies = new SelectList(Currency, "Code", "Name");

        }

    }
}