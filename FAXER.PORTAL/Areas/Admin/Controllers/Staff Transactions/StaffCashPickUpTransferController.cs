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
    public class StaffCashPickUpTransferController : Controller
    {
        // GET: Admin/StaffCashPickUpTransfer
        LoggedStaff StaffInfo = Common.StaffSession.LoggedStaff ?? new LoggedStaff();
        Admin.Services.CommonServices common = new Admin.Services.CommonServices();

        SCashPickUpTransferService _cashPickUp = new SCashPickUpTransferService(Module.Staff);
        AgentCommonServices _commonServices = new AgentCommonServices();
        SSenderCashPickUp _cashPickUpSender = new SSenderCashPickUp();
        public ActionResult Index(string AccountNoORPhoneNo = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
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
                return RedirectToAction("CashPickUpEnterAmount");
            }
            return View(Vm);
        }

        public ActionResult CashPickUpEnterAmount()
        {
            CashPickUpEnterAmountViewModel vm = new CashPickUpEnterAmountViewModel();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();

            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();
            ViewBag.SenderName = senderInfo.SenderFullName;
            ViewBag.SenderAccountNo = common.GetSenderAccountNoBySenderId(senderInfo.Id);
            ViewBag.SenderCountry = common.getCountryNameFromCode(senderInfo.Country);
            vm.SendingCountry = senderInfo.Country;
            vm.SendingCurrencyCode = Common.Common.GetCountryCurrency(senderInfo.Country);
            vm.SendingCurrency = Common.Common.GetCountryCurrency(senderInfo.Country);
            vm.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(senderInfo.Country);
            ViewBag.TransferMethod = (int)TransactionServiceType.CashPickUp;
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
            if (!string.IsNullOrEmpty(paymentInfo.ReceivingCountry))
            {
                vm = paymentInfo;
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult CashPickUpEnterAmount([Bind(Include = CashPickUpEnterAmountViewModel.BindProperty)] CashPickUpEnterAmountViewModel model)
        {
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();
            ViewBag.TransferMethod = (int)TransactionServiceType.CashPickUp;

            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();
            model.ReceivingCountry = paymentInfo.ReceivingCountryCode;
            if (ModelState.IsValid)
            {
                if (model.SendingAmount == 0)
                {
                    ModelState.AddModelError("SendingAmount", "Please Enter Sending Amount ");
                    return View(model);
                }
                if (model.ReceivingAmount == 0)
                {
                    ModelState.AddModelError("RecevingAmount", "Please Enter Sending Amount ");
                    return View(model);
                }
                if (string.IsNullOrEmpty((model.Fee).ToString()))
                {
                    ModelState.AddModelError("FaxingFee", "Please calculate estimated fee ");
                    return View(model);
                }
                _cashPickUp.SetStaffCashPickUpEnterAmount(model);
                return RedirectToAction("CashPickupDetails");
            }
            return View();

        }

        public ActionResult CashPickupDetails(int receiverId = 0)
        {
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
            int Id = senderInfo.Id;
            string Country = paymentInfo.ReceivingCountry;
            SetViewBagForIdCardType();
            var existingReceiver = _cashPickUp.getExistingReceiver(Id, Country);
            ViewBag.existingReceiver = new SelectList(existingReceiver, "Id", "FirstName");
            CashPickUpReceiverDetailsInformationViewModel vm = _cashPickUp.GetCashPickUpReceiverInfoViewModel();
            vm.Country = Country;
            vm.MobileCode = Common.Common.GetCountryPhoneCode(Country);
            if (receiverId > 0)
            {
                vm = _cashPickUp.GetReceiverDetailsById(receiverId);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult CashPickupDetails([Bind(Include = CashPickUpReceiverDetailsInformationViewModel.BindProperty)] CashPickUpReceiverDetailsInformationViewModel Vm)
        {
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();

            int Id = senderInfo.Id;
            SetViewBagForIdCardType();

            var existingReceiver = _cashPickUp.getExistingReceiver(Id, Vm.Country);
            ViewBag.existingReceiver = new SelectList(existingReceiver, "Id", "FirstName");

            Vm.MobileCode = Common.Common.GetCountryPhoneCode(Vm.Country);

            if (Vm.ReasonForTransfer == PORTAL.Models.ReasonForTransfer.Non)
            {
                ModelState.AddModelError("Reason", "Select Reason");
                return View(Vm);
            }

            if (string.IsNullOrEmpty(Vm.ReceiverFullName))
            {
                ModelState.AddModelError("ReceiverFullName", "Enter Receiver Name");
                return View(Vm);
            }
            if (string.IsNullOrEmpty(Vm.MobileNo))
            {
                ModelState.AddModelError("MobileNo", "Enter Mobile No");
                return View(Vm);
            }
            if (Vm.Country == "MA")
            {
                var Apiservice = Common.Common.GetApiservice(senderInfo.Country, Vm.Country, paymentInfo.SendingAmount,
                    TransactionTransferMethod.CashPickUp, TransactionTransferType.Admin);
                if (Apiservice == null)
                {
                    ModelState.AddModelError("ServiceNotAvialable", "Service Not Available");
                    return View(Vm);
                }
                if (Vm.IdenityCardId < 0)
                {
                    ModelState.AddModelError("IdenityCardId", "Select Id card type");
                    return View(Vm);
                }
                if (string.IsNullOrEmpty(Vm.IdentityCardNumber))
                {
                    ModelState.AddModelError("IdentityCardNumber", "Enter card number");
                    return View(Vm);
                }

                SmsApi smsApi = new SmsApi();
                var IsValidMobileNo = smsApi.IsValidMobileNo(Vm.MobileCode + "" + Vm.MobileNo);
                if (IsValidMobileNo == false)
                {
                    ModelState.AddModelError("", "Enter Valid Number");
                    return View(Vm);
                }
            }
            bool IsValidReceiver = Common.Common.IsValidBankDepositReceiver(Vm.MobileNo, Service.CashPickUP);
            if (IsValidReceiver == false)
            {
                ModelState.AddModelError("InvalidReceiver", " Receiver is banned");
                return View(Vm);
            }
            _cashPickUp.SetStaffCashPickUpReceiverInfoViewModel(Vm);
            return RedirectToAction("TransactionSummary");
        }
        public ActionResult TransactionSummary()
        {
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();

            var receiverinfo = _cashPickUp.GetStaffCashPickUpReceiverInfoViewModel();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            ViewBag.SenderName = senderInfo.SenderFullName;
            if (senderInfo.Id > 0)
            {
                ViewBag.SenderAccountNo = common.GetSenderAccountNoBySenderId(senderInfo.Id);
                ViewBag.SenderCountry = common.getCountryNameFromCode(senderInfo.Country);
            }
            string ReceiverFirstname = receiverinfo.ReceiverFullName.Split(' ')[0];
            CommonTransactionSummaryViewModel vm = new CommonTransactionSummaryViewModel()
            {
                Fee = paymentInfo.Fee,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                ReceivingCurrecyCode = paymentInfo.ReceivingCurrency,
                ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCurrencyCode = paymentInfo.SendingCurrency,
                SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol,
                ReceiverFirstName = ReceiverFirstname,
                TotalAmount = paymentInfo.TotalAmount

            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult TransactionSummary([Bind(Include = CommonTransactionSummaryViewModel.BindProperty)] CommonTransactionSummaryViewModel model)
        {

            if (ModelState.IsValid)
            {
                return RedirectToAction("SenderPaymentMethod", "StaffCashPickUpTransfer");
            }

            return View(model);
        }

        public ActionResult SenderPaymentMethod()
        {
            PaymentMethodViewModel vm = new PaymentMethodViewModel();
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
            var receiverinfo = _cashPickUp.GetStaffCashPickUpReceiverInfoViewModel();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            ViewBag.SenderName = senderInfo.SenderFullName;
            if (senderInfo.Id > 0)
            {
                ViewBag.SenderAccountNo = common.GetSenderAccountNoBySenderId(senderInfo.Id);
                ViewBag.SenderCountry = common.getCountryNameFromCode(senderInfo.Country);
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
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
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
            _cashPickUp.SetStaffPaymentMethod(vm);
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

            var transactionSummaryvm = _senderForAllTransferServices.GetAdminTransactionSummary();
            Common.AdminSession.ReceiptNo = Common.Common.GenerateCashPickUpReceiptNo(6);
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
                        CurrencyCode = Common.Common.GetCurrencyCode(senderInfo.Country),
                        SenderId = senderInfo.Id
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
            ViewBag.SenderName = senderInfo.SenderFullName;
            ViewBag.SenderCountry = common.getCountryNameFromCode(senderInfo.Country);
            ViewBag.SenderAccountNo = common.GetSenderAccountNoBySenderId(senderInfo.Id);
            CreditDebitCardViewModel vm = new CreditDebitCardViewModel(senderInfo.Country);
            vm = common.GetDebitCreditCardDetail(senderInfo.Country);
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
            var receiverinfo = _cashPickUp.GetStaffCashPickUpReceiverInfoViewModel();

            vm.FaxingAmount = paymentInfo.TotalAmount + vm.CreditDebitCardFee;
            vm.FaxingCurrencySymbol = paymentInfo.SendingCurrencySymbol;
            vm.FaxingCurrency = paymentInfo.SendingCurrency;
            vm.SaveCard = IsAddDebitCreditCard;
            vm.AddressLineOne = senderInfo.AddressLine1;
            vm.ReceiverName = receiverinfo.ReceiverFullName;
            ViewBag.IsFromSavedDebitCard = IsFromSavedDebitCard;
            ViewBag.HasOneSavedCard = common.HasOneCardSaved(senderInfo.Id);
            SetTransactionSummary();
            return View(vm);
        }

        [HttpPost]
        public JsonResult ThreeDQuery([Bind(Include = CreditDebitCardViewModel.BindProperty)] CreditDebitCardViewModel vm)
        {
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
            var receiverinfo = _cashPickUp.GetStaffCashPickUpReceiverInfoViewModel();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            vm.NameOnCard = senderInfo.SenderFullName;
            var serviceResult = new ServiceResult<ThreeDRequestVm>();
            if (string.IsNullOrEmpty(vm.CardNumber))
            {

                ModelState.AddModelError("CardNumber", "Enter Card Number");
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
                common.SetDebitCreditCardDetail(vm);


                decimal faxingAmount = paymentInfo.TotalAmount + new CreditDebitCardViewModel(senderInfo.Country).CreditDebitCardFee;
                decimal TotalAmount = Common.Common.CreditTypeFee(CardType, faxingAmount);

                var transferSummary = _senderForAllTransferServices.GetAdminTransactionSummary();
                transferSummary.CreditORDebitCardDetials = vm;
                transferSummary.CreditORDebitCardDetials.FaxingAmount = TotalAmount;
                transferSummary.KiiPayTransferPaymentSummary.Fee = paymentInfo.Fee;
                transferSummary.KiiPayTransferPaymentSummary.TotalAmount = paymentInfo.TotalAmount;
                _senderForAllTransferServices.SetAdminTransactionSummary(transferSummary);

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
                    termurl = "/Admin/StaffCashPickUpTransfer/ThreeDQueryResponseCallBack",
                    billingpostcode = senderInfo.PostCode,
                    billingpremise = senderInfo.AddressLine1,
                    ReceiptNo = Common.AdminSession.ReceiptNo,
                    SenderEmail = senderInfo.Email,
                    SenderFirstName = senderInfo.FirstName,
                    SenderLastName = !string.IsNullOrEmpty(senderInfo.MiddleName) == true ? senderInfo.LastName + " " + senderInfo.LastName : senderInfo.LastName,
                    SenderId = StaffInfo.StaffId,
                };

                try
                {
                    var resultThreedQuery = StripServices.CreateThreedQuery(stripeCreateTransaction, TransactionTransferType.Admin, TransactionTransferMethod.CashPickUp);
                    serviceResult.Message = resultThreedQuery.Message;
                    serviceResult.Status = resultThreedQuery.Status;
                    serviceResult.Data = resultThreedQuery.Data;
                    serviceResult.IsGetType3dAuth = resultThreedQuery.IsGetType3dAuth;
                    if (serviceResult.Status == ResultStatus.OK)
                    {
                        vm.ThreeDEnrolled = serviceResult.Data.ThreeDEnrolled == "Y" ? true : false;
                        common.SetDebitCreditCardDetail(vm);
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
                    Log.Write(ex.Message, ErrorType.PaymentGateway, "StaffCashPickUpTransfer/ThreeDQuery");
                }
            }
            return Json(serviceResult);
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

                decimal amount = _cashPickUp.GetStaffCashPickUpEnterAmount().TotalAmount;
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
                Log.Write(ex.Message, ErrorType.PaymentGateway, "StaffCashPickUpTransfer/ThreeDQueryResponseCallBack");
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

            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();

            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            ViewBag.HasOneSavedCard = common.HasOneCardSaved(senderInfo.Id);

            var transferSummary = _senderForAllTransferServices.GetAdminTransactionSummary();
            var vm = new CreditDebitCardViewModel();
            vm = transferSummary.CreditORDebitCardDetials;
            AdminForAlTransferServices _adminForAlTransferServices = new AdminForAlTransferServices();

            if (senderInfo.Id == 0)
            {
                SFaxerSignUp faxerSignUpService = new SFaxerSignUp();
                string accountNo = faxerSignUpService.GetNewAccount(10);


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
                    break;
                case CardProcessorApi.T365:
                    break;
            }
            _adminForAlTransferServices.CompleteTransaction(transferSummary);
            return RedirectToAction("SenderPaymentMethodSuccess", "StaffCashPickUpTransfer");
        }
        public ActionResult SenderPaymentMethodSuccess()
        {
            // Data to show on success
            SenderCashPickUpSuccessVm vm = new SenderCashPickUpSuccessVm();
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
            var receiverinfo = _cashPickUp.GetStaffCashPickUpReceiverInfoViewModel();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            var paymentMethod = _cashPickUp.GetStaffPaymentMethod();
            ViewBag.SenderName = senderInfo.SenderFullName;
            ViewBag.SenderCountry = common.getCountryNameFromCode(senderInfo.Country);
            if (senderInfo.Id > 0)
            {
                ViewBag.SenderAccountNo = common.GetSenderAccountNoBySenderId(senderInfo.Id);

            }
            vm.SentAmount = paymentInfo.SendingAmount;
            vm.SendingCurrency = paymentInfo.SendingCurrencySymbol;
            vm.ReceiverName = receiverinfo.ReceiverFullName;
            ViewBag.Sendername = senderInfo.SenderFullName;

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();

            senderCommonFunc.ClearCashPickUpSession();
            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Payment Confirmation");
            return View(vm);


        }

        public void SetTransactionSummary()
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
            var receiverinfo = _cashPickUp.GetStaffCashPickUpReceiverInfoViewModel();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            var paymentMethod = _cashPickUp.GetStaffPaymentMethod();

            //Completing Transaction
            TransactionSummaryVM transactionSummaryVm = new TransactionSummaryVM();
            //transactionSummaryVm.KiiPayTransferPaymentSummary = cashPickUp;
            transactionSummaryVm.CashPickUpVmStaff = receiverinfo;
            int senderWalletId = 0;
            var senderWalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(senderInfo.Id);
            if (senderWalletInfo != null)
            {

                senderWalletId = senderWalletInfo.Id;
            }
            transactionSummaryVm.SenderAndReceiverDetail = new SenderAndReceiverDetialVM()
            {
                SenderId = senderInfo.Id,
                SenderCountry = senderInfo.Country,
                ReceiverCountry = paymentInfo.ReceivingCountry,
                ReceiverId = 0,
                SenderWalletId = senderWalletId,
                SenderPhoneNo = senderInfo.MobileNo,
            };

            //Set Sms Fee 
            transactionSummaryVm.KiiPayTransferPaymentSummary = new KiiPayTransferPaymentSummary()
            {
                ReceiverName = receiverinfo.ReceiverFullName,
                SendingCurrency = paymentInfo.SendingCurrency,
                SendingAmount = paymentInfo.SendingAmount,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                TotalAmount = paymentInfo.TotalAmount,
                ExchangeRate = paymentInfo.ExchangeRate,
                Fee = paymentInfo.Fee,
                PaymentReference = "",
                ReceivingCurrency = paymentInfo.ReceivingCurrency,
                ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol,
                SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol,
                SendSMS = true,
                SMSFee = 0,
            };

            transactionSummaryVm.PaymentMethodAndAutoPaymentDetail = new PaymentMethodViewModel()
            {
                TotalAmount = paymentInfo.TotalAmount,
                SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol,
                SenderPaymentMode = paymentMethod.SenderPaymentMode,
                EnableAutoPayment = false
            };

            //For DebitCreditCardDetail
            var debitCreditCardDetail = common.GetDebitCreditCardDetail(senderInfo.Country);

            transactionSummaryVm.CreditORDebitCardDetials = debitCreditCardDetail;

            //var moneyFexBankAccountDepositData = _cashPickUpServices.GetMoneyFexBankAccountDeposit();
            //transactionSummaryVm.MoneyFexBankDeposit = moneyFexBankAccountDepositData;

            transactionSummaryVm.TransferType = TransferType.CashPickup;
            if (transactionSummaryVm.SenderAndReceiverDetail.SenderCountry == transactionSummaryVm.SenderAndReceiverDetail.ReceiverCountry)
            {
                transactionSummaryVm.IsLocalPayment = true;
            }
            else
            {
                transactionSummaryVm.IsLocalPayment = false;
            }
            var stripeCreateTransaction = new StripeCreateTransactionVM()
            {
                Currency = paymentInfo.SendingCurrency,
                ReceivingCurrency = paymentInfo.ReceivingCurrency,
                SendingCountry = paymentInfo.SendingCountry,
                ReceivingCountry = paymentInfo.ReceivingCountry,
                Amount = paymentInfo.SendingAmount,
            };
            var cardProcessor = StripServices.GetCardProcessor(stripeCreateTransaction,
                                                               TransactionTransferType.Admin, TransactionTransferMethod.CashPickUp);
            transactionSummaryVm.CardProcessorApi = cardProcessor;

            _senderForAllTransferServices.SetAdminTransactionSummary(transactionSummaryVm);
        }
        private void SetViewBagForCountry(string Country)
        {
            var countries = common.GetCountries();
            ViewBag.countries = new SelectList(countries, "Code", "Name", Country);
        }
        private void SetViewBagForIdCardType()
        {
            var IdCardType = common.GetCardType();
            ViewBag.IdCardTypes = new SelectList(IdCardType, "Id", "CardType");

        }
        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount, bool IsReceivingAmount, string receiverCountry, string sendingCountry)
        {

            var paymentSummary = _cashPickUp.GetStaffCashPickUpEnterAmount();

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

            var feeInfo = SEstimateFee.GetTransferFee(sendingCountry, ReceivingCountry, TransactionTransferMethod.CashPickUp, SendingAmount, TransactionTransferType.Admin);
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
                SExchangeRate.GetExchangeRateValue(sendingCountry, ReceivingCountry, TransactionTransferMethod.CashPickUp, AgentId, TransactionTransferType.Admin), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(sendingCountry, ReceivingCountry, result.FaxingAmount
                , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, IsReceivingAmount, TransactionTransferMethod.CashPickUp, AgentId, false);

            if (introductoryRateResult != null)
            {

                result = introductoryRateResult;
            }

            paymentSummary.Fee = result.FaxingFee;
            paymentSummary.SendingAmount = result.FaxingAmount;
            paymentSummary.ReceivingAmount = result.ReceivingAmount;
            paymentSummary.TotalAmount = result.TotalAmount;
            paymentSummary.ExchangeRate = result.ExchangeRate;

            _cashPickUp.SetStaffCashPickUpEnterAmount(paymentSummary);
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


        public List<PreviousReceiverVm> GetPreviousReceivers()
        {
            var result = new List<PreviousReceiverVm>();
            return result;
        }

        public ActionResult GetPhoneCode(string countryCode)
        {
            var phoneCode = common.getPhoneCodeFromCountry(countryCode);
            return Json(new
            {
                MobileCode = phoneCode
            }, JsonRequestBehavior.AllowGet);
        }
    }
}