using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Businesses.Services;
using FAXER.PORTAL.Areas.Businesses.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Controllers;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class NoCardPaymentByMerchantController : Controller
    {
        DB.FAXEREntities context = new DB.FAXEREntities();
        NonCardPaymentByMerchantServices _noncardPaymentServices = new NonCardPaymentByMerchantServices();
        MFTCCardPaymentByMerchantServices _mFTCCardPaymentByMerchantServices = new MFTCCardPaymentByMerchantServices();
        Services.CommonServices _commonServices = new Services.CommonServices();
        int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant == null ? 0 : Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
        // GET: Businesses/NoCardPaymentByMerchant
        [HttpGet]
        public ActionResult Index()
        {
            NonCardPaymentViewModel vm = new NonCardPaymentViewModel();
            var countries = _noncardPaymentServices.getCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            Services.BusinessCardServices cardServices = new Services.BusinessCardServices();
            ViewBag.CreditonCard = cardServices.GetCreditOnCard() + " " + Common.Common.GetCountryCurrency(Common.BusinessSession.LoggedBusinessMerchant.CountryCode);

            if (Common.BusinessSession.NonCardPaymentViewModel != null)
            {

                vm = Common.BusinessSession.NonCardPaymentViewModel;
                return View(vm);
            }
            vm.ReceivingCountry = Common.BusinessSession.LoggedBusinessMerchant.CountryCode;
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = NonCardPaymentViewModel.BindProperty)]NonCardPaymentViewModel model)
        {
            var countries = _noncardPaymentServices.getCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (string.IsNullOrEmpty(model.ReceivingCountry))
            {
                ModelState.AddModelError("ReceivingCountry", "please select receiving country");
            }
            else if (model.TopUpAmount == 0 && model.ReceivingAmount == 0)
            {
                ModelState.AddModelError("TopUpAmount", "Please enter an amount to proceed");
            }
            else if (model.ReceivingCountry == Common.BusinessSession.LoggedBusinessMerchant.CountryCode)
            {


                Common.BusinessSession.NonCardPaymentViewModel = model;
                Common.BusinessSession.FaxingCountry = Common.BusinessSession.LoggedBusinessMerchant.CountryCode;
                Common.BusinessSession.ReceivingCountry = model.ReceivingCountry;
                return RedirectToAction("ReceiverDetails", "NonCardNationalPaymentByMerchant");
            }

            else
            {
                Common.BusinessSession.ReceivingCountry = model.ReceivingCountry;
                Common.BusinessSession.ReceivingCurrency = Common.Common.GetCountryCurrency(model.ReceivingCountry);
                Common.BusinessSession.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(model.ReceivingCountry);
                string FaxingCountryCode = Common.BusinessSession.FaxingCountry;
                string ReceivingCountryCode = Common.BusinessSession.ReceivingCountry;
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
                    model.TopUpAmount = model.ReceivingAmount;
                }
                var feeSummary = SEstimateFee.CalculateFaxingFee(model.TopUpAmount, model.IncludingFee, model.ReceivingAmount > 0, exchangeRate, SEstimateFee.GetFaxingCommision(FaxingCountryCode));
                Common.BusinessSession.FaxingAmountSummary = feeSummary;
                Common.BusinessSession.NonCardPaymentViewModel = model;
                return RedirectToAction("SendingAmountDetails");
            }
            return View(model);
        }

        public ActionResult SendingAmountDetails()
        {
            decimal CurrentBalanceOnCard = _commonServices.GetCurrentBalanceOnCard();

            EstimateFaxingFeeSummary faxingSummarry = new EstimateFaxingFeeSummary();
            faxingSummarry = Common.BusinessSession.FaxingAmountSummary;
            if (faxingSummarry.TotalAmount > CurrentBalanceOnCard)
            {

                TempData["InSufficientBalance"] = "Insufficient balance on card";
                return RedirectToAction("Index");
            }
            TopUpDetailsViewModel model = new TopUpDetailsViewModel()
            {
                FaxingFee = faxingSummarry.FaxingFee,
                AmountToBeReceived = faxingSummarry.ReceivingAmount,
                CurrentExchangeRate = faxingSummarry.ExchangeRate,
                faxingAmount = faxingSummarry.FaxingAmount,
                TotalAmountIncludingFee = faxingSummarry.TotalAmount,

            };
            return View(model);
            
        }

        public ActionResult ReceiverDetails(int id = 0)
        {
            if (Common.BusinessSession.NonCardReceiverId_merchantNonCard > 0)
            {
                id = Common.BusinessSession.NonCardReceiverId_merchantNonCard;
            }
            var countries = _noncardPaymentServices.getCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            string receivingCountryCode = Common.BusinessSession.ReceivingCountry;
            ViewBag.RecvingCountry = context.Country.Where(x => x.CountryCode.Equals(receivingCountryCode)).Select(x => x.CountryName).FirstOrDefault();

            var ReceiverList = (from c in context.MerchantNonCardReceiverDetail.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList()
                                select new PreviousReceiversDropDown()
                                {
                                    Id = c.Id,
                                    FullName = c.FirstName + " " + c.MiddleName + " " + c.LastName

                                }).ToList();
            //ViewBag.PreviousReceivers = new SelectList(dbContext.ReceiversDetails.Where(x => x.FaxerID == FaxerSession.LoggedUser.Id).OrderBy(x => x.FirstName), "ID", "FirstName");
            ViewBag.PreviousReceivers = new SelectList(ReceiverList, "Id", "FullName");
            ReceiverDetailsViewModel model = new ReceiverDetailsViewModel();
            model.CountryPhoneCode = Common.Common.GetCountryPhoneCode(receivingCountryCode);
            model.ReceiverCountry = Common.Common.GetCountryName(Common.BusinessSession.ReceivingCountry);
            if ((Common.BusinessSession.ReceiverDetailsViewModel != null) && int.Parse(Common.BusinessSession.ReceiverDetailsViewModel.PreviousReceivers) == id)
            {

                model = Common.BusinessSession.ReceiverDetailsViewModel;
                return View(model);
            }
            if (id > 0)
            {

                var data = (from c in context.MerchantNonCardReceiverDetail.Where(x => x.Id == id).ToList()
                            select new ReceiverDetailsViewModel()
                            {
                                ReceiverFirstName = c.FirstName,
                                ReceiverMiddleName = c.MiddleName,
                                ReceiverLastName = c.LastName,
                                ReceiverCity = c.City,
                                ReceiverCountry = c.Country,
                                ReceiverPhoneNumber = c.PhoneNumber,
                                ReceiverEmailAddress = c.EmailAddress,
                                CountryPhoneCode = Common.Common.GetCountryPhoneCode(c.Country),
                                PreviousReceivers = c.Id.ToString()
                            }).FirstOrDefault();
                if (data.ReceiverCountry != receivingCountryCode)
                {
                    ModelState.AddModelError("PreviousReceivers", "Receiver and Receiving countries do not match");
                    return View(model);
                }
                Common.BusinessSession.NonCardReceiverId_merchantNonCard = id;
                return View(data);
            }
            else
            {
                model.PreviousReceivers = "0";

            }
            return View(model);
            return View();

        }
        [HttpPost]
        public ActionResult ReceiverDetails([Bind(Include = ReceiverDetailsViewModel.BindProperty)]ReceiverDetailsViewModel model)
        {
            var countries = _noncardPaymentServices.getCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            string receivingCountryCode = Common.BusinessSession.ReceivingCountry;
            ViewBag.RecvingCountry = context.Country.Where(x => x.CountryCode.Equals(receivingCountryCode)).Select(x => x.CountryName).FirstOrDefault();
            var ReceiverList = (from c in context.MerchantNonCardReceiverDetail.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList()
                                select new PreviousReceiversDropDown()
                                {
                                    Id = c.Id,
                                    FullName = c.FirstName + " " + c.MiddleName + " " + c.LastName

                                }).ToList();
            //ViewBag.PreviousReceivers = new SelectList(dbContext.ReceiversDetails.Where(x => x.FaxerID == FaxerSession.LoggedUser.Id).OrderBy(x => x.FirstName), "ID", "FirstName");
            ViewBag.PreviousReceivers = new SelectList(ReceiverList, "Id", "FullName");


            if (ModelState.IsValid)
            {
                if (model.PreviousReceivers == null)
                {
                    model.PreviousReceivers = "0";
                    Common.BusinessSession.NonCardReceiverId_merchantNonCard = 0;
                    var EmailExist = context.MerchantNonCardReceiverDetail.Where(x => x.EmailAddress == model.ReceiverEmailAddress).FirstOrDefault();
                    if (EmailExist != null)
                    {
                        ModelState.AddModelError("ReceiverEmailAddress", "This Receiver's email address is already registered in the system, please either use a different email address or select Receiver's information from the list. ");
                        return View(model);

                    }
                }
                Common.BusinessSession.ReceiverDetailsViewModel = model;
                Common.BusinessSession.ReceivingCurrency = Common.Common.GetCountryCurrency(Common.BusinessSession.ReceivingCountry);
                Common.BusinessSession.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(Common.BusinessSession.ReceivingCountry);

                if (!string.IsNullOrEmpty(Common.BusinessSession.TransactionSummaryURL))
                {

                    return Redirect(Common.BusinessSession.TransactionSummaryURL);

                }
                return RedirectToAction("FraudAlert");
            }

            return View(model);
        }
        public ActionResult FraudAlert()
        {

            return View();
        }

        [HttpGet]
        public ActionResult NoCardPaymentTransactionSummary()
        {
            NoCardPaymentTransactionSummaryViewModel vm = new NoCardPaymentTransactionSummaryViewModel();

            #region Receiver Detials 
            var receiverDetails = Common.BusinessSession.ReceiverDetailsViewModel;
            vm.ReceiverName = receiverDetails.ReceiverFirstName + " " + receiverDetails.ReceiverMiddleName + " " + receiverDetails.ReceiverLastName;
            vm.ReceiveOption = "Cash pick up";
            #endregion
            #region Sender MerchantDetails 
            var SenderDetails = _noncardPaymentServices.getSenderDetails();
            vm.MerchantName = SenderDetails.KiiPayBusinessInformation.BusinessName;
            vm.MerchantEmail = SenderDetails.KiiPayBusinessInformation.Email;
            vm.MerchantPhoneNumber = Common.Common.GetCountryPhoneCode(SenderDetails.KiiPayBusinessInformation.BusinessOperationCountryCode) + " " + SenderDetails.KiiPayBusinessInformation.PhoneNumber;
            vm.CountryOfBirth = Common.Common.GetCountryName(SenderDetails.KiiPayBusinessInformation.BusinessOperationCountryCode);
            vm.streetAddress = SenderDetails.KiiPayBusinessInformation.BusinessOperationAddress1;
            vm.City = SenderDetails.KiiPayBusinessInformation.BusinessOperationCity;
            vm.State = SenderDetails.KiiPayBusinessInformation.BusinessOperationState;
            vm.PostalCode = SenderDetails.KiiPayBusinessInformation.BusinessOperationPostalCode;
            vm.SenderMFTCCardNumber = SenderDetails.MobileNo.Decrypt().FormatMFBCCard();
            #endregion

            #region Payment Information 
            var paymentDetails = Common.BusinessSession.FaxingAmountSummary;
            vm.SentAmount = paymentDetails.FaxingAmount.ToString();
            vm.Fees = paymentDetails.FaxingFee.ToString();
            vm.TotalAmount = paymentDetails.TotalAmount.ToString();
            vm.TotalReceiveAmount = paymentDetails.ReceivingAmount.ToString();

            #endregion

            Common.BusinessSession.TransactionSummaryURL = "/Businesses/NoCardPaymentByMerchant/NoCardPaymentTransactionSummary";


            return View(vm);
        }

        [HttpPost]
        public ActionResult NoCardPaymentTransactionSummary([Bind(Include = NoCardPaymentTransactionSummaryViewModel.BindProperty)]NoCardPaymentTransactionSummaryViewModel vm)
        {

            var receiverDetails = Common.BusinessSession.ReceiverDetailsViewModel;
            int receiverID = 0;
            DB.MerchantNonCardReceiverDetails newreceiver = new DB.MerchantNonCardReceiverDetails()
            {
                KiiPayBusinessInformationId = KiiPayBusinessInformationId,
                FirstName = receiverDetails.ReceiverFirstName,
                MiddleName = receiverDetails.ReceiverMiddleName,
                LastName = receiverDetails.ReceiverLastName,
                City = receiverDetails.ReceiverCity,
                Country = Common.BusinessSession.ReceivingCountry,
                EmailAddress = receiverDetails.ReceiverEmailAddress,
                PhoneNumber = receiverDetails.ReceiverPhoneNumber,
                IsDeleted = false,
                CreatedDate = DateTime.Now
            };
            if (Common.BusinessSession.NonCardReceiverId_merchantNonCard > 0)
            {
                receiverID = Common.BusinessSession.NonCardReceiverId_merchantNonCard;

            }
            else
            {

                var result = _noncardPaymentServices.SaveReceiverDetails(newreceiver);
                receiverID = result.Id;
            }

            #region Non Card Transaction 
            var PaymentDetails = Common.BusinessSession.FaxingAmountSummary;
            string MFCNNumber = _noncardPaymentServices.getMFCN();
            var SenderMFBCDetails = _noncardPaymentServices.getSenderDetails();
            DB.MerchantNonCardTransaction transaction = new DB.MerchantNonCardTransaction()
            {
                KiiPayBusinessInformationId = KiiPayBusinessInformationId,
                NonCardRecieverId = receiverID,
                MFBCCardID = SenderMFBCDetails.Id,
                ExchangeRate = PaymentDetails.ExchangeRate,
                FaxingAmount = PaymentDetails.FaxingAmount,
                FaxingFee = PaymentDetails.FaxingFee,
                FaxingStatus = DB.FaxingStatus.NotReceived,
                TotalAmount = PaymentDetails.TotalAmount,
                MFCN = MFCNNumber,
                ReceivingAmount = PaymentDetails.ReceivingAmount,
                TransactionDate = DateTime.Now,
                ReceiptNumber = _commonServices.GetNonCardPaymentReceiptNumberToSave(),
                PaymentType = TopUpType.International
            };
            var saveTrans = _noncardPaymentServices.saveTransaction(transaction);

            var DeductCreditOnCard = _commonServices.DeductTheCreditOnCard(transaction.MFBCCardID, transaction.TotalAmount);
            #endregion


            var NonCardReceiverDetials = _commonServices.GetNonCardReceiverDetails(transaction.NonCardRecieverId);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string body_Sender = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyTransferEmailToSender?CardUserName" +
                SenderMFBCDetails.FirstName + "" + SenderMFBCDetails.MiddleName + " " + SenderMFBCDetails.LastName +
                "&ReceiverName=" + NonCardReceiverDetials.FirstName + " " + NonCardReceiverDetials.MiddleName + " " + NonCardReceiverDetials.LastName
                + "&ReceiverCountry=" + Common.Common.GetCountryName(NonCardReceiverDetials.Country)
                + "&ReceiverCity=" + NonCardReceiverDetials.City + "&MFCN=" + transaction.MFCN + "&TransferredAmount=" + transaction.FaxingAmount + "&FaxingCurrency=" + Common.BusinessSession.FaxingCurrency);

            string URL = baseUrl + "/EmailTemplate/NonCardTransferReceipt?MFReceiptNumber=" + transaction.ReceiptNumber +
                              "&TransactionDate=" + transaction.TransactionDate.ToString("dd/MM/yyy") + "&TransactionTime=" + transaction.TransactionDate.ToString("HH:mm")
                                + "&FaxerFullName=" + SenderMFBCDetails.FirstName + " " + SenderMFBCDetails.MiddleName + " " + SenderMFBCDetails.LastName +
                              "&MFCN=" + transaction.MFCN + "&ReceiverFullName=" + NonCardReceiverDetials.FirstName + " " + NonCardReceiverDetials.MiddleName + " " + NonCardReceiverDetials.LastName
                              + "&Telephone=" + Common.Common.GetCountryPhoneCode(NonCardReceiverDetials.Country) + " " + NonCardReceiverDetials.PhoneNumber
                              + "&AmountSent=" + transaction.FaxingAmount
                              + "&ExchangeRate=" + transaction.ExchangeRate + "&Fee=" + transaction.FaxingFee
                              + "&AmountReceived=" + transaction.ReceivingAmount + "&SendingCurrency=" + Common.Common.GetCountryCurrency(SenderMFBCDetails.Country)
                              + "&ReceivingCurrency=" + Common.Common.GetCountryCurrency(NonCardReceiverDetials.Country)
                              + "&SenderTelephoneNo=" + Common.Common.GetCountryPhoneCode(SenderMFBCDetails.Country) + " " + SenderMFBCDetails.PhoneNumber; ;

            var Receipt = Common.Common.GetPdf(URL);


            mail.SendMail(SenderMFBCDetails.Email, "Confirmation of Money Transfer ", body_Sender, Receipt);




            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyTransfer?ReceiverCountry=" +
                Common.Common.GetCountryName(NonCardReceiverDetials.Country)
                + "&ReceiverName=" + NonCardReceiverDetials.FirstName + " " + NonCardReceiverDetials.MiddleName + " " + NonCardReceiverDetials.LastName
                + "&SenderCountry=" + Common.Common.GetCountryName(NonCardReceiverDetials.Business.BusinessOperationCountryCode));
            mail.SendMail(NonCardReceiverDetials.EmailAddress, "Confirmation of Money Transfer ", body);


            //Sms Function
            SmsApi smsApiServices = new SmsApi();
            string senderName = SenderMFBCDetails.FirstName + "" + SenderMFBCDetails.MiddleName + " " + SenderMFBCDetails.LastName;
            string smsMFCN = transaction.MFCN;
            string amount = Common.Common.GetCurrencySymbol(SenderMFBCDetails.Country) + transaction.FaxingAmount;
            string Fee = Common.Common.GetCurrencySymbol(SenderMFBCDetails.Country) + transaction.FaxingFee;
            string receivingAmount = Common.Common.GetCountryCurrency(NonCardReceiverDetials.Country) + transaction.ReceivingAmount;
            string message = smsApiServices.GetCashToCashTransferMessage(senderName, smsMFCN, amount, Fee, receivingAmount);
            string phoneNumber = Common.Common.GetCountryPhoneCode(SenderMFBCDetails.Country) + SenderMFBCDetails.PhoneNumber;
            smsApiServices.SendSMS(phoneNumber, message);
            return RedirectToAction("PaymentSuccessful", new { MFCN = saveTrans.MFCN });

            // return View(vm);
        }
        public ActionResult PaymentSuccessful(string MFCN)
        {
            var receiverDetails = Common.BusinessSession.ReceiverDetailsViewModel;
            var paymentDetials = Common.BusinessSession.FaxingAmountSummary;
            MerchantNonCardPaymentSuccessful vm = new MerchantNonCardPaymentSuccessful()
            {

                MFCN = MFCN,
                ExchangeRate = paymentDetials.ExchangeRate,
                SentAmount = paymentDetials.FaxingAmount,
                ReceivingAmount = paymentDetials.ReceivingAmount,
                ReceiverName = receiverDetails.ReceiverFirstName + " " + receiverDetails.ReceiverMiddleName + " " + receiverDetails.ReceiverLastName,
                ReceiverCountry = receiverDetails.ReceiverCountry,
                ReceiveOption = "Cash Collection from Agent"
            };
            Session.Remove("ReceiverDetailsViewModel");
            Session.Remove("FaxingAmountSummary");
            Session.Remove("TransactionSummaryURL");
            Session.Remove("NonCardReceiverId_merchantNonCard");

            return View(vm);
        }
    }
}