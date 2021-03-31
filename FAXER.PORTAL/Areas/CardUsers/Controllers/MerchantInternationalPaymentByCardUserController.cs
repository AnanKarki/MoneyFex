using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.CardUsers.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class MerchantInternationalPaymentByCardUserController : Controller
    {
        Services.CardUserCommonServices _cardUserCommonServices = null;
        Services.CardUserMerchantInternationalPaymentServices _cardUserMerchantInternationalPaymentServices = null;
        int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel == null ? 0 : Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
        DB.FAXEREntities context = null;
        public MerchantInternationalPaymentByCardUserController()
        {
            _cardUserCommonServices = new Services.CardUserCommonServices();
            _cardUserMerchantInternationalPaymentServices = new Services.CardUserMerchantInternationalPaymentServices();
            context = new DB.FAXEREntities();
        }
        // GET: CardUsers/MerchantInternationalPaymentByCardUser
        [HttpGet]
        public ActionResult Index()
        {

            CardUserPreviousPayeeViewModel vm = new CardUserPreviousPayeeViewModel();
            var PreviousPayee = _cardUserCommonServices.getInternationalPreviousPayees();
            ViewBag.PreviousPayee = new SelectList(PreviousPayee, "BusinessMFCode", "Name");
            Common.CardUserSession.BackButtonURL = "/CardUsers/MerchantInternationalPaymentByCardUser/Index";
            if (!string.IsNullOrEmpty(Common.CardUserSession.MerchantAccountNumber))
            {
                vm.BusinessMFCode = Common.CardUserSession.MerchantAccountNumber;
            }
            return View(vm);
            // return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = CardUserPreviousPayeeViewModel.BindProperty)]CardUserPreviousPayeeViewModel vm)
        {
            if (!string.IsNullOrEmpty(vm.BusinessMFCode))
            {
                Common.CardUserSession.MerchantAccountNumber = vm.BusinessMFCode;

                return RedirectToAction("MerchantDetails", new { MerchantAccountNo = vm.BusinessMFCode });

            }
            else
            {

                return RedirectToAction("SearchMerchantByAccountNo");
            }
            //return View();
            //return View();
        }

        public ActionResult SearchMerchantByAccountNo()
        {

            if (!string.IsNullOrEmpty(Common.CardUserSession.MerchantAccountNumber))
            {

                ViewBag.ReceivingCountry = Common.CardUserSession.ReceivingCountry;
                ViewBag.MerchantAccountNo = Common.CardUserSession.MerchantAccountNumber;
                ViewBag.BusinessName = Common.CardUserSession.MerchantName;
            }


            ViewBag.Countries = new SelectList(context.Country.Where(x => x.CountryCode != Common.CardUserSession.LoggedCardUserViewModel.Country).ToList(), "CountryCode", "CountryName");
            Common.CardUserSession.BackButtonURL = "/CardUsers/MerchantInternationalPaymentByCardUser/SearchMerchantByAccountNo";
            return View();
        }
        [HttpGet]
        public ActionResult getMerchants(string term, string Country)
        {
            var data = context.KiiPayBusinessInformation.Where(x => x.BusinessName.StartsWith(term) && x.BusinessOperationCountryCode.ToLower() == Country.ToLower());
            if (data.Count() > 0)
            {
                return Json(context.KiiPayBusinessInformation.Where(x => x.BusinessName.StartsWith(term) && x.BusinessOperationCountryCode.ToLower() == Country.ToLower()).Select(a => new { label = a.BusinessName, id = a.BusinessMobileNo }),
                    JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(new { label = "No Result Found", id = 0 },
                   JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult MerchantDetails(string MerchantAccountNo = "")
        {
            ViewModels.MerchantDetailsViewModel_CardUserViewModel vm = new ViewModels.MerchantDetailsViewModel_CardUserViewModel();
            if (string.IsNullOrEmpty(MerchantAccountNo))
            {

                TempData["InvalidMFBCCard"] = "Plese enter a valid card number";
                return RedirectToAction("SearchMerchantByAccountNo");

            }
            else
            {
                var merchantDetails = _cardUserCommonServices.GetBusinessInformation(MerchantAccountNo);
                if (merchantDetails == null)
                {
                    TempData["InvalidMFBCCard"] = "Plese enter a valid card number";
                    return RedirectToAction("SearchMerchantByAccountNo");

                }
                var receiverCardDetails = _cardUserCommonServices.GetMFBCCardInformation(merchantDetails.Id);
                if (receiverCardDetails == null)
                {

                    TempData["InvalidMFBCCard"] = "This merchant does not have a withdrawal card to accept payments";
                    return RedirectToAction("SearchMerchantByAccountNo");

                }
                if (receiverCardDetails.CardStatus == DB.CardStatus.InActive)
                {

                    TempData["InvalidMFBCCard"] = "This card has been deactivated";
                    return RedirectToAction("SearchMerchantByAccountNo");


                }
                CommonServices _adminCommonServices = new CommonServices();
                vm = new ViewModels.MerchantDetailsViewModel_CardUserViewModel()
                {
                    KiiPayBusinessInformationId = merchantDetails.Id,
                    MerchantAccountNo = merchantDetails.BusinessMobileNo,
                    City = merchantDetails.BusinessOperationCity,
                    Country = Common.Common.GetCountryName(merchantDetails.BusinessOperationCountryCode),
                    MerchantName = merchantDetails.BusinessName,
                    MFBCCardID = _cardUserCommonServices.GetMFBCCardInformation(merchantDetails.Id).Id,
                    CountryPhoneCode = _adminCommonServices.getPhoneCodeFromCountry(merchantDetails.BusinessOperationCountryCode),
                    PhoneNo = merchantDetails.PhoneNumber,
                    Email = merchantDetails.Email,
                    Website = merchantDetails.Website
                };
                Common.CardUserSession.ReceivingCountry = merchantDetails.BusinessOperationCountryCode;
                Common.CardUserSession.ReceivingCurrency = Common.Common.GetCountryCurrency(merchantDetails.BusinessOperationCountryCode);
                Common.CardUserSession.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(merchantDetails.BusinessOperationCountryCode);
                Common.CardUserSession.MerchantDetailsViewModel_CardUserViewModel = vm;
                Common.CardUserSession.MerchantAccountNumber = vm.MerchantAccountNo;
                Common.CardUserSession.MerchantName = vm.MerchantName;

            }
            return View(vm);

        }
        [HttpPost]
        public ActionResult MerchantDetails([Bind(Include = MerchantDetailsViewModel_CardUserViewModel.BindProperty)]MerchantDetailsViewModel_CardUserViewModel vm)
        {

            if (ModelState.IsValid)
            {

                if (vm.confirm == false)
                {
                    ModelState.AddModelError("confirm", "Please accept the terms and condition");

                }
                else
                {

                    if (!string.IsNullOrEmpty(Common.CardUserSession.TransactionSummaryURL))
                    {
                        return Redirect(Common.CardUserSession.TransactionSummaryURL);
                    }
                    return RedirectToAction("PayingAmount");
                }
            }
            return View(vm);
            
        }
        [HttpGet]
        public ActionResult PayingAmount()
        {

            ViewModels.MerchantInternationalPayingAmount_CardUserViewModel vm = new MerchantInternationalPayingAmount_CardUserViewModel();
            if (Common.CardUserSession.MerchantInternationalPayingAmount_CardUserViewModel != null)
            {

                vm = Common.CardUserSession.MerchantInternationalPayingAmount_CardUserViewModel;
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult PayingAmount([Bind(Include = MerchantInternationalPayingAmount_CardUserViewModel.BindProperty)]MerchantInternationalPayingAmount_CardUserViewModel vm)
        {

            var currentbalance = _cardUserCommonServices.getCurrentBalanceOnCard(MFTCCardId);
            Common.CardUserSession.FaxingCountry = Common.CardUserSession.LoggedCardUserViewModel.Country;
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
                string FaxingCountryCode = Common.CardUserSession.FaxingCountry;
                string ReceivingCountryCode = Common.CardUserSession.ReceivingCountry;
                decimal exchangeRate = SExchangeRate.GetExchangeRateValue(FaxingCountryCode, ReceivingCountryCode); 

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

                // Check whether the paying Amount is valid according to purchase Limit
                var validAmountAccordingToPurchaseLimit = _cardUserCommonServices.ValidAmountAccordingToPurchaseLimit(MFTCCardId, feeSummary.TotalAmount);

                if (validAmountAccordingToPurchaseLimit == false)
                {
                    ViewBag.ExchangeRate = "Sorry, You have exceeded your purchase limit";
                    return View(vm);
                }
                // Check for the sufficient balance 
                decimal CurrentBalanceOnCard = _cardUserCommonServices.getCurrentBalanceOnCard(MFTCCardId);


                if (feeSummary.TotalAmount > CurrentBalanceOnCard)
                {
                    ViewBag.ExchangeRate = "Insufficient balance on card";
                    return View(vm);
                }
                Common.CardUserSession.FaxingAmountSummary = feeSummary;
                Common.CardUserSession.MerchantInternationalPayingAmount_CardUserViewModel = vm;

                return RedirectToAction("PaymentDetials");
            }
            return View();
        }
        [HttpGet]
        public ActionResult PaymentDetials()
        {

            decimal CurrentBalanceOnCard = _cardUserCommonServices.getCurrentBalanceOnCard(MFTCCardId);


            EstimateFaxingFeeSummary faxingSummarry = new EstimateFaxingFeeSummary();
            faxingSummarry = Common.CardUserSession.FaxingAmountSummary;
            if (faxingSummarry.TotalAmount > CurrentBalanceOnCard)
            {

                TempData["InSufficientBalance"] = "Insufficient balance on card";
                return RedirectToAction("PayingAmount");
            }

            PaymentDetailsViewModel model = new PaymentDetailsViewModel()
            {
                FaxingFee = faxingSummarry.FaxingFee,
                AmountToBeReceived = faxingSummarry.ReceivingAmount,
                CurrentExchangeRate = faxingSummarry.ExchangeRate,
                faxingAmount = faxingSummarry.FaxingAmount,
                PaymentReference = Common.CardUserSession.MerchantInternationalPayingAmount_CardUserViewModel.PaymentReference,
                TotalAmountIncludingFee = faxingSummarry.TotalAmount,

            };
            return View(model);
            return View();
        }


        public ActionResult FraudAlertProtection()
        {

            return View();
        }
        [HttpGet]
        public ActionResult MerchantInternationalPaymentTransactionSummary()
        {
            CardUser_MerchantInternationalPaymentTransactionSummaryViewModel vm = new CardUser_MerchantInternationalPaymentTransactionSummaryViewModel();
            #region Receiver Merchant Details 
            var receiverDetials = Common.CardUserSession.MerchantDetailsViewModel_CardUserViewModel;
            vm.ReceiverBusinessname = receiverDetials.MerchantName;
            vm.ReceiverAccountNO = receiverDetials.MerchantAccountNo;
            #endregion

            #region Sender CardUser details
            var senderDetails = _cardUserCommonServices.GetMFTCCardUserInformation(MFTCCardId);
            vm.CardUserName = senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName;
            vm.CardUserEmail = senderDetails.CardUserEmail;
            vm.CardUserCountry = Common.Common.GetCountryName(senderDetails.CardUserCountry);
            vm.City = senderDetails.CardUserCity;
            vm.streetAddress = senderDetails.Address1;
            vm.State = senderDetails.CardUserState;
            vm.PostalCode = senderDetails.CardUserPostalCode;
            vm.SenderMFBCCardNumber = senderDetails.MobileNo.Decrypt().FormatMFTCCard();

            #endregion

            #region Trasaction Details 

            var payingDetails = Common.CardUserSession.FaxingAmountSummary;
            vm.SentAmount = payingDetails.FaxingAmount.ToString();
            vm.TotalReceiveAmount = payingDetails.ReceivingAmount.ToString();
            vm.TotalAmount = payingDetails.TotalAmount.ToString();
            vm.Fees = payingDetails.FaxingFee.ToString();
            vm.PaymentReference = Common.CardUserSession.MerchantInternationalPayingAmount_CardUserViewModel.PaymentReference;
            #endregion

            Common.CardUserSession.TransactionSummaryURL = "/CardUsers/MerchantInternationalPaymentByCardUser/MerchantInternationalPaymentTransactionSummary";

            return View(vm);
        }
        [HttpPost]
        public ActionResult MerchantInternationalPaymentTransactionSummary([Bind(Include = CardUser_MerchantInternationalPaymentTransactionSummaryViewModel.BindProperty)]CardUser_MerchantInternationalPaymentTransactionSummaryViewModel vm)
        {


            // Receiver is a Merchant 
            var receiverMerchantDetials = Common.CardUserSession.MerchantDetailsViewModel_CardUserViewModel;

            // sender is a MFTC card User
            var senderDetails = _cardUserCommonServices.GetMFTCCardUserInformation(MFTCCardId);
            var paymentDetails = Common.CardUserSession.FaxingAmountSummary;
            DB.KiiPayPersonalInternationalKiiPayBusinessPayment transaction = new DB.KiiPayPersonalInternationalKiiPayBusinessPayment()
            {

                PayedFromKiiPayPersonalWalletId = senderDetails.Id,
                PayedToKiiPayBusinessInformationId = receiverMerchantDetials.KiiPayBusinessInformationId,
                PayedToKiiPayBusinessWalletId = receiverMerchantDetials.MFBCCardID,
                ExchangeRate = paymentDetails.ExchangeRate,
                FaxingAmount = paymentDetails.FaxingAmount,
                TotalAmount = paymentDetails.TotalAmount,
                ReceivingAmount = paymentDetails.ReceivingAmount,
                FaxingFee = paymentDetails.FaxingFee,
                PaymentReference = Common.CardUserSession.MerchantInternationalPayingAmount_CardUserViewModel.PaymentReference,
                TransactionDate = System.DateTime.Now,
                ReceiptNumber = _cardUserCommonServices.ReceiptNoForMerchantInternationalPayment()
            };
            var transctionResult = _cardUserMerchantInternationalPaymentServices.SaveTransaction(transaction);

            Common.Common.DeductCreditOnCard(transaction.PayedFromKiiPayPersonalWalletId, transaction.TotalAmount);
            Common.Common.IncreaseBalanceOnMFBCCard(transaction.PayedToKiiPayBusinessWalletId, transaction.ReceivingAmount);


            var ReceiverDetails = _cardUserCommonServices.GetMFBCCardInformationByMFBCID(transaction.PayedToKiiPayBusinessWalletId);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationPaymentServiceProvider?SenderName=" +
                senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName +
                "&ReceivingAmount=" + transaction.ReceivingAmount + "&ReceiverBusinessName=" + ReceiverDetails.KiiPayBusinessInformation.BusinessName
                + "&ReceiverCountry=" + Common.Common.GetCountryName(ReceiverDetails.Country) + "&ReceivingCurrency=" + Common.Common.GetCountryCurrency(ReceiverDetails.Country));

            string URL = baseUrl + "/EmailTemplate/ConfirmationofServiceProviderPaymentReceipt?ReceiptNumber=" + transaction.ReceiptNumber
                + "&Date=" + transaction.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + transaction.TransactionDate.ToString("HH:mm") + "&SenderName=" +
                senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName
                + "&ServiceProvideName=" + ReceiverDetails.FirstName + "" + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName +
                "&BusinessMFCode=" + ReceiverDetails.KiiPayBusinessInformation.BusinessMobileNo +
                "&TopUpAmount=" + transaction.FaxingAmount + "&Fees=" + transaction.FaxingFee +
                "&ExchangeRate=" + transaction.ExchangeRate + "&TotalAmount=" + transaction.TotalAmount + "&AmountInLocalCurrency=" + transaction.ReceivingAmount
                + "&SendingCurrency=" + Common.Common.GetCountryCurrency(senderDetails.CardUserCountry) + "&ReceivingCurrency=" + Common.Common.GetCountryCurrency(ReceiverDetails.Country)
                + "&SenderCountry=" + Common.Common.GetCountryName(senderDetails.CardUserCountry) + "&BusinessCountry=" + Common.Common.GetCountryName(ReceiverDetails.Country) +
                "&BusinessCity=" + ReceiverDetails.City;

            var Receipt = Common.Common.GetPdf(URL);

            mail.SendMail(senderDetails.CardUserEmail, "Confirmation of Payment to Service Provider ", body, Receipt);

            //SMS Function

            SmsApi smsApiServices = new SmsApi();
            string senderName = senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName;
            string businessAccounntNo = ReceiverDetails.KiiPayBusinessInformation.BusinessMobileNo;
            string businessName = ReceiverDetails.KiiPayBusinessInformation.BusinessName;
            string amount = Common.Common.GetCurrencySymbol(senderDetails.CardUserCountry) + transaction.TotalAmount;
            string paymentReference = transaction.PaymentReference;
            string receivingAmount = Common.Common.GetCountryCurrency(ReceiverDetails.Country) + transaction.ReceivingAmount;

            string message = smsApiServices.GetBusinessPaymentMessage(senderName, businessAccounntNo, businessName, amount, paymentReference, receivingAmount);
            string phoneNumber = Common.Common.GetCountryPhoneCode(senderDetails.CardUserCountry) + senderDetails.CardUserTel;
            smsApiServices.SendSMS(phoneNumber, message);


            // Check  Auto Top-Up is enable for The Logged In CardUser or Not 

            if (senderDetails.CurrentBalance == 0)
            {
                _cardUserCommonServices.SendMailWhenBalanceISZero(senderDetails.Id);

                if (senderDetails.AutoTopUp == true)
                {

                    _cardUserCommonServices.AutoTopUp(senderDetails.Id);
                }

            }


            return RedirectToAction("PaymentSuccessful");
            return View();
        }
        public ActionResult PaymentSuccessful()
        {
            var receiverMerchantDetials = Common.CardUserSession.MerchantDetailsViewModel_CardUserViewModel;

            var paymentDetails = Common.CardUserSession.FaxingAmountSummary;
            CardUser_MerchantInternationalPaymentSuccessful vm = new CardUser_MerchantInternationalPaymentSuccessful()
            {
                MerchantName = receiverMerchantDetials.MerchantName,
                MerchantAccountNo = receiverMerchantDetials.MerchantAccountNo,
                AmountPaid = paymentDetails.TotalAmount.ToString(),
                MerchantCountry = receiverMerchantDetials.Country,
                PaymentReference = Common.CardUserSession.MerchantInternationalPayingAmount_CardUserViewModel.PaymentReference,
                ReceiveOption = "MFBC Card withdrawal"

            };
            Session.Remove("MerchantDetailsViewModel_CardUserViewModel");
            Session.Remove("FaxingAmountSummary");
            Session.Remove("MerchantInternationalPayingAmount_CardUserViewModel");
            Session.Remove("ReceivingCountry");
            Session.Remove("MerchantAccountNumber");
            Session.Remove("MerchantName");
            return View(vm);
        }
    }
}