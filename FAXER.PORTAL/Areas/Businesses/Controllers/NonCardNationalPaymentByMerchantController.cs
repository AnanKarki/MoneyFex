using FAXER.PORTAL.Areas.Businesses.Services;
using FAXER.PORTAL.Areas.Businesses.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class NonCardNationalPaymentByMerchantController : Controller
    {
        DB.FAXEREntities context = new DB.FAXEREntities();
        NonCardPaymentByMerchantServices _noncardPaymentServices = new NonCardPaymentByMerchantServices();
        MFTCCardPaymentByMerchantServices _mFTCCardPaymentByMerchantServices = new MFTCCardPaymentByMerchantServices();
        Services.CommonServices _commonServices = new Services.CommonServices();
        int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
        // GET: Businesses/NonCardNationalPaymentByMerchant
        public ActionResult Index()
        {
            return View();
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
            model.ReceiverCountry = Common.BusinessSession.LoggedBusinessMerchant.CountryCode;
            return View(model);
            

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
                return RedirectToAction("NoCardPaymentTransactionSummary");
            }

            return View(model);
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
            vm.SentAmount = Common.BusinessSession.NonCardPaymentViewModel.TopUpAmount.ToString();

            #endregion
            ViewBag.SendSMS = _commonServices.CheckBalanceForMessage(Convert.ToDecimal(vm.SentAmount));
            Common.BusinessSession.TransactionSummaryURL = "/Businesses/NonCardNationalPaymentByMerchant/NoCardPaymentTransactionSummary";


            return View(vm);
        }

        [HttpPost]
        public ActionResult NoCardPaymentTransactionSummary([Bind(Include = NoCardPaymentTransactionSummaryViewModel.BindProperty)]NoCardPaymentTransactionSummaryViewModel vm)
        {
            decimal smsFee = 0;
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
            
            string MFCNNumber = _noncardPaymentServices.getMFCN();
            var SenderMFBCDetails = _noncardPaymentServices.getSenderDetails();
            DB.MerchantNonCardTransaction transaction = new DB.MerchantNonCardTransaction()
            {
                KiiPayBusinessInformationId = KiiPayBusinessInformationId,
                NonCardRecieverId = receiverID,
                MFBCCardID = SenderMFBCDetails.Id,
                ExchangeRate = 0,
                FaxingAmount = Common.BusinessSession.NonCardPaymentViewModel.TopUpAmount,
                FaxingFee = 0,
                FaxingStatus = DB.FaxingStatus.NotReceived,
                TotalAmount = 0,
                MFCN = MFCNNumber,
                ReceivingAmount = 0,
                TransactionDate = DateTime.Now,
                ReceiptNumber = _commonServices.GetNonCardPaymentReceiptNumberToSave(),
                PaymentType = TopUpType.National
            };
            var saveTrans = _noncardPaymentServices.saveTransaction(transaction);
            ViewBag.SendSMS = _commonServices.CheckBalanceForMessage(Convert.ToDecimal(transaction.FaxingAmount));
            if (vm.SendSms == true)
            {
                smsFee = Common.Common.GetSmsFee(SenderMFBCDetails.Country);
            }
            if (SenderMFBCDetails.CurrentBalance < transaction.FaxingAmount + smsFee)
            {
                smsFee = 0;
                vm.SendSms = false;
            }
            var DeductCreditOnCard = _commonServices.DeductTheCreditOnCard(transaction.MFBCCardID, transaction.FaxingAmount + smsFee);
            #endregion


            var NonCardReceiverDetials = _commonServices.GetNonCardReceiverDetails(transaction.NonCardRecieverId);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string body_Sender = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyTransferEmailToSender?CardUserName" +
                SenderMFBCDetails.FirstName + "" + SenderMFBCDetails.MiddleName + " " + SenderMFBCDetails.LastName +
                "&ReceiverName=" + NonCardReceiverDetials.FirstName + " " + NonCardReceiverDetials.MiddleName + " " + NonCardReceiverDetials.LastName
                + "&ReceiverCountry=" + Common.Common.GetCountryName(NonCardReceiverDetials.Country)
                + "&ReceiverCity=" + NonCardReceiverDetials.City + "&MFCN=" + transaction.MFCN + "&TransferredAmount=" + transaction.FaxingAmount + "&FaxingCurrency=" + Common.BusinessSession.FaxingCurrency +
                "&IsLocalPayment=" + true);


            mail.SendMail(SenderMFBCDetails.Email, "Confirmation of Money Transfer ", body_Sender);

            


            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyTransfer?ReceiverCountry=" +
                Common.Common.GetCountryName(NonCardReceiverDetials.Country)
                + "&ReceiverName=" + NonCardReceiverDetials.FirstName + " " + NonCardReceiverDetials.MiddleName + " " + NonCardReceiverDetials.LastName
                + "&SenderCountry=" + Common.Common.GetCountryName(NonCardReceiverDetials.Business.BusinessOperationCountryCode));
            mail.SendMail(NonCardReceiverDetials.EmailAddress, "Confirmation of Money Transfer ", body);

            if (vm.SendSms == true)
            {
                SmsApi smsApiServices = new SmsApi();
                string senderName = SenderMFBCDetails.FirstName + "" + SenderMFBCDetails.MiddleName + " " + SenderMFBCDetails.LastName;
                string MFCN = transaction.MFCN;
                string amount = Common.Common.GetCurrencySymbol(SenderMFBCDetails.Country) + transaction.FaxingAmount;
                string receivingAmount = Common.Common.GetCountryName(NonCardReceiverDetials.Country) + transaction.FaxingAmount;

                string message = smsApiServices.GetCashToCashTransferMessage(senderName, MFCN, amount , "0",receivingAmount);
                string senderPhoneNumber = Common.Common.GetCountryPhoneCode(SenderMFBCDetails.Country) + SenderMFBCDetails.PhoneNumber;
                string receiverphoneNumber = Common.Common.GetCountryPhoneCode(SenderMFBCDetails.Country) + NonCardReceiverDetials.PhoneNumber;
                smsApiServices.SendSMS(receiverphoneNumber, message);
            }
            
            return RedirectToAction("PaymentSuccessful", new { MFCN = saveTrans.MFCN });

            //return View(vm);
        }
        public ActionResult PaymentSuccessful(string MFCN)
        {
            var receiverDetails = Common.BusinessSession.ReceiverDetailsViewModel;
            
            MerchantNonCardPaymentSuccessful vm = new MerchantNonCardPaymentSuccessful()
            {

                MFCN = MFCN,
                SentAmount = Common.BusinessSession.NonCardPaymentViewModel.TopUpAmount,
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