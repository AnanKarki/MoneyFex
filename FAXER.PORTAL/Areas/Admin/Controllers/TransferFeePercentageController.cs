using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class TransferFeePercentageController : Controller
    {
        STransferFeePercentageServices _transferFee = null;
        CommonServices _commonServices = null;
        public TransferFeePercentageController()
        {
            _transferFee = new STransferFeePercentageServices();
            _commonServices = new CommonServices();
        }
        // GET: Admin/TransferFeePercentage
        public ActionResult Index(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int TransferMethod = 0, int? page = null)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountryViewBag();
            ViewBag.TransferType = TransferType;
            ViewBag.TransferMethod = TransferMethod;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<TransferFeePercentageViewModel> vm = _transferFee.GetTranferfee(SendingCountry, ReceivingCountry, TransferType, TransferMethod).Data.ToPagedList(pageNumber, pageSize); ;
            return View(vm);
        }

        [HttpGet]
        public ActionResult SetTransferfee(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountryViewBag();
            SetAgentViewBag();
            SetCurrencyViewBag();
            TransferFeePercentageViewModel vm = new TransferFeePercentageViewModel();
            if (id > 0)
            {
                vm = _transferFee.GetTranferfeeById(id);
            }
            return View(vm);

        }
        [HttpPost]
        public ActionResult SetTransferFee([Bind(Include = TransferFeePercentageViewModel.BindProperty)]TransferFeePercentageViewModel vm)
        {
            SetCountryViewBag();
            SetAgentViewBag();
            SetCurrencyViewBag();
            if (ModelState.IsValid)
            {
                if (vm.Range == 0)
                {
                    ModelState.AddModelError("", "Select Range");
                    return View(vm);    
                }
                if (vm.TransferType == 0)
                {
                    ModelState.AddModelError("TransferType", "Select Transfer Type");
                    return View(vm);
                }
                if (vm.TransferMethod == 0)
                {
                    ModelState.AddModelError("Transfermethod", "Select Transfer Method");
                    return View(vm);
                }
                if (vm.Id > 0)
                {
                    _transferFee.UpdatetransferFeePercentage(vm);
                }
                else
                {
                    _transferFee.CreateTransferFee(vm);
                }
                return RedirectToAction("Index", "TransferFeePercentage");
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult UpdateTransferFee(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountryViewBag();
            SetAgentViewBag();
            SetCurrencyViewBag();

            TransferFeePercentageViewModel vm = _transferFee.GetTranferfeeById(id);
            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateTransferFee([Bind(Include = TransferFeePercentageViewModel.BindProperty)]TransferFeePercentageViewModel vm)
        {
            SetCountryViewBag();
            SetAgentViewBag();
            SetCurrencyViewBag();
            if (ModelState.IsValid)
            {
                if (vm.Range == 0)
                {
                    ModelState.AddModelError("", "Select Range");
                    return View(vm);

                }

                if (vm.TransferType == 0)
                {
                    ModelState.AddModelError("TransferType", "Select Transfer Type");
                    return View(vm);

                }
                if (vm.TransferMethod == 0)
                {
                    ModelState.AddModelError("Transfermethod", "Select Transfer Method");
                    return View(vm);

                }

                _transferFee.UpdatetransferFeePercentage(vm);
                _transferFee.CreateTransferFeeHistory(vm);
                return RedirectToAction("Index", "TransferFeePercentage");
            }
            return View(vm);
        }


        [HttpGet]
        public JsonResult DeleteTransferFee(int id)
        {
            if (id > 0)
            {
                var data = _transferFee.List().Data.Where(x => x.Id == id).FirstOrDefault();
                var result = _transferFee.Remove(data);

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

        public ActionResult TransferFeeHistory(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int TransferMethod = 0, int Year = 0, int Month = 0, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var sendingcountries = _transferFee.GetCountries(SendingCountry);
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var receivingCountries = _transferFee.GetCountries(ReceivingCountry);
            ViewBag.ReceivingCountries = new SelectList(receivingCountries, "Code", "Name");

            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            ViewBag.TransferType = TransferType;
            ViewBag.Month = Month;
            ViewBag.TransferMethod = TransferMethod;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<TransferFeePercentageHistoryViewModel> vm = _transferFee.GetTranferfeeHistory(SendingCountry, ReceivingCountry, TransferType, TransferMethod, Year, Month).Data.ToPagedList(pageNumber, pageSize);


            return View(vm);
        }
        public JsonResult GetAgentByCountry(string Country)
        {

            var data = _commonServices.GetAgents().Where(x => x.Country == Country).ToList();
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTransferFee(string sendingCountry = "", string receivingCounrty = "",
            string sendingCurrency = "", string receivingCurrency = "", int transferType = 0,
            int method = 0, int Range = 0, int FeeType = 0, string otherRange = "",
            int AgentId = 0)
        {
            var transferFee = _transferFee.GetFee(sendingCountry, receivingCounrty, sendingCurrency, receivingCurrency, transferType, method, Range, FeeType, AgentId);
            return Json(new
            {
                transferFee
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