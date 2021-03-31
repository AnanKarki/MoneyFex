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
    public class IntroductoryRateByCurencyController : Controller
    {
        CommonServices _CommonServices = null;
        IntroductoryRateByCurencyServices _services = null;
        public IntroductoryRateByCurencyController()
        {
            _CommonServices = new CommonServices();
            _services = new IntroductoryRateByCurencyServices();
        }


        // GET: Admin/IntroductoryRateByCurency
        public ActionResult Index(string SendingCurrency = "", string ReceivingCurrency = "", int TransferType = 0, int Agent = 0, int transferMethod = 0, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var currencies = _CommonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(currencies, "Code", "Name");
            ViewBag.ReceivingCurrencies = new SelectList(currencies, "Code", "Name");

            var agents = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            ViewBag.TransferType = TransferType;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<IntroductoryRateByCurencyViewModel> vm = _services.GetIntroductoryRateList(SendingCurrency, ReceivingCurrency, TransferType, Agent, transferMethod).ToPagedList(pageNumber, pageSize);

            return View(vm);
        }
        [HttpGet]
        public ActionResult SetRate(int Id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var currencies = _CommonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(currencies, "Code", "Name");
            ViewBag.ReceivingCurrencies = new SelectList(currencies, "Code", "Name");

            var agents = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");

            IntroductoryRateByCurencyViewModel vm = new IntroductoryRateByCurencyViewModel();

            if (Id != 0)
            {
                vm = _services.GetIntroductoryRate(Id);
            }

            return View(vm);
        }


        [HttpPost]
        public ActionResult SetRate([Bind(Include = IntroductoryRateByCurencyViewModel.BindProperty)] IntroductoryRateByCurencyViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var currencies = _CommonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(currencies, "Code", "Name");
            ViewBag.ReceivingCurrencies = new SelectList(currencies, "Code", "Name");

            var agents = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            if (ModelState.IsValid)
            {
                if (vm.TransactionTransferType == 0)
                {
                    ModelState.AddModelError("TransactionTransferType", "Select Transfer Type");
                    return View(vm);

                }
                if (vm.TransactionTransferMethod == 0)
                {
                    ModelState.AddModelError("TransactionTransferMethod", "Select Transfer Method");
                    return View(vm);

                }
                if (vm.Rate == 0)
                {
                    ModelState.AddModelError("Invalid", "Enter Introductory Rate");
                    return View(vm);
                }
                if (string.IsNullOrEmpty(vm.Range) == true)
                {
                    ModelState.AddModelError("", "Select Range");
                    return View(vm);

                }
                if (vm.Id > 0)
                {
                    _services.UpdateIntroductoryRate(vm);

                }
                else
                {
                    _services.AddIntroductoryRate(vm);

                }
                return RedirectToAction("Index", "IntroductoryRateByCurency");

            }
            return View(vm);

        }

       
        [HttpGet]
        public JsonResult Delete(int Id)
        {
            if (Id > 0)
            {
                _services.Delete(Id);
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

        public JsonResult GetAgentByCurrency(string Currency)
        {
            var country = _CommonServices.getCountryCodeFromCurrency(Currency);


            var data = _CommonServices.GetAgents().Where(x => x.Country == country).ToList();
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPreviousRate(int AgentId)
        {

            var data = _services.GetRates().Where(x => x.AgentId == AgentId).FirstOrDefault();
            if (data != null)
            {
                return Json(new
                {
                    Id = data.Id,
                    Range = data.FromRange + "-" + data.ToRange,
                    Rate = data.Rate,
                    NoOfTransaction = data.NoOfTransaction
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {

                Id = 0,
                Range = "",
                Rate = 0,
                NoOfTransaction = ""
            }, JsonRequestBehavior.AllowGet);
        }

    }
}