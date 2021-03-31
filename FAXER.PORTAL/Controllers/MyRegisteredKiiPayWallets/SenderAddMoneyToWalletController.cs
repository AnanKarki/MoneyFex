using FAXER.PORTAL.Areas.Agent.Controllers;
using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.MyRegisteredKiiPayWallets
{
    public class SenderAddMoneyToWalletController : Controller
    {
        private SSenderAddMoneyToWalletService _SenderAddMoneyToWalletServices = null;
        private Common.SenderCommonFunc _senderCommonFunc = null;

        public SenderAddMoneyToWalletController()
        {
            _SenderAddMoneyToWalletServices = new SSenderAddMoneyToWalletService();
            _senderCommonFunc = new Common.SenderCommonFunc();
        }
        // GET:  
        [HttpGet]
        public ActionResult SenderAddMoneyToWallet()
        {
            var vm = _SenderAddMoneyToWalletServices.GetSenderAddMoneyToWalletEnterAmount();
            var loggedUser = Common.FaxerSession.LoggedUser;
            vm.CurrencyCode = Common.Common.GetCountryCurrency(loggedUser.CountryCode);
            //
            var senderWalletInfo = _senderCommonFunc.GetSenderKiiPayWalletInfo(loggedUser.Id);
            vm.AvailableBalance = _senderCommonFunc.GetCurrentKiiPayWalletBal(senderWalletInfo.Id);
            vm.CurrencySymbol = Common.Common.GetCurrencySymbol(loggedUser.CountryCode);
            return View(vm);

        }
        [HttpPost]
        public ActionResult SenderAddMoneyToWallet([Bind(Include = SenderAddMoneyToWalletViewModel.BindProperty)]SenderAddMoneyToWalletViewModel Vm)
        {
            if (ModelState.IsValid)
            {
                _SenderAddMoneyToWalletServices.SetSenderAddMoneyToWalletEnterAmount(Vm);

                return RedirectToAction("SenderAddMoneyToWalletSavedCard", new { Vm.Amount });
            }
            return View(Vm);
        }
        [HttpGet]
        public ActionResult SenderAddMoneyToWalletSavedCard(decimal Amount)
        {
            SenderAddMoneyToWalletSavedCardViewModel Vm = new SenderAddMoneyToWalletSavedCardViewModel();
            Vm.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(Common.FaxerSession.LoggedUser.CountryCode);
            Vm.Amount = Amount;

            var loggedUser = Common.FaxerSession.LoggedUser;
            var senderWalletInfo = _senderCommonFunc.GetSenderKiiPayWalletInfo(loggedUser.Id);
            Vm.AvailableBalance = _senderCommonFunc.GetCurrentKiiPayWalletBal(senderWalletInfo.Id);
            Vm.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(loggedUser.CountryCode);
            Vm.CardDetails = _SenderAddMoneyToWalletServices.GetSavedDebitCreditCardDetails();
            return View(Vm);


        }

        [HttpPost]
        public ActionResult SenderAddMoneyToWalletSavedCard([Bind(Include = SenderAddMoneyToWalletSavedCardViewModel.BindProperty)]SenderAddMoneyToWalletSavedCardViewModel Vm)
        {
            if (ModelState.IsValid && Vm.CardDetails.Count > 0)
            {
                int selectedCardId = 0;

                foreach (var item in Vm.CardDetails)
                {
                    if (item.IsChecked == true)
                    {
                        selectedCardId = item.CardId;
                    }
                }
                //Set this card id in session and use this card id to fetch all the data with this card and use card use name and others
                _SenderAddMoneyToWalletServices.SetCardId(Vm.CardDetails.Where(x => x.IsChecked == true).FirstOrDefault().CardId);


                SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();

                var cardInfo = _saveCreditDebitCard.GetCardInfo(selectedCardId);
                StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
                {
                    CardName = Common.FaxerSession.LoggedUser.FullName,
                    ExpirationMonth = cardInfo.EMonth.Decrypt(),
                    ExpiringYear = cardInfo.EYear.Decrypt(),
                    Number = cardInfo.Num.Decrypt(),
                    SecurityCode = cardInfo.ClientCode.Decrypt(),

                };
                var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);
                if (ModelState.IsValid)
                {


                    if (!StripeResult.IsValid)
                    {

                        ModelState.AddModelError("TransactionError", StripeResult.Message);

                    }

                    else
                    {

                        StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
                        {
                            Amount = Vm.Amount,
                            Currency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode),
                            NameOnCard = "Charge for " + stripeResultIsValidCardVm.CardName,
                            StripeTokenId = StripeResult.StripeTokenId,
                            CardNum = stripeResultIsValidCardVm.Number,
                            ReceivingCountry = Common.FaxerSession.LoggedUser.CountryCode,
                            SendingCountry = Common.FaxerSession.LoggedUser.CountryCode,
                            ExipiryDate = stripeResultIsValidCardVm.ExpirationMonth + "/" + stripeResultIsValidCardVm.ExpiringYear,
                            SecurityCode = stripeResultIsValidCardVm.SecurityCode

                        };

                        var transactionResult = StripServices.CreateTransaction(stripeCreateTransaction);

                        if (transactionResult.IsValid == false)
                        {
                            ModelState.AddModelError("StripeError", transactionResult.Message);
                            return View(Vm);

                        }
                        if (transactionResult != null)
                        {

                            // KiiPay Persoanl Wallet Services 
                            AddMoneyToWalletServices addMoneyToWalletServices = new AddMoneyToWalletServices();
                            SenderCommonFunc _senderCommonFunc = new SenderCommonFunc();
                            int WalletId = _senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id).Id;

                            addMoneyToWalletServices.AddMoneyToKiiPayBusinessWallet(stripeCreateTransaction, WalletId);
                            return RedirectToAction("SenderAddMoneyToWalletSuccess", new { Vm.Amount });
                        }
                        else
                        {
                            ModelState.AddModelError("TransactionError", transactionResult.Message);
                        }
                    }

                }

            }
            return View(Vm);
        }

        [HttpGet]
        public ActionResult SenderAddMoneyToWalletSuccess()
        {
            var data = _SenderAddMoneyToWalletServices.GetSenderAddMoneyToWalletEnterAmount();
            SenderAddMoneyToWalletSuccessViewModel result = new SenderAddMoneyToWalletSuccessViewModel()
            {
                SendToName = Common.FaxerSession.LoggedUser.FullName,
                SendingBalance = data.Amount,
                CurrencyCode = data.CurrencySymbol
            };
            return View(result);

            ///Send more Money Portion is left
        }

        [HttpGet]
        public ActionResult SenderAddMoneyCard()
        {
            var Vm = _SenderAddMoneyToWalletServices.GetSenderAddMoneyCard();
            Vm.CardHolderName = Common.FaxerSession.LoggedUser.FullName;
            Vm.SendingBalance = Common.FaxerSession.SenderAddMoneyToWallet.Amount;
            Vm.SendingCurrency = Common.FaxerSession.SenderAddMoneyToWallet.CurrencySymbol;
            ViewBag.SelectCards = new SelectList(GetSelectCards(), "Code", "Name");
            ViewBag.Addresses = new SelectList(GetAddresses(), "Code", "Name");
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            Vm.Address = senderCommonFunc.GetSenderAddress();
            return View(Vm);

        }
        [HttpPost]
        public ActionResult SenderAddMoneyCard([Bind(Include = SenderAddMoneyCardViewModel.BindProperty)]SenderAddMoneyCardViewModel vm)
        {

            ViewBag.SelectCards = new SelectList(GetSelectCards(), "Code", "Name");
            ViewBag.Addresses = new SelectList(GetAddresses(), "Code", "Name");

            SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
            var result = Common.Common.ValidationOfSenderMoneyCardCard(vm);
            if (result.Data == false)
            {
                ModelState.AddModelError("", result.Message);
                return View(vm);
            }

            //string[] splittedDate = vm.ExpiringDateYear.Split('-');
            //vm.ExpiringDateMonth = splittedDate[1];
            //vm.ExpiringDateYear = splittedDate[0];
            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                CardName = Common.FaxerSession.LoggedUser.FullName,
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

                    StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
                    {
                        Amount = vm.SendingBalance,
                        Currency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode),
                        NameOnCard = "Charge for " + stripeResultIsValidCardVm.CardName,
                        StripeTokenId = StripeResult.StripeTokenId,
                        CardNum = stripeResultIsValidCardVm.Number,
                        ReceivingCountry = Common.FaxerSession.LoggedUser.CountryCode,
                        SendingCountry = Common.FaxerSession.LoggedUser.CountryCode,
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

                        // KiiPay Persoanl Wallet Services 
                        AddMoneyToWalletServices addMoneyToWalletServices = new AddMoneyToWalletServices();
                        int WalletId = _senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id).Id;

                        addMoneyToWalletServices.AddMoneyToKiiPayBusinessWallet(stripeCreateTransaction, WalletId);


                        bool SaveSuccess = _saveCreditDebitCard.SaveSenderCreditDebitCard(vm);


                        return RedirectToAction("SenderAddMoneyToWalletSuccess");
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
        public ActionResult SenderConfirmAddCardSuccess()
        {
            return View();
        }




        public List<BankVM> GetSelectCards()
        {

            //var result = new List<BankVM>();
            //return result;

            var result = new List<BankVM>();
            var Card1 = new BankVM()
            {
                Code = "10",
                Name = "agaga"
            };
            var Card2 = new BankVM()
            {
                Code = " 22",
                Name = "fasfdsdf"
            };
            result.Add(Card1);
            result.Add(Card2);
            return result;

        }
        public List<AddressVm> GetAddresses()
        {
            //var result = new List<AddressVm>();
            //return result;
            var result = new List<AddressVm>();
            var Card1 = new AddressVm()
            {
                Code = "10",
                Name = "agaga"
            };
            var Card2 = new AddressVm()
            {
                Code = " 22",
                Name = "fasfdsdf"
            };
            result.Add(Card1);
            result.Add(Card2);
            return result;

        }


    }
}