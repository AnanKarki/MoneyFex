using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.MoblieTransferApi.Models;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.SecureTradingPaymentGateway;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransferZero.Sdk.Model;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Controllers
{
    public class SenderMobileMoneyTransferController : Controller
    {

        SSenderMobileMoneyTransfer _mobileMoneyTransferServices = null;
        SenderCommonFunc senderCommonFunc = null;
        SSenderKiiPayWalletTransfer _kiiPaytrasferServices = null;
        SSenderBankAccountDeposit _senderBankAccountDepositServices = null;

        public SenderMobileMoneyTransferController()
        {
            _mobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            senderCommonFunc = new SenderCommonFunc();
            _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
        }

        // GET: SenderMobileMoneyTransfer

        #region International Mobile Money Transfer
        private List<DropDownViewModel> GetRecentPaidReceivers(int senderId, DB.Module module, int WalletId, string Country)
        {


            var result = _mobileMoneyTransferServices.GetRecentlyPaidNumbers(senderId, module, WalletId).
                Where(x => x.CountryCode == Country).ToList();
            return result;

        }
        private List<DropDownViewModel> GetWallets(string countryCode)
        {

            var result = _mobileMoneyTransferServices.GetWallets().Where(x => x.CountryCode == countryCode).ToList();
            return result;

        }

        private void SetViewBagForEnterReceiverPageSummary()
        {

            ViewBag.ReceivingCountryCurrency = _kiiPaytrasferServices.GetCommonEnterAmount().ReceivingCurrency;
            ViewBag.TransferMethod = "Mobile Wallet";
            ViewBag.SendingCountryCurrency = _kiiPaytrasferServices.GetCommonEnterAmount().SendingCurrency;
            ViewBag.SendingAmount = _kiiPaytrasferServices.GetCommonEnterAmount().SendingAmount;
            ViewBag.ReceivingCountry = _kiiPaytrasferServices.GetCommonEnterAmount().ReceivingCountryCode.ToLower();
        }

        public ActionResult Index(string CountryCode = "", int WalletId = 0)
        {

            string RepeatedTransactionCountry = CountryCode;
            var country = Common.Common.GetCountries();
            int senderId = Common.FaxerSession.LoggedUser.Id;
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");
            ViewBag.RecentlyPaidNumbers = new SelectList(GetRecentPaidReceivers(senderId, Module.Faxer
                                                         , WalletId, CountryCode), "Code", "Name");

            SenderMobileMoneyTransferVM vm = new SenderMobileMoneyTransferVM();

            vm = _mobileMoneyTransferServices.GetSenderMobileMoneyTransfer();
            if (Common.FaxerSession.IsTransferFromHomePage || Common.FaxerSession.IsCommonEstimationPage)
            {

                vm.CountryCode = _kiiPaytrasferServices.GetCommonEnterAmount().ReceivingCountryCode;
                CountryCode = vm.CountryCode;
                SetViewBagForEnterReceiverPageSummary();

            }
            if (String.IsNullOrEmpty(CountryCode))
            {
                CountryCode = RepeatedTransactionCountry;
            }

            vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(CountryCode);
            ViewBag.Wallets = new SelectList(GetWallets(CountryCode), "Id", "Name", WalletId);

            var amountSummary = _kiiPaytrasferServices.GetCommonEnterAmount(); // GetSummary set in Session 
            if (Common.FaxerSession.IsTransferFromHomePage
        && amountSummary.SendingCountryCode != Common.FaxerSession.LoggedUser.CountryCode)
            {

                return RedirectToAction("Index", "SenderTransferMoneyNow");
            }


            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = SenderMobileMoneyTransferVM.BindProperty)]SenderMobileMoneyTransferVM model)
        {
            var country = Common.Common.GetCountries();
            int senderId = Common.FaxerSession.LoggedUser.Id;

            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName", model.CountryCode);
            ViewBag.RecentlyPaidNumbers = new SelectList(GetRecentPaidReceivers(senderId, DB.Module.Faxer, model.Id, model.CountryCode),
                "Code", "Name", model.RecentlyPaidMobile);
            ViewBag.Wallets = new SelectList(GetWallets(model.CountryCode), "Id", "Name", model.WalletId);
            model.CountryPhoneCode = Common.Common.GetCountryPhoneCode(model.CountryCode);
            SetViewBagForEnterReceiverPageSummary();
            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();

            if (ModelState.IsValid)
            {
                bool IsValidReceiver = Common.Common.IsValidBankDepositReceiver(model.MobileNumber, Service.MobileWallet);

                if (IsValidReceiver == false)
                {
                    ModelState.AddModelError("InvalidReceiver", " Receiver is banned");
                    return View(model);
                }

                _mobileMoneyTransferServices.SetSenderMobileMoneyTransfer(model);
                decimal SendingAmount = paymentInfo.SendingAmount;
                decimal ReceivingAmount = paymentInfo.ReceivingAmount;


                var Apiservice = Common.Common.GetApiservice(Common.FaxerSession.LoggedUser.CountryCode, model.CountryCode,
                   SendingAmount, TransactionTransferMethod.OtherWallet, TransactionTransferType.Online);


                if (Apiservice == null)
                {
                    ModelState.AddModelError("ServiceNotAvialable", "Service Not Avialable");
                    return View(model);

                }
                if (Apiservice == DB.Apiservice.MTN)
                {
                    if (ReceivingAmount > 1000000)
                    {
                        ModelState.AddModelError("", "Transaction limit exceeded");
                        return View(model);
                    }
                }
                if (Apiservice == DB.Apiservice.FlutterWave)
                {
                    ViewBag.isFlutterwaveApi = true;
                    if (string.IsNullOrEmpty(model.ReceiverEmail))
                    {
                        ModelState.AddModelError("ReceiverEmail", "Enter email");
                        return View(model);
                    }
                }
                #region old
                //switch (Apiservice)
                //{
                //    case DB.Apiservice.MTN:

                //        FAXER.PORTAL.MoblieTransferApi.MobileTransferApi mobileTransferApiServices = new FAXER.PORTAL.MoblieTransferApi.MobileTransferApi();

                //        MobileTransferApiConfigurationVm configurationVm = new MobileTransferApiConfigurationVm()
                //        {
                //            apirefId = Common.Common.GetNewRefIdForMobileTransfer(),
                //            apiKey = "",
                //            apiUrl = "",
                //            //subscriptionKey = "9277bd87e874418e928cfdba3032b423"
                //            subscriptionKey = Common.Common.GetAppSettingValue("MTNApiSubscriptionKey")
                //        };

                //        var accesstoken = mobileTransferApiServices.Login<MobileTransferAccessTokeneResponse>(configurationVm);

                //        MobileTransferAccessTokeneResponse tokenModel = new MobileTransferAccessTokeneResponse();
                //        tokenModel = accesstoken.Result;
                //        tokenModel.apirefId = configurationVm.apirefId;
                //        tokenModel.apiKey = configurationVm.apiKey;
                //        tokenModel.apiUrl = "";
                //        tokenModel.subscriptionKey = configurationVm.subscriptionKey;

                //        MobileNoLookUpResponseData mobileNoLookUp = new MobileNoLookUpResponseData()
                //        {
                //            accountHolderIdType = "MSISDN",
                //            accountHolderId = model.CountryPhoneCode + "" + model.MobileNumber
                //        };
                //        SAgentMobileTransferWallet _sAgentMobileTransferWalletServices = new SAgentMobileTransferWallet();
                //        _sAgentMobileTransferWalletServices.SetMobileTransferAccessTokeneResponse(tokenModel);
                //        break;

                //    case DB.Apiservice.TransferZero:
                //        TransferZeroApi transferZeroApi = new TransferZeroApi();
                //        if (model.CountryCode == "GH" || model.CountryCode == "NG")
                //        {
                //            string[] phoneCode = model.CountryPhoneCode.Split('+');
                //            string receiverPhoneCode = phoneCode[phoneCode.Length - 1];
                //            AccountValidationRequest accountValidationRequest = new AccountValidationRequest(
                //                            phoneNumber: receiverPhoneCode + model.MobileNumber,
                //                            country: (AccountValidationRequest.CountryEnum)Common.Common.getAccountValidationCountryCodeForTZ(model.CountryCode),
                //                            currency: (AccountValidationRequest.CurrencyEnum)Common.Common.getAccountValidationCountryCurrencyForTZ(model.CountryCode),
                //                            method: AccountValidationRequest.MethodEnum.Mobile
                //             );
                //            var result = transferZeroApi.ValidateAccountNo(accountValidationRequest);
                //            var IsValidateAccount = false;
                //            try
                //            {

                //                IsValidateAccount = result.Meta == null ? true : false;
                //            }
                //            catch (Exception)
                //            {

                //            }
                //            if (IsValidateAccount == false)
                //            {
                //                ModelState.AddModelError("", "Enter Valid Number");
                //                return View(model);
                //            }
                //        }
                //        break;
                //    case DB.Apiservice.EmergentApi:
                //        EmergentApi emergentApiServices = new EmergentApi();
                //        EmergentApiMobileMoneyCustomerCheck customerCheck = new EmergentApiMobileMoneyCustomerCheck() {
                //            mobile = model.CountryPhoneCode + model.MobileNumber
                //        };
                //        var emergentApiResponse = emergentApiServices.IsValidMobileCustomer(customerCheck);
                //        if (emergentApiResponse.Data == false)
                //        {
                //            ModelState.AddModelError("", emergentApiResponse.Message);
                //            return View(model);
                //        }
                //        break;
                //    default:
                //        break;
                //}
                #endregion

                SmsApi smsApi = new SmsApi();
                var IsValidMobileNo = smsApi.IsValidMobileNo(model.CountryPhoneCode + "" + model.MobileNumber);
                if (IsValidMobileNo == false)
                {
                    ModelState.AddModelError("InvalidNumber", "Enter Valid Number");
                    return View(model);
                }
                var IsValidAccount = _mobileMoneyTransferServices.IsValidMobileAccount(model, SendingAmount, Common.FaxerSession.LoggedUser.CountryCode);

                if (IsValidAccount.Data == false)
                {

                    ModelState.AddModelError("", IsValidAccount.Message);
                    return View(model);

                }



                int RecipientId = _mobileMoneyTransferServices.GetRecipientId(model.WalletId, model.MobileNumber);
                bool HasExceededMobileWalletReceiverLimit = Common.Common.HasExceededReceiverLimit(Common.FaxerSession.SenderId, RecipientId, Common.FaxerSession.LoggedUser.CountryCode,
                    model.CountryCode, TransactionTransferMethod.OtherWallet);
                if (HasExceededMobileWalletReceiverLimit)
                {
                    ModelState.AddModelError("", "Transaction for Recipient limit exceeded");
                    return View(model);
                }
                bool HasSenderTransacionExceedLimit = Common.Common.HasExceededSenderTransactionLimit(Common.FaxerSession.SenderId,
                    Common.FaxerSession.LoggedUser.CountryCode, model.CountryCode, TransactionTransferMethod.OtherWallet);
                if (HasSenderTransacionExceedLimit)
                {
                    ModelState.AddModelError("", "Sender daily transaction limit exceeded");
                    return View(model);
                }

                // Update Payment Summary Session 
                _kiiPaytrasferServices.setPaymentSummary(TransactionTransferMethod.OtherWallet);
                paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();
                //SetData in EnterAmount Session

                var paymentSummary = _kiiPaytrasferServices.GetCommonEnterAmount();
                SenderMobileEnrterAmountVm mobileEnter = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();

                var loggedInSenderData = _mobileMoneyTransferServices.GetLoggedUserData();
                mobileEnter.ReceiverName = model.ReceiverName;
                mobileEnter.SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol;
                mobileEnter.SendingCurrencyCode = paymentInfo.SendingCurrency;
                mobileEnter.ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol;
                mobileEnter.ReceivingCurrencyCode = paymentInfo.ReceivingCurrency;
                //mobileEnter.ExchangeRate = Common.Common.GetExchangeRate(loggedInSenderData.CountryCode, model.CountryCode);
                mobileEnter.SendingCountryCode = loggedInSenderData.CountryCode;
                mobileEnter.ReceivingCountryCode = model.CountryCode;
                //SSenderForAllTransfer.SetPaymentSummarySession(model.CountryCode);
                _mobileMoneyTransferServices.SetSenderMobileEnrterAmount(mobileEnter);
                return RedirectToAction("MobileSummaryAbroad", "SenderMobileMoneyTransfer");
            }
            return View(model);
        }

        public ActionResult MobileEnterAmount()
        {

            SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm();

            var loggedInSenderData = _mobileMoneyTransferServices.GetLoggedUserData();

            vm = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();
            if (Common.FaxerSession.IsTransferFromHomePage == true)
            {


                return RedirectToAction("MobileSummaryAbroad");
            }
            //vm.ReceiverName = enterAmountData.ReceiverName;
            //vm.SendingCurrencySymbol = enterAmountData.SendingCurrencySymbol;
            //vm.SendingCurrencyCode = enterAmountData.SendingCurrencyCode;
            //vm.ReceivingCurrencySymbol = enterAmountData.ReceivingCurrencySymbol;
            //vm.ReceivingCurrencyCode = enterAmountData.ReceivingCurrencyCode;
            //vm.ExchangeRate = enterAmountData.ExchangeRate;
            //_mobileMoneyTransferServices.SetSenderMobileEnrterAmount(vm);
            return View(vm);
        }

        [HttpPost]
        public ActionResult MobileEnterAmount([Bind(Include = SenderMobileEnrterAmountVm.BindProperty)]SenderMobileEnrterAmountVm model)
        {
            if (ModelState.IsValid)
            {
                _mobileMoneyTransferServices.SetSenderMobileEnrterAmount(model);

                if (Common.FaxerSession.IsTransferFromHomePage == false)
                {

                    return RedirectToAction("InternationalPayment");
                }
                return RedirectToAction("MobileSummaryAbroad", "SenderMobileMoneyTransfer");
            }

            return View(model);
        }

        public ActionResult MobileSummaryAbroad()
        {

            SenderTransferSummaryVm vm = new SenderTransferSummaryVm();
            if (Common.FaxerSession.TransactionId != 0)
            {
                var setAmountModel = _kiiPaytrasferServices.GetCommonEnterAmount();
                SenderMobileEnrterAmountVm amountVm = new SenderMobileEnrterAmountVm()
                {
                    ReceivingAmount = setAmountModel.ReceivingAmount,
                    ExchangeRate = setAmountModel.ExchangeRate,
                    Fee = setAmountModel.Fee,
                    ReceivingCountryCode = setAmountModel.ReceivingCountryCode,
                    ReceivingCurrencyCode = setAmountModel.ReceivingCurrency,
                    ReceivingCurrencySymbol = setAmountModel.ReceivingCurrencySymbol,
                    SendingAmount = setAmountModel.SendingAmount,
                    SendingCountryCode = setAmountModel.SendingCountryCode,
                    SendingCurrencyCode = setAmountModel.SendingCurrency,
                    SendingCurrencySymbol = setAmountModel.SendingCurrencySymbol,
                    TotalAmount = setAmountModel.TotalAmount,
                };
                _mobileMoneyTransferServices.SetSenderMobileEnrterAmount(amountVm);
                _mobileMoneyTransferServices.setAmount(Common.FaxerSession.TransactionId);

            }
            if (Common.FaxerSession.RecipientId != 0)
            {
                _mobileMoneyTransferServices.setReciverInfo(Common.FaxerSession.RecipientId);

            }

            var enterAmount = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();
            var mobileTransfer = _mobileMoneyTransferServices.GetSenderMobileMoneyTransfer();
            ViewBag.ReceivingCountryCurrency = enterAmount.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Mobile Wallet";
            ViewBag.SendingCountryCurrency = enterAmount.SendingCurrencyCode;
            ViewBag.SendingAmount = enterAmount.SendingAmount;
            ViewBag.ReceiverName = enterAmount.ReceiverName;

            ViewBag.ReceivingCountry = enterAmount.ReceivingCountryCode.ToLower();
            string fullName = enterAmount.ReceiverName;
            string firstName = "";
            try
            {
                var names = fullName.Split(' ');
                firstName = names[0];
            }
            catch (Exception ex) { 
            
            }
            ViewBag.ReceiverFirstName = firstName;

            vm.ReceiverName = enterAmount.ReceiverName;
            vm.Amount = enterAmount.SendingAmount;
            vm.Fee = enterAmount.Fee;
            vm.ReceivedAmount = enterAmount.ReceivingAmount;
            vm.SendingCurrencySymbol = enterAmount.SendingCurrencySymbol;
            vm.SendingCurrencyCode = enterAmount.SendingCurrencyCode;
            vm.ReceivingCurrencyCode = enterAmount.ReceivingCurrencyCode;
            vm.ReceivingCurrencySymbol = enterAmount.ReceivingCurrencySymbol;
            vm.PaidAmount = enterAmount.TotalAmount;
            //reference not available


            return View(vm);
        }

        [HttpPost]
        public ActionResult MobileSummaryAbroad([Bind(Include = SenderTransferSummaryVm.BindProperty)]SenderTransferSummaryVm model)
        {
            bool Isvalid = true;

            var paymentSummary = _kiiPaytrasferServices.GetCommonEnterAmount();
            var Mobiletransfer = _mobileMoneyTransferServices.SaveIncompleteTransaction();
            Common.FaxerSession.TransactionId = Mobiletransfer.Id;


            Common.FaxerSession.IsTransactionOnpending = true;

            TransactionPendingViewModel transactionPending = new TransactionPendingViewModel()
            {
                TransactionId = Mobiletransfer.Id,
                IsTransactionPending = true,
                TransferMethod = TransactionServiceType.MobileWallet,
                Fee = Mobiletransfer.Fee,
                ExchangeRate = Mobiletransfer.ExchangeRate,
                ReceiverFullName = Mobiletransfer.ReceiverName,
                ReceiptNumber = Mobiletransfer.ReceiptNo,
                ReceivingCountry = Common.Common.GetCountryName(Mobiletransfer.ReceivingCountry),
                Receivingurrency = paymentSummary.ReceivingCurrency, //Common.Common.GetCountryCurrency(Mobiletransfer.ReceivingCountry),
                SenderId = Mobiletransfer.SenderId,
                SendingAmount = Mobiletransfer.SendingAmount,
                SendingCurrency = paymentSummary.SendingCurrency, //Common.Common.GetCountryCurrency(Mobiletransfer.SendingCountry),
                TransactionNumber = Mobiletransfer.ReceiptNo,
                WalletName = Common.Common.GetMobileWalletInfo(Mobiletransfer.WalletOperatorId).Name,
                MobileNo = Mobiletransfer.PaidToMobileNo
            };

            _mobileMoneyTransferServices.SetTransactionPendingViewModel(transactionPending);

            if (Common.FaxerSession.TransactionId == 0 && Common.FaxerSession.RecipientId == 0)
            {

            }
            else
            {
                //   Isvalid = _mobileMoneyTransferServices.RepeatTransaction(Common.FaxerSession.TransactionId, Common.FaxerSession.RecipientId);
            }
            if (!Isvalid)
            {
                string ErrorMessage = Common.FaxerSession.ErrorMessage;
                ModelState.AddModelError("", ErrorMessage);

                var enterAmount = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();
                var mobileTransfer = _mobileMoneyTransferServices.GetSenderMobileMoneyTransfer();
                ViewBag.ReceivingCountryCurrency = enterAmount.ReceivingCurrencyCode;
                ViewBag.TransferMethod = "Mobile Wallet";
                ViewBag.SendingCountryCurrency = enterAmount.SendingCurrencyCode;
                ViewBag.SendingAmount = enterAmount.SendingAmount;
                ViewBag.ReceiverName = enterAmount.ReceiverName;

                ViewBag.ReceivingCountry = enterAmount.ReceivingCountryCode.ToLower();
                string fullName = enterAmount.ReceiverName;
                var names = fullName.Split(' ');
                string firstName = names[0];
                ViewBag.ReceiverFirstName = firstName;

                model.ReceiverName = enterAmount.ReceiverName;
                model.Amount = enterAmount.SendingAmount;
                model.Fee = enterAmount.Fee;
                model.ReceivedAmount = enterAmount.ReceivingAmount;
                model.SendingCurrencySymbol = enterAmount.SendingCurrencySymbol;
                model.SendingCurrencyCode = enterAmount.SendingCurrencyCode;
                model.ReceivingCurrencyCode = enterAmount.ReceivingCurrencyCode;
                model.ReceivingCurrencySymbol = enterAmount.ReceivingCurrencySymbol;
                model.PaidAmount = enterAmount.TotalAmount;
                return View(model);
            }
            return RedirectToAction("InternationalPayment", "SenderMobileMoneyTransfer");
        }


        public ActionResult InternationalPayment()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var vm = _mobileMoneyTransferServices.GetPaymentMethod();
            var transferMobileData = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();
            vm.TotalAmount = transferMobileData.TotalAmount;
            vm.SendingCurrencySymbol = transferMobileData.SendingCurrencySymbol;

            ViewBag.ReceivingCountryCurrency = transferMobileData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Mobile Wallet";
            ViewBag.SendingCountryCurrency = transferMobileData.SendingCurrencyCode;
            ViewBag.SendingAmount = transferMobileData.SendingAmount;
            ViewBag.ReceiverName = transferMobileData.ReceiverName;
            ViewBag.Fee = transferMobileData.Fee;

            ViewBag.ReceivingCountry = transferMobileData.ReceivingCountryCode.ToLower();


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
        public ActionResult InternationalPayment([Bind(Include = PaymentMethodViewModel.BindProperty)]PaymentMethodViewModel vm)
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var enterAmount = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();

            var mobileTransfer = _mobileMoneyTransferServices.GetSenderMobileMoneyTransfer();
            ViewBag.ReceivingCountryCurrency = enterAmount.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Mobile Wallet";
            ViewBag.SendingCountryCurrency = enterAmount.SendingCurrencyCode;
            ViewBag.SendingAmount = enterAmount.SendingAmount;
            ViewBag.ReceiverName = enterAmount.ReceiverName;
            ViewBag.ReceivingCountry = enterAmount.ReceivingCountryCode.ToLower();
            ViewBag.Fee = enterAmount.Fee;

            //Credit/Debit card 
            //Fee: GBP 0.05
            //Manual Bank Deposit
            //Fee: GBP 0.79
            ViewBag.CreditDebitFee = new CreditDebitCardViewModel().CreditDebitCardFee;
            ViewBag.ManualBankDepositFee = new SenderMoneyFexBankDepositVM().BankFee;

            int selectedCardId = 0;
            string cardNumber = null;
            string CreditCardSecurityCode = "";
            if (vm.CardDetails != null)
            {
                foreach (var item in vm.CardDetails)
                {
                    if (item.IsChecked == true)
                    {
                        selectedCardId = item.CardId;
                        cardNumber = item.CardNumber;
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                        CreditCardSecurityCode = item.SecurityCode;
                    }
                }
            }
            if (vm.CardDetails == null)
            {

                vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails();
            }
            _mobileMoneyTransferServices.SetPaymentMethod(vm);


            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            int RecipientId = _mobileMoneyTransferServices.GetRecipientId(mobileTransfer.WalletId, mobileTransfer.MobileNumber);


            var HasExceededBankDepositReceiverLimit = Common.Common.HasExceededReceiverLimit(Common.FaxerSession.SenderId,
                RecipientId, Common.FaxerSession.LoggedUser.CountryCode, enterAmount.ReceivingCountryCode, TransactionTransferMethod.OtherWallet);
            if (HasExceededBankDepositReceiverLimit)
            {
                ModelState.AddModelError("TransactionError", "Recipient daily transaction limit exceeded");
                return View(vm);
            }
            bool HasSenderTransacionExceedLimit = Common.Common.HasExceededSenderTransactionLimit(Common.FaxerSession.SenderId, Common.FaxerSession.LoggedUser.CountryCode, enterAmount.ReceivingCountryCode, TransactionTransferMethod.OtherWallet);
            if (HasSenderTransacionExceedLimit)
            {
                ModelState.AddModelError("TransactionError", "Sender daily transaction limit exceeded");
                return View(vm);
            }
            //var HasExceededTransactionLimit = Common.Common.
            //    HasExceededAmountLimit(Common.FaxerSession.LoggedUser.Id, Common.FaxerSession.LoggedUser.CountryCode, enterAmount.ReceivingCountryCode, enterAmount.SendingAmount, Module.Faxer);
            //if (HasExceededTransactionLimit)
            //{

            //    ModelState.AddModelError("TransactionError", "MoneyFex account daily transaction limit exceeded");
            //    return View(vm);
            //}


            SetTransactionSummary();

            var transactionSummaryvm = _senderForAllTransferServices.GetTransactionSummary();

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
                        SecurityCode = CreditCardSecurityCode
                        //SecurityCode = cardInfo.ClientCode.Decrypt()
                    };

                    var result = _senderForAllTransferServices.ValidateTransactionUsingStripe(transactionSummaryvm, Common.FaxerSession.LoggedUser.CountryCode, enterAmount.ReceivingCountryCode);
                    if (result.IsValid == false)
                    {

                        ModelState.AddModelError("TransactionError", result.Message);
                        return View(vm);
                    }


                    transactionSummaryvm.CreditORDebitCardDetials.StripeTokenID = result.StripeTokenId;


                    var validateCardresult = Common.Common.ValidateCreditDebitCard(transactionSummaryvm.CreditORDebitCardDetials);
                    if (validateCardresult.Data == false)
                    {
                        ModelState.AddModelError("TransactionError", validateCardresult.Message);
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                        return View(vm);
                    }
                    _senderBankAccountDepositServices.SetDebitCreditCardDetail(transactionSummaryvm.CreditORDebitCardDetials);

                    //_senderForAllTransferServices.CompleteTransaction(transactionSummaryvm);



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

                    var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm, Common.FaxerSession.LoggedUser.CountryCode, enterAmount.ReceivingCountryCode);
                    if (StripeResult.IsValid == false)
                    {
                        ModelState.AddModelError("TransactionError", StripeResult.Message);
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                        return View(vm);
                    }
                    _senderBankAccountDepositServices.SetDebitCreditCardDetail(transactionSummaryvm.CreditORDebitCardDetials);

                    return RedirectToAction("DebitCreditCardDetails", new { IsFromSavedDebitCard = true });

                //break;
                case SenderPaymentMode.CreditDebitCard:


                    return RedirectToAction("DebitCreditCardDetails");

                case SenderPaymentMode.KiiPayWallet:
                    //var hasEnoughBal = senderCommonFunc.SenderHasEnoughWalletBaltoTransfer(transactionSummaryvm.KiiPayTransferPaymentSummary.TotalAmount, transactionSummaryvm.SenderAndReceiverDetail.SenderWalletId);

                    //if (hasEnoughBal == false)
                    //{


                    //    ModelState.AddModelError("TransactionError", "Your wallet doesn't have enough balance!");
                    //    return View(vm);
                    //}

                    //_senderForAllTransferServices.CompleteTransaction(transactionSummaryvm);
                    break;
                case SenderPaymentMode.MoneyFexBankAccount:

                    return RedirectToAction("MoneyFexBankDeposit");

                default:
                    break;
            }

            return RedirectToAction("AddMoneyToWalletSuccess");

        }


        public ActionResult DebitCreditCardDetails(bool IsAddDebitCreditCard = false, bool IsFromSavedDebitCard = false)

        {
            CreditDebitCardViewModel vm = new CreditDebitCardViewModel();
            vm = _senderBankAccountDepositServices.GetDebitCreditCardDetail();
            var sendingAmountData = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();
            ViewBag.ReceivingCountryCurrency = sendingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Mobile Wallet";
            ViewBag.SendingCountryCurrency = sendingAmountData.SendingCurrencyCode;
            ViewBag.SendingAmount = sendingAmountData.SendingAmount;
            ViewBag.ReceiverName = sendingAmountData.ReceiverName;
            ViewBag.ReceivingCountry = sendingAmountData.ReceivingCountryCode.ToLower();
            ViewBag.Fee = sendingAmountData.Fee;

            //  Fee Added statically (Bank Fee) it will be added dynamically after payment setup 
            //Credit/Debit card 
            //Fee: GBP 0.80
            //Manual Bank Deposit
            //Fee: GBP 0.79


            decimal addedBankFee = sendingAmountData.TotalAmount + vm.CreditDebitCardFee;

            vm.FaxingAmount = addedBankFee;
            vm.FaxingCurrencySymbol = sendingAmountData.SendingCurrencySymbol;
            vm.FaxingCurrency = sendingAmountData.SendingCurrencyCode;
            vm.SaveCard = IsAddDebitCreditCard;
            vm.ReceiverName = sendingAmountData.ReceiverName;
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            vm.AddressLineOne = senderCommonFunc.GetSenderAddress();
            ViewBag.IsFromSavedDebitCard = IsFromSavedDebitCard;
            ViewBag.HasOneSavedCard = Common.Common.HasOneCardSaved();

            return View(vm);

        }


        [HttpPost]
        public JsonResult ThreeDQuery([Bind(Include = CreditDebitCardViewModel.BindProperty)] CreditDebitCardViewModel vm)
        {
            var serviceResult = new ServiceResult<ThreeDRequestVm>();
            var sendingAmountData = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();
            var mobileTransfer = _mobileMoneyTransferServices.GetSenderMobileMoneyTransfer();
            int RecipientId = _mobileMoneyTransferServices.GetRecipientId(mobileTransfer.WalletId, mobileTransfer.MobileNumber);


            var HasExceededBankDepositReceiverLimit = Common.Common.HasExceededReceiverLimit(Common.FaxerSession.SenderId,
                  RecipientId, Common.FaxerSession.LoggedUser.CountryCode, sendingAmountData.ReceivingCountryCode, TransactionTransferMethod.OtherWallet);
            if (HasExceededBankDepositReceiverLimit)
            {
                serviceResult.Data = null;
                serviceResult.Status = ResultStatus.Error;
                serviceResult.Message = "Recipient daily transaction limit exceeded";
                return Json(serviceResult, JsonRequestBehavior.AllowGet);

            }
            bool HasSenderTransacionExceedLimit = Common.Common.HasExceededSenderTransactionLimit(Common.FaxerSession.SenderId, Common.FaxerSession.LoggedUser.CountryCode,
                sendingAmountData.ReceivingCountryCode, TransactionTransferMethod.OtherWallet);
            if (HasSenderTransacionExceedLimit)
            {
                serviceResult.Data = null;
                serviceResult.Status = ResultStatus.Error;
                serviceResult.Message = "Sender daily transaction limit exceeded";
                return Json(serviceResult, JsonRequestBehavior.AllowGet);

            }
            var HasExceededTransactionLimit = Common.Common.
                    HasExceededAmountLimit(Common.FaxerSession.LoggedUser.Id, Common.FaxerSession.LoggedUser.CountryCode, sendingAmountData.ReceivingCountryCode,
                    vm.FaxingAmount, Module.Faxer);

            if (HasExceededTransactionLimit)
            {
                serviceResult.Data = null;
                serviceResult.Status = ResultStatus.Error;
                serviceResult.Message = "MoneyFex account daily transaction limit exceeded";
                return Json(serviceResult, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(vm.CardNumber))
            {
                ModelState.AddModelError("", "Enter Card Number");
                serviceResult.Data = null;
                serviceResult.Message = "Enter Card Number";
                serviceResult.Status = ResultStatus.Error;
                return Json(serviceResult);
            }
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

            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm,
                Common.FaxerSession.LoggedUser.CountryCode,
                sendingAmountData.ReceivingCountryCode);

            string CardType = AgentSession.CardType;


            if (!StripeResult.IsValid)
            {
                serviceResult.Data = null;
                serviceResult.Message = StripeResult.Message;
                serviceResult.Status = ResultStatus.Error;
                serviceResult.IsCardUsageMsg = StripeResult.IsCardUsageMsg;
                serviceResult.CardUsageMessage = StripeResult.CardUsageMsg;
                return Json(serviceResult);
            }

            else

            {

                //  Fee Added statically (Bank Fee) it will be added dynamically after payment setup 
                //Credit/Debit card 
                //Fee: GBP 0.80
                //Manual Bank Deposit
                //Fee: GBP 0.79 


                // paymentInfo.Fee add vairacha dai back garda 
                //decimal TotalFee = sendingAmountData.Fee + 0.05M;
                //decimal TotalSendingAmount = sendingAmountData.SendingAmount + sendingAmountData.Fee;




                SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
                _senderBankAccountDepositServices.SetDebitCreditCardDetail(vm);

                decimal faxingAmount = sendingAmountData.TotalAmount + new CreditDebitCardViewModel().CreditDebitCardFee;
                decimal TotalAmount = Common.Common.CreditTypeFee(CardType, faxingAmount);

                SetTransactionSummary();
                var transferSummary = _senderForAllTransferServices.GetTransactionSummary();
                transferSummary.CreditORDebitCardDetials = vm;
                transferSummary.CreditORDebitCardDetials.FaxingAmount = TotalAmount;

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
                    termurl = "/SenderMobileMoneyTransfer/ThreeDQueryResponseCallBack",
                    billingpostcode = senderInfo.PostCode,
                    billingpremise = senderInfo.HouseNo,
                    ReceiptNo = Common.FaxerSession.ReceiptNo,
                    SenderId = senderInfo.Id,
                    SenderFirstName = senderInfo.FirstName,
                    SenderLastName = Common.Common.GetSenderLastName(),
                    SenderEmail = Common.Common.GetSenderInfo(senderInfo.Id).Email,
                    ReceivingCurrency = Common.Common.GetCountryCurrency(transferSummary.SenderAndReceiverDetail.ReceiverCountry),
                };
                try
                {
                    var resultThreedQuery = StripServices.CreateThreedQuery(stripeCreateTransaction, TransactionTransferType.Online, TransactionTransferMethod.OtherWallet);
                    serviceResult.Message = resultThreedQuery.Message;
                    serviceResult.Status = resultThreedQuery.Status;
                    serviceResult.Data = resultThreedQuery.Data;
                    serviceResult.IsGetType3dAuth = resultThreedQuery.IsGetType3dAuth;

                    if (serviceResult.Status == ResultStatus.OK)
                    {
                        vm.ThreeDEnrolled = serviceResult.Data.ThreeDEnrolled == "Y" ? true : false;
                        SSenderBankAccountDeposit _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
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
                    Log.Write(ex.Message, ErrorType.PaymentGateway, "SenderMobileMoneyTransfer/ThreeDQuery");
                }

            }
            return Json(serviceResult);

        }

        [HttpGet]
        public ActionResult ThreeDQueryResponseCallBack(string uid, string id)
        {
            try
            {
                CustomerResponseVm vm = new CustomerResponseVm();

                Log.Write("Status Fetched");
                var result = Transact365Serivces.GetTransationDetails(uid, id);
                if (result.Status == ResultStatus.Error)
                {
                    Log.Write("Status Fetched");
                    return RedirectToAction("DebitCreditCardDetails");
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ErrorType.PaymentGateway, "SenderMobileMoneyTransfer/ThreeDQueryResponseCallBack");
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
                Log.Write(ex.Message, ErrorType.PaymentGateway, "SenderMobileMoneyTransfer/ThreeDQueryResponseCallBack");
                return RedirectToAction("DebitCreditCardDetails");

            }
        }

        [HttpPost]
        [PreventSpam]
        public ActionResult DebitCreditCardDetails([Bind(Include = CustomerResponseVm.BindProperty)] CustomerResponseVm responseVm)
        {
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            var transferSummary = _senderForAllTransferServices.GetTransactionSummary();
            ViewBag.HasOneSavedCard = Common.Common.HasOneCardSaved();

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
            var cardProcessor = StripServices.GetCardProcessor(stripeCreateTransaction, TransactionTransferType.Online, TransactionTransferMethod.OtherWallet);
            transferSummary.CardProcessorApi = cardProcessor;
            _senderForAllTransferServices.SetTransactionSummary(transferSummary);

            switch (cardProcessor)
            {
                case CardProcessorApi.TrustPayment:
                    var transactionResult = new StripeResult();

                    transactionResult = StripServices.CreateTransaction(stripeCreateTransaction);

                    if (transactionResult == null)
                    {
                        ModelState.AddModelError("TransactionError", transactionResult.Message);
                        transferSummary.CreditORDebitCardDetials.SecurityCode = "";
                        return View(vm);
                    }
                    else
                    {
                        if (transactionResult.IsValid == false)
                        {
                            vm.IsCardUsageMsg = transactionResult.IsCardUsageMsg;
                            vm.CardUsageMsg = transactionResult.CardUsageMsg;
                            vm.ErrorMsg = transactionResult.Message;
                            ModelState.AddModelError("StripeError", transactionResult.Message);
                            return View(vm);
                        }
                        try
                        {

                            var transactionIsIntiated = StripServices.GetTransactionLog().Where(x => x.orderreference == stripeCreateTransaction.ReceiptNo
                        && x.requesttypedescription == "AUTH" && x.errorcode == "0").FirstOrDefault();
                            if (transactionIsIntiated == null)
                            {
                                ModelState.AddModelError("StripeError", "Transaction has not be paid ! Try again");
                                return View(vm);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Write(ex.Message, ErrorType.PaymentGateway, "SenderMobileMoneyTransfer/DebitCreditCardDetails");
                            return View(vm);
                        }
                    }
                    break;
                case CardProcessorApi.T365:
                    break;
            }
            var mobileMoneydata = _mobileMoneyTransferServices.list().Data.Where(x => x.Id == Common.FaxerSession.TransactionId).FirstOrDefault();
            //if (mobileMoneydata != null)
            //{

            //    mobileMoneydata.Status = MobileMoneyTransferStatus.Abnormal;
            //    _mobileMoneyTransferServices.Update(mobileMoneydata);
            //}
            var SenderDocumentApprovalStatus = Common.Common.GetSenderIdentificationStatus(Common.FaxerSession.LoggedUser.Id);
            transferSummary.IsIdCheckInProgress = Common.Common.IsSenderIdCheckInProgress(Common.FaxerSession.LoggedUser.Id);
            //if (SenderDocumentApprovalStatus == null)
            //{
            //    transferSummary.IsIdCheckInProgress = true;
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
                default:
                    break;
            }
            //if (SenderDocumentApprovalStatus == null || SenderDocumentApprovalStatus == DocumentApprovalStatus.Disapproved)
            //{
            //    return RedirectToAction("IdentificationInformation", "SenderBankAccountDeposit");
            //}
            if (transferSummary.IsLocalPayment == false)
            {
                return RedirectToAction("AddMoneyToWalletSuccess");
            }
            else
            {
                return RedirectToAction("LocalAddMoneyToWalletSuccess");
            }
        }



        //[HttpPost]
        //public ActionResult DebitCreditCardDetails(CreditDebitCardViewModel model)
        //{
        //    var result = Common.Common.ValidateCreditDebitCard(model);
        //    if (result.Data == false)
        //    {
        //        ModelState.AddModelError("", result.Message);
        //        return View(model);
        //    }
        //    var number = model.CardNumber.Split(' ');
        //    model.CardNumber = string.Join("", number);
        //    var CardResult = Common.Common.ValidateCreditDebitCard(model);
        //    if (result.Data == false)
        //    {
        //        ModelState.AddModelError("", result.Message);
        //        return View(model);
        //    }
        //    SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

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
        //    string CardType = AgentSession.CardType;
        //    decimal TotalAmount = Common.Common.CreditTypeFee(CardType, model.FaxingAmount);
        //    var transferSummary = _senderForAllTransferServices.GetTransactionSummary();
        //    transferSummary.CreditORDebitCardDetials.FaxingAmount = TotalAmount;

        //    if (!StripeResult.IsValid)
        //    {
        //        ModelState.AddModelError("StripeError", StripeResult.Message);
        //    }

        //    else
        //    {
        //        StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
        //        {
        //            Amount = /*(Int32)model.FaxingAmount*/ TotalAmount,
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
        //            transferSummary.CreditORDebitCardDetials.FaxingAmount = TotalAmount;

        //            _senderForAllTransferServices.CompleteTransaction(transferSummary);

        //            if (transferSummary.IsLocalPayment == false)
        //            {
        //                return RedirectToAction("AddMoneyToWalletSuccess", "SenderMobileMoneyTransfer");
        //            }
        //            return RedirectToAction("LocalAddMoneyToWalletSuccess", "SenderMobileMoneyTransfer");
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
                    Amount = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount().TotalAmount,
                    IsCreditDebitCardCheck = true,
                    SenderId = Common.FaxerSession.LoggedUser.Id

                };
                var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);

                decimal amount = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount().TotalAmount;
                amount = (decimal)Math.Round((0.60 / 100), 2) * amount;
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

            var sendingAmountData = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();

            ViewBag.ReceivingCountryCurrency = sendingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Mobile Wallet";
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
            _mobileMoneyTransferServices.SetMoneyFexBankAccountDeposit(vm);
            vm = _mobileMoneyTransferServices.GetMoneyFexBankAccountDeposit();

            return View(vm);
        }

        [HttpPost]
        [PreventSpam]
        public ActionResult MoneyFexBankDeposit([Bind(Include = SenderMoneyFexBankDepositVM.BindProperty)]SenderMoneyFexBankDepositVM model)
        {
            if (ModelState.IsValid)
            {
                SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
                var transferSummary = _senderForAllTransferServices.GetTransactionSummary();

                var moneyFexBankAccountDeposit = _mobileMoneyTransferServices.GetMoneyFexBankAccountDeposit();
                moneyFexBankAccountDeposit.HasMadePaymentToBankAccount = model.HasMadePaymentToBankAccount;
                //_mobileMoneyTransferServices.SetMoneyFexBankAccountDeposit(moneyFexBankAccountDeposit);

                var sendingAmountData = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();
                ;

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

                return RedirectToAction("Success", "MoneyFexBankDeposit");
            }

            return View(model);
        }



        public ActionResult AddMoneyToWalletSuccess()
        {
            var details = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();
            SenderAddMoneySuccessVM model = new SenderAddMoneySuccessVM()
            {
                Amount = details.SendingAmount,
                Currnecy = details.SendingCurrencySymbol,
                ReceiverName = details.ReceiverName,

            };
            Common.FaxerSession.SenderMobileMoneyTransfer = null;
            senderCommonFunc.ClearMobileTransferSession();

            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Payment Confirmation");
            return View(model);

        }

        #endregion

        #region local
        public ActionResult SendingMoneyLocal()
        {

            int senderId = Common.FaxerSession.LoggedUser.Id;
            var recentlyPaidNumbers = _mobileMoneyTransferServices.GetRecentlyPaidNumbers(senderId, DB.Module.Faxer);
            ViewBag.RecentlyPaidNumbers = new SelectList(recentlyPaidNumbers, "Code", "Name");
            var wallets = _mobileMoneyTransferServices.GetWallets().Where(x => x.CountryCode == Common.FaxerSession.LoggedUser.CountryCode).ToList();
            ViewBag.Wallets = new SelectList(wallets, "Id", "Name");
            SenderMobileMoneyTransferVM vm = new SenderMobileMoneyTransferVM();

            vm = _mobileMoneyTransferServices.GetSenderMobileMoneyTransfer();
            vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Common.FaxerSession.LoggedUser.CountryCode);
            return View(vm);
        }

        [HttpPost]
        public ActionResult SendingMoneyLocal([Bind(Include = SenderMobileMoneyTransferVM.BindProperty)] SenderMobileMoneyTransferVM model)
        {

            int senderId = Common.FaxerSession.LoggedUser.Id;
            var recentlyPaidNumbers = _mobileMoneyTransferServices.GetRecentlyPaidNumbers(senderId, DB.Module.Faxer);
            ViewBag.RecentlyPaidNumbers = new SelectList(recentlyPaidNumbers, "Code", "Name");
            var wallets = _mobileMoneyTransferServices.GetWallets().Where(x => x.CountryCode == Common.FaxerSession.LoggedUser.CountryCode).ToList();
            ViewBag.Wallets = new SelectList(wallets, "Id", "Name");
            if (ModelState.IsValid)
            {
                model.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Common.FaxerSession.LoggedUser.CountryCode);
                model.CountryCode = Common.FaxerSession.LoggedUser.CountryCode;

                _mobileMoneyTransferServices.SetSenderMobileMoneyTransfer(model);
                return RedirectToAction("LocalEnterAmount", "SenderMobileMoneyTransfer");
            }
            _mobileMoneyTransferServices.SetSenderMobileMoneyTransfer(model);
            return View(model);
        }

        public ActionResult LocalEnterAmount()
        {


            var data = _mobileMoneyTransferServices.GetSenderMobileMoneyTransfer();
            SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm()
            {
                ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(Common.FaxerSession.LoggedUser.CountryCode),
                SendingCurrencyCode = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode),
                SendingCurrencySymbol = Common.Common.GetCurrencySymbol(Common.FaxerSession.LoggedUser.CountryCode),
                ReceivingCurrencyCode = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode),
                ImageUrl = "",
                ReceiverName = data.ReceiverName,

            };
            //SSenderForAllTransfer.SetPaymentSummarySession(Common.FaxerSession.LoggedUser.CountryCode);
            _mobileMoneyTransferServices.SetSenderMobileEnrterAmount(vm);
            return View(vm);
        }

        [HttpPost]
        public ActionResult LocalEnterAmount([Bind(Include = SenderMobileEnrterAmountVm.BindProperty)] SenderMobileEnrterAmountVm model)
        {
            if (model.SendingAmount != 0)
            {
                model.TotalAmount = model.SendingAmount;

                // Rewrite session with additional value 
                model.Fee = 0;
                model.SendingAmount = model.SendingAmount;
                model.ReceivingAmount = model.SendingAmount;

                if (model.SendSms == true)
                {
                    model.SmsCharge = Common.Common.GetSmsFee(Common.FaxerSession.LoggedUser.CountryCode);
                }
                model.TotalAmount = model.SendingAmount + model.SmsCharge;
                _mobileMoneyTransferServices.SetSenderMobileEnrterAmount(model);
                return RedirectToAction("AccountPaymentSummary", "SenderMobileMoneyTransfer");
            }
            return View(model);

        }

        public ActionResult AccountPaymentSummary()
        {
            SenderTransferSummaryVm vm = new SenderTransferSummaryVm();

            var sendingAmountData = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();
            var loggedUSerData = _mobileMoneyTransferServices.GetLoggedUserData();

            vm.Amount = sendingAmountData.SendingAmount;
            vm.SendingCurrencyCode = sendingAmountData.SendingCurrencyCode;
            vm.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(Common.FaxerSession.LoggedUser.CountryCode);
            vm.ReceivedAmount = sendingAmountData.SendingAmount;
            vm.Fee = 0M;
            vm.PaidAmount = sendingAmountData.TotalAmount;
            vm.ReceiverName = sendingAmountData.ReceiverName;
            vm.ReceivingCurrencyCode = sendingAmountData.ReceivingCurrencyCode;
            vm.ReceivingCurrencySymbol = sendingAmountData.ReceivingCurrencySymbol;
            return View(vm);
        }

        [HttpPost]
        public ActionResult AccountPaymentSummary([Bind(Include = SenderTransferSummaryVm.BindProperty)] SenderTransferSummaryVm model)
        {


            return RedirectToAction("LocalPayment", "SenderMobileMoneyTransfer");
        }


        public ActionResult LocalPayment()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var vm = _mobileMoneyTransferServices.GetPaymentMethod();
            var sendingAmountData = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();

            vm.TotalAmount = sendingAmountData.TotalAmount;
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
        public ActionResult LocalPayment([Bind(Include = PaymentMethodViewModel.BindProperty)] PaymentMethodViewModel vm)
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
            //_kiiPayWalletTransferServices.SetPaymentMethod(vm);
            _mobileMoneyTransferServices.SetPaymentMethod(vm);



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

                case SenderPaymentMode.KiiPayWallet:
                    var hasEnoughBal = senderCommonFunc.SenderHasEnoughWalletBaltoTransfer(transactionSummaryvm.KiiPayTransferPaymentSummary.TotalAmount, transactionSummaryvm.SenderAndReceiverDetail.SenderWalletId);

                    if (hasEnoughBal == false)
                    {


                        ModelState.AddModelError("TransactionError", "Your wallet doesn't have enough balance!");
                        return View(vm);
                    }
                    //_senderForAllTransferServices.CompleteTransaction(transactionSummaryvm);
                    break;
                case SenderPaymentMode.MoneyFexBankAccount:

                    return RedirectToAction("MoneyFexBankDeposit");

                default:
                    break;
            }

            return RedirectToAction("LocalAddMoneyToWalletSuccess");



        }

        //public ActionResult LocalMoneyFexBankDeposit()
        //{


        //    var sendingAmountData = _mobileMoneyTransferServices.GetSenderLocalEnterAmount();
        //    SenderMoneyFexBankDepositVM vm = new SenderMoneyFexBankDepositVM()
        //    {
        //        Amount = sendingAmountData.Amount,
        //        SendingCurrencyCode = sendingAmountData.CurrencyCode,
        //        SendingCurrencySymbol = sendingAmountData.CurrencySymbol
        //    };

        //    return View(vm);
        //}

        //[HttpPost]
        //public ActionResult LocalMoneyFexBankDeposit(SenderMoneyFexBankDepositVM model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _mobileMoneyTransferServices.SetMoneyFexBankAccountDeposit(model);
        //        return RedirectToAction("LocalAddMoneyToWalletSuccess", "SenderMobileMoneyTransfer");
        //    }

        //    return View(model);
        //}

        public ActionResult LocalAddMoneyToWalletSuccess()
        {
            var receiver = _mobileMoneyTransferServices.GetSenderMobileMoneyTransfer();
            var details = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();
            SenderAddMoneySuccessVM model = new SenderAddMoneySuccessVM()
            {
                Amount = details.SendingAmount,
                Currnecy = details.SendingCurrencySymbol,
                ReceiverName = receiver.ReceiverName,

            };
            Common.FaxerSession.SenderMobileMoneyTransfer = null;
            senderCommonFunc.ClearMobileTransferSession();

            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Payment Confirmation");
            return View(model);

        }

        #endregion
        //For DropDowns

        public JsonResult GetRecentlyPaidNumberInfo(string mobileNumber)
        {

            var ReceiverName = _mobileMoneyTransferServices.list().Data.Where(x => x.PaidToMobileNo == mobileNumber).Select(x => x.ReceiverName).FirstOrDefault();
            return Json(new
            {
                ReceiverName = ReceiverName,
                MobileNumber = mobileNumber,
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCountryPhone(string countryCode)
        {
            if (!string.IsNullOrEmpty(countryCode))
            {
                var phoneCode = Common.Common.GetCountryPhoneCode(countryCode);
                return Json(new
                {
                    countryPhoneCode = phoneCode,
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                countryPhoneCode = "",
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult IsCVVCodeValid(string securityCode = "", int cardId = 0)
        {
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            var enterAmount = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();
            var mobileTransfer = _mobileMoneyTransferServices.GetSenderMobileMoneyTransfer();
            int RecipientId = _mobileMoneyTransferServices.GetRecipientId(mobileTransfer.WalletId, mobileTransfer.MobileNumber);
            var HasExceededBankDepositReceiverLimit = Common.Common.HasExceededReceiverLimit(Common.FaxerSession.SenderId,
                                                       RecipientId, Common.FaxerSession.LoggedUser.CountryCode,
                                                       enterAmount.ReceivingCountryCode, TransactionTransferMethod.OtherWallet);
            if (HasExceededBankDepositReceiverLimit)
            {
                return Json(new ServiceResult<CreditDebitCardViewModel>()
                {
                    Data = new CreditDebitCardViewModel(),
                    Status = ResultStatus.Error,
                    Message = "Recipient daily transaction limit exceeded"
                }, JsonRequestBehavior.AllowGet);
            }
            bool HasSenderTransacionExceedLimit = Common.Common.HasExceededSenderTransactionLimit(Common.FaxerSession.SenderId, Common.FaxerSession.LoggedUser.CountryCode, enterAmount.ReceivingCountryCode, TransactionTransferMethod.OtherWallet);
            if (HasSenderTransacionExceedLimit)
            {
                return Json(new ServiceResult<CreditDebitCardViewModel>()
                {
                    Data = new CreditDebitCardViewModel(),
                    Status = ResultStatus.Error,
                    Message = "Sender daily transaction limit exceeded"
                }, JsonRequestBehavior.AllowGet);
            }

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            PaymentMethodViewModel vm = new PaymentMethodViewModel()
            {
                HasEnableMoneyFexBankAccount = senderCommonFunc.IsEnabledMoneyFexbankAccount(Common.FaxerSession.LoggedUser.CountryCode),
                SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard,
            };
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
                //SecurityCode = cardInfo.ClientCode.Decrypt()
            };

            var result = _senderForAllTransferServices.ValidateTransactionUsingStripe(transactionSummaryvm, Common.FaxerSession.LoggedUser.CountryCode, enterAmount.ReceivingCountryCode);
            if (result.IsValid == false)
            {
                return Json(new ServiceResult<CreditDebitCardViewModel>()
                {
                    Data = new CreditDebitCardViewModel(),
                    Status = ResultStatus.Error,
                    Message = result.Message
                }, JsonRequestBehavior.AllowGet);
            }


            transactionSummaryvm.CreditORDebitCardDetials.StripeTokenID = result.StripeTokenId;


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
            _senderBankAccountDepositServices.SetDebitCreditCardDetail(transactionSummaryvm.CreditORDebitCardDetials);

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

            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm, Common.FaxerSession.LoggedUser.CountryCode, enterAmount.ReceivingCountryCode);
            if (StripeResult.IsValid == false)
            {
                return Json(new ServiceResult<CreditDebitCardViewModel>()
                {
                    Data = new CreditDebitCardViewModel(),
                    Status = ResultStatus.Error,
                    Message = StripeResult.Message
                }, JsonRequestBehavior.AllowGet);
            }
            _senderBankAccountDepositServices.SetDebitCreditCardDetail(transactionSummaryvm.CreditORDebitCardDetials);
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

            var enterAmountData = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();
            var loggedInSenderData = _mobileMoneyTransferServices.GetLoggedUserData();
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

            _mobileMoneyTransferServices.SetSenderMobileEnrterAmount(enterAmountData);
            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount = result.FaxingAmount,
                ReceiveAmount = result.ReceivingAmount
            }, JsonRequestBehavior.AllowGet);
        }
        public void SetTransactionSummary()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();

            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

            var sendingAmountData = _mobileMoneyTransferServices.GetSenderMobileEnrterAmount();
            var mobileTransfer = _mobileMoneyTransferServices.GetSenderMobileMoneyTransfer();
            //Completing Transaction
            var loggedUserData = _mobileMoneyTransferServices.GetLoggedUserData();
            var paymentMethod = _mobileMoneyTransferServices.GetPaymentMethod();

            TransactionSummaryVM transactionSummaryVm = new TransactionSummaryVM();
            transactionSummaryVm.MobileMoneyTransfer = mobileTransfer;
            int SenderWalletId = 0;
            var senderWalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
            if (senderWalletInfo != null)
            {

                SenderWalletId = senderWalletInfo.Id;
            }
            transactionSummaryVm.SenderAndReceiverDetail = new SenderAndReceiverDetialVM()
            {
                SenderId = loggedUserData.Id,
                SenderCountry = loggedUserData.CountryCode,
                ReceiverCountry = mobileTransfer.CountryCode,
                SenderWalletId = SenderWalletId,
                WalletOperatorId = Common.FaxerSession.SenderMobileMoneyTransfer.WalletId
                //ReceiverId = mobileTransfer.RecentReceiverId == null ? 0 : (int)mobileTransfer.RecentReceiverId

            };

            //Set Sms Fee 
            transactionSummaryVm.KiiPayTransferPaymentSummary = new KiiPayTransferPaymentSummary()
            {
                ReceiverName = mobileTransfer.ReceiverName,
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


            var debitCreditCardDetail = _mobileMoneyTransferServices.GetDebitCreditCardDetail();

            transactionSummaryVm.CreditORDebitCardDetials = debitCreditCardDetail;



            var moneyFexBankAccountDepositData = _mobileMoneyTransferServices.GetMoneyFexBankAccountDeposit();
            transactionSummaryVm.MoneyFexBankDeposit = moneyFexBankAccountDepositData;

            transactionSummaryVm.TransferType = TransferType.MobileTransfer;
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
        public JsonResult CheckifFlutterWaveApi(string CountryCode = "")
        {
            var isFlutterwaveApi = false;
            decimal SendingAmount = _kiiPaytrasferServices.GetCommonEnterAmount().SendingAmount;

            var Apiservice = Common.Common.GetApiservice(Common.FaxerSession.LoggedUser.CountryCode, CountryCode, SendingAmount,
                       TransactionTransferMethod.OtherWallet, TransactionTransferType.Online);
            if (Apiservice == DB.Apiservice.FlutterWave)
            {
                isFlutterwaveApi = true;
            }
            return Json(isFlutterwaveApi, JsonRequestBehavior.AllowGet);
        }


    }
}