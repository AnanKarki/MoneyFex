using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessAddMoneyToWalletController : Controller
    {
        KiiPayBusinessAddMoneyToWalletServices _kiiPayBusinessAddMoneyToWalletServices = null;
        public KiiPayBusinessAddMoneyToWalletController()
        {
            _kiiPayBusinessAddMoneyToWalletServices = new KiiPayBusinessAddMoneyToWalletServices();
        }

        // GET: KiiPayBusiness/KiiPayBusinessAddMoneyToWallet
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddMoneyToWalletEnterAmount()
        {

            var vm = _kiiPayBusinessAddMoneyToWalletServices.GetKiiPayBusinessAddMoneyToWalletEnterAmount();
            vm.CurrencySymbol = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CurrencySymbol;
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddMoneyToWalletEnterAmount([Bind(Include = AddMoneyToWalletEnterAmountVm.BindProperty)]AddMoneyToWalletEnterAmountVm vm)
        {
            if (ModelState.IsValid)
            {
                _kiiPayBusinessAddMoneyToWalletServices.SetKiiPayBusinessAddMoneyToWalletEnterAmount(vm);
                return RedirectToAction("AddMoneyToWalletChooseCard" , new { vm.Amount});
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddMoneyToWalletChooseCard(decimal Amount)
        {

          
            KiiPayBusinessSavedDebitCreditCard vm = new KiiPayBusinessSavedDebitCreditCard();
            vm.DepositingAmount = Amount;
            vm.KiiPayBusinessSavedDebitCreditCardVM = _kiiPayBusinessAddMoneyToWalletServices.GetSavedDebitCreditCardDetails();
            return View(vm);
        }


        [HttpPost]
        public ActionResult AddMoneyToWalletChooseCard([Bind(Include = KiiPayBusinessSavedDebitCreditCard.BindProperty)]KiiPayBusinessSavedDebitCreditCard vm)
        {
            SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
            if (ModelState.IsValid)
            {
                var CreditCardInfo = _saveCreditDebitCard.GetCardInfo(vm.KiiPayBusinessSavedDebitCreditCardVM.Where(x => x.IsChecked == true).FirstOrDefault().CardId);
                StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
                {
                    CardName = "",
                    ExpirationMonth = CreditCardInfo.EMonth.Decrypt(),
                    ExpiringYear = CreditCardInfo.EYear.Decrypt(),
                    Number = CreditCardInfo.Num.Decrypt(),
                    SecurityCode = CreditCardInfo.ClientCode.Decrypt(),
                    
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

                        StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
                        {
                            Amount = Common.BusinessSession.KiiPayBusinessAddMoneyToWalletEnterAmount.Amount,
                             Currency = Common.Common.GetCountryCurrency( Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode),
                             NameOnCard = "Charge for " + CreditCardInfo.CardName,
                             StripeTokenId = StripeResult.StripeTokenId,
                             CardNum = stripeResultIsValidCardVm.Number,
                            ExipiryDate = stripeResultIsValidCardVm.ExpirationMonth + "/"  + stripeResultIsValidCardVm.ExpiringYear,
                            SecurityCode = stripeResultIsValidCardVm.SecurityCode

                        };

                       var transactionResult= StripServices.CreateTransaction(stripeCreateTransaction);

                        if (transactionResult.IsValid == false)
                        {
                            ModelState.AddModelError("StripeError", transactionResult.Message);
                            return View(vm);

                        }
                        if (transactionResult!=null)
                        {
                            _kiiPayBusinessAddMoneyToWalletServices.AddMoneyToKiiPayBusinessWallet(stripeCreateTransaction);
                            return RedirectToAction("AddMoneyToWalletSuccess");
                        }
                        else
                        {
                            ModelState.AddModelError("TransactionError", transactionResult.Message);
                        }
                        return View(vm);
                    }
                }
                return View(vm);
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddMoneyToWalletSuccess()
        {
            
            var vm = _kiiPayBusinessAddMoneyToWalletServices.GetKiiPayBusinessAddMoneyToWalletEnterAmount();
            vm.CurrencySymbol = Common.Common.GetCurrencySymbol(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode);

            return View(vm);
        }
        [HttpGet]
        public ActionResult AddMoneyToWalletEnterCardInformation()
        {
            AddMoneyToWalletEnterCardInfoVm vm = new AddMoneyToWalletEnterCardInfoVm();
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddMoneyToWalletEnterCardInformation([Bind(Include = AddMoneyToWalletEnterCardInfoVm.BindProperty)]AddMoneyToWalletEnterCardInfoVm vm)
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

                    if (vm.SaveCard == true)
                    {
                        bool SaveSuccess = _saveCreditDebitCard.SaveCreditDebitCard(vm);
                    }

                    StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
                    {
                        Amount = Common.BusinessSession.KiiPayBusinessAddMoneyToWalletEnterAmount.Amount,
                        Currency = Common.Common.GetCountryCurrency(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode),
                        NameOnCard = "Charge for " + stripeResultIsValidCardVm.CardName,
                        StripeTokenId = StripeResult.StripeTokenId,
                        CardNum = stripeResultIsValidCardVm.Number,
                        ExipiryDate = stripeResultIsValidCardVm.ExpirationMonth + "/" + stripeResultIsValidCardVm.ExpiringYear,
                        SecurityCode = stripeResultIsValidCardVm.SecurityCode

                    };

                    var transactionResult = StripServices.CreateTransaction(stripeCreateTransaction);

                    if (transactionResult.IsValid == false)
                    {
                        ModelState.AddModelError("StripeError", transactionResult.Message);
                        return View(vm);

                    }
                    if (transactionResult != null)
                    {
                        _kiiPayBusinessAddMoneyToWalletServices.AddMoneyToKiiPayBusinessWallet(stripeCreateTransaction);
                        return RedirectToAction("AddMoneyToWalletSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("TransactionError", transactionResult.Message);
                    }
                    return View(vm);
                }
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddMoneyToWalletAddNewCard()
        {
            AddMoneyToWalletEnterCardInfoVm vm = new AddMoneyToWalletEnterCardInfoVm();

            // Session.Remove("KiiPayBusinessAddMoneyToWalletEnterAmount");
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddMoneyToWalletAddNewCard([Bind(Include = AddMoneyToWalletEnterCardInfoVm.BindProperty)]AddMoneyToWalletEnterCardInfoVm vm)
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
                    return RedirectToAction("AddMoneyToWalletChooseCard");
                }
            }
            return View(vm);
        }
    }
}