using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
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

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class BankAccountPaymentConfirmationController : Controller
    {
        BankAccountPaymentConfirmationServices _services = null;
        public BankAccountPaymentConfirmationController()
        {
            _services = new BankAccountPaymentConfirmationServices();
        }

        // GET: Admin/BankAccountPaymentConfirmation
        public ActionResult Index()
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            return View();
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = BankAccountPaymentConfirmationViewModel.BindProperty)]BankAccountPaymentConfirmationViewModel vm)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            bool IsPaymentReferenceCorrect = _services.IsPaymentReference(vm.PaymentReference);
            if (IsPaymentReferenceCorrect)
            {
                return RedirectToAction("Details", "BankAccountPaymentConfirmation", new { @reference = vm.PaymentReference });
            }
            else
            {
                ModelState.AddModelError("", "Enter Valid Reference");
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult Details(string reference)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }


            BankAccountPaymentConfirmationViewModel vm = _services.GetBankAccountPaymentDetails(reference);

            return View(vm);
        }
        [HttpPost]
        public ActionResult Details([Bind(Include = BankAccountPaymentConfirmationViewModel.BindProperty)]BankAccountPaymentConfirmationViewModel vm)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            if (vm.IsPaid)
            {

                _services.CompleteTransaction();

                return RedirectToAction("Complete", "BankAccountPaymentConfirmation");
            }
            else
            {
                return RedirectToAction("CardDetails", "BankAccountPaymentConfirmation");
            }
            return View(vm);
        }
        public ActionResult CardDetails()
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            CreditDebitCardViewModel vm = new CreditDebitCardViewModel();
            var sendingAmountData = _services.GetBankAccountPaymentConfirmationViewModel();
            vm.FaxingAmount = sendingAmountData.TotalAmount;
            vm.FaxingCurrencySymbol = sendingAmountData.SendingCurrencySymbol;
            vm.FaxingCurrency = sendingAmountData.SendingCurrency;
            //SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            //vm.AddressLineOne = senderCommonFunc.GetSenderAddress();
            vm.ReceiverName = sendingAmountData.ReceiverName;

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
            var transferSummary = _services.GetBankAccountPaymentConfirmationViewModel();
            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                CardName = transferSummary.SenderName,
                ExpirationMonth = vm.EndMM,
                ExpiringYear = vm.EndYY,
                Number = vm.CardNumber,
                SecurityCode = vm.SecurityCode,
                CurrencyCode = Common.Common.GetCurrencyCode(transferSummary.SenderCountry)

            };

            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);

            string CardType = AgentSession.CardType;
            decimal TotalAmount = Common.Common.CreditTypeFee(CardType, vm.FaxingAmount);


            if (!StripeResult.IsValid)
            {
                serviceResult.Data = null;
                serviceResult.Message = StripeResult.Message;
                serviceResult.Status = ResultStatus.Error;
                return Json(serviceResult);
            }

            else

            {
                _services.SetBankAccountPaymentConfirmationViewModel(transferSummary);
                StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
                {
                    Amount = TotalAmount,
                    Currency = Common.Common.GetCountryCurrency(transferSummary.SenderCountry),
                    NameOnCard = "Charge for " + stripeResultIsValidCardVm.CardName,
                    StripeTokenId = StripeResult.StripeTokenId,
                    CardNum = stripeResultIsValidCardVm.Number,
                    ReceivingCountry = transferSummary.ReciverCountry,
                    SendingCountry = transferSummary.SenderCountry,
                    ExipiryDate = stripeResultIsValidCardVm.ExpirationMonth + "/" + stripeResultIsValidCardVm.ExpiringYear,
                    SecurityCode = stripeResultIsValidCardVm.SecurityCode,
                    termurl = "/Admin/BankAccountPaymentConfirmation/ThreeDQueryResponseCallBack",
                    ReceiptNo = transferSummary.RecipetNo,
                    billingpostcode = _services.GetSenderInfo(transferSummary.SenderId).PostalCode,
                    billingpremise = _services.GetSenderInfo(transferSummary.SenderId).Address2,
                    SenderId = transferSummary.SenderId
                };


                var resultThreedQuery = StripServices.CreateThreedQuery(stripeCreateTransaction, DB.TransactionTransferType.Admin, DB.TransactionTransferMethod.BankDeposit);

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
                var transferSummary = _services.GetBankAccountPaymentConfirmationViewModel();

                CustomerResponseVm vm = new CustomerResponseVm();
                
                var result = Transact365Serivces.GetTransationDetails(uid);
                if (result.Status == ResultStatus.Error)
                {
                    return RedirectToAction("CardDetails");
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ErrorType.UnSpecified, "StaffCashPickUpTransfer/ThreeDQueryResponseCallBack");
                return RedirectToAction("CardDetails");
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
                Log.Write(ex.Message, ErrorType.UnSpecified, "StaffCashPickUpTransfer/ThreeDQueryResponseCallBack");
                return RedirectToAction("CardDetails");
            }
        }
        [HttpPost]
        public ActionResult CardDetails([Bind(Include = CustomerResponseVm.BindProperty)] CustomerResponseVm responseVm)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

            CreditDebitCardViewModel vm = new CreditDebitCardViewModel();

            vm = _services.GetDebitCreditCardDetail();
            var transferSummary = _services.GetBankAccountPaymentConfirmationViewModel();

            StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
            {

                Amount = vm.FaxingAmount,
                Currency = Common.Common.GetCurrencyCode(transferSummary.SenderCountry),
                md = responseVm.MD,
                pares = responseVm.PaRes,
                parenttransactionreference = responseVm.parenttransactionreference,
                billingpostcode = _services.GetSenderInfo(transferSummary.SenderId).PostalCode,
                billingpremise = _services.GetSenderInfo(transferSummary.SenderId).Address2,
                SenderId = transferSummary.SenderId,
            };


            var cardProcessor = StripServices.GetCardProcessor(stripeCreateTransaction, TransactionTransferType.Admin, TransactionTransferMethod.BankDeposit);
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

            _services.ReInitialTransaction();
            return RedirectToAction("Complete", "BankAccountPaymentConfirmation");
        }

        public ActionResult Complete()
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var transferSummary = _services.GetBankAccountPaymentConfirmationViewModel();
            ViewBag.SendingAmount = transferSummary.sendingAmount;
            ViewBag.SendingCurrencySymbol = transferSummary.SendingCurrencySymbol;
            ViewBag.ReceiverName = transferSummary.ReceiverName;

            Session.Remove("GetBankAccountPaymentConfirmationViewModel");

            return View();
        }

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

                };
                var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);

                decimal amount = _services.GetBankAccountPaymentConfirmationViewModel().TotalAmount;
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



    }
}