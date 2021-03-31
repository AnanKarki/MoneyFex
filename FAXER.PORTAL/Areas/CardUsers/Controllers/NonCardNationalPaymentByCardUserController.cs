using FAXER.PORTAL.Areas.CardUsers.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class NonCardNationalPaymentByCardUserController : Controller
    {

        Services.CardUserCommonServices _cardUserCommonServices = new Services.CardUserCommonServices();
        Services.CardUser_NonCardPaymentServices _cardUser_NonCardPaymentServices = new Services.CardUser_NonCardPaymentServices();
        DB.FAXEREntities context = new DB.FAXEREntities();
        int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel == null ? 0 : Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
        // GET: CardUsers/NonCardNationalPaymentByCardUser
        public ActionResult Index()
        {
            return View();
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
            if ((Common.CardUserSession.CardUser_ReceiverDetailsViewModel != null) && int.Parse(Common.CardUserSession.CardUser_ReceiverDetailsViewModel.PreviousReceivers) == id)
            {

                model = Common.CardUserSession.CardUser_ReceiverDetailsViewModel;
                return View(model);
            }
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
                return RedirectToAction("NoCardPaymentTransactionSummary", "NonCardNationalPaymentByCardUser");
            }

            return View(model);
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

            vm.SentAmount = Common.CardUserSession.CardUser_NonCardPayingAmountViewModel.TopUpAmount.ToString();

            #endregion

            ViewBag.SendSMS = _cardUserCommonServices.CheckBalanceForMessage(Convert.ToDecimal(vm.SentAmount));
            Common.CardUserSession.TransactionSummaryURL = "/CardUsers/NonCardNationalPaymentByCardUser/NoCardPaymentTransactionSummary";
            return View(vm);
        }

        [HttpPost]
        public ActionResult NoCardPaymentTransactionSummary([Bind(Include = CardUser_NonCardPaymentTransactionSummaryViewModel.BindProperty)]CardUser_NonCardPaymentTransactionSummaryViewModel vm)
        {

            ViewBag.SendSMS = _cardUserCommonServices.CheckBalanceForMessage(Convert.ToDecimal(vm.SentAmount));
            decimal smsFee = 0;

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
            string MFCNNumber = _cardUserCommonServices.getMFCN();
            var SenderMFTCDetails = _cardUserCommonServices.GetMFTCCardUserInformation(MFTCCardId);
            DB.CardUserNonCardTransaction transaction = new DB.CardUserNonCardTransaction()
            {

                NonCardRecieverId = receiverID,
                MFTCCardId = SenderMFTCDetails.Id,
                ExchangeRate = 0,
                FaxingAmount = Common.CardUserSession.CardUser_NonCardPayingAmountViewModel.TopUpAmount,
                FaxingFee = 0,
                FaxingStatus = DB.FaxingStatus.NotReceived,
                TotalAmount = Common.CardUserSession.CardUser_NonCardPayingAmountViewModel.TopUpAmount,
                MFCN = MFCNNumber,
                ReceivingAmount = Common.CardUserSession.CardUser_NonCardPayingAmountViewModel.TopUpAmount,
                TransactionDate = DateTime.Now,
                ReceiptNumber = _cardUserCommonServices.GetNewReceiptNumberToSave(),
                PaymentType = TopUpType.National
            };
            var saveTrans = _cardUser_NonCardPaymentServices.SaveTransaction(transaction);


            if (vm.SendSms == true)
            {
                smsFee = Common.Common.GetSmsFee(SenderMFTCDetails.CardUserCountry);
            }
            if (SenderMFTCDetails.CurrentBalance < transaction.TotalAmount + smsFee)
            {
                smsFee = 0;
                vm.SendSms = false;
            }
            Common.Common.DeductCreditOnCard(transaction.MFTCCardId, transaction.TotalAmount + smsFee);

            #endregion


            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);


            var ReceiverDetails = _cardUserCommonServices.GetReceiverDetails(saveTrans.NonCardRecieverId);

            string body_Sender = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyTransferEmailToSender?CardUserName=" +
                SenderMFTCDetails.FirstName + "" + SenderMFTCDetails.MiddleName + " " + SenderMFTCDetails.LastName +
                "&ReceiverName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName
                + "&ReceiverCountry=" + Common.Common.GetCountryName(ReceiverDetails.Country)
                + "&ReceiverCity=" + ReceiverDetails.City + "&MFCN=" + saveTrans.MFCN + "&TransferredAmount=" + saveTrans.FaxingAmount + "&FaxingCurrency=" + Common.CardUserSession.FaxingCurrency
                + "&IsLocalPayment=" + true);



            mail.SendMail(SenderMFTCDetails.CardUserEmail, "Confirmation of Money Transfer ", body_Sender);

            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyTransfer?ReceiverCountry=" +
                Common.Common.GetCountryName(ReceiverDetails.Country)
                + "&ReceiverName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName
                + "&SenderCountry=" + Common.Common.GetCountryName(ReceiverDetails.MFTCCardInformation.CardUserCountry));
            mail.SendMail(ReceiverDetails.EmailAddress, "Confirmation of Money Transfer ", body);

            if (vm.SendSms == true)
            {
                SmsApi smsApiServices = new SmsApi();
                string senderName = SenderMFTCDetails.FirstName + " " + SenderMFTCDetails.MiddleName + " " + SenderMFTCDetails.LastName;
                string smsMFCN = saveTrans.MFCN;
                string amount = Common.Common.GetCurrencySymbol(SenderMFTCDetails.CardUserCountry) + transaction.TotalAmount;
                string receivingAmount = Common.Common.GetCurrencySymbol(SenderMFTCDetails.CardUserCountry) + transaction.TotalAmount;

                string message = smsApiServices.GetCashToCashTransferMessage(senderName, smsMFCN, amount, "0", receivingAmount);
                string senderPhoneNumber = Common.Common.GetCountryPhoneCode(SenderMFTCDetails.CardUserCountry) + SenderMFTCDetails.CardUserTel;
                string receiverPhoneNumber = Common.Common.GetCountryPhoneCode(ReceiverDetails.Country) + ReceiverDetails.PhoneNumber;
                smsApiServices.SendSMS(senderPhoneNumber, message);
                smsApiServices.SendSMS(receiverPhoneNumber, message);
            }


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

            return View();
        }

        public ActionResult PaymentSuccessful(string MFCN = "")
        {

            var receiverDetails = Common.CardUserSession.CardUser_ReceiverDetailsViewModel;
            CardUser_NonCardPaymentSuccessfulViewModel vm = new CardUser_NonCardPaymentSuccessfulViewModel()
            {
                ReceiverName = receiverDetails.ReceiverFirstName + " " + receiverDetails.ReceiverMiddleName + " " + receiverDetails.ReceiverLastName,

                SentAmount = Common.CardUserSession.CardUser_NonCardPayingAmountViewModel.TopUpAmount.ToString(),
                MFCNNumber = MFCN,

                ReceiverCountry = receiverDetails.ReceiverCountry
            };

            Session.Remove("CardUser_ReceiverDetailsViewModel");
            Session.Remove("FaxingAmountSummary");
            Session.Remove("CardUser_ReceiverDetailsViewModel");
            return View(vm);
        }


    }
}