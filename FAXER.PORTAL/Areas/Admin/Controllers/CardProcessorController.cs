using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using Microsoft.Office.Interop.Excel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class CardProcessorController : Controller
    {
        CardProcessorManager _cardProcessorManager = null;
        CommonServices _commonServices = null;
        public CardProcessorController()
        {
            _cardProcessorManager = new CardProcessorManager();
            _commonServices = new CommonServices();
        }
        // GET: Admin/CardProcessor
        public ActionResult Index(int transfertype = 0, int transferMethod = 0, string country = "", int page = 1, int PageSize = 10)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountyViewBag(country);
            IPagedList<CardProcessorViewModel> vm = _cardProcessorManager.GetCardProcessors(transfertype, transferMethod, country).ToPagedList(page, PageSize);
            ViewBag.TransferType = transfertype;
            ViewBag.TransferMethod = transferMethod;
            ViewBag.PageSize = PageSize;
            return View(vm);
        }

        public ActionResult AddCardProcessor(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountyViewBag();
            CardProcessorViewModel vm = new CardProcessorViewModel();
            if (id > 0)
            {
                vm = _cardProcessorManager.GetCardProcessorById(id);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult AddCardProcessor([Bind(Include = CardProcessorViewModel.BindProperty)] CardProcessorViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountyViewBag(vm.Country);
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
                if (vm.CardProcessorApi == DB.CardProcessorApi.Select)
                {
                    ModelState.AddModelError("CardProcessorApi", "Select Card Processor");
                    return View(vm);
                }
                vm.CreatedBy = Common.StaffSession.LoggedStaff.StaffId;
                vm.CreatedDate = DateTime.Now;
                var cardProcessor = _cardProcessorManager.BindViewModelIntoCardProcessorModel(vm);
                if (vm.Id > 0)
                {
                    _cardProcessorManager.UpdateCardProcessor(cardProcessor);
                }
                else
                {
                    _cardProcessorManager.AddCardProcessor(cardProcessor);
                }
                return RedirectToAction("Index", "CardProcessor");
            }
            return View(vm);
        }
        [HttpGet]
        public JsonResult DeleteCardProcessor(int id)
        {
            if (id > 0)
            {
                _cardProcessorManager.DeleteCardProcessor(id);
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
        public void SetCountyViewBag(string country = "")
        {
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name", country);

        }

    }
}