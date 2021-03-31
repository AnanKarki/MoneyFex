using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Areas.Staff.ViewModels;
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
using TransferZero.Sdk.Model;
using Twilio.TwiML.Voice;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Areas.Admin.Controllers.Staff_Transactions
{
    public class StaffOtherMobileWalletsTransferController : Controller
    {

        LoggedStaff StaffInfo = Common.StaffSession.LoggedStaff ?? new LoggedStaff();
        Admin.Services.CommonServices common = new Admin.Services.CommonServices();
        SSenderMobileMoneyTransfer _mobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
        SCashPickUpTransferService _cashPickUp = new SCashPickUpTransferService(Module.Staff);
        SAgentMobileTransferWallet _sAgentMobileTransferWalletServices = new SAgentMobileTransferWallet(Module.Staff);
        AgentCommonServices _commonServices = new AgentCommonServices();

        // GET: Admin/StaffOtherMobileWalletsTransfer
        public ActionResult Index(string AccountNoORPhoneNo = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            Common.FaxerSession.TransferMethod = "otherwallet";

            var countries = common.GetCountries();
            var identifyCardType = common.GetCardType();

            ViewBag.countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType");
            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();

            var backbuttonRecentSenderDetails = _cashPickUp.GetStaffCashPickupInformationViewModel();
            if (backbuttonRecentSenderDetails.Id != 0)
            {
                ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType", backbuttonRecentSenderDetails.IdType);
                ViewBag.countries = new SelectList(countries, "Code", "Name", backbuttonRecentSenderDetails.Country);
                ViewBag.FaxerCountryPhoneCode = Common.Common.GetCountryPhoneCode(backbuttonRecentSenderDetails.Country);

                return View(backbuttonRecentSenderDetails);
            }
            if (AccountNoORPhoneNo != "")
            {

                var result = _cashPickUp.getFaxer(AccountNoORPhoneNo, vm);

                if (result != null)
                {
                    ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType", result.IdType);
                    ViewBag.countries = new SelectList(countries, "Code", "Name", result.Country);
                    ViewBag.FaxerCountryPhoneCode = Common.Common.GetCountryPhoneCode(result.Country);

                    if (result.Id != 0)
                    {
                        result.Search = AccountNoORPhoneNo;
                        return View(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("Invalid", "Account does not exist");

                    return View(vm);
                }


            }
            vm.Country = StaffInfo.Country;
            vm.Search = vm.Search;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = CashPickupInformationViewModel.BindProperty)]CashPickupInformationViewModel Vm)
        {
            SetViewBagForCountry(Vm.CountryCode);
            var identifyCardType = common.GetCardType();
            ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType");
            var CurrentYear = DateTime.Now.Year;
            var AccountNoORPhoneNo = Common.Common.IgnoreZero(Vm.Search);
            var SenderDetails = _cashPickUp.getFaxer(AccountNoORPhoneNo, Vm);
            if (Vm.DOB == null)
            {
                ModelState.AddModelError("", "Enter Date Of Birth .");

                return View(Vm);
            }
            var DOB = Vm.DOB;
            DateTime date = Convert.ToDateTime(DOB);
            var DOByear = date.Year;
            var Age = CurrentYear - DOByear;
            Vm.MobleCode = Common.Common.GetCountryPhoneCode(Vm.IssuingCountry);
            if (ModelState.IsValid)
            {
                Vm.SenderFullName = Vm.FirstName + " " + Vm.MiddleName + " " + Vm.LastName;
                if (Age <= 18)
                {
                    ModelState.AddModelError("InvalidAge", "Sender's should be more than 18 years to do the transaction.");
                    return View(Vm);
                }
                if (Vm.ExpiryDate < DateTime.Now)
                {
                    ModelState.AddModelError("IDExpired", "Expired ID");
                    return View(Vm);
                }

                _cashPickUp.SetStaffCashPickupInformationViewModel(SenderDetails);
                return RedirectToAction("MobileWalletEnterAmount");
            }
            return View(Vm);
        }
        public ActionResult MobileWalletEnterAmount()
        {
            MobileMoneyTransferEnterAmountViewModel vm = new MobileMoneyTransferEnterAmountViewModel();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();
            ViewBag.SenderName = senderInfo.SenderFullName;
            ViewBag.SenderCountry = common.getCountryNameFromCode(senderInfo.Country);
            vm.SendingCountry = senderInfo.Country;
            vm.SendingCurrencyCode = Common.Common.GetCurrencyCode(senderInfo.Country);
            ViewBag.SenderAccountNo = common.GetSenderAccountNoBySenderId(senderInfo.Id);
            var paymentInfo = _sAgentMobileTransferWalletServices.GetStaffMobileMoneyTransferEnterAmountViewModel();
            if (!string.IsNullOrEmpty(paymentInfo.ReceivingCountry))
            {
                vm = paymentInfo;
            }
            ViewBag.TransferMethod = (int)TransactionServiceType.MobileWallet;
            return View(vm);
        }
        [HttpPost]
        public ActionResult MobileWalletEnterAmount([Bind(Include = MobileMoneyTransferEnterAmountViewModel.BindProperty)]MobileMoneyTransferEnterAmountViewModel vm)
        {
            ViewBag.TransferMethod = (int)TransactionServiceType.MobileWallet;
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();

            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();
            vm.ReceivingCurrencyCode = paymentInfo.ReceivingCurrency;
            vm.ReceivingCountry = paymentInfo.ReceivingCountryCode;
            if (ModelState.IsValid)
            {
                if (vm.SendingAmount == 0)
                {
                    ModelState.AddModelError("SendingAmount", "Please Enter Sending Amount ");
                    return View(vm);
                }
                if (vm.ReceivingAmount == 0)
                {
                    ModelState.AddModelError("RecevingAmount", "Please Enter Sending Amount ");
                    return View(vm);
                }
                if (string.IsNullOrEmpty((vm.Fee).ToString()))
                {
                    ModelState.AddModelError("FaxingFee", "Please calculate estimated fee ");
                    return View(vm);
                }

                _sAgentMobileTransferWalletServices.SetStaffMobileMoneyTransferEnterAmountViewModel(vm);
                return RedirectToAction("OtherMobileWalletDetails");
            }
            return View();
        }

        public ActionResult OtherMobileWalletDetails(string MobileNo = "")
        {
            var paymentInfo = _sAgentMobileTransferWalletServices.GetStaffMobileMoneyTransferEnterAmountViewModel();
            string Country = paymentInfo.ReceivingCountry;
            SetViewBagForPhoneNumbers(Country);
            SetViewBagForMobileWalletProvider(Country);
            ReceiverDetailsInformationViewModel vm = new ReceiverDetailsInformationViewModel();
            vm = _sAgentMobileTransferWalletServices.GetStaffReceiverDetailsInformation();
            vm.MobileCode = Common.Common.GetCountryPhoneCode(Country);
            vm.Country = Country;
            if (!string.IsNullOrEmpty(MobileNo))
            {
                vm = _sAgentMobileTransferWalletServices.GetReceiverDetailsByMobileNO(MobileNo);
            }
            return View(vm);

        }
        [HttpPost]
        public ActionResult OtherMobileWalletDetails([Bind(Include = ReceiverDetailsInformationViewModel.BindProperty)] ReceiverDetailsInformationViewModel model)
        {
            SetViewBagForPhoneNumbers(model.Country);
            SetViewBagForMobileWalletProvider(model.Country);
            if (ModelState.IsValid)
            {
                bool IsValidReceiver = Common.Common.IsValidBankDepositReceiver(model.MobileNumber, Service.MobileWallet);

                if (IsValidReceiver == false)
                {
                    ModelState.AddModelError("InvalidReceiver", " Receiver is banned");
                    return View(model);
                }

                var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
                var paymentInfo = _sAgentMobileTransferWalletServices.GetStaffMobileMoneyTransferEnterAmountViewModel();
                SmsApi smsApi = new SmsApi();
                var IsValidMobileNo = smsApi.IsValidMobileNo(ViewBag.PhoneCode + "" + model.MobileNumber);
                if (IsValidMobileNo == false)
                {
                    ModelState.AddModelError("", "Enter Valid Number");
                    return View(model);
                }
                var IsValidAccount = _mobileMoneyTransferServices.IsValidMobileAccount(new SenderMobileMoneyTransferVM()
                {
                    CountryCode = model.Country,
                    WalletId = model.MobileWalletProvider,
                    MobileNumber = model.MobileNumber,
                    ReceiverName = model.ReceiverName,
                    CountryPhoneCode = Common.Common.GetCountryPhoneCode(model.Country),
                }, paymentInfo.SendingAmount, senderInfo.Country, TransactionTransferType.Admin);

                if (IsValidAccount.Data == false)
                {
                    ModelState.AddModelError("", IsValidAccount.Message);
                    return View(model);
                }
                _sAgentMobileTransferWalletServices.SetStaffReceiverDetailsInformation(model);
                return RedirectToAction("MobileWalletSummary");
            }
            return View();

        }


        public ActionResult MobileWalletSummary()
        {
            var paymentInfo = _sAgentMobileTransferWalletServices.GetStaffMobileMoneyTransferEnterAmountViewModel();

            var receiverinfo = _sAgentMobileTransferWalletServices.GetStaffReceiverDetailsInformation();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            ViewBag.SenderName = senderInfo.SenderFullName;
            ViewBag.SenderCountry = common.getCountryNameFromCode(senderInfo.Country);
            if (senderInfo.Id > 0)
            {
                ViewBag.SenderAccountNo = common.GetSenderAccountNoBySenderId(senderInfo.Id);

            }
            string ReceiverFirstname = receiverinfo.ReceiverName.Split(' ')[0];
            CommonTransactionSummaryViewModel vm = new CommonTransactionSummaryViewModel()
            {

                Fee = paymentInfo.Fee,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                ReceivingCurrecyCode = paymentInfo.ReceivingCurrencyCode,
                ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCurrencyCode = paymentInfo.SendingCurrencyCode,
                SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol,
                ReceiverFirstName = ReceiverFirstname,
                TotalAmount = paymentInfo.TotalAmount,

            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult MobileWalletSummary([Bind(Include = CommonTransactionSummaryViewModel.BindProperty)] CommonTransactionSummaryViewModel vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("SenderPaymentMethod", "StaffOtherMobileWalletsTransfer");
            }

            return View(vm);


        }
        public ActionResult SenderPaymentMethod()
        {
            PaymentMethodViewModel vm = new PaymentMethodViewModel();
            var paymentInfo = _sAgentMobileTransferWalletServices.GetStaffMobileMoneyTransferEnterAmountViewModel();
            var receiverinfo = _sAgentMobileTransferWalletServices.GetStaffReceiverDetailsInformation();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();

            ViewBag.SenderName = senderInfo.SenderFullName;
            ViewBag.SenderCountry = common.getCountryNameFromCode(senderInfo.Country);
            if (senderInfo.Id > 0)
            {
                ViewBag.SenderAccountNo = common.GetSenderAccountNoBySenderId(senderInfo.Id);

            }
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();

            vm.TotalAmount = paymentInfo.TotalAmount;
            vm.SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol;
            ViewBag.Fee = paymentInfo.Fee;


            ViewBag.CreditDebitFee = new CreditDebitCardViewModel(senderInfo.Country).CreditDebitCardFee;
            ViewBag.ManualBankDepositFee = new SenderMoneyFexBankDepositVM(senderInfo.Country).BankFee;


            var senderwalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(senderInfo.Id);
            if (senderwalletInfo != null)
            {
                vm.KiipayWalletBalance = senderwalletInfo.CurrentBalance;
                vm.HasKiiPayWallet = true;
            }
            else
            {
                vm.HasKiiPayWallet = false;
            }

            vm.HasEnableMoneyFexBankAccount = senderCommonFunc.IsEnabledMoneyFexbankAccount(senderInfo.CountryCode);
            vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails(senderInfo.Id, Module.Staff);

            return View(vm);

        }

        [HttpPost]
        public ActionResult SenderPaymentMethod([Bind(Include = PaymentMethodViewModel.BindProperty)] PaymentMethodViewModel vm)
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var paymentInfo = _sAgentMobileTransferWalletServices.GetStaffMobileMoneyTransferEnterAmountViewModel();
            var receiverinfo = _sAgentMobileTransferWalletServices.GetStaffReceiverDetailsInformation();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();

            ViewBag.CreditDebitFee = new CreditDebitCardViewModel(senderInfo.CountryCode).CreditDebitCardFee;
            ViewBag.ManualBankDepositFee = new SenderMoneyFexBankDepositVM(senderInfo.CountryCode).BankFee;

            vm.TotalAmount = paymentInfo.TotalAmount;
            vm.SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol;
            ViewBag.Fee = paymentInfo.Fee;

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
            _sAgentMobileTransferWalletServices.SetStaffPaymentMethod(vm);
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

            var transactionSummaryvm = _senderForAllTransferServices.GetAdminTransactionSummary();
            Common.AdminSession.ReceiptNo = Common.Common.GenerateMobileMoneyTransferReceiptNo(8);
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
                    var validateCardresult = Common.Common.ValidateCreditDebitCard(transactionSummaryvm.CreditORDebitCardDetials);
                    if (validateCardresult.Data == false)
                    {
                        ModelState.AddModelError("TransactionError", validateCardresult.Message);
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                        return View(vm);
                    }
                    StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
                    {
                        CardName = senderInfo.SenderFullName,
                        ExpirationMonth = transactionSummaryvm.CreditORDebitCardDetials.EndMM,
                        ExpiringYear = transactionSummaryvm.CreditORDebitCardDetials.EndYY,
                        Number = transactionSummaryvm.CreditORDebitCardDetials.CardNumber,
                        SecurityCode = transactionSummaryvm.CreditORDebitCardDetials.SecurityCode,

                        billingpostcode = senderInfo.PostCode,
                        billingpremise = senderInfo.AddressLine1,
                        CurrencyCode = Common.Common.GetCurrencyCode(senderInfo.Country)
                    };

                    var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);
                    if (StripeResult.IsValid == false)
                    {

                        ModelState.AddModelError("TransactionError", StripeResult.Message);
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                        return View(vm);
                    }
                    common.SetDebitCreditCardDetail(transactionSummaryvm.CreditORDebitCardDetials);

                    return RedirectToAction("DebitCreditCardDetails", new { IsFromSavedDebitCard = true });

                case SenderPaymentMode.CreditDebitCard:
                    return RedirectToAction("DebitCreditCardDetails");
                case SenderPaymentMode.KiiPayWallet:
                    var hasEnoughBal = senderCommonFunc.SenderHasEnoughWalletBaltoTransfer(transactionSummaryvm.KiiPayTransferPaymentSummary.TotalAmount, transactionSummaryvm.SenderAndReceiverDetail.SenderWalletId);
                    if (hasEnoughBal == false)
                    {
                        ModelState.AddModelError("TransactionError", "Your wallet doesn't have enough balance!");
                        vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails();
                        return View(vm);
                    }
                    Common.AdminSession.ReceiptNo = Common.Common.GenerateCashPickUpReceiptNo(6);
                    break;
                default:
                    break;
            }

            return RedirectToAction("SenderPaymentMethodSuccess");
        }


        public ActionResult DebitCreditCardDetails(bool IsAddDebitCreditCard = false, bool IsFromSavedDebitCard = false)

        {
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            var receiverInfo = _sAgentMobileTransferWalletServices.GetStaffReceiverDetailsInformation();
            ViewBag.SenderName = senderInfo.SenderFullName;
            ViewBag.SenderCountry = common.getCountryNameFromCode(senderInfo.Country);
            if (senderInfo.Id > 0)
            {
                ViewBag.SenderAccountNo = common.GetSenderAccountNoBySenderId(senderInfo.Id);
            }
            CreditDebitCardViewModel vm = new CreditDebitCardViewModel(senderInfo.Country);
            vm = common.GetDebitCreditCardDetail(senderInfo.Country);
            var sendingAmountData = _sAgentMobileTransferWalletServices.GetMobileMoneyTransferEnterAmountViewModel();
            ViewBag.ReceivingCountryCurrency = sendingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Mobile Wallet";
            ViewBag.SendingCountryCurrency = sendingAmountData.SendingCurrencyCode;
            ViewBag.SendingAmount = sendingAmountData.SendingAmount;
            ViewBag.ReceiverName = receiverInfo.ReceiverName;
            // ViewBag.ReceivingCountry = sendingAmountData.ReceivingCountry.ToLower();
            ViewBag.Fee = sendingAmountData.Fee;

            vm.NameOnCard = senderInfo.SenderFullName;

            decimal addedBankFee = sendingAmountData.TotalAmount + vm.CreditDebitCardFee;

            vm.FaxingAmount = addedBankFee;
            vm.FaxingCurrencySymbol = sendingAmountData.SendingCurrencySymbol;
            vm.FaxingCurrency = sendingAmountData.SendingCurrencyCode;
            vm.SaveCard = IsAddDebitCreditCard;
            vm.ReceiverName = receiverInfo.ReceiverName;

            vm.AddressLineOne = senderInfo.AddressLine1;
            ViewBag.IsFromSavedDebitCard = IsFromSavedDebitCard;
            ViewBag.HasOneSavedCard = common.HasOneCardSaved(senderInfo.Id);
            SetTransactionSummary();
            return View(vm);

        }
        [HttpPost]
        public JsonResult ThreeDQuery([Bind(Include = CreditDebitCardViewModel.BindProperty)] CreditDebitCardViewModel vm)
        {

            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            var receiverinfo = _sAgentMobileTransferWalletServices.GetStaffReceiverDetailsInformation();
            var serviceResult = new ServiceResult<ThreeDRequestVm>();
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
            vm.NameOnCard = senderInfo.SenderFullName;

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
                CardName = senderInfo.SenderFullName,
                ExpirationMonth = vm.EndMM,
                ExpiringYear = vm.EndYY,
                Number = vm.CardNumber,
                SecurityCode = vm.SecurityCode,
                billingpostcode = senderInfo.PostCode,
                billingpremise = senderInfo.AddressLine1,
                CurrencyCode = Common.Common.GetCurrencyCode(senderInfo.Country)

            };

            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);

            string CardType = AdminSession.CardType;


            if (!StripeResult.IsValid)
            {
                serviceResult.Data = null;
                serviceResult.Message = StripeResult.Message;
                serviceResult.Status = ResultStatus.Error;
                return Json(serviceResult);
            }

            else

            {

                //  Fee Added statically (Bank Fee) it will be added dynamically after payment setup 
                //Credit/Debit card 
                //Fee: GBP 0.80
                //Manual Bank Deposit
                //Fee: GBP 0.79 

                var sendingAmountData = _sAgentMobileTransferWalletServices.GetMobileMoneyTransferEnterAmountViewModel();


                // paymentInfo.Fee add vairacha dai back garda 
                //decimal TotalFee = sendingAmountData.Fee + 0.05M;
                //decimal TotalSendingAmount = sendingAmountData.SendingAmount + sendingAmountData.Fee;




                SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
                _mobileMoneyTransferServices.SetStaffDebitCreditCardDetail(vm);


                decimal faxingAmount = sendingAmountData.TotalAmount + new CreditDebitCardViewModel().CreditDebitCardFee;
                decimal TotalAmount = Common.Common.CreditTypeFee(CardType, faxingAmount);


                var transferSummary = _senderForAllTransferServices.GetStaffTransactionSummary();
                transferSummary.CreditORDebitCardDetials = vm;
                transferSummary.CreditORDebitCardDetials.FaxingAmount = TotalAmount;

                _senderForAllTransferServices.SetStaffTransactionSummary(transferSummary);
                SetTransactionSummary();

                SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
                StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
                {
                    Amount = TotalAmount,
                    Currency = Common.Common.GetCountryCurrency(senderInfo.Country),
                    NameOnCard = "Charge for " + stripeResultIsValidCardVm.CardName,
                    StripeTokenId = StripeResult.StripeTokenId,
                    CardNum = stripeResultIsValidCardVm.Number,
                    ReceivingCountry = transferSummary.SenderAndReceiverDetail.ReceiverCountry,
                    SendingCountry = transferSummary.SenderAndReceiverDetail.SenderCountry,
                    ExipiryDate = stripeResultIsValidCardVm.ExpirationMonth + "/" + stripeResultIsValidCardVm.ExpiringYear,
                    SecurityCode = stripeResultIsValidCardVm.SecurityCode,
                    termurl = "/Admin/StaffOtherMobileWalletsTransfer/ThreeDQueryResponseCallBack",
                    billingpostcode = senderInfo.PostCode,
                    billingpremise = senderInfo.AddressLine1,
                    ReceiptNo = Common.AdminSession.ReceiptNo,
                    SenderEmail = senderInfo.Email,
                    SenderId = StaffInfo.StaffId,
                    SenderFirstName = senderInfo.FirstName,
                    SenderLastName = !string.IsNullOrEmpty(senderInfo.MiddleName) == true ? senderInfo.LastName + " " + senderInfo.LastName : senderInfo.LastName,
                };
                try
                {
                    var resultThreedQuery = StripServices.CreateThreedQuery(stripeCreateTransaction, TransactionTransferType.Admin, TransactionTransferMethod.OtherWallet);
                    serviceResult.Message = resultThreedQuery.Message;
                    serviceResult.Status = resultThreedQuery.Status;
                    serviceResult.Data = resultThreedQuery.Data;
                    serviceResult.IsGetType3dAuth = resultThreedQuery.IsGetType3dAuth;
                    if (serviceResult.Status == ResultStatus.OK)
                    {
                        vm.ThreeDEnrolled = serviceResult.Data.ThreeDEnrolled == "Y" ? true : false;
                        _mobileMoneyTransferServices.SetStaffDebitCreditCardDetail(vm);
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
                    Log.Write(ex.Message, ErrorType.PaymentGateway, "StaffOtherMobileWalletsTransfer/ThreeDQuery");
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

                var result = Transact365Serivces.GetTransationDetails(uid);
                if (result.Status == ResultStatus.Error)
                {
                    return RedirectToAction("DebitCreditCardDetails");
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ErrorType.PaymentGateway, "StaffOtherMobileWalletsTransfer/ThreeDQueryResponseCallBack");
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
                Log.Write(ex.Message, ErrorType.PaymentGateway, "StaffCashPickUpTransfer/ThreeDQueryResponseCallBack");
                return RedirectToAction("DebitCreditCardDetails");
            }
        }


        [HttpPost]
        [PreventSpam]
        public ActionResult DebitCreditCardDetails([Bind(Include = CustomerResponseVm.BindProperty)] CustomerResponseVm responseVm)
        {


            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            var transferSummary = _senderForAllTransferServices.GetStaffTransactionSummary();

            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            ViewBag.HasOneSavedCard = common.HasOneCardSaved(senderInfo.Id);
            var vm = new CreditDebitCardViewModel();
            vm = transferSummary.CreditORDebitCardDetials;
            if (senderInfo.Id == 0)
            {
                SFaxerSignUp faxerSignUpService = new SFaxerSignUp();
                string accountNo = faxerSignUpService.GetNewAccount(10);

                //var DOB = new DateTime().AddDays(sender.Day).AddMonths((int)sender.Month).AddYears(sender.Year);

                DB.FaxerInformation FaxerDetails = new DB.FaxerInformation()
                {
                    FirstName = senderInfo.FirstName,
                    MiddleName = senderInfo.MiddleName,
                    LastName = senderInfo.LastName,
                    Address1 = senderInfo.AddressLine1,
                    City = senderInfo.City,
                    Country = senderInfo.Country,
                    Email = senderInfo.Email,
                    PhoneNumber = senderInfo.MobileNo,
                    IdCardNumber = senderInfo.IdNumber,
                    IdCardType = senderInfo.IdType.ToString(),
                    IssuingCountry = senderInfo.IssuingCountry,
                    RegisteredByAgent = false,
                    IsDeleted = false,
                    IdCardExpiringDate = DateTime.Now,
                    AccountNo = accountNo,
                    DateOfBirth = senderInfo.DOB,
                    Address2 = senderInfo.AddressLine2,
                    GGender = senderInfo.Gender.ToInt()
                };
                AdminForAlTransferServices _adminForAlTransferServices = new AdminForAlTransferServices();
                var SenderAddedNew = _adminForAlTransferServices.AddSender(FaxerDetails);
                senderInfo.Id = SenderAddedNew.Id;

            }
            StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
            {
                Amount = transferSummary.CreditORDebitCardDetials.FaxingAmount,
                Currency = Common.Common.GetCurrencyCode(senderInfo.Country),
                md = responseVm.MD,
                pares = responseVm.PaRes,
                parenttransactionreference = responseVm.parenttransactionreference,
                billingpostcode = senderInfo.PostCode,
                billingpremise = senderInfo.AddressLine1,
                ReceiptNo = Common.AdminSession.ReceiptNo,
                SenderId = senderInfo.Id,
                ThreeDEnrolled = transferSummary.CreditORDebitCardDetials.ThreeDEnrolled,
                CardNum = vm.CardNumber
            };
            var cardProcessor = StripServices.GetCardProcessor(stripeCreateTransaction, TransactionTransferType.Admin, TransactionTransferMethod.BankDeposit);
            transferSummary.CardProcessorApi = cardProcessor;
            _senderForAllTransferServices.SetAdminTransactionSummary(transferSummary);
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
                        ModelState.AddModelError("StripeError", transactionResult.Message);
                        return View(vm);

                    }
                    if (transactionResult != null)
                    {
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
                        catch (Exception)
                        {

                        }
                    }

                    break;
                case CardProcessorApi.T365:
                    break;
            }
            var mobileMoneydata = _mobileMoneyTransferServices.list().Data.Where(x => x.Id == Common.AdminSession.TransactionId).FirstOrDefault();
            AdminForAlTransferServices AdminServices = new AdminForAlTransferServices();
            AdminServices.CompleteTransaction(transferSummary);
            return RedirectToAction("SenderPaymentMethodSuccess", "StaffOtherMobileWalletsTransfer");
        }

        public ActionResult SenderPaymentMethodSuccess()
        {
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();

            ViewBag.SenderName = senderInfo.SenderFullName;
            ViewBag.SenderCountry = common.getCountryNameFromCode(senderInfo.Country);
            if (senderInfo.Id > 0)
            {
                ViewBag.SenderAccountNo = common.GetSenderAccountNoBySenderId(senderInfo.Id);
            }
            var receiverInfo = _sAgentMobileTransferWalletServices.GetStaffReceiverDetailsInformation();
            var details = _sAgentMobileTransferWalletServices.GetStaffMobileMoneyTransferEnterAmountViewModel();
            SenderAddMoneySuccessVM model = new SenderAddMoneySuccessVM()
            {
                Amount = details.SendingAmount,
                Currnecy = details.SendingCurrencySymbol,
                ReceiverName = receiverInfo.ReceiverName,

            };

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            Common.AdminSession.SenderMobileMoneyTransfer = null;
            senderCommonFunc.ClearStaffMobileTransferSession();

            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Payment Confirmation");
            return View(model);

        }

        public JsonResult IsCrebitCard(string cardNumber, string month, string year, string securityCode)
        {
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();

            try
            {

                var number = cardNumber.Split(' ');
                cardNumber = string.Join("", number);
                StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
                {

                    CardName = senderInfo.SenderFullName,
                    ExpirationMonth = month,
                    ExpiringYear = year,
                    Number = cardNumber,
                    SecurityCode = securityCode,
                    billingpostcode = senderInfo.PostCode,
                    billingpremise = senderInfo.AddressLine1,
                    CurrencyCode = Common.Common.GetCurrencyCode(senderInfo.Country)



                };
                var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);

                decimal amount = _sAgentMobileTransferWalletServices.GetStaffMobileMoneyTransferEnterAmountViewModel().TotalAmount;
                amount = (decimal)Math.Round((0.60 / 100), 2) * amount;
                return Json(new
                {
                    IsCrebitCard = StripeResult.IsCreditCard,
                    ExtraAmount = Common.Common.GetCurrencySymbol(senderInfo.Country) + amount
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
        public void SetTransactionSummary()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();

            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

            var sendingAmountData = _sAgentMobileTransferWalletServices.GetStaffMobileMoneyTransferEnterAmountViewModel();

            var receiverDetails = _sAgentMobileTransferWalletServices.GetStaffReceiverDetailsInformation();
            //Completing Transaction
            var loggedUserData = Common.StaffSession.LoggedStaff;
            var paymentMethod = _mobileMoneyTransferServices.GetPaymentMethod();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();

            TransactionSummaryVM transactionSummaryVm = new TransactionSummaryVM();

            transactionSummaryVm.MobileMoneyTransferAgent = receiverDetails;
            int SenderWalletId = 0;
            var senderWalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(senderInfo.Id);
            if (senderWalletInfo != null)
            {

                SenderWalletId = senderWalletInfo.Id;
            }
            transactionSummaryVm.SenderAndReceiverDetail = new SenderAndReceiverDetialVM()
            {
                SenderId = senderInfo.Id,
                SenderCountry = senderInfo.Country,
                ReceiverCountry = receiverDetails.Country,
                SenderWalletId = SenderWalletId,
                WalletOperatorId = receiverDetails.MobileWalletProvider
                //ReceiverId = mobileTransfer.RecentReceiverId == null ? 0 : (int)mobileTransfer.RecentReceiverId

            };

            //Set Sms Fee 
            transactionSummaryVm.KiiPayTransferPaymentSummary = new KiiPayTransferPaymentSummary()
            {
                ReceiverName = receiverDetails.ReceiverName,
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


            var debitCreditCardDetail = _mobileMoneyTransferServices.GetStaffDebitCreditCardDetail();

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

            _senderForAllTransferServices.SetStaffTransactionSummary(transactionSummaryVm);


        }

        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount, bool IsReceivingAmount, string receiverCountry, string sendingCountry)
        {

            var paymentSummary = _sAgentMobileTransferWalletServices.GetStaffMobileMoneyTransferEnterAmountViewModel();

            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();

            string ReceivingCountry = receiverCountry;
            var SendingCurrencySymbol = Common.Common.GetCurrencySymbol(sendingCountry);
            var SendingCurrencyCode = Common.Common.GetCurrencyCode(sendingCountry);
            var RecevingCurrencySymbol = Common.Common.GetCurrencySymbol(receiverCountry);
            var ReceivingCurrencyCode = Common.Common.GetCurrencyCode(receiverCountry);

            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }

            var feeInfo = SEstimateFee.GetTransferFee(sendingCountry, ReceivingCountry, TransactionTransferMethod.OtherWallet, SendingAmount, TransactionTransferType.Admin);
            if (feeInfo == null)
            {

                return Json(new
                {
                    Fee = 0,
                    TotalAmount = 0,
                    ReceivingAmount = 0,
                    SendingAmount = 0,
                    AgentCommission = 0,
                    SendingCurrencySymbol = SendingCurrencySymbol,
                    RecevingCurrencySymbol = RecevingCurrencySymbol,
                    SendingCurrencyCode = SendingCurrencyCode,
                    ReceivingCurrency = ReceivingCurrencyCode,

                }, JsonRequestBehavior.AllowGet);
            }
            var result = new EstimateFaxingFeeSummary();
            int AgentId = 0;
            result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                SExchangeRate.GetExchangeRateValue(sendingCountry, ReceivingCountry, TransactionTransferMethod.OtherWallet, AgentId, TransactionTransferType.Admin), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(sendingCountry, ReceivingCountry, result.FaxingAmount
                , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, IsReceivingAmount, TransactionTransferMethod.OtherWallet, AgentId, false);

            if (introductoryRateResult != null)
            {

                result = introductoryRateResult;
            }


            //var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
            //    enterAmountData.ExchangeRate, SEstimateFee.GetFaxingCommision(agentCountry));
            // Rewrite session with additional value 
            paymentSummary.Fee = result.FaxingFee;
            paymentSummary.SendingAmount = result.FaxingAmount;
            paymentSummary.ReceivingAmount = result.ReceivingAmount;
            paymentSummary.TotalAmount = result.TotalAmount;
            paymentSummary.ExchangeRate = result.ExchangeRate;

            _sAgentMobileTransferWalletServices.SetStaffMobileMoneyTransferEnterAmountViewModel(paymentSummary);
            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount = result.FaxingAmount,
                ExchangeRate = result.ExchangeRate,
                SendingCurrencySymbol = SendingCurrencySymbol,
                RecevingCurrencySymbol = RecevingCurrencySymbol,
                SendingCurrencyCode = SendingCurrencyCode,
                ReceivingCurrency = ReceivingCurrencyCode,

            }, JsonRequestBehavior.AllowGet);
        }

        private void SetViewBagForPhoneNumbers(string Country)
        {
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            var recentlyPaidNumbers = _mobileMoneyTransferServices.GetRecentlyPaidNumbersForAgent(StaffId, DB.Module.Staff, Country);

            ViewBag.PhoneNumbers = new SelectList(recentlyPaidNumbers, "Code", "Name");
        }
        private void SetViewBagForMobileWalletProvider(string Country)
        {
            var wallets = _mobileMoneyTransferServices.GetWallets().Where(x => x.CountryCode == Country).ToList();
            ViewBag.MobileWalletProviders = new SelectList(wallets, "Id", "Name", Country);
        }

        private void SetViewBagForCountry(string Country)
        {
            var countries = common.GetCountries();
            ViewBag.countries = new SelectList(countries, "Code", "Name", Country);
        }

    }
}