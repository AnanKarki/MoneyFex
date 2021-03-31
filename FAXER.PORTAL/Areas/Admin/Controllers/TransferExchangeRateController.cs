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
    public class TransferExchangeRateController : Controller
    {
        STransferExchangeRateServices _transferExchangeRate = null;
        CommonServices _commonServices = null;

        public TransferExchangeRateController()
        {
            _transferExchangeRate = new STransferExchangeRateServices();
            _commonServices = new CommonServices();
        }

        // GET: Admin/TransferExchangeRate
        public ActionResult Index(int StaffId = 0, string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int agent = 0, int transferMethod = 0, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountryViewBag();
            SetAgentViewBag();

            ViewBag.TransferType = TransferType;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<ExchangeRateSettingViewModel> vm = _transferExchangeRate.GetExchangeRate(StaffId, SendingCountry, ReceivingCountry, TransferType, agent, transferMethod).Data.ToPagedList(pageNumber, pageSize);
            return View(vm);
        }

        public ActionResult SetTransferExchangeRate(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountryViewBag();
            SetAgentViewBag();
            SetCurrencyViewBag();
            ExchangeRateSettingViewModel vm = new ExchangeRateSettingViewModel();
            if (id > 0)
            {
                vm = _transferExchangeRate.GetExchangeRateById(id);
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult SetTransferExchangeRate([Bind(Include = ExchangeRateSettingViewModel.BindProperty)]ExchangeRateSettingViewModel vm)
        {
            SetCountryViewBag();
            SetAgentViewBag();
            SetCurrencyViewBag();
            if (ModelState.IsValid)
            {
                if (vm.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransferMethod", "Select Transfer Method");
                    return View(vm);
                }
                if (vm.TransferType == 0)
                {
                    ModelState.AddModelError("TransferType", "Select Transfer Method");
                    return View(vm);
                }
                if (vm.ExchangeRate == 0)
                {
                    ModelState.AddModelError("Invalid", "Enter Exhange Rate");
                    return View(vm);
                }
                if (string.IsNullOrEmpty(vm.Range) == true)
                {
                    ModelState.AddModelError("", "Select Range");
                    return View(vm);

                }

                if (vm.Id == 0)
                {
                    _transferExchangeRate.AddExchangeRate(vm);
                }
                else
                {
                    _transferExchangeRate.UpdateExchangeRate(vm);
                }

                return RedirectToAction("Index", "TransferExchangeRate");
            }
            return View(vm);
        }

        public ActionResult UpdateTransferExchangeRate(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetCountryViewBag();
            SetAgentViewBag();
            SetCurrencyViewBag();

            ExchangeRateSettingViewModel vm = _transferExchangeRate.GetExchangeRateById(id);

            var Agent = _commonServices.GetAgent(vm.SourceCountryName);
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");

            return View(vm);
        }

        [HttpPost]
        public ActionResult UpdateTransferExchangeRate([Bind(Include = ExchangeRateSettingViewModel.BindProperty)]ExchangeRateSettingViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetCountryViewBag();
            if (ModelState.IsValid)
            {
                if (vm.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransferMethod", "Select Transfer Method");
                    return View(vm);
                }
                if (vm.TransferType == 0)
                {
                    ModelState.AddModelError("TransferType", "Select Transfer Method");
                    return View(vm);
                }
                if (vm.ExchangeRate == 0)
                {
                    ModelState.AddModelError("Invalid", "Enter Exhange Rate");

                    return View(vm);
                }
                if (string.IsNullOrEmpty(vm.Range) == true)
                {
                    ModelState.AddModelError("", "Select Range");
                    return View(vm);

                }



                return RedirectToAction("Index", "TransferExchangeRate");
            }
            return View(vm);
        }

        [HttpGet]
        public JsonResult DeleteTExchangeRate(int id)
        {
            if (id > 0)
            {
                _transferExchangeRate.RemoveExchangeRate(id);
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


        public ActionResult TransferExchangeRateHistory(string SendingCountry = "", string ReceivingCountry = "",
            int TransferType = 0, int Agent = 0, int Year = 0, int Month = 0, int Day = 0, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetCountryViewBag();
            SetAgentViewBag();

            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            ViewBag.TransferType = TransferType;
            ViewBag.Month = Month;
            ViewBag.Day = Day;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<TransferExchangerateHistoryViewModel> vm = _transferExchangeRate.GetExchangeRateHistory(SendingCountry, ReceivingCountry, TransferType, Agent, Year, Month, Day).Data.ToPagedList(pageNumber, pageSize);
            return View(vm);
        }

        public JsonResult GetAgentByCountry(string Country)
        {
            CommonServices _CommonServices = new CommonServices();
            var data = _CommonServices.GetAgents().Where(x => x.Country == Country).ToList();
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
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRates(string sendingCurency = "", string receivingCurrency = "", string sendingCountry = "", string receivingCounrty = "",
            int transferType = 0, int method = 0, int agent = 0, string range = "")
        {
            var ExchangeRate = _transferExchangeRate.GetRate(sendingCurency, receivingCurrency, sendingCountry, receivingCounrty, transferType, method, agent, range);
            return Json(new
            {
                Rate = ExchangeRate
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
            var Agent = _commonServices.GetAgent();
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");
        }
        public void SetCurrencyViewBag()
        {
            var Currency = _commonServices.GetCountryCurrencies();
            ViewBag.SendingCurrency = new SelectList(Currency, "Code", "Name");
            ViewBag.ReceivingCurrency = new SelectList(Currency, "Code", "Name");
        }
    }
}