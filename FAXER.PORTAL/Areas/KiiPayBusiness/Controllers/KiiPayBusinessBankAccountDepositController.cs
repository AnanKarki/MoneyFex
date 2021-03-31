using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessBankAccountDepositController : Controller
    {
        KiiPayBusinessBankAccountDepositServices _kiiPayBusinessBankAccountDepositServices = null;
        KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = null;
        public KiiPayBusinessBankAccountDepositController()
        {
            _kiiPayBusinessBankAccountDepositServices = new KiiPayBusinessBankAccountDepositServices();
            _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
        }
        // GET: KiiPayBusiness/KiiPayBusinessBankAccountDeposit
        public ActionResult Index()
        {
            return View();
        }


        #region Local Transaction 

        [HttpGet]
        public ActionResult LocalBankAccountDeposit() {

            GetAllDropDownList();
            return View();
        }
        [HttpPost]
        public ActionResult LocalBankAccountDeposit([Bind(Include = BankAccountDepositVM.BindProperty)]BankAccountDepositVM vm) {

            GetAllDropDownList();

            if (ModelState.IsValid) {

                return RedirectToAction("LocalBankDepositAmount");
            }
            return View(vm);
        }
        

      

        [HttpGet]
        public ActionResult LocalBankDepositAmount() {
            LocalBankDepositAmountVM vm = new LocalBankDepositAmountVM();
            return View(vm);
        }

        [HttpPost]
        public ActionResult LocalBankDepositAmount([Bind(Include = LocalBankDepositAmountVM.BindProperty)]LocalBankDepositAmountVM vm)
        {

            if (ModelState.IsValid)
            { 
                return RedirectToAction("LocalBankDepositSummary");

            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult LocalBankDepositSummary()
        {

            BankDepositSummaryVM vm = new BankDepositSummaryVM();
            return View(vm);

        }

        public ActionResult CompleteLocalBankDeposit()
        {

            BankDepositCompletedSummaryVM vm = new BankDepositCompletedSummaryVM();
            return View(vm);

        }

        #endregion


        #region International Transaction 

        [HttpGet]
        public ActionResult InternationalBankAccountDeposit()
        {

            GetAllDropDownList();
            GetCountryDropDownList();
            return View();
        }
        [HttpPost]
        public ActionResult InternationalBankAccountDeposit([Bind(Include = BankAccountDepositInternationalVM.BindProperty)]BankAccountDepositInternationalVM vm)
        {

            GetAllDropDownList();
            GetCountryDropDownList();
            if (ModelState.IsValid)
            {

                return RedirectToAction("InternationalBankDepositAmount");
            }
            return View(vm);


        }


        [HttpGet]
        public ActionResult InternationalBankDepositAmount()
        {
            KiiPayBusinessInternationalPaymentPayingAmountDetailsVM vm = new KiiPayBusinessInternationalPaymentPayingAmountDetailsVM();
            return View(vm);
        }

        [HttpPost]
        public ActionResult InternationalBankDepositAmount([Bind(Include = KiiPayBusinessInternationalPaymentPayingAmountDetailsVM.BindProperty)]KiiPayBusinessInternationalPaymentPayingAmountDetailsVM vm)
        {

            if (ModelState.IsValid)
            {
                return RedirectToAction("InternationalBankDepositSummary");
            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult InternationalBankDepositSummary()
        {

            BankDepositSummaryVM vm = new BankDepositSummaryVM();
            return View(vm);

        }

        public ActionResult CompleteInternationalBankDeposit()
        {

            BankDepositCompletedSummaryVM vm = new BankDepositCompletedSummaryVM();
            return View(vm);

        }
        #endregion
        public void GetAllDropDownList()
        {
            ViewBag.RecentAccountNumbers = new SelectList(_kiiPayBusinessBankAccountDepositServices.GetRecentAccountsPaid(),
                                            "AccountNo", "AccountNo");
            ViewBag.Banks = new SelectList(_kiiPayBusinessBankAccountDepositServices.GetBanks(), "Id", "Name");
            ViewBag.Branches = new SelectList(_kiiPayBusinessBankAccountDepositServices.GetBankBranches(), "Id", "Name");
        }

        public void GetCountryDropDownList() {

            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown(), "CountryCode", "CountryName");

        }

    }
}