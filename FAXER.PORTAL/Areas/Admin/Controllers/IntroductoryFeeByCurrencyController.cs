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
    public class IntroductoryFeeByCurrencyController : Controller
    {
        CommonServices _CommonServices = null;
        IntroductoryFeeByCurrencyServices _services = null;
        public IntroductoryFeeByCurrencyController()
        {
            _CommonServices = new CommonServices();
            _services = new IntroductoryFeeByCurrencyServices();
        }

        // GET: Admin/IntroductoryFeeByCurrency
        public ActionResult Index(string SendingCurrency = "", string ReceivingCurrency = "", int TransferType = 0, int TransferMethod = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var currency = _CommonServices.GetCountryCurrencies();

            ViewBag.SendingCurrencies = new SelectList(currency, "Code", "Name");
            ViewBag.ReceivingCurrencies = new SelectList(currency, "Code", "Name");
            ViewBag.TransferType = TransferType;
            ViewBag.TransferType = TransferType;
            ViewBag.TransferMethod = TransferMethod;

            List<IntroductoryFeeByCurrencyViewModel> vm = _services.GetIntroductoryfeeList(SendingCurrency, ReceivingCurrency, TransferType, TransferMethod);
            return View(vm);
        }


        [HttpGet]
        public ActionResult SetIntroductoryFeeByCurrency(int Id = 0)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var currency = _CommonServices.GetCountryCurrencies();

            ViewBag.SendingCurrencies = new SelectList(currency, "Code", "Name");

            ViewBag.ReceivingCurrencies = new SelectList(currency, "Code", "Name");

            var agents = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");

            IntroductoryFeeByCurrencyViewModel vm = new IntroductoryFeeByCurrencyViewModel();

            if (Id != 0)
            {
                vm = _services.GetIntroductoryFee(Id);
            }
            return View(vm);

        }
        [HttpPost]
        public ActionResult SetIntroductoryFeeByCurrency([Bind(Include = IntroductoryFeeByCurrencyViewModel.BindProperty)]IntroductoryFeeByCurrencyViewModel vm)
        {
            var currency = _CommonServices.GetCountryCurrencies();

            ViewBag.SendingCurrencies = new SelectList(currency, "Code", "Name");

            ViewBag.ReceivingCurrencies = new SelectList(currency, "Code", "Name");

            var agents = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");

            if (ModelState.IsValid)
            {
                if (vm.TransferType == 0)
                {
                    ModelState.AddModelError("TransferType", "Select Transfer Type");
                    return View(vm);

                }
                if (vm.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransferMethod", "Select Transfer Method");
                    return View(vm);

                }
                if (string.IsNullOrEmpty(vm.Range))
                {
                    ModelState.AddModelError("", "Select Range");
                    return View(vm);
                }
                if (vm.FeeType == 0)
                {
                    ModelState.AddModelError("FeeType", "Select Fee Type");
                    return View(vm);

                }
                if (vm.Id == 0)
                {
                    _services.Add(vm);

                }
                else
                {
                    _services.Update(vm);
                }

                return RedirectToAction("Index", "IntroductoryFeeByCurrency");
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


        public JsonResult GetFees(string sendingCurrency = "", string receivingCurrency = "", int transferType = 0, int method = 0, string Range = "", int FeeType = 0, string otherRange = "", int AgentId = 0)
        {
            CommonServices _CommonServices = new CommonServices();
            var ExchangeRate = _services.GetFee(sendingCurrency, receivingCurrency, transferType, method, Range, FeeType, otherRange, AgentId);

            return Json(new
            {
                TransferFee = ExchangeRate

            }, JsonRequestBehavior.AllowGet);
        }


    }
}