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
    public class APIProviderSelectionController : Controller
    {
        APIProviderSelectionServices _services = null;
        CommonServices _CommonServices = null;
        public APIProviderSelectionController()
        {
            _services = new APIProviderSelectionServices();
            _CommonServices = new CommonServices();
        }

        // GET: Admin/APIProviderSelection
        public ActionResult Index(string sendingCountry = "", string receivingCounty = "", int transferMethod = 0, int transferType = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name", receivingCounty);
            ViewBag.TransferMethod = transferMethod;
            ViewBag.TransferType = transferType;

            List<APIProviderSelectionViewModel> vm = _services.GetAPIProviderSelctionList(sendingCountry, receivingCounty, transferMethod, transferType);
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

        public ActionResult AddAPIProviderSelection(int id = 0, string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int TransferMethod = 0, string Range = "")
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();

            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.Range = Range;

            var APIProviders = _CommonServices.GetAPIProviders();
            ViewBag.APIProviders = new SelectList(APIProviders, "Id", "Name");
            bool isAuxAgent = TransferType == 4 ? true : false;
            var Agent = _CommonServices.GetAgent(SendingCountry, isAuxAgent);
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");

            APIProviderSelectionViewModel vm = new APIProviderSelectionViewModel();

            if (id > 0)
            {
                vm = _services.GetAPIProviderSelction(id);
                ViewBag.Range = vm.Range;

                return View(vm);
            }
            if (!string.IsNullOrEmpty(SendingCountry) && !string.IsNullOrEmpty(ReceivingCountry) && TransferType != 0 && TransferMethod != 0 && !string.IsNullOrEmpty(Range))
            {
                vm = _services.GetPreviousAPIProviderSelction(SendingCountry, ReceivingCountry, TransferType, TransferMethod, Range);
                return View(vm);
            }
            return View();

        }
        [HttpPost]
        public ActionResult AddAPIProviderSelection([Bind(Include = APIProviderSelectionViewModel.BindProperty)]APIProviderSelectionViewModel vm)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");

            var APIProviders = _CommonServices.GetAPIProviders();
            ViewBag.APIProviders = new SelectList(APIProviders, "Id", "Name");
            bool isAuxAgent = vm.TransferType == TransactionTransferType.AuxAgent ? true : false;
            var Agent = _CommonServices.GetAgent(vm.SendingCountry, isAuxAgent);
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");

            if (ModelState.IsValid)
            {
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
                    _services.AddAPiProviderSelection(vm);
                }
                else
                {
                    _services.UpdateAPiProviderSelection(vm);
                }
                return RedirectToAction("Index", "APIProviderSelection");
            }
            return View(vm);

        }

    }
}