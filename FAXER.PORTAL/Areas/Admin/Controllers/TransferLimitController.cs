using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class TransferLimitController : Controller
    {
        CommonServices _CommonServices = null;
        TransferLimitServices _services = null;
        public TransferLimitController()
        {
            _CommonServices = new CommonServices();
            _services = new TransferLimitServices();
        }

        // GET: Admin/TransferLimit
        public ActionResult Index(string Country = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            List<TransferLimitViewModel> vm = _services.GetTransferLimitList(Country);
            return View(vm);
        }
        public ActionResult DeleteTransferLimit(int id)
        {
            _services.DeleteTransferLimit(id);
            return RedirectToAction("Index", "TransferLimit");
        }
        public ActionResult AddTransferLimit(int id = 0)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            if (id != 0)
            {
                TransferLimitViewModel vm = _services.GetTransferLimit(id);
                return View(vm);
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddTransferLimit([Bind(Include = TransferLimitViewModel.BindProperty)] TransferLimitViewModel vm)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            if (ModelState.IsValid)
            {
                if (vm.Amount == 0)
                {
                    ModelState.AddModelError("Amount", "Enter Amount");
                    return View(vm);
                }
                if (vm.Id != null)
                {
                    _services.Update(vm);
                }
                else
                {
                    _services.Add(vm);

                }
                return RedirectToAction("Index", "TransferLimit");
            }

            return View(vm);
        }

    }
}