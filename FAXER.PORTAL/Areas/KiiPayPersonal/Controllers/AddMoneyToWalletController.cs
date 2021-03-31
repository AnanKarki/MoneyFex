using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Controllers
{
    public class AddMoneyToWalletController : Controller
    {
        private AddMoneyToWalletServices _services = null;
        public AddMoneyToWalletController()
        {
            _services = new AddMoneyToWalletServices();
        }
        // GET: KiiPayPersonal/AddMoneyToWallet
        [HttpGet]
        public ActionResult Index()
        {
            var vm = _services.GetKiiPayPersonalAddMoneyToWalletEnterAmount();
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = AddMoneyToWalletEnterAmountVM.BindProperty)] AddMoneyToWalletEnterAmountVM vm)
        {
            if (ModelState.IsValid)
            {
                _services.SetKiiPayPersonalAddMoneyToWalletEnterAmount(vm);
                return RedirectToAction("AddMoneyToWalletChooseCard", new { vm.Amount });
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddMoneyToWalletChooseCard(decimal Amount)
        {
            KiiPayPersonalSavedDebitCreditCard vm = new KiiPayPersonalSavedDebitCreditCard();
            vm.DepositingAmount = Amount;
            vm.KiiPayPersonalSavedDebitCreditCardVM = _services.GetSavedDebitCreditCardDetails();
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddMoneyToWalletChooseCard([Bind(Include = KiiPayPersonalSavedDebitCreditCard.BindProperty)] KiiPayPersonalSavedDebitCreditCard vm)
        {
            SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
            if (ModelState.IsValid)
            {
                var CreditCardInfo = _saveCreditDebitCard.GetCardInfo(vm.KiiPayPersonalSavedDebitCreditCardVM.Where(x => x.IsChecked == true).FirstOrDefault().CardId);
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
                            Amount = Common.CardUserSession.KiiPayPersonalAddMoneyToWalletEnterAmount.Amount,
                            Currency = Common.Common.GetCountryCurrency(Common.CardUserSession.LoggedCardUserViewModel.CountryCode),
                            NameOnCard = "Charge for " + CreditCardInfo.CardName,
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
                            _services.AddMoneyToKiiPayBusinessWallet(stripeCreateTransaction , Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId);
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

            var vm = _services.GetKiiPayPersonalAddMoneyToWalletEnterAmount();
            vm.CurrencySymbol = Common.Common.GetCurrencySymbol(Common.CardUserSession.LoggedCardUserViewModel.CountryCode);
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddMoneyToWalletEnterCardInformation()
        {
            AddMoneyToWalletEnterCardInfoVM vm = new AddMoneyToWalletEnterCardInfoVM();
            vm.DepositingAmount = _services.GetKiiPayPersonalAddMoneyToWalletEnterAmount().Amount;
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddMoneyToWalletEnterCardInformation([Bind(Include = AddMoneyToWalletEnterCardInfoVM.BindProperty)] AddMoneyToWalletEnterCardInfoVM vm)
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
                        Amount = Common.CardUserSession.KiiPayPersonalAddMoneyToWalletEnterAmount.Amount,
                        Currency = Common.Common.GetCountryCurrency(Common.CardUserSession.LoggedCardUserViewModel.CountryCode),
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
                        _services.AddMoneyToKiiPayBusinessWallet(stripeCreateTransaction , Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId);
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
            AddMoneyToWalletEnterCardInfoVM vm = new AddMoneyToWalletEnterCardInfoVM();

            // Session.Remove("KiiPayBusinessAddMoneyToWalletEnterAmount");
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddMoneyToWalletAddNewCard([Bind(Include = AddMoneyToWalletEnterCardInfoVM.BindProperty)]AddMoneyToWalletEnterCardInfoVM vm)
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