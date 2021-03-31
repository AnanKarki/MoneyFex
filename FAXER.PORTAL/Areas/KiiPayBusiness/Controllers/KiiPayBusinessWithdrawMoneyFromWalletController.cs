using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessWithdrawMoneyFromWalletController : Controller
    {

        KiiPayBusinessWithdrawMoneyFromWalletServices _kiiPayBusinessWithdrawMoneyFromWalletServices = null;
        public KiiPayBusinessWithdrawMoneyFromWalletController()
        {
            _kiiPayBusinessWithdrawMoneyFromWalletServices = new KiiPayBusinessWithdrawMoneyFromWalletServices();
        }
        // GET: KiiPayBusiness/KiiPayBusinessWithdrawMoneyFromWallet
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult WithdrawMoneyFromWalletChooseBanckAccount()
        {

            var vm = _kiiPayBusinessWithdrawMoneyFromWalletServices.GetSavedBankAccount();
            return View(vm);
        }

        [HttpPost]
        public ActionResult WithdrawMoneyFromWalletChooseBanckAccount([Bind(Include = KiiPayBusinessUserBanckAccountsVM.BindProperty)]KiiPayBusinessUserBanckAccountsVM vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("WithdrawMoneyFromWalletEnterAmount");
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult WithdrawMoneyFromWalletEnterAmount()
        {
            WithdrawMoneyFromWalletEnterAmountVM vm = new WithdrawMoneyFromWalletEnterAmountVM();
            return View(vm);
        }

        [HttpPost]
        public ActionResult WithdrawMoneyFromWalletEnterAmount([Bind(Include = WithdrawMoneyFromWalletEnterAmountVM.BindProperty)]WithdrawMoneyFromWalletEnterAmountVM vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("WithdrawMoneyFromWalletSummary");
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult WithdrawMoneyFromWalletSummary()
        {
            var vm = _kiiPayBusinessWithdrawMoneyFromWalletServices.GetWithdrawMoneySummary();
            return View(vm);
        }
        [HttpPost]
        public ActionResult WithdrawMoneyFromWalletSummary([Bind(Include = WithdrawMoneyFromWalletSummaryVM.BindProperty)]WithdrawMoneyFromWalletSummaryVM vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("WithdrawMoneyFromWalletSuccess");
            }
            return View(vm);

        }

        [HttpGet]
        public ActionResult WithdrawMoneyFromWalletSuccess()
        {
            var vm = _kiiPayBusinessWithdrawMoneyFromWalletServices.GetWithdrawMoneySuccess();
            return View(vm);
        }

        [HttpGet]
        public ActionResult WithdrawMoneyFromWalletAddBankAccount()
        {
            GetCountryDropDown();
            WithdrawMoneyFromWalletAddBankAccountVM vm= new WithdrawMoneyFromWalletAddBankAccountVM();
            return View(vm);
        }
        [HttpPost]
        public ActionResult WithdrawMoneyFromWalletAddBankAccount([Bind(Include = WithdrawMoneyFromWalletAddBankAccountVM.BindProperty)]WithdrawMoneyFromWalletAddBankAccountVM vm)
        {
            GetCountryDropDown();
            if (ModelState.IsValid)
            {
                return RedirectToAction("WithdrawMoneyFromWalletChooseBanckAccount");
            }
            return View(vm);
        }

        private void GetCountryDropDown()
        {
            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown()
               , "CountryCode", "CountryName");
        }

    }
}