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
    public class CustomerPaymentFeeController : Controller
    {
        CustomerPaymentFeeServices _services = null;
        CommonServices _CommonServices = null;
        public CustomerPaymentFeeController()
        {
            _services = new CustomerPaymentFeeServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/CustomerPaymentFee
        public ActionResult Index(string Country = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            PagedList.IPagedList<CustomerPaymentFeeViewModel> vm = _services.GetCoustmerPaymentFeeList(Country).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }

        [HttpGet]
        public JsonResult Delete(int id = 0)
        {
            _services.DeleteCustomerPaymentFee(id);
            return Json(new { Data = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SetNewPayment(int id = 0, string Country = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            CustomerPaymentFeeViewModel vm = new CustomerPaymentFeeViewModel();
            if (id != 0 || !string.IsNullOrEmpty(Country))
            {
                vm = _services.GetCoustmerPaymentFee(id, Country);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult SetNewPayment([Bind(Include = CustomerPaymentFeeViewModel.BindProperty)] CustomerPaymentFeeViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            if (ModelState.IsValid)
            {
                if (vm.Id == null)
                {
                    _services.Add(vm);
                }
                else
                {
                    _services.Update(vm);
                }
                return RedirectToAction("Index", "CustomerPaymentFee");
            }
            return View(vm);
        }
    }
}