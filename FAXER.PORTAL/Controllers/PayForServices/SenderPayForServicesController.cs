using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Controllers.PayForServices
{
    public class SenderPayForServicesController : Controller
    {
        SSenderPayForServices _payForSenderServices = null;
        SenderCommonFunc senderCommonFunc = null;

        public SenderPayForServicesController()
        {
            _payForSenderServices = new SSenderPayForServices();
            senderCommonFunc = new SenderCommonFunc();
        }

        #region

        // GET: SenderPayForServices
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PayForGoodsAndServices(string category, string Country = "")
        {
            GetCountriesDropDown(Country);
            GetRecentlyPaidInternationBusiness(Country);
            SenderPayForGoodsAndServicesVM vm = new SenderPayForGoodsAndServicesVM();
            vm = _payForSenderServices.GetSenderPayForGoodsAndServices();
            vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Country); 
            return View(vm);
        }


        [HttpPost]
        public ActionResult PayForGoodsAndServices([Bind(Include = SenderPayForGoodsAndServicesVM.BindProperty)] SenderPayForGoodsAndServicesVM model)
        {
            GetCountriesDropDown();
            GetRecentlyPaidInternationBusiness();

            if (ModelState.IsValid)
            {

                var result = _payForSenderServices.GetBusinessInfo(model.BusinessMobileNo);

                if (result == null)
                {

                    ModelState.AddModelError("InvalidMobileNo", "Enter valid mobile no.");
                    return View(model);
                }
                model.ReceiverId = result.Id;
                _payForSenderServices.SetSenderPayForGoodsAndServices(model);

                //SetData in EnterAmount Session
                var loggedInSenderData = Common.FaxerSession.LoggedUser;

                SenderMobileEnrterAmountVm mobileEnter = new SenderMobileEnrterAmountVm()
                {
                    ReceiverName = result.BusinessName,
                    SendingCurrencySymbol = Common.Common.GetCurrencySymbol(loggedInSenderData.CountryCode),
                    SendingCurrencyCode = Common.Common.GetCountryCurrency(loggedInSenderData.CountryCode),
                    ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(result.BusinessCountry),
                    ReceivingCurrencyCode = Common.Common.GetCountryCurrency(result.BusinessCountry),
                    ExchangeRate = Common.Common.GetExchangeRate(loggedInSenderData.CountryCode, result.BusinessCountry),
                    ReceiverId = result.Id,
                };

                _payForSenderServices.SetEnterAmount(mobileEnter);


                return RedirectToAction("PayForServicesEnterAmount", "SenderPayForServices");
            }

            return View();
        }


        public ActionResult PayForServicesEnterAmount()
        {

            var loggedInSenderData = Common.FaxerSession.LoggedUser;

            var vm = _payForSenderServices.GetEnterAmount();


            return View(vm);
        }

        [HttpPost]
        public ActionResult PayForServicesEnterAmount([Bind(Include = SenderMobileEnrterAmountVm.BindProperty)] SenderMobileEnrterAmountVm vm)
        {
            if (ModelState.IsValid)
            {
                var paymentSummary = _payForSenderServices.GetEnterAmount();
                paymentSummary.PaymentReference = vm.PaymentReference;
                _payForSenderServices.SetEnterAmount(paymentSummary);
                return RedirectToAction("PayForServicesAbroadSummary", "SenderPayForServices");
            }
            return View(vm);
        }


        public ActionResult PayForServicesAbroadSummary()
        {

            var enterAmount = _payForSenderServices.GetEnterAmount();
            var payforGoodsAndServicesData = _payForSenderServices.GetSenderPayForGoodsAndServices();

            SenderTransferSummaryVm vm = new SenderTransferSummaryVm()
            {
                Amount = enterAmount.SendingAmount,
                Fee = enterAmount.Fee,
                ReceivedAmount = enterAmount.ReceivingAmount,
                SendingCurrencySymbol = enterAmount.SendingCurrencySymbol,
                SendingCurrencyCode = enterAmount.SendingCurrencyCode,
                ReceivingCurrencyCode = enterAmount.ReceivingCurrencyCode,
                ReceivingCurrencySymbol = enterAmount.ReceivingCurrencySymbol,
                PaidAmount = enterAmount.TotalAmount,
                PaymentReference = enterAmount.PaymentReference
            };


            return View(vm);
        }

        [HttpPost]

        public ActionResult PayForServicesAbroadSummary([Bind(Include = SenderTransferSummaryVm.BindProperty)]SenderTransferSummaryVm model)
        {
            return RedirectToAction("InternationalPayment", "SenderPayForServices");
        }


        public ActionResult InternationalPayment()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();

            var vm = _payForSenderServices.GetPaymentMethod();
            var paidAmount = _payForSenderServices.GetEnterAmount();
            vm.TotalAmount = paidAmount.TotalAmount;
            vm.SendingCurrencySymbol = paidAmount.SendingCurrencySymbol;
            var senderwalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
            if (senderwalletInfo != null)
            {

                vm.KiipayWalletBalance = senderwalletInfo.CurrentBalance;

            }

            SetTransactionSummary();
            vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails();
            return View(vm);
        }


        [HttpPost]
        public ActionResult InternationalPayment([Bind(Include = PaymentMethodViewModel.BindProperty)]PaymentMethodViewModel vm)
        {
            int selectedCardId = 0;
            if (vm.CardDetails != null)
            {
                foreach (var item in vm.CardDetails)
                {
                    if (item.IsChecked == true)
                    {
                        selectedCardId = item.CardId;
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                    }
                }
            }
            //_kiiPayWalletTransferServices.SetPaymentMethod(vm);
            //_cashPickUpServices.SetPaymentMethod(vm);

            _payForSenderServices.SetPaymentMethod(vm);

            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();


            SetTransactionSummary();

            var transactionSummaryvm = _senderForAllTransferServices.GetTransactionSummary();

            switch (vm.SenderPaymentMode)
            {
                case SenderPaymentMode.SavedDebitCreditCard:
                    SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
                    var cardInfo = _saveCreditDebitCard.GetCardInfo(selectedCardId);
                    transactionSummaryvm.CreditORDebitCardDetials = new CreditDebitCardViewModel()
                    {
                        CardNumber = cardInfo.Num.Decrypt(),
                        NameOnCard = cardInfo.CardName.Decrypt(),
                        EndMM = cardInfo.EMonth.Decrypt(),
                        EndYY = cardInfo.EYear.Decrypt(),
                        SecurityCode = cardInfo.ClientCode.Decrypt()
                    };

                    var result = _senderForAllTransferServices.ValidateTransactionUsingStripe(transactionSummaryvm);
                    if (result.IsValid == false)
                    {

                        ModelState.AddModelError("TransactionError", result.Message);
                        return View(vm);
                    }

                    transactionSummaryvm.CreditORDebitCardDetials.StripeTokenID = result.StripeTokenId;
                    _senderForAllTransferServices.CompleteTransaction(transactionSummaryvm);

                    break;
                case SenderPaymentMode.CreditDebitCard:


                    return RedirectToAction("DebitCreditCardDetails");
                    break;
                case SenderPaymentMode.KiiPayWallet:
                    _senderForAllTransferServices.CompleteTransaction(transactionSummaryvm);
                    break;
                case SenderPaymentMode.MoneyFexBankAccount:

                    return RedirectToAction("MoneyFexBankDeposit");
                    break;
                default:
                    break;
            }

            return RedirectToAction("PayForServicesAbroadSuccess");
            if (vm.SenderPaymentMode == SenderPaymentMode.CreditDebitCard)
            {
                return RedirectToAction("DebitCreditCardDetails", "SenderPayForServices");
            }

            if (vm.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
            {
                return RedirectToAction("MoneyFexBankDeposit", "SenderPayForServices");
            }

            return RedirectToAction("PayForServicesAbroadSuccess", "SenderPayForServices");


            return View(vm);
        }

        private void SetTransactionSummary()
        {
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();


            // Data to show on success

            var payingAmountData = _payForSenderServices.GetEnterAmount();
            var payForServicesData = _payForSenderServices.GetSenderPayForGoodsAndServices();

            //Completing Transaction
            var loggedUserData = _payForSenderServices.GetLoggedUserData();
            var paymentMethod = _payForSenderServices.GetPaymentMethod();

            TransactionSummaryVM transactionSummaryVm = new TransactionSummaryVM();
            transactionSummaryVm.SenderAndReceiverDetail = new SenderAndReceiverDetialVM()
            {
                SenderId = loggedUserData.Id,
                SenderCountry = loggedUserData.CountryCode,
                ReceiverCountry = payForServicesData.CountryCode,
                ReceiverId = payForServicesData.ReceiverId
            };

            //Set Sms Fee 
            transactionSummaryVm.KiiPayTransferPaymentSummary = new KiiPayTransferPaymentSummary()
            {
                ReceiverName = payForServicesData.BusinessMobileNo,
                SendingCurrency = payingAmountData.SendingCurrencyCode,
                SendingAmount = payingAmountData.SendingAmount,
                ReceivingAmount = payingAmountData.ReceivingAmount,
                TotalAmount = payingAmountData.TotalAmount,
                ExchangeRate = payingAmountData.ExchangeRate,
                Fee = payingAmountData.Fee,
                PaymentReference = "",
                ReceivingCurrency = payingAmountData.ReceivingCurrencyCode,
                ReceivingCurrencySymbol = payingAmountData.ReceivingCurrencySymbol,
                SendingCurrencySymbol = payingAmountData.SendingCurrencySymbol,
                SendSMS = true,
                SMSFee = 0,
            };


            transactionSummaryVm.PaymentMethodAndAutoPaymentDetail = new PaymentMethodViewModel()
            {
                TotalAmount = payingAmountData.TotalAmount,
                SendingCurrencySymbol = payingAmountData.SendingCurrencySymbol,
                SenderPaymentMode = paymentMethod.SenderPaymentMode,
                EnableAutoPayment = false
            };

            //For DebitCreditCardDetail


            var debitCreditCardDetail = _payForSenderServices.GetDebitCreditCardDetail();

            transactionSummaryVm.CreditORDebitCardDetials = debitCreditCardDetail;



            var moneyFexBankAccountDepositData = _payForSenderServices.GetMoneyFexBankAccountDeposit();
            transactionSummaryVm.MoneyFexBankDeposit = moneyFexBankAccountDepositData;



            transactionSummaryVm.TransferType = TransferType.PayForServices;
            transactionSummaryVm.IsLocalPayment = false;

            _senderForAllTransferServices.SetTransactionSummary(transactionSummaryVm);


        }

        public ActionResult DebitCreditCardDetails()
        {
            //var addresses = _payForSenderServices.GetAddress();
            //ViewBag.Address = new SelectList(addresses, "Name", "Name");

            var payingAmountData = _payForSenderServices.GetEnterAmount();
            CreditDebitCardViewModel vm = new CreditDebitCardViewModel()
            {
                NameOnCard = Common.FaxerSession.LoggedUser.FullName,
                FaxingAmount = payingAmountData.TotalAmount,
                FaxingCurrencySymbol = payingAmountData.SendingCurrencySymbol,
                FaxingCurrency = payingAmountData.SendingCurrencyCode,
            };


            vm.AddressLineOne = senderCommonFunc.GetSenderAddress();

            return View(vm);
        }


        [HttpPost]
        public ActionResult DebitCreditCardDetails([Bind(Include = CreditDebitCardViewModel.BindProperty)]CreditDebitCardViewModel model)
        {
            //var addresses = _payForSenderServices.GetAddress();
            //ViewBag.Address = new SelectList(addresses, "Name", "Name");


            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            var transferSummary = _senderForAllTransferServices.GetTransactionSummary();
            SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
            var result = Common.Common.ValidateCreditDebitCard(model);
            if (result.Data == false)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                CardName = Common.FaxerSession.LoggedUser.FullName,
                ExpirationMonth = model.EndMM,
                ExpiringYear = model.EndYY,
                Number = model.CardNumber,
                SecurityCode = model.SecurityCode,
                billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                CurrencyCode = Common.Common.GetCurrencyCode(Common.FaxerSession.LoggedUser.CountryCode)

            };
            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);



            if (!StripeResult.IsValid)
            {

                ModelState.AddModelError("StripeError", StripeResult.Message);

            }

            else
            {

                StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
                {
                    Amount = model.FaxingAmount,
                    Currency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode),
                    NameOnCard = "Charge for " + stripeResultIsValidCardVm.CardName,
                    StripeTokenId = StripeResult.StripeTokenId,
                    CardNum = stripeResultIsValidCardVm.Number,
                    ReceivingCountry = transferSummary.SenderAndReceiverDetail.SenderCountry,
                    SendingCountry = transferSummary.SenderAndReceiverDetail.ReceiverCountry,
                    ExipiryDate = stripeResultIsValidCardVm.ExpirationMonth + "/" + stripeResultIsValidCardVm.ExpiringYear,
                    SecurityCode = stripeResultIsValidCardVm.SecurityCode,
                    billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                    billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                    ReceiptNo = Common.FaxerSession.ReceiptNo,
                    SenderId = Common.FaxerSession.LoggedUser.Id,
                };

                var transactionResult = StripServices.CreateTransaction(stripeCreateTransaction);

                if (transactionResult.IsValid == false)
                {
                    ModelState.AddModelError("StripeError", transactionResult.Message);
                    return View(model);

                }
                if (transactionResult != null)
                {

                    // KiiPay Persoanl Wallet Services 
                    transferSummary.CreditORDebitCardDetials = model;
                    _senderForAllTransferServices.CompleteTransaction(transferSummary);

                    if (transferSummary.IsLocalPayment == false)
                    {

                        return RedirectToAction("PayForServicesAbroadSuccess");
                    }
                    else
                    {

                        return RedirectToAction("LocalPayForServicesSuccess");
                    }

                }
                else
                {
                    ModelState.AddModelError("TransactionError", transactionResult.Message);
                }
                return View(model);
            }


            return View(model);
            //if (ModelState.IsValid)
            //{
            //    _payForSenderServices.SetDebitCreditCardDetail(model);

            //    return RedirectToAction("PayForServicesAbroadSuccess", "SenderPayForServices");
            //}
            //return View(model);
        }


        public ActionResult MoneyFexBankDeposit()
        {
            SenderMoneyFexBankDepositVM vm = new SenderMoneyFexBankDepositVM();

            var payingAmountData = _payForSenderServices.GetEnterAmount();

            vm.Amount = payingAmountData.SendingAmount;
            vm.SendingCurrencyCode = payingAmountData.SendingCurrencyCode;
            vm.SendingCurrencySymbol = payingAmountData.SendingCurrencySymbol;

            return View(vm);
        }

        [HttpPost]
        public ActionResult MoneyFexBankDeposit([Bind(Include = SenderMoneyFexBankDepositVM.BindProperty)]SenderMoneyFexBankDepositVM model)
        {
            if (ModelState.IsValid)
            {
                _payForSenderServices.SetMoneyFexBankAccountDeposit(model);
                return RedirectToAction("PayForServicesAbroadSuccess", "SenderPayForServices");
            }

            return View(model);
        }


        public ActionResult PayForServicesAbroadSuccess()
        {

            var payingAmountData = _payForSenderServices.GetEnterAmount();
            SenderAddMoneySuccessVM successData = new SenderAddMoneySuccessVM()
            {
                Amount = payingAmountData.SendingAmount,
                Currnecy = payingAmountData.SendingCurrencySymbol,
                ReceiverName = payingAmountData.ReceiverName,
            };
            senderCommonFunc.ClearPayForServiceSession();
            return View(successData);

        }

        #endregion


        #region Locla Pay For Services 

        [HttpGet]
        public ActionResult PayForServicesLocal()
        {
            string Country = Common.FaxerSession.LoggedUser.CountryCode;
            GetRecentlyPaidInternationBusiness(Country);
            var vm = _payForSenderServices.GetSenderPayForGoodsAndServices();
            vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Country);
            return View(vm);
        }

        [HttpPost]
        public ActionResult PayForServicesLocal([Bind(Include = SenderPayForGoodsAndServicesVM.BindProperty)]SenderPayForGoodsAndServicesVM model)
        {

            GetRecentlyPaidInternationBusiness();
            if (ModelState.IsValid)
            {

                var result = _payForSenderServices.GetBusinessInfo(model.BusinessMobileNo);

                if (result == null)
                {
                    ModelState.AddModelError("BusinessMobileNo", "Enter valid mobile no.");
                    return View(model);
                }
                if (result.BusinessCountry.ToLower() != Common.FaxerSession.LoggedUser.CountryCode.ToLower())
                {

                    ModelState.AddModelError("BusinessMobileNo", "Business is not local");
                    return View(model);
                }

                model.ReceiverName = result.BusinessName;
                model.ReceiverId = result.Id;
                _payForSenderServices.SetSenderPayForGoodsAndServices(model);


                SenderLocalEnterAmountVM senderLocalEnterAmountvm = new SenderLocalEnterAmountVM()
                {
                    CurrencySymbol = Common.Common.GetCurrencySymbol(Common.FaxerSession.LoggedUser.CountryCode),
                    CurrencyCode = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode),
                    ReceiverImage = "",
                    ReceiverName = result.BusinessName,
                    ReceiverId = result.Id
                };

                _payForSenderServices.SetLocalPayForServiceEnterAmount(senderLocalEnterAmountvm);


                return RedirectToAction("LocalPayForServicesEnterAmount", "SenderPayForServices");
            }
            return View(model);
        }


        public ActionResult LocalPayForServicesEnterAmount()
        {
            var loggedUserData = _payForSenderServices.GetLoggedUserData();

            var vm = _payForSenderServices.GetLocalPayForServiceEnterAmount();

            return View(vm);
        }

        [HttpPost]
        public ActionResult LocalPayForServicesEnterAmount([Bind(Include = SenderLocalEnterAmountVM.BindProperty)]SenderLocalEnterAmountVM model)
        {
            if (ModelState.IsValid)
            {

                var vm = _payForSenderServices.GetLocalPayForServiceEnterAmount();
                vm.Amount = model.Amount;
                vm.PaymentReference = model.PaymentReference;
                _payForSenderServices.SetLocalPayForServiceEnterAmount(vm);
                return RedirectToAction("LocalPayForServicesSummary", "SenderPayForServices");
            }
            return View(model);
        }


        public ActionResult LocalPayForServicesSummary()
        {


            var sendingAmountData = _payForSenderServices.GetLocalPayForServiceEnterAmount();
            var loggedUSerData = _payForSenderServices.GetLoggedUserData();
            SenderAccountPaymentSummaryViewModel vm = new SenderAccountPaymentSummaryViewModel()
            {
                Amount = sendingAmountData.Amount,
                SendingCurrencyCode = sendingAmountData.CurrencyCode,
                SendingCurrencySymbol = sendingAmountData.CurrencySymbol,
                PaymentReference = sendingAmountData.PaymentReference,
                PaidAmount = sendingAmountData.Amount,
                ReceivedAmount = sendingAmountData.Amount,
                ReceiverName = sendingAmountData.ReceiverName,
            };

            if (sendingAmountData.SendSms == true)
            {
                vm.LocalSmsCharge = Common.Common.GetSmsFee(loggedUSerData.CountryCode);
            }
            vm.PaidAmount = vm.Amount + vm.LocalSmsCharge + vm.Fee;
            SetTransactionSummary();
            return View(vm);
        }

        [HttpPost]
        public ActionResult LocalPayForServicesSummary([Bind(Include = SenderAccountPaymentSummaryViewModel.BindProperty)]SenderAccountPaymentSummaryViewModel model)
        {
            _payForSenderServices.SetLocalPaymentSummary(model);
            return RedirectToAction("LocalPayment", "SenderPayForServices");

        }

        public ActionResult LocalPayment()
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var vm = _payForSenderServices.GetPaymentMethod();

            var payingAmountData = _payForSenderServices.GetLocalPaymentSummary();

            vm.TotalAmount = payingAmountData.PaidAmount;
            vm.SendingCurrencySymbol = payingAmountData.SendingCurrencySymbol;
            var senderwalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
            if (senderwalletInfo != null)
            {

                vm.KiipayWalletBalance = senderwalletInfo.CurrentBalance;

            }

            vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails();
            return View(vm);
        }


        [HttpPost]
        public ActionResult LocalPayment([Bind(Include = PaymentMethodViewModel.BindProperty)]PaymentMethodViewModel vm)
        {


            int selectedCardId = 0;

            foreach (var item in vm.CardDetails)
            {
                if (item.IsChecked == true)
                {
                    selectedCardId = item.CardId;
                    vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                }
            }
            //_kiiPayWalletTransferServices.SetPaymentMethod(vm);
            //_cashPickUpServices.SetPaymentMethod(vm);

            _payForSenderServices.SetPaymentMethod(vm);

            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();


            SetTransactionSummaryForLocalPayment();

            var transactionSummaryvm = _senderForAllTransferServices.GetTransactionSummary();

            switch (vm.SenderPaymentMode)
            {
                case SenderPaymentMode.SavedDebitCreditCard:
                    SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
                    var cardInfo = _saveCreditDebitCard.GetCardInfo(selectedCardId);
                    transactionSummaryvm.CreditORDebitCardDetials = new CreditDebitCardViewModel()
                    {
                        CardNumber = cardInfo.Num.Decrypt(),
                        NameOnCard = cardInfo.CardName.Decrypt(),
                        EndMM = cardInfo.EMonth.Decrypt(),
                        EndYY = cardInfo.EYear.Decrypt(),
                        SecurityCode = cardInfo.ClientCode.Decrypt()
                    };

                    var result = _senderForAllTransferServices.ValidateTransactionUsingStripe(transactionSummaryvm);
                    if (result.IsValid == false)
                    {

                        ModelState.AddModelError("TransactionError", result.Message);
                        return View(vm);
                    }

                    transactionSummaryvm.CreditORDebitCardDetials.StripeTokenID = result.StripeTokenId;
                    _senderForAllTransferServices.CompleteTransaction(transactionSummaryvm);

                    break;
                case SenderPaymentMode.CreditDebitCard:


                    return RedirectToAction("DebitCreditCardDetails");
                    break;
                case SenderPaymentMode.KiiPayWallet:
                    _senderForAllTransferServices.CompleteTransaction(transactionSummaryvm);
                    break;
                case SenderPaymentMode.MoneyFexBankAccount:

                    return RedirectToAction("MoneyFexBankDeposit");
                    break;
                default:
                    break;
            }

            return RedirectToAction("LocalPayForServicesSuccess");



            //if (model.SenderPaymentMode == SenderPaymentMode.CreditDebitCard)
            //{
            //    return RedirectToAction("LocalDebitCreditCardDetails", "SenderPayForServices");
            //}

            //if (model.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
            //{
            //    return RedirectToAction("LocalMoneyFexBankDeposit", "SenderPayForServices");
            //}

            //return RedirectToAction("", "SenderPayForServices");

            return View(vm);
        }

        public ActionResult LocalDebitCreditCardDetails()
        {
            var addresses = _payForSenderServices.GetAddress();
            ViewBag.Address = new SelectList(addresses, "Name", "Name");

            var transferMobileData = _payForSenderServices.GetLocalPayForServiceEnterAmount();

            CreditDebitCardViewModel vm = new CreditDebitCardViewModel()
            {
                FaxingAmount = transferMobileData.Amount,
                FaxingCurrencySymbol = transferMobileData.CurrencySymbol,
                FaxingCurrency = transferMobileData.CurrencyCode
            };


            return View(vm);
        }


        [HttpPost]
        public ActionResult LocalDebitCreditCardDetails([Bind(Include = CreditDebitCardViewModel.BindProperty)]CreditDebitCardViewModel model)
        {
            var addresses = _payForSenderServices.GetAddress();
            ViewBag.Address = new SelectList(addresses, "Name", "Name");

            if (ModelState.IsValid)
            {
                _payForSenderServices.SetDebitCreditCardDetail(model);

                return RedirectToAction("", "SenderPayForServices");
            }
            return View(model);
        }


        public ActionResult LocalMoneyFexBankDeposit()
        {


            var sendingAmountData = _payForSenderServices.GetLocalPayForServiceEnterAmount();
            SenderMoneyFexBankDepositVM vm = new SenderMoneyFexBankDepositVM()
            {
                Amount = sendingAmountData.Amount,
                SendingCurrencyCode = sendingAmountData.CurrencyCode,
                SendingCurrencySymbol = sendingAmountData.CurrencySymbol
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult LocalMoneyFexBankDeposit([Bind(Include = SenderMoneyFexBankDepositVM.BindProperty)]SenderMoneyFexBankDepositVM model)
        {
            if (ModelState.IsValid)
            {
                _payForSenderServices.SetMoneyFexBankAccountDeposit(model);
                return RedirectToAction("", "SenderPayForServices");
            }

            return View(model);
        }


        public ActionResult LocalPayForServicesSuccess()
        {

            var localEnterAmountData = _payForSenderServices.GetLocalPayForServiceEnterAmount();

            SenderAddMoneySuccessVM vm = new SenderAddMoneySuccessVM()
            {
                Amount = localEnterAmountData.Amount,
                Currnecy = localEnterAmountData.CurrencySymbol,
                ReceiverName = localEnterAmountData.ReceiverName
            };
            senderCommonFunc.ClearPayForServiceSession();
            return View(vm);
        }

        private void SetTransactionSummaryForLocalPayment()
        {


            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();


            // Data to show on success

            var payingAmountData = _payForSenderServices.GetLocalPaymentSummary();
            var localPayForServiceData = _payForSenderServices.GetSenderPayForGoodsAndServices();

            var localEnterAmountData = _payForSenderServices.GetLocalPayForServiceEnterAmount();

            //Completing Transaction
            var loggedUserData = _payForSenderServices.GetLoggedUserData();
            var paymentMethod = _payForSenderServices.GetPaymentMethod();


            TransactionSummaryVM transactionSummaryVm = new TransactionSummaryVM();
            transactionSummaryVm.SenderAndReceiverDetail = new SenderAndReceiverDetialVM()
            {
                SenderId = loggedUserData.Id,
                SenderCountry = loggedUserData.CountryCode,
                ReceiverCountry = loggedUserData.CountryCode,
                ReceiverId = localPayForServiceData.ReceiverId
            };

            //Set Sms Fee 
            transactionSummaryVm.KiiPayTransferPaymentSummary = new KiiPayTransferPaymentSummary()
            {
                ReceiverName = localPayForServiceData.ReceiverName,
                SendingCurrency = payingAmountData.SendingCurrencyCode,
                SendingAmount = payingAmountData.Amount,
                ReceivingAmount = payingAmountData.Amount,
                TotalAmount = payingAmountData.PaidAmount,
                ExchangeRate = 1,
                Fee = payingAmountData.Fee,
                PaymentReference = payingAmountData.PaymentReference,
                ReceivingCurrency = payingAmountData.SendingCurrencyCode,
                ReceivingCurrencySymbol = payingAmountData.SendingCurrencySymbol,
                SendingCurrencySymbol = payingAmountData.SendingCurrencySymbol,
                SendSMS = localEnterAmountData.SendSms,
                SMSFee = payingAmountData.LocalSmsCharge,
            };


            transactionSummaryVm.PaymentMethodAndAutoPaymentDetail = new PaymentMethodViewModel()
            {
                TotalAmount = payingAmountData.PaidAmount,
                SendingCurrencySymbol = payingAmountData.SendingCurrencySymbol,
                SenderPaymentMode = paymentMethod.SenderPaymentMode,
                EnableAutoPayment = false
            };

            //For DebitCreditCardDetail


            var debitCreditCardDetail = _payForSenderServices.GetDebitCreditCardDetail();

            transactionSummaryVm.CreditORDebitCardDetials = debitCreditCardDetail;



            var moneyFexBankAccountDepositData = _payForSenderServices.GetMoneyFexBankAccountDeposit();
            transactionSummaryVm.MoneyFexBankDeposit = moneyFexBankAccountDepositData;



            transactionSummaryVm.TransferType = TransferType.PayForServices;
            transactionSummaryVm.IsLocalPayment = true;

            _senderForAllTransferServices.SetTransactionSummary(transactionSummaryVm);




        }

        #endregion

        private void GetRecentlyPaidInternationBusiness(string country = "")
        {   
            var RecenltyPaidInternationalBusinesses = _payForSenderServices.GetRecentlyPaidInternationalServices(country).ToList();
            ViewBag.RecenltyPaidInternationalBusinesses = new SelectList(RecenltyPaidInternationalBusinesses, "Name", "Name");
        }

        private void GetCountriesDropDown(string Country = "")
        {
            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown()
               , "CountryCode", "CountryName", Country);
        }

        public JsonResult GetCountryPhonCode(string CountryCode)
        {

            var CountryPhoneCode = Common.Common.GetCountryPhoneCode(CountryCode);
            return Json(new
            {
                CountryCode = CountryPhoneCode
            }, JsonRequestBehavior.AllowGet);

        }

        //For Payment Summary

        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount, bool IsReceivingAmount)
       {
            //bool IsReceivingAmount = false;
            var enterAmountData = _payForSenderServices.GetEnterAmount();
            var loggedInSenderData = _payForSenderServices.GetLoggedUserData();
            //if ((SendingAmount > 0 && ReceivingAmount > 0) && enterAmountData.ReceivingAmount != ReceivingAmount)
            //{

            //    SendingAmount = ReceivingAmount;
            //    IsReceivingAmount = true;
            //}

            //if (SendingAmount == 0)
            //{

            //    IsReceivingAmount = true;
            //    SendingAmount = ReceivingAmount;
            //}
            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }
            var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                enterAmountData.ExchangeRate, SEstimateFee.GetFaxingCommision(loggedInSenderData.CountryCode));

            // Rewrite session with additional value 
            enterAmountData.Fee = result.FaxingFee;
            enterAmountData.SendingAmount = result.FaxingAmount;
            enterAmountData.ReceivingAmount = result.ReceivingAmount;
            enterAmountData.TotalAmount = result.TotalAmount;

            _payForSenderServices.SetEnterAmount(enterAmountData);
            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount = result.FaxingAmount
            }, JsonRequestBehavior.AllowGet);
        }



    }
}