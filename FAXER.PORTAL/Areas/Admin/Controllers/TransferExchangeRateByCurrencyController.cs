using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class TransferExchangeRateByCurrencyController : Controller
    {
        TransferExchangeRateByCurrencyServices _services = null;
        CommonServices _commonServices = null;
        public TransferExchangeRateByCurrencyController()
        {
            _services = new TransferExchangeRateByCurrencyServices();
            _commonServices = new CommonServices();
        }

        // GET: Admin/TransferExchangeRateByCurrency
        public ActionResult Index(string SendingCurrency = "", string ReceivingCurrecny = "", int TransferType = 0, int agent = 0, int TransferMethod = 0, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCurrencyViewBag();
            SetAgentViewBag();

            ViewBag.TransferType = TransferType;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<TransferExchangeRateByCurrencyViewModel> vm = _services.GetExchangeRateList(SendingCurrency, ReceivingCurrecny, TransferType, agent, TransferMethod).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }
        public ActionResult SetTransferExchangeRate(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountryViewBag();
            SetCurrencyViewBag();
            SetAgentViewBag();

            TransferExchangeRateByCurrencyViewModel vm = new TransferExchangeRateByCurrencyViewModel();
            if (id != 0)
            {
                vm = _services.GetExchangeRate(id);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult SetTransferExchangeRate([Bind(Include = TransferExchangeRateByCurrencyViewModel.BindProperty)]TransferExchangeRateByCurrencyViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountryViewBag();
            SetCurrencyViewBag();
            SetAgentViewBag();
            if (ModelState.IsValid)
            {
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
                if (vm.Rate == 0)
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
                    _services.UpdateRate(vm);
                }
                else
                {
                    _services.AddRate(vm);
                }
                return RedirectToAction("Index", "TransferExchangeRateByCurrency");
            }

            return View(vm);
        }
        [HttpGet]
        public JsonResult Delete(int id)
        {
            if (id > 0)
            {
                _services.Delete(id);
                return Json(new
                {
                    Data = true,
                    Message = "Deleted Sucessfully"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Data = false,
                    Message = "Something went wrong. Please try again!"
                }, JsonRequestBehavior.AllowGet);
            }
        }
    

        public JsonResult GetRates(string sendingCurency = "", string receivingCurrency = "", string sendingCountry = "", string receivingCounrty = "",
           int transferType = 0, int method = 0, int agent = 0, string range = "")
        {
            var ExchangeRate = _services.GetRate(sendingCurency, receivingCurrency, sendingCountry, receivingCounrty, transferType, method, agent, range);
            return Json(new
            {
                Rate = ExchangeRate
            }, JsonRequestBehavior.AllowGet);
        }
        public void SetCountryViewBag()
        {
            var countries = _commonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(countries, "Code", "Name");
        }
        public void SetAgentViewBag()
        {
            var Agent = _commonServices.GetAgent();
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");
        }
        public void SetCurrencyViewBag()
        {
            var Currency = _commonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(Currency, "Code", "Name");
            ViewBag.ReceivingCurrencies = new SelectList(Currency, "Code", "Name");
        }

    }
}