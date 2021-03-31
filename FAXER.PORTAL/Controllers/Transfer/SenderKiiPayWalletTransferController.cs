using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Controllers.Transfer
{

    // Step :1 Create Services Class 
    // Step :2 Create Setter getter of all viewmodel in services   
    public class SenderKiiPayWalletTransferController : Controller
    {

        SSenderKiiPayWalletTransfer _kiiPayWalletTransferServices = null;


        public SenderKiiPayWalletTransferController()
        {
            _kiiPayWalletTransferServices = new SSenderKiiPayWalletTransfer();
        }
        // GET: SenderKiiPayWalletTransfer
        public ActionResult Index()
        {
            return View();
        }

        #region Local payment

        [HttpGet]
        public ActionResult SearchLocalKiiPayWallet()
        {

            string SenderCountry = Common.FaxerSession.LoggedUser.CountryCode; // Get sender country from session 
            GetRecentMobileNos(SenderCountry);

            var vm = _kiiPayWalletTransferServices.GetSearchKiiPayWallet();
            vm.CountryCode = SenderCountry;
            vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(SenderCountry);
            _kiiPayWalletTransferServices.SetSearchKiiPayWallet(vm);
            return View(vm);

        }

        [HttpPost]
        public ActionResult SearchLocalKiiPayWallet([Bind(Include = SearchKiiPayWalletVM.BindProperty)]SearchKiiPayWalletVM vm)
        {


            GetRecentMobileNos(vm.CountryCode);

            _kiiPayWalletTransferServices.SetSearchKiiPayWallet(vm);
            if (ModelState.IsValid)
            {


                var result = _kiiPayWalletTransferServices.ValidateMobileNo(vm.MobileNo,vm.CountryCode);
                if (result == null)
                {
                    ModelState.AddModelError("MobileNo", "Enter valid mobile no");

                    return View(vm);
                }

                // Set Sender And Receiver Details in session 

                SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
                var senderwalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
                _kiiPayWalletTransferServices.SetSenderAndReceiverDetails(new SenderAndReceiverDetialVM()
                {

                    ReceiverCountry = result.CardUserCountry,
                    ReceiverId = result.Id,
                    SenderCountry = Common.FaxerSession.LoggedUser.CountryCode,
                    SenderId = Common.FaxerSession.LoggedUser.Id,
                    ReceiverMobileNo = result.MobileNo,
                    SenderWalletId = senderwalletInfo == null ? 0 : senderwalletInfo.Id
                });

                // set Sending Currency And Receiving Currency 
                string ReceivingCountry = result.CardUserCountry;
                string SendingCountry = Common.FaxerSession.LoggedUser.CountryCode;
               
                _kiiPayWalletTransferServices.SetKiiPayTransferPaymentSummary(new KiiPayTransferPaymentSummary()
                {
                    ReceiverName = result.FirstName + " " + result.MiddleName + " " + result.LastName,
                    ReceivingCurrency = Common.Common.GetCountryCurrency(ReceivingCountry),
                    ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(ReceivingCountry),
                    SendingCurrency = Common.Common.GetCountryCurrency(SendingCountry),
                    SendingCurrencySymbol = Common.Common.GetCurrencySymbol(SendingCountry),
                    ExchangeRate = SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry),
                });
                //SSenderForAllTransfer.SetPaymentSummarySession(ReceivingCountry);
                _kiiPayWalletTransferServices.SetSearchKiiPayWallet(vm);

                return RedirectToAction("LocalEnterAmount");
            }
            return View(vm);

        }


        [HttpGet]
        public ActionResult LocalEnterAmount()
        {


            var vm = _kiiPayWalletTransferServices.GetKiiPayTransferPaymentSummary();
            return View(vm);
        }

        [HttpPost]
        public ActionResult LocalEnterAmount([Bind(Include = KiiPayTransferPaymentSummary.BindProperty)] KiiPayTransferPaymentSummary vm)
        {

            if (ModelState.IsValid)
            {

                // other  Session Value has already been set 
                // so only paymentreference has been ovewrite to session 
                var model = _kiiPayWalletTransferServices.GetKiiPayTransferPaymentSummary();
                model.PaymentReference = vm.PaymentReference;
                model.ReceivingAmount = vm.SendingAmount;
                
                model.SendingAmount = vm.SendingAmount;
                
                model.SMSFee = Common.Common.GetSmsFee(Common.FaxerSession.LoggedUser.CountryCode);
                model.TotalAmount = model.SendingAmount + model.SMSFee;
                _kiiPayWalletTransferServices.SetKiiPayTransferPaymentSummary(model);

                PaymentMethodViewModel paymentMethodViewModel = new PaymentMethodViewModel();
                paymentMethodViewModel.TotalAmount = model.TotalAmount;
                paymentMethodViewModel.SendingCurrencySymbol = model.SendingCurrencySymbol;
                _kiiPayWalletTransferServices.SetPaymentMethod(paymentMethodViewModel);

                return RedirectToAction("LocalPaymentSummary");
            }
            return View(vm);
        }


        public ActionResult LocalPaymentSummary()
        {

            var sendingAmountData = _kiiPayWalletTransferServices.GetKiiPayTransferPaymentSummary();
            string fullName = sendingAmountData.ReceiverName;
            var names = fullName.Split(' ');
            string firstName = names[0];
            KiiPayTransferPaymentSummary vm = new KiiPayTransferPaymentSummary()
            {
                SendingAmount = sendingAmountData.SendingAmount,
                Fee = sendingAmountData.Fee,
                ReceivingAmount = sendingAmountData.ReceivingAmount,
                SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
                SendingCurrency = sendingAmountData.SendingCurrency,
                TotalAmount = sendingAmountData.TotalAmount,
                ReceivingCurrencySymbol = sendingAmountData.ReceivingCurrencySymbol,
                ReceivingCurrency = sendingAmountData.ReceivingCurrency,
                ReceiverName = firstName,
                PaymentReference = sendingAmountData.PaymentReference
            };

            return View(vm);
        }
        [HttpGet]
        public ActionResult LocalPayNow()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var vm = _kiiPayWalletTransferServices.GetPaymentMethod();
            var senderwalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
            if (senderwalletInfo != null)
            {

                vm.KiipayWalletBalance = senderwalletInfo.CurrentBalance;
                vm.HasKiiPayWallet = true;
            }


            vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails();
            if (vm.CardDetails.Count == 0)
            {

                vm.SenderPaymentMode = SenderPaymentMode.CreditDebitCard;
            }
            return View(vm);

        }
        [HttpPost]
        public ActionResult LocalPayNow([Bind(Include = PaymentMethodViewModel.BindProperty)]PaymentMethodViewModel vm)
        {



            //_kiiPayWalletTransferServices.SetPaymentMethod(vm);

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
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
            _kiiPayWalletTransferServices.SetPaymentMethod(vm);

            TransactionSummaryVM transactionSummaryvm = new TransactionSummaryVM();
            transactionSummaryvm.IsLocalPayment = true;
            transactionSummaryvm.KiiPayTransferPaymentSummary = _kiiPayWalletTransferServices.GetKiiPayTransferPaymentSummary();
            transactionSummaryvm.SenderAndReceiverDetail = _kiiPayWalletTransferServices.GetSenderAndReceiverDetails();
            transactionSummaryvm.TransferType = TransferType.KiiPayWallet;
            transactionSummaryvm.PaymentMethodAndAutoPaymentDetail = _kiiPayWalletTransferServices.GetPaymentMethod();


            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();




            _senderForAllTransferServices.SetTransactionSummary(transactionSummaryvm);
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


                    return RedirectToAction("SenderAddMoneyCard");
                    break;
                case SenderPaymentMode.KiiPayWallet:
                    var hasEnoughBal = senderCommonFunc.SenderHasEnoughWalletBaltoTransfer(transactionSummaryvm.KiiPayTransferPaymentSummary.TotalAmount, transactionSummaryvm.SenderAndReceiverDetail.SenderWalletId);

                    if (hasEnoughBal == false)
                    {


                        ModelState.AddModelError("TransactionError", "Your wallet doesn't have enough balance!");
                        return View(vm);
                    }
                    _senderForAllTransferServices.CompleteTransaction(transactionSummaryvm);
                    break;
                case SenderPaymentMode.MoneyFexBankAccount:
                    break;
                default:
                    break;
            }

            return RedirectToAction("LocalPaymentSuccessfullyCompleted");

            return View(vm);

        }







        public ActionResult LocalPaymentSuccessfullyCompleted()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();

            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            var paymentSummary = _senderForAllTransferServices.GetTransactionSummary().KiiPayTransferPaymentSummary;

       

            PaymentCompletedVM vm = new PaymentCompletedVM()
            {
                Amount = paymentSummary.ReceivingAmount,
                Currency = paymentSummary.ReceivingCurrency,
                CurrencySymbol = paymentSummary.ReceivingCurrencySymbol,
                ReceiverName = paymentSummary.ReceiverName
            };

            senderCommonFunc.ClearKiiPayTransferSession();
            return View(vm);
        }
        #endregion

        #region International Payment 

        [HttpGet]
        public ActionResult SearchInternationalKiiPayWallet(string Country = "")
        {

            GetCountries();
            GetRecentMobileNos(Country);

            var vm = _kiiPayWalletTransferServices.GetSearchKiiPayWallet();
            if (vm.CountryCode != null)
            {
                vm.CountryCode = vm.CountryCode;
                vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);
                GetRecentMobileNos(vm.CountryCode);
            }
            else
            {
                vm.CountryCode = Country;
                vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Country);

                GetRecentMobileNos(Country);
            }
            return View(vm);

        }

        [HttpPost]
        public ActionResult SearchInternationalKiiPayWallet([Bind(Include = SearchKiiPayWalletVM.BindProperty)] SearchKiiPayWalletVM vm)
        {


            GetCountries();
            GetRecentMobileNos(vm.CountryCode);

            if (ModelState.IsValid)
            {


                var result = _kiiPayWalletTransferServices.ValidateMobileNo(vm.MobileNo,vm.CountryCode);
                if (result == null)
                {
                    ModelState.AddModelError("MobileNo", "Enter valid mobile no");

                    return View(vm);
                }

                // Set Sender And Receiver Details in session 

                SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
                var senderwalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
                _kiiPayWalletTransferServices.SetSenderAndReceiverDetails(new SenderAndReceiverDetialVM()
                {

                    ReceiverCountry = result.CardUserCountry,
                    ReceiverId = result.Id,
                    SenderCountry = Common.FaxerSession.LoggedUser.CountryCode,
                    SenderId = Common.FaxerSession.LoggedUser.Id,
                    SenderWalletId= senderwalletInfo == null ? 0 : senderwalletInfo.Id
                });

                // set Sending Currency And Receiving Currency 
                string ReceivingCountry = vm.CountryCode;
                string SendingCountry = Common.FaxerSession.LoggedUser.CountryCode;
                //SSenderForAllTransfer.SetPaymentSummarySession(ReceivingCountry);
                _kiiPayWalletTransferServices.SetKiiPayTransferPaymentSummary(new KiiPayTransferPaymentSummary()
                {
                    ReceiverName = result.FirstName + " " + result.MiddleName + " " + result.LastName,
                    ReceivingCurrency = Common.Common.GetCountryCurrency(ReceivingCountry),
                    ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(ReceivingCountry),
                    SendingCurrency = Common.Common.GetCountryCurrency(SendingCountry),
                    SendingCurrencySymbol = Common.Common.GetCurrencySymbol(SendingCountry),
                    ExchangeRate = SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry),
                });
               
                _kiiPayWalletTransferServices.SetSearchKiiPayWallet(vm);
                return RedirectToAction("InternationalEnterAmount");
            }
            return View(vm);

        }



        [HttpGet]
        public ActionResult InternationalEnterAmount()
        {


            var vm = _kiiPayWalletTransferServices.GetKiiPayTransferPaymentSummary();
            return View(vm);
        }

        [HttpPost]
        public ActionResult InternationalEnterAmount([Bind(Include = KiiPayTransferPaymentSummary.BindProperty)] KiiPayTransferPaymentSummary vm)
        {

            if (ModelState.IsValid)
            {

                // other  Session Value has already been set 
                // so only paymentreference has been ovewrite to session 

                var model = _kiiPayWalletTransferServices.GetKiiPayTransferPaymentSummary();
                model.PaymentReference = vm.PaymentReference;
                model.ReceiverName = vm.ReceiverName;
                _kiiPayWalletTransferServices.SetKiiPayTransferPaymentSummary(model);
                PaymentMethodViewModel paymentMethodViewModel = new PaymentMethodViewModel();
                paymentMethodViewModel.TotalAmount = model.TotalAmount;
                paymentMethodViewModel.SendingCurrencySymbol = model.SendingCurrencySymbol;
                _kiiPayWalletTransferServices.SetPaymentMethod(paymentMethodViewModel);
                return RedirectToAction("InternationalPaymentSummary");
            }
            return View(vm);
        }


        [HttpGet]
        public ActionResult InternationalPaymentSummary()
        {

            var sendingAmountData = _kiiPayWalletTransferServices.GetKiiPayTransferPaymentSummary();
            
            string fullName = sendingAmountData.ReceiverName;
            var names = fullName.Split(' ');
            string firstName = names[0];
            KiiPayTransferPaymentSummary vm = new KiiPayTransferPaymentSummary()
            {
                SendingAmount = sendingAmountData.SendingAmount,
                Fee = sendingAmountData.Fee,
                ReceivingAmount = sendingAmountData.ReceivingAmount,
                SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
                SendingCurrency = sendingAmountData.SendingCurrency,
                TotalAmount = sendingAmountData.TotalAmount,
                ReceivingCurrencySymbol = sendingAmountData.ReceivingCurrencySymbol,
                ReceivingCurrency = sendingAmountData.ReceivingCurrency,
                ReceiverName = firstName,
                PaymentReference = sendingAmountData.PaymentReference
            };
            return View(vm);
        }

        [HttpGet]
        public ActionResult InternationalPayNow()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var vm = _kiiPayWalletTransferServices.GetPaymentMethod();
            var senderwalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
            if (senderwalletInfo != null)
            {

                vm.KiipayWalletBalance = senderwalletInfo.CurrentBalance;
                vm.HasKiiPayWallet = true;

            }

            vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails();
            return View(vm);
        }

        [HttpPost]
        public ActionResult InternationalPayNow([Bind(Include = PaymentMethodViewModel.BindProperty)] PaymentMethodViewModel vm)
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
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
            _kiiPayWalletTransferServices.SetPaymentMethod(vm);

            TransactionSummaryVM transactionSummaryvm = new TransactionSummaryVM();
            transactionSummaryvm.IsLocalPayment = false;
            transactionSummaryvm.KiiPayTransferPaymentSummary = _kiiPayWalletTransferServices.GetKiiPayTransferPaymentSummary();
            transactionSummaryvm.SenderAndReceiverDetail = _kiiPayWalletTransferServices.GetSenderAndReceiverDetails();
            transactionSummaryvm.TransferType = TransferType.KiiPayWallet;
            transactionSummaryvm.PaymentMethodAndAutoPaymentDetail = _kiiPayWalletTransferServices.GetPaymentMethod();

            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

            _senderForAllTransferServices.SetTransactionSummary(transactionSummaryvm);
            vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails();

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


                    return RedirectToAction("SenderAddMoneyCard");
                    
                case SenderPaymentMode.KiiPayWallet:
                    var hasEnoughBal = senderCommonFunc.SenderHasEnoughWalletBaltoTransfer(transactionSummaryvm.KiiPayTransferPaymentSummary.TotalAmount, transactionSummaryvm.SenderAndReceiverDetail.SenderWalletId);

                    if (hasEnoughBal == false)
                    {


                        ModelState.AddModelError("TransactionError", "Your wallet doesn't have enough balance!");
                        return View(vm);
                    }
                    _senderForAllTransferServices.CompleteTransaction(transactionSummaryvm);
                    break;
                case SenderPaymentMode.MoneyFexBankAccount:
                    break;
                default:
                    break;
            }

            return RedirectToAction("InternationalPaymentSuccessfullyCompleted");
         
        }

        public ActionResult InternationalPaymentSuccessfullyCompleted()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();


            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            var transSummary = _senderForAllTransferServices.GetTransactionSummary();
            PaymentCompletedVM vm = new PaymentCompletedVM()
            {
                Amount = transSummary.KiiPayTransferPaymentSummary.ReceivingAmount,
                Currency = transSummary.KiiPayTransferPaymentSummary.ReceivingCurrency,
                CurrencySymbol = transSummary.KiiPayTransferPaymentSummary.ReceivingCurrencySymbol,
                ReceiverName = transSummary.KiiPayTransferPaymentSummary.ReceiverName
            };
            senderCommonFunc.ClearKiiPayTransferSession();
            return View(vm);
           
        }
        [HttpGet]
        public ActionResult SenderAddMoneyCard(bool IsAddDebitCreditCard = false)
        {
           
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            var Vm = new CreditDebitCardViewModel();
            
            //var paymentInfo = _senderForAllTransferServices.GetTransactionSummary().KiiPayTransferPaymentSummary;

            var paymentInfo = _kiiPayWalletTransferServices.GetKiiPayTransferPaymentSummary();
            Vm.FaxingAmount = paymentInfo.TotalAmount;
            Vm.FaxingCurrencySymbol = paymentInfo.SendingCurrencySymbol;
            Vm.FaxingCurrency = paymentInfo.SendingCurrency;
            Vm.SaveCard = IsAddDebitCreditCard;
            Vm.NameOnCard = Common.FaxerSession.LoggedUser.FullName;
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            Vm.AddressLineOne = senderCommonFunc.GetSenderAddress();
            return View(Vm);

        }
        [HttpPost]
        public ActionResult SenderAddMoneyCard( [Bind(Include = CreditDebitCardViewModel.BindProperty)]CreditDebitCardViewModel vm)
        {
            var result = Common.Common.ValidateCreditDebitCard(vm);
            if (result.Data == false)
            {
                ModelState.AddModelError("", result.Message);
                return View(vm);
            }

            //ViewBag.SelectCards = new SelectList(GetSelectCards(), "Code", "Name");
            //ViewBag.Addresses = new SelectList(GetAddresses(), "Code", "Name");

            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            var transferSummary = _senderForAllTransferServices.GetTransactionSummary();
            SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();

            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                CardName = Common.FaxerSession.LoggedUser.FullName,
                ExpirationMonth = vm.EndMM,
                ExpiringYear = vm.EndYY,
                Number = vm.CardNumber,
                SecurityCode = vm.SecurityCode,

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
                    Amount = vm.FaxingAmount,
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
                    ReceiptNo = Common.FaxerSession.ReceiptNo
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
                    transferSummary.CreditORDebitCardDetials = vm;
                    _senderForAllTransferServices.CompleteTransaction(transferSummary);

                    if (transferSummary.IsLocalPayment == false)
                    {

                        return RedirectToAction("InternationalPaymentSuccessfullyCompleted");
                    }
                    else
                    {

                        return RedirectToAction("LocalPaymentSuccessfullyCompleted");
                    }

                }
                else
                {
                    ModelState.AddModelError("TransactionError", transactionResult.Message);
                }
                return View(vm);
            }


            return View(vm);

        }

        // Estimate The payment summary 

        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount , bool IsReceivingAmount)
        {
            var InternationalTransferAmountSummary = Common.FaxerSession.KiiPayTransferPaymentSummary;

            if (IsReceivingAmount) {

                SendingAmount = ReceivingAmount;
            }
            var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                InternationalTransferAmountSummary.ExchangeRate, SEstimateFee.GetFaxingCommision(Common.FaxerSession.LoggedUser.CountryCode));

            // Rewrite session with additional value 
            InternationalTransferAmountSummary.Fee = result.FaxingFee;
            InternationalTransferAmountSummary.SendingAmount = result.FaxingAmount;
            InternationalTransferAmountSummary.ReceivingAmount = result.ReceivingAmount;
            InternationalTransferAmountSummary.TotalAmount = result.TotalAmount;
            _kiiPayWalletTransferServices.SetKiiPayTransferPaymentSummary(InternationalTransferAmountSummary);

            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount = result.FaxingAmount
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public void GetCountries()
        {

            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown(), "CountryCode", "CountryName");
        }
        public void GetRecentMobileNos(string CountryCode)
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            ViewBag.MobileNumbers = new SelectList(senderCommonFunc.GetRecenltyPaidReceivers(CountryCode), "ReceiverMobileNo" , "ReceiverMobileNo");

        }
        public ActionResult PayLocalUsingDebitCreditCard()
        {

            return View();
        }

        public ActionResult PayLocalToMoneyFexBankAccount()
        {

            return View();
        }
        public JsonResult GetNumberName(string PhoneNumber,string CountryCode)
        {
            var receivername = _kiiPayWalletTransferServices.ValidateMobileNo(PhoneNumber,CountryCode).FirstName;
            return Json(new
            {

                phoneTextBox = PhoneNumber,
                ReceiverName = receivername
            }, JsonRequestBehavior.AllowGet);
        }

    }
}