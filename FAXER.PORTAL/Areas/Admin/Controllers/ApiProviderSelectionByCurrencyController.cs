using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ApiProviderSelectionByCurrencyController : Controller
    {
        // GET: Admin/ApiProviderSelectionByCurrency
        APIProviderSelectionByCurrencyServices _services = null;
        CommonServices _CommonServices = null;
        public ApiProviderSelectionByCurrencyController()
        {
            _services = new APIProviderSelectionByCurrencyServices();
            _CommonServices = new CommonServices();
        }

        public ActionResult Index(string sendingCurrency = "", string receivingCurrency = "", int transferMethod = 0, int transferType = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCurrencyViewBag();
            ViewBag.TransferMethod = transferMethod;
            ViewBag.TransferType = transferType;

            List<APIProviderSelectionViewModel> vm = _services.GetAPIProviderSelctionByCurrencyList(sendingCurrency, receivingCurrency, transferMethod, transferType);
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

        public ActionResult AddAPIProviderSelectionByCurrency(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCurrencyViewBag();
            SetCountryViewBag();
            var APIProviders = _CommonServices.GetAPIProviders();
            ViewBag.APIProviders = new SelectList(APIProviders, "Id", "Name");
            APIProviderSelectionViewModel vm = new APIProviderSelectionViewModel();
            if (id > 0)
            {
                vm = _services.GetAPIProviderSelctionByCurrency(id);
                ViewBag.Range = vm.Range;
            }
            var Agent = _CommonServices.GetAgent();
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");
            return View(vm);
        }
        [HttpPost]
        public ActionResult AddAPIProviderSelectionByCurrency([Bind(Include = APIProviderSelectionViewModel.BindProperty)] APIProviderSelectionViewModel vm)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCurrencyViewBag();
            SetCountryViewBag();
            var APIProviders = _CommonServices.GetAPIProviders();
            ViewBag.APIProviders = new SelectList(APIProviders, "Id", "Name");

            bool isAuxAgent = vm.TransferType == DB.TransactionTransferType.AuxAgent ? true : false;
            var Agent = _CommonServices.GetAgent(IsAUXAgent: isAuxAgent);
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");


            if (vm.TransferMethod == 0)
            {
                ModelState.AddModelError("TransferMethod", "Select Method");
                return View(vm);
            }
            if (vm.TransferType == 0)
            {
                ModelState.AddModelError("TransferType", "Select Method");
                return View(vm);
            }
            if (vm.Id == 0)
            {
                _services.AddAPiProviderSelectionByCurrency(vm);
            }
            else
            {
                _services.UpdateAPiProviderSelectionByCurrency(vm);
            }
            return RedirectToAction("Index", "ApiProviderSelectionByCurrency");

        }

        public JsonResult GetPreviousAPiProvider(string SendingCurrency = "", string ReceivingCurrency = "", string sendingCountry = "",
          string receivingCountry ="",int TransferType = 0, int TransferMethod = 0, string Range = "",int agentId = 0)
        {
            var Data = _services.GetPreviousAPIProviderSelctionByCurrency(SendingCurrency, ReceivingCurrency, sendingCountry , receivingCountry,
                TransferType, TransferMethod, Range, agentId);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        private void SetCountryViewBag()
        {
            var countries = _CommonServices.GetCountries();
            ViewBag.SendingCountry = new SelectList(countries, "Code", "Name");
            ViewBag.RecevingCountry = new SelectList(countries, "Code", "Name");
        }
        private void SetCurrencyViewBag()
        {
            var Currency = _CommonServices.GetCountryCurrencies();
            ViewBag.SendingCurrency = new SelectList(Currency, "Code", "Name");
            ViewBag.ReceivingCurrency = new SelectList(Currency, "Code", "Name");
        }
    }
}