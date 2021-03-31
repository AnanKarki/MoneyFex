using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class TopUpSomeoneElseMFTCCardController : Controller
    {
        // GET: TopUpSomeoneElseMFTCCard
        Services.STopUpSomeoneElseCard TopUpSomeoneElseCardServices = null;

        CommonServices CommonService = new CommonServices();

        DB.FAXEREntities context = new DB.FAXEREntities();
        public TopUpSomeoneElseMFTCCardController()
        {
            TopUpSomeoneElseCardServices = new Services.STopUpSomeoneElseCard();
        }
        public ActionResult Index()
        {
            if (FaxerSession.LoggedUser == null)
            {

                return RedirectToAction("login", "faxeraccount");
            }

            if (Common.FaxerSession.MFTCCard == null)
            {

                ViewBag.MFTCCardNO = "";

            }
            else
            {
                ViewBag.MFTCCardNO = Common.FaxerSession.MFTCCard;
            }
            return View();
        }
        [HttpGet]
        public ActionResult MFTCCardAccountNo(string MFTCCardNO)
        {
            if (FaxerSession.LoggedUser == null)
            {

                return RedirectToAction("login", "faxeraccount");
            }
            if (Common.FaxerSession.MFTCCard == null)
            {
            }
            else
            {
                if (Common.FaxerSession.MFTCCard == MFTCCardNO && !string.IsNullOrEmpty(Common.FaxerSession.TransactionSummaryUrl))
                {
                    string[] tokens = Common.FaxerSession.TransactionSummaryUrl.Split('/');
                    return RedirectToAction(tokens[2]);
                }
            }

            var Card = new KiiPayPersonalWalletInformation();

            // MFTC Card No is null
            if (string.IsNullOrEmpty(MFTCCardNO))
            {
                TempData["MFTCCardNumber"] = "Please enter a valid virtual account no to continue";
                return RedirectToAction("Index");
            }
            //end 
            string[] tokens2 = MFTCCardNO.Split('-');
            var result = new DB.KiiPayBusinessWalletInformation();
            if (tokens2.Length < 2)
            {
                Card = TopUpSomeoneElseCardServices.GetCardUserInfoByNumberOnly(MFTCCardNO.Trim());

            }
            else
            {
                Card = TopUpSomeoneElseCardServices.GetCarduserInfo(MFTCCardNO.Trim());
            }
            Models.TopUpSomeoneElseCardViewModel vm = new Models.TopUpSomeoneElseCardViewModel();
            if (Card != null)
            {
                //if (Card.FaxerId == Common.FaxerSession.LoggedUser.Id)
                //{

                //    TempData["MFTCCardNumber"] = "The virtual accout no you have entered is your own";
                //    return RedirectToAction("Index");
                //}
                 if (Card.CardStatus == CardStatus.InActive)
                {


                    TempData["MFTCCardNumber"] = "Virtual Account Deactivated, please contact MoneyFex Support";
                    return RedirectToAction("Index");
                }
                else if (Card.CardStatus == CardStatus.IsDeleted)
                {


                    TempData["MFTCCardNumber"] = "Virtual Account Deleted, please contact MoneyFex Support";
                    return RedirectToAction("Index");
                }
                else if (Card.CardStatus == CardStatus.IsRefunded)
                {


                    TempData["MFTCCardNumber"] = "Virtual Account Refunded, please contact MoneyFex Support";
                    return RedirectToAction("Index");
                }
                vm.Id = Card.Id;
                vm.NameOfCardUser = Card.FirstName + " " + Card.MiddleName + " " + Card.LastName;
                vm.TopUpCardNumber = Card.MobileNo.Decrypt().GetVirtualAccountNo();
                vm.RegisteredCity = Card.CardUserCity;
                vm.RegisteredCountry = Common.Common.GetCountryName(Card.CardUserCountry);
                vm.RegisteredCountryCode = Card.CardUserCountry;
                vm.Telephone = Card.CardUserTel;
                vm.PhoneCode = Common.Common.GetCountryPhoneCode(Card.CardUserCountry);
                vm.Email = Card.CardUserEmail;
                vm.Website = "";

                Common.FaxerSession.MFTCCard = MFTCCardNO;
            }
            else
            {

                TempData["MFTCCardNumber"] = "Please enter a valid virtual account no";
                return RedirectToAction("Index");
            }

            Common.FaxerSession.FaxingCountry = Common.FaxerSession.LoggedUser.CountryCode;
            return View(vm);





        }
        [HttpPost]
        public ActionResult MFTCCardAccountNo([Bind(Include = TopUpSomeoneElseCardViewModel.BindProperty)]TopUpSomeoneElseCardViewModel vm)
        {

            if (vm.Confirm == false)
            {
                ModelState.AddModelError("Confirm", "please check the box to proceed the transction");
            }
            else
            {
                Common.FaxerSession.TopUpCardId = vm.Id.ToString();
                Common.FaxerSession.ReceivingCountry = vm.RegisteredCountryCode;
                return RedirectToAction("TopUpAmount");

            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult TopUpAmount()
        {
            TopUpSomeoneElseCardEstimateFaxingSummaryViewModel model = new TopUpSomeoneElseCardEstimateFaxingSummaryViewModel();
            model.FaxingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.FaxingCountry);
            model.FaxingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.FaxingCountry);
            model.ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.ReceivingCountry);
            model.ReceivingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.ReceivingCountry);
            if (Common.FaxerSession.FaxingAmountSummary != null)
            {
                model.FaxingAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount - Common.FaxerSession.FaxingAmountSummary.FaxingFee;
                model.PaymentReference = Common.FaxerSession.PaymentRefrence;
                model.IncludeFaxingFee = Common.FaxerSession.FaxingAmountSummary.IncludingFaxingFee;

            }
            return View(model);
        }


        [HttpPost]
        public ActionResult TopUpAmount([Bind(Include = TopUpSomeoneElseCardEstimateFaxingSummaryViewModel.BindProperty)]TopUpSomeoneElseCardEstimateFaxingSummaryViewModel model)
        {

            var FaxerCardPhoto = context.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            //If User has entered a receiving amount then we should
            // take receiving Country else sending country 
            string SendORReceivingCountry = model.ReceivingAmount > 0 ? Common.FaxerSession.ReceivingCountry : Common.FaxerSession.LoggedUser.CountryCode;
            if (model.FaxingAmount == 0 && model.ReceivingAmount == 0)
            {
                ModelState.AddModelError("FaxingAmount", "Please enter an amount to proceed");
            }
            else if (string.IsNullOrEmpty(FaxerCardPhoto.CardUrl) &&
                (Common.Common.IsValidAmountToTransfer(model.FaxingAmount, model.ReceivingAmount, SendORReceivingCountry) == false))
            {

                ViewBag.ToUrl = "/TopUpSomeoneElseMFTCCard/TopUpAmount";
                Common.FaxerSession.ToUrl = "/TopUpSomeoneElseMFTCCard/TopUpAmount";
                ModelState.AddModelError("CardURLError", "You are about to make a transfer of over {currency} 1000, to comply with anti-money laundering regulations, MoneyFex is required by law to ask for a copy of /n " +
                    "your Photo Identification Document (ID).Please upload a copy of your ID to proceed with this Transaction.");



            }
            else if (model.PaymentReference == null)
            {
                ModelState.AddModelError("PaymentReference", "Payment Reference is Required");
            }

            else
            {
                string FaxingCountryCode = Common.FaxerSession.FaxingCountry;
                string ReceivingCountryCode = Common.FaxerSession.ReceivingCountry;
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
                    return View(model);
                }
                if (model.ReceivingAmount > 0)
                {
                    model.FaxingAmount = model.ReceivingAmount;
                }
                var feeSummary = Services.SEstimateFee.CalculateFaxingFee(model.FaxingAmount, model.IncludeFaxingFee, model.ReceivingAmount > 0, exchangeRate, Services.SEstimateFee.GetFaxingCommision(FaxingCountryCode) );
                Common.FaxerSession.FaxingAmountSummary = feeSummary;
                Common.FaxerSession.PaymentRefrence = model.PaymentReference;
                return RedirectToAction("TopUpDetails", new { model = model.PaymentReference });
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult TopUpDetails(string model)
        {
            if (Common.FaxerSession.FaxingAmountSummary != null)
            {
                var faxingSummarry = Common.FaxerSession.FaxingAmountSummary;
                TopUpSomeoneElseCardPayingDetailsViewModel vm = new TopUpSomeoneElseCardPayingDetailsViewModel()
                {
                    FaxingFee = faxingSummarry.FaxingFee,
                    AmountToBeReceivedByCardUser = faxingSummarry.ReceivingAmount,
                    CurrentExchangeRate = faxingSummarry.ExchangeRate,
                    faxingAmount = faxingSummarry.FaxingAmount,
                    PaymentReference = Common.FaxerSession.PaymentRefrence,
                    TotalAmountIncludingFee = faxingSummarry.TotalAmount,
                    FaxingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.FaxingCountry),
                    FaxingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.FaxingCountry),
                    ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.ReceivingCountry),
                    ReceivingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.ReceivingCountry)

                };
                return View(vm);
            }
            return RedirectToAction("MerchantDetails", Common.FaxerSession.MerchantACNumber);
        }
        [HttpPost]
        public ActionResult TopUpDetails()
        {
            if (!string.IsNullOrEmpty(Common.FaxerSession.TransactionSummaryUrl))
            {
                return Redirect(Common.FaxerSession.TransactionSummaryUrl);
            }
            return RedirectToAction("Index", "FraudAlert", new { FormURL = "/TopUpSomeoneElseMFTCCard/PaymentMethod", BackUrl = "/TopUpSomeoneElseMFTCCard/TopUpDetails" });
        }

        public ActionResult PaymentMethod()
        {
            if (Common.FaxerSession.LoggedUser == null)
            {

                return RedirectToAction("login", "faxeraccount");
            }

            return View();

        }
        [HttpGet]
        public ActionResult TopUpSomeoneElseCard()
        {

            Models.CreditDebitCardViewModel model = new Models.CreditDebitCardViewModel()
            {
                FaxingAmount = FaxerSession.FaxingAmountSummary.TotalAmount,
                FaxingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode),
                FaxingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode)

            };

            if (Common.FaxerSession.CreditDebitDetails != null)
            {
                model = Common.FaxerSession.CreditDebitDetails;
            }

            var CardCount = context.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).Count();
            ViewBag.CardCount = CardCount;
            ViewBag.Countries = new SelectList(context.Country.ToList(), "CountryCode", "CountryName");

            #region Auto Payment has been setuped or not 
            int MFTCCardId = int.Parse(FaxerSession.TopUpCardId);
            var AutohasBeenSetUped = context.OtherMFTCCardAutoTopUpInformation.Where(x => x.FaxerId == Common.FaxerSession.LoggedUser.Id && x.MFTCCardId == MFTCCardId).FirstOrDefault();
            if ((AutohasBeenSetUped != null) && AutohasBeenSetUped.EnableAutoPayment == true)
            {

                ViewBag.AutoTopUP = 1;

            }
            else
            {

                ViewBag.AutoTopUP = 0;
            }
            #endregion
            return View(model);


        }
        [HttpPost]
        public ActionResult TopUpSomeoneElseCard([Bind(Include = CreditDebitCardViewModel.BindProperty)]CreditDebitCardViewModel model)
        {
            // Countries Drop Down List
            ViewBag.Countries = new SelectList(context.Country.ToList(), "CountryCode", "CountryName");

            #region Auto Payment has been setuped or not 
            int MFTCCardId = int.Parse(FaxerSession.TopUpCardId);
            var AutohasBeenSetUped = context.OtherMFTCCardAutoTopUpInformation.Where(x => x.FaxerId == Common.FaxerSession.LoggedUser.Id && x.MFTCCardId == MFTCCardId).FirstOrDefault();
            if ((AutohasBeenSetUped != null) && AutohasBeenSetUped.EnableAutoPayment == true)
            {

                ViewBag.AutoTopUP = 1;

            }
            else
            {

                ViewBag.AutoTopUP = 0;
            }
            #endregion
            bool valid = true;

            var CardCount = context.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).Count();
            ViewBag.CardCount = CardCount;
            //string CountryCode = db.Country.Where(x => x.CountryName.ToLower() == model.CountyName.ToLower()).FirstOrDefault().CountryCode;
            //var Country = context.Country.Where(x => x.CountryName.ToLower() == model.CountyName.ToLower()).FirstOrDefault();
            //if (Country == null)
            //{
            //    ModelState.AddModelError("CountyName", "Country Name Is Not Matched!!");
            //    return View(model);
            //}
            string CountryCode = model.CountyName;
            model.CountyName = CountryCode;

            Services.SFaxerSignUp faService = new Services.SFaxerSignUp();
            var faInformation = faService.GetInformation(FaxerSession.LoggedUser.UserName);
            if (!string.IsNullOrEmpty(model.EndMM) && !string.IsNullOrEmpty(model.EndYY))
            {
                var CurrentYearToString = DateTime.Now.Year.ToString();
                var CurrentYear = Convert.ToInt32(CurrentYearToString.Substring(2, 2));
                var CurrentMonth = DateTime.Now.Month;
                if (Convert.ToInt32(model.EndMM) > 12 || Convert.ToInt32(model.EndMM) < 0)
                {

                    ModelState.AddModelError("EndMM", "Please enter a valid month");
                    valid = false;
                }
                if (Convert.ToInt32(model.EndYY) < CurrentYear)
                {

                    ModelState.AddModelError("EndYY", "Your Card Has been expired");
                    valid = false;
                }
                if ((Convert.ToInt32(model.EndYY) == CurrentYear) && Convert.ToInt32(model.EndMM) < CurrentMonth)
                {
                    ModelState.AddModelError("EndYY", "Your Card has been expired");
                    valid = false;
                }
            }

            if (string.IsNullOrEmpty(model.NameOnCard))
            {
                ModelState.AddModelError("NameOnCard", "Name is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.CardNumber))
            {
                ModelState.AddModelError("CardNumber", "Card Number is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.EndMM))
            {
                ModelState.AddModelError("EndMM", "End Month is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.EndYY))
            {
                ModelState.AddModelError("EndYY", "End Year is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.SecurityCode))
            {
                ModelState.AddModelError("SecurityCode", "SecurityCode is required");
                valid = false;
            }
            bool MatchSenderName = false;

            if (!string.IsNullOrEmpty(model.NameOnCard))
            {

                MatchSenderName = Common.Common.compareFaxerNameOnCard(model.NameOnCard, FaxerSession.LoggedUser.Id);

            }


            if (MatchSenderName == false)

            {
                ModelState.AddModelError("NameOnCard", "Credit/Debit Card Name doesn't match with sender's name !");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.AddressLineOne))
            {

                ModelState.AddModelError("AddressLineOne", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.CityName))
            {

                ModelState.AddModelError("CityName", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.CountyName))
            {

                ModelState.AddModelError("CountyName", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.ZipCode))
            {

                ModelState.AddModelError("ZipCode", "The field is required");
                valid = false;
            }


            if (!string.IsNullOrEmpty(model.AddressLineOne) && model.AddressLineOne.Trim().ToLower() != faInformation.Address1.Trim().ToLower())
            {
                ModelState.AddModelError("AddressLineOne", "Address line one does not match with your registered address,");
                valid = false;
            }

            if (!string.IsNullOrEmpty(model.CityName) && model.CityName.Trim().ToLower() != faInformation.City.Trim().ToLower())
            {
                ModelState.AddModelError("CityName", "City Name Is Not Matched!!");
                valid = false;
            }
            if (!string.IsNullOrEmpty(model.CountyName) && model.CountyName.Trim().ToLower() != faInformation.Country.Trim().ToLower())
            {
                ModelState.AddModelError("CountyName", "County Name Is Not Matched!!");
                valid = false;
            }

            if (!string.IsNullOrEmpty(model.ZipCode) && model.ZipCode.Trim().ToLower() != faInformation.PostalCode.Trim().ToLower())
            {
                ModelState.AddModelError("ZipCode", "Zip/Post Code does not match with your registered Zip/Post Code     ");
                valid = false;
            }
            if (model.Confirm == false)
            {
                ModelState.AddModelError("Confirm", "Accept our Terms and Conditions before Continue!!");
                valid = false;
            }
            var SavedCardCount = context.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).Count();

            if ((SavedCardCount == 0) && model.SaveCard == false && model.AutoTopUp == true)
            {

                ModelState.AddModelError("AutoTopUp", "please add credit/debit card to enable Auto Top-up");
                valid = false;

            }


            #region  Strip portion
            //StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");
            StripeConfiguration.SetApiKey(System.Configuration.ConfigurationManager.AppSettings["StripeApiLiveKey"]);

            var stripeTokenCreateOptions = new StripeTokenCreateOptions
            {
                Card = new StripeCreditCardOptions
                {
                    Number = model.CardNumber,
                    ExpirationMonth = int.Parse(model.EndMM),
                    ExpirationYear = int.Parse(model.EndYY),
                    Cvc = model.SecurityCode,
                    Name = model.NameOnCard
                }
            };

            string token = "";
            var tokenService = new StripeTokenService();

            StripeResponse stripeResponse = new StripeResponse();
            try
            {
                var stripeToken = tokenService.Create(stripeTokenCreateOptions);

                model.StripeTokenID = stripeToken.Id;
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("ErrorMessage", ex.Message);
                valid = false;

                //Log.Write("Merchant Auto Payment Exception : " + ex.Message);
            }
            #endregion
            if (valid)
            {
                // Initiate Session Credit/DebitCardDetails 
                Common.FaxerSession.CreditDebitDetails = model;
                // End 
                return RedirectToAction("TopUpSomeoneElseCardTransactionSummary");
            }
            else
            {
                return View(model);
            }


            return View();

        }

        [HttpGet]
        public ActionResult TopUpSomeoneElseCardTransactionSummary()
        {
            var FaxerDetails = context.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            var FaxerCountry = Common.Common.GetCountryName(FaxerDetails.Country);
            var faxerPhoneCode = Common.Common.GetCountryPhoneCode(FaxerDetails.Country);
            int CardUserId = int.Parse(FaxerSession.TopUpCardId);
            var CardUserDetails = context.KiiPayPersonalWalletInformation.Where(x => x.Id == CardUserId).FirstOrDefault();
            string FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            string CardUserCurrency = Common.Common.GetCountryCurrency(CardUserDetails.CardUserCountry);
            Models.MFTCCardTransactionSummaryViewModel model = new MFTCCardTransactionSummaryViewModel()
            {
                CardUserId = CardUserId,
                CardUsername = CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName,
                MFTCCardNumber = CardUserDetails.MobileNo.Decrypt().GetVirtualAccountNo(),
                ReceiveOption = "Virtual Account Withdrawal",
                FaxerName = Common.FaxerSession.LoggedUser.FullName,
                FaxerEmail = Common.FaxerSession.LoggedUser.UserName,
                FaxerPhoneNumber = faxerPhoneCode + " " + FaxerDetails.PhoneNumber,
                CardNumber = Common.FaxerSession.CreditDebitDetails.CardNumber.FormatSavedCardNumber(),
                CardExpriyDate = Common.FaxerSession.CreditDebitDetails.EndMM + "/" + Common.FaxerSession.CreditDebitDetails.EndYY,
                CountryOfBirth = FaxerCountry,
                streetAddress = Common.FaxerSession.CreditDebitDetails.AddressLineOne,
                City = Common.FaxerSession.CreditDebitDetails.CityName,
                PostalCode = Common.FaxerSession.CreditDebitDetails.ZipCode,
                SentAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount.ToString() + " " + FaxerCurrency,
                Fees = Common.FaxerSession.FaxingAmountSummary.FaxingFee.ToString() + " " + FaxerCurrency,
                TotalAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount.ToString() + " " + FaxerCurrency,
                TotalReceiveAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount.ToString() + " " + CardUserCurrency

            };
            Common.FaxerSession.TransactionSummaryUrl = "/TopUpSomeoneElseMFTCCard/TopUpSomeoneElseCardTransactionSummary";
            return View(model);



        }


        [HttpPost]
        public ActionResult TopUpSomeoneElseCardTransactionSummary([Bind(Include = MFTCCardTransactionSummaryViewModel.BindProperty)]MFTCCardTransactionSummaryViewModel vm)
        {

            bool valid = true;

            Services.SFaxerSignUp faService = new Services.SFaxerSignUp();
            var faInformation = faService.GetInformation(FaxerSession.LoggedUser.UserName);

            //if (ModelState.IsValid)
            //{
            if (valid)
            {
                #region  Strip portion
                //StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");
                StripeConfiguration.SetApiKey(System.Configuration.ConfigurationManager.AppSettings["StripeApiLiveKey"]);


                var chargeOptions = new StripeChargeCreateOptions()
                {
                    Amount = (Int32)Common.FaxerSession.CreditDebitDetails.FaxingAmount * 100,
                    Currency = Common.FaxerSession.CreditDebitDetails.FaxingCurrency,
                    Description = "Charge for " + Common.FaxerSession.CreditDebitDetails.NameOnCard,
                    SourceTokenOrExistingSourceId = Common.FaxerSession.CreditDebitDetails.StripeTokenID // obtained with Stripe.js
                };
                var chargeService = new StripeChargeService();


                StripeCharge charge = chargeService.Create(chargeOptions);
                #endregion

                SFaxingTopUpCardTransaction service = new SFaxingTopUpCardTransaction();

                STopUpSomeoneElseCard TopUpSomeoneElseServices = new STopUpSomeoneElseCard();
                string ReceiptNumber = service.GetNewMFTCCardTopUpReceipt();
                //transaction history object
                DB.TopUpSomeoneElseCardTransaction obj = new DB.TopUpSomeoneElseCardTransaction()
                {
                    KiiPayPersonalWalletId = int.Parse(FaxerSession.TopUpCardId),
                    FaxerId = FaxerSession.LoggedUser.Id,
                    FaxingAmount = FaxerSession.FaxingAmountSummary.FaxingAmount,
                    RecievingAmount = FaxerSession.FaxingAmountSummary.ReceivingAmount,
                    ExchangeRate = FaxerSession.FaxingAmountSummary.ExchangeRate,
                    FaxingFee = FaxerSession.FaxingAmountSummary.FaxingFee,
                    ReceiptNumber = ReceiptNumber,
                    TransactionDate = System.DateTime.Now,
                    TopUpReference = Common.FaxerSession.PaymentRefrence,
                    PaymentMethod = "PM001",
                };
                obj = TopUpSomeoneElseServices.SaveTransaction(obj);

                /// Send Email 
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                string body = "";

                string CardUserCountry = service.GetCardUserCountry(obj.KiiPayPersonalWalletId);

                var MFTCCardDetails = service.GetMFTCCardInformation(obj.KiiPayPersonalWalletId);
                string SetAutoTopUp = baseUrl + "/FaxerAutoPayments/AddSomeoneElseMoneyFaxCardAutoTopUp?MFTCCardid=" + obj.KiiPayPersonalWalletId;
                string TopUpMoneyfaxCard = baseUrl + "/TopUpSomeoneElseMFTCCard/MFTCCardAccountNo?MFTCCardNO=" + MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo();
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardTopUp?FaxerName=" + FaxerName +
                    "&CardUserCountry=" + CardUserCountry + "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard + "&SetAutoTopUp=" + SetAutoTopUp);


                string FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
                string CardUserCurrency = Common.Common.GetCountryCurrency(MFTCCardDetails.CardUserCountry);
                string ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardTopUpReceipt?ReceiptNumber=" + obj.ReceiptNumber + "&Date=" +
                    obj.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + obj.TransactionDate.ToString("HH:mm")
                    + "&FaxerFullName=" + FaxerName + "&MFTCCardNumber=" + MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo()
                    + "&CardUserFullName=" + MFTCCardDetails.FirstName + " " + MFTCCardDetails.MiddleName + " " + MFTCCardDetails.LastName
                    + "&AmountTopUp=" + obj.FaxingAmount + " " + FaxerCurrency + "&ExchangeRate=" + obj.ExchangeRate +
                    "&AmountInLocalCurrency=" + obj.RecievingAmount + " " + CardUserCurrency + "&Fee=" + obj.FaxingFee + " " + FaxerCurrency + "&BalanceOnCard=" + MFTCCardDetails.CurrentBalance + " " + CardUserCurrency + "&TopupReference=" + Common.FaxerSession.PaymentRefrence;
                var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);

                mail.SendMail(FaxerEmail, "Confirmation of Virtual Account Payment", body, ReceiptPDF);
                // End 

                #region Virtual Account Deposit Sms


                SmsApi smsApiServices = new SmsApi();
                string receivingAmount = Common.Common.GetCurrencySymbol(MFTCCardDetails.CardUserCountry) + obj.RecievingAmount;

                string message = smsApiServices.GetVirtualAccountDepositMessage(FaxerName, MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo(),
                                                                  Common.Common.GetCurrencySymbol(faInformation.Country) + obj.FaxingAmount, receivingAmount);

                string PhoneNo = Common.Common.GetCountryPhoneCode(faInformation.Country) + "" + faInformation.PhoneNumber;
                smsApiServices.SendSMS(PhoneNo, message);


                string receiverPhoneNo = Common.Common.GetCountryPhoneCode(MFTCCardDetails.CardUserCountry) + "" + MFTCCardDetails.CardUserTel;
                smsApiServices.SendSMS(receiverPhoneNo, message);
                #endregion

                if (Common.FaxerSession.CreditDebitDetails.SaveCard)
                {
                    int SavedCardCount = context.SavedCard.Where(x => x.UserId == FaxerSession.LoggedUser.Id).Count();
                    if (SavedCardCount < 1)
                    {
                        DB.SavedCard savedCardObject = new DB.SavedCard()
                        {
                            CardName = Common.FaxerSession.CreditDebitDetails.NameOnCard.Encrypt(),
                            EMonth = Common.FaxerSession.CreditDebitDetails.EndMM.Encrypt(),
                            EYear = Common.FaxerSession.CreditDebitDetails.EndYY.Encrypt(),
                            CreatedDate = System.DateTime.Now,
                            UserId = FaxerSession.LoggedUser.Id,
                            Num = Common.FaxerSession.CreditDebitDetails.CardNumber.Encrypt(),
                            ClientCode = Common.FaxerSession.CreditDebitDetails.SecurityCode.Encrypt()

                        };
                        SSavedCard cardservices = new SSavedCard();
                        savedCardObject = cardservices.Add(savedCardObject);


                        // Send Email For Card saved
                        string CardNumber = "xxxx-xxxx-xxxx-" + savedCardObject.Num.Decrypt().Right(4);
                        string body1 = "";
                        string TopUpMoneyFaxCard_CreditOrDebitCard = baseUrl + "/StartFaxingMoney/Index?mFTCCardInformationId=";
                        string PayForGoodsAbroad = baseUrl + "/PayForGoodsAndServicesAbroad/MerchantDetails?MerchantACNumber=" + "";
                        string SetAutoTopUpPayment = baseUrl + "/FaxerAutoPayments/MoneyFaxCardAutoTopUp?mftcCardInformationId=";

                        body1 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/NewCreditDebitCardAddedEmail?FaxerName=" + FaxerName
                            + "&LastForDigitOfCreditOrDebitCard=" + CardNumber + "&TopUpMoneyfaxCard=" + TopUpMoneyFaxCard_CreditOrDebitCard
                            + "&SetAutoTopUp=" + SetAutoTopUpPayment + "&PayForGoodsAbroad=" + PayForGoodsAbroad);

                        mail.SendMail(FaxerEmail, "New Credit/Debit Card Added ", body1);
                        // End 

                        //if (savedCardObject != null) {
                        //    IsSaved = true;
                        //}
                    }

                }
                if (obj != null)
                {
                    bool IsSaved = false;
                    string cardNumber = Common.FaxerSession.CreditDebitDetails.CardNumber.Encrypt();
                    var SavedCardCount = context.SavedCard.Where(x => x.Num == cardNumber).FirstOrDefault();
                    if (SavedCardCount != null)
                    {
                        IsSaved = true;
                    }
                    DB.CardTopUpCreditDebitInformation cardDetails = new DB.CardTopUpCreditDebitInformation()
                    {
                        TopUpSomeoneElseTransactionId = obj.Id,
                        NameOnCard = Common.FaxerSession.CreditDebitDetails.NameOnCard,
                        ExpiryDate = Common.FaxerSession.CreditDebitDetails.EndMM + "/" + Common.FaxerSession.CreditDebitDetails.EndYY,
                        CardNumber = "xxxx-xxxx-xxxx-" + Common.FaxerSession.CreditDebitDetails.CardNumber.Right(4),
                        IsSavedCard = IsSaved,
                        AutoRecharged = Common.FaxerSession.CreditDebitDetails.AutoTopUp,
                    };
                    SSavedCard cardInformationservices = new SSavedCard();
                    cardDetails = cardInformationservices.Save(cardDetails);
                }

                if ((Common.FaxerSession.CreditDebitDetails.AutoTopUp == true)
               && Common.FaxerSession.CreditDebitDetails.AutoTopUpAmount > 0 && (int)Common.FaxerSession.CreditDebitDetails.PaymentFrequency > 0)
                {
                    var cardDetails = context.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                    if (cardDetails != null)
                    {
                        var data = context.OtherMFTCCardAutoTopUpInformation.Where(x => x.MFTCCardId == obj.KiiPayPersonalWalletId && x.FaxerId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();


                        if (data != null)
                        {

                            data.AutoPaymentAmount = Common.FaxerSession.CreditDebitDetails.AutoTopUpAmount;
                            data.AutoPaymentFrequency = Common.FaxerSession.CreditDebitDetails.PaymentFrequency;
                            data.EnableAutoPayment = true;
                            data.FrequencyDetails = Common.FaxerSession.CreditDebitDetails.PaymentDay;
                            data.TopUpReference = Common.FaxerSession.PaymentRefrence;
                            context.Entry(data).State = EntityState.Modified;
                            context.SaveChanges();

                        }
                        else
                        {
                            OtherMFTCCardAutoTopUpInformation autoTopUpInformation = new OtherMFTCCardAutoTopUpInformation()
                            {

                                MFTCCardId = obj.KiiPayPersonalWalletId,
                                AutoPaymentAmount = Common.FaxerSession.CreditDebitDetails.AutoTopUpAmount,
                                AutoPaymentFrequency = Common.FaxerSession.CreditDebitDetails.PaymentFrequency,
                                EnableAutoPayment = true,
                                FaxerId = Common.FaxerSession.LoggedUser.Id,
                                TopUpReference = Common.FaxerSession.PaymentRefrence,
                                FrequencyDetails = Common.FaxerSession.CreditDebitDetails.PaymentDay

                            };
                            context.OtherMFTCCardAutoTopUpInformation.Add(autoTopUpInformation);
                            context.SaveChanges();

                        }

                        string FaxerCountry = context.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).Select(x => x.Country).FirstOrDefault();
                        string CountryCurrency = Common.Common.GetCountryCurrency(FaxerCountry);
                        var MFTCCardDetials = context.KiiPayPersonalWalletInformation.Where(x => x.Id == obj.KiiPayPersonalWalletId).FirstOrDefault();
                        string CardNumber = "xxxx-xxxx-xxxx-" + cardDetails.Num.Decrypt().Right(4);
                        string SetAutoPaymentLink = baseUrl + "/FaxerAutoPayments/AddSomeoneElseMoneyFaxCardAutoTopUp?MFTCCardid=" + obj.KiiPayPersonalWalletId;
                        string TopUCard = baseUrl + "/TopUpSomeoneElseMFTCCard/MFTCCardAccountNo?MFTCCardNO=" + MFTCCardDetials.MobileNo.Decrypt();
                        string AutoTOpUP_body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfAutoPaymentSetupSomeoneElseMFTCCard/?FaxerName=" +
                            FaxerName + "&AutoPaymentAmount=" + Common.FaxerSession.CreditDebitDetails.AutoTopUpAmount + "&CountryCurrencySymbol=" + CountryCurrency
                            + "&CardUserName=" + MFTCCardDetials.FirstName + " " + MFTCCardDetials.MiddleName + " " + MFTCCardDetials.LastName
                            + "&AutoPaymentFrequency=" + Common.FaxerSession.CreditDebitDetails.PaymentFrequency
                            + "&CreditORDebitCardlast4digits=" + CardNumber
                            + "&SetAutoPayment=" + SetAutoPaymentLink +
                            "&TopUPMFTCCard=" + TopUCard);
                        mail.SendMail(FaxerEmail, "Confirmation of Auto Payment Setup to someone else's card", AutoTOpUP_body);

                    }
                }


                string MFTCCardNumber = MFTCCardDetails.MobileNo.Decrypt();
                string CardUserName = MFTCCardDetails.FirstName + " " + MFTCCardDetails.MiddleName + " " + MFTCCardDetails.LastName;
                string BalanceOnCard = MFTCCardDetails.CurrentBalance.ToString() + " " + CardUserCurrency;
                string TopUpAmount = FaxerSession.FaxingAmountSummary.FaxingAmount.ToString() + " " + FaxerCurrency;
                string ReceiveAmount = FaxerSession.FaxingAmountSummary.ReceivingAmount.ToString() + " " + CardUserCurrency;
                string ExchangeRate = FaxerSession.FaxingAmountSummary.ExchangeRate.ToString();
                string CardCountry = Common.Common.GetCountryName(CardUserCountry);
                return RedirectToAction("PaymentSuccessed", new { MFTCCardNumber, CardUserName, CardCountry, TopUpAmount, BalanceOnCard, ExchangeRate });

            }
            else
            {
                return View(vm);
            }

        }
        public ActionResult PaymentSuccessed(string MFTCCardNumber, string CardUserName, string CardCountry, string TopUpAmount, string BalanceOnCard, string ExchangeRate)
        {
            Session.Remove("MFTCCard");
            Session.Remove("TransactionSummaryUrl");
            Session.Remove("CreditDebitDetails");
            Session.Remove("FaxingAmountSummary");
            ViewBag.MFTCCardNumber = MFTCCardNumber.GetVirtualAccountNo();
            ViewBag.CardUserName = CardUserName;
            ViewBag.CardUserCountry = CardCountry;
            ViewBag.TopUpAmount = TopUpAmount;
            ViewBag.BalanceOnCard = BalanceOnCard;
            ViewBag.TopUpReference = Common.FaxerSession.PaymentRefrence;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.ReceiveOption = "Virtual Account Withdrawl";
            return View();
        }

        [HttpGet]
        public ActionResult TopUpSomeoneElseCardUsingSavedCreditDebitCard(int savedCardId = 0)
        {
            //List<SavedCard> list = context.SavedCard.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id).OrderBy(x => x.CardName).ToList();

            var list = Common.Common.GetSavedCardDetails();
            if (list.Count() == 0)
            {

                @TempData["CardCount"] = list.Count();
                FaxerSession.FromUrl = "/TopUpSomeoneElseMFTCCard/TopUpSomeoneElseCardUsingSavedCreditDebitCard";
                return RedirectToAction("Index", "PaymentMethod");
            }
            //foreach (var item in list)
            //{
            //    item.CardName = Common.Common.Decrypt(item.CardName);
            //}
            PaymentUsingSavedCreditDebitCardVm vm = new PaymentUsingSavedCreditDebitCardVm();
            if (savedCardId == 0)
            {
                ViewBag.SavedCard = new SelectList(list, "Id", "CardNo");
                vm.TopUpAmount = FaxerSession.FaxingAmountSummary.TotalAmount;
            }
            else
            {
                ViewBag.SavedCard = new SelectList(list, "Id", "CardNo", savedCardId);
                vm = (from c in context.SavedCard.Where(x => x.Id == savedCardId).ToList()
                      select new PaymentUsingSavedCreditDebitCardVm()
                      {
                          NameOnCard = c.CardName.Decrypt(),
                          CardNumberMasked = "**** **** **** " + c.Num.Decrypt().Substring(c.Num.Decrypt().Length - 4, 4),
                          CardNumber = c.Num.Decrypt(),
                          EndMonth = c.EMonth.Decrypt(),
                          EndYear = c.EYear.Decrypt(),
                          SecurityCode = c.ClientCode.Decrypt(),
                          TopUpAmount = FaxerSession.FaxingAmountSummary.TotalAmount,

                      }).FirstOrDefault();

                var billingAddress = Common.Common.GetBillingAddress(savedCardId);
                vm.Address1 = billingAddress.Address1;
                vm.Address2 = billingAddress.Address2;
                vm.City = billingAddress.City;
                vm.PostalCode = billingAddress.PostalCode;
                vm.Country = billingAddress.Country;
            }

            if (Common.FaxerSession.SavedCreditDebitCardDetails != null)
            {
                vm.Address1 = Common.FaxerSession.SavedCreditDebitCardDetails.Address1;
                vm.Address2 = Common.FaxerSession.SavedCreditDebitCardDetails.Address2;
                vm.City = Common.FaxerSession.SavedCreditDebitCardDetails.City;
                vm.Country = Common.FaxerSession.SavedCreditDebitCardDetails.Country;
                vm.PostalCode = Common.FaxerSession.SavedCreditDebitCardDetails.PostalCode;
            }
            vm.FaxingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode);
            vm.FaxingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode);

            ViewBag.Countries = new SelectList(context.Country.ToList(), "CountryCode", "CountryName");
            #region Auto Top Payment has been setuped or not 
            int MFTCCardId = int.Parse(Common.FaxerSession.TopUpCardId);
            var AutohasBeenSetUped = context.OtherMFTCCardAutoTopUpInformation.Where(x => x.FaxerId == Common.FaxerSession.LoggedUser.Id && x.MFTCCardId == MFTCCardId).FirstOrDefault();
            if ((AutohasBeenSetUped != null) && AutohasBeenSetUped.EnableAutoPayment == true)
            {

                ViewBag.AutoTopUP = 1;

            }
            else
            {

                ViewBag.AutoTopUP = 0;
            }
            #endregion
            return View(vm);
        }

        [HttpPost]
        public ActionResult TopUpSomeoneElseCardUsingSavedCreditDebitCard([Bind(Include = PaymentUsingSavedCreditDebitCardVm.BindProperty)]PaymentUsingSavedCreditDebitCardVm vm)
        {

            // Countries Drop Down List 
            ViewBag.Countries = new SelectList(context.Country.ToList(), "CountryCode", "CountryName");

            #region Auto Top Payment has been setuped or not 
            int MFTCCardId = int.Parse(Common.FaxerSession.TopUpCardId);
            var AutohasBeenSetUped = context.OtherMFTCCardAutoTopUpInformation.Where(x => x.FaxerId == Common.FaxerSession.LoggedUser.Id && x.MFTCCardId == MFTCCardId).FirstOrDefault();
            if ((AutohasBeenSetUped != null) && AutohasBeenSetUped.EnableAutoPayment == true)
            {

                ViewBag.AutoTopUP = 1;

            }
            else
            {

                ViewBag.AutoTopUP = 0;
            }
            #endregion
            //List<SavedCard> list = context.SavedCard.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id).OrderBy(x => x.CardName).ToList();

            var list = Common.Common.GetSavedCardDetails();
            //foreach (var item in list)
            //{
            //    item.CardName = Common.Common.Decrypt(item.CardName);
            //}
            ViewBag.SavedCard = new SelectList(list, "Id", "CardNo");

            vm.Country = vm.Country;

            bool valid = true;
            Services.SFaxerSignUp faService = new Services.SFaxerSignUp();
            var faInformation = faService.GetInformation(FaxerSession.LoggedUser.UserName);
            if (!string.IsNullOrEmpty(vm.EndMonth) && !string.IsNullOrEmpty(vm.EndYear))
            {
                var CurrentYearToString = DateTime.Now.Year.ToString();
                var CurrentYear = Convert.ToInt32(CurrentYearToString.Substring(2, 2));
                var CurrentMonth = DateTime.Now.Month;
                if (Convert.ToInt32(vm.EndMonth) > 12 || Convert.ToInt32(vm.EndMonth) < 0)
                {

                    ModelState.AddModelError("EndMonth", "Please enter a valid month");
                    valid = false;
                }
                if (Convert.ToInt32(vm.EndYear) < CurrentYear)
                {

                    ModelState.AddModelError("EndYear", "Your Card Has been expired");
                    valid = false;
                }
                if ((Convert.ToInt32(vm.EndYear) == CurrentYear) && Convert.ToInt32(vm.EndMonth) < CurrentMonth)
                {
                    ModelState.AddModelError("EndYear", "Your Card has been expired");
                    valid = false;
                }
            }
            if (string.IsNullOrEmpty(vm.NameOnCard))
            {
                ModelState.AddModelError("NameOnCard", "Name is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.CardNumber))
            {
                ModelState.AddModelError("CardNumber", "Card Number is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.EndMonth))
            {
                ModelState.AddModelError("EndMonth", "End Month is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.EndYear))
            {
                ModelState.AddModelError("EndYear", "End Year is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.SecurityCode))
            {
                ModelState.AddModelError("SecurityCode", "SecurityCode is required");
                valid = false;
            }

            if (string.IsNullOrEmpty(vm.Address1))
            {

                ModelState.AddModelError("Address1", "The Field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.City))
            {

                ModelState.AddModelError("City", "The Field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.Country))
            {

                ModelState.AddModelError("Country", "The Field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.PostalCode))
            {

                ModelState.AddModelError("PostalCode", "The Field is required");
                valid = false;
            }



            if (!string.IsNullOrEmpty(vm.PostalCode) && vm.Address1.Trim().ToLower() != faInformation.Address1.Trim().ToLower())
            {
                ModelState.AddModelError("Address1", "Address line one does not match with your registered address,");
                valid = false;
            }

            if (!string.IsNullOrEmpty(vm.City) && vm.City.Trim().ToLower() != faInformation.City.Trim().ToLower())
            {
                ModelState.AddModelError("City", "City Name Is Not Matched!!");
                valid = false;
            }
            if (!string.IsNullOrEmpty(vm.Country) && vm.Country.Trim().ToLower() != faInformation.Country.Trim().ToLower())
            {
                ModelState.AddModelError("Country", "County Name Is Not Matched!!");
                valid = false;
            }

            if (!string.IsNullOrEmpty(vm.PostalCode) && vm.PostalCode.Trim().ToLower() != faInformation.PostalCode.Trim().ToLower())
            {
                ModelState.AddModelError("PostalCode", "Zip/Post Code does not match with your registered Zip/Post Code     ");
                valid = false;
            }
            if (vm.Confirm == false)
            {
                ModelState.AddModelError("Confirm", "Accept our Terms and Conditions before Continue!!");
                valid = false;
            }


            #region  Strip portion
            //StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");
            StripeConfiguration.SetApiKey(System.Configuration.ConfigurationManager.AppSettings["StripeApiLiveKey"]);


            var stripeTokenCreateOptions = new StripeTokenCreateOptions
            {
                Card = new StripeCreditCardOptions
                {
                    Number = vm.CardNumber,
                    ExpirationMonth = int.Parse(vm.EndMonth),
                    ExpirationYear = int.Parse(vm.EndYear),
                    Cvc = vm.SecurityCode,
                    Name = vm.NameOnCard
                }
            };
            
            var tokenService = new StripeTokenService();

            StripeResponse stripeResponse = new StripeResponse();
            try
            {
                var stripeToken = tokenService.Create(stripeTokenCreateOptions);

                vm.StripeTokenID = stripeToken.Id;
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("ErrorMessage", ex.Message);
                valid = false;

                //Log.Write("Merchant Auto Payment Exception : " + ex.Message);
            }
            #endregion
            if (valid)
            {
                // Intiate Save Credit/debit Card Session
                Common.FaxerSession.SavedCreditDebitCardDetails = vm;
                //End

                return RedirectToAction("TopUpSomeoneElseCardUsingSavedCreditDebitCardTransactionsumary");
            }

            else
            {
                return View(vm);
            }

        }
        [HttpGet]
        public ActionResult TopUpSomeoneElseCardUsingSavedCreditDebitCardTransactionsumary()
        {

            var FaxerDetails = context.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            var FaxerCountry = Common.Common.GetCountryName(FaxerDetails.Country);
            var faxerPhoneCode = Common.Common.GetCountryPhoneCode(FaxerDetails.Country);
            int CardUserId = int.Parse(FaxerSession.TopUpCardId);
            var CardUserDetails = context.KiiPayPersonalWalletInformation.Where(x => x.Id == CardUserId).FirstOrDefault();
            string FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            string FaxerCurrencySymbol = Common.Common.GetCurrencySymbol(FaxerDetails.Country);
            string CardUserCurrency = Common.Common.GetCountryCurrency(CardUserDetails.CardUserCountry);
            string CardUserCurrencySymbol = Common.Common.GetCurrencySymbol(CardUserDetails.CardUserCountry);
            Models.MFTCCardTransactionSummaryViewModel model = new MFTCCardTransactionSummaryViewModel()
            {
                CardUserId = CardUserId,
                CardUsername = CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName,
                MFTCCardNumber = CardUserDetails.MobileNo.Decrypt(),
                ReceiveOption = "Virtual Account Withdrawal",
                FaxerName = Common.FaxerSession.LoggedUser.FullName,
                FaxerEmail = Common.FaxerSession.LoggedUser.UserName,
                FaxerPhoneNumber = faxerPhoneCode + " " + FaxerDetails.PhoneNumber,
                SavedCardId = int.Parse(Common.FaxerSession.SavedCreditDebitCardDetails.SavedCard),
                CardNumber = Common.FaxerSession.SavedCreditDebitCardDetails.CardNumber.FormatSavedCardNumber(),
                CardExpriyDate = Common.FaxerSession.SavedCreditDebitCardDetails.EndMonth + "/" + Common.FaxerSession.SavedCreditDebitCardDetails.EndYear,
                CountryOfBirth = FaxerCountry,
                streetAddress = Common.FaxerSession.SavedCreditDebitCardDetails.Address1,
                City = Common.FaxerSession.SavedCreditDebitCardDetails.City,
                PostalCode = Common.FaxerSession.SavedCreditDebitCardDetails.PostalCode,
                SentAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount.ToString() + " " + FaxerCurrency,
                Fees = Common.FaxerSession.FaxingAmountSummary.FaxingFee.ToString() + " " + FaxerCurrency,
                TotalAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount.ToString() + " " + FaxerCurrency,
                TotalReceiveAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount.ToString() + " " + CardUserCurrency

            };

            Common.FaxerSession.TransactionSummaryUrl = "/TopUpSomeoneElseMFTCCard/TopUpSomeoneElseCardUsingSavedCreditDebitCardTransactionsumary";
            return View(model);

        }
        [HttpPost]
        public ActionResult TopUpSomeoneElseCardUsingSavedCreditDebitCardTransactionsumary([Bind(Include = MFTCCardTransactionSummaryViewModel.BindProperty)]MFTCCardTransactionSummaryViewModel vm)
        {

            bool valid = true;
            Services.SFaxerSignUp faService = new Services.SFaxerSignUp();
            var faInformation = faService.GetInformation(FaxerSession.LoggedUser.UserName);

            if (valid)
            {
                #region  Strip portion
                //StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");

                StripeConfiguration.SetApiKey(System.Configuration.ConfigurationManager.AppSettings["StripeApiLiveKey"]);


                var chargeOptions = new StripeChargeCreateOptions()
                {
                    Amount = (Int32)Common.FaxerSession.SavedCreditDebitCardDetails.TopUpAmount * 100,
                    Currency = Common.FaxerSession.SavedCreditDebitCardDetails.FaxingCurrency,
                    Description = "Charge for " + Common.FaxerSession.SavedCreditDebitCardDetails.NameOnCard,
                    SourceTokenOrExistingSourceId = Common.FaxerSession.SavedCreditDebitCardDetails.StripeTokenID // obtained with Stripe.js
                };
                var chargeService = new StripeChargeService();
                StripeCharge charge = chargeService.Create(chargeOptions);
                #endregion
                //transaction history object
                SFaxingTopUpCardTransaction service = new SFaxingTopUpCardTransaction();
                string ReceiptNumber = service.GetNewMFTCCardTopUpReceipt();

                STopUpSomeoneElseCard TopUpSomeoneElseServices = new STopUpSomeoneElseCard();

                DB.TopUpSomeoneElseCardTransaction obj = new DB.TopUpSomeoneElseCardTransaction()
                {
                    KiiPayPersonalWalletId = int.Parse(FaxerSession.TopUpCardId),
                    FaxerId = FaxerSession.LoggedUser.Id,
                    FaxingAmount = FaxerSession.FaxingAmountSummary.FaxingAmount,
                    RecievingAmount = FaxerSession.FaxingAmountSummary.ReceivingAmount,
                    ExchangeRate = FaxerSession.FaxingAmountSummary.ExchangeRate,
                    FaxingFee = FaxerSession.FaxingAmountSummary.FaxingFee,
                    ReceiptNumber = ReceiptNumber,
                    TransactionDate = System.DateTime.Now,
                    TopUpReference = Common.FaxerSession.PaymentRefrence,
                    PaymentMethod = "PM002",
                };
                obj = TopUpSomeoneElseServices.SaveTransaction(obj);


                //End Top Up Email 
                MailCommon mail = new MailCommon();

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                string body = "";

                string CardUserCountry = service.GetCardUserCountry(obj.KiiPayPersonalWalletId);

                var MFTCCardDetails = service.GetMFTCCardInformation(obj.KiiPayPersonalWalletId);
                string SetAutoTopUp = baseUrl + "/FaxerAutoPayments/AddSomeoneElseMoneyFaxCardAutoTopUp?MFTCCardid=" + obj.KiiPayPersonalWalletId;
                string TopUpMoneyfaxCard = baseUrl + "/TopUpSomeoneElseMFTCCard/MFTCCardAccountNo?MFTCCardNO=" + MFTCCardDetails.MobileNo.Decrypt();
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardTopUp?FaxerName=" + FaxerName +
                    "&CardUserCountry=" + CardUserCountry + "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard + "&SetAutoTopUp=" + SetAutoTopUp);

                string FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
                var CardUserCurrency = Common.Common.GetCountryCurrency(MFTCCardDetails.CardUserCountry);
                string ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardTopUpReceipt?ReceiptNumber=" + obj.ReceiptNumber + "&Date=" +
                    obj.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + obj.TransactionDate.ToString("HH:mm")
                    + "&FaxerFullName=" + FaxerName + "&MFTCCardNumber=" + MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo()
                    + "&CardUserFullName=" + MFTCCardDetails.FirstName + " " + MFTCCardDetails.MiddleName + " " + MFTCCardDetails.LastName
                    + "&AmountTopUp=" + obj.FaxingAmount + " " + FaxerCurrency + "&ExchangeRate=" + obj.ExchangeRate +
                    "&AmountInLocalCurrency=" + obj.RecievingAmount + " " + CardUserCurrency + "&Fee=" + obj.FaxingFee + " " + FaxerCurrency + "&BalanceOnCard=" + MFTCCardDetails.CurrentBalance + " " + CardUserCurrency;
                var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);

                mail.SendMail(FaxerEmail, "Confirmation of Virtual Account Payment", body, ReceiptPDF);
                //mail.SendMail(FaxerEmail, "Confirmation of Virtual Account Payment", body);

                // End 


                #region Virtual Account Deposit Sms


                SmsApi smsApiServices = new SmsApi();

                string receivingAmount = Common.Common.GetCurrencySymbol(MFTCCardDetails.CardUserCountry) + obj.RecievingAmount;
                string message = smsApiServices.GetVirtualAccountDepositMessage(FaxerName, MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo(),
                                                                  Common.Common.GetCurrencySymbol(faInformation.Country) + obj.FaxingAmount, receivingAmount);

                string PhoneNo = Common.Common.GetCountryPhoneCode(faInformation.Country) + "" + faInformation.PhoneNumber;
                smsApiServices.SendSMS(PhoneNo, message);

                string receiverPhoneNo = Common.Common.GetCountryPhoneCode(MFTCCardDetails.CardUserCountry) + "" + MFTCCardDetails.CardUserTel;
                smsApiServices.SendSMS(receiverPhoneNo, message);
                #endregion


                if (obj != null)
                {
                    DB.CardTopUpCreditDebitInformation cardDetails_TopUp = new DB.CardTopUpCreditDebitInformation()
                    {
                        TopUpSomeoneElseTransactionId = obj.Id,
                        NameOnCard = Common.FaxerSession.SavedCreditDebitCardDetails.NameOnCard,
                        ExpiryDate = Common.FaxerSession.SavedCreditDebitCardDetails.EndMonth.Substring(0, 2) + "/" + Common.FaxerSession.SavedCreditDebitCardDetails.EndYear.Substring(0, 2),
                        IsSavedCard = true,
                        CardNumber = "xxxx-xxxx-xxxx-" + Common.FaxerSession.SavedCreditDebitCardDetails.CardNumber.Right(4),
                        AutoRecharged = Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUp,
                    };
                    SSavedCard cardInformationservices = new SSavedCard();
                    cardDetails_TopUp = cardInformationservices.Save(cardDetails_TopUp);



                }

                if ((Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUp == true)
             && Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUpAmount > 0 && (int)Common.FaxerSession.SavedCreditDebitCardDetails.AutoPaymentFrequency > 0)
                {
                    var cardDetails = context.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                    if (cardDetails != null)
                    {
                        var data = context.OtherMFTCCardAutoTopUpInformation.Where(x => x.MFTCCardId == obj.KiiPayPersonalWalletId && x.FaxerId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();


                        if (data != null)
                        {

                            data.AutoPaymentAmount = Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUpAmount;
                            data.AutoPaymentFrequency = Common.FaxerSession.SavedCreditDebitCardDetails.AutoPaymentFrequency;
                            data.EnableAutoPayment = true;
                            data.FrequencyDetails = Common.FaxerSession.SavedCreditDebitCardDetails.PaymentDay;
                            data.TopUpReference = Common.FaxerSession.PaymentRefrence;
                            context.Entry(data).State = EntityState.Modified;
                            context.SaveChanges();

                        }
                        else
                        {
                            OtherMFTCCardAutoTopUpInformation autoTopUpInformation = new OtherMFTCCardAutoTopUpInformation()
                            {

                                MFTCCardId = obj.KiiPayPersonalWalletId,
                                AutoPaymentAmount = Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUpAmount,
                                AutoPaymentFrequency = Common.FaxerSession.SavedCreditDebitCardDetails.AutoPaymentFrequency,
                                EnableAutoPayment = true,
                                FaxerId = Common.FaxerSession.LoggedUser.Id,
                                TopUpReference = Common.FaxerSession.PaymentRefrence,
                                FrequencyDetails = Common.FaxerSession.SavedCreditDebitCardDetails.PaymentDay

                            };
                            context.OtherMFTCCardAutoTopUpInformation.Add(autoTopUpInformation);
                            context.SaveChanges();

                        }


                        string FaxerCountry = context.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).Select(x => x.Country).FirstOrDefault();
                        string CountryCurrency = Common.Common.GetCountryCurrency(FaxerCountry);
                        var MFTCCardDetials = context.KiiPayPersonalWalletInformation.Where(x => x.Id == obj.KiiPayPersonalWalletId).FirstOrDefault();
                        string CardNumber = "xxxx-xxxx-xxxx-" + cardDetails.Num.Decrypt().Right(4);
                        string SetAutoPaymentLink = baseUrl + "/FaxerAutoPayments/AddSomeoneElseMoneyFaxCardAutoTopUp?MFTCCardid=" + obj.KiiPayPersonalWalletId;
                        string TopUCard = baseUrl + "/TopUpSomeoneElseMFTCCard/MFTCCardAccountNo?MFTCCardNO=" + MFTCCardDetials.MobileNo.Decrypt();
                        string AutoTOpUP_body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfAutoPaymentSetupSomeoneElseMFTCCard/?FaxerName=" +
                            FaxerName + "&AutoPaymentAmount=" + Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUpAmount + "&CountryCurrencySymbol=" + CountryCurrency
                            + "&CardUserName=" + MFTCCardDetials.FirstName + " " + MFTCCardDetials.MiddleName + " " + MFTCCardDetials.LastName
                            + "&AutoPaymentFrequency=" + Common.FaxerSession.SavedCreditDebitCardDetails.AutoPaymentFrequency
                            + "&CreditORDebitCardlast4digits=" + CardNumber
                            + "&SetAutoPayment=" + SetAutoPaymentLink +
                            "&TopUPMFTCCard=" + TopUCard);
                        mail.SendMail(FaxerEmail, "Confirmation of Auto Payment Setup to someone else's card", AutoTOpUP_body);

                    }
                }

                string MFTCCardNumber = MFTCCardDetails.MobileNo.Decrypt();
                string CardUserName = MFTCCardDetails.FirstName + " " + MFTCCardDetails.MiddleName + " " + MFTCCardDetails.LastName;
                string TopUpAmount = FaxerSession.FaxingAmountSummary.FaxingAmount.ToString() + " " + FaxerCurrency;
                string ReceiveAmount = FaxerSession.FaxingAmountSummary.ReceivingAmount.ToString() + " " + CardUserCurrency;
                string ExchangeRate = FaxerSession.FaxingAmountSummary.ExchangeRate.ToString();
                string CardCountry = Common.Common.GetCountryName(CardUserCountry);
                return RedirectToAction("TopUpSomeoneElseCardUsingSavedCreditDebitHurrayMessage", new { MFTCCardNumber, CardUserName, CardCountry, TopUpAmount, ReceiveAmount, ExchangeRate });

                //return RedirectToAction("TopUpUsingSavedCreditDebitHurrayMessage");
            }

            else
            {
                return View(vm);
            }

        }

        public ActionResult TopUpSomeoneElseCardUsingSavedCreditDebitHurrayMessage(string MFTCCardNumber, string CardUserName, string CardCountry, string TopUpAmount, string ReceiveAmount, string ExchangeRate)
        {
            Session.Remove("TransactionSummaryUrl");
            Session.Remove("SavedCreditDebitCardDetails");
            Session.Remove("FaxingAmountSummary");
            Session.Remove("MFTCCard");
            ViewBag.MFTCCardNumber = MFTCCardNumber.GetVirtualAccountNo();
            ViewBag.CardUserName = CardUserName;
            ViewBag.CardUserCountry = CardCountry;
            ViewBag.TopUpAmount = TopUpAmount;
            ViewBag.ReceiveAmount = ReceiveAmount;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.ReceiveOption = "Virtual Account Withdrawl";
            return View();
        }
        
        [HttpGet]
        public ActionResult TopUpSomeoneElseUsingBankToBankPaymentMethod()
        {

            //Models.BankToBankTransferViewModel model = new Models.BankToBankTransferViewModel();
            //if (Common.FaxerSession.BankToBankTransfer != null)
            //{
            //    model = Common.FaxerSession.BankToBankTransfer;
            //}
            //model.SendingAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount;
            //model.SendingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode);
            //model.SendingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode);

            Models.BankToBankTransferViewModel model = new Models.BankToBankTransferViewModel();

            if (Common.FaxerSession.BankToBankTransfer != null)
            {

                model = Common.FaxerSession.BankToBankTransfer;
            }
            model.SendingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode);
            model.SendingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode);
            model.SendingAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount;
            model.SenderSurname = Common.Common.GetSenderLastName();
            var BankAccountInfo = Common.Common.GetBankAccountInfo(FaxerSession.LoggedUser.CountryCode);
            model.PaymentReference = model.SenderSurname + "-" + model.SendingAmount;


            if (BankAccountInfo == null)
            {

                @TempData["BankHasbeenSetuped"] = 0;
                return RedirectToAction("PaymentMethod");
            }
            if (BankAccountInfo != null)
            {
                model.AccountNumber = BankAccountInfo.AccountNo;
                model.LabelName = BankAccountInfo.LabelName + ":";
                model.LabelValue = BankAccountInfo.LabelValue;

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult TopUpSomeoneElseUsingBankToBankPaymentMethod([Bind(Include = BankToBankTransferViewModel.BindProperty)]BankToBankTransferViewModel model)
        {
            if (ModelState.IsValid)
            {

                //var Country = context.Country.Where(x => x.CountryName.ToLower() == model.Country.ToLower()).FirstOrDefault();
                //if (Country == null)
                //{
                //    ModelState.AddModelError("Country", "Country Name Does not match");
                //    return View(model);
                //}
                //model.Country = Country.CountryCode;
                //var FaxerInformation = context.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                //if (model.Address1.ToLower() != FaxerInformation.Address1.ToLower())
                //{
                //    ModelState.AddModelError("Address1", "Address does not match");
                //    return View(model);

                //}
                //if (model.City.ToLower() != FaxerInformation.City.ToLower())
                //{
                //    ModelState.AddModelError("City", "City does not match");
                //    return View(model);
                //}
                //if (model.PostalCode != FaxerInformation.PostalCode)
                //{
                //    ModelState.AddModelError("PostalCode", "PostalCode does not match");
                //    return View(model);
                //}
                //if (model.Country.ToLower() != FaxerInformation.Country.ToLower())
                //{
                //    ModelState.AddModelError("Country", "country doesnot match");
                //    return View(model);
                //}
                if (model.Confirm == false)
                {
                    ModelState.AddModelError("Confirm", "Please confirm the Payment ");
                    return View(model);

                }
                if (model.Accept == false)
                {
                    ModelState.AddModelError("Accept", "Please accept the terms and condition to make Payment ");
                    return View(model);


                }
                Common.FaxerSession.BankToBankTransfer = model;


                ViewBag.ModelIsValid = 1;
                return View(model);

                //return RedirectToAction("ContactMoneyFex");
            }
            return View(model);

        }

        [HttpGet]
        public ActionResult TopUpSomeoneElseUsingBankToBankDepositConfirmation()
        {

            return View();
        }
        public ActionResult TopUpSomeoneElseUsingBankToBankDepositConfirmation_Yes()
        {
            int MFTCCardId = int.Parse(Common.FaxerSession.TopUpCardId);
            var MFTCCardNumber = context.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault().MobileNo.Decrypt();
            DB.BanktoBankTransferTopUpSomeoneElseCard SomeOneElseMFTCCardTopUp = new DB.BanktoBankTransferTopUpSomeoneElseCard()
            {

                FaxerId = Common.FaxerSession.LoggedUser.Id,
                MFTCCardId = int.Parse(Common.FaxerSession.TopUpCardId),
                MFTCCardNumber = MFTCCardNumber,
                PaymentReference = Common.FaxerSession.BankToBankTransfer.PaymentReference,
                TopUpAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount,
                TransactionDate = DateTime.Now,
                ExchangeRate = Common.FaxerSession.FaxingAmountSummary.ExchangeRate,
                FaxingFee = Common.FaxerSession.FaxingAmountSummary.FaxingFee,
                ReceivingAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount,
                TotalAmountImcludingFee = Common.FaxerSession.FaxingAmountSummary.TotalAmount,
                TopUpReference = Common.FaxerSession.PaymentRefrence

            };
            var obj = context.BanktoBankTransferTopUpSomeoneElseCard.Add(SomeOneElseMFTCCardTopUp);
            context.SaveChanges();


            var FaxerDetails = context.FaxerInformation.Where(x => x.Id == obj.FaxerId).FirstOrDefault();
            var CardUserDetails = context.KiiPayPersonalWalletInformation.Where(x => x.Id == obj.MFTCCardId).FirstOrDefault();

            var FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string body = "";


            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BankToBankMFTCCardTopUp?FaxerName=" + FaxerDetails.FirstName + " " +
                FaxerDetails.MiddleName + " " + FaxerDetails.LastName +
                 "&CardUserName=" + CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName
                 + "&CardNumber=" + CardUserDetails.MobileNo.Decrypt().GetVirtualAccountNo()
                 + "&Amount=" + obj.TopUpAmount + " " + FaxerCurrency + "&PaymentReference=" + obj.TopUpReference + "&BankReference=" + Common.FaxerSession.BankToBankTransfer.PaymentReference);

            //mail.SendMail("anankarki97@gmail.com", "Bank to Bank Payment MFTC Card Top-Up", body);
            mail.SendMail("noncardpayment@moneyfex.com", "Bank to bank transfer", body);

            mail.SendMail(FaxerDetails.Email, "Bank to bank transfer", body);

            //#region Virtual Account Deposit Sms


            //SmsApi smsApiServices = new SmsApi();


            //string SenderName = FaxerDetails.FirstName + " " + FaxerDetails.MiddleName + " " + FaxerDetails.LastName;
            //string message = smsApiServices.GetVirtualAccountDepositMessage(SenderName, CardUserDetails.MFTCCardNumber.Decrypt().GetVirtualAccountNo(),
            //                                                  Common.Common.GetCurrencySymbol(FaxerDetails.Country) + obj.TopUpAmount);

            //string PhoneNo = Common.Common.GetCountryPhoneCode(FaxerDetails.Country) + "" + FaxerDetails.PhoneNumber;
            //smsApiServices.SendSMS(PhoneNo, message);



            //#endregion
            return RedirectToAction("BankToBankPaymentSuccessful");

        }
        public ActionResult TopUpSomeoneElseUsingBankToBankDepositConfirmation_NO()
        {
            return RedirectToAction("TopUpSomeoneElseUsingBankToBankPaymentMethod");
        }

        //[HttpGet]
        //public ActionResult ContactMoneyFex()
        //{
        //    int MFTCCardId = int.Parse(Common.FaxerSession.TopUpCardId);
        //    var MFTCCardNumber = context.MFTCCardInformation.Where(x => x.Id == MFTCCardId).Select(x => x.MFTCCardNumber).FirstOrDefault();


        //    Models.ContactMoneyFexViewModel model = new Models.ContactMoneyFexViewModel();
        //    model.MFTCCardNumber = MFTCCardNumber.Decrypt();
        //    model.MoneyFexEmail = "noncardfex@moneyfex.com";
        //    model.Subject = "Top-Up payment bank transfer";

        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult ContactMoneyFex(Models.ContactMoneyFexViewModel model)
        //{
        //    if (string.IsNullOrEmpty(model.PaymentRefernce))
        //    {
        //        ModelState.AddModelError("PaymentRefernce", "Please enter a payment reference");
        //        return View(model);
        //    }
        //    DB.BanktoBankTransferTopUpSomeoneElseCard SomeOneElseMFTCCardTopUp = new DB.BanktoBankTransferTopUpSomeoneElseCard()
        //    {

        //        FaxerId = Common.FaxerSession.LoggedUser.Id,
        //        MFTCCardId = int.Parse(Common.FaxerSession.TopUpCardId),
        //        MFTCCardNumber = model.MFTCCardNumber.Encrypt(),
        //        PaymentReference = model.PaymentRefernce,
        //        TopUpAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount,
        //        TransactionDate = DateTime.Now,
        //        ExchangeRate = Common.FaxerSession.FaxingAmountSummary.ExchangeRate,
        //        FaxingFee = Common.FaxerSession.FaxingAmountSummary.FaxingFee,
        //        ReceivingAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount,
        //        TotalAmountImcludingFee = Common.FaxerSession.FaxingAmountSummary.TotalAmount,
        //        TopUpReference = Common.FaxerSession.PaymentRefrence

        //    };
        //    var obj = context.BanktoBankTransferTopUpSomeoneElseCard.Add(SomeOneElseMFTCCardTopUp);
        //    context.SaveChanges();


        //    var FaxerDetails = context.FaxerInformation.Where(x => x.Id == obj.FaxerId).FirstOrDefault();
        //    var CardUserDetails = context.MFTCCardInformation.Where(x => x.Id == obj.MFTCCardId).FirstOrDefault();

        //    var FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
        //    MailCommon mail = new MailCommon();
        //    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

        //    string body = "";


        //    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BankToBankMFTCCardTopUp?FaxerName=" + FaxerDetails.FirstName + " " +
        //        FaxerDetails.MiddleName + " " + FaxerDetails.LastName +
        //         "&CardUserName=" + CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName
        //         + "&CardNumber=" + CardUserDetails.MFTCCardNumber.Decrypt()
        //         + "&Amount=" + obj.TopUpAmount + " " + FaxerCurrency + "&BankReference=" + obj.PaymentReference);

        //    //mail.SendMail("noncardfex@moneyfex.com", "Bank to Bank Payment MFTC Card Top-Up", body);
        //    mail.SendMail("noncardpayment@moneyfex.com", "Bank to bank transfer", body);


        //    return RedirectToAction("BankToBankPaymentSuccessful");
        //}

        public ActionResult BankToBankPaymentSuccessful()
        {

            Session.Remove("BankToBankTransfer");
            return View();

        }


        public ActionResult GetTopUpSomeoneElseCardPaymentsDetails(string FromDate, string ToDate)
        {
            STopUpSomeoneElseCard topUpSomeoneElseCardServices = new STopUpSomeoneElseCard();


            var result = topUpSomeoneElseCardServices.GetPaymentDetails(FromDate, ToDate);

            return View(result);

        }

        public void PrintReceipt(int TransactionId)
        {


            //var obj = new Models.TopUpSomeoneElseCardReceiptViewModel();
            var obj = context.TopUpSomeoneElseCardTransaction.Where(x => x.Id == TransactionId).FirstOrDefault();


            var Faxerdetials = context.FaxerInformation.Where(x => x.Id == obj.FaxerId).FirstOrDefault();

            string FaxerCurrency = Common.Common.GetCountryCurrency(Faxerdetials.Country);
            string CardUserCurrency = Common.Common.GetCountryCurrency(obj.KiiPayPersonalWalletInformation.CardUserCountry);

            string FaxerName = Faxerdetials.FirstName + " " + Faxerdetials.MiddleName + " " + Faxerdetials.LastName;

            MailCommon mail = new MailCommon();

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardTopUpReceipt?ReceiptNumber=" + obj.ReceiptNumber + "&Date=" +
                   obj.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + obj.TransactionDate.ToString("HH:mm")
                   + "&FaxerFullName=" + FaxerName + "&MFTCCardNumber=" + obj.KiiPayPersonalWalletInformation.MobileNo.Decrypt().GetVirtualAccountNo()
                   + "&CardUserFullName=" + obj.KiiPayPersonalWalletInformation.FirstName + " " + obj.KiiPayPersonalWalletInformation.MiddleName + " " + obj.KiiPayPersonalWalletInformation.LastName
                   + "&AmountTopUp=" + obj.FaxingAmount + " " + FaxerCurrency + "&ExchangeRate=" + obj.ExchangeRate +
                   "&AmountInLocalCurrency=" + obj.RecievingAmount + " " + CardUserCurrency + "&Fee=" + obj.FaxingFee + " " + FaxerCurrency + "&BalanceOnCard=" + obj.KiiPayPersonalWalletInformation.CurrentBalance + " " + CardUserCurrency
                   + "&TopupReference=" + obj.TopUpReference;
            var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);

            byte[] bytes = ReceiptPDF.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();

        }


    }
}