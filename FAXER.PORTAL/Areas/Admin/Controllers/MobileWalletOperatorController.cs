using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class MobileWalletOperatorController : Controller
    {
        MobileWalletOperatorServices _services = null;
        CommonServices _CommonServices = null;
        public MobileWalletOperatorController()
        {

            _services = new MobileWalletOperatorServices();
            _CommonServices = new CommonServices();


        }
        // GET: Admin/MobileWalletOperator
        public ActionResult Index(string country = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            List<MobileWalletOperatorViewModel> vm = _services.GetMobileWalletOperatorList(country);
            return View(vm);
        }
        public ActionResult DeleteMobileWalletOperator(int id)
        {
            _services.DeleteMobileWalletOperator(id);
            return RedirectToAction("Index", "MobileWalletOperator");
        }
        public ActionResult AddMobileWalletOperator(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            if (id != 0)
            {
                MobileWalletOperatorViewModel vm = _services.GetMobileWalletOperator(id);
                return View(vm);
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddMobileWalletOperator([Bind(Include = MobileWalletOperatorViewModel.BindProperty)] MobileWalletOperatorViewModel vm)
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
                return RedirectToAction("Index", "MobileWalletOperator");
            }
            return View(vm);
        }
    }
}