using FAXER.PORTAL.Areas.Businesses.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class MerchantInternationalPaymentController : Controller
    {
        private Services.MerchantNationalPaymentServices _merchantNationalPaymentServices = null;
        Services.CommonServices _CommonServices = null;
        DB.FAXEREntities context = null;
        Services.MerchantInternationalPaymentServices _merchantInternationalPaymentServices = null;
        public MerchantInternationalPaymentController()
        {
            _merchantNationalPaymentServices = new Services.MerchantNationalPaymentServices();
            _merchantInternationalPaymentServices = new Services.MerchantInternationalPaymentServices();
            _CommonServices = new Services.CommonServices();
            context = new DB.FAXEREntities();
        }

        // GET: Businesses/MerchantInternationalPayment
        [HttpGet]
        public ActionResult Index()
        {
            // Business Area View Model
            ViewModels.PreviousPayeeViewModel vm = new ViewModels.PreviousPayeeViewModel();
            // end 
            var PreviousPayee = _merchantInternationalPaymentServices.GetPreviousPayee();
            ViewBag.PreviousPayee = new SelectList(PreviousPayee, "BusinessMFCode", "Name");
            Common.BusinessSession.BackButtonURL = "/Businesses/MerchantInternationalPayment/Index";
            if (!string.IsNullOrEmpty(Common.BusinessSession.MerchantAccountNumber))
            {

                vm.BusinessMFCode = Common.BusinessSession.MerchantAccountNumber;
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = ViewModels.PreviousPayeeViewModel.BindProperty)]ViewModels.PreviousPayeeViewModel vm)
        {

            if (!string.IsNullOrEmpty(vm.BusinessMFCode))
            {
                Common.BusinessSession.MerchantAccountNumber = vm.BusinessMFCode;
                return RedirectToAction("MerchantDetails", new { MerchantAccountNo = vm.BusinessMFCode });
            }
            else
            {

                return RedirectToAction("SearchMerchantByAccountNumber");
            }
        }


        public ActionResult getMerchants(string term , string Country)
        {
            var data = context.KiiPayBusinessInformation.Where(x => x.BusinessName.StartsWith(term) && x.BusinessOperationCountryCode.ToLower() == Country.ToLower());
            if (data.Count() > 0)
            {
                return Json(context.KiiPayBusinessInformation.Where(x => x.BusinessName.StartsWith(term) && x.BusinessOperationCountryCode.ToLower() == Country.ToLower()).Select(a => new { label = a.BusinessName, id = a.BusinessMobileNo }),
                    JsonRequestBehavior.AllowGet);
            }
            else {

                return Json(new { label = "No Result Found", id = 0 },
                   JsonRequestBehavior.AllowGet);
            }
        }




        public ActionResult SearchMerchantByAccountNumber()
        {
            if (!string.IsNullOrEmpty(Common.BusinessSession.MerchantAccountNumber))
            {
                ViewBag.ReceivingCountry = Common.BusinessSession.ReceivingCountry;
                ViewBag.BusinessName = Common.BusinessSession.MerchantDetialsViewModel.MerchantName;
                ViewBag.MerchantAccountNo = Common.BusinessSession.MerchantAccountNumber;
            }
            ViewBag.Countries = new SelectList(context.Country.Where(x => x.CountryCode != Common.BusinessSession.LoggedBusinessMerchant.CountryCode).ToList(), "CountryCode", "CountryName");
            Common.BusinessSession.BackButtonURL = "/Businesses/MerchantInternationalPayment/SearchMerchantByAccountNumber";
            return View();
        }
        public ActionResult MerchantDetails(string MerchantAccountNo = "")
        {
            ViewModels.MerchantDetialsViewModel vm = new ViewModels.MerchantDetialsViewModel();
            if (string.IsNullOrEmpty(MerchantAccountNo))
            {

                TempData["InvalidMFBCCard"] = "Plese enter a valid card number";
                return RedirectToAction("SearchMerchantByAccountNumber");

            }
            else
            {
                var merchantDetails = _merchantNationalPaymentServices.GetBusinessInformation(MerchantAccountNo);
                if (merchantDetails == null)
                {
                    TempData["InvalidMFBCCard"] = "Plese enter a valid card number";
                    return RedirectToAction("SearchMerchantByAccountNumber");

                }
                var receiverCardDetails = _merchantNationalPaymentServices.GetMFBCCardInformation(merchantDetails.Id);
                if (receiverCardDetails == null)
                {

                    TempData["InvalidMFBCCard"] = "This merchant does not have a withdrawal card to accept payments";
                    return RedirectToAction("SearchMerchantByAccountNumber");

                }
                if (receiverCardDetails.CardStatus == DB.CardStatus.InActive)
                {

                    TempData["InvalidMFBCCard"] = "This card has been deactivated";
                    return RedirectToAction("SearchMerchantByAccountNumber");


                }

                vm = new ViewModels.MerchantDetialsViewModel()
                {
                    KiiPayBusinessInformationId = merchantDetails.Id,
                    MerchantAccountNo = merchantDetails.BusinessMobileNo,
                    City = merchantDetails.BusinessOperationCity,
                    Country = Common.Common.GetCountryName(merchantDetails.BusinessOperationCountryCode),
                    MerchantName = merchantDetails.BusinessName,
                    MFBCCardID = _merchantNationalPaymentServices.GetMFBCCardInformation(merchantDetails.Id).Id,
                    Phone = merchantDetails.PhoneNumber,
                    Email = merchantDetails.Email,
                    Website = merchantDetails.Website
                };

                Common.BusinessSession.ReceivingCountry = merchantDetails.BusinessOperationCountryCode;
                Common.BusinessSession.ReceivingCurrency = Common.Common.GetCountryCurrency(merchantDetails.BusinessOperationCountryCode);
                Common.BusinessSession.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(merchantDetails.BusinessOperationCountryCode);
                Common.BusinessSession.MerchantDetialsViewModel = vm;
                Common.BusinessSession.MerchantAccountNumber = vm.MerchantAccountNo;

            }
            return View(vm);
            
        }

        [HttpPost]
        public ActionResult MerchantDetails([Bind(Include = ViewModels.MerchantDetialsViewModel.BindProperty)]ViewModels.MerchantDetialsViewModel vm)
        {

            if (ModelState.IsValid)
            {

                if (vm.confirm == false)
                {
                    ModelState.AddModelError("confirm", "Please accept the terms and condition");

                }
                else
                {

                    if (!string.IsNullOrEmpty(Common.BusinessSession.TransactionSummaryURL))
                    {
                        return Redirect(Common.BusinessSession.TransactionSummaryURL);
                    }
                    return RedirectToAction("PayingAmount");
                }
            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult PayingAmount()
        {
            MerchantInternationalPaymentAmountViewModel vm = new MerchantInternationalPaymentAmountViewModel();
            if (Common.BusinessSession.MerchantInternationalPaymentAmountViewModel != null)
            {
                vm = Common.BusinessSession.MerchantInternationalPaymentAmountViewModel;
            }

            Services.BusinessCardServices cardServices = new Services.BusinessCardServices();
            ViewBag.CreditonCard = cardServices.GetCreditOnCard() +" "+ Common.Common.GetCountryCurrency(Common.BusinessSession.LoggedBusinessMerchant.CountryCode);
            return View(vm);

            //return View();
        }
        [HttpPost]
        public ActionResult PayingAmount([Bind(Include = ViewModels.MerchantInternationalPaymentAmountViewModel.BindProperty)]ViewModels.MerchantInternationalPaymentAmountViewModel vm)
        {
            var currentbalance = _CommonServices.GetCurrentBalanceOnCard();
            if (vm.FaxingAmount == 0 && vm.ReceivingAmount == 0)
            {
                ModelState.AddModelError("FaxingAmount", "Please enter an amount to proceed");
            }
            else if (vm.FaxingAmount > currentbalance)
            {

                ModelState.AddModelError("FaxingAmount", "Insufficient balance on card");
            }
            else if (vm.PaymentReference == null)
            {
                ModelState.AddModelError("PaymentReference", "Payment Reference is Required");
            }
            else
            {
                string FaxingCountryCode = Common.BusinessSession.FaxingCountry;
                string ReceivingCountryCode = Common.BusinessSession.ReceivingCountry;
                decimal exchangeRate = 0;
                var exchangeRateObj = context.ExchangeRate.Where(x => x.CountryCode1 == FaxingCountryCode && x.CountryCode2 == ReceivingCountryCode).FirstOrDefault();
                if (exchangeRateObj == null)
                {
                    var exchangeRateobj2 = context.ExchangeRate.Where(x => x.CountryCode1 == ReceivingCountryCode && x.CountryCode2 == FaxingCountryCode).FirstOrDefault();
                    if (exchangeRateobj2 != null)
                    {
                        exchangeRateObj = exchangeRateobj2;
                        //exchangeRateObj.CountryCode1 = exchangeRateobj2.CountryCode2;
                        //exchangeRateObj.CountryCode2 = exchangeRateobj2.CountryCode1;
                        //exchangeRateObj.FaxingFee2 = exchangeRateobj2.FaxingFee1;
                        //exchangeRateObj.FaxingFee1 = exchangeRateobj2.FaxingFee2;
                        //exchangeRateObj.CountryRate1 = exchangeRateobj2.CountryRate2;
                        //exchangeRateObj.CountryRate2 = exchangeRateobj2.CountryRate1;
                        exchangeRate = Math.Round( 1 / exchangeRateObj.CountryRate1 , 6 , MidpointRounding.AwayFromZero);
                    }

                }
                else
                {
                    exchangeRate = exchangeRateObj.CountryRate1;
                }
                if (ReceivingCountryCode == FaxingCountryCode)
                {

                    exchangeRate = 1m;

                }
                if (exchangeRate == 0)
                {
                        
                    ViewBag.ExchangeRate = "We are yet to start operations to this country, please try again later!";
                    return View(vm);
                }
                if (vm.ReceivingAmount > 0)
                {
                    vm.FaxingAmount = vm.ReceivingAmount;
                }
                var feeSummary = SEstimateFee.CalculateFaxingFee(vm.FaxingAmount, vm.IncludingFee, vm.ReceivingAmount > 0, exchangeRate, SEstimateFee.GetFaxingCommision(FaxingCountryCode));
                Common.BusinessSession.FaxingAmountSummary = feeSummary;
                Common.BusinessSession.MerchantInternationalPaymentAmountViewModel = vm;
                return RedirectToAction("PaymentDetails");
            }
            return View();
        }
        public ActionResult PaymentDetails()
        {

            decimal CurrentBalanceOnCard = _CommonServices.GetCurrentBalanceOnCard();

            EstimateFaxingFeeSummary faxingSummarry = new EstimateFaxingFeeSummary();
            faxingSummarry = Common.BusinessSession.FaxingAmountSummary;
            if (faxingSummarry.TotalAmount > CurrentBalanceOnCard)
            {

                TempData["InSufficientBalance"] = "Insufficient balance on card";
                return RedirectToAction("PayingAmount");
            }
            TopUpDetailsViewModel model = new TopUpDetailsViewModel()
            {
                FaxingFee = faxingSummarry.FaxingFee,
                AmountToBeReceived = faxingSummarry.ReceivingAmount,
                CurrentExchangeRate = faxingSummarry.ExchangeRate,
                faxingAmount = faxingSummarry.FaxingAmount,
                PaymentReference = Common.BusinessSession.MerchantInternationalPaymentAmountViewModel.PaymentReference,
                TotalAmountIncludingFee = faxingSummarry.TotalAmount,

            };
            return View(model);
        }

        public ActionResult FraudAlertPage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult InternationalPaymentTransactionSummary()
        {

            MerchantInternationalPaymentTransactionSummaryViewModel vm = new MerchantInternationalPaymentTransactionSummaryViewModel();
            #region recevier merchant Information 
            var ReceiverDetials = Common.BusinessSession.MerchantDetialsViewModel;
            vm.ReceiverBusinessname = ReceiverDetials.MerchantName;
            vm.ReceiverAccountNO = ReceiverDetials.MerchantAccountNo;
            vm.ReceiveOption = "MFTC Card Withdrawal";
            #endregion
            #region Sender Merchant Information 

            var senderMerchantDetials = _merchantNationalPaymentServices.GetSenderBusinessInformation();
            vm.KiiPayBusinessInformationId = senderMerchantDetials.KiiPayBusinessInformationId;
            vm.CountryOfBirth = Common.Common.GetCountryName(senderMerchantDetials.KiiPayBusinessInformation.BusinessOperationCountryCode);
            vm.MerchantEmail = senderMerchantDetials.KiiPayBusinessInformation.Email;
            vm.MerchantName = senderMerchantDetials.KiiPayBusinessInformation.BusinessName;
            vm.MerchantPhoneNumber = Common.Common.GetCountryPhoneCode(senderMerchantDetials.KiiPayBusinessInformation.BusinessOperationCountryCode) + " " + senderMerchantDetials.KiiPayBusinessInformation.PhoneNumber;
            vm.streetAddress = senderMerchantDetials.KiiPayBusinessInformation.BusinessOperationAddress1;
            vm.City = senderMerchantDetials.KiiPayBusinessInformation.BusinessOperationCity;
            vm.State = senderMerchantDetials.KiiPayBusinessInformation.BusinessOperationState;
            vm.PostalCode = senderMerchantDetials.KiiPayBusinessInformation.BusinessOperationPostalCode;
            vm.MerchantCardID = senderMerchantDetials.Id;
            vm.SenderMerchantMFBCCardNumber = senderMerchantDetials.MobileNo.Decrypt().FormatMFBCCard();
            #endregion

            #region Trasaction Info 
            var TransactionDetails = Common.BusinessSession.FaxingAmountSummary;
            vm.SentAmount = TransactionDetails.FaxingAmount.ToString();
            vm.Fees = TransactionDetails.FaxingFee.ToString();
            vm.TotalAmount = TransactionDetails.TotalAmount.ToString();
            vm.TotalReceiveAmount = TransactionDetails.ReceivingAmount.ToString();
            vm.PaymentReference = Common.BusinessSession.MerchantInternationalPaymentAmountViewModel.PaymentReference;
            #endregion

            Common.BusinessSession.TransactionSummaryURL = "/Businesses/MerchantInternationalPayment/InternationalPaymentTransactionSummary";
            return View(vm);
        }

        [HttpPost]
        public ActionResult InternationalPaymentTransactionSummary([Bind(Include = MerchantInternationalPaymentTransactionSummaryViewModel.BindProperty)]MerchantInternationalPaymentTransactionSummaryViewModel vm)
        {
            var senderMerchantDetials = _merchantNationalPaymentServices.GetSenderBusinessInformation();
            var PaymentDetails = Common.BusinessSession.FaxingAmountSummary;

            var ReceiverDetials = Common.BusinessSession.MerchantDetialsViewModel;
            KiiPayBusinessInternationalPaymentTransaction transaction = new KiiPayBusinessInternationalPaymentTransaction()
            {
                PayedFromKiiPayBusinessInformationId = senderMerchantDetials.KiiPayBusinessInformationId,
                PayedFromKiiPayBusinessWalletId = senderMerchantDetials.Id,
                PayedToKiiPayBusinessInformationId = ReceiverDetials.KiiPayBusinessInformationId,
                PayedToKiiPayBusinessWalletId = ReceiverDetials.MFBCCardID,
                FaxingAmount = PaymentDetails.FaxingAmount,
                FaxingFee = PaymentDetails.FaxingFee,
                ExchangeRate = PaymentDetails.ExchangeRate,
                RecievingAmount = PaymentDetails.ReceivingAmount,
                PaymentReference = Common.BusinessSession.MerchantInternationalPaymentAmountViewModel.PaymentReference,
                TotalAmount = PaymentDetails.TotalAmount,
                TransactionDate = DateTime.Now,
                ReceiptNumber = _CommonServices.ReceiptNoForMerchantInternationalPayment()

            };
            var result = _merchantInternationalPaymentServices.SaveTransaction(transaction);
            var deductCreditOnCard = _CommonServices.DeductTheCreditOnCard(transaction.PayedFromKiiPayBusinessWalletId, transaction.TotalAmount);
            var IncreaseReceiverCredit = _CommonServices.IncreaseTheCreditBalanceonMFBCCard(transaction.PayedToKiiPayBusinessWalletId, transaction.RecievingAmount);


            var ReceiverDetails = _CommonServices.GetMFBCCardInformationByKiiPayBusinessInformationId(transaction.PayedToKiiPayBusinessInformationId);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationPaymentServiceProvider?SenderName=" +
               senderMerchantDetials.FirstName + " " + senderMerchantDetials.MiddleName + " " + senderMerchantDetials.LastName +
               "&ReceivingAmount=" + transaction.RecievingAmount + "&ReceiverBusinessName=" + ReceiverDetails.KiiPayBusinessInformation.BusinessName
               + "&ReceiverCountry=" + Common.Common.GetCountryName(ReceiverDetails.Country));
         

            string URL = baseUrl + "/EmailTemplate/ConfirmationofServiceProviderPaymentReceipt?ReceiptNumber=" + transaction.ReceiptNumber
                + "&Date=" + transaction.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + transaction.TransactionDate.ToString("HH:mm") + "&SenderName=" +
                senderMerchantDetials.FirstName + " " + senderMerchantDetials.MiddleName + " " + senderMerchantDetials.LastName
                + "&ServiceProvideName=" + ReceiverDetails.FirstName  + ""  + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName + 
                "&BusinessMFCode=" + ReceiverDetails.KiiPayBusinessInformation.BusinessMobileNo  +
                "&TopUpAmount=" + transaction.FaxingAmount + "&Fees=" + transaction.FaxingFee +
                "&ExchangeRate=" + transaction.ExchangeRate + "&TotalAmount=" + transaction.TotalAmount + "&AmountInLocalCurrency=" + transaction.RecievingAmount
                 + "&SendingCurrency=" + Common.Common.GetCountryCurrency(senderMerchantDetials.Country) + "&ReceivingCurrency=" + Common.Common.GetCountryCurrency(ReceiverDetails.Country)
                + "&SenderCountry=" + Common.Common.GetCountryName(senderMerchantDetials.Country) + "&BusinessCountry=" + Common.Common.GetCountryName(ReceiverDetails.Country) +
                "&BusinessCity=" + ReceiverDetails.City;
            var Receipt = Common.Common.GetPdf(URL);

            mail.SendMail(senderMerchantDetials.Email, "Confirmation of Payment to Service Provider ", body, Receipt);


            // Sms Function Execute here 
            SmsApi smsApiServices = new SmsApi();

            string senderName = senderMerchantDetials.FirstName + " " + senderMerchantDetials.MiddleName + " " + senderMerchantDetials.LastName;
            string businessAccounntNo = ReceiverDetails.KiiPayBusinessInformation.BusinessMobileNo;
            string businessName = ReceiverDetails.KiiPayBusinessInformation.BusinessName;
            string amount = Common.Common.GetCurrencySymbol(senderMerchantDetials.Country) + transaction.TotalAmount;
            string paymentReference = transaction.PaymentReference;
            string reveingAmount = Common.Common.GetCurrencySymbol(ReceiverDetails.Country) + transaction.RecievingAmount;
            string message = smsApiServices.GetBusinessPaymentMessage(senderName, businessAccounntNo, businessName, amount, paymentReference, reveingAmount);
            string senderPhoneNumber = Common.Common.GetCountryPhoneCode(senderMerchantDetials.Country) + senderMerchantDetials.PhoneNumber;
            string receiverPhoneNumber = Common.Common.GetCountryPhoneCode(ReceiverDetails.Country) + ReceiverDetials.Phone;
            smsApiServices.SendSMS(senderPhoneNumber, message);
            smsApiServices.SendSMS(receiverPhoneNumber, message);


            return RedirectToAction("PaymentSuccessful");
            
        }
        public ActionResult PaymentSuccessful()
        {
            var PaymentDetails = Common.BusinessSession.FaxingAmountSummary;

            var ReceiverDetials = Common.BusinessSession.MerchantDetialsViewModel;
            ViewModels.MerchantInternationalPaymentSuccessfulViewModel vm = new ViewModels.MerchantInternationalPaymentSuccessfulViewModel()
            {
                MerchantName = ReceiverDetials.MerchantName,
                MerchantAccountNo = ReceiverDetials.MerchantAccountNo,
                PaymentReference = Common.BusinessSession.MerchantInternationalPaymentAmountViewModel.PaymentReference,
                FaxingAmount = PaymentDetails.FaxingAmount,
                ExchangeRate = PaymentDetails.ExchangeRate,
                ReceivingAmount = PaymentDetails.ReceivingAmount,
                MerchantCountry = ReceiverDetials.Country,
                TotalAmount = PaymentDetails.TotalAmount,
                ReceiveOption = "MFBC Card Withdrawal"
            };

            
            Session.Remove("TransactionSummaryURL");
            Session.Remove("FaxingAmountSummary");
            Session.Remove("MerchantDetialsViewModel");
            Session.Remove("MerchantInternationalPaymentAmountViewModel");
            return View(vm);
        }
    }
}