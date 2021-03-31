using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.SecureTradingPaymentGateway;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Controllers.Transfer
{
    public class SenderCashPickUpController : Controller
    {
        SSenderCashPickUp _cashPickUpServices = null;
        public SenderCashPickUpController()
        {
            _cashPickUpServices = new SSenderCashPickUp();
        }

        // GET: SenderCashPickUp
        public ActionResult Index()
        {
            var country = Common.Common.GetCountries();
            //fetch data from db

            //var reasons = _cashPickUpServices.GetReasons();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");
            var IdCardTypes = Common.Common.GetIdCardType();
            ViewBag.IdCardTypes = new SelectList(IdCardTypes, "Id", "Name");
            //ViewBag.Reasons = new SelectList(reasons, "Id", "Name");
            var vm = _cashPickUpServices.GetSenderCashPickUp();
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            if (Common.FaxerSession.IsTransferFromHomePage || Common.FaxerSession.IsCommonEstimationPage)
            {

                vm.CountryCode = _kiiPaytrasferServices.GetCommonEnterAmount().ReceivingCountryCode;
                ViewBag.ReceivingCountryCurrency = _kiiPaytrasferServices.GetCommonEnterAmount().ReceivingCurrency;
                ViewBag.ReceivingCountry = _kiiPaytrasferServices.GetCommonEnterAmount().ReceivingCountryCode.ToLower();
                ViewBag.TransferMethod = "Cash PickUp";
                ViewBag.SendingCountryCurrency = _kiiPaytrasferServices.GetCommonEnterAmount().SendingCurrency;
                ViewBag.SendingAmount = _kiiPaytrasferServices.GetCommonEnterAmount().SendingAmount;

            }
            var senderStatus = Common.Common.SenderStatus(Common.FaxerSession.LoggedUser.Id);
            //if (Common.FaxerSession.IsTransferFromHomePage == true && (senderStatus != DocumentApprovalStatus.Approved))
            //{
            //    return RedirectToAction("Index", "SenderTransferMoneyNow", new { @IsFormHomePage = true });
            //}
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);
            var recentReceivers = _cashPickUpServices.GetRecentReceivers(vm.CountryCode);

            ViewBag.RecentReceivers = new SelectList(recentReceivers, "Id", "ReceiverName");

            var amountSummary = _kiiPaytrasferServices.GetCommonEnterAmount(); // GetSummary set in Session 
            if (Common.FaxerSession.IsTransferFromHomePage
        && amountSummary.SendingCountryCode != Common.FaxerSession.LoggedUser.CountryCode)
            {

                return RedirectToAction("Index", "SenderTransferMoneyNow");
            }
            return View(vm);


        }

        [HttpPost]
        public ActionResult Index([Bind(Include = SenderCashPickUpVM.BindProperty)] SenderCashPickUpVM vm)
        {
            //vm = _cashPickUpServices.GetSenderCashPickUp();
            var country = Common.Common.GetCountries();
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);

            var IdCardTypes = Common.Common.GetIdCardType();
            ViewBag.IdCardTypes = new SelectList(IdCardTypes, "Id", "Name");
            //fetch data from db
            var recentReceivers = _cashPickUpServices.GetRecentReceivers(vm.CountryCode);
            var reasons = _cashPickUpServices.GetReasons();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");
            ViewBag.RecentReceivers = new SelectList(recentReceivers, "Id", "ReceiverName");
            ViewBag.Reasons = new SelectList(reasons, "Id", "Name");
            SSenderKiiPayWalletTransfer _senderKiiPayWalletTransfer = new SSenderKiiPayWalletTransfer();
            var paymentInfo = _senderKiiPayWalletTransfer.GetCommonEnterAmount();
            ViewBag.ReceivingCountryCurrency = paymentInfo.ReceivingCurrency;
            ViewBag.TransferMethod = "Cash PickUp";
            ViewBag.SendingCountryCurrency = paymentInfo.SendingCurrency;
            ViewBag.SendingAmount = paymentInfo.SendingAmount;
            ViewBag.ReceivingCountry = paymentInfo.ReceivingCountryCode.ToLower();

            var isServiceAvailable = Common.Common.
                            GetTransferServices(Common.FaxerSession.LoggedUser.CountryCode, vm.CountryCode
                            ).Where(x => x.ServiceType == DB.TransferService.CahPickUp).FirstOrDefault();
            if (isServiceAvailable == null)
            {
                ModelState.AddModelError("", "Cash Pick up Service is not available");
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                if (vm.Reason == ReasonForTransfer.Non)
                {
                    ModelState.AddModelError("Reason", "Select Reason for Transfer");
                    return View(vm);
                }

                if (vm.CountryCode == "MA")
                {

                    var Apiservice = Common.Common.GetApiservice(Common.FaxerSession.LoggedUser.CountryCode, vm.CountryCode, ViewBag.SendingAmount,
                        TransactionTransferMethod.CashPickUp, TransactionTransferType.Online);

                    if (Apiservice == null)
                    {

                        ModelState.AddModelError("ServiceNotAvialable", "Service Not Available");
                        return View(vm);

                    }
                    if (vm.IdenityCardId < 0)
                    {
                        ModelState.AddModelError("IdenityCardId", "Select Id card type");
                        return View(vm);
                    }
                    if (string.IsNullOrEmpty(vm.IdentityCardNumber))
                    {
                        ModelState.AddModelError("IdentityCardNumber", "Enter card number");
                        return View(vm);
                    }

                    SmsApi smsApi = new SmsApi();
                    var IsValidMobileNo = smsApi.IsValidMobileNo(ViewBag.CountryPhoneCode + "" + vm.MobileNumber);
                    if (IsValidMobileNo == false)
                    {
                        ModelState.AddModelError("", "Enter Valid Number");
                        return View(vm);
                    }
                }


                bool IsValidChasPickUpReceiver = Common.Common.IsValidBankDepositReceiver(vm.MobileNumber, Service.CashPickUP);

                if (IsValidChasPickUpReceiver == false)
                {
                    ModelState.AddModelError("", " Receiver is banned");
                    return View(vm);
                }



                int RecipientId = _cashPickUpServices.GetRecipientId(vm.MobileNumber, Service.CashPickUP);
                bool HasExceededMobileWalletReceiverLimit = Common.Common.HasExceededReceiverLimit(Common.FaxerSession.SenderId, RecipientId, Common.FaxerSession.LoggedUser.CountryCode,
                    vm.CountryCode, TransactionTransferMethod.CashPickUp);
                if (HasExceededMobileWalletReceiverLimit)
                {
                    ModelState.AddModelError("", "Transaction for Recipient limit exceeded");
                    return View(vm);
                }
                bool HasSenderTransacionExceedLimit = Common.Common.HasExceededSenderTransactionLimit(Common.FaxerSession.SenderId,
                    Common.FaxerSession.LoggedUser.CountryCode, vm.CountryCode, TransactionTransferMethod.CashPickUp);
                if (HasSenderTransacionExceedLimit)
                {
                    ModelState.AddModelError("", "Sender daily transaction limit exceeded");
                    return View(vm);
                }

                //Set Vm in Session
                vm.SendingCurrency = paymentInfo.SendingCurrency;
                vm.ReceivingCurrency = paymentInfo.ReceivingCurrency;
                _cashPickUpServices.SetSenderCashPickUp(vm);
                //SetData in EnterAmount Session
                var loggedInSenderData = _cashPickUpServices.GetLoggedUserData();

                SenderMobileEnrterAmountVm mobileEnter = new SenderMobileEnrterAmountVm()
                {
                    ReceiverName = vm.FullName,
                    ReceiverId = vm.RecentReceiverId ?? 0,
                    SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol,
                    SendingCurrencyCode = paymentInfo.SendingCurrency,
                    ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol,
                    ReceivingCurrencyCode = paymentInfo.ReceivingCurrency,
                    ExchangeRate = paymentInfo.ExchangeRate,
                    SendingCountryCode = loggedInSenderData.CountryCode,
                    ReceivingCountryCode = vm.CountryCode,

                };
                //SSenderForAllTransfer.SetPaymentSummarySession(vm.CountryCode);


                _senderKiiPayWalletTransfer.setPaymentSummary(TransactionTransferMethod.CashPickUp);
                _cashPickUpServices.SetSenderMobileEnrterAmount(mobileEnter);
                //return RedirectToAction("CashPickUpEnterAmount", "SenderCashPickUp");
                return RedirectToAction("CashPickUpSummary", "SenderCashPickUp");
            }
            return View(vm);
        }


        public ActionResult CashPickUpEnterAmount()
        {
            var loggedInSenderData = _cashPickUpServices.GetLoggedUserData();
            var cashPickUpData = _cashPickUpServices.GetSenderCashPickUp();

            var vm = _cashPickUpServices.GetSenderMobileEnrterAmount();
            //SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm()
            //{
            //    ReceiverName = enterAmountData.ReceiverName,
            //    ReceiverId = enterAmountData.ReceiverId,
            //    SendingCurrencySymbol = enterAmountData.SendingCurrencySymbol,
            //    SendingCurrencyCode = enterAmountData.SendingCurrencyCode,
            //    ReceivingCurrencySymbol = enterAmountData.ReceivingCurrencySymbol,
            //    ReceivingCurrencyCode = enterAmountData.ReceivingCurrencyCode,
            //    ExchangeRate = enterAmountData.ExchangeRate,
            //};
            //_cashPickUpServices.SetSenderMobileEnrterAmount(vm);
            return View(vm);
        }

        [HttpPost]
        public ActionResult CashPickUpEnterAmount([Bind(Include = SenderMobileEnrterAmountVm.BindProperty)] SenderMobileEnrterAmountVm model)
        {

            if (ModelState.IsValid)
            {
                var enterAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();
                enterAmountData.PaymentReference = model.PaymentReference;
                _cashPickUpServices.SetSenderMobileEnrterAmount(enterAmountData);

                return RedirectToAction("CashPickUpSummary", "SenderCashPickUp");
            }
            return View(model);

        }

        public ActionResult CashPickUpSummary()
        {

            if (Common.FaxerSession.TransactionId != 0)
            {
                _cashPickUpServices.setAmount(Common.FaxerSession.TransactionId);

            }
            if (Common.FaxerSession.RecipientId != 0)
            {
                _cashPickUpServices.setReciverInfo(Common.FaxerSession.RecipientId);

            }

            //Fetching data for summary
            var sendingAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();
            ViewBag.ReceivingCountryCurrency = sendingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Cash PickUp";
            ViewBag.SendingCountryCurrency = sendingAmountData.SendingCurrencyCode;
            ViewBag.SendingAmount = sendingAmountData.SendingAmount;
            ViewBag.ReceivingCountry = Common.Common.GetCountryCodeByCurrency(sendingAmountData.ReceivingCurrencyCode).ToLower();

            string fullName = sendingAmountData.ReceiverName;
            var names = fullName.Split(' ');
            string firstName = names[0];
            ViewBag.ReceiverName = fullName;

            SenderAccountPaymentSummaryViewModel vm = new SenderAccountPaymentSummaryViewModel()
            {
                Amount = sendingAmountData.SendingAmount,
                Fee = sendingAmountData.Fee,
                ReceivedAmount = sendingAmountData.ReceivingAmount,
                SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
                SendingCurrencyCode = sendingAmountData.SendingCurrencyCode,
                PaidAmount = sendingAmountData.TotalAmount,
                ReceiverName = firstName,
                ReceivingCurrencySymbol = sendingAmountData.ReceivingCurrencySymbol,
                ReceivingCurrecyCoode = sendingAmountData.ReceivingCurrencyCode

            };

            //reference not available
            vm.PaymentReference = "";
            var cashPickup = _cashPickUpServices.GetSenderCashPickUp();



            return View(vm);
        }


        [HttpPost]
        public ActionResult CashPickUpSummary([Bind(Include = SenderAccountPaymentSummaryViewModel.BindProperty)] SenderAccountPaymentSummaryViewModel model)
        {
            bool Isvalid = true;

            var CashPckUP = _cashPickUpServices.SaveIncompleteTransaction();
            Common.FaxerSession.TransactionId = CashPckUP.Id;
            Common.FaxerSession.IsTransactionOnpending = true;
            var sendingAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();
            TransactionPendingViewModel transactionPending = new TransactionPendingViewModel()
            {
                TransactionId = CashPckUP.Id,
                IsTransactionPending = true,
                TransferMethod = TransactionServiceType.CashPickUp,
                Fee = CashPckUP.FaxingFee,
                ExchangeRate = CashPckUP.ExchangeRate,
                ReceiverFullName = sendingAmountData.ReceiverName,
                ReceiptNumber = CashPckUP.ReceiptNumber,
                ReceivingCountry = Common.Common.GetCountryName(CashPckUP.ReceivingCountry),
                Receivingurrency = CashPckUP.ReceivingCurrency,
                SenderId = Common.FaxerSession.LoggedUser.Id,
                SendingAmount = CashPckUP.FaxingAmount,
                SendingCurrency = CashPckUP.SendingCurrency,
                TransactionNumber = CashPckUP.ReceiptNumber,
                MFCN = CashPckUP.MFCN

            };
            _cashPickUpServices.SetTransactionPendingViewModel(transactionPending);


            if (Common.FaxerSession.TransactionId == 0 && Common.FaxerSession.RecipientId == 0)
            {
            }
            else
            {
                Isvalid = _cashPickUpServices.RepeatTransaction(Common.FaxerSession.TransactionId, Common.FaxerSession.RecipientId);
            }

            if (!Isvalid)
            {
                string ErrorMessage = Common.FaxerSession.ErrorMessage;
                ModelState.AddModelError("", ErrorMessage);

                ViewBag.ReceivingCountryCurrency = sendingAmountData.ReceivingCurrencyCode;
                ViewBag.TransferMethod = "Cash PickUp";
                ViewBag.SendingCountryCurrency = sendingAmountData.SendingCurrencyCode;
                ViewBag.SendingAmount = sendingAmountData.SendingAmount;
                ViewBag.ReceivingCountry = Common.Common.GetCountryCodeByCurrency(sendingAmountData.ReceivingCurrencyCode).ToLower();
                ViewBag.ReceiverName = sendingAmountData.ReceiverName;
                string fullName = sendingAmountData.ReceiverName;
                var names = fullName.Split(' ');
                string firstName = names[0];
                model.Amount = sendingAmountData.SendingAmount;
                model.Fee = sendingAmountData.Fee;
                model.ReceivedAmount = sendingAmountData.ReceivingAmount;
                model.SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol;
                model.SendingCurrencyCode = sendingAmountData.SendingCurrencyCode;
                model.PaidAmount = sendingAmountData.TotalAmount;
                model.ReceiverName = firstName;
                model.ReceivingCurrencySymbol = sendingAmountData.ReceivingCurrencySymbol;
                model.ReceivingCurrecyCoode = sendingAmountData.ReceivingCurrencyCode;
                return View(model);
            }
            return RedirectToAction("InternationalPayNow", "SenderCashPickUp");
        }


        [HttpGet]
        public ActionResult InternationalPayNow()
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var vm = _cashPickUpServices.GetPaymentMethod();
            var sendsingAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();
            ViewBag.ReceivingCountryCurrency = sendsingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Cash PickUp";
            ViewBag.SendingCountryCurrency = sendsingAmountData.SendingCurrencyCode;
            ViewBag.SendingAmount = sendsingAmountData.SendingAmount;
            ViewBag.ReceiverName = sendsingAmountData.ReceiverName;
            vm.TotalAmount = sendsingAmountData.TotalAmount;
            vm.SendingCurrencySymbol = sendsingAmountData.SendingCurrencySymbol;
            ViewBag.Fee = sendsingAmountData.Fee;

            //Credit/Debit card 
            //Fee: GBP 0.05
            //Manual Bank Deposit
            //Fee: GBP 0.79
            ViewBag.CreditDebitFee = new CreditDebitCardViewModel().CreditDebitCardFee;
            ViewBag.ManualBankDepositFee = new SenderMoneyFexBankDepositVM().BankFee;

            var senderwalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
            if (senderwalletInfo != null)
            {
                vm.KiipayWalletBalance = senderwalletInfo.CurrentBalance;
                vm.HasKiiPayWallet = true;
            }
            else
            {
                vm.HasKiiPayWallet = false;
            }

            vm.HasEnableMoneyFexBankAccount = senderCommonFunc.IsEnabledMoneyFexbankAccount(Common.FaxerSession.LoggedUser.CountryCode);
            vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails();

            return View(vm);
        }


        [HttpPost]
        public ActionResult InternationalPayNow([Bind(Include = PaymentMethodViewModel.BindProperty)] PaymentMethodViewModel vm)
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var sendsingAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();
            var cashPickUpData = _cashPickUpServices.GetSenderCashPickUp();
            ViewBag.ReceivingCountryCurrency = sendsingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Cash PickUp";
            ViewBag.SendingCountryCurrency = sendsingAmountData.SendingCurrencyCode;
            ViewBag.SendingAmount = sendsingAmountData.SendingAmount;
            ViewBag.ReceiverName = sendsingAmountData.ReceiverName;
            ViewBag.ReceivingCountry = Common.Common.GetCountryCodeByCurrency(sendsingAmountData.ReceivingCurrencyCode).ToLower();
            ViewBag.Fee = sendsingAmountData.Fee;

            //Credit/Debit card 
            //Fee: GBP 0.05
            //Manual Bank Deposit
            //Fee: GBP 0.79
            ViewBag.CreditDebitFee = new CreditDebitCardViewModel().CreditDebitCardFee;
            ViewBag.ManualBankDepositFee = new SenderMoneyFexBankDepositVM().BankFee;


            vm.TotalAmount = sendsingAmountData.TotalAmount;
            int selectedCardId = 0;
            string cardNumber = null;
            if (vm.CardDetails != null)
            {
                foreach (var item in vm.CardDetails)
                {
                    if (item.IsChecked == true)
                    {
                        selectedCardId = item.CardId;
                        cardNumber = item.CardNumber;
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                    }
                }
            }
            int RecipientId = _cashPickUpServices.GetRecipientId(cashPickUpData.MobileNumber, Service.CashPickUP);
            bool HasExceededMobileWalletReceiverLimit = Common.Common.HasExceededReceiverLimit(Common.FaxerSession.SenderId, RecipientId, Common.FaxerSession.LoggedUser.CountryCode,
                cashPickUpData.CountryCode, TransactionTransferMethod.CashPickUp);
            if (HasExceededMobileWalletReceiverLimit)
            {
                ModelState.AddModelError("", "Transaction for Recipient limit exceeded");
                return View(vm);
            }
            bool HasSenderTransacionExceedLimit = Common.Common.HasExceededSenderTransactionLimit(Common.FaxerSession.SenderId,
                Common.FaxerSession.LoggedUser.CountryCode, cashPickUpData.CountryCode, TransactionTransferMethod.CashPickUp);
            if (HasSenderTransacionExceedLimit)
            {
                ModelState.AddModelError("", "Sender daily transaction limit exceeded");
                return View(vm);
            }
            //bool HasTransacionAmountExceedLimit = Common.Common.HasExceededAmountLimit(Common.FaxerSession.SenderId, Common.FaxerSession.LoggedUser.CountryCode,
            //    cashPickUpData.CountryCode, sendsingAmountData.SendingAmount, Module.Faxer);
            //if (HasTransacionAmountExceedLimit)
            //{
            //    ModelState.AddModelError("", "MoneyFex account daily transaction limit exceeded");
            //    return View(vm);
            //}

            _cashPickUpServices.SetPaymentMethod(vm);
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            SetTransactionSummary();
            var transactionSummaryvm = _senderForAllTransferServices.GetTransactionSummary();
            //Common.FaxerSession.ReceiptNo = Common.Common.GenerateCashPickUpReceiptNo(6);
            switch (vm.SenderPaymentMode)
            {
                case SenderPaymentMode.SavedDebitCreditCard:

                    ViewBag.IsSavedCreditDebitCard = true;
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

                    var result = _senderForAllTransferServices.ValidateTransactionUsingStripe(transactionSummaryvm, Common.FaxerSession.LoggedUser.CountryCode, cashPickUpData.CountryCode);
                    if (result.IsValid == false)
                    {
                        ModelState.AddModelError("TransactionError", result.Message);
                        return View(vm);
                    }
                    var validateCardresult = Common.Common.ValidateCreditDebitCard(transactionSummaryvm.CreditORDebitCardDetials);
                    if (validateCardresult.Data == false)
                    {
                        ModelState.AddModelError("TransactionError", validateCardresult.Message);
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                        return View(vm);
                    }
                    StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
                    {
                        CardName = Common.FaxerSession.LoggedUser.FullName,
                        ExpirationMonth = transactionSummaryvm.CreditORDebitCardDetials.EndMM,
                        ExpiringYear = transactionSummaryvm.CreditORDebitCardDetials.EndYY,
                        Number = transactionSummaryvm.CreditORDebitCardDetials.CardNumber,
                        SecurityCode = transactionSummaryvm.CreditORDebitCardDetials.SecurityCode,

                        billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                        billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                        CurrencyCode = Common.Common.GetCurrencyCode(Common.FaxerSession.LoggedUser.CountryCode),
                        Amount = transactionSummaryvm.CreditORDebitCardDetials.FaxingAmount,
                        SenderId = Common.FaxerSession.LoggedUser.Id
                    };

                    var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm, Common.FaxerSession.LoggedUser.CountryCode, cashPickUpData.CountryCode);
                    if (StripeResult.IsValid == false)
                    {

                        ModelState.AddModelError("TransactionError", StripeResult.Message);
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                        return View(vm);
                    }
                    _cashPickUpServices.SetDebitCreditCardDetail(transactionSummaryvm.CreditORDebitCardDetials);

                    //transactionSummaryvm.CreditORDebitCardDetials.StripeTokenID = result.StripeTokenId;

                    return RedirectToAction("DebitCreditCardDetails", new { IsFromSavedDebitCard = true });

                case SenderPaymentMode.CreditDebitCard:
                    //Common.FaxerSession.ReceiptNo = Common.Common.GenerateCashPickUpReceiptNo(6);
                    return RedirectToAction("DebitCreditCardDetails");
                case SenderPaymentMode.KiiPayWallet:
                    var hasEnoughBal = senderCommonFunc.SenderHasEnoughWalletBaltoTransfer(transactionSummaryvm.KiiPayTransferPaymentSummary.TotalAmount, transactionSummaryvm.SenderAndReceiverDetail.SenderWalletId);

                    if (hasEnoughBal == false)
                    {
                        ModelState.AddModelError("TransactionError", "Your wallet doesn't have enough balance!");
                        vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails();
                        return View(vm);
                    }
                    //Common.FaxerSession.ReceiptNo = Common.Common.GenerateCashPickUpReceiptNo(6);
                    // _senderForAllTransferServices.CompleteTransaction(transactionSummaryvm);
                    break;
                case SenderPaymentMode.MoneyFexBankAccount:
                    return RedirectToAction("MoneyFexBankDeposit");
                default:
                    break;
            }
            #region SMS to Receiver
            //SmsApi smsService = new SmsApi();

            //var ReceiverInfo = _cashPickUpServices.GetSenderCashPickUp();
            //var AmountInfo = _cashPickUpServices.GetSenderMobileEnrterAmount();
            //var senderName = Common.FaxerSession.LoggedUser.FullName;

            //string fullName = AmountInfo.ReceiverName;
            //var names = fullName.Split(' ');
            //string ReceiverFirstName = names[0];

            //var msg = smsService.GetCashPickUPReceivedMessage(senderName, ReceiverFirstName, Common.FaxerSession.MFCN, AmountInfo.SendingCurrencySymbol + " " + AmountInfo.SendingAmount,
            //    AmountInfo.SendingCurrencySymbol + " " + AmountInfo.Fee, AmountInfo.ReceivingCurrencySymbol + " " + AmountInfo.ReceivingAmount);

            ////var msg = smsService.GetCashToCashTransferMessage(senderName, Common.FaxerSession.MFCN, AmountInfo.SendingCurrencySymbol + " " + AmountInfo.SendingAmount,
            ////    AmountInfo.SendingCurrencySymbol + " " + AmountInfo.Fee, AmountInfo.ReceivingCurrencySymbol + " " + AmountInfo.ReceivingAmount);


            //var PhoneNo = Common.Common.GetCountryPhoneCode(ReceiverInfo.CountryCode) + ReceiverInfo.MobileNumber;
            //smsService.SendSMS(PhoneNo, msg);
            #endregion
            return RedirectToAction("CashPickUpSuccess");
        }


        public ActionResult DebitCreditCardDetails(bool IsAddDebitCreditCard = false, bool IsFromSavedDebitCard = false)
        {
            //var addresses = _cashPickUpServices.GetAddress();
            //ViewBag.Address = new SelectList(addresses, "Name", "Name");

            CreditDebitCardViewModel vm = new CreditDebitCardViewModel();
            vm = _cashPickUpServices.GetDebitCreditCardDetail();
            var sendingAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();

            //  Fee Added statically (Bank Fee) it will be added dynamically after payment setup 
            //Credit/Debit card 
            //Fee: GBP 0.80
            //Manual Bank Deposit
            //Fee: GBP 0.79



            vm.FaxingAmount = sendingAmountData.TotalAmount + vm.CreditDebitCardFee;
            vm.FaxingCurrencySymbol = sendingAmountData.SendingCurrencySymbol;
            vm.FaxingCurrency = sendingAmountData.SendingCurrencyCode;
            vm.SaveCard = IsAddDebitCreditCard;
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            vm.AddressLineOne = senderCommonFunc.GetSenderAddress();
            vm.ReceiverName = sendingAmountData.ReceiverName;
            ViewBag.IsFromSavedDebitCard = IsFromSavedDebitCard;
            ViewBag.HasOneSavedCard = Common.Common.HasOneCardSaved();
            ViewBag.ReceivingCountryCurrency = sendingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Cash PickUp";
            ViewBag.SendingCountryCurrency = sendingAmountData.SendingCurrencyCode;
            ViewBag.SendingAmount = sendingAmountData.SendingAmount;
            ViewBag.ReceiverName = sendingAmountData.ReceiverName;
            ViewBag.ReceivingCountry = Common.Common.GetCountryCodeByCurrency(sendingAmountData.ReceivingCurrencyCode).ToLower();
            ViewBag.Fee = sendingAmountData.Fee;

            return View(vm);
        }



        [HttpPost]
        public JsonResult ThreeDQuery([Bind(Include = CreditDebitCardViewModel.BindProperty)] CreditDebitCardViewModel vm)
        {
            var serviceResult = new ServiceResult<ThreeDRequestVm>();
            if (string.IsNullOrEmpty(vm.CardNumber))
            {

                ModelState.AddModelError("", "Enter Card Number");
                serviceResult.Data = null;
                serviceResult.Message = "Enter Card Number";
                serviceResult.Status = ResultStatus.Error;
                return Json(serviceResult);
            }

            var sendsingAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();
            var cashPickUpData = _cashPickUpServices.GetSenderCashPickUp();
            int RecipientId = _cashPickUpServices.GetRecipientId(cashPickUpData.MobileNumber, Service.CashPickUP);


            var HasExceededBankDepositReceiverLimit = Common.Common.HasExceededReceiverLimit(Common.FaxerSession.SenderId,
                  RecipientId, Common.FaxerSession.LoggedUser.CountryCode, cashPickUpData.CountryCode, TransactionTransferMethod.CashPickUp);
            if (HasExceededBankDepositReceiverLimit)
            {
                serviceResult.Data = null;
                serviceResult.Status = ResultStatus.Error;
                serviceResult.Message = "Recipient daily transaction limit exceeded";
                return Json(serviceResult, JsonRequestBehavior.AllowGet);

            }
            bool HasSenderTransacionExceedLimit = Common.Common.HasExceededSenderTransactionLimit(Common.FaxerSession.SenderId, Common.FaxerSession.LoggedUser.CountryCode,
                 cashPickUpData.CountryCode, TransactionTransferMethod.CashPickUp);
            if (HasSenderTransacionExceedLimit)
            {
                serviceResult.Data = null;
                serviceResult.Status = ResultStatus.Error;
                serviceResult.Message = "Sender daily transaction limit exceeded";
                return Json(serviceResult, JsonRequestBehavior.AllowGet);

            }
            //var HasExceededTransactionLimit = Common.Common.
            //        HasExceededAmountLimit(Common.FaxerSession.LoggedUser.Id, Common.FaxerSession.LoggedUser.CountryCode, cashPickUpData.CountryCode,
            //        vm.FaxingAmount, Module.Faxer);

            //if (HasExceededTransactionLimit)
            //{
            //    serviceResult.Data = null;
            //    serviceResult.Status = ResultStatus.Error;
            //    serviceResult.Message = "MoneyFex account daily transaction limit exceeded";
            //    return Json(serviceResult, JsonRequestBehavior.AllowGet);
            //}

            var number = vm.CardNumber.Split(' ');
            vm.CardNumber = string.Join("", number);
            var validateCardresult = Common.Common.ValidateCreditDebitCard(vm);
            if (validateCardresult.Data == false)
            {
                serviceResult.Data = null;
                serviceResult.Message = validateCardresult.Message;
                serviceResult.Status = ResultStatus.Error;
                return Json(serviceResult);
            }
            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                CardName = Common.FaxerSession.LoggedUser.FullName,
                ExpirationMonth = vm.EndMM,
                ExpiringYear = vm.EndYY,
                Number = vm.CardNumber,
                SecurityCode = vm.SecurityCode,

                billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                CurrencyCode = Common.Common.GetCurrencyCode(Common.FaxerSession.LoggedUser.CountryCode),
                Amount = vm.FaxingAmount,
                SenderId = Common.FaxerSession.LoggedUser.Id

            };

            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm, Common.FaxerSession.LoggedUser.CountryCode, cashPickUpData.CountryCode);

            string CardType = AgentSession.CardType;


            if (!StripeResult.IsValid)
            {
                serviceResult.Data = null;
                serviceResult.Message = StripeResult.Message;
                serviceResult.Status = ResultStatus.Error;
                return Json(serviceResult);
            }

            else

            {
                SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
                SSenderBankAccountDeposit _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
                _senderBankAccountDepositServices.SetDebitCreditCardDetail(vm);


                //  Fee Added statically (Bank Fee) it will be added dynamically after payment setup 
                //Credit/Debit card 
                //Fee: GBP 0.0.05
                //Manual Bank Deposit
                //Fee: GBP 0.79 
                var sendingAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();

                // paymentInfo.Fee add vairacha dai back garda 



                decimal faxingAmount = sendingAmountData.TotalAmount + new CreditDebitCardViewModel().CreditDebitCardFee;
                decimal TotalAmount = Common.Common.CreditTypeFee(CardType, faxingAmount);

                SetTransactionSummary();


                var transferSummary = _senderForAllTransferServices.GetTransactionSummary();
                transferSummary.CreditORDebitCardDetials = vm;
                transferSummary.CreditORDebitCardDetials.FaxingAmount = TotalAmount;
                transferSummary.KiiPayTransferPaymentSummary.Fee = sendingAmountData.Fee;
                transferSummary.KiiPayTransferPaymentSummary.TotalAmount = sendingAmountData.TotalAmount;
                _senderForAllTransferServices.SetTransactionSummary(transferSummary);

                
                SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
                var senderInfo = Common.FaxerSession.LoggedUser;
                StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
                {
                    Amount = TotalAmount,
                    Currency = Common.Common.GetCountryCurrency(senderInfo.CountryCode),
                    NameOnCard = "Charge for " + stripeResultIsValidCardVm.CardName,
                    StripeTokenId = StripeResult.StripeTokenId,
                    CardNum = stripeResultIsValidCardVm.Number,
                    ReceivingCountry = transferSummary.SenderAndReceiverDetail.ReceiverCountry,
                    SendingCountry = transferSummary.SenderAndReceiverDetail.SenderCountry,
                    ExipiryDate = stripeResultIsValidCardVm.ExpirationMonth + "/" + stripeResultIsValidCardVm.ExpiringYear,
                    SecurityCode = stripeResultIsValidCardVm.SecurityCode,
                    termurl = "/SenderCashPickUp/ThreeDQueryResponseCallBack",
                    billingpostcode = senderInfo.PostCode,
                    billingpremise = senderInfo.HouseNo,
                    ReceiptNo = Common.FaxerSession.ReceiptNo,
                    SenderFirstName = senderInfo.FirstName,
                    SenderLastName = Common.Common.GetSenderLastName(),
                    SenderEmail = Common.Common.GetSenderInfo(senderInfo.Id).Email,
                    SenderId = senderInfo.Id,
                    ReceivingCurrency = Common.Common.GetCountryCurrency(transferSummary.SenderAndReceiverDetail.ReceiverCountry),
                };
                try
                {
                    var resultThreedQuery = StripServices.CreateThreedQuery(stripeCreateTransaction, TransactionTransferType.Online, TransactionTransferMethod.CashPickUp);

                    serviceResult.Message = resultThreedQuery.Message;
                    serviceResult.Status = resultThreedQuery.Status;
                    serviceResult.Data = resultThreedQuery.Data;
                    serviceResult.IsGetType3dAuth = resultThreedQuery.IsGetType3dAuth;

                    if (serviceResult.Status == ResultStatus.OK)
                    {
                        vm.ThreeDEnrolled = serviceResult.Data.ThreeDEnrolled == "Y" ? true : false;
                        _senderBankAccountDepositServices.SetDebitCreditCardDetail(vm);
                    }
                    if (!vm.ThreeDEnrolled)
                    {
                        serviceResult.Data = null;
                        serviceResult.Message = resultThreedQuery.Message;
                        serviceResult.Status = ResultStatus.Error;
                        return Json(serviceResult);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex.Message, ErrorType.PaymentGateway, "SenderCashPickUp/ThreeDQuery");
                }

            }

            //return Json(new ServiceResult<ThreeDRequestVm>()
            //{
            //    Data = 

            //});
            return Json(serviceResult);

        }
        [HttpGet]
        public ActionResult ThreeDQueryResponseCallBack(string uid, string id)
        {
            try
            {
                CustomerResponseVm vm = new CustomerResponseVm();
                var result = Transact365Serivces.GetTransationDetails(uid,id);
                if (result.Status == ResultStatus.Error)
                {
                    return RedirectToAction("DebitCreditCardDetails");
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ErrorType.PaymentGateway, "SenderCashPickUp/ThreeDQueryResponseCallBack");
                return RedirectToAction("DebitCreditCardDetails");
            }
        }


        [HttpPost]
        public ActionResult ThreeDQueryResponseCallBack([Bind(Include = CustomerResponseVm.BindProperty)] CustomerResponseVm responseVm)
        {
            try
            {
                return View(responseVm);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ErrorType.PaymentGateway, "SenderCashPickUp/ThreeDQueryResponseCallBack");
                return RedirectToAction("DebitCreditCardDetails");
            }

        }

        [HttpPost]
        public ActionResult DebitCreditCardDetails([Bind(Include = CustomerResponseVm.BindProperty)] CustomerResponseVm responseVm)
        {
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            ViewBag.HasOneSavedCard = Common.Common.HasOneCardSaved();

            var transferSummary = _senderForAllTransferServices.GetTransactionSummary();
            var vm = new CreditDebitCardViewModel();
            vm = transferSummary.CreditORDebitCardDetials;
            StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
            {

                Amount = transferSummary.CreditORDebitCardDetials.FaxingAmount,
                Currency = Common.Common.GetCurrencyCode(Common.FaxerSession.LoggedUser.CountryCode),

                md = responseVm.MD,
                pares = responseVm.PaRes,
                parenttransactionreference = responseVm.parenttransactionreference,
                billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                ReceiptNo = Common.FaxerSession.ReceiptNo,
                SenderId = Common.FaxerSession.LoggedUser.Id,
                CardNum = vm.CardNumber
            };

            var cardProcessor = StripServices.GetCardProcessor(stripeCreateTransaction, TransactionTransferType.Online, TransactionTransferMethod.CashPickUp);
            transferSummary.CardProcessorApi = cardProcessor;
            _senderForAllTransferServices.SetTransactionSummary(transferSummary);

            switch (cardProcessor)
            {
                case CardProcessorApi.TrustPayment:
                    var transactionResult = StripServices.CreateTransaction(stripeCreateTransaction);

                    if (transactionResult == null)
                    {
                        ModelState.AddModelError("TransactionError", transactionResult.Message);
                        return View(vm);
                    }
                    if (transactionResult.IsValid == false)
                    {
                        transferSummary.CreditORDebitCardDetials.IsCardUsageMsg = transactionResult.IsCardUsageMsg;
                        transferSummary.CreditORDebitCardDetials.CardUsageMsg = transactionResult.CardUsageMsg;
                        transferSummary.CreditORDebitCardDetials.ErrorMsg = transactionResult.Message;
                        ModelState.AddModelError("StripeError", transactionResult.Message);
                        return View(transferSummary.CreditORDebitCardDetials);
                    }
                    break;
                case CardProcessorApi.T365:
                    break;
            }


            var SenderDocumentApprovalStatus = Common.Common.GetSenderIdentificationStatus(Common.FaxerSession.LoggedUser.Id);

            transferSummary.IsIdCheckInProgress = Common.Common.IsSenderIdCheckInProgress(Common.FaxerSession.LoggedUser.Id);
            //if (SenderDocumentApprovalStatus == null)
            //{
            //    transferSummary.IsIdCheckInProgress = false;
            //}
            //if (SenderDocumentApprovalStatus != null && SenderDocumentApprovalStatus != DocumentApprovalStatus.Approved)
            //{
            //    transferSummary.IsIdCheckInProgress = true;
            //}
            _senderForAllTransferServices.CompleteTransaction(transferSummary);

            switch (SenderDocumentApprovalStatus)
            {
                case null:
                    return RedirectToAction("IdentificationInformation", "SenderBankAccountDeposit");
                case DocumentApprovalStatus.Approved:
                    break;
                case DocumentApprovalStatus.Disapproved:
                    return RedirectToAction("IdentificationInformation", "SenderBankAccountDeposit");
                case DocumentApprovalStatus.InProgress:
                    return RedirectToAction("IdentificationInformation", "SenderBankAccountDeposit");
                    break;
                default:
                    break;
            }
            //if (SenderDocumentApprovalStatus == null || SenderDocumentApprovalStatus == DocumentApprovalStatus.Disapproved)
            //{

            //    return RedirectToAction("IdentificationInformation", "SenderBankAccountDeposit");
            //}

            return RedirectToAction("CashPickUpSuccess", "SenderCashPickUp");



        }

        //[HttpPost]
        //public ActionResult DebitCreditCardDetails([Bind(Include = CreditDebitCardViewModel.BindProperty)]CreditDebitCardViewModel model)
        //{
        //    //var addresses = _cashPickUpServices.GetAddress();
        //    //ViewBag.Address = new SelectList(addresses, "Name", "Name");

        //    var result = Common.Common.ValidateCreditDebitCard(model);
        //    if (result.Data == false)
        //    {
        //        ModelState.AddModelError("", result.Message);
        //        return View(model);
        //    }
        //    SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
        //    var transferSummary = _senderForAllTransferServices.GetTransactionSummary();
        //    SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();

        //    StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
        //    {
        //        CardName = Common.FaxerSession.LoggedUser.FullName,
        //        ExpirationMonth = model.EndMM,
        //        ExpiringYear = model.EndYY,
        //        Number = model.CardNumber,
        //        SecurityCode = model.SecurityCode,

        //    };
        //    var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);
        //    if (!StripeResult.IsValid)
        //    {
        //        ModelState.AddModelError("StripeError", StripeResult.Message);
        //    }
        //    else
        //    {
        //        StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
        //        {
        //            Amount = model.FaxingAmount,
        //            Currency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode),
        //            NameOnCard = "Charge for " + stripeResultIsValidCardVm.CardName,
        //            StripeTokenId = StripeResult.StripeTokenId,
        //            CardNum = stripeResultIsValidCardVm.Number,
        //            ReceivingCountry = transferSummary.SenderAndReceiverDetail.SenderCountry,
        //            SendingCountry = transferSummary.SenderAndReceiverDetail.ReceiverCountry,
        //            ExipiryDate = stripeResultIsValidCardVm.ExpirationMonth + "/" + stripeResultIsValidCardVm.ExpiringYear,
        //            SecurityCode = stripeResultIsValidCardVm.SecurityCode
        //        };

        //        var transactionResult = StripServices.CreateTransaction(stripeCreateTransaction);


        //        if (transactionResult.IsValid == false)
        //        {
        //            ModelState.AddModelError("StripeError", transactionResult.Message);
        //            return View(model);

        //        }
        //        if (transactionResult != null)
        //        {
        //            // KiiPay Persoanl Wallet Services 
        //            transferSummary.CreditORDebitCardDetials = model;
        //            _senderForAllTransferServices.CompleteTransaction(transferSummary);

        //            return RedirectToAction("CashPickUpSuccess", "SenderCashPickUp");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("TransactionError", transactionResult.Message);
        //        }
        //        return View(model);
        //    }
        //    return View(model);
        //}

        public JsonResult IsCrebitCard(string cardNumber, string month, string year, string securityCode)
        {
            try
            {
                var number = cardNumber.Split(' ');
                cardNumber = string.Join("", number);
                StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
                {
                    CardName = Common.FaxerSession.LoggedUser.FullName,
                    ExpirationMonth = month,
                    ExpiringYear = year,
                    Number = cardNumber,
                    SecurityCode = securityCode,
                    billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                    billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                    CurrencyCode = Common.Common.GetCurrencyCode(Common.FaxerSession.LoggedUser.CountryCode),
                    Amount = _cashPickUpServices.GetSenderMobileEnrterAmount().TotalAmount,
                    IsCreditDebitCardCheck = true,
                    SenderId = Common.FaxerSession.LoggedUser.Id
                };
                var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);

                decimal amount = _cashPickUpServices.GetSenderMobileEnrterAmount().TotalAmount;
                amount = (decimal)Math.Round((0.601 / 100), 2) * amount;
                return Json(new
                {
                    IsCrebitCard = StripeResult.IsCreditCard,
                    ExtraAmount = Common.Common.GetCurrencySymbol(Common.FaxerSession.LoggedUser.CountryCode) + amount
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    IsCrebitCard = false,
                    ExtraAmount = 0
                }, JsonRequestBehavior.AllowGet);
            }

        }




        public ActionResult MoneyFexBankDeposit()
        {
            SenderMoneyFexBankDepositVM vm = new SenderMoneyFexBankDepositVM();

            var sendingAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();


            ViewBag.ReceivingCountryCurrency = sendingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Cash PickUp";
            ViewBag.SendingCountryCurrency = sendingAmountData.SendingCurrencyCode;
            ViewBag.SendingAmount = sendingAmountData.SendingAmount;
            ViewBag.ReceiverName = sendingAmountData.ReceiverName;
            ViewBag.ReceivingCountry = Common.Common.GetCountryCodeByCurrency(sendingAmountData.ReceivingCurrencyCode).ToLower();
            ViewBag.Fee = sendingAmountData.Fee;



            //  Fee Added statically (Bank Fee) it will be added dynamically after payment setup 
            //Credit/Debit card 
            //Fee: GBP 0.80
            //Manual Bank Deposit
            //Fee: GBP 0.79


            decimal addedBankFee = sendingAmountData.TotalAmount + vm.BankFee;
            vm.Amount = addedBankFee;
            vm.SendingCurrencyCode = sendingAmountData.SendingCurrencyCode;
            vm.SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol;
            _cashPickUpServices.SetMoneyFexBankAccountDeposit(vm);
            vm = _cashPickUpServices.GetMoneyFexBankAccountDeposit();

            return View(vm);
        }

        [HttpPost]
        public ActionResult MoneyFexBankDeposit([Bind(Include = SenderMoneyFexBankDepositVM.BindProperty)] SenderMoneyFexBankDepositVM model)
        {

            if (ModelState.IsValid)
            {
                SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

                var transferSummary = _senderForAllTransferServices.GetTransactionSummary();

                var sendingAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();
                sendingAmountData.Fee = sendingAmountData.Fee + 0.79M;
                sendingAmountData.TotalAmount = sendingAmountData.SendingAmount + sendingAmountData.Fee;
                transferSummary.KiiPayTransferPaymentSummary.Fee = sendingAmountData.Fee;
                transferSummary.KiiPayTransferPaymentSummary.TotalAmount = sendingAmountData.TotalAmount;

                var moneyFexBankAccountDeposit = _cashPickUpServices.GetMoneyFexBankAccountDeposit();
                moneyFexBankAccountDeposit.HasMadePaymentToBankAccount = model.HasMadePaymentToBankAccount;
                //_cashPickUpServices.SetMoneyFexBankAccountDeposit(moneyFexBankAccountDeposit);
                SetTransactionSummary();

                var SenderDocumentApprovalStatus = Common.Common.GetSenderIdentificationStatus(Common.FaxerSession.LoggedUser.Id);

                transferSummary.IsIdCheckInProgress = Common.Common.IsSenderIdCheckInProgress(Common.FaxerSession.LoggedUser.Id);
                //if (SenderDocumentApprovalStatus != null && SenderDocumentApprovalStatus == DocumentApprovalStatus.Approved)
                //{


                //    transferSummary.IsIdCheckInProgress = false;
                //}

                _senderForAllTransferServices.CompleteTransaction(transferSummary);

                switch (SenderDocumentApprovalStatus)
                {
                    case null:
                        return RedirectToAction("IdentificationInformation", "SenderBankAccountDeposit");
                    case DocumentApprovalStatus.Approved:
                        break;
                    case DocumentApprovalStatus.Disapproved:
                        return RedirectToAction("IdentificationInformation", "SenderBankAccountDeposit");
                    case DocumentApprovalStatus.InProgress:
                        return RedirectToAction("IdentificationInformation", "SenderBankAccountDeposit");
                        break;
                    default:
                        break;
                }
                //if (SenderDocumentApprovalStatus != DocumentApprovalStatus.Approved && SenderDocumentApprovalStatus != DocumentApprovalStatus.InProgress)
                //{


                //    return RedirectToAction("IdentificationInformation", "SenderBankAccountDeposit");
                //}

                return RedirectToAction("Success", "MoneyFexBankDeposit");

            }
            return View(model);
        }


        public ActionResult CashPickUpSuccess()
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();

            // Data to show on success
            SenderCashPickUpSuccessVm vm = new SenderCashPickUpSuccessVm();
            var sendingAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();
            var cashPickUp = _cashPickUpServices.GetSenderCashPickUp();

            vm.SentAmount = sendingAmountData.SendingAmount;
            vm.SendingCurrency = sendingAmountData.SendingCurrencySymbol;
            vm.ReceiverName = cashPickUp.FullName;

            //this number 
            vm.MFCNNumber = Common.FaxerSession.MFCN;
            Common.FaxerSession.SenderCashPickUp = null;
            senderCommonFunc.ClearCashPickUpSession();

            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Payment Confirmation");
            return View(vm);


        }

        public void SetTransactionSummary()
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

            var sendingAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();
            var cashPickUp = _cashPickUpServices.GetSenderCashPickUp();

            //Completing Transaction
            var loggedUserData = _cashPickUpServices.GetLoggedUserData();
            var paymentMethod = _cashPickUpServices.GetPaymentMethod();

            TransactionSummaryVM transactionSummaryVm = new TransactionSummaryVM();
            transactionSummaryVm.CashPickUpVM = cashPickUp;

            int senderWalletId = 0;
            var senderWalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
            if (senderWalletInfo != null)
            {

                senderWalletId = senderWalletInfo.Id;
            }
            transactionSummaryVm.SenderAndReceiverDetail = new SenderAndReceiverDetialVM()
            {
                SenderId = loggedUserData.Id,
                SenderCountry = loggedUserData.CountryCode,
                ReceiverCountry = cashPickUp.CountryCode,
                ReceiverId = cashPickUp.RecentReceiverId == null ? 0 : (int)cashPickUp.RecentReceiverId,
                SenderWalletId = senderWalletId,
                ReceivingCurrency = cashPickUp.ReceivingCurrency,
                SendingCurrency = cashPickUp.SendingCurrency
            };

            //Set Sms Fee 
            transactionSummaryVm.KiiPayTransferPaymentSummary = new KiiPayTransferPaymentSummary()
            {
                ReceiverName = cashPickUp.FullName,
                SendingCurrency = sendingAmountData.SendingCurrencyCode,
                SendingAmount = sendingAmountData.SendingAmount,
                ReceivingAmount = sendingAmountData.ReceivingAmount,
                TotalAmount = sendingAmountData.TotalAmount,
                ExchangeRate = sendingAmountData.ExchangeRate,
                Fee = sendingAmountData.Fee,
                PaymentReference = "",
                ReceivingCurrency = sendingAmountData.ReceivingCurrencyCode,
                ReceivingCurrencySymbol = sendingAmountData.ReceivingCurrencySymbol,
                SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
                SendSMS = true,
                SMSFee = 0,
            };

            transactionSummaryVm.PaymentMethodAndAutoPaymentDetail = new PaymentMethodViewModel()
            {
                TotalAmount = sendingAmountData.TotalAmount,
                SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
                SenderPaymentMode = paymentMethod.SenderPaymentMode,
                EnableAutoPayment = false
            };

            //For DebitCreditCardDetail
            var debitCreditCardDetail = _cashPickUpServices.GetDebitCreditCardDetail();

            transactionSummaryVm.CreditORDebitCardDetials = debitCreditCardDetail;

            var moneyFexBankAccountDepositData = _cashPickUpServices.GetMoneyFexBankAccountDeposit();
            transactionSummaryVm.MoneyFexBankDeposit = moneyFexBankAccountDepositData;

            transactionSummaryVm.TransferType = TransferType.CashPickup;
            if (transactionSummaryVm.SenderAndReceiverDetail.SenderCountry == transactionSummaryVm.SenderAndReceiverDetail.ReceiverCountry)
            {
                transactionSummaryVm.IsLocalPayment = true;
            }
            else
            {
                transactionSummaryVm.IsLocalPayment = false;
            }
            _senderForAllTransferServices.SetTransactionSummary(transactionSummaryVm);
        }


        [HttpGet]
        public JsonResult IsCVVCodeValid(string securityCode = "", int cardId = 0)
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var sendsingAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();
            var cashPickUpData = _cashPickUpServices.GetSenderCashPickUp();
            int RecipientId = _cashPickUpServices.GetRecipientId(cashPickUpData.MobileNumber, Service.CashPickUP);
            bool HasExceededMobileWalletReceiverLimit = Common.Common.HasExceededReceiverLimit(Common.FaxerSession.SenderId,
                                                       RecipientId, Common.FaxerSession.LoggedUser.CountryCode,
                                                  cashPickUpData.CountryCode, TransactionTransferMethod.CashPickUp);
            if (HasExceededMobileWalletReceiverLimit)
            {
                return Json(new ServiceResult<CreditDebitCardViewModel>()
                {
                    Data = new CreditDebitCardViewModel(),
                    Status = ResultStatus.Error,
                    Message = "Recipient daily transaction limit exceeded"
                }, JsonRequestBehavior.AllowGet);
            }
            bool HasSenderTransacionExceedLimit = Common.Common.HasExceededSenderTransactionLimit(Common.FaxerSession.SenderId,
                Common.FaxerSession.LoggedUser.CountryCode, cashPickUpData.CountryCode, TransactionTransferMethod.CashPickUp);
            if (HasSenderTransacionExceedLimit)
            {
                return Json(new ServiceResult<CreditDebitCardViewModel>()
                {
                    Data = new CreditDebitCardViewModel(),
                    Status = ResultStatus.Error,
                    Message = "Sender daily transaction limit exceeded"
                }, JsonRequestBehavior.AllowGet);
            }
            PaymentMethodViewModel vm = new PaymentMethodViewModel()
            {
                HasEnableMoneyFexBankAccount = senderCommonFunc.IsEnabledMoneyFexbankAccount(Common.FaxerSession.LoggedUser.CountryCode),
                SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard,
            };
            _cashPickUpServices.SetPaymentMethod(vm);
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            SetTransactionSummary();
            var transactionSummaryvm = _senderForAllTransferServices.GetTransactionSummary();
            SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
            var cardInfo = _saveCreditDebitCard.GetCardInfo(cardId);
            transactionSummaryvm.CreditORDebitCardDetials = new CreditDebitCardViewModel()
            {
                CardNumber = cardInfo.Num.Decrypt(),
                NameOnCard = cardInfo.CardName.Decrypt(),
                EndMM = cardInfo.EMonth.Decrypt(),
                EndYY = cardInfo.EYear.Decrypt(),
                SecurityCode = securityCode
            };
            var result = _senderForAllTransferServices.ValidateTransactionUsingStripe(transactionSummaryvm, Common.FaxerSession.LoggedUser.CountryCode, cashPickUpData.CountryCode);
            if (result.IsValid == false)
            {
                return Json(new ServiceResult<CreditDebitCardViewModel>()
                {
                    Data = new CreditDebitCardViewModel(),
                    Status = ResultStatus.Error,
                    Message = result.Message
                }, JsonRequestBehavior.AllowGet);
            }
            var validateCardresult = Common.Common.ValidateCreditDebitCard(transactionSummaryvm.CreditORDebitCardDetials);
            if (validateCardresult.Data == false)
            {
                return Json(new ServiceResult<CreditDebitCardViewModel>()
                {
                    Data = new CreditDebitCardViewModel(),
                    Status = ResultStatus.Error,
                    Message = validateCardresult.Message
                }, JsonRequestBehavior.AllowGet);

            }
            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                CardName = Common.FaxerSession.LoggedUser.FullName,
                ExpirationMonth = transactionSummaryvm.CreditORDebitCardDetials.EndMM,
                ExpiringYear = transactionSummaryvm.CreditORDebitCardDetials.EndYY,
                Number = transactionSummaryvm.CreditORDebitCardDetials.CardNumber,
                SecurityCode = transactionSummaryvm.CreditORDebitCardDetials.SecurityCode,

                billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                CurrencyCode = Common.Common.GetCurrencyCode(Common.FaxerSession.LoggedUser.CountryCode),
                Amount = transactionSummaryvm.CreditORDebitCardDetials.FaxingAmount,
                SenderId = Common.FaxerSession.LoggedUser.Id
            };

            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm, Common.FaxerSession.LoggedUser.CountryCode, cashPickUpData.CountryCode);
            if (StripeResult.IsValid == false)
            {
                return Json(new ServiceResult<CreditDebitCardViewModel>()
                {
                    Data = new CreditDebitCardViewModel(),
                    Status = ResultStatus.Error,
                    Message = StripeResult.Message
                }, JsonRequestBehavior.AllowGet);
            }
            _cashPickUpServices.SetDebitCreditCardDetail(transactionSummaryvm.CreditORDebitCardDetials);
            return Json(new ServiceResult<CreditDebitCardViewModel>()
            {
                Data = transactionSummaryvm.CreditORDebitCardDetials,
                Status = ResultStatus.OK,
                Message = "Success"
            }, JsonRequestBehavior.AllowGet);
        }

        //For Payment Summary

        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount, bool IsReceivingAmount)
        {

            //IsReceivingAmount
            var enterAmountData = _cashPickUpServices.GetSenderMobileEnrterAmount();
            var loggedInSenderData = _cashPickUpServices.GetLoggedUserData();
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
            if (IsReceivingAmount == true)
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

            _cashPickUpServices.SetSenderMobileEnrterAmount(enterAmountData);
            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount = result.FaxingAmount

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReceiverInformation(int receiverId)
        {

            var receiverData = _cashPickUpServices.GetReceiverInformationFromReceiverId(receiverId);

            return Json(new
            {
                FullName = receiverData.FullName,
                Country = receiverData.CountryCode.ToUpper(),
                MobileNumber = receiverData.MobileNumber,
                EmailAddress = receiverData.EmailAddress,
                CountryPhoneCode = Common.Common.GetCountryPhoneCode(receiverData.CountryCode),
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCountryPhoneCode(string CountryCode)
        {


            string CountryPhoneCode = Common.Common.GetCountryPhoneCode(CountryCode);
            return Json(new
            {

                CountryPhoneCode = CountryPhoneCode
            }, JsonRequestBehavior.AllowGet);
        }
    }
}