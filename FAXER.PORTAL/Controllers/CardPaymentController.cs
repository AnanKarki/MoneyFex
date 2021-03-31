using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class CardPaymentController : Controller
    {
        DB.FAXEREntities db = new DB.FAXEREntities();
        CommonServices CommonService = new CommonServices();
        // GET: CardPayment
        public ActionResult Index()
        {

            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");
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

            var CardCount = db.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).Count();
            ViewBag.CardCount = CardCount;

            #region Auto Topup has been setuped or not  
            int MFTCCardId = int.Parse(FaxerSession.TopUpCardId);
            var AutoTopUpHasbeenSetuped = db.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
            if (AutoTopUpHasbeenSetuped.AutoTopUp == true)
            {

                ViewBag.AutoTopup = 1;
            }
            else
            {
                ViewBag.AutoTopup = 0;

            }
            #endregion
            return View(model);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = CreditDebitCardViewModel.BindProperty)] Models.CreditDebitCardViewModel model)
        {


            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");

            #region Auto Topup has been setuped or not  
            int MFTCCardId = int.Parse(FaxerSession.TopUpCardId);
            var AutoTopUpHasbeenSetuped = db.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
            if (AutoTopUpHasbeenSetuped.AutoTopUp == true)
            {

                ViewBag.AutoTopup = 1;
            }
            else
            {
                ViewBag.AutoTopup = 0;

            }
            #endregion
            bool valid = true;

            var CardCount = db.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).Count();
            ViewBag.CardCount = CardCount;
            //string CountryCode = db.Country.Where(x => x.CountryName.ToLower() == model.CountyName.ToLower()).FirstOrDefault().CountryCode;
            //var Country = db.Country.Where(x => x.CountryName.ToLower() == model.CountyName.ToLower()).FirstOrDefault();
            //if (Country == null)
            //{
            //    ModelState.AddModelError("CountyName", "Country Name Is Not Matched!!");
            //    return View(model);
            //}
            string CountryCode = model.CountyName;
            model.CountyName = CountryCode;
            //model.FaxingCurrency = "USD"; // This is HardCode

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

            if (string.IsNullOrEmpty(model.AddressLineOne))
            {

                ModelState.AddModelError("AddressLineOne", "This field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.CityName))
            {

                ModelState.AddModelError("CityName", "This field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.CountyName))
            {

                ModelState.AddModelError("CountyName", "This field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.ZipCode))
            {

                ModelState.AddModelError("ZipCode", "This field is required");
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
            if (model.CountyName.Trim().ToLower() != faInformation.Country.Trim().ToLower())
            {
                ModelState.AddModelError("CountyName", "County Name Is Not Matched!!");
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
            if (model.ZipCode.ToLower() != faInformation.PostalCode.ToLower())
            {
                ModelState.AddModelError("ZipCode", "Zip/Post Code does not match with your registered Zip/Post Code     ");
                valid = false;
            }
            if (model.Confirm == false)
            {
                ModelState.AddModelError("Confirm", "Accept our Terms and Conditions before Continue!!");
                valid = false;
            }
            var SavedCardCount = db.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).Count();

            if ((SavedCardCount == 0) && model.SaveCard == false && model.AutoTopUp == true)
            {

                ModelState.AddModelError("AutoTopUp", "please add credit/debit card to enable Auto Deposit");
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
            //if (ModelState.IsValid)
            //{
            if (valid)
            {
                // Initiate Session Credit/DebitCardDetails 
                Common.FaxerSession.CreditDebitDetails = model;
                // End 

                return RedirectToAction("MFTCCardTransactionSummary");
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult MFTCCardTransactionSummary()
        {


            var FaxerDetails = db.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            var FaxerCountry = Common.Common.GetCountryName(FaxerDetails.Country);
            var faxerPhoneCode = Common.Common.GetCountryPhoneCode(FaxerDetails.Country);
            int CardUserId = int.Parse(FaxerSession.TopUpCardId);
            var CardUserDetails = db.KiiPayPersonalWalletInformation.Where(x => x.Id == CardUserId).FirstOrDefault();
            string FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            string CardUserCurrency = Common.Common.GetCountryCurrency(CardUserDetails.CardUserCountry);
            Models.MFTCCardTransactionSummaryViewModel model = new MFTCCardTransactionSummaryViewModel()
            {
                CardUserId = CardUserId,
                CardUsername = CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName,
                MFTCCardNumber = CardUserDetails.MobileNo.Decrypt().GetVirtualAccountNo(),
                ReceiveOption = "E-Card Withdrawal",
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
            Common.FaxerSession.TransactionSummaryUrl = "/CardPayment/MFTCCardTransactionSummary";
            return View(model);


        }
        public ActionResult MFTCCardTransactionSummary([Bind(Include = MFTCCardTransactionSummaryViewModel.BindProperty)]MFTCCardTransactionSummaryViewModel vm)
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
                string ReceiptNumber = service.GetNewMFTCCardTopUpReceipt();
                //transaction history object
                DB.SenderKiiPayPersonalWalletPayment obj = new DB.SenderKiiPayPersonalWalletPayment()
                {
                    KiiPayPersonalWalletInformationId = int.Parse(FaxerSession.TopUpCardId),
                    FaxerId = FaxerSession.LoggedUser.Id,
                    FaxingAmount = FaxerSession.FaxingAmountSummary.FaxingAmount,
                    RecievingAmount = FaxerSession.FaxingAmountSummary.ReceivingAmount,
                    ExchangeRate = FaxerSession.FaxingAmountSummary.ExchangeRate,
                    FaxingFee = FaxerSession.FaxingAmountSummary.FaxingFee,
                    ReceiptNumber = ReceiptNumber,
                    TransactionDate = System.DateTime.Now,
                    PaymentMethod = "PM001"
                };
                obj = service.SaveTransaction(obj);

                /// Send Email 
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                string body = "";

                string CardUserCountry = service.GetCardUserCountry(obj.KiiPayPersonalWalletInformationId);
                string TopUpMoneyfaxCard = baseUrl + "/StartFaxingMoney/Index?mFTCCardInformationId=" + obj.KiiPayPersonalWalletInformationId;
                string SetAutoTopUp = baseUrl + "/FaxerAutoPayments/MoneyFaxCardAutoTopUp?mftcCardInformationId=" + obj.KiiPayPersonalWalletInformationId;
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardTopUp?FaxerName=" + FaxerName +
                    "&CardUserCountry=" + CardUserCountry + "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard + "&SetAutoTopUp=" + SetAutoTopUp);


                var MFTCCardDetails = service.GetMFTCCardInformation(obj.KiiPayPersonalWalletInformationId);
                string FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
                string CardUserCurrency = Common.Common.GetCountryCurrency(MFTCCardDetails.CardUserCountry);
                string ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardTopUpReceipt?ReceiptNumber=" + obj.ReceiptNumber + "&Date=" +
                    obj.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + obj.TransactionDate.ToString("HH:mm")
                    + "&FaxerFullName=" + FaxerName + "&MFTCCardNumber=" + MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo()
                    + "&CardUserFullName=" + MFTCCardDetails.FirstName + " " + MFTCCardDetails.MiddleName + " " + MFTCCardDetails.LastName
                    + "&AmountTopUp=" + obj.FaxingAmount + " " + FaxerCurrency + "&ExchangeRate=" + obj.ExchangeRate +
                    "&AmountInLocalCurrency=" + obj.RecievingAmount + " " + CardUserCurrency + "&Fee=" + obj.FaxingFee + " " + FaxerCurrency + "&BalanceOnCard=" + MFTCCardDetails.CurrentBalance + " " + CardUserCurrency;
                var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);

                mail.SendMail(FaxerEmail, "Confirmation of Virtual Account Deposit", body, ReceiptPDF);
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
                    int SavedCardCount = db.SavedCard.Where(x => x.UserId == FaxerSession.LoggedUser.Id).Count();
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
                    var SavedCardCount = db.SavedCard.Where(x => x.Num == cardNumber).FirstOrDefault();
                    if (SavedCardCount != null)
                    {
                        IsSaved = true;
                    }
                    DB.CardTopUpCreditDebitInformation cardDetails = new DB.CardTopUpCreditDebitInformation()
                    {
                        CardTransactionId = obj.Id,
                        NameOnCard = Common.FaxerSession.CreditDebitDetails.NameOnCard,
                        ExpiryDate = Common.FaxerSession.CreditDebitDetails.EndMM + "/" + Common.FaxerSession.CreditDebitDetails.EndYY,
                        CardNumber = "xxxx-xxxx-xxxx-" + Common.FaxerSession.CreditDebitDetails.CardNumber.Right(4),
                        IsSavedCard = IsSaved,
                        AutoRecharged = Common.FaxerSession.CreditDebitDetails.AutoTopUp,
                    };
                    SSavedCard cardInformationservices = new SSavedCard();
                    cardDetails = cardInformationservices.Save(cardDetails);
                }

                if ((Common.FaxerSession.CreditDebitDetails.AutoTopUp == true) && Common.FaxerSession.CreditDebitDetails.AutoTopUpAmount > 0)
                {
                    var cardDetails = db.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                    if (cardDetails != null)
                    {

                        int MFTCCardId = int.Parse(FaxerSession.TopUpCardId);
                        var MFTCCardAutoTopUP = db.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
                        MFTCCardAutoTopUP.AutoTopUp = true;
                        MFTCCardAutoTopUP.AutoTopUpAmount = Common.FaxerSession.CreditDebitDetails.AutoTopUpAmount;
                        db.Entry(MFTCCardAutoTopUP).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        // Send email for confirmation of Auto Top Up Set Up
                        string bodyAutoTopUp = "";
                        string PayForGoodsAbroad = baseUrl + "/PayForGoodsAndServicesAbroad";
                        bodyAutoTopUp = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfAutoPaymentMFTCTopUpSetupFaxer/?FaxerName=" +
                            FaxerName + "&AutoTopUpAmount=" + MFTCCardAutoTopUP.AutoTopUpAmount + "&CountryCurrencySymbol=" + CardUserCurrency
                            + "&MFTCCardNumber=" + MFTCCardAutoTopUP.MobileNo.Decrypt().GetVirtualAccountNo()
                            + "&CardUserName=" + MFTCCardAutoTopUP.FirstName + " " + MFTCCardAutoTopUP.MiddleName + " " + MFTCCardAutoTopUP.LastName +
                            "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard + "&SetAutoTopUp=" + SetAutoTopUp +
                            "&PayForGoodsAbroad=" + PayForGoodsAbroad);
                        mail.SendMail(FaxerEmail, "Confirmation of Auto Deposit Setup", bodyAutoTopUp);
                        // End 
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

            Session.Remove("TransactionSummaryUrl");
            Session.Remove("CreditDebitDetails");
            Session.Remove("FaxingAmountSummary");
            ViewBag.MFTCCardNumber = MFTCCardNumber.GetVirtualAccountNo();
            ViewBag.CardUserName = CardUserName;
            ViewBag.CardUserCountry = CardCountry;
            ViewBag.TopUpAmount = TopUpAmount;
            ViewBag.BalanceOnCard = BalanceOnCard;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.ReceiveOption = "E-Card Withdrawl";
            return View();
        }

        public ActionResult TopUpUsingSavedCreditDebit(int savedCardId = 0)
        {
            //List<SavedCard> list = db.SavedCard.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id).OrderBy(x => x.CardName).ToList();

            var list = Common.Common.GetSavedCardDetails();
            if (list.Count() == 0)
            {

                @TempData["CardCount"] = list.Count();
                FaxerSession.FromUrl = "/CardPayment/TopUpUsingSavedCreditDebit";
                return RedirectToAction("Index", "PaymentMethod");
            }
            //foreach (var item in list)
            //{
            //    item.Num = Common.Common.Decrypt(item.Num);
            //    item.Num = "**** **** **** " + item.Num.Substring(item.Num.Length - 4, 4);
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
                vm = (from c in db.SavedCard.Where(x => x.Id == savedCardId).ToList()
                      select new PaymentUsingSavedCreditDebitCardVm()
                      {
                          NameOnCard = c.CardName.Decrypt(),
                          CardNumber = c.Num.Decrypt(),
                          CardNumberMasked = "**** **** ****" + c.Num.Decrypt().Substring(c.Num.Decrypt().Length - 4, 4),
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


            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");
            #region Auto Topup has been setuped or not 
            int MFTCCardId = int.Parse(FaxerSession.TopUpCardId);
            var AutoTopUpHasbeenSetuped = db.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
            if (AutoTopUpHasbeenSetuped.AutoTopUp == true)
            {

                ViewBag.AutoTopup = 1;
            }
            else
            {
                ViewBag.AutoTopup = 0;

            }
            #endregion

            return View(vm);
        }

        [HttpPost]
        public ActionResult TopUpUsingSavedCreditDebit([Bind(Include = PaymentUsingSavedCreditDebitCardVm.BindProperty)]PaymentUsingSavedCreditDebitCardVm vm)
        {

            // Countries Drop Down List 

            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");
            #region Auto Topup has been setuped or not  
            int MFTCCardId = int.Parse(FaxerSession.TopUpCardId);
            var AutoTopUpHasbeenSetuped = db.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
            if (AutoTopUpHasbeenSetuped.AutoTopUp == true)
            {

                ViewBag.AutoTopup = 1;
            }
            else
            {
                ViewBag.AutoTopup = 0;

            }
            #endregion

            //List<SavedCard> list = db.SavedCard.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id).OrderBy(x => x.CardName).ToList();

            var list = Common.Common.GetSavedCardDetails();
            //foreach (var item in list)
            //{
            //    item.Num = Common.Common.Decrypt(item.Num);
            //    item.Num = "**** **** **** " +  item.Num.Substring(item.CardName.Length - 4, 4);  
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

                ModelState.AddModelError("Address1", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.City))
            {

                ModelState.AddModelError("City", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.Country))
            {

                ModelState.AddModelError("Country", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.PostalCode))
            {

                ModelState.AddModelError("PostalCode", "The field is required");
                valid = false;
            }
            if (!string.IsNullOrEmpty(vm.Address1) && vm.Address1.Trim().ToLower() != faInformation.Address1.Trim().ToLower())
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

            if (!string.IsNullOrEmpty(vm.PostalCode) && vm.PostalCode.Trim() != faInformation.PostalCode.Trim())
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

                return RedirectToAction("MFTCCardTopUpUsingSavedCreditDebitTransactionsumary");
            }

            else
            {
                return View(vm);
            }

        }

        [HttpGet]
        public ActionResult MFTCCardTopUpUsingSavedCreditDebitTransactionsumary()
        {

            var FaxerDetails = db.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            var FaxerCountry = Common.Common.GetCountryName(FaxerDetails.Country);
            var faxerPhoneCode = Common.Common.GetCountryPhoneCode(FaxerDetails.Country);
            int CardUserId = int.Parse(FaxerSession.TopUpCardId);
            var CardUserDetails = db.KiiPayPersonalWalletInformation.Where(x => x.Id == CardUserId).FirstOrDefault();
            string FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            string FaxerCurrencySymbol = Common.Common.GetCurrencySymbol(FaxerDetails.Country);
            string CardUserCurrency = Common.Common.GetCountryCurrency(CardUserDetails.CardUserCountry);
            string CardUserCurrencySymbol = Common.Common.GetCurrencySymbol(CardUserDetails.CardUserCountry);
            Models.MFTCCardTransactionSummaryViewModel model = new MFTCCardTransactionSummaryViewModel()
            {
                CardUserId = CardUserId,
                CardUsername = CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName,
                MFTCCardNumber = CardUserDetails.MobileNo.Decrypt().GetVirtualAccountNo(),
                ReceiveOption = "E-Card Withdrawal",
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

            Common.FaxerSession.TransactionSummaryUrl = "/CardPayment/MFTCCardTopUpUsingSavedCreditDebitTransactionsumary";
            return View(model);

        }
        [HttpPost]
        public ActionResult MFTCCardTopUpUsingSavedCreditDebitTransactionsumary([Bind(Include = MFTCCardTransactionSummaryViewModel.BindProperty)]MFTCCardTransactionSummaryViewModel vm)
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

                DB.SenderKiiPayPersonalWalletPayment obj = new DB.SenderKiiPayPersonalWalletPayment()
                {
                    KiiPayPersonalWalletInformationId = int.Parse(FaxerSession.TopUpCardId),
                    FaxerId = FaxerSession.LoggedUser.Id,
                    FaxingAmount = FaxerSession.FaxingAmountSummary.FaxingAmount,
                    RecievingAmount = FaxerSession.FaxingAmountSummary.ReceivingAmount,
                    ReceiptNumber = ReceiptNumber,
                    ExchangeRate = FaxerSession.FaxingAmountSummary.ExchangeRate,
                    FaxingFee = FaxerSession.FaxingAmountSummary.FaxingFee,
                    TransactionDate = System.DateTime.Now,
                };

                obj = service.SaveTransaction(obj);


                //End Top Up Email 
                MailCommon mail = new MailCommon();

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                string body = "";

                string CardUserCountry = service.GetCardUserCountry(obj.KiiPayPersonalWalletInformationId);
                string CardUserCountryName = Common.Common.GetCountryName(CardUserCountry);
                string TopUpMoneyfaxCard = baseUrl + "/StartFaxingMoney/Index?mFTCCardInformationId=" + obj.KiiPayPersonalWalletInformationId;
                string SetAutoTopUp = baseUrl + "/FaxerAutoPayments/MoneyFaxCardAutoTopUp?mftcCardInformationId=" + obj.KiiPayPersonalWalletInformationId;
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardTopUp?FaxerName=" + FaxerName +
                    "&CardUserCountry=" + CardUserCountryName + "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard + "&SetAutoTopUp=" + SetAutoTopUp);

                var MFTCCardDetails = service.GetMFTCCardInformation(obj.KiiPayPersonalWalletInformationId);
                string FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
                var CardUserCurrency = Common.Common.GetCountryCurrency(MFTCCardDetails.CardUserCountry);
                string ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardTopUpReceipt?ReceiptNumber=" + obj.ReceiptNumber + "&Date=" +
                    obj.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + obj.TransactionDate.ToString("HH:mm")
                    + "&FaxerFullName=" + FaxerName + "&MFTCCardNumber=" + MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo()
                    + "&CardUserFullName=" + MFTCCardDetails.FirstName + " " + MFTCCardDetails.MiddleName + " " + MFTCCardDetails.LastName
                    + "&AmountTopUp=" + obj.FaxingAmount + " " + FaxerCurrency + "&ExchangeRate=" + obj.ExchangeRate +
                    "&AmountInLocalCurrency=" + obj.RecievingAmount + " " + CardUserCurrency + "&Fee=" + obj.FaxingFee + " " + FaxerCurrency + "&BalanceOnCard=" + MFTCCardDetails.CurrentBalance + " " + CardUserCurrency;
                var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);

                mail.SendMail(FaxerEmail, "Confirmation of Virtual Account Deposit", body, ReceiptPDF);
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
                    DB.CardTopUpCreditDebitInformation cardDetails = new DB.CardTopUpCreditDebitInformation()
                    {
                        CardTransactionId = obj.Id,
                        NameOnCard = Common.FaxerSession.SavedCreditDebitCardDetails.NameOnCard,
                        ExpiryDate = Common.FaxerSession.SavedCreditDebitCardDetails.EndMonth.Substring(0, 2) + "/" + Common.FaxerSession.SavedCreditDebitCardDetails.EndYear.Substring(0, 2),
                        IsSavedCard = true,
                        CardNumber = "xxxx-xxxx-xxxx-" + Common.FaxerSession.SavedCreditDebitCardDetails.CardNumber.Right(4),
                        AutoRecharged = Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUp,
                    };
                    SSavedCard cardInformationservices = new SSavedCard();
                    cardDetails = cardInformationservices.Save(cardDetails);

                }
                if ((Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUp == true) && Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUpAmount > 0)
                {

                    int MFTCCardId = int.Parse(FaxerSession.TopUpCardId);
                    var MFTCCardAutoTopUP = db.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
                    MFTCCardAutoTopUP.AutoTopUp = true;
                    MFTCCardAutoTopUP.AutoTopUpAmount = Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUpAmount;
                    db.Entry(MFTCCardAutoTopUP).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    // Send email for confirmation of Auto Top Up Set Up
                    string bodyAutoTopUp = "";
                    string PayForGoodsAbroad = baseUrl + "/PayForGoodsAndServicesAbroad";
                    bodyAutoTopUp = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfAutoPaymentMFTCTopUpSetupFaxer/?FaxerName=" +
                        FaxerName + "&AutoTopUpAmount=" + MFTCCardAutoTopUP.AutoTopUpAmount + "&CountryCurrencySymbol=" + CardUserCurrency
                        + "&MFTCCardNumber=" + MFTCCardAutoTopUP.MobileNo.Decrypt().GetVirtualAccountNo()
                        + "&CardUserName=" + MFTCCardAutoTopUP.FirstName + " " + MFTCCardAutoTopUP.MiddleName + " " + MFTCCardAutoTopUP.LastName +
                        "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard + "&SetAutoTopUp=" + SetAutoTopUp +
                        "&PayForGoodsAbroad=" + PayForGoodsAbroad);
                    mail.SendMail(FaxerEmail, "Confirmation of Auto Deposit Setup", bodyAutoTopUp);
                    // End 

                }
                string MFTCCardNumber = MFTCCardDetails.MobileNo.Decrypt();
                string CardUserName = MFTCCardDetails.FirstName + " " + MFTCCardDetails.MiddleName + " " + MFTCCardDetails.LastName;
                string TopUpAmount = FaxerSession.FaxingAmountSummary.FaxingAmount.ToString() + " " + FaxerCurrency;
                string ReceiveAmount = FaxerSession.FaxingAmountSummary.ReceivingAmount.ToString() + " " + CardUserCurrency;
                string ExchangeRate = FaxerSession.FaxingAmountSummary.ExchangeRate.ToString();
                string CardCountry = Common.Common.GetCountryName(CardUserCountry);
                return RedirectToAction("TopUpUsingSavedCreditDebitHurrayMessage", new { MFTCCardNumber, CardUserName, CardCountry, TopUpAmount, ReceiveAmount, ExchangeRate });

                //return RedirectToAction("TopUpUsingSavedCreditDebitHurrayMessage");
            }

            else
            {
                return View(vm);
            }

        }

        public ActionResult TopUpUsingSavedCreditDebitHurrayMessage(string MFTCCardNumber, string CardUserName, string CardCountry, string TopUpAmount, string ReceiveAmount, string ExchangeRate)
        {
            Session.Remove("TransactionSummaryUrl");
            Session.Remove("SavedCreditDebitCardDetails");
            Session.Remove("FaxingAmountSummary");
            ViewBag.MFTCCardNumber = MFTCCardNumber.GetVirtualAccountNo();
            ViewBag.CardUserName = CardUserName;
            ViewBag.CardUserCountry = CardCountry;
            ViewBag.TopUpAmount = TopUpAmount;
            ViewBag.ReceiveAmount = ReceiveAmount;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.ReceiveOption = "E-Card Withdrawl";
            return View();
        }


        public ActionResult Receiver()
        {

            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");
            Models.CreditDebitCardViewModel model = new Models.CreditDebitCardViewModel()
            {
                FaxingAmount = FaxerSession.FaxingAmountSummary.TotalAmount,
                FaxingCurrency = FaxerSession.FaxingAmountSummary.FaxingCurrency,
                FaxingCurrencySymbol = FaxerSession.FaxingAmountSummary.FaxingCurrencySymbol
            };
            if (Common.FaxerSession.CreditDebitDetails != null)
            {

                model = Common.FaxerSession.CreditDebitDetails;
                return View(model);
            }

            return View(model);

        }
        [HttpPost]
        public ActionResult Receiver([Bind(Include = CreditDebitCardViewModel.BindProperty)]Models.CreditDebitCardViewModel model)
        {

            // Countries Drop Down List 

            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");


            bool valid = true;
            Services.SFaxerSignUp faService = new Services.SFaxerSignUp();
            var faInformation = faService.GetInformation(FaxerSession.LoggedUser.UserName);
            //var Country = db.Country.Where(x => x.CountryName.ToLower() == model.CountyName.ToLower()).FirstOrDefault();
            //if (Country == null)
            //{
            //    ModelState.AddModelError("CountyName", "County Name Is Not Matched!!");
            //    return View(model);
            //}
            string CountryCode = model.CountyName;
            model.CountyName = CountryCode;
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
            if (string.IsNullOrEmpty(model.SecurityCode))
            {
                ModelState.AddModelError("SecurityCode", "SecurityCode is required");
                valid = false;
            }
            if (model.AddressLineOne.Trim().ToLower() != faInformation.Address1.Trim().ToLower())
            {
                ModelState.AddModelError("AddressLineOne", "Address line one does not match with your registered address,");
                valid = false;
            }

            if (model.CityName.Trim().ToLower() != faInformation.City.Trim().ToLower())
            {
                ModelState.AddModelError("CityName", "City Name Is Not Matched!!");
                valid = false;
            }
            if (model.CountyName.Trim().ToLower() != faInformation.Country.Trim().ToLower())
            {
                ModelState.AddModelError("CountyName", "County Name Is Not Matched!!");
                valid = false;
            }

            if (model.ZipCode.Trim().ToLower() != faInformation.PostalCode.Trim().ToLower())
            {
                ModelState.AddModelError("ZipCode", "Zip/Post Code does not match with your registered Zip/Post Code     ");
                valid = false;
            }
            if (model.Confirm == false)
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
                    Number = model.CardNumber,
                    ExpirationMonth = int.Parse(model.EndMM),
                    ExpirationYear = int.Parse(model.EndYY),
                    Cvc = model.SecurityCode,
                    Name = model.NameOnCard
                }
            };

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
            //if (ModelState.IsValid)
            //{
            if (valid)
            {
                //Initial Credit/Debit Card Detials  Session 

                Common.FaxerSession.CreditDebitDetails = model;

                // End 
                return RedirectToAction("ReceiverTransactionsummary");
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult ReceiverTransactionsummary()
        {


            var FaxerDetails = db.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            var FaxerCountry = Common.Common.GetCountryName(FaxerDetails.Country);
            var faxerCountryCode = Common.Common.GetCountryPhoneCode(FaxerDetails.Country);
            string FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            string ReceiverCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.ReceivingCountry);

            Models.NonCardTransactionSummaryViewModel model = new NonCardTransactionSummaryViewModel()
            {
                ReceiverName = Common.FaxerSession.ReceiversDetails.ReceiverFirstName + " " + Common.FaxerSession.ReceiversDetails.ReceiverMiddleName + " " + Common.FaxerSession.ReceiversDetails.ReceiverLastName,
                ReceiveOption = "Cash Collection",
                FaxerName = Common.FaxerSession.LoggedUser.FullName,
                FaxerEmail = Common.FaxerSession.LoggedUser.UserName,
                FaxerPhoneNumber = faxerCountryCode + " " + FaxerDetails.PhoneNumber,
                CardNumber = Common.FaxerSession.CreditDebitDetails.CardNumber.FormatSavedCardNumber(),
                CardExpriyDate = Common.FaxerSession.CreditDebitDetails.EndMM + "/" + Common.FaxerSession.CreditDebitDetails.EndYY,
                CountryOfBirth = FaxerCountry,
                streetAddress = Common.FaxerSession.CreditDebitDetails.AddressLineOne,
                City = Common.FaxerSession.CreditDebitDetails.CityName,
                PostalCode = Common.FaxerSession.CreditDebitDetails.ZipCode,
                SentAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount.ToString() + " " + FaxerCurrency,
                Fees = Common.FaxerSession.FaxingAmountSummary.FaxingFee.ToString() + " " + FaxerCurrency,
                TotalAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount.ToString() + " " + FaxerCurrency,
                TotalReceiveAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount.ToString() + " " + ReceiverCurrency

            };

            Common.FaxerSession.TransactionSummaryUrl = "/CardPayment/ReceiverTransactionsummary";
            return View(model);

        }

        [HttpPost]
        public ActionResult ReceiverTransactionsummary([Bind(Include = NonCardTransactionSummaryViewModel.BindProperty)]NonCardTransactionSummaryViewModel vm)
        {
            //model.FaxingCurrency = "USD"; // This is HardCode
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
                //transaction history object
                Services.SReceiverDetails receiverService = new SReceiverDetails();
                DB.ReceiversDetails recDetailObj = new DB.ReceiversDetails()
                {
                    City = FaxerSession.ReceiversDetails.ReceiverCity,
                    CreatedDate = System.DateTime.Now,
                    Country = FaxerSession.ReceivingCountry,
                    EmailAddress = FaxerSession.ReceiversDetails.ReceiverEmailAddress,
                    FaxerID = FaxerSession.LoggedUser.Id,
                    FirstName = FaxerSession.ReceiversDetails.ReceiverFirstName,
                    IsDeleted = false,
                    LastName = FaxerSession.ReceiversDetails.ReceiverLastName,
                    MiddleName = FaxerSession.ReceiversDetails.ReceiverMiddleName == null ? "" : FaxerSession.ReceiversDetails.ReceiverMiddleName,
                    PhoneNumber = FaxerSession.ReceiversDetails.ReceiverPhoneNumber
                };
                receiverService.Add(recDetailObj);

                // Add City in City Table 
                City newCity = new City()
                {

                    CountryCode = recDetailObj.Country,
                    Module = Module.Faxer,
                    Name = recDetailObj.City
                };
                SCity.Save(newCity);
                //End 
                SFaxingNonCardTransaction service = new SFaxingNonCardTransaction();
                //get unique new mfcn
                string MFCN = service.GetNewMFCNToSave();
                var receiptNumber = service.GetNewReceiptNumberToSave();


                DB.FaxingNonCardTransaction obj = new DB.FaxingNonCardTransaction()
                {
                    stripe_ChargeId = charge.Id,
                    NonCardRecieverId = int.Parse(recDetailObj.Id.ToString()),
                    UserId = 0,
                    FaxingStatus = FaxingStatus.NotReceived,
                    MFCN = MFCN,
                    ReceiptNumber = receiptNumber,
                    FaxingMethod = "PM001",
                    FaxingAmount = FaxerSession.FaxingAmountSummary.FaxingAmount,
                    ReceivingAmount = FaxerSession.FaxingAmountSummary.ReceivingAmount,
                    ExchangeRate = FaxerSession.FaxingAmountSummary.ExchangeRate,
                    FaxingFee = FaxerSession.FaxingAmountSummary.FaxingFee,
                    TransactionDate = System.DateTime.Now,
                };
                //save transaction for non card
                obj = service.SaveTransaction(obj);

                //End Top Up Email 
                MailCommon mail = new MailCommon();

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                string FaxerCountry = Common.Common.GetCountryName(Common.FaxerSession.LoggedUser.CountryCode);
                string body = "";
                string ReceiverName = recDetailObj.FirstName + " " + recDetailObj.MiddleName + " " + recDetailObj.LastName;
                string ReceiverCity = recDetailObj.City;
                string FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
                string ReceiverCurrency = Common.Common.GetCountryCurrency(recDetailObj.Country);
                string ReceiverPhonecode = Common.Common.GetCountryPhoneCode(recDetailObj.Country);
                string ReceiverCountry = Common.Common.GetCountryName(recDetailObj.Country);
                string RegisterMFTCLink = baseUrl + "/TopUpRegestration/UserContactDetails";
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyFaxedWithReceipt?FaxerName=" + FaxerName +
                    "&ReceiverName=" + ReceiverName + "&ReceiverCity=" + ReceiverCity
                    + "&MFCN=" + MFCN + "&FaxAmount=" + obj.FaxingAmount + " " + FaxerCurrency + "&RegisterMFTC=" + RegisterMFTCLink + "&FaxerCountry=" + FaxerCountry);


                string URL = baseUrl + "/EmailTemplate/NonCardUserReceiver?MFReceiptNumber=" + obj.ReceiptNumber +
                                  "&TransactionDate=" + obj.TransactionDate.ToString("dd/MM/yyy") + "&TransactionTime=" + obj.TransactionDate.ToString("HH:mm")
                                    + "&FaxerFullName=" + faInformation.FirstName + " " + faInformation.MiddleName + " " + faInformation.LastName +
                                  "&MFCN=" + obj.MFCN + "&ReceiverFullName=" + recDetailObj.FirstName + " " + recDetailObj.MiddleName + " " + recDetailObj.LastName
                                  + "&Telephone=" + ReceiverPhonecode + " " + recDetailObj.PhoneNumber + "&AgentName=" + "" + "&AgentCode=" + "" +
                                  "&AmountSent=" + obj.FaxingAmount
                                  + "&ExchangeRate=" + obj.ExchangeRate + "&Fee=" + obj.FaxingFee +
                                  "&AmountReceived=" + obj.ReceivingAmount + "&SendingCurrency=" + FaxerCurrency + "&ReceivingCurrency=" + ReceiverCurrency;

                var Receipt = Common.Common.GetPdf(URL);


                mail.SendMail(FaxerEmail, "Confirmation of Money Faxed with MFCN", body, Receipt);

                string EamilToReceiver_body = "";
                EamilToReceiver_body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/NonCardMoneyTransferConfirmationToReceiver?ReceiverName="
                    + ReceiverName + "&ReceiverCountry=" + ReceiverCountry +
                    "&SenderCountry=" + FaxerCountry);


                mail.SendMail(recDetailObj.EmailAddress, "Money Transfer Confirmation", EamilToReceiver_body);

                // End 

                #region Cash To Cash Transfer SMS


                SmsApi smsApiServices = new SmsApi();
                string receivingAmount = Common.Common.GetCurrencySymbol(recDetailObj.Country) + obj.ReceivingAmount;
                string Amount = Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry) + obj.FaxingAmount;
                string Fee = Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry) + obj.FaxingFee;
                string message = smsApiServices.GetCashToCashTransferMessage(FaxerName, MFCN, Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry) + Amount, Fee, receivingAmount);

                string PhoneNo = Common.Common.GetCountryPhoneCode(Common.FaxerSession.FaxerCountry) + "" + faInformation.PhoneNumber;
                smsApiServices.SendSMS(PhoneNo, message);

                string receiverPhoneNo = Common.Common.GetCountryPhoneCode(recDetailObj.Country) + "" + recDetailObj.PhoneNumber;
                smsApiServices.SendSMS(receiverPhoneNo, message);


                #endregion

                if (obj != null)
                {

                    bool IsSaved = false;
                    string cardNumber = Common.FaxerSession.CreditDebitDetails.CardNumber.Encrypt();
                    var SavedCardCount = db.SavedCard.Where(x => x.Num == cardNumber).FirstOrDefault();
                    if (SavedCardCount != null)
                    {
                        IsSaved = true;
                    }
                    DB.CardTopUpCreditDebitInformation cardDetails = new DB.CardTopUpCreditDebitInformation()
                    {
                        NonCardTransactionId = obj.Id,
                        NameOnCard = Common.FaxerSession.CreditDebitDetails.NameOnCard,
                        ExpiryDate = Common.FaxerSession.CreditDebitDetails.EndMM + "/" + Common.FaxerSession.CreditDebitDetails.EndYY,
                        IsSavedCard = IsSaved,
                        CardNumber = "xxxx-xxxx-xxxx-" + Common.FaxerSession.CreditDebitDetails.CardNumber.Right(4),
                        AutoRecharged = Common.FaxerSession.CreditDebitDetails.AutoTopUp,
                    };
                    SSavedCard cardInformationservices = new SSavedCard();
                    cardDetails = cardInformationservices.Save(cardDetails);
                }
                string ReceiverFullName = recDetailObj.FirstName + " " + recDetailObj.MiddleName + " " + recDetailObj.LastName;
                string Country = db.Country.Where(x => x.CountryCode == recDetailObj.Country).Select(x => x.CountryName).FirstOrDefault();
                string FaxingAmount = obj.FaxingAmount + " " + FaxerCurrency;
                string ReceivingAmount = obj.ReceivingAmount + " " + ReceiverCurrency;
                return RedirectToAction("HurrayMessage", new { obj.MFCN, ReceiverFullName, Country, FaxingAmount, ReceivingAmount, obj.ExchangeRate });

            }
            else
            {
                return View(vm);
            }

        }
        [HttpGet]
        public ActionResult ReceiverTransferUsingSavedCreditDebitCard(int savedCardId = 0)
        {


            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");
            //List<SavedCard> list = db.SavedCard.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id).OrderBy(x => x.CardName).ToList();

            var list = Common.Common.GetSavedCardDetails();
            if (list.Count() == 0)
            {
                TempData["CardCount"] = list.Count();

                FaxerSession.FromUrl = "/CardPayment/ReceiverTransferUsingSavedCreditDebitCard";
                return RedirectToAction("ReciverPaymentMethod", "PaymentMethod");
            }
            //foreach (var item in list)
            //{
            //    item.Num = Common.Common.Decrypt(item.Num);
            //    item.Num = "**** **** **** " + item.Num.Substring(item.Num.Length - 4, 4);
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
                vm = (from c in db.SavedCard.Where(x => x.Id == savedCardId).ToList()
                      select new PaymentUsingSavedCreditDebitCardVm()
                      {
                          NameOnCard = c.CardName.Decrypt(),
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
            vm.FaxingCurrency = FaxerSession.FaxingAmountSummary.FaxingCurrency;
            vm.FaxingCurrencySymbol = FaxerSession.FaxingAmountSummary.FaxingCurrencySymbol;


            return View(vm);

        }
        [HttpPost]
        public ActionResult ReceiverTransferUsingSavedCreditDebitCard([Bind(Include = PaymentUsingSavedCreditDebitCardVm.BindProperty)]PaymentUsingSavedCreditDebitCardVm vm)
        {

            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");

            //List<SavedCard> list = db.SavedCard.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id).OrderBy(x => x.CardName).ToList();

            var list = Common.Common.GetSavedCardDetails();
            //foreach (var item in list)
            //{
            //    item.Num = Common.Common.Decrypt(item.Num);
            //    item.Num = "**** **** **** " + item.Num.Substring(item.Num.Length - 4, 4);
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

                ModelState.AddModelError("Address1", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.City))
            {

                ModelState.AddModelError("City", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.Country))
            {

                ModelState.AddModelError("Country", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.PostalCode))
            {

                ModelState.AddModelError("PostalCode", "The field is required");
                valid = false;
            }
            if (!string.IsNullOrEmpty(vm.Address1) && vm.Address1.Trim().ToLower() != faInformation.Address1.Trim().ToLower())
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

            if (!string.IsNullOrEmpty(vm.PostalCode) && vm.PostalCode.Trim() != faInformation.PostalCode.Trim())
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

                return RedirectToAction("ReceiverUsingSavedCreditDebitCardTransactionSummary");
            }

            else
            {
                return View(vm);
            }

        }

        [HttpGet]
        public ActionResult ReceiverUsingSavedCreditDebitCardTransactionSummary()
        {


            var FaxerDetails = db.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            var FaxerCountry = Common.Common.GetCountryName(FaxerDetails.Country);
            var faxerCountryCode = Common.Common.GetCountryPhoneCode(FaxerDetails.Country);
            string FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            string ReceiverCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.ReceivingCountry);

            Models.NonCardTransactionSummaryViewModel model = new NonCardTransactionSummaryViewModel()
            {
                SavedcardId = int.Parse(Common.FaxerSession.SavedCreditDebitCardDetails.SavedCard),
                ReceiverName = Common.FaxerSession.ReceiversDetails.ReceiverFirstName + " " + Common.FaxerSession.ReceiversDetails.ReceiverMiddleName + " " + Common.FaxerSession.ReceiversDetails.ReceiverLastName,
                ReceiveOption = "Cash Collection",
                FaxerName = Common.FaxerSession.LoggedUser.FullName,
                FaxerEmail = Common.FaxerSession.LoggedUser.UserName,
                FaxerPhoneNumber = faxerCountryCode + " " + FaxerDetails.PhoneNumber,
                CardNumber = Common.FaxerSession.SavedCreditDebitCardDetails.CardNumber.FormatSavedCardNumber(),
                CardExpriyDate = Common.FaxerSession.SavedCreditDebitCardDetails.EndMonth + "/" + Common.FaxerSession.SavedCreditDebitCardDetails.EndYear,
                CountryOfBirth = FaxerCountry,
                streetAddress = Common.FaxerSession.SavedCreditDebitCardDetails.Address1,
                City = Common.FaxerSession.SavedCreditDebitCardDetails.City,
                PostalCode = Common.FaxerSession.SavedCreditDebitCardDetails.PostalCode,
                SentAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount.ToString() + " " + FaxerCurrency,
                Fees = Common.FaxerSession.FaxingAmountSummary.FaxingFee.ToString() + " " + FaxerCurrency,
                TotalAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount.ToString() + " " + FaxerCurrency,
                TotalReceiveAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount.ToString() + " " + ReceiverCurrency

            };

            Common.FaxerSession.TransactionSummaryUrl = "/CardPayment/ReceiverUsingSavedCreditDebitCardTransactionSummary";
            return View(model);

        }

        [HttpPost]
        public ActionResult ReceiverUsingSavedCreditDebitCardTransactionSummary([Bind(Include = NonCardTransactionSummaryViewModel.BindProperty)]NonCardTransactionSummaryViewModel vm)
        {

            //model.FaxingCurrency = "USD"; // This is HardCode
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
                    Amount = (Int32)Common.FaxerSession.SavedCreditDebitCardDetails.TopUpAmount * 100,
                    Currency = Common.FaxerSession.SavedCreditDebitCardDetails.FaxingCurrency,
                    Description = "Charge for " + Common.FaxerSession.SavedCreditDebitCardDetails.NameOnCard,
                    SourceTokenOrExistingSourceId = Common.FaxerSession.SavedCreditDebitCardDetails.StripeTokenID  // obtained with Stripe.js
                };
                var chargeService = new StripeChargeService();
                StripeCharge charge = chargeService.Create(chargeOptions);
                #endregion
                //transaction history object
                Services.SReceiverDetails receiverService = new SReceiverDetails();
                DB.ReceiversDetails recDetailObj = new DB.ReceiversDetails()
                {
                    City = FaxerSession.ReceiversDetails.ReceiverCity,
                    CreatedDate = System.DateTime.Now,
                    Country = FaxerSession.ReceivingCountry,
                    EmailAddress = FaxerSession.ReceiversDetails.ReceiverEmailAddress,
                    FaxerID = FaxerSession.LoggedUser.Id,
                    FirstName = FaxerSession.ReceiversDetails.ReceiverFirstName,
                    IsDeleted = false,
                    LastName = FaxerSession.ReceiversDetails.ReceiverLastName,
                    MiddleName = FaxerSession.ReceiversDetails.ReceiverMiddleName == null ? "" : FaxerSession.ReceiversDetails.ReceiverMiddleName,
                    PhoneNumber = FaxerSession.ReceiversDetails.ReceiverPhoneNumber
                };
                receiverService.Add(recDetailObj);

                City newCity = new City()
                {
                    CountryCode = recDetailObj.Country,
                    Module = Module.Faxer,
                    Name = recDetailObj.City
                };
                SCity.Save(newCity);

                SFaxingNonCardTransaction service = new SFaxingNonCardTransaction();
                //get unique new mfcn
                string MFCN = service.GetNewMFCNToSave();
                var receiptNumber = service.GetNewReceiptNumberToSave();


                DB.FaxingNonCardTransaction obj = new DB.FaxingNonCardTransaction()
                {
                    stripe_ChargeId = charge.Id,
                    NonCardRecieverId = int.Parse(recDetailObj.Id.ToString()),
                    UserId = 0,
                    FaxingStatus = FaxingStatus.NotReceived,
                    MFCN = MFCN,
                    ReceiptNumber = receiptNumber,
                    FaxingMethod = "PM002",
                    FaxingAmount = FaxerSession.FaxingAmountSummary.FaxingAmount,
                    ReceivingAmount = FaxerSession.FaxingAmountSummary.ReceivingAmount,
                    ExchangeRate = FaxerSession.FaxingAmountSummary.ExchangeRate,
                    FaxingFee = FaxerSession.FaxingAmountSummary.FaxingFee,
                    TransactionDate = System.DateTime.Now,
                };
                //save transaction for non card
                obj = service.SaveTransaction(obj);

                //End Top Up Email 
                MailCommon mail = new MailCommon();

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                string FaxerCountry = Common.Common.GetCountryName(Common.FaxerSession.FaxerCountry);
                string body = "";
                string ReceiverName = recDetailObj.FirstName + " " + recDetailObj.MiddleName + " " + recDetailObj.LastName;
                string ReceiverCity = recDetailObj.City;
                string FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
                string ReceiverCurrency = Common.Common.GetCountryCurrency(recDetailObj.Country);
                string ReceiverPhonecode = Common.Common.GetCountryPhoneCode(recDetailObj.Country);
                string ReceiverCountry = Common.Common.GetCountryName(recDetailObj.Country);

                string RegisterMFTCLink = baseUrl + "/TopUpRegestration/UserContactDetails";
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyFaxedWithReceipt?FaxerName=" + FaxerName +
                    "&ReceiverName=" + ReceiverName + "&ReceiverCity=" + ReceiverCity
                    + "&MFCN=" + MFCN + "&FaxAmount=" + obj.FaxingAmount + " " + FaxerCurrency + "&RegisterMFTC=" + RegisterMFTCLink + "&FaxerCountry=" + FaxerCountry);


                string URL = baseUrl + "/EmailTemplate/NonCardUserReceiver?MFReceiptNumber=" + obj.ReceiptNumber +
                                  "&TransactionDate=" + obj.TransactionDate.ToString("dd/MM/yyy") + "&TransactionTime=" + obj.TransactionDate.ToString("HH:mm")
                                    + "&FaxerFullName=" + faInformation.FirstName + " " + faInformation.MiddleName + " " + faInformation.LastName +
                                  "&MFCN=" + obj.MFCN + "&ReceiverFullName=" + recDetailObj.FirstName + " " + recDetailObj.MiddleName + " " + recDetailObj.LastName
                                  + "&Telephone=" + ReceiverPhonecode + " " + recDetailObj.PhoneNumber + "&AgentName=" + "" + "&AgentCode=" + "" +
                                  "&AmountSent=" + obj.FaxingAmount
                                  + "&ExchangeRate=" + obj.ExchangeRate + "&Fee=" + obj.FaxingFee +
                                  "&AmountReceived=" + obj.ReceivingAmount + "&SendingCurrency=" + FaxerCurrency + "&ReceivingCurrency=" + ReceiverCurrency;

                var Receipt = Common.Common.GetPdf(URL);


                mail.SendMail(FaxerEmail, "Confirmation of Money Faxed with MFCN", body, Receipt);
                //mail.SendMail("anankarki97@gmail.com", "Confirmation of Money Faxed with MFCN", body, Receipt);


                string EamilToReceiver_body = "";
                EamilToReceiver_body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/NonCardMoneyTransferConfirmationToReceiver?ReceiverName="
                    + ReceiverName + "&ReceiverCountry=" + ReceiverCountry +
                    "&SenderCountry=" + FaxerCountry);


                mail.SendMail(recDetailObj.EmailAddress, "Money Transfer Confirmation", EamilToReceiver_body);

                // End 


                #region Cash To Cash Transfer SMS


                SmsApi smsApiServices = new SmsApi();
                string receivingAmount = Common.Common.GetCurrencySymbol(recDetailObj.Country) + obj.ReceivingAmount;
                string Fee = Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry) + obj.FaxingFee;
                string message = smsApiServices.GetCashToCashTransferMessage(FaxerName, MFCN, Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry) + obj.FaxingAmount, Fee, receivingAmount);

                string PhoneNo = Common.Common.GetCountryPhoneCode(Common.FaxerSession.FaxerCountry) + "" + faInformation.PhoneNumber;
                smsApiServices.SendSMS(PhoneNo, message);


                string receiverPhoneNo = Common.Common.GetCountryPhoneCode(recDetailObj.Country) + "" + recDetailObj.PhoneNumber;
                smsApiServices.SendSMS(receiverPhoneNo, message);

                #endregion
                if (obj != null)
                {

                    bool IsSaved = false;
                    string cardNumber = Common.FaxerSession.SavedCreditDebitCardDetails.CardNumber.Encrypt();
                    var SavedCardCount = db.SavedCard.Where(x => x.Num == cardNumber).FirstOrDefault();
                    if (SavedCardCount != null)
                    {
                        IsSaved = true;
                    }
                    DB.CardTopUpCreditDebitInformation cardDetails = new DB.CardTopUpCreditDebitInformation()
                    {
                        NonCardTransactionId = obj.Id,
                        NameOnCard = Common.FaxerSession.SavedCreditDebitCardDetails.NameOnCard,
                        ExpiryDate = Common.FaxerSession.SavedCreditDebitCardDetails.EndMonth + "/" + Common.FaxerSession.SavedCreditDebitCardDetails.EndYear,
                        IsSavedCard = IsSaved,
                        CardNumber = "xxxx-xxxx-xxxx-" + Common.FaxerSession.SavedCreditDebitCardDetails.CardNumber.Right(4),
                        AutoRecharged = Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUp,
                    };
                    SSavedCard cardInformationservices = new SSavedCard();
                    cardDetails = cardInformationservices.Save(cardDetails);
                }
                string ReceiverFullName = recDetailObj.FirstName + " " + recDetailObj.MiddleName + " " + recDetailObj.LastName;
                string Country = db.Country.Where(x => x.CountryCode == recDetailObj.Country).Select(x => x.CountryName).FirstOrDefault();
                string FaxingAmount = obj.FaxingAmount + " " + FaxerCurrency;
                string ReceivingAmount = obj.ReceivingAmount + " " + ReceiverCurrency;
                return RedirectToAction("HurrayMessage", new { obj.MFCN, ReceiverFullName, Country, FaxingAmount, ReceivingAmount, obj.ExchangeRate });

            }
            else
            {
                return View(vm);
            }

        }


        public ActionResult HurrayMessage(string MFCN, string ReceiverFullName, string Country, string FaxingAmount, string ReceivingAmount, string ExchangeRate)
        {

            Session.Remove("TransactionSummaryUrl");
            Session.Remove("CreditDebitDetails");
            Session.Remove("SavedCreditDebitCardDetails");
            Session.Remove("ReceiversDetails");
            Session.Remove("FaxingAmountSummary");
            Session.Remove("NonCardReceiversDetails");
            ViewBag.MFCN = MFCN;
            ViewBag.ReceiverName = ReceiverFullName;
            ViewBag.ReceiverCountry = Country;
            ViewBag.SentAmount = FaxingAmount;
            ViewBag.ReceiveAmount = ReceivingAmount;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.ReceiveOption = "Cash Collection From Agent";
            return View();

        }

        public ActionResult NonCardReciver()
        {

            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "countryName");
            if (Common.FaxerSession.CreditDebitDetails != null)
            {

                Models.CreditDebitCardViewModel vm = new CreditDebitCardViewModel();
                vm = Common.FaxerSession.CreditDebitDetails;
                vm.CountyName = Common.FaxerSession.CreditDebitDetails.CountyName;
                vm.FaxingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode);
                vm.FaxingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode);
                return View(vm);
            }
            Models.CreditDebitCardViewModel model = new Models.CreditDebitCardViewModel()
            {
                FaxingAmount = FaxerSession.FaxingAmountSummary.TotalAmount,
                FaxingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode),
                FaxingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult NonCardReciver([Bind(Include = CreditDebitCardViewModel.BindProperty)]Models.CreditDebitCardViewModel model)
        {

            // Countries DropDown List 

            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "countryName");
            //model.FaxingCurrency = "USD"; // This is HardCode
            bool valid = true;
            Services.SFaxerSignUp faService = new Services.SFaxerSignUp();
            var faInformation = faService.GetInformation(FaxerSession.LoggedUser.UserName);
            //var Country = db.Country.Where(x => x.CountryName.ToLower() == model.CountyName.ToLower()).FirstOrDefault();
            //if (Country == null)
            //{
            //    ModelState.AddModelError("CountyName", "County Name Is Not Matched!!");
            //    return View(model);
            //}
            string CountryCode = model.CountyName;
            model.CountyName = CountryCode;
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


            if (string.IsNullOrEmpty(model.SecurityCode))
            {

                ModelState.AddModelError("SecurityCode", "The field is required");
                valid = false;
            }

            if (string.IsNullOrEmpty(model.AddressLineOne))
            {

                ModelState.AddModelError("AddressLineOne", "This field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.CityName))
            {

                ModelState.AddModelError("CityName", "This field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.CountyName))
            {

                ModelState.AddModelError("CountyName", "This field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.ZipCode))
            {

                ModelState.AddModelError("ZipCode", "This field is required");
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

                ModelState.AddModelError("Confirm", "Please accept our terms and condition to proceed your payment ");
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


            //if (ModelState.IsValid)
            //{
            if (valid)
            {

                Common.FaxerSession.CreditDebitDetails = model;
                // end

                return RedirectToAction("NonCardTransactionSummary");
            }

            else
            {
                return View(model);
            }
        }


        [HttpGet]
        public ActionResult NonCardTransactionSummary()
        {


            var FaxerDetails = db.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            var FaxerCountry = Common.Common.GetCountryName(FaxerDetails.Country);
            var faxerCountryCode = Common.Common.GetCountryPhoneCode(FaxerDetails.Country);
            string FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            string ReceiverCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.ReceivingCountry);
            Models.NonCardTransactionSummaryViewModel model = new NonCardTransactionSummaryViewModel()
            {
                ReceiverId = int.Parse(Common.FaxerSession.NonCardReceiversDetails.PreviousReceivers),
                ReceiverName = Common.FaxerSession.NonCardReceiversDetails.ReceiverFirstName + " " + Common.FaxerSession.NonCardReceiversDetails.ReceiverMiddleName + " " + Common.FaxerSession.NonCardReceiversDetails.ReceiverLastName,
                ReceiveOption = "Cash Collection ",
                FaxerName = Common.FaxerSession.LoggedUser.FullName,
                FaxerEmail = Common.FaxerSession.LoggedUser.UserName,
                FaxerPhoneNumber = faxerCountryCode + " " + FaxerDetails.PhoneNumber,
                CardNumber = Common.FaxerSession.CreditDebitDetails.CardNumber.FormatSavedCardNumber(),
                CardExpriyDate = Common.FaxerSession.CreditDebitDetails.EndMM + "/" + Common.FaxerSession.CreditDebitDetails.EndYY,
                CountryOfBirth = FaxerCountry,
                streetAddress = Common.FaxerSession.CreditDebitDetails.AddressLineOne,
                City = Common.FaxerSession.CreditDebitDetails.CityName,
                PostalCode = Common.FaxerSession.CreditDebitDetails.ZipCode,
                SentAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount.ToString() + " " + FaxerCurrency,
                Fees = Common.FaxerSession.FaxingAmountSummary.FaxingFee.ToString() + " " + FaxerCurrency,
                TotalAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount.ToString() + " " + FaxerCurrency,
                TotalReceiveAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount.ToString() + " " + ReceiverCurrency

            };
            Common.FaxerSession.TransactionSummaryUrl = "/CardPayment/NonCardTransactionSummary";
            return View(model);
        }
        [HttpPost]
        public ActionResult NonCardTransactionSummary([Bind(Include = NonCardFaxingFeeSummaryViewModel.BindProperty)]NonCardFaxingFeeSummaryViewModel vm)
        {

            //model.FaxingCurrency = "USD"; // This is HardCode
            bool valid = true;
            Services.SFaxerSignUp faService = new Services.SFaxerSignUp();
            var faInformation = faService.GetInformation(FaxerSession.LoggedUser.UserName);

            //if (ModelState.IsValid)
            //{
            if (valid)
            {
                #region  Strip portion
                //StripeCharge charge = FaxerStripe.CreateTransaction((Int32)Common.FaxerSession.CreditDebitDetails.FaxingAmount * 100 
                //    , Common.FaxerSession.CreditDebitDetails.FaxingCurrency, Common.FaxerSession.CreditDebitDetails.NameOnCard);

                var chargeOptions = new StripeChargeCreateOptions()
                {
                    Amount = (Int32)Common.FaxerSession.CreditDebitDetails.FaxingAmount * 100,
                    Currency = Common.FaxerSession.CreditDebitDetails.FaxingCurrency,
                    Description = "Charge for " + Common.FaxerSession.CreditDebitDetails.NameOnCard,
                    SourceTokenOrExistingSourceId = Common.FaxerSession.CreditDebitDetails.StripeTokenID // obtained with Stripe.js
                };
                var chargeService = new StripeChargeService();
                StripeCharge charge = chargeService.Create(chargeOptions);
                //TODO Stripe Success Validation

                #endregion
                //transaction history object
                Services.SReceiverDetails receiverService = new SReceiverDetails();

                DB.ReceiversDetails recDetailObj = new DB.ReceiversDetails()
                {
                    City = FaxerSession.NonCardReceiversDetails.ReceiverCity,
                    CreatedDate = System.DateTime.Now,
                    Country = FaxerSession.ReceivingCountry,
                    EmailAddress = FaxerSession.NonCardReceiversDetails.ReceiverEmailAddress,
                    FaxerID = FaxerSession.LoggedUser.Id,
                    FirstName = FaxerSession.NonCardReceiversDetails.ReceiverFirstName,
                    IsDeleted = false,
                    LastName = FaxerSession.NonCardReceiversDetails.ReceiverLastName,
                    MiddleName = FaxerSession.NonCardReceiversDetails.ReceiverMiddleName == null ? "" : FaxerSession.NonCardReceiversDetails.ReceiverMiddleName,
                    PhoneNumber = FaxerSession.NonCardReceiversDetails.ReceiverPhoneNumber
                };
                int NonCardReceiveId;
                if (int.Parse(FaxerSession.NonCardReceiversDetails.PreviousReceivers) == 0)
                {
                    receiverService.Add(recDetailObj);
                    NonCardReceiveId = recDetailObj.Id;
                    // Add City in city table 
                    City newCity = new City()
                    {
                        CountryCode = recDetailObj.Country,
                        Module = Module.Faxer,
                        Name = recDetailObj.City
                    };
                    SCity.Save(newCity);
                    //End
                }
                else
                {
                    NonCardReceiveId = int.Parse(FaxerSession.NonCardReceiversDetails.PreviousReceivers);
                }

                SFaxingNonCardTransaction service = new SFaxingNonCardTransaction();
                //get unique new mfcn 
                string MFCN = service.GetNewMFCNToSave();
                var receiptNumber = service.GetNewReceiptNumberToSave();

                DB.FaxingNonCardTransaction obj = new DB.FaxingNonCardTransaction()
                {
                    stripe_ChargeId = charge.Id,
                    NonCardRecieverId = NonCardReceiveId,
                    UserId = 0,
                    FaxingStatus = FaxingStatus.NotReceived,
                    MFCN = MFCN,
                    ReceiptNumber = receiptNumber,
                    FaxingMethod = "PM001",
                    FaxingAmount = FaxerSession.FaxingAmountSummary.FaxingAmount,
                    ReceivingAmount = FaxerSession.FaxingAmountSummary.ReceivingAmount,
                    ExchangeRate = FaxerSession.FaxingAmountSummary.ExchangeRate,
                    FaxingFee = FaxerSession.FaxingAmountSummary.FaxingFee,
                    TransactionDate = System.DateTime.Now,
                };
                //save transaction for non card
                obj = service.SaveTransaction(obj);

                //End Top Up Email 
                // Send Email For Cofirmation of Moneyfaxed
                MailCommon mail = new MailCommon();

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                string FaxerCountry = Common.Common.GetCountryName(Common.FaxerSession.FaxerCountry);
                string body = "";
                string ReceiverName = recDetailObj.FirstName + " " + recDetailObj.MiddleName + " " + recDetailObj.LastName;
                string ReceiverCity = recDetailObj.City;
                string FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
                string ReceiverCurremcy = Common.Common.GetCountryCurrency(recDetailObj.Country);
                string ReceiverPhonecode = Common.Common.GetCountryPhoneCode(recDetailObj.Country);
                string ReceiverCountry = Common.Common.GetCountryName(recDetailObj.Country);
                string RegisterMFTCLink = baseUrl + "/TopUpRegestration/UserContactDetails";
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyFaxedWithReceipt?FaxerName=" + FaxerName +
                    "&ReceiverName=" + ReceiverName + "&ReceiverCity=" + ReceiverCity
                    + "&MFCN=" + MFCN + "&FaxAmount=" + obj.FaxingAmount + " " + FaxerCurrency + "&RegisterMFTC=" + RegisterMFTCLink + "&FaxerCountry=" + FaxerCountry);


                string URL = baseUrl + "/EmailTemplate/NonCardUserReceiver?MFReceiptNumber=" + obj.ReceiptNumber +
                                  "&TransactionDate=" + obj.TransactionDate.ToString("dd/MM/yyy") + "&TransactionTime=" + obj.TransactionDate.ToString("HH:mm")
                                    + "&FaxerFullName=" + faInformation.FirstName + " " + faInformation.MiddleName + " " + faInformation.LastName +
                                  "&MFCN=" + obj.MFCN + "&ReceiverFullName=" + recDetailObj.FirstName + " " + recDetailObj.MiddleName + " " + recDetailObj.LastName
                                  + "&Telephone=" + ReceiverPhonecode + " " + recDetailObj.PhoneNumber + "&AgentName=" + "" + "&AgentCode=" + ""
                                  + "&AmountSent=" + obj.FaxingAmount
                                  + "&ExchangeRate=" + obj.ExchangeRate + "&Fee=" + obj.FaxingFee
                                  + "&AmountReceived=" + obj.ReceivingAmount + "&SendingCurrency=" + FaxerCurrency + "&ReceivingCurrency=" + ReceiverCurremcy;

                var Receipt = Common.Common.GetPdf(URL);

                mail.SendMail(FaxerEmail, "Confirmation of Money Faxed with MFCN", body, Receipt);

                string EamilToReceiver_body = "";
                EamilToReceiver_body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/NonCardMoneyTransferConfirmationToReceiver?ReceiverName="
                    + ReceiverName + "&ReceiverCountry=" + ReceiverCountry +
                    "&SenderCountry=" + FaxerCountry);


                mail.SendMail(recDetailObj.EmailAddress, "Money Transfer Confirmation", EamilToReceiver_body);





                // End

                #region Cash To Cash Transfer SMS


                SmsApi smsApiServices = new SmsApi();


                string Fee = Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry) + obj.FaxingFee;
                string receivingAmount = Common.Common.GetCurrencySymbol(recDetailObj.Country) + obj.ReceivingAmount;
                string message = smsApiServices.GetCashToCashTransferMessage(FaxerName, MFCN, Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry) + obj.FaxingAmount, Fee, receivingAmount);

                string PhoneNo = Common.Common.GetCountryPhoneCode(Common.FaxerSession.FaxerCountry) + "" + faInformation.PhoneNumber;
                smsApiServices.SendSMS(PhoneNo, message);

                string receiverPhoneNo = Common.Common.GetCountryPhoneCode(recDetailObj.Country) + "" + recDetailObj.PhoneNumber;
                smsApiServices.SendSMS(receiverPhoneNo, message);
                #endregion
                if (obj != null)
                {
                    bool IsSaved = false;
                    string cardNumber = Common.FaxerSession.CreditDebitDetails.CardNumber.Encrypt();
                    var SavedCardCount = db.SavedCard.Where(x => x.Num == cardNumber).FirstOrDefault();
                    if (SavedCardCount != null)
                    {
                        IsSaved = true;
                    }
                    DB.CardTopUpCreditDebitInformation cardDetails = new DB.CardTopUpCreditDebitInformation()
                    {
                        NonCardTransactionId = obj.Id,
                        NameOnCard = Common.FaxerSession.CreditDebitDetails.NameOnCard,
                        ExpiryDate = Common.FaxerSession.CreditDebitDetails.EndMM + "/" + Common.FaxerSession.CreditDebitDetails.EndYY,
                        IsSavedCard = IsSaved,
                        CardNumber = "xxxx-xxxx-xxxx-" + Common.FaxerSession.CreditDebitDetails.CardNumber.Right(4),
                        AutoRecharged = Common.FaxerSession.CreditDebitDetails.AutoTopUp,
                    };
                    SSavedCard cardInformationservices = new SSavedCard();
                    cardDetails = cardInformationservices.Save(cardDetails);
                }
                string ReceiverFullName = recDetailObj.FirstName + " " + recDetailObj.MiddleName + " " + recDetailObj.LastName;
                string Country = db.Country.Where(x => x.CountryCode == recDetailObj.Country).Select(x => x.CountryName).FirstOrDefault();
                string FaxingAmount = obj.FaxingAmount + " " + FaxerCurrency;
                string ReceivingAmount = obj.ReceivingAmount + " " + ReceiverCurremcy;
                return RedirectToAction("HurrayMessage", new { obj.MFCN, ReceiverFullName, Country, FaxingAmount, ReceivingAmount, obj.ExchangeRate });

            }

            else
            {
                return View(vm);
            }
        }
        [HttpGet]
        public ActionResult NonCardReceiverTransferUsingSavedCreditDebitCard(int savedCardId = 0)
        {

            //List<SavedCard> list = db.SavedCard.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id).OrderBy(x => x.CardName).ToList();

            var list = Common.Common.GetSavedCardDetails();
            if (list.Count() == 0)
            {
                @TempData["CardCount"] = list.Count();
                FaxerSession.FromUrl = "/CardPayment/NonCardReceiverTransferUsingSavedCreditDebitCard";
                return RedirectToAction("ChoosePaymentOption", "FaxMoney");
            }
            //foreach (var item in list)
            //{
            //    item.CardName = Common.Common.Decrypt(item.CardName);
            //}
            PaymentUsingSavedCreditDebitCardVm vm = new PaymentUsingSavedCreditDebitCardVm();
            vm.FaxingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode);
            vm.FaxingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode);
            if (savedCardId == 0)
            {
                ViewBag.SavedCard = new SelectList(list, "Id", "CardNo");
                vm.TopUpAmount = FaxerSession.FaxingAmountSummary.TotalAmount;
            }
            else
            {
                ViewBag.SavedCard = new SelectList(list, "Id", "CardNo", savedCardId);
                vm = (from c in db.SavedCard.Where(x => x.Id == savedCardId).ToList()
                      select new PaymentUsingSavedCreditDebitCardVm()
                      {
                          NameOnCard = c.CardName.Decrypt(),
                          CardNumber = c.Num.Decrypt(),
                          EndMonth = c.EMonth.Decrypt(),
                          EndYear = c.EYear.Decrypt(),
                          SecurityCode = c.ClientCode.Decrypt(),
                          TopUpAmount = FaxerSession.FaxingAmountSummary.TotalAmount,
                          FaxingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode),
                          FaxingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode)

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
            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");
            return View(vm);

        }
        [HttpPost]
        public ActionResult NonCardReceiverTransferUsingSavedCreditDebitCard([Bind(Include = PaymentUsingSavedCreditDebitCardVm.BindProperty)]PaymentUsingSavedCreditDebitCardVm vm)
        {

            // Countries Drop Down List 
            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");
            //List<SavedCard> list = db.SavedCard.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id).OrderBy(x => x.CardName).ToList();
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

                ModelState.AddModelError("Address1", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.City))
            {

                ModelState.AddModelError("City", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.Country))
            {

                ModelState.AddModelError("Country", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.PostalCode))
            {

                ModelState.AddModelError("PostalCode", "The field is required");
                valid = false;
            }
            if (!string.IsNullOrEmpty(vm.Address1) && vm.Address1.Trim().ToLower() != faInformation.Address1.Trim().ToLower())
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
                ModelState.AddModelError("Country", "Please select your account registered country!");
                valid = false;
            }

            if (!string.IsNullOrEmpty(vm.PostalCode) && vm.PostalCode.Trim() != faInformation.PostalCode.Trim())
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

                return RedirectToAction("NonCardReceiverUsingSavedCreditDebitCardTransactionSummary");
            }

            else
            {
                return View(vm);
            }

        }

        [HttpGet]
        public ActionResult NonCardReceiverUsingSavedCreditDebitCardTransactionSummary()
        {

            var FaxerDetails = db.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            var FaxerCountry = Common.Common.GetCountryName(FaxerDetails.Country);
            var faxerCountryCode = Common.Common.GetCountryPhoneCode(FaxerDetails.Country);
            string FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            string ReceiverCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.ReceivingCountry);
            Models.NonCardTransactionSummaryViewModel model = new NonCardTransactionSummaryViewModel()
            {
                SavedcardId = int.Parse(Common.FaxerSession.SavedCreditDebitCardDetails.SavedCard),
                ReceiverId = int.Parse(Common.FaxerSession.NonCardReceiversDetails.PreviousReceivers),
                ReceiverName = Common.FaxerSession.NonCardReceiversDetails.ReceiverFirstName + " " + Common.FaxerSession.NonCardReceiversDetails.ReceiverMiddleName + " " + Common.FaxerSession.NonCardReceiversDetails.ReceiverLastName,
                ReceiveOption = "Cash Collection ",
                FaxerName = Common.FaxerSession.LoggedUser.FullName,
                FaxerEmail = Common.FaxerSession.LoggedUser.UserName,
                FaxerPhoneNumber = faxerCountryCode + " " + FaxerDetails.PhoneNumber,
                CardNumber = Common.FaxerSession.SavedCreditDebitCardDetails.CardNumber.FormatSavedCardNumber(),
                CardExpriyDate = Common.FaxerSession.SavedCreditDebitCardDetails.EndMonth + "/" + Common.FaxerSession.SavedCreditDebitCardDetails.EndYear,
                CountryOfBirth = FaxerCountry,
                streetAddress = Common.FaxerSession.SavedCreditDebitCardDetails.Address1,
                City = Common.FaxerSession.SavedCreditDebitCardDetails.City,
                PostalCode = Common.FaxerSession.SavedCreditDebitCardDetails.PostalCode,
                SentAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount.ToString() + " " + FaxerCurrency,
                Fees = Common.FaxerSession.FaxingAmountSummary.FaxingFee.ToString() + " " + FaxerCurrency,
                TotalAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount.ToString() + " " + FaxerCurrency,
                TotalReceiveAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount.ToString() + " " + ReceiverCurrency

            };
            Common.FaxerSession.TransactionSummaryUrl = "/CardPayment/NonCardReceiverUsingSavedCreditDebitCardTransactionSummary";
            return View(model);

        }

        [HttpPost]
        public ActionResult NonCardReceiverUsingSavedCreditDebitCardTransactionSummary([Bind(Include = NonCardTransactionSummaryViewModel.BindProperty)]NonCardTransactionSummaryViewModel vm)
        {


            //model.FaxingCurrency = "USD"; // This is HardCode
            bool valid = true;
            Services.SFaxerSignUp faService = new Services.SFaxerSignUp();
            var faInformation = faService.GetInformation(FaxerSession.LoggedUser.UserName);

            //if (ModelState.IsValid)
            //{
            if (valid)
            {
                #region  Strip portion

                //Difference between stripe charge option AND CREATE TRANASACTION 
                // IN CHARGE OPTION AMOUNT NEED TO BE MULTIPLIED BY 100 TO GET OUR EXACT AMOUNT

                // IN FaxerStripe.CreateTransaction NEED NOT TO BE MULTIPLIED BY 100 IT WILL AUTOMATICALLY MULTIPLIED BUT THE DECIMAL NUMBER WILL GET LOST
                var chargeOptions = new StripeChargeCreateOptions()
                {
                    Amount = (Int32)Common.FaxerSession.SavedCreditDebitCardDetails.TopUpAmount * 100,
                    Currency = Common.FaxerSession.SavedCreditDebitCardDetails.FaxingCurrency,
                    Description = "Charge for " + Common.FaxerSession.SavedCreditDebitCardDetails.NameOnCard,
                    SourceTokenOrExistingSourceId = Common.FaxerSession.SavedCreditDebitCardDetails.StripeTokenID // obtained with Stripe.js
                };
                var chargeService = new StripeChargeService();
                StripeCharge charge = chargeService.Create(chargeOptions);


                //StripeCharge charge = FaxerStripe.CreateTransaction((Int32)Common.FaxerSession.SavedCreditDebitCardDetails.TopUpAmount 
                //    , Common.FaxerSession.SavedCreditDebitCardDetails.FaxingCurrency, Common.FaxerSession.SavedCreditDebitCardDetails.NameOnCard);


                //TODO Stripe Success Validation

                #endregion
                //transaction history object
                Services.SReceiverDetails receiverService = new SReceiverDetails();

                DB.ReceiversDetails recDetailObj = new DB.ReceiversDetails()
                {
                    City = FaxerSession.NonCardReceiversDetails.ReceiverCity,
                    CreatedDate = System.DateTime.Now,
                    Country = FaxerSession.ReceivingCountry,
                    EmailAddress = FaxerSession.NonCardReceiversDetails.ReceiverEmailAddress,
                    FaxerID = FaxerSession.LoggedUser.Id,
                    FirstName = FaxerSession.NonCardReceiversDetails.ReceiverFirstName,
                    IsDeleted = false,
                    LastName = FaxerSession.NonCardReceiversDetails.ReceiverLastName,
                    MiddleName = FaxerSession.NonCardReceiversDetails.ReceiverMiddleName == null ? "" : FaxerSession.NonCardReceiversDetails.ReceiverMiddleName,
                    PhoneNumber = FaxerSession.NonCardReceiversDetails.ReceiverPhoneNumber
                };
                int NonCardReceiveId;
                if (int.Parse(FaxerSession.NonCardReceiversDetails.PreviousReceivers) == 0)
                {
                    receiverService.Add(recDetailObj);
                    NonCardReceiveId = recDetailObj.Id;

                    City newCity = new City()
                    {
                        CountryCode = recDetailObj.Country,
                        Module = Module.Faxer,
                        Name = recDetailObj.City
                    };
                    SCity.Save(newCity);
                }
                else
                {
                    NonCardReceiveId = int.Parse(FaxerSession.NonCardReceiversDetails.PreviousReceivers);
                }

                SFaxingNonCardTransaction service = new SFaxingNonCardTransaction();
                //get unique new mfcn 
                string MFCN = service.GetNewMFCNToSave();
                var receiptNumber = service.GetNewReceiptNumberToSave();

                DB.FaxingNonCardTransaction obj = new DB.FaxingNonCardTransaction()
                {
                    NonCardRecieverId = NonCardReceiveId,
                    stripe_ChargeId = charge.Id,
                    UserId = 0,
                    FaxingStatus = FaxingStatus.NotReceived,
                    MFCN = MFCN,
                    ReceiptNumber = receiptNumber,
                    FaxingMethod = "PM002",
                    FaxingAmount = FaxerSession.FaxingAmountSummary.FaxingAmount,
                    ReceivingAmount = FaxerSession.FaxingAmountSummary.ReceivingAmount,
                    ExchangeRate = FaxerSession.FaxingAmountSummary.ExchangeRate,
                    FaxingFee = FaxerSession.FaxingAmountSummary.FaxingFee,
                    TransactionDate = System.DateTime.Now,
                };
                //save transaction for non card
                obj = service.SaveTransaction(obj);


                //End Top Up Email 
                // Send Email For Cofirmation of Moneyfaxed
                MailCommon mail = new MailCommon();

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                string FaxerCountry = Common.Common.GetCountryName(Common.FaxerSession.FaxerCountry);
                string body = "";
                string ReceiverName = recDetailObj.FirstName + " " + recDetailObj.MiddleName + " " + recDetailObj.LastName;
                string ReceiverCity = recDetailObj.City;
                string FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
                string ReceiverCurremcy = Common.Common.GetCountryCurrency(recDetailObj.Country);
                string ReceiverPhonecode = Common.Common.GetCountryPhoneCode(recDetailObj.Country);
                string ReceiverCountry = Common.Common.GetCountryName(recDetailObj.Country);
                string RegisterMFTCLink = baseUrl + "/TopUpRegestration/UserContactDetails";
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyFaxedWithReceipt?FaxerName=" + FaxerName +
                    "&ReceiverName=" + ReceiverName + "&ReceiverCity=" + ReceiverCity
                    + "&MFCN=" + MFCN + "&FaxAmount=" + obj.FaxingAmount + " " + FaxerCurrency +
                    "&RegisterMFTC=" + RegisterMFTCLink + "&FaxerCountry=" + FaxerCountry);


                string URL = baseUrl + "/EmailTemplate/NonCardUserReceiver?MFReceiptNumber=" + obj.ReceiptNumber +
                                  "&TransactionDate=" + obj.TransactionDate.ToString("dd/MM/yyy") + "&TransactionTime=" + obj.TransactionDate.ToString("HH:mm")
                                    + "&FaxerFullName=" + faInformation.FirstName + " " + faInformation.MiddleName + " " + faInformation.LastName +
                                  "&MFCN=" + obj.MFCN + "&ReceiverFullName=" + recDetailObj.FirstName + " " + recDetailObj.MiddleName + " " + recDetailObj.LastName
                                  + "&Telephone=" + ReceiverPhonecode + " " + recDetailObj.PhoneNumber + "&AgentName=" + "" + "&AgentCode=" + ""
                                  + "&AmountSent=" + obj.FaxingAmount
                                  + "&ExchangeRate=" + obj.ExchangeRate + "&Fee=" + obj.FaxingFee
                                  + "&AmountReceived=" + obj.ReceivingAmount + "&SendingCurrency=" + FaxerCurrency + "&ReceivingCurrency=" + ReceiverCurremcy;

                var Receipt = Common.Common.GetPdf(URL);

                mail.SendMail(FaxerEmail, "Confirmation of Money Faxed with MFCN", body, Receipt);


                string EamilToReceiver_body = "";
                EamilToReceiver_body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/NonCardMoneyTransferConfirmationToReceiver?ReceiverName="
                    + ReceiverName + "&ReceiverCountry=" + ReceiverCountry +
                    "&SenderCountry=" + FaxerCountry);


                mail.SendMail(recDetailObj.EmailAddress, "Money Transfer Confirmation", EamilToReceiver_body);


                #region Cash To Cash Transfer SMS


                SmsApi smsApiServices = new SmsApi();

                string receivingAmount = Common.Common.GetCurrencySymbol(recDetailObj.Country) + obj.ReceivingAmount;
                string Fee = Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry) + obj.FaxingFee;
                string message = smsApiServices.GetCashToCashTransferMessage(FaxerName, MFCN, Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry) + obj.FaxingAmount, Fee, receivingAmount);

                string PhoneNo = Common.Common.GetCountryPhoneCode(Common.FaxerSession.FaxerCountry) + "" + faInformation.PhoneNumber;
                smsApiServices.SendSMS(PhoneNo, message);


                string receiverPhoneNo = Common.Common.GetCountryPhoneCode(recDetailObj.Country) + "" + recDetailObj.PhoneNumber;
                smsApiServices.SendSMS(receiverPhoneNo, message);
                #endregion


                // End
                if (obj != null)
                {
                    bool IsSaved = false;
                    string cardNumber = Common.FaxerSession.SavedCreditDebitCardDetails.CardNumber.Encrypt();
                    var SavedCardCount = db.SavedCard.Where(x => x.Num == cardNumber).FirstOrDefault();
                    if (SavedCardCount != null)
                    {
                        IsSaved = true;
                    }
                    DB.CardTopUpCreditDebitInformation cardDetails = new DB.CardTopUpCreditDebitInformation()
                    {
                        NonCardTransactionId = obj.Id,
                        NameOnCard = Common.FaxerSession.SavedCreditDebitCardDetails.NameOnCard,
                        ExpiryDate = Common.FaxerSession.SavedCreditDebitCardDetails.EndMonth + "/" + Common.FaxerSession.SavedCreditDebitCardDetails.EndYear,
                        IsSavedCard = IsSaved,
                        CardNumber = "xxxx-xxxx-xxxx-" + Common.FaxerSession.SavedCreditDebitCardDetails.CardNumber.Right(4),
                        AutoRecharged = Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUp,
                    };
                    SSavedCard cardInformationservices = new SSavedCard();
                    cardDetails = cardInformationservices.Save(cardDetails);
                }
                string ReceiverFullName = recDetailObj.FirstName + " " + recDetailObj.MiddleName + " " + recDetailObj.LastName;
                string Country = db.Country.Where(x => x.CountryCode == recDetailObj.Country).Select(x => x.CountryName).FirstOrDefault();
                string FaxingAmount = obj.FaxingAmount + " " + FaxerCurrency;
                string ReceivingAmount = obj.ReceivingAmount + " " + ReceiverCurremcy;
                return RedirectToAction("HurrayMessage", new { obj.MFCN, ReceiverFullName, Country, FaxingAmount, ReceivingAmount, obj.ExchangeRate });

            }

            else
            {
                return View(vm);
            }
        }

        public ActionResult MerchantCardPayment()
        {


            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");


            Models.CreditDebitCardViewModel model = new Models.CreditDebitCardViewModel()
            {
                FaxingAmount = FaxerSession.FaxingAmountSummary.TotalAmount
            };
            if (Common.FaxerSession.CreditDebitDetails != null)
            {
                model = Common.FaxerSession.CreditDebitDetails;
            }
            var CardCount = db.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).Count();
            ViewBag.CardCount = CardCount;
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry);

            model.FaxingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode);
            model.FaxingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode);

            #region Auto Payment has been setuped or not
            var AutoPaymenthasBeenSetuped = db.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == Common.FaxerSession.LoggedUser.Id && x.KiiPayBusinessInformationId == Common.FaxerSession.BusinessInformationId).FirstOrDefault();
            if ((AutoPaymenthasBeenSetuped != null) && AutoPaymenthasBeenSetuped.EnableAutoPayment == true)
            {

                ViewBag.AutoPayment = 1;
            }
            else
            {
                ViewBag.AutoPayment = 0;
            }
            #endregion 
            return View(model);
        }
        [HttpPost]
        public ActionResult MerchantCardPayment([Bind(Include = CreditDebitCardViewModel.BindProperty)]Models.CreditDebitCardViewModel model)
        {
            // Countries Drop Down List 

            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");
            #region Auto Payment has been setuped or not
            var AutoPaymenthasBeenSetuped = db.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == Common.FaxerSession.LoggedUser.Id && x.KiiPayBusinessInformationId == Common.FaxerSession.BusinessInformationId).FirstOrDefault();
            if ((AutoPaymenthasBeenSetuped != null) && AutoPaymenthasBeenSetuped.EnableAutoPayment == true)
            {

                ViewBag.AutoPayment = 1;
            }
            else
            {
                ViewBag.AutoPayment = 0;
            }
            #endregion 

            // Faxer Currency
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry);

            var CardCount = db.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).Count();
            ViewBag.CardCount = CardCount;
            bool valid = true;
            Services.SFaxerSignUp faService = new Services.SFaxerSignUp();
            var faInformation = faService.GetInformation(FaxerSession.LoggedUser.UserName);
            //var Country = db.Country.Where(x => x.CountryName.ToLower() == model.CountyName.ToLower()).FirstOrDefault();
            //if (Country == null)
            //{
            //    ModelState.AddModelError("CountyName", "County Name Is Not Matched!!");
            //    return View(model);
            //}
            string CountryCode = model.CountyName;
            model.CountyName = CountryCode;
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
            if (string.IsNullOrEmpty(model.NameOnCard))
            {

                ModelState.AddModelError("NameOnCard", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.CardNumber))
            {

                ModelState.AddModelError("CardNumber", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.EndMM))
            {

                ModelState.AddModelError("EndMM", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.EndYY))
            {

                ModelState.AddModelError("EndYY", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.SecurityCode))
            {

                ModelState.AddModelError("SecurityCode", "The field is required");
                valid = false;
            }

            if (string.IsNullOrEmpty(model.AddressLineOne))
            {

                ModelState.AddModelError("AddressLineOne", "This field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.CityName))
            {

                ModelState.AddModelError("CityName", "This field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.CountyName))
            {

                ModelState.AddModelError("CountyName", "This field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(model.ZipCode))
            {

                ModelState.AddModelError("ZipCode", "This field is required");
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

                ModelState.AddModelError("Confirm", "Please Accept our the terms and condition");
                valid = false;
            }
            var SavedCardCount = db.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).Count();

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
                //Intitate Credit/Debit Details Session
                Common.FaxerSession.CreditDebitDetails = model;


                //End

                return RedirectToAction("MerchantPaymentTransactionSummay");
            }
            else
            {
                return View(model);
            }
        }



        [HttpGet]
        public ActionResult MerchantPaymentTransactionSummay()
        {

            var FaxerDetails = db.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            var FaxerCountry = Common.Common.GetCountryName(FaxerDetails.Country);
            var faxerCountryCode = Common.Common.GetCountryPhoneCode(FaxerDetails.Country);
            int BuinessId = FaxerSession.BusinessInformationId;
            var BusinessInfo = db.KiiPayBusinessInformation.Where(x => x.Id == BuinessId).FirstOrDefault();
            string BusinessCurrency = Common.Common.GetCountryCurrency(BusinessInfo.BusinessOperationCountryCode);
            string FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            Models.MerchantPaymentTransactionSummaryViewModel model = new MerchantPaymentTransactionSummaryViewModel()
            {
                BusinessMerchantName = BusinessInfo.BusinessName,
                BusinessMFCode = BusinessInfo.BusinessMobileNo,
                ReceiveOption = "MFBC Card Withdrawal",
                FaxerName = Common.FaxerSession.LoggedUser.FullName,
                FaxerEmail = Common.FaxerSession.LoggedUser.UserName,
                FaxerPhoneNumber = faxerCountryCode + " " + FaxerDetails.PhoneNumber,
                CardNumber = Common.FaxerSession.CreditDebitDetails.CardNumber.FormatSavedCardNumber(),
                CardExpriyDate = Common.FaxerSession.CreditDebitDetails.EndMM + "/" + Common.FaxerSession.CreditDebitDetails.EndYY,
                CountryOfBirth = FaxerCountry,
                streetAddress = Common.FaxerSession.CreditDebitDetails.AddressLineOne,
                City = Common.FaxerSession.CreditDebitDetails.CityName,
                PostalCode = Common.FaxerSession.CreditDebitDetails.ZipCode,
                SentAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount.ToString() + " " + FaxerCurrency,
                Fees = Common.FaxerSession.FaxingAmountSummary.FaxingFee.ToString() + " " + FaxerCurrency,
                TotalAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount.ToString() + " " + FaxerCurrency,
                TotalReceiveAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount.ToString() + " " + BusinessCurrency,
                PaymentReference = Common.FaxerSession.PaymentRefrence

            };
            Common.FaxerSession.TransactionSummaryUrl = "/CardPayment/MerchantPaymentTransactionSummay";
            return View(model);
        }
        [HttpPost]
        public ActionResult MerchantPaymentTransactionSummay([Bind(Include = MerchantPaymentTransactionSummaryViewModel.BindProperty)]MerchantPaymentTransactionSummaryViewModel vm)
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
                    Amount = (Int32)Common.FaxerSession.CreditDebitDetails.FaxingAmount * 100,
                    Currency = Common.FaxerSession.CreditDebitDetails.FaxingCurrency,
                    Description = "Charge for " + Common.FaxerSession.CreditDebitDetails.NameOnCard,
                    SourceTokenOrExistingSourceId = Common.FaxerSession.CreditDebitDetails.StripeTokenID // obtained with Stripe.js
                };
                var chargeService = new StripeChargeService();
                StripeCharge charge = chargeService.Create(chargeOptions);
                #endregion
                //transaction history object
                DB.SenderKiiPayBusinessPaymentInformation obj = new DB.SenderKiiPayBusinessPaymentInformation()
                {
                    KiiPayBusinessInformationId = FaxerSession.BusinessInformationId,
                    SenderInformationId = FaxerSession.LoggedUser.Id,
                    PaymentAmount = FaxerSession.FaxingAmountSummary.FaxingAmount,
                    PaymentRefrence = FaxerSession.PaymentRefrence,
                };
                SFaxerMerchantPaymentInformation service = new SFaxerMerchantPaymentInformation();
                obj = service.SaveTransaction(obj, FaxerSession.BusinessInformationId);

                // Send Email for Confirmation Of Merchant Payment 
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                MailCommon mail = new MailCommon();
                string body = "";

                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                var BusinessMerchantDetails = service.GetBusinessMerchantDetials(obj.KiiPayBusinessInformationId);
                string BusinessEmail = service.GetBusinessEmail(obj.KiiPayBusinessInformationId);
                string PayForGoodsAbroad = baseUrl + "/PayForGoodsAndServicesAbroad/MerchantDetails?MerchantACNumber=" + BusinessMerchantDetails.BusinessMobileNo;
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMerchantPaymentWithReceiptFaxer?FaxerName="
                    + FaxerName + "&MerchantBusinessName=" + BusinessMerchantDetails.BusinessName + "&PayForGoodsAbroad=" + PayForGoodsAbroad);

                // Generate a receipt PDF

                string fee = Common.FaxerSession.FaxingAmountSummary.FaxingFee.ToString();
                string AmountPaid = Common.FaxerSession.FaxingAmountSummary.TotalAmount.ToString();
                string AmountReceived = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount.ToString();
                string exchangeRate = Common.FaxerSession.FaxingAmountSummary.ExchangeRate.ToString();
                string transactionDate = DateTime.Now.ToString("dd/MM/yyyy");
                string transactionTime = DateTime.Now.ToString("HH:mm");
                string FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
                string ReceiverCurremcy = Common.Common.GetCountryCurrency(BusinessMerchantDetails.BusinessOperationCountryCode);
                string ReceiverPhonecode = Common.Common.GetCountryPhoneCode(BusinessMerchantDetails.BusinessOperationCountryCode);
                string ReceiptNumber = FaxerSession.PayGoodsAndServicesReceiptNumber;
                var ReceiptURL = baseUrl + "/EmailTemplate/PayGoodsAndServicesFaxerReceipt?ReceiptNumber=" + ReceiptNumber +
                     "&Date=" + transactionDate + "&Time=" + transactionTime + "&FaxerFullName=" + FaxerName + "&FaxerCountry=" + Common.Common.GetCountryName(Common.FaxerSession.LoggedUser.CountryCode) + "&BusinessMerchantName=" + BusinessMerchantDetails.BusinessName
                     + "&BusinessMFCode=" + BusinessMerchantDetails.BusinessMobileNo + "&BusinessCountry=" + Common.Common.GetCountryName(BusinessMerchantDetails.BusinessOperationCountryCode) + "&BusinessCity=" + BusinessMerchantDetails.BusinessOperationCity
                     + "&AmountPaid=" + AmountPaid + "&ExchangeRate=" + exchangeRate +
                     "&AmountInLocalCurrency=" + AmountReceived + ReceiverCurremcy + "&Fee=" + fee + "&FaxerCurrency=" + FaxerCurrency + "&ReceiverCurrreny=" + ReceiverCurremcy;

                var receipt = Common.Common.GetPdf(ReceiptURL);

                // receipt end 
                // send Email With receipt attachment 

                mail.SendMail(FaxerEmail, "Confirmation of Payment to a Merchant", body, receipt);

                // End 
                string body2 = "";
                //Send Email for Confirmation of Payment to a Merchant 
                body2 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMerchantPaymentMerchant?BusinessMerchantName="
                    + BusinessMerchantDetails.BusinessName + "&FaxerName=" + FaxerName);
                mail.SendMail(BusinessEmail, "Confirmation of Payment to a Merchant", body2);

                // End 



                #region Send  Business Payment SMS 

                SmsApi smsApiServices = new SmsApi();
                string receivingAmount = Common.Common.GetCurrencySymbol(BusinessMerchantDetails.BusinessOperationCountryCode) + AmountReceived;
                string Amount = Common.Common.GetCurrencySymbol(faInformation.Country) + AmountPaid;
                string message = smsApiServices.GetBusinessPaymentMessage(FaxerName, BusinessMerchantDetails.BusinessMobileNo, BusinessMerchantDetails.BusinessName,
                                                                   Amount, obj.PaymentRefrence, receivingAmount);

                string PhoneNo = Common.Common.GetCountryPhoneCode(faInformation.Country) + "" + faInformation.PhoneNumber;
                smsApiServices.SendSMS(PhoneNo, message);
                string ReceiverPhoneNo = Common.Common.GetCountryPhoneCode(BusinessMerchantDetails.BusinessOperationCountryCode) + "" + BusinessMerchantDetails.PhoneNumber;
                smsApiServices.SendSMS(ReceiverPhoneNo, message);
                #endregion

                if (Common.FaxerSession.CreditDebitDetails.SaveCard)
                {

                    var card = db.SavedCard.Where(x => x.UserId == FaxerSession.LoggedUser.Id).Count();
                    if (card < 1)
                    {
                        DB.SavedCard savedCardObject = new DB.SavedCard()
                        {
                            CardName = Common.FaxerSession.CreditDebitDetails.NameOnCard.Encrypt(),
                            EMonth = Common.FaxerSession.CreditDebitDetails.EndMM.Encrypt(),
                            EYear = Common.FaxerSession.CreditDebitDetails.EndYY.Encrypt(),
                            CreatedDate = System.DateTime.Now,
                            UserId = FaxerSession.LoggedUser.Id,
                            Num = Common.FaxerSession.CreditDebitDetails.CardNumber.Encrypt(),
                            ClientCode = Common.FaxerSession.CreditDebitDetails.SecurityCode.Encrypt(),
                        };
                        SSavedCard cardservices = new SSavedCard();
                        savedCardObject = cardservices.Add(savedCardObject);

                        // Send Email For Card saved
                        string CardNumber = "xxxx-xxxx-xxxx-" + savedCardObject.Num.Decrypt().Right(4);
                        string body1 = "";
                        string TopUpMoneyfaxCard = baseUrl + "/StartFaxingMoney/Index?mFTCCardInformationId=";
                        string PayForGoodsAbroad_CreditOrdebitCard = baseUrl + "/PayForGoodsAndServicesAbroad/MerchantDetails?MerchantACNumber=" + "";
                        string SetAutoTopUpPayment = baseUrl + "/FaxerAutoPayments/MoneyFaxCardAutoTopUp?mftcCardInformationId=";

                        body1 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/NewCreditDebitCardAddedEmail?FaxerName=" + FaxerName
                            + "&LastForDigitOfCreditOrDebitCard=" + CardNumber + "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard +
                            "&PayForGoodsAbroad=" + PayForGoodsAbroad_CreditOrdebitCard + "&SetAutoTopUp=" + SetAutoTopUpPayment);
                        mail.SendMail(FaxerEmail, "New Credit/Debit Card Added ", body1);
                        // End 
                    }

                }
                if ((Common.FaxerSession.CreditDebitDetails.AutoTopUp == true)
                    && Common.FaxerSession.CreditDebitDetails.AutoTopUpAmount > 0 && (int)Common.FaxerSession.CreditDebitDetails.PaymentFrequency > 0)
                {
                    var cardDetails = db.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                    if (cardDetails != null)
                    {

                        var MFBCCard = db.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == obj.KiiPayBusinessInformationId).FirstOrDefault();
                        MFBCCard.AutoTopUp = true;
                        db.Entry(MFBCCard).State = System.Data.Entity.EntityState.Modified;
                        var PaymentInformation = db.FaxerMerchantPaymentInformation.Where(x => x.KiiPayBusinessInformationId == obj.KiiPayBusinessInformationId && x.SenderInformationId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                        PaymentInformation.AutoPaymentAmount = Common.FaxerSession.CreditDebitDetails.AutoTopUpAmount;
                        PaymentInformation.AutoPaymentFrequency = Common.FaxerSession.CreditDebitDetails.PaymentFrequency;
                        PaymentInformation.EnableAutoPayment = true;
                        PaymentInformation.FrequencyDetails = Common.FaxerSession.CreditDebitDetails.PaymentDay;
                        PaymentInformation.PaymentRefrence = Common.FaxerSession.PaymentRefrence;
                        db.Entry(PaymentInformation).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        string bodyAutoPayment = "";
                        string FaxerCountry = db.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).Select(x => x.Country).FirstOrDefault();
                        string CountryCurrency = Common.Common.GetCountryCurrency(FaxerCountry);

                        string CardNumber = "xxxx-xxxx-xxxx-" + cardDetails.Num.Decrypt().Right(4);
                        string SetAutoPaymentLink = baseUrl + "/FaxerAutoPayments/MerchantAutoPayments?faxMerchantPayInfoId=" + obj.Id;
                        string PayforGoodsBusinessMerchant = baseUrl + "/PayForGoodsAndServicesAbroad/MerchantDetails?MerchantACNumber=" + BusinessMerchantDetails.BusinessMobileNo;
                        bodyAutoPayment = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfAutoPaymentSetupFaxer/?FaxerName=" +
                            FaxerName + "&AutoPaymentAmount=" + PaymentInformation.AutoPaymentAmount + "&CountryCurrencySymbol=" + CountryCurrency
                            + "&BusinessMerchantName=" + BusinessMerchantDetails.BusinessName + "&AutoPaymentFrequency=" + PaymentInformation.AutoPaymentFrequency
                            + "&CreditORDebitCardlast4digits=" + CardNumber
                            + "&SetAutoPayment=" + SetAutoPaymentLink +
                            "&PayforGoodsBusinessMerchant=" + PayforGoodsBusinessMerchant);
                        mail.SendMail(FaxerEmail, "Confirmation of Auto Payment Setup", bodyAutoPayment);
                    }
                }
                string BusinessMFCode = BusinessMerchantDetails.BusinessMobileNo;
                string BusinessMerchantName = BusinessMerchantDetails.BusinessName;
                string BusinessMerchantCountry = Common.Common.GetCountryName(BusinessMerchantDetails.BusinessOperationCountryCode);
                string ReceiveAmounAmountInLocalCurrency = AmountReceived + " " + ReceiverCurremcy;
                string ExchangeRate = exchangeRate;
                string PaymentReference = FaxerSession.PaymentRefrence;
                string AmountPaided = AmountPaid + " " + FaxerCurrency;
                return RedirectToAction("MerchantHurrayMessage", new { BusinessMFCode, BusinessMerchantName, BusinessMerchantCountry, AmountPaided, ReceiveAmounAmountInLocalCurrency, PaymentReference, ExchangeRate });

            }
            else
            {
                return View(vm);
            }

        }
        public ActionResult MerchantHurrayMessage(string BusinessMFCode, string BusinessMerchantName, string BusinessMerchantCountry, string AmountPaided, string ReceiveAmounAmountInLocalCurrency, string PaymentReference, string ExchangeRate)
        {
            Session.Remove("TransactionSummaryUrl");
            Session.Remove("CreditDebitDetails");
            Session.Remove("FaxingAmountSummary");

            ViewBag.BusinessMFCode = BusinessMFCode;
            ViewBag.BusinessMerchantName = BusinessMerchantName;
            ViewBag.BusinessMerchantCountry = BusinessMerchantCountry;
            ViewBag.AmountPaid = AmountPaided;
            ViewBag.AmountInLocalCurrency = ReceiveAmounAmountInLocalCurrency;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.PaymentReference = PaymentReference;
            ViewBag.ReceiveOption = "MFBC Card Withdrawl";
            return View();
        }

        [HttpGet]
        public ActionResult MerchantPaymentUsingSavedCreditDebitCard(int savedCardId = 0)
        {
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry);
            //List<SavedCard> list = db.SavedCard.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id).OrderBy(x => x.CardName).ToList();

            var list = Common.Common.GetSavedCardDetails();
            if (list.Count() == 0)
            {

                @TempData["CardCount"] = list.Count();
                FaxerSession.FromUrl = "/CardPayment/MerchantPaymentUsingSavedCreditDebitCard";
                return RedirectToAction("MerchantPaymentMethod", "PayForGoodsAndServicesAbroad");
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
                vm = (from c in db.SavedCard.Where(x => x.Id == savedCardId).ToList()
                      select new PaymentUsingSavedCreditDebitCardVm()
                      {
                          NameOnCard = c.CardName.Decrypt(),
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

            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");

            #region Auto Payment has been setuped or not 
            var AutoPaymenthasBeenSetuped = db.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == Common.FaxerSession.LoggedUser.Id && x.KiiPayBusinessInformationId == Common.FaxerSession.BusinessInformationId).FirstOrDefault();
            if ((AutoPaymenthasBeenSetuped != null) && AutoPaymenthasBeenSetuped.EnableAutoPayment == true)
            {

                ViewBag.AutoPayment = 1;
            }
            else
            {
                ViewBag.AutoPayment = 0;
            }
            #endregion
            return View(vm);

        }
        [HttpPost]
        public ActionResult MerchantPaymentUsingSavedCreditDebitCard([Bind(Include = PaymentUsingSavedCreditDebitCardVm.BindProperty)]PaymentUsingSavedCreditDebitCardVm vm)
        {

            // Countries DropDown List 
            ViewBag.Countries = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");

            #region Auto Payment has been setuped or not 
            var AutoPaymenthasBeenSetuped = db.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == Common.FaxerSession.LoggedUser.Id && x.KiiPayBusinessInformationId == Common.FaxerSession.BusinessInformationId).FirstOrDefault();
            if ((AutoPaymenthasBeenSetuped != null) && AutoPaymenthasBeenSetuped.EnableAutoPayment == true)
            {

                ViewBag.AutoPayment = 1;
            }
            else
            {
                ViewBag.AutoPayment = 0;
            }
            #endregion
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry);
            //List<SavedCard> list = db.SavedCard.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id).OrderBy(x => x.CardName).ToList();

            var list = Common.Common.GetSavedCardDetails();
            //foreach (var item in list)
            //{
            //    item.CardName = Common.Common.Decrypt(item.CardName);
            //}
            ViewBag.SavedCard = new SelectList(list, "Id", "CardNo");
            //string CountryCode = db.Country.Where(x => x.CountryName.ToLower() == vm.Country.ToLower()).FirstOrDefault().CountryCode;
            //var Country = db.Country.Where(x => x.CountryName.ToLower() == vm.Country.ToLower()).FirstOrDefault();
            //if (Country == null)
            //{
            //    ModelState.AddModelError("Country", "County Name Is Not Matched!!");
            //    return View(vm);
            //}
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

                    ModelState.AddModelError("EndYear", "Please enter a valid month");
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

                ModelState.AddModelError("NameOnCard", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.CardNumber))
            {

                ModelState.AddModelError("CardNumber", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.EndMonth))
            {

                ModelState.AddModelError("EndMonth", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.EndYear))
            {

                ModelState.AddModelError("EndYear", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.SecurityCode))
            {

                ModelState.AddModelError("SecurityCode", "The field is required");
                valid = false;
            }

            if (string.IsNullOrEmpty(vm.Address1))
            {

                ModelState.AddModelError("Address1", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.City))
            {

                ModelState.AddModelError("City", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.Country))
            {

                ModelState.AddModelError("Country", "The field is required");
                valid = false;
            }
            if (string.IsNullOrEmpty(vm.PostalCode))
            {

                ModelState.AddModelError("PostalCode", "The field is required");
                valid = false;
            }
            if (!string.IsNullOrEmpty(vm.Address1) && vm.Address1.Trim().ToLower() != faInformation.Address1.Trim().ToLower())
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

            if (!string.IsNullOrEmpty(vm.PostalCode) && vm.PostalCode.Trim() != faInformation.PostalCode.Trim())
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

                return RedirectToAction("MerchantPaymentUsingSavedCreditDebitCardTransactionSummary");
            }

            else
            {
                return View(vm);
            }

        }
        [HttpGet]
        public ActionResult MerchantPaymentUsingSavedCreditDebitCardTransactionSummary()
        {

            var FaxerDetails = db.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            var FaxerCountry = Common.Common.GetCountryName(FaxerDetails.Country);
            var faxerCountryCode = Common.Common.GetCountryPhoneCode(FaxerDetails.Country);
            int BuinessId = FaxerSession.BusinessInformationId;
            var BusinessInfo = db.KiiPayBusinessInformation.Where(x => x.Id == BuinessId).FirstOrDefault();
            string BusinessCurrency = Common.Common.GetCountryCurrency(BusinessInfo.BusinessOperationCountryCode);
            string FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            Models.MerchantPaymentTransactionSummaryViewModel model = new MerchantPaymentTransactionSummaryViewModel()
            {
                SavedCardId = int.Parse(Common.FaxerSession.SavedCreditDebitCardDetails.SavedCard),
                BusinessMerchantName = BusinessInfo.BusinessName,
                BusinessMFCode = BusinessInfo.BusinessMobileNo,
                ReceiveOption = "MFBC Card Withdrawal",
                FaxerName = Common.FaxerSession.LoggedUser.FullName,
                FaxerEmail = Common.FaxerSession.LoggedUser.UserName,
                FaxerPhoneNumber = faxerCountryCode + " " + FaxerDetails.PhoneNumber,
                CardNumber = Common.FaxerSession.SavedCreditDebitCardDetails.CardNumber.FormatSavedCardNumber(),
                CardExpriyDate = Common.FaxerSession.SavedCreditDebitCardDetails.EndMonth + "/" + Common.FaxerSession.SavedCreditDebitCardDetails.EndYear,
                CountryOfBirth = FaxerCountry,
                streetAddress = Common.FaxerSession.SavedCreditDebitCardDetails.Address1,
                City = Common.FaxerSession.SavedCreditDebitCardDetails.City,
                PostalCode = Common.FaxerSession.SavedCreditDebitCardDetails.PostalCode,
                SentAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount.ToString() + " " + FaxerCurrency,
                Fees = Common.FaxerSession.FaxingAmountSummary.FaxingFee.ToString() + " " + FaxerCurrency,
                TotalAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount.ToString() + " " + FaxerCurrency,
                TotalReceiveAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount.ToString() + " " + BusinessCurrency,
                PaymentReference = Common.FaxerSession.PaymentRefrence

            };
            Common.FaxerSession.TransactionSummaryUrl = "/CardPayment/MerchantPaymentUsingSavedCreditDebitCardTransactionSummary";
            return View(model);

        }
        [HttpPost]
        public ActionResult MerchantPaymentUsingSavedCreditDebitCardTransactionSummary([Bind(Include = MerchantPaymentTransactionSummaryViewModel.BindProperty)]MerchantPaymentTransactionSummaryViewModel vm)
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
                DB.SenderKiiPayBusinessPaymentInformation obj = new DB.SenderKiiPayBusinessPaymentInformation()
                {
                    KiiPayBusinessInformationId = FaxerSession.BusinessInformationId,
                    SenderInformationId = FaxerSession.LoggedUser.Id,
                    PaymentAmount = FaxerSession.FaxingAmountSummary.FaxingAmount,
                    PaymentRefrence = FaxerSession.PaymentRefrence,
                };
                SFaxerMerchantPaymentInformation service = new SFaxerMerchantPaymentInformation();
                obj = service.SaveTransaction(obj, FaxerSession.BusinessInformationId);

                // Send Email for Confirmation Of Merchant Payment 
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                MailCommon mail = new MailCommon();
                string body = "";

                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                var BusinessMerchantDetails = service.GetBusinessMerchantDetials(obj.KiiPayBusinessInformationId);
                string BusinessEmail = service.GetBusinessEmail(obj.KiiPayBusinessInformationId);
                string PayForGoodsAbroad = baseUrl + "/PayForGoodsAndServicesAbroad/MerchantDetails?MerchantACNumber=" + BusinessMerchantDetails.BusinessMobileNo;
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMerchantPaymentWithReceiptFaxer?FaxerName="
                    + FaxerName + "&MerchantBusinessName=" + BusinessMerchantDetails.BusinessName + "&PayForGoodsAbroad=" + PayForGoodsAbroad);

                // Generate a receipt PDF

                string fee = Common.FaxerSession.FaxingAmountSummary.FaxingFee.ToString();
                string AmountPaid = Common.FaxerSession.FaxingAmountSummary.TotalAmount.ToString();
                string AmountReceived = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount.ToString();
                string exchangeRate = Common.FaxerSession.FaxingAmountSummary.ExchangeRate.ToString();
                string transactionDate = DateTime.Now.ToString("dd/MM/yyyy");
                string transactionTime = DateTime.Now.ToString("HH:mm");
                string FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
                string ReceiverCurremcy = Common.Common.GetCountryCurrency(BusinessMerchantDetails.BusinessOperationCountryCode);
                string ReceiverPhonecode = Common.Common.GetCountryPhoneCode(BusinessMerchantDetails.BusinessOperationCountryCode);
                string ReceiptNumber = FaxerSession.PayGoodsAndServicesReceiptNumber;
                var ReceiptURL = baseUrl + "/EmailTemplate/PayGoodsAndServicesFaxerReceipt?ReceiptNumber=" + ReceiptNumber +
                     "&Date=" + transactionDate + "&Time=" + transactionTime + "&FaxerFullName=" + FaxerName + "&FaxerCountry=" + Common.Common.GetCountryName(Common.FaxerSession.LoggedUser.CountryCode)
                     + "&BusinessMerchantName=" + BusinessMerchantDetails.BusinessName
                     + "&BusinessMFCode=" + BusinessMerchantDetails.BusinessMobileNo + "&BusinessCountry=" + Common.Common.GetCountryName(BusinessMerchantDetails.BusinessOperationCountryCode) + "&BusinessCity=" + BusinessMerchantDetails.BusinessOperationCity +
                      "&AmountPaid=" + AmountPaid + "&ExchangeRate=" + exchangeRate +
                     "&AmountInLocalCurrency=" + AmountReceived + " " + "&Fee=" + fee + "&FaxerCurrency=" + FaxerCurrency + "&ReceiverCurrreny=" + ReceiverCurremcy;

                var receipt = Common.Common.GetPdf(ReceiptURL);

                // receipt end 
                // send Email With receipt attachment 

                mail.SendMail(FaxerEmail, "Confirmation of Payment to a Merchant", body, receipt);

                // End 
                string body2 = "";
                //Send Email for Confirmation of Payment to a Merchant 
                body2 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMerchantPaymentMerchant?BusinessMerchantName="
                    + BusinessMerchantDetails.BusinessName + "&FaxerName=" + FaxerName);
                mail.SendMail(BusinessEmail, "Confirmation of Payment to a Merchant", body2);

                // End 



                #region Send  Business Payment SMS 

                SmsApi smsApiServices = new SmsApi();
                string receivingAmount = Common.Common.GetCurrencySymbol(BusinessMerchantDetails.BusinessOperationCountryCode) + AmountReceived;
                string Amount = Common.Common.GetCurrencySymbol(faInformation.Country) + AmountPaid;
                string message = smsApiServices.GetBusinessPaymentMessage(FaxerName, BusinessMerchantDetails.BusinessMobileNo, BusinessMerchantDetails.BusinessName,
                                                                  Amount, obj.PaymentRefrence, receivingAmount);

                string PhoneNo = Common.Common.GetCountryPhoneCode(faInformation.Country) + "" + faInformation.PhoneNumber;
                smsApiServices.SendSMS(PhoneNo, message);

                string ReceiverPhoneNo = Common.Common.GetCountryPhoneCode(BusinessMerchantDetails.BusinessOperationCountryCode) + "" + BusinessMerchantDetails.PhoneNumber;
                smsApiServices.SendSMS(ReceiverPhoneNo, message);

                #endregion
                //if (Common.FaxerSession.CreditDebitDetails.SaveCard)
                //{

                //    bool IsSaved = false;
                //    string cardNumber = Common.FaxerSession.SavedCreditDebitCardDetails.CardNumber.Encrypt();
                //    var SavedCardCount = db.SavedCard.Where(x => x.Num == cardNumber).FirstOrDefault();
                //    if (SavedCardCount != null)
                //    {
                //        IsSaved = true;
                //    }
                //    DB.CardTopUpCreditDebitInformation cardDetails = new DB.CardTopUpCreditDebitInformation()
                //    {
                //        NonCardTransactionId = obj.Id,
                //        NameOnCard = Common.FaxerSession.SavedCreditDebitCardDetails.NameOnCard,
                //        ExpiryDate = Common.FaxerSession.SavedCreditDebitCardDetails.EndMonth + "/" + Common.FaxerSession.SavedCreditDebitCardDetails.EndYear,
                //        IsSavedCard = IsSaved,
                //        CardNumber = "xxxx-xxxx-xxxx-" + Common.FaxerSession.SavedCreditDebitCardDetails.CardNumber.Right(4),
                //        AutoRecharged = Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUp,
                //    };
                //    SSavedCard cardInformationservices = new SSavedCard();
                //    cardDetails = cardInformationservices.Save(cardDetails);

                //}

                if ((Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUp == true)
                    && Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUpAmount > 0 && (int)Common.FaxerSession.SavedCreditDebitCardDetails.AutoPaymentFrequency > 0)
                {
                    var cardDetails = db.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                    if (cardDetails != null)
                    {

                        var MFBCCard = db.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == obj.KiiPayBusinessInformationId).FirstOrDefault();
                        MFBCCard.AutoTopUp = true;
                        db.Entry(MFBCCard).State = System.Data.Entity.EntityState.Modified;
                        var PaymentInformation = db.FaxerMerchantPaymentInformation.Where(x => x.KiiPayBusinessInformationId == obj.KiiPayBusinessInformationId && x.SenderInformationId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                        PaymentInformation.AutoPaymentAmount = Common.FaxerSession.SavedCreditDebitCardDetails.AutoTopUpAmount;
                        PaymentInformation.AutoPaymentFrequency = Common.FaxerSession.SavedCreditDebitCardDetails.AutoPaymentFrequency;
                        PaymentInformation.EnableAutoPayment = true;
                        PaymentInformation.FrequencyDetails = Common.FaxerSession.SavedCreditDebitCardDetails.PaymentDay;
                        PaymentInformation.PaymentRefrence = Common.FaxerSession.PaymentRefrence;
                        db.Entry(PaymentInformation).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        string bodyAutoPayment = "";
                        string FaxerCountry = db.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).Select(x => x.Country).FirstOrDefault();
                        string CountryCurrency = Common.Common.GetCountryCurrency(FaxerCountry);

                        string CardNumber = "xxxx-xxxx-xxxx-" + cardDetails.Num.Decrypt().Right(4);
                        string SetAutoPaymentLink = baseUrl + "/FaxerAutoPayments/MerchantAutoPayments?faxMerchantPayInfoId=" + obj.Id;
                        string PayforGoodsBusinessMerchant = baseUrl + "/PayForGoodsAndServicesAbroad/MerchantDetails?MerchantACNumber=" + BusinessMerchantDetails.BusinessMobileNo;
                        bodyAutoPayment = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfAutoPaymentSetupFaxer/?FaxerName=" +
                            FaxerName + "&AutoPaymentAmount=" + PaymentInformation.AutoPaymentAmount + "&CountryCurrencySymbol=" + CountryCurrency
                            + "&BusinessMerchantName=" + BusinessMerchantDetails.BusinessName + "&AutoPaymentFrequency=" + PaymentInformation.AutoPaymentFrequency
                            + "&CreditORDebitCardlast4digits=" + CardNumber
                            + "&SetAutoPayment=" + SetAutoPaymentLink +
                            "&PayforGoodsBusinessMerchant=" + PayforGoodsBusinessMerchant);
                        mail.SendMail(FaxerEmail, "Confirmation of Auto Payment Setup", bodyAutoPayment);
                    }
                }

                string BusinessMFCode = BusinessMerchantDetails.BusinessMobileNo;
                string BusinessMerchantName = BusinessMerchantDetails.BusinessName;
                string BusinessMerchantCountry = Common.Common.GetCountryName(BusinessMerchantDetails.BusinessOperationCountryCode);
                string ReceiveAmounAmountInLocalCurrency = AmountReceived + " " + ReceiverCurremcy;
                string ExchangeRate = exchangeRate;
                string PaymentReference = FaxerSession.PaymentRefrence;
                string AmountPaided = AmountPaid + " " + FaxerCurrency;
                return RedirectToAction("MerchantHurrayMessage", new { BusinessMFCode, BusinessMerchantName, BusinessMerchantCountry, AmountPaided, ReceiveAmounAmountInLocalCurrency, PaymentReference, ExchangeRate });

            }
            else
            {
                return View(vm);
            }

        }




    }
}