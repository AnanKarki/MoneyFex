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
    public class TransferFeePercentageByCurrencyController : Controller
    {
        CommonServices _commonServices = null;
        TransferFeePercentageByCurrencyServices _services = null;
        public TransferFeePercentageByCurrencyController()
        {
            _commonServices = new CommonServices();
            _services = new TransferFeePercentageByCurrencyServices();

        }
        // GET: Admin/TransferFeePercentageByCurrency
        public ActionResult Index(string SendingCurrency = "", string ReceivingCurrency = "", int TransferType = 0, int TransferMethod = 0, int? page = null)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetAgentViewBag();
            SetCurrencyViewBag();
            ViewBag.TransferType = TransferType;
            ViewBag.TransferMethod = TransferMethod;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<TransferFeePercentageByCurrencyViewModel> vm = _services.GetTranferfeeList(SendingCurrency, ReceivingCurrency, TransferType, TransferMethod).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }
        public ActionResult AddTransferFeePercentageByCurrency(int Id = 0)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountryViewBag();
            SetAgentViewBag();
            SetCurrencyViewBag();
            TransferFeePercentageByCurrencyViewModel vm = new TransferFeePercentageByCurrencyViewModel();
            if (Id != 0)
            {
                vm = _services.GetTranferFee(Id);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult AddTransferFeePercentageByCurrency([Bind(Include = TransferFeePercentageByCurrencyViewModel.BindProperty)]TransferFeePercentageByCurrencyViewModel vm)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountryViewBag();
            SetAgentViewBag();
            SetCurrencyViewBag();
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Range))
                {
                    ModelState.AddModelError("", "Select Range");
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

                return RedirectToAction("Index", "TransferFeePercentageByCurrency");
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


        public void SetCountryViewBag()
        {
            var countries = _commonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(countries, "Code", "Name");
        }
        public void SetAgentViewBag()
        {
            var agents = _commonServices.GetAgent();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
        }
        public void SetCurrencyViewBag()
        {
            var Currency = _commonServices.GetCountryCurrencies();
            ViewBag.SendingCurrency = new SelectList(Currency, "Code", "Name");
            ViewBag.ReceivingCurrency = new SelectList(Currency, "Code", "Name");
        }
    }
}

