using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using Microsoft.Office.Interop.Excel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AUXExchangeRateController : Controller
    {
        // GET: Admin/AUXExchangeRate
        CommonServices _commonServices = null;
        AuxAgentExhchangeRateLimitServices _auxAgentExhchangeRateLimitServices = null;

        public AUXExchangeRateController()
        {
            _commonServices = new CommonServices();
            _auxAgentExhchangeRateLimitServices = new AuxAgentExhchangeRateLimitServices();
        }

        #region Exchange Rate Limit
        public ActionResult Index(string SendingCountry = "", string City = "", string Date = "", int Method = 0, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var sendingcountries = _commonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var Cities = _commonServices.GetCities();
            ViewBag.Cities = new SelectList(Cities, "City", "City");
            ViewBag.Method = Method;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<ExchangeRateSettingViewModel> vm = _auxAgentExhchangeRateLimitServices.GetAuxAgentExchangeRateLimitList(SendingCountry, City, Date, Method).ToPagedList(pageNumber, pageSize);

            return View(vm);
        }

        public ActionResult SetAUXExchangeRate(int Id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var Countries = _commonServices.GetCountries();
            Countries.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");

            var Currencies = _commonServices.GetCountryCurrencies();
            ViewBag.sendingCurrency = new SelectList(Currencies, "Code", "Name");
            ViewBag.ReceivingCurrency = new SelectList(Currencies, "Code", "Name");

            ExchangeRateSettingViewModel vm = new ExchangeRateSettingViewModel();
            if (Id != 0)
            {
                vm = _auxAgentExhchangeRateLimitServices.GetAuxAgentExchangeRateLimitList().Where(x => x.Id == Id).FirstOrDefault();
                ViewBag.SelectedAgent = vm.AgentId;
            }

            var Agent = _commonServices.GetAgent(vm.SourceCountryCode, true);
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");
            return View(vm);
        }

        [HttpPost]
        public ActionResult SetAUXExchangeRate([Bind(Include = ExchangeRateSettingViewModel.BindProperty)]ExchangeRateSettingViewModel vm)
        {
            var Countries = _commonServices.GetCountries();
            Countries.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");

            var Currencies = _commonServices.GetCountryCurrencies();
            ViewBag.sendingCurrency = new SelectList(Currencies, "Code", "Name");
            ViewBag.ReceivingCurrency = new SelectList(Currencies, "Code", "Name");
            var Agent = _commonServices.GetAgent(vm.SourceCountryName);
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");
            if (ModelState.IsValid)
            {
                if (vm.ExchangeRate == 0)
                {
                    ModelState.AddModelError("Invalid", "Enter Exhange Rate");
                    return View(vm);
                }
                if (vm.Id == 0)
                {
                    _auxAgentExhchangeRateLimitServices.AddAuxAgentExchangeRateLimit(vm);
                }
                else
                {
                    _auxAgentExhchangeRateLimitServices.UpdateAuxAgentExchangeRateLimit(vm);
                }
                return RedirectToAction("Index");
            }
            return View(vm);
        }

        #endregion

        #region Exchange Rate
        public ActionResult ViewExchangeRate(string SendingCountry = "", string City = "", string Date = "", int Method = 0, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var sendingcountries = _commonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var Cities = _commonServices.GetCities();
            ViewBag.Cities = new SelectList(Cities, "City", "City");
            ViewBag.Method = Method;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<ExchangeRateSettingViewModel> vm = _auxAgentExhchangeRateLimitServices.GetAuxAgentExchangeRates(SendingCountry, City, Date, Method).ToPagedList(pageNumber, pageSize);

            return View(vm);
        }

        public ActionResult SetExchangeRateForAux(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var Countries = _commonServices.GetCountries();
            Countries.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");

            var Currency = _commonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(Currency, "Code", "Name");
            ViewBag.ReceivingCurrencies = new SelectList(Currency, "Code", "Name");

            ExchangeRateSettingViewModel vm = new ExchangeRateSettingViewModel();
            vm.TransferType = DB.TransactionTransferType.AuxAgent;
            if (id != 0)
            {
                vm = _auxAgentExhchangeRateLimitServices.GetAuxAgentExchangeRate(id);
            }
            vm.TransferType = DB.TransactionTransferType.AuxAgent;
            var Agent = _commonServices.GetAgent(vm.SourceCountryCode, true);
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");

            return View(vm);

        }
        [HttpPost]
        public ActionResult SetExchangeRateForAux([Bind(Include = ExchangeRateSettingViewModel.BindProperty)]ExchangeRateSettingViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var Countries = _commonServices.GetCountries();
            Countries.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");

            var Currency = _commonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(Currency, "Code", "Name");
            ViewBag.ReceivingCurrencies = new SelectList(Currency, "Code", "Name");


            var Agent = _commonServices.GetAgent(vm.SourceCountryCode, true);
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.SourceCurrencyCode))
                {
                    ModelState.AddModelError("SourceCurrencyCode", "Select Currency");
                    return View(vm);
                }
                if (string.IsNullOrEmpty(vm.DestinationCurrencyCode))
                {
                    ModelState.AddModelError("DestinationCurrencyCode", "Select Currency");
                    return View(vm);
                }
                if (vm.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransferMethod", "Select Transfer Method");
                    return View(vm);
                }
                if (vm.TransferType == 0)
                {
                    ModelState.AddModelError("TransferType", "Select Transfer Method");
                    return View(vm);
                }
                if (vm.ExchangeRate == 0)
                {
                    ModelState.AddModelError("Invalid", "Enter Exhange Rate");
                    return View(vm);
                }
                if (string.IsNullOrEmpty(vm.Range) == true)
                {
                    ModelState.AddModelError("", "Select Range");
                    return View(vm);

                }
                if (vm.Id > 0)
                {
                    _auxAgentExhchangeRateLimitServices.UpdateAuxAgentExchangeRate(vm);
                }
                else
                {

                    _auxAgentExhchangeRateLimitServices.AddAuxAgentExchangeRate(vm);
                }
            }
            return RedirectToAction("ViewExchangeRate", "AUXExchangeRate");
        }
        #endregion

        public JsonResult GetCountyByCurrency(string currency)
        {

            var countries = _commonServices.GetCountries().Where(x => x.CountryCurrency == currency).ToList();
            return Json(new
            {
                Data = countries
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAgentByCountry(string Country, string Currency = "")
        {
            var data = new List<AgentsListDropDown>();
            if (Country == "All" || Country == "")
            {
                var countries = _commonServices.GetCountries().Where(x => x.CountryCurrency == Currency).ToList();
                foreach (var item in countries)
                {
                    var agents = _commonServices.GetAuxAgents().Where(x => x.Country == item.Code).ToList();
                    data.AddRange(agents);
                }

            }
            else
            {
                data = _commonServices.GetAuxAgents().Where(x => x.Country == Country).ToList();
            }
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAuxAgentCode(int AgentId)
        {
            CommonServices _CommonServices = new CommonServices();
            var data = _CommonServices.GetAuxAgents().Where(x => x.AgentId == AgentId).Select(x => x.AgentCode).FirstOrDefault();
            return Json(new
            {
                AuxAgentCode = data
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPreviousRate(string sendingCurrency, string ReceivingCurrency, string SendingCountry, string ReceivingCountry, int TransferMethod, string Range, int AgentId)
        {

            var data = _auxAgentExhchangeRateLimitServices.GetRates(sendingCurrency, ReceivingCurrency, SendingCountry, ReceivingCountry, TransferMethod, Range, AgentId);
            if (data != null)
            {
                return Json(new
                {
                    Id = data.Id,
                    ExchangeRate = data.Rate,
                    TransferFeeByCurrencyId = data.TransferFeeByCurrencyId

                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {

                ExchangeRate = 0,
            }, JsonRequestBehavior.AllowGet);
        }


    }
}