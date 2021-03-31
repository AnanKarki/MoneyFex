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
    public class NonCardPaymentByCardUserController : Controller
    {

        Services.CardUserCommonServices _cardUserCommonServices = null;
        Services.CardUser_NonCardPaymentServices _cardUser_NonCardPaymentServices = null;
        DB.FAXEREntities context = null;
        int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel == null ? 0 : Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
        public NonCardPaymentByCardUserController()
        {
            _cardUserCommonServices = new Services.CardUserCommonServices();
            context = new DB.FAXEREntities();
            _cardUser_NonCardPaymentServices = new Services.CardUser_NonCardPaymentServices();
        }
        // GET: CardUsers/NonCardPaymentByCardUser
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult PayingAmount()
        {
            CardUser_NonCardPayingAmountViewModel vm = new CardUser_NonCardPayingAmountViewModel();
            var countries = _cardUserCommonServices.getCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (Common.CardUserSession.CardUser_NonCardPayingAmountViewModel != null)
            {

                vm = Common.CardUserSession.CardUser_NonCardPayingAmountViewModel;
                return View(vm);
            }
            vm.ReceivingCountry = Common.CardUserSession.LoggedCardUserViewModel.Country;
            return View(vm);

        }

        [HttpPost]
        public ActionResult PayingAmount([Bind(Include = CardUser_NonCardPayingAmountViewModel.BindProperty)]CardUser_NonCardPayingAmountViewModel model)
        {

            var countries = _cardUserCommonServices.getCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (string.IsNullOrEmpty(model.ReceivingCountry))
            {
                ModelState.AddModelError("ReceivingCountry", "please select receiving country");
            }
            else if (model.TopUpAmount == 0 && model.ReceivingAmount == 0)
            {
                ModelState.AddModelError("TopUpAmount", "Please enter an amount to proceed");
            }
            else if (model.ReceivingCountry == Common.CardUserSession.LoggedCardUserViewModel.Country)
            {

                Common.CardUserSession.ReceivingCountry = model.ReceivingCountry;
                Common.CardUserSession.ReceivingCurrency = Common.Common.GetCountryCurrency(model.ReceivingCountry);
                Common.CardUserSession.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(model.ReceivingCountry);

                Common.CardUserSession.CardUser_NonCardPayingAmountViewModel = model;

                return RedirectToAction("ReceiverDetails", "NonCardNationalPaymentByCardUser");

            }
            else
            {
                Common.CardUserSession.ReceivingCountry = model.ReceivingCountry;
                Common.CardUserSession.ReceivingCurrency = Common.Common.GetCountryCurrency(model.ReceivingCountry);
                Common.CardUserSession.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(model.ReceivingCountry);
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
                    return View(model);
                }
                if (model.ReceivingAmount > 0)
                {
                    model.TopUpAmount = model.ReceivingAmount;
                }
                var feeSummary = SEstimateFee.CalculateFaxingFee(model.TopUpAmount, model.IncludingFee, model.ReceivingAmount > 0, exchangeRate, SEstimateFee.GetFaxingCommision(FaxingCountryCode));

                // Is Valid Amount to Transfer according to the withdrawal limit set by sender
                var ValidAmountAccordingToWithdrawalLimit = _cardUserCommonServices.ValidAmountAccordingToWithdrawalLimit(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId, feeSummary.TotalAmount);
                if (ValidAmountAccordingToWithdrawalLimit == false)
                {

                    ViewBag.ExchangeRate = "Sorry! your cannot transfer more amount than your withdrawal limit ";
                    return View(model);


                }
                Common.CardUserSession.FaxingAmountSummary = feeSummary;
                Common.CardUserSession.CardUser_NonCardPayingAmountViewModel = model;
                return RedirectToAction("PaymentDetails");
            }
            return View(model);
        }

        public JsonResult GetCountryCurrency(string CountryCode)
        {

            return Json(new
            {
                ReceiverCurrency = Common.Common.GetCountryCurrency(CountryCode),
                ReceiverCurrencySymbol = Common.Common.GetCurrencySymbol(CountryCode)
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult PaymentDetails()
        {
            decimal CurrentBalanceOnCard = _cardUserCommonServices.getCurrentBalanceOnCard(MFTCCardId);

            EstimateFaxingFeeSummary faxingSummarry = new EstimateFaxingFeeSummary();
            faxingSummarry = Common.CardUserSession.FaxingAmountSummary;
            if (faxingSummarry.TotalAmount > CurrentBalanceOnCard)
            {

                TempData["InSufficientBalance"] = "Insufficient balance on account";
                return RedirectToAction("PayingAmount");
            }
            CardUser_NonCardPaymentDetailsViewModel model = new CardUser_NonCardPaymentDetailsViewModel()
            {
                FaxingFee = faxingSummarry.FaxingFee,
                AmountToBeReceived = faxingSummarry.ReceivingAmount,
                CurrentExchangeRate = faxingSummarry.ExchangeRate,
                faxingAmount = faxingSummarry.FaxingAmount,
                TotalAmountIncludingFee = faxingSummarry.TotalAmount,

            };
            return View(model);

        }
        [HttpGet]
        public ActionResult ReceiverDetails(int id = 0)
        {

            var countries = _cardUserCommonServices.getCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            string receivingCountryCode = Common.CardUserSession.ReceivingCountry;
            ViewBag.RecvingCountry = context.Country.Where(x => x.CountryCode.Equals(receivingCountryCode)).Select(x => x.CountryName).FirstOrDefault();

            var ReceiverList = (from c in context.CardUserReceiverDetails.Where(x => x.MFTCCardInformationID == MFTCCardId).ToList()
                                select new CardUser_PreviousReceiversDropDown()
                                {
                                    Id = c.Id,
                                    FullName = c.FirstName + " " + c.MiddleName + " " + c.LastName

                                }).ToList();
            //ViewBag.PreviousReceivers = new SelectList(dbContext.ReceiversDetails.Where(x => x.FaxerID == FaxerSession.LoggedUser.Id).OrderBy(x => x.FirstName), "ID", "FirstName");
            ViewBag.PreviousReceivers = new SelectList(ReceiverList, "Id", "FullName");
            CardUser_ReceiverDetailsViewModel model = new CardUser_ReceiverDetailsViewModel();
            model.CountryPhoneCode = Common.Common.GetCountryPhoneCode(receivingCountryCode);
            model.ReceiverCountry = Common.Common.GetCountryName(Common.CardUserSession.ReceivingCountry);
            //if ((Common.CardUserSession.CardUser_ReceiverDetailsViewModel != null) && int.Parse(Common.CardUserSession.CardUser_ReceiverDetailsViewModel.PreviousReceivers) == id)
            //{

            //    model = Common.CardUserSession.CardUser_ReceiverDetailsViewModel;
            //    return View(model);
            //}
            if (id > 0)
            {

                var data = (from c in context.CardUserReceiverDetails.Where(x => x.Id == id).ToList()
                            select new CardUser_ReceiverDetailsViewModel()
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
                Common.CardUserSession.NonCardReceiverId = id;
                return View(data);
            }
            else
            {
                model.PreviousReceivers = "0";

            }
            return View(model);

        }
        [HttpPost]
        public ActionResult ReceiverDetails([Bind(Include = CardUser_ReceiverDetailsViewModel.BindProperty)]CardUser_ReceiverDetailsViewModel model)
        {


            var countries = _cardUserCommonServices.getCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            string receivingCountryCode = Common.CardUserSession.ReceivingCountry;
            ViewBag.RecvingCountry = context.Country.Where(x => x.CountryCode.Equals(receivingCountryCode)).Select(x => x.CountryName).FirstOrDefault();

            var ReceiverList = (from c in context.CardUserReceiverDetails.Where(x => x.MFTCCardInformationID == MFTCCardId).ToList()
                                select new CardUser_PreviousReceiversDropDown()
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
                    Common.CardUserSession.NonCardReceiverId = 0;
                    var EmailExist = context.CardUserReceiverDetails.Where(x => x.EmailAddress == model.ReceiverEmailAddress && x.MFTCCardInformationID == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault();
                    if (EmailExist != null)
                    {
                        ModelState.AddModelError("ReceiverEmailAddress", "This Receiver's email address is already registered in the system, please either use a different email address or select Receiver's information from the list. ");
                        return View(model);

                    }
                }
                Common.CardUserSession.CardUser_ReceiverDetailsViewModel = model;
                Common.CardUserSession.ReceivingCurrency = Common.Common.GetCountryCurrency(Common.CardUserSession.ReceivingCountry);
                Common.CardUserSession.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(Common.CardUserSession.ReceivingCountry);

                if (!string.IsNullOrEmpty(Common.CardUserSession.TransactionSummaryURL))
                {

                    return Redirect(Common.CardUserSession.TransactionSummaryURL);

                }
                return RedirectToAction("NoCardPaymentTransactionSummary");
            }

            return View(model);
        }
        public ActionResult FraudAlertProtection()
        {


            return View();
        }

        [HttpGet]
        public ActionResult NoCardPaymentTransactionSummary()
        {
            CardUser_NonCardPaymentTransactionSummaryViewModel vm = new CardUser_NonCardPaymentTransactionSummaryViewModel();

            #region Receiver Details 
            var receiverDetails = Common.CardUserSession.CardUser_ReceiverDetailsViewModel;
            vm.ReceiverName = receiverDetails.ReceiverFirstName + " " + receiverDetails.ReceiverMiddleName + " " + receiverDetails.ReceiverLastName;
            vm.ReceiverId = int.Parse(receiverDetails.PreviousReceivers);
            #endregion


            #region Sender Details 

            var senderDetails = _cardUserCommonServices.GetMFTCCardUserInformation(MFTCCardId);

            vm.CardUserName = senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName;
            vm.CardUserPhoneNo = senderDetails.CardUserTel;
            vm.CardUserEmail = senderDetails.CardUserEmail;
            vm.CardUserMFTCCardNumber = senderDetails.MobileNo.Decrypt().FormatMFTCCard();
            vm.streetAddress = senderDetails.Address1;
            vm.State = senderDetails.CardUserState;
            vm.City = senderDetails.CardUserCity;
            vm.CountryOfBirth = Common.Common.GetCountryName(senderDetails.CardUserCountry);
            #endregion

            #region Transaction Detials 

            var paymentDetails = Common.CardUserSession.FaxingAmountSummary;
            vm.SentAmount = paymentDetails.FaxingAmount.ToString();
            vm.TotalAmount = paymentDetails.TotalAmount.ToString();
            vm.TotalReceiveAmount = paymentDetails.ReceivingAmount.ToString();
            vm.Fees = paymentDetails.FaxingFee.ToString();
            #endregion
            Common.CardUserSession.TransactionSummaryURL = "/CardUsers/NonCardPaymentByCardUser/NoCardPaymentTransactionSummary";
            return View(vm);
        }

        public ActionResult NoCardPaymentTransactionSummary([Bind(Include = CardUser_NonCardPaymentTransactionSummaryViewModel.BindProperty)]CardUser_NonCardPaymentTransactionSummaryViewModel vm)
        {


            var receiverDetails = Common.CardUserSession.CardUser_ReceiverDetailsViewModel;
            int receiverID = 0;
            DB.CardUserReceiverDetails newreceiver = new DB.CardUserReceiverDetails()
            {
                MFTCCardInformationID = MFTCCardId,
                FirstName = receiverDetails.ReceiverFirstName,
                MiddleName = receiverDetails.ReceiverMiddleName,
                LastName = receiverDetails.ReceiverLastName,
                City = receiverDetails.ReceiverCity,
                Country = Common.CardUserSession.ReceivingCountry,
                EmailAddress = receiverDetails.ReceiverEmailAddress,
                PhoneNumber = receiverDetails.ReceiverPhoneNumber,
                IsDeleted = false,
                CreatedDate = DateTime.Now
            };
            if (Common.CardUserSession.NonCardReceiverId > 0)
            {
                receiverID = Common.CardUserSession.NonCardReceiverId;

            }
            else
            {

                var result = _cardUser_NonCardPaymentServices.SaveReceiverDetails(newreceiver);
                receiverID = result.Id;
            }

            #region Non Card Transaction 
            var PaymentDetails = Common.CardUserSession.FaxingAmountSummary;
            string MFCNNumber = _cardUserCommonServices.getMFCN();
            var SenderMFTCDetails = _cardUserCommonServices.GetMFTCCardUserInformation(MFTCCardId);
            DB.CardUserNonCardTransaction transaction = new DB.CardUserNonCardTransaction()
            {

                NonCardRecieverId = receiverID,
                MFTCCardId = SenderMFTCDetails.Id,
                ExchangeRate = PaymentDetails.ExchangeRate,
                FaxingAmount = PaymentDetails.FaxingAmount,
                FaxingFee = PaymentDetails.FaxingFee,
                FaxingStatus = DB.FaxingStatus.NotReceived,
                TotalAmount = PaymentDetails.TotalAmount,
                MFCN = MFCNNumber,
                ReceivingAmount = PaymentDetails.ReceivingAmount,
                TransactionDate = DateTime.Now,
                ReceiptNumber = _cardUserCommonServices.GetNewReceiptNumberToSave(),
                PaymentType = TopUpType.International
            };
            var saveTrans = _cardUser_NonCardPaymentServices.SaveTransaction(transaction);

            Common.Common.DeductCreditOnCard(transaction.MFTCCardId, transaction.TotalAmount);

            #endregion


            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);


            var ReceiverDetails = _cardUserCommonServices.GetReceiverDetails(saveTrans.NonCardRecieverId);

            string body_Sender = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyTransferEmailToSender?CardUserName" +
                SenderMFTCDetails.FirstName + "" + SenderMFTCDetails.MiddleName + " " + SenderMFTCDetails.LastName +
                "&ReceiverName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName
                + "&ReceiverCountry=" + Common.Common.GetCountryName(ReceiverDetails.Country)
                + "&ReceiverCity=" + ReceiverDetails.City + "&MFCN=" + saveTrans.MFCN + "&TransferredAmount=" + saveTrans.FaxingAmount + "&FaxingCurrency=" + Common.CardUserSession.FaxingCurrency);

            string URL = baseUrl + "/EmailTemplate/NonCardTransferReceipt?MFReceiptNumber=" + transaction.ReceiptNumber +
                           "&TransactionDate=" + saveTrans.TransactionDate.ToString("dd/MM/yyy") + "&TransactionTime=" + saveTrans.TransactionDate.ToString("HH:mm")
                             + "&FaxerFullName=" + SenderMFTCDetails.FirstName + " " + SenderMFTCDetails.MiddleName + " " + SenderMFTCDetails.LastName +
                           "&MFCN=" + transaction.MFCN + "&ReceiverFullName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName
                           + "&Telephone=" + Common.Common.GetCountryPhoneCode(ReceiverDetails.Country) + " " + ReceiverDetails.PhoneNumber
                           + "&AmountSent=" + saveTrans.FaxingAmount
                           + "&ExchangeRate=" + saveTrans.ExchangeRate + "&Fee=" + saveTrans.FaxingFee
                           + "&AmountReceived=" + saveTrans.ReceivingAmount + "&SendingCurrency=" + Common.Common.GetCountryCurrency(SenderMFTCDetails.CardUserCountry)
                           + "&ReceivingCurrency=" + Common.Common.GetCountryCurrency(ReceiverDetails.Country)
                           + "&SenderTelephoneNo=" + Common.Common.GetCountryPhoneCode(SenderMFTCDetails.CardUserCountry) + " " + SenderMFTCDetails.CardUserTel;

            var Receipt = Common.Common.GetPdf(URL);

            mail.SendMail(SenderMFTCDetails.CardUserEmail, "Confirmation of Money Transfer ", body_Sender, Receipt);

            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyTransfer?ReceiverCountry=" +
                Common.Common.GetCountryName(ReceiverDetails.Country)
                + "&ReceiverName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName
                + "&SenderCountry=" + Common.Common.GetCountryName(ReceiverDetails.MFTCCardInformation.CardUserCountry));
            mail.SendMail(ReceiverDetails.EmailAddress, "Confirmation of Money Transfer ", body);

            //Sms Function
            SmsApi smsApiServices = new SmsApi();
            string senderName = SenderMFTCDetails.FirstName + "" + SenderMFTCDetails.MiddleName + " " + SenderMFTCDetails.LastName;
            string MFCN = transaction.MFCN;
            string amount = Common.Common.GetCurrencySymbol(SenderMFTCDetails.CardUserCountry) + transaction.FaxingAmount;
            string fee = Common.Common.GetCurrencySymbol(SenderMFTCDetails.CardUserCountry) + transaction.FaxingFee;
            string receivingAmount = Common.Common.GetCurrencySymbol(ReceiverDetails.Country) + transaction.ReceivingAmount;
            string message = smsApiServices.GetCashToCashTransferMessage(senderName, MFCN, amount, fee, receivingAmount);
            string phoneNumber = Common.Common.GetCountryPhoneCode(SenderMFTCDetails.CardUserCountry) + SenderMFTCDetails.CardUserTel;
            smsApiServices.SendSMS(phoneNumber, message);

            // Check  Auto Top-Up is enable for The Logged In CardUser or Not 

            if (SenderMFTCDetails.CurrentBalance == 0)
            {

                _cardUserCommonServices.SendMailWhenBalanceISZero(SenderMFTCDetails.Id);
                if (SenderMFTCDetails.AutoTopUp == true)
                {

                    _cardUserCommonServices.AutoTopUp(SenderMFTCDetails.Id);
                }
            }


            return RedirectToAction("PaymentSuccessful", new { MFCN = saveTrans.MFCN });



        }

        public ActionResult PaymentSuccessful(string MFCN = "")
        {
            var PaymentDetails = Common.CardUserSession.FaxingAmountSummary;
            var receiverDetails = Common.CardUserSession.CardUser_ReceiverDetailsViewModel;
            CardUser_NonCardPaymentSuccessfulViewModel vm = new CardUser_NonCardPaymentSuccessfulViewModel()
            {
                ReceiverName = receiverDetails.ReceiverFirstName + " " + receiverDetails.ReceiverMiddleName + " " + receiverDetails.ReceiverLastName,
                ReceiveAmount = PaymentDetails.ReceivingAmount.ToString(),
                SentAmount = PaymentDetails.FaxingAmount.ToString(),
                MFCNNumber = MFCN,
                ExchangeRate = PaymentDetails.ExchangeRate.ToString(),
                ReceiverCountry = receiverDetails.ReceiverCountry
            };

            Session.Remove("CardUser_ReceiverDetailsViewModel");
            Session.Remove("FaxingAmountSummary");
            Session.Remove("CardUser_ReceiverDetailsViewModel");
            return View(vm);
        }
    }
}