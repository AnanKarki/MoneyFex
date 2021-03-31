using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{

    public class AUXAgentExchangeFeeController : Controller
    {
        /// <summary>
        /// Aux Agnet Transfer Fee
        /// </summary>

        // GET: Admin/AUXAgentExchangeFee
        CommonServices _commonServices = null;
        AuxAgentTransferFeeLimitServices _auxAgentTransferFee = null;
        public AUXAgentExchangeFeeController()
        {
            _commonServices = new CommonServices();
            _auxAgentTransferFee = new AuxAgentTransferFeeLimitServices();
        }
        public ActionResult Index(string SendingCountry = "", int AgentId = 0, string Date = "", int Method = 0, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var sendingcountries = _commonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var agents = _commonServices.GetAuxAgents();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<TransferFeePercentageViewModel> vm = _auxAgentTransferFee.GetAuxAgentTranferfees(SendingCountry, AgentId, Date, Method).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }

        [HttpGet]
        public ActionResult SetAUXExchangeFeeForAux(int Id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _commonServices.GetCountries();

            Countries.Add(new DropDownViewModel()
            {
                Code = "ALl",
                Name = "All"
            });
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");
            var Currencies = _commonServices.GetCountryCurrencies();
            ViewBag.sendingCurrency = new SelectList(Currencies, "Code", "Name");
            ViewBag.ReceivingCurrency = new SelectList(Currencies, "Code", "Name");


            TransferFeePercentageViewModel vm = new TransferFeePercentageViewModel();
            if (Id != 0)
            {
                vm = _auxAgentTransferFee.GetAuxAgentTranferFee(Id);

                ViewBag.SelectedSendingCurrency = vm.SendingCurrency;
                ViewBag.SelectedReceivingCurrency = vm.ReceivingCurrency;
            }
            vm.TransferType = DB.TransactionTransferType.AuxAgent;
            var agents = _commonServices.GetAgent(vm.SendingCountry, true);
            ViewBag.Agent = new SelectList(agents, "AgentId", "AgentName");

            return View(vm);
        }

        [HttpPost]
        public ActionResult SetAUXExchangeFeeForAux([Bind(Include = TransferFeePercentageViewModel.BindProperty)]TransferFeePercentageViewModel vm)
        {
            var Countries = _commonServices.GetCountries();
            Countries.Add(new DropDownViewModel()
            {
                Code = "ALl",
                Name = "All"
            });
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");
            var Currencies = _commonServices.GetCountryCurrencies();
            ViewBag.sendingCurrency = new SelectList(Currencies, "Code", "Name");
            ViewBag.ReceivingCurrency = new SelectList(Currencies, "Code", "Name");

            var agents = _commonServices.GetAgent(vm.SendingCountry, true);
            ViewBag.Agent = new SelectList(agents, "AgentId", "AgentName");

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.SendingCurrency))
                {
                    ModelState.AddModelError("SendingCurrency", "Select Currency");
                    return View(vm);
                }
                if (string.IsNullOrEmpty(vm.ReceivingCurrency))
                {
                    ModelState.AddModelError("ReceivingCurrency", "Select Currency");
                    return View(vm);
                }
                if (vm.TransferMethod == DB.TransactionTransferMethod.Select)
                {
                    ModelState.AddModelError("TransferMethod", "select Transfer Method");
                    return View(vm);
                }
                if (string.IsNullOrEmpty(vm.RangeName))
                {
                    ModelState.AddModelError("Range", "select Range");
                    return View(vm);
                }
                if (vm.FeeType == DB.FeeType.Select)
                {
                    ModelState.AddModelError("FeeType", "Enter Fee Type");
                    return View(vm);
                }
                if (vm.Fee == 0)
                {
                    ModelState.AddModelError("Fee", "Enter Fee");
                    return View(vm);
                }
                if (vm.Id == 0)
                {

                    _auxAgentTransferFee.AddTransferFee(vm);
                }
                else
                {
                    _auxAgentTransferFee.UpdateTransferFee(vm);
                }



                return RedirectToAction("Index");
            }
            return View(vm);
        }

        #region transfer Fee Limit
        [HttpGet]
        public ActionResult SetAUXExchangeFee(int Id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _commonServices.GetCountries();

            Countries.Add(new DropDownViewModel()
            {
                Code = "ALl",
                Name = "All"
            });
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");
            var Currencies = _commonServices.GetCountryCurrencies();
            ViewBag.sendingCurrency = new SelectList(Currencies, "Code", "Name");
            ViewBag.ReceivingCurrency = new SelectList(Currencies, "Code", "Name");

            TransferFeePercentageViewModel vm = new TransferFeePercentageViewModel();
            if (Id != 0)
            {
                vm = _auxAgentTransferFee.GetAuxAgentTranferfeeLimit(Id);
                ViewBag.SelectedSendingCurrency = vm.SendingCurrency;
                ViewBag.SelectedReceivingCurrency = vm.ReceivingCurrency;
            }
            var agents = _commonServices.GetAgent(vm.SendingCountry, true);
            ViewBag.Agent = new SelectList(agents, "AgentId", "AgentName");

            return View(vm);
        }
        [HttpPost]
        public ActionResult SetAUXExchangeFee([Bind(Include = TransferFeePercentageViewModel.BindProperty)]TransferFeePercentageViewModel vm)
        {
            var Countries = _commonServices.GetCountries();
            Countries.Add(new DropDownViewModel()
            {
                Code = "ALl",
                Name = "All"
            });
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");
            var Currencies = _commonServices.GetCountryCurrencies();
            ViewBag.sendingCurrency = new SelectList(Currencies, "Code", "Name");
            ViewBag.ReceivingCurrency = new SelectList(Currencies, "Code", "Name");

            var Agent = _commonServices.GetAuxAgents().Where(x => x.Country == vm.SendingCountry);
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");

            if (ModelState.IsValid)
            {

                if (vm.Fee == 0)
                {
                    ModelState.AddModelError("Invalid", "Enter Exhange Fee");
                    return View(vm);
                }
                if (vm.Id == 0)
                {

                    _auxAgentTransferFee.AddAuxAgentTransferFeeLimit(vm);
                }
                else
                {
                    _auxAgentTransferFee.UpdateAuxAgentTransferFeeLimit(vm);
                }



                return RedirectToAction("Index");
            }
            return View(vm);
        }
        [HttpGet]
        public JsonResult DeleteAUXAgentExchangeFee(int id)
        {
            if (id > 0)
            {
                _auxAgentTransferFee.RemoveAuxAgentTransferFeeLimit(id);
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
                AccountNo = data
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPreviousRate(string sendingCurrency, string ReceivingCurrency, string SendingCountry, string ReceivingCountry,
            int TransferMethod, string Range, int AgentId, int Feetype)
        {

            var data = _auxAgentTransferFee.GetRates(sendingCurrency, ReceivingCurrency, SendingCountry, ReceivingCountry, TransferMethod, Range, AgentId, Feetype);
            if (data != null)
            {
                return Json(new
                {
                    Id = data.Id,
                    Fee = data.Fee,
                    TransfeFeeByCurrencyId = data.TransfeFeeByCurrencyId

                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {

                Fee = 0,
            }, JsonRequestBehavior.AllowGet);
        }


    }
}