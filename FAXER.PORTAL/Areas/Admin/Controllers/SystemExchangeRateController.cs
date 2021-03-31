using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class SystemExchangeRateController : Controller
    {
        CommonServices _commonServices = null;
        SystemExchangeRateServices _systemExchangeRateServices = null;

        public SystemExchangeRateController()
        {
            _systemExchangeRateServices = new SystemExchangeRateServices();
            _commonServices = new CommonServices();
        }
        // GET: Admin/SystemExchangeRate
        public ActionResult Index()
        {
            return View(_systemExchangeRateServices.SystemExchangeRateTypeList());
        }
        public ActionResult SetSystemExchangeRate(int id = 0)
        {
            setCurrencyViewBag();
            SystemExchangeRateViewModel vm = new SystemExchangeRateViewModel();
            if (id != 0)
            {
                vm = _systemExchangeRateServices.SystemExchangeRateType(id);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult SetSystemExchangeRate([Bind(Include = SystemExchangeRateViewModel.BindProperty)]SystemExchangeRateViewModel systemExchangeRateTypeVm)
        {
            setCurrencyViewBag();
            if (ModelState.IsValid)
            {
                if (systemExchangeRateTypeVm.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransferMethod", "Select Transfer Method");
                    return View(systemExchangeRateTypeVm);
                }
                if (systemExchangeRateTypeVm.Id == 0)
                {
                    _systemExchangeRateServices.Add(systemExchangeRateTypeVm);
                }
                else
                {
                    _systemExchangeRateServices.Update(systemExchangeRateTypeVm);
                }
                return RedirectToAction("Index", "SystemExchangeRate");
            }
            return View(systemExchangeRateTypeVm);
        }
        [HttpGet]
        public JsonResult Delete(int id)
        {
            if (id > 0)
            {
                _systemExchangeRateServices.Delete(id);
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

        public void setCurrencyViewBag()
        {
            var currency = _commonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(currency, "Code", "Name");
            ViewBag.ReceivingCurrencies = new SelectList(currency, "Code", "Name");
        }

        public JsonResult UpdateSystemExhangeRate(int id)
        {
            _systemExchangeRateServices.UpdateSystemExhangeRate(id);
            return Json(true, JsonRequestBehavior.AllowGet);


        }
    }
}