using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class TransferFeeController : Controller
    {
        AgentInformation agentInfo = null;
        TransferFeeServices _services = null;
        CommonServices _commonServices = null;
        public TransferFeeController()
        {
            agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            _services = new TransferFeeServices();
            _commonServices = new CommonServices();
        }
        // GET: Agent/TransferFee
        public ActionResult Index(string ReceivingCountry = "", int TransferType = 0, int TransferMethod = 0)
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            var Countries = _commonServices.GetCountries();
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "name", ReceivingCountry);
            List<TransferFeePercentageViewModel> vm = new List<TransferFeePercentageViewModel>();
            vm = _services.GetTranferfeeList(ReceivingCountry, TransferType, TransferMethod);
            ViewBag.TransferMethod = TransferMethod;
            ViewBag.TransferType = TransferType;
            return View(vm);
        }
        public ActionResult SetTransferFee(int Id = 0)
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            var Countries = _commonServices.GetCountries();
            Countries.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "name");
            SetCurrenciesViewBag();
            TransferFeePercentageViewModel vm = new TransferFeePercentageViewModel();

            if (Id > 0)
            {
                vm = _services.GetTranferfeeByCurrency(Id);
            }
            vm.SendingCountry = agentInfo.CountryCode;
            vm.SendingCurrency = Common.Common.GetCountryCurrency(agentInfo.CountryCode);
            vm.TransferType = TransactionTransferType.AuxAgent;
            return View(vm);
        }
        [HttpPost]
        public ActionResult SetTransferFee([Bind(Include = TransferFeePercentageViewModel.BindProperty)] TransferFeePercentageViewModel vm)
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            var Countries = _commonServices.GetCountries();
            Countries.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "name", vm.ReceivingCountry);
            SetCurrenciesViewBag();
            vm.SendingCountry = agentInfo.CountryCode;
            vm.SendingCurrency = Common.Common.GetCountryCurrency(agentInfo.CountryCode);
            if (string.IsNullOrEmpty(vm.ReceivingCurrency))
            {
                ModelState.AddModelError("ReceivingCurrency", "Select Currency");
                return View(vm);

            }
            if (string.IsNullOrEmpty(vm.ReceivingCountry))
            {
                ModelState.AddModelError("ReceivingCountry", "Select Country");
                return View(vm);

            }
            if (vm.TransferType == TransactionTransferType.Select)
            {
                ModelState.AddModelError("TransferType", "Select Type");
                return View(vm);

            }
            if (vm.TransferMethod == TransactionTransferMethod.Select)
            {
                ModelState.AddModelError("TransferMethod", "Select Transfer Method");
                return View(vm);

            }

            if (string.IsNullOrEmpty(vm.RangeName))
            {
                ModelState.AddModelError("Range", "select Range");
                return View(vm);
            }

            if (vm.FeeType == FeeType.Select)
            {
                ModelState.AddModelError("FeeType", "Select Fee Type");
                return View(vm);

            }
            if (vm.Fee == 0m)
            {
                ModelState.AddModelError("Fee", "Enter Fee");
                return View(vm);

            }
            vm.AgentId = agentInfo.Id;
            ////var HasExceededTransferFeeLimit = _services.HasExceededTransferFeeLimit(vm);
            ////if (HasExceededTransferFeeLimit)
            ////{

            ////    ModelState.AddModelError("HasExceededTransferFeeLimit", "Transfer Fee Limit exceeded");
            ////    return View(vm);

            ////}
            if (vm.Id > 0)
            {
                _services.UpdateTransferFee(vm);
            }
            else
            {
                _services.AddTransferFee(vm);
            }

            return RedirectToAction("Index", "TransferFee");



        }

        [HttpGet]
        public JsonResult GetPreviousRate(string ReceivingCurrency, string ReceivingCountry, int Transfertype,
            int TransferMethod, string Range, int FeeType)
        {
            var SendingCountry = agentInfo.CountryCode;
            var sendingCurrency = Common.Common.GetCountryCurrency(agentInfo.CountryCode);
            int AgentId = agentInfo.Id;
            var data = _services.GetRates(SendingCountry, ReceivingCountry, sendingCurrency,
                ReceivingCurrency, Transfertype, TransferMethod, Range, FeeType, AgentId);
            if (data != null)
            {
                return Json(new
                {
                    Id = data.Id,
                    Fee = data.Fee,
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {

                Fee = 0,
            }, JsonRequestBehavior.AllowGet);
        }
        public void SetCurrenciesViewBag()
        {
            var Currency = _commonServices.GetCountryCurrencies();
            ViewBag.ReceivingCurrencies = new SelectList(Currency, "Code", "Name");

        }
    }
}