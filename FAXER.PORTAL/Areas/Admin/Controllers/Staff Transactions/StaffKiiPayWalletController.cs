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
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Areas.Admin.Controllers.Staff_Transactions
{
    public class StaffKiiPayWalletController : Controller
    {

        LoggedStaff StaffInfo = Common.StaffSession.LoggedStaff ?? new LoggedStaff();
        Admin.Services.CommonServices _commonServices = new Admin.Services.CommonServices();
        SAgentKiiPayWalletTransferServices _kiiPayWalletTransfer = new SAgentKiiPayWalletTransferServices();
        // GET: Admin/StaffKiiPayWallet

        [HttpGet]
        public ActionResult Index(string AccountNoORPhoneNo = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            Common.FaxerSession.TransferMethod = "kiipaywallet";

            var countries = _commonServices.GetCountries();
            var identifyCardType = _commonServices.GetCardType();

            ViewBag.countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType");
            SendMoneToKiiPayWalletViewModel vm = new SendMoneToKiiPayWalletViewModel();
            if (AccountNoORPhoneNo != "")
            {

                var result = _kiiPayWalletTransfer.getFaxerInfo(AccountNoORPhoneNo, vm);

                if (result != null)
                {
                    ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType", result.IdType);
                    ViewBag.countries = new SelectList(countries, "Code", "Name", result.Country);
                    string FaxerCountryPhoneCode = Common.Common.GetCountryPhoneCode(result.Country);
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

            vm.Search = vm.Search;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = SendMoneToKiiPayWalletViewModel.BindProperty)]SendMoneToKiiPayWalletViewModel Vm)
        {

            var countries = _commonServices.GetCountries();
            var identifyCardType = _commonServices.GetCardType();

            ViewBag.countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType");
            var CurrentYear = DateTime.Now.Year;

            if (Vm.DOB == null)
            {
                ModelState.AddModelError("", "Enter Date Of Birth .");

                return View(Vm);
            }
            var DOB = Vm.DOB;
            DateTime date = Convert.ToDateTime(DOB);
            var DOByear = date.Year;
            var Age = CurrentYear - DOByear;
            Vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Vm.IssuingCountry);
            if (ModelState.IsValid)
            {
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
                Vm.SenderFullName = Vm.FirstName + " " + Vm.MiddleName + " " + Vm.LastName;
                _kiiPayWalletTransfer.SetAdminSendMoneToKiiPayWalletViewModel(Vm);
                return RedirectToAction("KiiPayWalletEnterAmount");
            }

            return View(Vm);


        }
        public ActionResult KiiPayWalletEnterAmount()
        {
            SendMoneyToKiiPayEnterAmountViewModel vm = new SendMoneyToKiiPayEnterAmountViewModel();
            var senderInfo = _kiiPayWalletTransfer.GetAdminSendMoneToKiiPayWalletViewModel();
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();

            vm.SendingCountry = senderInfo.Country;
            vm.SendingCurrency = Common.Common.GetCountryCurrency(senderInfo.Country);
            vm.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(senderInfo.Country);
            ViewBag.SenderName = senderInfo.SenderFullName;

            var paymentInfo = _kiiPayWalletTransfer.GetAdminKiiPayEnterAmount();
            if (!string.IsNullOrEmpty(paymentInfo.ReceivingCountry))
            {
                vm = paymentInfo;
            }

            ViewBag.TransferMethod = (int)TransactionServiceType.KiiPayWallet;
            return View(vm);
        }
        [HttpPost]
        public ActionResult KiiPayWalletEnterAmount([Bind(Include = SendMoneyToKiiPayEnterAmountViewModel.BindProperty)]SendMoneyToKiiPayEnterAmountViewModel vm)
        {
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();

            ViewBag.TransferMethod = (int)TransactionServiceType.KiiPayWallet;
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();
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

                _kiiPayWalletTransfer.SetAdminKiiPayEnterAmount(vm);
                return RedirectToAction("KiiPayWalletDetails");
            }
            return View(vm);
        }

        public ActionResult KiiPayWalletDetails(string mobileNo = "")
        {
            AgentResult agentResult = new AgentResult();
            var paymentInfo = _kiiPayWalletTransfer.GetKiiPayEnterAmount();
            string Country = paymentInfo.ReceivingCountry;
            SetViewBagForCountry(Country);
            SetViewBagForPhoneNumbers(Country);
            ViewBag.AgentResult = agentResult;
            KiiPayReceiverDetailsInformationViewModel vm = _kiiPayWalletTransfer.GetAdminKiiPayReceiverDetailsInformationViewModel();
            vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Country);
            vm.Country = Country;
            if (!string.IsNullOrEmpty(mobileNo))
            {
                vm = _kiiPayWalletTransfer.GetKiiPayReceiverDetailsByMobileNo(mobileNo);
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult KiiPayWalletDetails([Bind(Include = KiiPayReceiverDetailsInformationViewModel.BindProperty)]KiiPayReceiverDetailsInformationViewModel Vm, string Country = "")
        {
            SetViewBagForCountry(Country);
            SetViewBagForPhoneNumbers(Country);
            if (ModelState.IsValid)
            {
                bool IsValidReceiver = Common.Common.IsValidBankDepositReceiver(Vm.MobileNo, Service.KiiPayWallet);
                if (IsValidReceiver == false)
                {
                    ModelState.AddModelError("InvalidReceiver", " Receiver is banned");
                    return View(Vm);
                }
                _kiiPayWalletTransfer.SetAdminKiiPayReceiverDetailsInformationViewModel(Vm);
                return RedirectToAction("KiiPayWalletSummary");
            }

            return View(Vm);
        }

        public ActionResult KiiPayWalletSummary()
        {
            var paymentInfo = _kiiPayWalletTransfer.GetAdminKiiPayEnterAmount();
            var senderInfo = _kiiPayWalletTransfer.GetAdminSendMoneToKiiPayWalletViewModel();
            var receiverinfo = _kiiPayWalletTransfer.GetAdminKiiPayReceiverDetailsInformationViewModel();
            ViewBag.SenderName = senderInfo.SenderFullName;
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
            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult KiiPayWalletSummary([Bind(Include = CommonTransactionSummaryViewModel.BindProperty)] CommonTransactionSummaryViewModel vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("SenderPaymentMethod", "StaffKiiPayWallet");
            }

            return View(vm);


        }

        public ActionResult SenderPaymentMethod()
        {
            PaymentMethodViewModel vm = new PaymentMethodViewModel();
            var paymentInfo = _kiiPayWalletTransfer.GetAdminKiiPayEnterAmount();
            var receiverinfo = _kiiPayWalletTransfer.GetAdminKiiPayReceiverDetailsInformationViewModel();
            var senderInfo = _kiiPayWalletTransfer.GetAdminSendMoneToKiiPayWalletViewModel();

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();

            vm.TotalAmount = paymentInfo.TotalAmount;
            vm.SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol;
            ViewBag.Fee = paymentInfo.Fee;
            ViewBag.SenderName = senderInfo.SenderFullName;


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
            var paymentInfo = _kiiPayWalletTransfer.GetAdminKiiPayEnterAmount();
            var receiverinfo = _kiiPayWalletTransfer.GetAdminKiiPayReceiverDetailsInformationViewModel();
            var senderInfo = _kiiPayWalletTransfer.GetAdminSendMoneToKiiPayWalletViewModel();

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
            _kiiPayWalletTransfer.SetAdminPaymentMethod(vm);
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

                    var result = _commonServices.ValidateTransactionUsingStripe(transactionSummaryvm);
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
                    _commonServices.SetDebitCreditCardDetail(transactionSummaryvm.CreditORDebitCardDetials);

                    return RedirectToAction("DebitCreditCardDetails", new { IsFromSavedDebitCard = true });


                    break;
                case SenderPaymentMode.CreditDebitCard:
                    return RedirectToAction("DebitCreditCardDetails");
                    break;
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

            return RedirectToAction("CashPickUpSuccess");
        }

        public ActionResult DebitCreditCardDetails(bool IsAddDebitCreditCard = false, bool IsFromSavedDebitCard = false)
        {
            var senderInfo = _kiiPayWalletTransfer.GetAdminSendMoneToKiiPayWalletViewModel();

            CreditDebitCardViewModel vm = new CreditDebitCardViewModel(senderInfo.Country);
            vm = _commonServices.GetDebitCreditCardDetail(senderInfo.Country);
            var paymentInfo = _kiiPayWalletTransfer.GetAdminKiiPayEnterAmount();

            var receiverinfo = _kiiPayWalletTransfer.GetAdminKiiPayReceiverDetailsInformationViewModel();

            vm.FaxingAmount = paymentInfo.TotalAmount + vm.CreditDebitCardFee;
            vm.FaxingCurrencySymbol = paymentInfo.SendingCurrencySymbol;
            vm.FaxingCurrency = paymentInfo.SendingCurrency;
            vm.SaveCard = IsAddDebitCreditCard;
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            vm.AddressLineOne = senderInfo.AddressLine1;
            vm.ReceiverName = receiverinfo.ReceiverFullName;
            ViewBag.IsFromSavedDebitCard = IsFromSavedDebitCard;
            ViewBag.HasOneSavedCard = _commonServices.HasOneCardSaved(senderInfo.Id);
            SetTransactionSummary();
            return View(vm);
        }
        [HttpPost]
        public JsonResult ThreeDQuery([Bind(Include = CreditDebitCardViewModel.BindProperty)] CreditDebitCardViewModel vm)
        {
            var paymentInfo = _kiiPayWalletTransfer.GetAdminKiiPayEnterAmount();
            var receiverinfo = _kiiPayWalletTransfer.GetAdminKiiPayReceiverDetailsInformationViewModel();
            var senderInfo = _kiiPayWalletTransfer.GetAdminSendMoneToKiiPayWalletViewModel();
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
                _commonServices.SetDebitCreditCardDetail(vm);


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
                    termurl = "/Admin/StaffKiiPayWallet/ThreeDQueryResponseCallBack",
                    billingpostcode = senderInfo.PostCode,
                    billingpremise = senderInfo.AddressLine1,
                    ReceiptNo = Common.AdminSession.ReceiptNo,
                    SenderEmail = senderInfo.Email,
                    SenderFirstName = senderInfo.FirstName,
                    SenderLastName = !string.IsNullOrEmpty(senderInfo.MiddleName) == true ? senderInfo.LastName + " " + senderInfo.LastName : senderInfo.LastName,
                    SenderId = StaffInfo.StaffId,
                };

                var resultThreedQuery = StripServices.CreateThreedQuery(stripeCreateTransaction, TransactionTransferType.Admin, TransactionTransferMethod.KiiPayWallet);

                serviceResult.Message = resultThreedQuery.Message;
                serviceResult.Status = resultThreedQuery.Status;
                serviceResult.Data = resultThreedQuery.Data;
                serviceResult.IsGetType3dAuth = resultThreedQuery.IsGetType3dAuth;
                if (serviceResult.Status == ResultStatus.OK)
                {
                    vm.ThreeDEnrolled = serviceResult.Data.ThreeDEnrolled == "Y" ? true : false;

                    _commonServices.SetDebitCreditCardDetail(vm);
                }
                if (!vm.ThreeDEnrolled)
                {
                    serviceResult.Data = null;
                    serviceResult.Message = resultThreedQuery.Message;
                    serviceResult.Status = ResultStatus.Error;
                    return Json(serviceResult);
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
                var result = Transact365Serivces.GetTransationDetails(uid);
                if (result.Status == ResultStatus.Error)
                {
                    return RedirectToAction("DebitCreditCardDetails");
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ErrorType.PaymentGateway, "StaffKiiPayWallet/ThreeDQueryResponseCallBack");
                return RedirectToAction("DebitCreditCardDetails");
            }
        }


        [HttpPost]
        public ActionResult ThreeDQueryResponseCallBack([Bind(Include = CustomerResponseVm.BindProperty)] CustomerResponseVm responseVm)
        {

            return View(responseVm);

        }

        [HttpPost]
        public ActionResult DebitCreditCardDetails([Bind(Include = CustomerResponseVm.BindProperty)] CustomerResponseVm responseVm)
        {
            var paymentInfo = _kiiPayWalletTransfer.GetAdminKiiPayEnterAmount();
            var receiverinfo = _kiiPayWalletTransfer.GetAdminKiiPayReceiverDetailsInformationViewModel();
            var senderInfo = _kiiPayWalletTransfer.GetAdminSendMoneToKiiPayWalletViewModel();

            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            ViewBag.HasOneSavedCard = _commonServices.HasOneCardSaved(senderInfo.Id);

            var transferSummary = _senderForAllTransferServices.GetAdminTransactionSummary();
            var vm = new CreditDebitCardViewModel();
            vm = transferSummary.CreditORDebitCardDetials;
            AdminForAlTransferServices _adminForAlTransferServices = new AdminForAlTransferServices();

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
            return RedirectToAction("SenderPaymentMethodSuccess", "StaffKiiPayWallet");
        }
        public ActionResult SenderPaymentMethodSuccess()
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();

            // Data to show on success
            SenderCashPickUpSuccessVm vm = new SenderCashPickUpSuccessVm();
            var paymentInfo = _kiiPayWalletTransfer.GetAdminKiiPayEnterAmount();
            var receiverinfo = _kiiPayWalletTransfer.GetAdminKiiPayReceiverDetailsInformationViewModel();
            var senderInfo = _kiiPayWalletTransfer.GetAdminSendMoneToKiiPayWalletViewModel();
            var paymentMethod = _kiiPayWalletTransfer.GetAdminPaymentMethod();

            vm.SentAmount = paymentInfo.SendingAmount;
            vm.SendingCurrency = paymentInfo.SendingCurrencySymbol;
            vm.ReceiverName = receiverinfo.ReceiverFullName;
            ViewBag.Sendername = senderInfo.SenderFullName;
            //this number
            //vm.MFCNNumber = Common.FaxerSession.MFCN;
            //Common.FaxerSession.SenderCashPickUp = null;
            //senderCommonFunc.ClearCashPickUpSession();

            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Payment Confirmation");
            return View(vm);


        }

        private void SetViewBagForPhoneNumbers(string Country)
        {
            var phoneNumbers = _kiiPayWalletTransfer.getRecentNumbers(Country);
            ViewBag.PhoneNumbers = new SelectList(phoneNumbers, "Code", "Name");
        }
        private void SetViewBagForCountry(string Country)
        {
            var countries = _commonServices.GetCountries();
            ViewBag.countries = new SelectList(countries, "Code", "Name", Country);
        }
        public void SetTransactionSummary()
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

            var paymentInfo = _kiiPayWalletTransfer.GetKiiPayEnterAmount();
            var receiverinfo = _kiiPayWalletTransfer.GetKiiPayReceiverDetailsInformationViewModel();
            var senderInfo = _kiiPayWalletTransfer.GetSendMoneToKiiPayWalletViewModel();
            var paymentMethod = _kiiPayWalletTransfer.GetAdminPaymentMethod();

            //Completing Transaction


            TransactionSummaryVM transactionSummaryVm = new TransactionSummaryVM();
            //transactionSummaryVm.KiiPayTransferPaymentSummary = cashPickUp;

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
                ReceiverCountry = receiverinfo.Country,
                ReceiverId = 0,
                SenderWalletId = senderWalletId
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
            var debitCreditCardDetail = _commonServices.GetDebitCreditCardDetail(senderInfo.Country);

            transactionSummaryVm.CreditORDebitCardDetials = debitCreditCardDetail;

            //var moneyFexBankAccountDepositData = _cashPickUpServices.GetMoneyFexBankAccountDeposit();
            //transactionSummaryVm.MoneyFexBankDeposit = moneyFexBankAccountDepositData;

            transactionSummaryVm.TransferType = TransferType.KiiPayWallet;
            if (transactionSummaryVm.SenderAndReceiverDetail.SenderCountry == transactionSummaryVm.SenderAndReceiverDetail.ReceiverCountry)
            {
                transactionSummaryVm.IsLocalPayment = true;
            }
            else
            {
                transactionSummaryVm.IsLocalPayment = false;
            }
            _senderForAllTransferServices.SetAdminTransactionSummary(transactionSummaryVm);
        }

        public JsonResult GetCountryPhoneCode(string CountryCode)
        {

            string PhoneCode = Common.Common.GetCountryPhoneCode(CountryCode);
            return Json(new
            {

                CountryPhoneCode = PhoneCode
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNumberName(string PhoneNumber)
        {
            var receivername = _kiiPayWalletTransfer.getReceiverDetails(PhoneNumber).FirstName;
            if (receivername == null)
            {
                return Json(new
                {
                    phoneTextBox = "",
                    ReceiverFullName = "",
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    phoneTextBox = PhoneNumber,
                    ReceiverFullName = receivername
                }, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount, bool IsReceivingAmount, string receiverCountry, string sendingCountry)
        {

            var paymentSummary = _kiiPayWalletTransfer.GetAdminKiiPayEnterAmount();

            var senderInfo = _kiiPayWalletTransfer.GetAdminSendMoneToKiiPayWalletViewModel();

            string ReceivingCountry = receiverCountry;
            var SendingCurrencySymbol = Common.Common.GetCurrencySymbol(sendingCountry);
            var SendingCurrencyCode = Common.Common.GetCurrencyCode(sendingCountry);
            var RecevingCurrencySymbol = Common.Common.GetCurrencySymbol(receiverCountry);
            var ReceivingCurrencyCode = Common.Common.GetCurrencyCode(receiverCountry);

            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }

            var feeInfo = SEstimateFee.GetTransferFee(sendingCountry, ReceivingCountry, TransactionTransferMethod.KiiPayWallet, SendingAmount, TransactionTransferType.Admin);
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
                SExchangeRate.GetExchangeRateValue(sendingCountry, ReceivingCountry, TransactionTransferMethod.KiiPayWallet, AgentId, TransactionTransferType.Admin), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(sendingCountry, ReceivingCountry, result.FaxingAmount
                , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, IsReceivingAmount, TransactionTransferMethod.KiiPayWallet, AgentId, false);

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

            _kiiPayWalletTransfer.SetAdminKiiPayEnterAmount(paymentSummary);

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
        public JsonResult IsCrebitCard(string cardNumber, string month, string year, string securityCode)
        {
            var paymentInfo = _kiiPayWalletTransfer.GetAdminKiiPayEnterAmount();
            var receiverinfo = _kiiPayWalletTransfer.GetAdminKiiPayReceiverDetailsInformationViewModel();
            var senderInfo = _kiiPayWalletTransfer.GetAdminSendMoneToKiiPayWalletViewModel();
            var paymentMethod = _kiiPayWalletTransfer.GetAdminPaymentMethod();

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

                decimal amount = paymentInfo.TotalAmount;
                amount = (decimal)Math.Round((0.601 / 100), 2) * amount;
                return Json(new
                {
                    IsCrebitCard = StripeResult.IsCreditCard,
                    ExtraAmount = Common.Common.GetCurrencySymbol(senderInfo.CountryCode) + amount
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

    }
}

