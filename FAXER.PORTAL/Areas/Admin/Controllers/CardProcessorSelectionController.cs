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
    public class CardProcessorSelectionController : Controller
    {
        CardProcessorManager _cardProcessorManager = null;
        CommonServices _commonServices = null;
        public CardProcessorSelectionController()
        {
            _cardProcessorManager = new CardProcessorManager();
            _commonServices = new CommonServices();
        }
        // GET: Admin/CardProcessorSelection
        public ActionResult Index(int transfertype = 0, int transferMethod = 0, string sendingCountry = "", string receivingCountry = "", int page = 1, int PageSize = 10)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountyViewBag();
            IPagedList<CardProcessorSelectionViewModel> vm = _cardProcessorManager.GetCardProcessorSelections(transfertype,
                 transferMethod, sendingCountry, receivingCountry).ToPagedList(page, PageSize);
            ViewBag.TransferType = transfertype;
            ViewBag.TransferMethod = transferMethod;
            ViewBag.PageSize = PageSize;
            return View(vm);

        }
        public ActionResult SetCardProcessorSelection(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCurrencyViewBag();
            SetCountyViewBag();

            CardProcessorSelectionViewModel vm = new CardProcessorSelectionViewModel();
            if (id > 0)
            {
                vm = _cardProcessorManager.GetCardProcessorSelectionById(id);

            }
            SetCardProcessorViewBag(vm.SendingCountry, vm.SendingCurrency);
            return View(vm);
        }
        [HttpPost]
        public ActionResult SetCardProcessorSelection([Bind(Include = CardProcessorSelectionViewModel.BindProperty)] CardProcessorSelectionViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCurrencyViewBag();
            SetCountyViewBag(vm.SendingCurrency, vm.ReceivingCurrency);
            SetCardProcessorViewBag(vm.SendingCountry, vm.SendingCurrency);
            if (ModelState.IsValid)
            {
                if (vm.TransferType == DB.TransactionTransferType.Select)
                {
                    ModelState.AddModelError("TransferType", "Select Transfer Type");
                    return View(vm);
                }
                if (vm.TransferMethod == DB.TransactionTransferMethod.Select)
                {
                    ModelState.AddModelError("TransferMethod", "Select Transfer Method");
                    return View(vm);
                }
                vm.CreatedBy = Common.StaffSession.LoggedStaff.StaffId;
                vm.CreatedDate = DateTime.Now;
                var cardProcessorSelection = _cardProcessorManager.BindViewModelIntoCardProcessorSelectionModel(vm);
                if (vm.Id > 0)
                {
                    _cardProcessorManager.UpdateCardProcessorSelection(cardProcessorSelection);
                }
                else
                {
                    _cardProcessorManager.AddCardProcessorSelection(cardProcessorSelection);
                }
                return RedirectToAction("Index", "CardProcessorSelection");
            }
            return View(vm);
        }
        [HttpGet]
        public JsonResult DeleteCardProcessorSelection(int id)
        {
            if (id > 0)
            {
                _cardProcessorManager.DeleteCardProcessorSelection(id);
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
        public void SetCountyViewBag(string sendingCurrency = "", string receivingCurrency = "")
        {
            var sendingCountries = new List<Services.DropDownViewModel>();
            sendingCountries.Add(new Services.DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            var receivingCountries = new List<Services.DropDownViewModel>();
            receivingCountries.Add(new Services.DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });

            sendingCountries.AddRange(GetCountriesOfCurrency(sendingCurrency));
            receivingCountries.AddRange(GetCountriesOfCurrency(sendingCurrency));
            ViewBag.SendingCountries = new SelectList(sendingCountries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(receivingCountries, "Code", "Name");

        }
        private List<Services.DropDownViewModel> GetCountriesOfCurrency(string currency = "")
        {
            var countries = _commonServices.GetCountries().ToList();

            if (!string.IsNullOrEmpty(currency))
            {

                countries = countries.Where(x => x.CountryCurrency == currency.Trim()).ToList();
            }
            return countries;
        }
        public void SetCurrencyViewBag()
        {
            var currencies = _commonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(currencies, "Code", "Name");
            currencies.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            ViewBag.ReceivingCurrencies = new SelectList(currencies, "Code", "Name");
        }
        public void SetCardProcessorViewBag(string country = "", string currency = "")
        {
            var cardProccessors = _cardProcessorManager.GetCardProcessDropdown(country, currency);
            ViewBag.CardProccessor = new SelectList(cardProccessors, "Id", "Name");
        }

        public JsonResult GetCardProcessorByCountry(string country = "", string currency = "")
        {
            var Data = _cardProcessorManager.GetCardProcessDropdown(country, currency);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
    }
}