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
using Twilio.Rest.Trunking.V1;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Areas.Admin.Controllers.Staff_Transactions
{
    public class StaffBankAccountDepositController : Controller
    {
        LoggedStaff StaffInfo = Common.StaffSession.LoggedStaff ?? new LoggedStaff();

        Admin.Services.CommonServices _CommonServices = null;
        SAgentBankAccountDeposit _agentBankAccountDepositServices = null;
        SCashPickUpTransferService _cashPickUp = null;
        SSenderBankAccountDeposit _senderBankAccountDepositServices = null;
        public StaffBankAccountDepositController()
        {
            _CommonServices = new Admin.Services.CommonServices();
            _agentBankAccountDepositServices = new SAgentBankAccountDeposit(Module.Staff);
            _cashPickUp = new SCashPickUpTransferService(Module.Staff);
            _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
        }
        // GET: Admin/StaffBankAccountDeposit
        public ActionResult Index(string AccountNoOrMobileNumber)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            string Country = "";
            SetCountryViewBag(Country);
            SetIdenitityTypeViewBag();
            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();
            vm = _cashPickUp.GetStaffCashPickupInformationViewModel();

            if (!string.IsNullOrEmpty(AccountNoOrMobileNumber))
            {

                AccountNoOrMobileNumber = Common.Common.IgnoreZero(AccountNoOrMobileNumber);
                var result = _cashPickUp.getFaxer(AccountNoOrMobileNumber, vm);
                SetCountryViewBag(result.Country);
                if (result != null)
                {
                    ViewBag.SenderCountryPhoneCode = Common.Common.GetCountryPhoneCode(result.Country);
                    if (result.Id != 0)
                    {

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

            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = CashPickupInformationViewModel.BindProperty)] CashPickupInformationViewModel vm)
        {
            SetCountryViewBag(vm.Country);
            SetIdenitityTypeViewBag();
            vm.SenderCountryCode = Common.Common.GetCountryPhoneCode(vm.IssuingCountry);


            var CurrentYear = DateTime.Now.Year;
            if (vm.DOB == null)
            {
                ModelState.AddModelError("", "Enter Date Of Birth .");
                return View(vm);
            }
            var DOB = vm.DOB;
            DateTime date = Convert.ToDateTime(DOB);
            var DOByear = date.Year;
            var Age = CurrentYear - DOByear;
            if (ModelState.IsValid)
            {
                if (Age <= 18)
                {
                    ModelState.AddModelError("InvalidAge", "Sender's should be more than 18 years to do the transaction.");
                    return View(vm);
                }
                if (vm.ExpiryDate < DateTime.Now)
                {
                    ModelState.AddModelError("IdExpired", "Expired ID");
                    return View(vm);
                }
                vm.SenderFullName = vm.FirstName + " " + vm.MiddleName + " " + vm.LastName;
                _cashPickUp.SetStaffCashPickupInformationViewModel(vm);

                return RedirectToAction("BankAccountDepositEnterAmount");
            }

            return View(vm);
        }

        public ActionResult BankAccountDeposit(string RecentAcccountNo = "")
        {
            var senderId = _cashPickUp.GetStaffCashPickupInformationViewModel().Id;
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
            SetViewBagForBanks(paymentInfo.ReceivingCountry, paymentInfo.ReceivingCurrency);
            SetViewBagForRecentAccountNumbers(paymentInfo.ReceivingCountry, RecentAcccountNo, senderId);
            SenderBankAccountDepositVm vm = new SenderBankAccountDepositVm();
            vm.CountryCode = paymentInfo.ReceivingCountry;
            vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(paymentInfo.ReceivingCountry);
            getRecentAccountno(ref vm, RecentAcccountNo, paymentInfo.ReceivingCountry);
            ViewBag.Branches = new SelectList(_senderBankAccountDepositServices.GetBranches(vm.BankId), "Code", "Name", vm.BranchCode);
            return View(vm);
        }
        [HttpPost]
        public ActionResult BankAccountDeposit([Bind(Include = SenderBankAccountDepositVm.BindProperty)] SenderBankAccountDepositVm vm)
        {
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();

            SetViewBagForBanks(vm.CountryCode, paymentInfo.ReceivingCurrency);
            SetViewBagForRecentAccountNumbers(vm.CountryCode, vm.RecentAccountNumber, senderInfo.Id);
            vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);
            ViewBag.Branches = new SelectList(_senderBankAccountDepositServices.GetBranches(vm.BankId), "Code", "Name", vm.BranchCode);
            vm.IsEuropeTransfer = _senderBankAccountDepositServices.IsEuropeTransfer(vm.CountryCode);
            vm.IsSouthAfricaTransfer = _senderBankAccountDepositServices.IsSouthAfricanTransfer(vm.CountryCode);
            vm.IsWestAfricaTransfer = _senderBankAccountDepositServices.IsWestAfricanTransfer(vm.CountryCode);
            if (ModelState.IsValid)
            {
                if (vm.CountryCode == "NG")
                {
                    vm.BranchCode = Common.Common.getBank(vm.BankId).Code;
                }
                if (!vm.IsEuropeTransfer) //have to select bank if it is not europe transfer  
                {
                    if (vm.BankId == 0)
                    {
                        ModelState.AddModelError("BankId", "Select Bank");
                        return View(vm);
                    }
                }
                else
                {
                    // Valid Bank Name if user is performing bank transfer to europe
                    if (string.IsNullOrEmpty(vm.BankName))
                    {
                        ModelState.AddModelError("BankName", "Enter Bank Name");
                        return View(vm);
                    }
                }
                if (vm.IsSouthAfricaTransfer)
                {
                    // validation
                    if (string.IsNullOrEmpty(vm.ReceiverStreet))
                    {
                        ModelState.AddModelError("ReceiverStreet", "Enter Address");
                        return View(vm);
                    }
                    if (string.IsNullOrEmpty(vm.ReceiverPostalCode))
                    {
                        ModelState.AddModelError("ReceiverPostalCode", "Enter Postcode");
                        return View(vm);
                    }
                    if (string.IsNullOrEmpty(vm.ReceiverCity))
                    {
                        ModelState.AddModelError("ReceiverCity", "Enter City");
                        return View(vm);
                    }
                    if (string.IsNullOrEmpty(vm.ReceiverEmail))
                    {
                        ModelState.AddModelError("ReceiverEmail", "Enter Email");
                        return View(vm);
                    }
                }
                bool IsValidBankDepositReceiver = Common.Common.IsValidBankDepositReceiver(vm.AccountNumber, Service.BankAccount);
                if (IsValidBankDepositReceiver == false)
                {
                    ModelState.AddModelError("", "Account no. not accepted");
                    return View(vm);
                }

                bool IsManualDeposit = Common.Common.IsManualDeposit(senderInfo.Country, vm.CountryCode);
                bool IsValidateAccountNo = true;

                if (!IsManualDeposit)
                {

                    var bankDeposit = _cashPickUp.GetStaffCashPickUpEnterAmount();
                    var IsValidAccountNo = _senderBankAccountDepositServices.IsValidBankAccount(vm, bankDeposit.SendingAmount,
                                           senderInfo.Country, TransactionTransferType.Admin);
                    if (IsValidAccountNo.Data == false)
                    {
                        ModelState.AddModelError("", IsValidAccountNo.Message);
                        return View(vm);
                    }
                }
                if (IsValidateAccountNo == true)
                {
                    vm.IsManualDeposit = IsManualDeposit;
                    _senderBankAccountDepositServices.SetStaffBankAccountDeposit(vm);
                    return RedirectToAction("TransactionSummary", "StaffBankAccountDeposit");
                }
                else
                {
                    ModelState.AddModelError("", "Enter a validate account number");
                    return View(vm);
                }
            }
            else
            {
                ModelState.AddModelError("", "Please enter the required field");
                return View(vm);
            }
        }

        public ActionResult BankAccountDepositEnterAmount()
        {
            CashPickUpEnterAmountViewModel vm = new CashPickUpEnterAmountViewModel();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();

            ViewBag.SenderName = senderInfo.SenderFullName;
            ViewBag.SenderAccountNo = _CommonServices.GetSenderAccountNoBySenderId(senderInfo.Id);
            ViewBag.SenderCountry = _CommonServices.getCountryNameFromCode(senderInfo.Country);

            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();
            vm.SendingCountry = senderInfo.Country;
            vm.SendingAmount = 1;
            vm.SendingCurrency = Common.Common.GetCountryCurrency(senderInfo.Country);
            vm.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(senderInfo.Country);

            ViewBag.TransferMethod = (int)TransactionServiceType.BankDeposit;
            return View(vm);
        }
        [HttpPost]
        public ActionResult BankAccountDepositEnterAmount([Bind(Include = CashPickUpEnterAmountViewModel.BindProperty)] CashPickUpEnterAmountViewModel model)
        {
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            ViewBag.SenderAccountNo = _CommonServices.GetSenderAccountNoBySenderId(senderInfo.Id);
            ViewBag.SenderCountry = _CommonServices.getCountryNameFromCode(senderInfo.Country);
            ViewBag.SenderName = senderInfo.SenderFullName;

            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();

            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();
            model.ReceivingCountry = paymentInfo.ReceivingCountryCode;
            ViewBag.TransferMethod = (int)TransactionServiceType.BankDeposit;
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
                if (senderInfo.Country == "GB")
                {
                    if (model.SendingAmount < 25 || model.SendingAmount > 50000)
                    {

                        ModelState.AddModelError("SendingAmount", "Enter amount higher than 25 and less than 50,000");
                        return View(model);
                    }
                }
                _cashPickUp.SetStaffCashPickUpEnterAmount(model);
                return RedirectToAction("BankAccountDeposit");
            }
            return View(model);

        }
        public ActionResult TransactionSummary()
        {
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
            var receiverinfo = _senderBankAccountDepositServices.GetStaffBankAccountDeposit();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();

            ViewBag.SenderName = senderInfo.SenderFullName;
            ViewBag.SenderAccountNo = _CommonServices.GetSenderAccountNoBySenderId(senderInfo.Id);
            ViewBag.SenderCountry = _CommonServices.getCountryNameFromCode(senderInfo.Country);

            string ReceiverFirstname = receiverinfo.AccountOwnerName.Split(' ')[0];
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
                TotalAmount = paymentInfo.TotalAmount,
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult TransactionSummary([Bind(Include = CommonTransactionSummaryViewModel.BindProperty)] CommonTransactionSummaryViewModel model)
        {

            if (ModelState.IsValid)
            {
                return RedirectToAction("SenderPaymentMethod", "StaffBankAccountDeposit");
            }

            return View(model);
        }

        public ActionResult SenderPaymentMethod()
        {
            PaymentMethodViewModel vm = new PaymentMethodViewModel();
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            ViewBag.SenderName = senderInfo.SenderFullName;
            ViewBag.SenderAccountNo = _CommonServices.GetSenderAccountNoBySenderId(senderInfo.Id);
            ViewBag.SenderCountry = _CommonServices.getCountryNameFromCode(senderInfo.Country);
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
            Common.AdminSession.ReceiptNo = Common.Common.GenerateBankAccountDepositReceiptNo(8);
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
                    _CommonServices.SetDebitCreditCardDetail(transactionSummaryvm.CreditORDebitCardDetials);

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
            ViewBag.SenderCountry = _CommonServices.getCountryNameFromCode(senderInfo.Country);
            if (senderInfo.Id > 0)
            {
                ViewBag.SenderAccountNo = _CommonServices.GetSenderAccountNoBySenderId(senderInfo.Id);
            }
            CreditDebitCardViewModel vm = new CreditDebitCardViewModel(senderInfo.Country);
            vm = _CommonServices.GetDebitCreditCardDetail(senderInfo.Country);
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
            var receiverinfo = _senderBankAccountDepositServices.GetStaffBankAccountDeposit();
            vm.FaxingAmount = paymentInfo.TotalAmount + vm.CreditDebitCardFee;
            vm.FaxingCurrencySymbol = paymentInfo.SendingCurrencySymbol;
            vm.FaxingCurrency = paymentInfo.SendingCurrency;
            vm.SaveCard = IsAddDebitCreditCard;
            vm.AddressLineOne = senderInfo.AddressLine1;
            vm.ReceiverName = receiverinfo.AccountOwnerName;
            ViewBag.IsFromSavedDebitCard = IsFromSavedDebitCard;
            ViewBag.HasOneSavedCard = _CommonServices.HasOneCardSaved(senderInfo.Id);
            SetTransactionSummary();
            return View(vm);
        }
        [HttpPost]
        public JsonResult ThreeDQuery([Bind(Include = CreditDebitCardViewModel.BindProperty)] CreditDebitCardViewModel vm)
        {
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
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
                _CommonServices.SetDebitCreditCardDetail(vm);

                decimal faxingAmount = paymentInfo.TotalAmount + new CreditDebitCardViewModel(senderInfo.Country).CreditDebitCardFee;
                decimal TotalAmount = Common.Common.CreditTypeFee(CardType, faxingAmount);
                var transferSummary = _senderForAllTransferServices.GetAdminTransactionSummary();
                transferSummary.CreditORDebitCardDetials = vm;
                transferSummary.CreditORDebitCardDetials.FaxingAmount = TotalAmount;
                transferSummary.KiiPayTransferPaymentSummary.Fee = paymentInfo.Fee;
                transferSummary.KiiPayTransferPaymentSummary.TotalAmount = paymentInfo.TotalAmount;
                _senderForAllTransferServices.SetAdminTransactionSummary(transferSummary);

                SetTransactionSummary();
                //transferSummary.SenderAndReceiverDetail =  ;

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
                    termurl = "/Admin/StaffBankAccountDeposit/ThreeDQueryResponseCallBack",
                    billingpostcode = senderInfo.PostCode,
                    billingpremise = senderInfo.AddressLine1,
                    ReceiptNo = Common.AdminSession.ReceiptNo,
                    SenderId = StaffInfo.StaffId,
                    SenderEmail = senderInfo.Email,
                    SenderFirstName = senderInfo.FirstName,
                    SenderLastName = !string.IsNullOrEmpty(senderInfo.MiddleName) == true ? senderInfo.LastName + " " + senderInfo.LastName : senderInfo.LastName,
                };
                try
                {

                    var resultThreedQuery = StripServices.CreateThreedQuery(stripeCreateTransaction, TransactionTransferType.Admin, TransactionTransferMethod.BankDeposit);
                    serviceResult.Message = resultThreedQuery.Message;
                    serviceResult.Status = resultThreedQuery.Status;
                    serviceResult.Data = resultThreedQuery.Data;
                    serviceResult.IsGetType3dAuth = resultThreedQuery.IsGetType3dAuth;
                    if (serviceResult.Status == ResultStatus.OK)
                    {
                        vm.ThreeDEnrolled = serviceResult.Data.ThreeDEnrolled == "Y" ? true : false;
                        _CommonServices.SetDebitCreditCardDetail(vm);
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
                    Log.Write(ex.Message, ErrorType.PaymentGateway, "StaffBankAccountDeposit/ThreeDQuery");
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
                Log.Write(ex.Message, ErrorType.PaymentGateway, "StaffBankAccountDeposit/ThreeDQueryResponseCallBack");
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
                Log.Write(ex.Message, ErrorType.PaymentGateway, "StaffBankAccountDeposit/ThreeDQueryResponseCallBack");
                return RedirectToAction("DebitCreditCardDetails");
            }

        }

        [HttpPost]
        //[PreventSpam]
        public ActionResult DebitCreditCardDetails([Bind(Include = CustomerResponseVm.BindProperty)] CustomerResponseVm responseVm)
        {

            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();

            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            ViewBag.HasOneSavedCard = _CommonServices.HasOneCardSaved(senderInfo.Id);

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
            return RedirectToAction("SenderPaymentMethodSuccess", "StaffBankAccountDeposit");
        }
        public ActionResult SenderPaymentMethodSuccess()
        {
            // Data to show on success
            SenderCashPickUpSuccessVm vm = new SenderCashPickUpSuccessVm();
            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
            var receiverinfo = _senderBankAccountDepositServices.GetStaffBankAccountDeposit();
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();
            var paymentMethod = _cashPickUp.GetStaffPaymentMethod();
            ViewBag.SenderName = senderInfo.SenderFullName;
            ViewBag.SenderCountry = _CommonServices.getCountryNameFromCode(senderInfo.Country);
            if (senderInfo.Id > 0)
            {
                ViewBag.SenderAccountNo = _CommonServices.GetSenderAccountNoBySenderId(senderInfo.Id);

            }
            vm.SentAmount = paymentInfo.SendingAmount;
            vm.SendingCurrency = paymentInfo.SendingCurrencySymbol;
            vm.ReceiverName = receiverinfo.AccountOwnerName;
            ViewBag.Sendername = senderInfo.SenderFullName;

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();

            senderCommonFunc.ClearTransferBankDepositSession();
            senderCommonFunc.ClearCashPickUpSession();
            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Payment Confirmation");
            return View(vm);


        }
        public void SetTransactionSummary()
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

            var paymentInfo = _cashPickUp.GetStaffCashPickUpEnterAmount();
            var bankAccountDeposit = _senderBankAccountDepositServices.GetStaffBankAccountDeposit();
            //Completing Transaction
            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();

            var paymentMethod = _cashPickUp.GetStaffPaymentMethod();

            TransactionSummaryVM transactionSummaryVm = new TransactionSummaryVM();
            transactionSummaryVm.BankAccountDeposit = bankAccountDeposit;
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
                ReceiverCountry = bankAccountDeposit.CountryCode,
                SenderWalletId = SenderWalletId
                //ReceiverId = bankAccountDeposit.RecentReceiverId == null ? 0 : (int)bankAccountDeposit.RecentReceiverId

            };

            //Set Sms Fee 
            transactionSummaryVm.KiiPayTransferPaymentSummary = new KiiPayTransferPaymentSummary()
            {
                ReceiverName = bankAccountDeposit.AccountOwnerName,
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
                EnableAutoPayment = false,
            };

            //For DebitCreditCardDetail


            var debitCreditCardDetail = _CommonServices.GetDebitCreditCardDetail(senderInfo.Country);

            transactionSummaryVm.CreditORDebitCardDetials = debitCreditCardDetail;
            transactionSummaryVm.CreditORDebitCardDetials.FaxingAmount = paymentInfo.TotalAmount;


            var moneyFexBankAccountDepositData = _senderBankAccountDepositServices.GetMoneyFexBankAccountDeposit();
            transactionSummaryVm.MoneyFexBankDeposit = moneyFexBankAccountDepositData;

            transactionSummaryVm.TransferType = TransferType.BankDeposit;
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
            var cardProcessor = StripServices.GetCardProcessor(stripeCreateTransaction, TransactionTransferType.Admin, TransactionTransferMethod.BankDeposit);
            transactionSummaryVm.CardProcessorApi = cardProcessor;

            _senderForAllTransferServices.SetTransactionSummary(transactionSummaryVm);

            _senderForAllTransferServices.GenerateReceiptNoForBankDepoist(transactionSummaryVm.BankAccountDeposit.IsManualDeposit);

            _senderForAllTransferServices.SetAdminTransactionSummary(transactionSummaryVm);

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

            var feeInfo = SEstimateFee.GetTransferFee(sendingCountry, ReceivingCountry, TransactionTransferMethod.BankDeposit, SendingAmount, TransactionTransferType.Admin);
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
                SExchangeRate.GetExchangeRateValue(sendingCountry, ReceivingCountry, TransactionTransferMethod.BankDeposit, AgentId, TransactionTransferType.Admin), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(sendingCountry, ReceivingCountry, result.FaxingAmount
                , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, IsReceivingAmount, TransactionTransferMethod.BankDeposit, AgentId, false);

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

        public JsonResult GetBankCode(int BankId)
        {

            var bankCode = _senderBankAccountDepositServices.GetBankCode(BankId);

            var branches = _senderBankAccountDepositServices.GetBranches(BankId);
            return Json(new
            {
                BranchCode = bankCode.Code,
                Branches = branches
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetAccountInformation(string accountNo, string CountryCode)
        {


            var accountData = _senderBankAccountDepositServices.GetAccountInformationFromAccountNumber(accountNo, CountryCode);
            if (accountData != null)
            {
                return Json(new
                {
                    AccountOwnerName = accountData.AccountOwnerName,
                    Country = accountData.CountryCode.ToUpper(),
                    CountryPhoneCode = accountData.CountryPhoneCode,
                    MobileNumber = accountData.MobileNumber,
                    AccountNumber = accountData.AccountNumber,
                    BankId = accountData.BankId,
                    BranchCode = accountData.BranchCode
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }


        public void SetCountryViewBag(string country = "")
        {
            var countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name", country);
        }
        public void SetIdenitityTypeViewBag()
        {
            var IdentityCards = _CommonServices.GetCardType();
            ViewBag.IdTypes = new SelectList(IdentityCards, "Id", "CardType");
        }


        private void SetViewBagForBanks(string Country, string currency)
        {
            var banks = _senderBankAccountDepositServices.getBanksByCurrency(Country, currency);
            ViewBag.BankNames = new SelectList(banks, "Id", "Name");
        }
        public void SetViewBagForRecentAccountNumbers(string Country = "", string RecentAcccountNo = "", int senderId = 0)
        {
            var recentAccountNumbers = new List<Common.DropDownViewModel>();
            if (!string.IsNullOrEmpty(Country))
            {
                recentAccountNumbers = _senderBankAccountDepositServices.GetStaffRecentAccountNumbers(senderId).Where(x => x.CountryCode == Country).ToList();
            }
            ViewBag.RecentAccountNumbers = new SelectList(recentAccountNumbers, "Code", "Name", RecentAcccountNo);
        }
        public void getRecentAccountno(ref SenderBankAccountDepositVm vm, string accountNo, string Country = "")
        {
            var accountData = _senderBankAccountDepositServices.GetAccountInformationFromAccountNumber(accountNo, Country);

            if (accountData != null)
            {

                if (Country.ToLower() != accountData.CountryCode.ToLower())
                {
                    vm.AccountOwnerName = "";
                    vm.MobileNumber = "";
                    vm.AccountNumber = "";
                    vm.BankId = 0;
                    vm.BranchCode = "";
                }
                else
                {
                    vm.AccountOwnerName = accountData.AccountOwnerName;
                    vm.MobileNumber = accountData.MobileNumber;
                    vm.AccountNumber = accountData.AccountNumber;
                    vm.BankId = accountData.BankId;
                    vm.BranchCode = accountData.BranchCode;
                    vm.ReceiverCity = accountData.ReceiverCity;
                    vm.ReceiverEmail = accountData.ReceiverEmail;
                    vm.ReceiverPostalCode = accountData.ReceiverPostalCode;
                    vm.ReceiverStreet = accountData.ReceiverStreet;
                }

            }
            if (!string.IsNullOrEmpty(Country))
            {
                vm.IsEuropeTransfer = _senderBankAccountDepositServices.IsEuropeTransfer(Country);
                vm.IsSouthAfricaTransfer = _senderBankAccountDepositServices.IsSouthAfricanTransfer(Country);
                vm.IsWestAfricaTransfer = _senderBankAccountDepositServices.IsWestAfricanTransfer(Country);
            }
        }

        public JsonResult GetCountryPhoneCode(string CountryCode)
        {

            string PhoneCode = Common.Common.GetCountryPhoneCode(CountryCode);
            return Json(new
            {

                CountryPhoneCode = PhoneCode
            }, JsonRequestBehavior.AllowGet);
        }

    }
}