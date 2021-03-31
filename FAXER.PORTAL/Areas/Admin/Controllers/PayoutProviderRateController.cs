using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class PayoutProviderRateController : Controller
    {
        PayoutProviderRateServices _services = null;
        CommonServices _CommonServices = null;
        public PayoutProviderRateController()
        {
            _services = new PayoutProviderRateServices();
            _CommonServices = new CommonServices();
        }

        // GET: Admin/PayoutProviderRate
        public ActionResult Index()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCurrencyViewBag();
            SetCountriesViewBag();
            var vm = _services.GetPayoutProviderRateViewModel();
            return View(vm);
        }
        public ActionResult SetPayoutProviderRate(int id = 0)
        {
            SetCurrencyViewBag();
            SetCountriesViewBag();
            PayoutProviderRateViewModel vm = new PayoutProviderRateViewModel();
            if (id > 0)
            {
                vm = _services.GetPayoutProviderRateViewModel().Where(x => x.Id == id).FirstOrDefault();
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult SetPayoutProviderRate([Bind(Include = PayoutProviderRateViewModel.BindProperty)] PayoutProviderRateViewModel vm)
        {
            SetCurrencyViewBag();
            SetCountriesViewBag();
            if (ModelState.IsValid)
            {
                if (vm.Rate == 0M)
                {
                    ModelState.AddModelError("Rate", "Enter Rate");
                    return View(vm);
                }
                var model = _services.GetPayoutProviderRateModel(vm);

                if (vm.Id == 0)
                {
                    _services.AddpayoutProviderRate(model);

                }
                else
                {
                    _services.UpdatepayoutProviderRate(model);
                }

                return RedirectToAction("Index", "PayoutProviderRate");
            }
            return View(vm);
        }

        [HttpGet]
        public JsonResult DeletePayoutProviderRate(int id)
        {
            if (id > 0)
            {
                _services.DeletepayoutProviderRate(id);
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

        private void SetCurrencyViewBag()
        {
            var currency = _CommonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(currency, "Code", "Name");
            ViewBag.RecevingCurrencies = new SelectList(currency, "Code", "Name");

        }
        private void SetCountriesViewBag()
        {
            var Countries = _CommonServices.GetCountries();
            Countries.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.RecevingCountries = new SelectList(Countries, "Code", "Name");
        }
    }
}