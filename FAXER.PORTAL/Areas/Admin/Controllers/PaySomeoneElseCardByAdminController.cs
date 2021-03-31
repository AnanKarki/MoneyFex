using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class PaySomeoneElseCardByAdminController : Controller
    {

        DB.FAXEREntities context = null;
        Services.PaySomeoneElseCardByAdminServices PaySomeoneElseCardByAdminServices = null;
        public PaySomeoneElseCardByAdminController()
        {
            PaySomeoneElseCardByAdminServices = new Services.PaySomeoneElseCardByAdminServices();
            context = new DB.FAXEREntities();
        }
        // GET: Admin/PaySomeoneElseCardByAdmin
        [HttpGet]
        public ActionResult Index(string SenderAccountNo = "")
        {
            ViewModels.PaySomeoneElseCardByAdminViewModel vm = new ViewModels.PaySomeoneElseCardByAdminViewModel();
            var savedCardList = new List<ViewModels.SavedDropDownVM>();


            ViewBag.cardNumbers = new SelectList(savedCardList, "CardNum", "CardNumMasked");


            ViewBag.Countries = new SelectList(Common.Common.GetCountries(), "CountryCode", "CountryName");
            if (!string.IsNullOrEmpty(SenderAccountNo))
            {
                var result = PaySomeoneElseCardByAdminServices.GetSenderInformation(SenderAccountNo);

                if (result == null)
                {

                    ModelState.AddModelError("CustomError", "Please enter a valid sender Account No");
                }
                else
                {

                    savedCardList = PaySomeoneElseCardByAdminServices.getsavedcardList(result.Id);

                    ViewBag.cardNumbers = new SelectList(savedCardList, "CardNum", "CardNumMasked");


                    vm = new ViewModels.PaySomeoneElseCardByAdminViewModel()
                    {
                        SenderAccountNo = result.AccountNo,
                        SenderId = result.Id,
                        SenderFirstName = result.FirstName,
                        SenderMiddleName = result.MiddleName,
                        SenderLastName = result.LastName,
                        SenderAddress = result.Address1,
                        SenderCity = result.City,
                        SenderCountry = Common.Common.GetCountryName(result.Country),
                        SenderCountryCode = result.Country,
                        SenderEmail = result.Email,
                        SenderIdCardExpDate = result.IdCardExpiringDate.ToString("dd/MM/yyyy"),
                        SenderIdCardIssuingCountry = Common.Common.GetCountryName(result.IssuingCountry),
                        SenderIdCardNumber = result.IdCardNumber,
                        SenderIdCardType = result.IdCardType,
                        SenderTelephone = Common.Common.GetCountryPhoneCode(result.Country) + " " + result.PhoneNumber,
                        SenderCurrency = Common.Common.GetCountryCurrency(result.Country),
                        SenderCurrencySymbol = Common.Common.GetCurrencySymbol(result.Country),


                    };
                }
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = ViewModels.PaySomeoneElseCardByAdminViewModel.BindProperty)] ViewModels.PaySomeoneElseCardByAdminViewModel model)
        {

            var savedCardList = new List<ViewModels.SavedDropDownVM>();

            ViewBag.Countries = new SelectList(Common.Common.GetCountries(), "CountryCode", "CountryName");

            ViewBag.cardNumbers = new SelectList(savedCardList, "CardNum", "CardNumMasked");
            if (model.SenderId == 0)
            {


                ViewBag.TransactionMessage = "Please search for sender information";
                return View(model);

            }
            savedCardList = PaySomeoneElseCardByAdminServices.getsavedcardList(model.SenderId);
            ViewBag.cardNumbers = new SelectList(savedCardList, "CardNum", "CardNumMasked");
            if (model.CardUserId == 0)
            {


                ViewBag.TransactionMessage = "Please search for card user information";
                return View(model);

            }

            if (model.TopUpAmount <= 0)
            {

                ModelState.AddModelError("TopUpAmount", "Top-up amount should be greater than zero {0}");
                return View(model);
            }
            if (model.ReceivingAmount <= 0) {


                ModelState.AddModelError("ReceivingAmount", "Top-up amount should be greater than zero {0}");
                return View(model);
            }

            if (string.IsNullOrEmpty(model.TopUpReference))
            {


                ModelState.AddModelError("TopUpReference", "Please enter a top-up reference");
                return View(model);
            }

            if (model.IsCardAvailabled == false)
            {
                if (model.bankToBankPayment == false)
                {

                    ModelState.AddModelError("bankToBankPayment", "Please check yes to bank to bank payment");
                    return View(model);
                }
            }
            else
            {

                var senderInfo = PaySomeoneElseCardByAdminServices.GetSenderInformation(model.SenderAccountNo);
                if (ModelState.IsValid == false) {

                    return View(model);
                }

                if (string.IsNullOrEmpty(model.BillingAddress1)) {

                    ModelState.AddModelError("BillingAddress1", "The Field is required");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.BillingCity))
                {

                    ModelState.AddModelError("BillingCity", "The Field is required");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.BillingPostalCode))
                {

                    ModelState.AddModelError("BillingPostalCode", "The Field is required");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.BillingCountry))
                {

                    ModelState.AddModelError("BillingCountry", "The Field is required");
                    return View(model);
                }
                if (model.BillingAddress1.Trim().ToLower() != senderInfo.Address1.Trim().ToLower())
                {

                    ModelState.AddModelError("BillingAddress1", "Address1 doesn't match to the sender Address1");
                    return View(model);
                }
                else if (model.BillingCity.Trim().ToLower() != senderInfo.City.Trim().ToLower())
                {

                    ModelState.AddModelError("BillingAddress1", "City doesn't match to the sender City");
                    return View(model);
                }

                else if (model.BillingPostalCode.Trim().ToLower() != senderInfo.PostalCode.Trim().ToLower())
                {

                    ModelState.AddModelError("BillingAddress1", "Postal Code doesn't match to the sender Postal Code");
                    return View(model);
                }
                else if (model.BillingCountry.Trim().ToLower() != senderInfo.Country.Trim().ToLower())
                {

                    ModelState.AddModelError("BillingAddress1", "Country doesn't match to the sender Country");
                    return View(model);
                }
                if (model.AcceptTerms == false)
                {

                    ModelState.AddModelError("AcceptTerms", "Please accept our terms and condition");
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.CardNumberDropDown)) {


                    var CreditDebitCardDetails = PaySomeoneElseCardByAdminServices.GetSavedCardByFaxerId(model.SenderId);

                    model.CardNumber = CreditDebitCardDetails.Num.Decrypt();
                    model.CardEndYear = CreditDebitCardDetails.EYear.Decrypt();
                    model.CardEndMonth = CreditDebitCardDetails.EMonth.Decrypt();
                    model.NameOnCard = CreditDebitCardDetails.CardName.Decrypt();
                    model.CardSecurityNo = CreditDebitCardDetails.ClientCode.Decrypt();

                }
                #region  Strip portion
                StripeConfiguration.SetApiKey(System.Configuration.ConfigurationManager.AppSettings["StripeApiLiveKey"]);
                //StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");
                //Stripe.SourceCard card = new SourceCard()
                //{
                //    //Number = savedCreditDebitCardDetails.Num.Decrypt(),
                //    Number = "5555555555554444",
                //    ExpirationYear = int.Parse(savedCreditDebitCardDetails.EYear.Decrypt()),
                //    ExpirationMonth = int.Parse(savedCreditDebitCardDetails.EMonth.Decrypt()),
                //    Cvc = savedCreditDebitCardDetails.ClientCode.Decrypt(),
                //    Name = savedCreditDebitCardDetails.CardName.Decrypt(),
                //    AddressCity = AutoPaymentInformation.FaxerInformation.City,
                //    AddressCountry = AutoPaymentInformation.FaxerInformation.Country,
                //    AddressLine1 = AutoPaymentInformation.FaxerInformation.Address1,
                //    AddressState = AutoPaymentInformation.FaxerInformation.State,
                //    AddressZip = AutoPaymentInformation.FaxerInformation.PostalCode,
                //};

                //Sample  Credit card

                //var stripeTokenCreateOptions = new StripeTokenCreateOptions
                //{
                //    Card = new StripeCreditCardOptions
                //    {
                //        Number = "4242424242424242",
                //        ExpirationMonth = 12,
                //        ExpirationYear = 20,
                //        Cvc = "123",
                //        Name = "John Appleseed"
                //    }
                //};

                var stripeTokenCreateOptions = new StripeTokenCreateOptions
                {
                    Card = new StripeCreditCardOptions
                    {
                        Number = model.CardNumber,
                        ExpirationMonth = int.Parse(model.CardEndMonth),
                        ExpirationYear = int.Parse(model.CardEndYear),
                        Cvc = model.CardSecurityNo,
                        Name = model.NameOnCard
                    }
                };

                string token = "";
                var tokenService = new StripeTokenService();

                StripeResponse stripeResponse = new StripeResponse();
                try
                {
                    var stripeToken = tokenService.Create(stripeTokenCreateOptions);

                    token = stripeToken.Id;
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("CardError", ex.Message);
                    return View(model);
                    //Log.Write("Merchant Auto Payment Exception : " + ex.Message);
                }
                if (string.IsNullOrEmpty(token))
                {
                    var chargeOptions = new StripeChargeCreateOptions()
                    {
                        Amount = (Int32)model.TopUpAmount * 100,
                        Currency = Common.Common.GetCountryCurrency(model.SenderCountryCode),
                        Description = "Charge for " + model.NameOnCard,
                        //SourceTokenOrExistingSourceId = "tok_mastercard",// obtained with Stripe.js
                        SourceTokenOrExistingSourceId = token
                    };
                    var chargeService = new StripeChargeService();
                    StripeCharge charge = chargeService.Create(chargeOptions);
                    #endregion

                }

            }



            string ReceiptNo = PaySomeoneElseCardByAdminServices.GetNewMFTCCardTopUpReceipt();
            TopUpSomeoneElseCardTransaction paysomeoneElseModel = new TopUpSomeoneElseCardTransaction()
            {

                FaxerId = model.SenderId,
                KiiPayPersonalWalletId = model.CardUserId,
                ExchangeRate = model.CurrentExchangeRate,
                FaxingAmount = model.TopUpAmount,
                FaxingFee = model.TopUpFee,
                PaymentMethod = "Payed by Admin",
                TopUpReference = model.TopUpReference,
                RecievingAmount = model.ReceivingAmount,
                PayedBy = PayedBy.Admin,
                ReceiptNumber = ReceiptNo,
                TransactionDate = DateTime.Now,
                PayingStaffName = Common.StaffSession.LoggedStaff.FirstName + " " + Common.StaffSession.LoggedStaff.MiddleName + " " + Common.StaffSession.LoggedStaff.LastName,
                PayingStaffId = Common.StaffSession.LoggedStaff.StaffId

            };
            var result = PaySomeoneElseCardByAdminServices.TopUpSomeoneElseCard(paysomeoneElseModel);

            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";

            var MFTCCardDetails = PaySomeoneElseCardByAdminServices.GetMFTCCardInformationByID(result.KiiPayPersonalWalletId);
            string SetAutoTopUp = baseUrl + "/FaxerAutoPayments/AddSomeoneElseMoneyFaxCardAutoTopUp?MFTCCardid=" + model.CardUserId;
            string TopUpMoneyfaxCard = baseUrl + "/TopUpSomeoneElseMFTCCard/MFTCCardAccountNo?MFTCCardNO=" + MFTCCardDetails.MobileNo.Decrypt();
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardTopUp?FaxerName=" + model.SenderFirstName + " " + model.SenderMiddleName + " " + model.SenderLastName +
                "&CardUserCountry=" + model.CardUserCountry + "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard + "&SetAutoTopUp=" + SetAutoTopUp);

            string ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardTopUpReceipt?ReceiptNumber=" + result.ReceiptNumber + "&Date=" +
                result.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + result.TransactionDate.ToString("HH:mm")
                + "&FaxerFullName=" + model.SenderFirstName + " " + model.SenderMiddleName + " " + model.SenderLastName +
                "&MFTCCardNumber=" + MFTCCardDetails.MobileNo.Decrypt()
                + "&CardUserFullName=" + MFTCCardDetails.FirstName + " " + MFTCCardDetails.MiddleName + " " + MFTCCardDetails.LastName
                + "&AmountTopUp=" + result.FaxingAmount + " " + model.SenderCurrency + "&ExchangeRate=" + result.ExchangeRate +
                "&AmountInLocalCurrency=" + result.RecievingAmount + " " + model.CardUserCurrency + "&Fee=" + result.FaxingFee + " " + model.SenderCurrency
                + "&BalanceOnCard=" + MFTCCardDetails.CurrentBalance + " " + model.CardUserCurrency + "&TopupReference=" + model.TopUpReference;
            var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);

            mail.SendMail(model.SenderEmail, "Confirmation of Virtual Account Payment", body, ReceiptPDF);
            //Sms
            SmsApi smsApiServices = new SmsApi();
            string senderName = model.SenderFirstName + " " + model.SenderMiddleName + " " + model.SenderLastName;
            string virtualAccounntNo = MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo();
            CommonServices commonservices = new CommonServices();

            string amount = commonservices.getCurrencySymbol(model.SenderCountryCode) + model.TopUpAmount;
            string receivingAmount = commonservices.getCurrencySymbol(model.CardUserCountryCode) + model.ReceivingAmount;
            string message = smsApiServices.GetVirtualAccountDepositMessage(senderName, virtualAccounntNo, amount, receivingAmount);
            string phoneNumber = commonservices.getPhoneCodeFromCountry(model.SenderCountryCode) + model.SenderTelephone;
            smsApiServices.SendSMS(phoneNumber, message);

            string receiverPhoneNo = Common.Common.GetCountryPhoneCode(MFTCCardDetails.CardUserCountry) + "" + MFTCCardDetails.CardUserTel;
            smsApiServices.SendSMS(receiverPhoneNo, message);

            //ModelState.Clear();

            ModelState.Clear();

            ViewBag.TransactionMessage = "Payment Completed Successfully";

          
            return View();
        }

        public JsonResult GetSavedCardDetails(string CardNum , int FaxerId)
        {


            var result = PaySomeoneElseCardByAdminServices.GetCreditCardDetails(CardNum , FaxerId);

            return Json(new
            {
                NameOnCard = result.CardName.Decrypt(),
                CardNumber = "**** **** ****" +  result.Num.Decrypt().Substring(result.Num.Decrypt().Length - 4 , 4),
                CardEndMonth = "**",
                CardEndYear = "**",
                CardSecurityNo = "***",
            }, JsonRequestBehavior.AllowGet);


        }


        public JsonResult GetSenderDetails(string SenderAccountNo)
        {

            var result = PaySomeoneElseCardByAdminServices.GetSenderInformation(SenderAccountNo);
            if (result == null)
            {

                return Json(new
                {
                    InvalidSenderDetails = true
                }, JsonRequestBehavior.AllowGet);
            }

            var savedCardList = PaySomeoneElseCardByAdminServices.getsavedcardList(result.Id);

            var card = new SelectList(savedCardList.ToList(), "Key", "value");
            return Json(new
            {
                SenderId = result.Id,
                SenderFirstName = result.FirstName,
                SenderMiddleName = result.MiddleName,
                SenderLastName = result.LastName,
                SenderAddress = result.Address1,
                SenderCity = result.City,
                SenderCountry = Common.Common.GetCountryName(result.Country),
                SenderCountryCode = result.Country,
                SenderEmail = result.Email,
                SenderIdCardExpDate = result.IdCardExpiringDate.ToString("dd/MM/yyyy"),
                SenderIdCardIssuingCountry = Common.Common.GetCountryName(result.IssuingCountry),
                SenderIdCardNumber = result.IdCardNumber,
                SenderIdCardType = result.IdCardType,
                SenderTelephone = Common.Common.GetCountryPhoneCode(result.Country) + " " + result.PhoneNumber,
                SenderCurrency = Common.Common.GetCountryCurrency(result.Country),
                SenderCurrencySymbol = Common.Common.GetCurrencySymbol(result.Country),
                InvalidSenderDetails = false,
                CardNumberList = savedCardList


            }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetMFTCCardDetails(string MFTCCardNo)
        {


            var result = PaySomeoneElseCardByAdminServices.GetMFTCCardInformation(MFTCCardNo);

            string ErrorMessage = "";
            if (result == null)
            {

                return Json(new { ErrorMessage = "Please enter a valid Virtual Account No", InvalidCardUserDetails = true }, JsonRequestBehavior.AllowGet);
            }
            else if (result.CardStatus != CardStatus.Active) {

                switch (result.CardStatus)
                {
                    case CardStatus.InActive:
                        ErrorMessage = "Virtual Account Deactivated , please contact MoneyFex support";
                        break;
                    case CardStatus.IsDeleted:

                        ErrorMessage = "Virtual Account Deleted , please contact MoneyFex support";
                        break;
                    case CardStatus.IsRefunded:

                        ErrorMessage = "Virtual Account Refunded , please contact MoneyFex support";
                        break;
                    default:
                        break;
                }
                return Json(new { ErrorMessage = ErrorMessage , InvalidCardUserDetails = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                CardUserId = result.Id,
                CardUserFirstName = result.FirstName,
                CardUserMiddleName = result.MiddleName,
                CardUserLastName = result.LastName,
                CardUserAddress = result.Address1,
                CardUserCity = result.CardUserCity,
                CardUserCountry = Common.Common.GetCountryName(result.CardUserCountry),
                CardUserCountryCode = result.CardUserCountry,
                CardUserCurrency = Common.Common.GetCountryCurrency(result.CardUserCountry),
                CardUserCurrencySymbol = Common.Common.GetCurrencySymbol(result.CardUserCountry),
                CardUserEmail = result.CardUserEmail,
                CardUserPhotoURL = result.UserPhoto,
                CardUserTelephone = Common.Common.GetCountryPhoneCode(result.CardUserCountry) + " " + result.CardUserTel,
                InvalidCardUserDetails = false,
                

            }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetTopUpDetails(decimal TopUpAmount, decimal RecevingAmount, string FaxingCountry, string ReceivingCountry)
        {

            string FaxingCountryCode = FaxingCountry;
            string ReceivingCountryCode = ReceivingCountry;
            decimal exchangeRate = 0, faxingFee = 0;
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
                    exchangeRate = Math.Round(1 / exchangeRateObj.CountryRate1, 6, MidpointRounding.AwayFromZero);
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
            }
            if (RecevingAmount > 0)
            {
                TopUpAmount = RecevingAmount;
            }
            var feeSummary = SEstimateFee.CalculateFaxingFee(TopUpAmount, false, RecevingAmount > 0, exchangeRate, SEstimateFee.GetFaxingCommision(FaxingCountryCode));


            return Json(new
            {
                TopUpAmount = feeSummary.FaxingAmount,
                TopUpFee = feeSummary.FaxingFee,
                IncludingFee = feeSummary.IncludingFaxingFee,
                AmountIncludingFee = feeSummary.TotalAmount,
                ReceivingAmount = feeSummary.ReceivingAmount,
                CurrentExchangeRate = feeSummary.ExchangeRate


            }, JsonRequestBehavior.AllowGet);
        }
    }
}