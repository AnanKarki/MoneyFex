using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class BankBranchController : Controller
    {
        CommonServices _CommonServices = null;
        BankBranchServices _services = null;
        public BankBranchController()
        {
            _CommonServices = new CommonServices();
            _services = new BankBranchServices();
        }
        // GET: Admin/BankBranch
        public ActionResult Index(int BankId = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Banks = _CommonServices.GetBanks();
            ViewBag.Banks = new SelectList(Banks, "Id", "Name");
            ViewBag.Bank = BankId;
          
            List<BankBranchViewModel> vm = _services.GetBranchbankList(BankId);
            return View(vm);
        }

        public ActionResult AddBankBranch(int id = 0, string Country = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Banks = _CommonServices.GetBanks(Country);
            ViewBag.Banks = new SelectList(Banks, "Id", "Name");

            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            BankBranchViewModel vm = new BankBranchViewModel();
            if (id != 0)
            {
                vm = _services.GetBranchbank(id);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult AddBankBranch([Bind(Include = BankBranchViewModel.BindProperty)]BankBranchViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Banks = _CommonServices.GetBanks();
            ViewBag.Banks = new SelectList(Banks, "Id", "Name");


            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            if (ModelState.IsValid)
            {
                if (vm.Id != 0)
                {
                    _services.UpdateBankBranch(vm);
                }
                else
                {
                    _services.AddBankBranch(vm);
                }
                return RedirectToAction("Index", "BankBranch");

            }
            return View(vm);
        }
        public ActionResult Delete(int id = 0)
        {
            _services.deleteBankBranch(id);
            return RedirectToAction("Index", "BankBranch");
        }




    }
}