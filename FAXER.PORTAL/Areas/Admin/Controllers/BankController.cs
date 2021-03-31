using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class BankController : Controller
    {
        BankServices _services = null;
        CommonServices _CommonServices = null;
        public BankController()
        {
            _services = new BankServices();
            _CommonServices = new CommonServices();

        }
        // GET: Admin/Bank
        public ActionResult Index(string Country = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            List<BankViewModel> vm = _services.GetBankList(Country);
            return View(vm);
        }
        public ActionResult DeleteBank(int id)
        {
            _services.DeleteBank(id);
            return RedirectToAction("Index", "Bank");
        }
        public ActionResult AddBank(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            if (id != 0)
            {
                BankViewModel vm = _services.GetBank(id);
                return View(vm);
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddBank([Bind(Include = BankViewModel.BindProperty)] BankViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            if (ModelState.IsValid)
            {
                if (vm.Id == null )
                {
                    _services.Add(vm);
                }
                else
                {
                    _services.Update(vm);

                }
                return RedirectToAction("Index", "Bank");
            }
            return View(vm);
        }
    }
}