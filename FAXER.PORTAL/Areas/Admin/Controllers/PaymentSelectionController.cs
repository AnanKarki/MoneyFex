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
    public class PaymentSelectionController : Controller
    {
        PaymentSelectionServices _services = null;
        CommonServices _CommonServices = null;
        public PaymentSelectionController()
        {
            _services = new PaymentSelectionServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/PaymentSelection
        public ActionResult Index(string SendingCountry = "", string ReceivingCountry = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<PaymentSelectionViewModel> model = _services.GetPaymentSelectionList(SendingCountry, ReceivingCountry).ToPagedList(pageNumber, pageSize); ;
            return View(model);
        }
        [HttpGet]
        public JsonResult Delete(int id = 0)
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
        public ActionResult AddPaymentSelection(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");
            PaymentSelectionViewModel vm = new PaymentSelectionViewModel();
            if (id > 0)
            {
                vm = _services.PaymentSelection(id);
                return View(vm);
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddPaymentSelection([Bind(Include = PaymentSelectionViewModel.BindProperty)]PaymentSelectionViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");
            if (ModelState.IsValid)
            {
                if (vm.Id == 0)
                {
                    _services.AddPaymentSelection(vm);
                }
                else
                {
                    _services.UpdatePaymentSelection(vm);

                }
                return RedirectToAction("Index", "PaymentSelection");

            }
            return View();
        }


    }
}