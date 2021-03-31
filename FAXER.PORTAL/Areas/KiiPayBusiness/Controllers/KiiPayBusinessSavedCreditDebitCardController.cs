using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessSavedCreditDebitCardController : Controller
    {
        KiiPayBusinessSavedCreditDebitCardServices _kiiPayBusinessSavedCreditDebitCardServices = null;
        public KiiPayBusinessSavedCreditDebitCardController()
        {
            _kiiPayBusinessSavedCreditDebitCardServices = new KiiPayBusinessSavedCreditDebitCardServices();
        }
        // GET: KiiPayBusiness/KiiPayBusinessSavedCreditDebitCard
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ListOfSavedCards()
        {
            var vm = _kiiPayBusinessSavedCreditDebitCardServices.GetSavedDebitCreditCardDetails();
            return View(vm);
        }
        [HttpPost]
        public ActionResult DeleteCard(int Id)
        {
            bool DeleteCardSuccess = _kiiPayBusinessSavedCreditDebitCardServices.DeleteCard(Id);
            if (DeleteCardSuccess)
            {
                return RedirectToAction("ListOfSavedCards");
            }
            return RedirectToAction("ListOfSavedCards");
        }
        [HttpGet]
        public ActionResult UpdateCard(int Id)
        {

            var vm = _kiiPayBusinessSavedCreditDebitCardServices.GetCardDetail(Id);
           
            return View(vm);
        }


        [HttpPost]
        public ActionResult UpdateCard([Bind(Include = AddMoneyToWalletEnterCardInfoVm.BindProperty)]AddMoneyToWalletEnterCardInfoVm vm)
        {
            SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                CardName = "",
                ExpirationMonth = vm.ExpiringDateMonth,
                ExpiringYear = vm.ExpiringDateYear,
                Number = vm.CardNumber,
                SecurityCode = vm.SecurityCode,

            };
            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);
            if (ModelState.IsValid)
            {


                if (!StripeResult.IsValid)
                {

                    ModelState.AddModelError("StripeError", StripeResult.Message);

                }
                else
                {
                    bool SaveSuccess = _saveCreditDebitCard.UpdateCreditDebitCard(vm);
                    return RedirectToAction("ListOfSavedCards");
                }
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddNewCard()
        {
            AddMoneyToWalletEnterCardInfoVm vm = new AddMoneyToWalletEnterCardInfoVm();

            // Session.Remove("KiiPayBusinessAddMoneyToWalletEnterAmount");
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddNewCard([Bind(Include = AddMoneyToWalletEnterCardInfoVm.BindProperty)]AddMoneyToWalletEnterCardInfoVm vm)
        {
            SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                CardName = "",
                ExpirationMonth = vm.ExpiringDateMonth,
                ExpiringYear = vm.ExpiringDateYear,
                Number = vm.CardNumber,
                SecurityCode = vm.SecurityCode,

            };
            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);
            if (ModelState.IsValid)
            {


                if (!StripeResult.IsValid)
                {

                    ModelState.AddModelError("StripeError", StripeResult.Message);

                }
                else
                {
                    bool SaveSuccess = _saveCreditDebitCard.SaveCreditDebitCard(vm);
                    return RedirectToAction("ListOfSavedCards");
                }
            }
            return View(vm);
        }
    }
}