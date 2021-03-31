using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
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
using System.Web.WebSockets;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class FundAccountController : Controller
    {
        AgentInformation agentInfo = null;
        FundAccountServices _services = null;
        public FundAccountController()
        {
            agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            _services = new FundAccountServices();
        }

        // GET: Agent/FundAccount
        public ActionResult Index()
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            List<AgentFundAccountViewModel> vm = _services.GetAgentFundAccountList(agentInfo.Id);
            return View(vm);
        }
        public ActionResult FundAccount()
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            AgentFundAccountViewModel vm = new AgentFundAccountViewModel();
            vm.AgentId = agentInfo.Id;
            vm.AgentCountry = agentInfo.CountryCode;
            vm.City = agentInfo.City;
            vm.AgentName = agentInfo.Name;
            vm.AgentCountryCurrency = Common.Common.GetCurrencyCode(vm.AgentCountry);
            vm.AgentCountryCurrencySymbol = Common.Common.GetCurrencySymbol(vm.AgentCountry);
            return View(vm);
        }
        [HttpPost]
        public ActionResult FundAccount([Bind(Include = AgentFundAccountViewModel.BindProperty)] AgentFundAccountViewModel vm)
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }

            if (vm.Amount == 0)
            {
                ModelState.AddModelError("Amount", "Enter Amount");
                return View(vm);
            }

            var ReceiptNo = Common.Common.GenerateFundAccountReceiptNo(6);
            vm.Receipt = ReceiptNo;

            _services.SetAccountFund(vm);
            return RedirectToAction("PaymentMethod", "FundAccount");
        }
        public ActionResult PaymentMethod()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var paymentInfo = _services.GetAccountFund();
            PaymentMethodViewModel vm = new PaymentMethodViewModel()
            {
                TotalAmount = paymentInfo.Amount,
                SendingCurrencySymbol = paymentInfo.AgentCountryCurrencySymbol,
                CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails(paymentInfo.AgentId, Module.Agent),
                HasEnableMoneyFexBankAccount = senderCommonFunc.IsEnabledMoneyFexbankAccount(paymentInfo.AgentCountry)
            };

            ViewBag.CreditDebitFee = new CreditDebitCardViewModel(paymentInfo.AgentCountry).CreditDebitCardFee;
            ViewBag.ManualBankDepositFee = new SenderMoneyFexBankDepositVM(paymentInfo.AgentCountry).BankFee;

            return View(vm);
        }
        [HttpPost]
        public ActionResult PaymentMethod([Bind(Include = PaymentMethodViewModel.BindProperty)] PaymentMethodViewModel vm)
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var paymentInfo = _services.GetAccountFund();
            ViewBag.CreditDebitFee = new CreditDebitCardViewModel(paymentInfo.AgentCountry).CreditDebitCardFee;
            ViewBag.ManualBankDepositFee = new SenderMoneyFexBankDepositVM(paymentInfo.AgentCountry).BankFee;

            vm.TotalAmount = paymentInfo.Amount;
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
            switch (vm.SenderPaymentMode)
            {
                case SenderPaymentMode.SavedDebitCreditCard:

                    SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
                    SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
                    var cardInfo = _saveCreditDebitCard.GetCardInfo(selectedCardId);
                    CreditDebitCardViewModel CreditORDebitCardDetials = new CreditDebitCardViewModel()
                    {
                        CardNumber = cardInfo.Num.Decrypt(),
                        NameOnCard = cardInfo.CardName.Decrypt(),
                        EndMM = cardInfo.EMonth.Decrypt(),
                        EndYY = cardInfo.EYear.Decrypt(),
                        SecurityCode = cardInfo.ClientCode.Decrypt(),
                        FaxingAmount = paymentInfo.Amount,

                    };

                    StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
                    {
                        CardName = CreditORDebitCardDetials.NameOnCard,
                        ExpirationMonth = CreditORDebitCardDetials.EndMM,
                        ExpiringYear = CreditORDebitCardDetials.EndYY,
                        Number = CreditORDebitCardDetials.CardNumber,
                        SecurityCode = CreditORDebitCardDetials.SecurityCode,
                        billingpostcode = Common.AgentSession.AgentInformation.PostalCode,
                        billingpremise = Common.AgentSession.AgentInformation.Address1,
                        CurrencyCode = Common.Common.GetCurrencyCode(Common.AgentSession.AgentInformation.CountryCode),
                        Amount = CreditORDebitCardDetials.FaxingAmount
                    };
                    var validateCardresult = Common.Common.ValidateCreditDebitCard(CreditORDebitCardDetials);
                    if (validateCardresult.Data == false)
                    {
                        ModelState.AddModelError("TransactionError", validateCardresult.Message);
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                        return View(vm);
                    }

                    var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm, Module.Agent);
                    if (StripeResult.IsValid == false)
                    {

                        ModelState.AddModelError("TransactionError", StripeResult.Message);
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                        return View(vm);
                    }
                    _services.SetDebitCreditCardDetail(CreditORDebitCardDetials);

                    //transactionSummaryvm.CreditORDebitCardDetials.StripeTokenID = result.StripeTokenId;

                    return RedirectToAction("DebitCreditCardDetails", new { IsFromSavedDebitCard = true });
                    break;

                case SenderPaymentMode.CreditDebitCard:
                    return RedirectToAction("DebitCreditCardDetails");
                    break;

                case SenderPaymentMode.MoneyFexBankAccount:
                    return RedirectToAction("BankDetails");
                    break;

                default:
                    break;
            }

            return View();
        }


        public ActionResult DebitCreditCardDetails(bool IsAddDebitCreditCard = false, bool IsFromSavedDebitCard = false)
        {

            CreditDebitCardViewModel vm = new CreditDebitCardViewModel();
            vm = _services.GetDebitCreditCardDetail();
            var paymentInfo = _services.GetAccountFund();

            vm.FaxingAmount = paymentInfo.Amount + vm.CreditDebitCardFee;
            vm.FaxingCurrencySymbol = paymentInfo.AgentCountryCurrencySymbol;
            vm.FaxingCurrency = paymentInfo.AgentCountryCurrency;
            vm.SaveCard = IsAddDebitCreditCard;
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            vm.AddressLineOne = agentInfo.Address1 + " " + agentInfo.PostalCode;
            vm.ReceiverName = paymentInfo.AgentName;
            vm.NameOnCard = paymentInfo.AgentName;
            ViewBag.IsFromSavedDebitCard = IsFromSavedDebitCard;
            ViewBag.HasOneSavedCard = Common.Common.HasOneCardSaved(agentInfo.Id, Module.Agent);
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
            var paymentInfo = _services.GetAccountFund();

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
                CardName = agentInfo.Name,
                ExpirationMonth = vm.EndMM,
                ExpiringYear = vm.EndYY,
                Number = vm.CardNumber,
                SecurityCode = vm.SecurityCode,
                billingpostcode = agentInfo.PostalCode,
                billingpremise = agentInfo.Address1,
                CurrencyCode = Common.Common.GetCurrencyCode(agentInfo.CountryCode),
                Amount = vm.FaxingAmount,
                SenderId = agentInfo.Id

            };
            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm, Module.Agent);
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
                _services.SetDebitCreditCardDetail(vm);

                decimal faxingAmount = paymentInfo.Amount + new CreditDebitCardViewModel().CreditDebitCardFee;
                decimal TotalAmount = Common.Common.CreditTypeFee(CardType, faxingAmount);

                SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
                var agentName = agentInfo.Name.Split(' ');
                string FirstName = agentName[0];
                string LastName = "";
                if (agentName.Length > 1)
                {
                    for (int i = 1; i < agentName.Length; i++)
                    {
                        LastName = LastName + agentName[i] + " ";
                    }
                }
                if (string.IsNullOrEmpty(LastName))
                {
                    LastName = FirstName;
                }

                StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
                {
                    Amount = TotalAmount,
                    Currency = Common.Common.GetCountryCurrency(agentInfo.CountryCode),
                    NameOnCard = "Charge for " + stripeResultIsValidCardVm.CardName,
                    StripeTokenId = StripeResult.StripeTokenId,
                    CardNum = stripeResultIsValidCardVm.Number,
                    ReceivingCountry = agentInfo.CountryCode,
                    SendingCountry = agentInfo.CountryCode,
                    ExipiryDate = stripeResultIsValidCardVm.ExpirationMonth + "/" + stripeResultIsValidCardVm.ExpiringYear,
                    SecurityCode = stripeResultIsValidCardVm.SecurityCode,
                    termurl = "/Agent/FundAccount/ThreeDQueryResponseCallBack",
                    billingpostcode = agentInfo.PostalCode,
                    billingpremise = agentInfo.Address1,
                    ReceiptNo = paymentInfo.Receipt,
                    SenderFirstName = FirstName,
                    SenderLastName = LastName,
                    SenderEmail = agentInfo.Email,
                    SenderId = agentInfo.Id,
                    ReceivingCurrency = Common.Common.GetCountryCurrency(agentInfo.CountryCode),
                };
                try
                {
                    var resultThreedQuery = StripServices.CreateThreedQuery(stripeCreateTransaction, TransactionTransferType.AuxAgent, TransactionTransferMethod.All);
                    serviceResult.Message = resultThreedQuery.Message;
                    serviceResult.Status = resultThreedQuery.Status;
                    serviceResult.Data = resultThreedQuery.Data;
                    serviceResult.IsGetType3dAuth = resultThreedQuery.IsGetType3dAuth;
                    if (serviceResult.Status == ResultStatus.OK)
                    {
                        vm.ThreeDEnrolled = serviceResult.Data.ThreeDEnrolled == "Y" ? true : false;
                        _services.SetDebitCreditCardDetail(vm);
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
                    Log.Write(ex.Message, ErrorType.PaymentGateway, "FundAccount/ThreeDQuery");
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
                Log.Write(ex.Message, ErrorType.PaymentGateway, "FundAccount/ThreeDQueryResponseCallBack");
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
                Log.Write(ex.Message, ErrorType.PaymentGateway, "FundAccount/ThreeDQueryResponseCallBack");
                return RedirectToAction("DebitCreditCardDetails");
            }
        }



        [HttpPost]
        public ActionResult DebitCreditCardDetails([Bind(Include = CustomerResponseVm.BindProperty)] CustomerResponseVm responseVm)
        {
            //SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            ViewBag.HasOneSavedCard = Common.Common.HasOneCardSaved(agentInfo.Id, Module.Agent);

            var paymentInfo = _services.GetAccountFund();
            CreditDebitCardViewModel vm = _services.GetDebitCreditCardDetail(); ;


            StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
            {
                Amount = vm.FaxingAmount,
                Currency = Common.Common.GetCurrencyCode(paymentInfo.AgentCountry),
                md = responseVm.MD,
                pares = responseVm.PaRes,
                parenttransactionreference = responseVm.parenttransactionreference,
                billingpostcode = agentInfo.PostalCode,
                billingpremise = agentInfo.Address1,
                ReceiptNo = paymentInfo.Receipt,
                SenderId = agentInfo.Id,
                CardNum = vm.CardNumber
            };

            var cardProcessor = StripServices.GetCardProcessor(stripeCreateTransaction, TransactionTransferType.Agent, TransactionTransferMethod.All);
            switch (cardProcessor)
            {
                case CardProcessorApi.TrustPayment:

                    var transactionResult = StripServices.CreateTransaction(stripeCreateTransaction, Module.Agent);
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
            AgentFundAccountViewModel agentFundAccountViewModel = new AgentFundAccountViewModel()
            {
                Amount = paymentInfo.Amount,
                AgentCountryCurrency = paymentInfo.AgentCountryCurrency,
                City = agentInfo.City,
                AgentCountryCurrencySymbol = paymentInfo.AgentCountryCurrencySymbol,
                AgentCountry = paymentInfo.AgentCountry,
                AgentId = paymentInfo.AgentId,
                Receipt = paymentInfo.Receipt,
                CardProcessorApi = cardProcessor,
                SenderPaymentMode = SenderPaymentMode.CreditDebitCard,
                FormattedCardNumber = "XXXX-XXXX-XXXX-" + vm.CardNumber.Substring(vm.CardNumber.Length - 4)
            };
            _services.AddFundInAccount(agentFundAccountViewModel);

            return RedirectToAction("AccountFunded", "FundAccount");


        }
        public ActionResult BankDetails()
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            AgentFundAccountViewModel vm = new AgentFundAccountViewModel();
            vm = _services.GetBankDetails();
            return View(vm);
        }

        [HttpPost]
        public ActionResult BankDetails([Bind(Include = AgentFundAccountViewModel.BindProperty)] AgentFundAccountViewModel vm)
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            vm = _services.GetAccountFund();

            if (ModelState.IsValid)
            {
                _services.AddFundInAccount(vm);
                return RedirectToAction("AccountFunded", "FundAccount");
            }
            return View(vm);
        }
        public ActionResult AccountFunded()
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            return View();
        }
    }
}