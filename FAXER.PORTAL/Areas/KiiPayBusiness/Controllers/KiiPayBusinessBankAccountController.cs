using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessBankAccountController : Controller
    {
        KiiPayBusinessBankAccountServices _kiiPayBusinessBankAccountServices = null;
        public KiiPayBusinessBankAccountController()
        {
            _kiiPayBusinessBankAccountServices = new KiiPayBusinessBankAccountServices();
        }
        // GET: KiiPayBusiness/KiiPayBusinessBankAccount
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BankAccountGrid()
        {
            var vm = _kiiPayBusinessBankAccountServices.GetSavedPersonalBankAccount();
            return View(vm);
        }

        public ActionResult RemoveBank( int id)
        {
            return View();
        }
      
        [HttpGet]
        public ActionResult BankAccountAddNewBankAccount()
        {
            GetCountryDropDown();
            BankAccountAddBankAccountVM vm = new BankAccountAddBankAccountVM();
            return View(vm);
        }
        [HttpPost]
        public ActionResult BankAccountAddNewBankAccount([Bind(Include = BankAccountAddBankAccountVM.BindProperty)]BankAccountAddBankAccountVM vm)
        {
            GetCountryDropDown();
            if (ModelState.IsValid)
            {
                return RedirectToAction("BankAccountAddedSuccess");
            }
            return View(vm);
        }

        public ActionResult BankAccountAddedSuccess()
        {
            return View();
        }
        private void GetCountryDropDown()
        {
            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown()
               , "CountryCode", "CountryName");
        }
    }
}