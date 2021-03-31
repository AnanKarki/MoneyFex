using FAXER.PORTAL.Areas.Businesses.ViewModels;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class MerchantNationalPaymentController : Controller
    {
        Services.MerchantNationalPaymentServices _merchantNationalPaymentServices = null;
        Services.CommonServices _CommonServices = null;
        DB.FAXEREntities dbContext = null;
        public MerchantNationalPaymentController()
        {
            _merchantNationalPaymentServices = new Services.MerchantNationalPaymentServices();
            _CommonServices = new Services.CommonServices();
            dbContext = new DB.FAXEREntities();
        }
        // GET: Businesses/MerchantNationalPayment
        [HttpGet]
        public ActionResult Index()
        {
            PreviousPayeeViewModel vm = new PreviousPayeeViewModel();
            var PreviousPayee = _merchantNationalPaymentServices.GetPreviousPayee();
            ViewBag.PreviousPayee = new SelectList(PreviousPayee, "BusinessMFCode", "Name");
            Common.BusinessSession.BackButtonURL = "/Businesses/MerchantNationalPayment/Index";
            if (!string.IsNullOrEmpty(Common.BusinessSession.MerchantAccountNumber))
            {

                vm.BusinessMFCode = Common.BusinessSession.MerchantAccountNumber;
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = PreviousPayeeViewModel.BindProperty)]ViewModels.PreviousPayeeViewModel vm)
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

        public ActionResult SearchMerchantByAccountNumber()
        {

            if (!string.IsNullOrEmpty(Common.BusinessSession.MerchantAccountNumber))
            {
                ViewBag.BusinessName = Common.BusinessSession.MerchantDetialsViewModel == null ? "" : Common.BusinessSession.MerchantDetialsViewModel.MerchantName;
                ViewBag.MerchantAccountNo = Common.BusinessSession.MerchantAccountNumber;
            }

            Common.BusinessSession.BackButtonURL = "/Businesses/MerchantNationalPayment/SearchMerchantByAccountNumber";
            return View();
        }
        public ActionResult MerchantDetails(string MerchantAccountNo = "")
        {
            if (Common.BusinessSession.LoggedBusinessMerchant.BusinessMobileNo == MerchantAccountNo)
            {
                TempData["InvalidMFBCCard"] = "You can't send money to yourself ! ";
                return RedirectToAction("SearchMerchantByAccountNumber");
            }
            ViewModels.MerchantDetialsViewModel vm = new ViewModels.MerchantDetialsViewModel();
            //if (!string.IsNullOrEmpty(Common.BusinessSession.MerchantAccountNumber))
            //{

            //    MerchantAccountNo = Common.BusinessSession.MerchantAccountNumber;
            //}
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

                if (_merchantNationalPaymentServices.GetMFBCCardInformation(merchantDetails.Id).Country != Common.BusinessSession.LoggedBusinessMerchant.CountryCode)
                {

                    TempData["InvalidMFBCCard"] = "The service provider is not of your country , choose international payment option";
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
                Common.BusinessSession.MerchantDetialsViewModel = vm;
                Common.BusinessSession.MerchantAccountNumber = vm.MerchantAccountNo;

            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult MerchantDetails([Bind(Include = MerchantDetialsViewModel.BindProperty)]ViewModels.MerchantDetialsViewModel vm)
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
                    return RedirectToAction("PaymentAmount");
                }
            }
            return View(vm);
        }


        public ActionResult getMerchants(string term)
        {
            var data = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessName.StartsWith(term) && x.BusinessOperationCountryCode == Common.BusinessSession.LoggedBusinessMerchant.CountryCode);
            if (data.Count() > 0)
            {
                return Json(dbContext.KiiPayBusinessInformation.Where(x => x.BusinessName.StartsWith(term) && x.BusinessOperationCountryCode == Common.BusinessSession.LoggedBusinessMerchant.CountryCode && x.Id != Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId).Select(a => new { label = a.BusinessName, id = a.BusinessMobileNo }),
                             JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { label = "No Result Found", id = 0 },
                        JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult PaymentAmount()
        {
            MerchantNationalPaymentAmountViewModel vm = new MerchantNationalPaymentAmountViewModel();
            if (Common.BusinessSession.MerchantNationalPaymentAmountViewModel != null)
            {
                vm = Common.BusinessSession.MerchantNationalPaymentAmountViewModel;
            }
            Services.BusinessCardServices cardServices = new Services.BusinessCardServices();
            ViewBag.CreditonCard = cardServices.GetCreditOnCard() + " " + Common.Common.GetCountryCurrency(Common.BusinessSession.LoggedBusinessMerchant.CountryCode);
            return View(vm);
        }
        [HttpPost]
        public ActionResult PaymentAmount([Bind(Include = MerchantNationalPaymentAmountViewModel.BindProperty)]ViewModels.MerchantNationalPaymentAmountViewModel vm)
        {
            Services.BusinessCardServices cardServices = new Services.BusinessCardServices();
            ViewBag.CreditonCard = cardServices.GetCreditOnCard() + " " + Common.Common.GetCountryCurrency(Common.BusinessSession.LoggedBusinessMerchant.CountryCode);
            var currentbalance = _CommonServices.GetCurrentBalanceOnCard();
            if (vm.FaxingAmount <= 0)
            {

                ModelState.AddModelError("FaxingAmount", "Paying amount should be greater than 0");

            }
            else if (vm.FaxingAmount > currentbalance)
            {
                ModelState.AddModelError("FaxingAmount", "Insufficient balance on card");

            }
            else if (string.IsNullOrEmpty(vm.PaymentReference))
            {

                ModelState.AddModelError("PaymentReference", "Please enter a payment reference");

            }
            else
            {
                Common.BusinessSession.MerchantNationalPaymentAmountViewModel = vm;
                return RedirectToAction("MerchantNationalPaymentTransactionSummary");
            }

            return View();
        }
        [HttpGet]
        public ActionResult MerchantNationalPaymentTransactionSummary()
        {
            MerchantNationalPaymentTransactionSummaryViewModel vm = new MerchantNationalPaymentTransactionSummaryViewModel();

            #region Sender merchant Details 
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
            vm.MerchantMFBCCardNumber = senderMerchantDetials.MobileNo.Decrypt().FormatMFBCCard();
            #endregion

            #region Receiver Merchant Details 

            var ReceiverDetails = Common.BusinessSession.MerchantDetialsViewModel;
            vm.ReceiverName = ReceiverDetails.MerchantName;
            vm.ReceiverKiiPayBusinessInformationId = ReceiverDetails.KiiPayBusinessInformationId;
            vm.ReceiverCardId = ReceiverDetails.MFBCCardID;
            vm.ReceiverAccountNo = ReceiverDetails.MerchantAccountNo;
            vm.ReceiveOption = "MFBC Card Withdrawal";

            ViewBag.SendSMS = _CommonServices.CheckBalanceForMessage(Convert.ToDecimal(Common.BusinessSession.MerchantNationalPaymentAmountViewModel.FaxingAmount));
            #endregion

            vm.TotalAmount = Common.BusinessSession.MerchantNationalPaymentAmountViewModel.FaxingAmount.ToString();
            Common.BusinessSession.TransactionSummaryURL = "/Businesses/MerchantNationalPayment/MerchantNationalPaymentTransactionSummary";

            return View(vm);
        }
        [HttpPost]
        public ActionResult MerchantNationalPaymentTransactionSummary([Bind(Include = MerchantNationalPaymentTransactionSummaryViewModel.BindProperty)]MerchantNationalPaymentTransactionSummaryViewModel vm)
        {
            ViewBag.SendSMS = _CommonServices.CheckBalanceForMessage(Convert.ToDecimal(Common.BusinessSession.MerchantNationalPaymentAmountViewModel.FaxingAmount));
            var ReceiverDetails = Common.BusinessSession.MerchantDetialsViewModel;
            var SenderMerchantDetails = _merchantNationalPaymentServices.GetSenderBusinessInformation();
            decimal smsFee = 0;
            var transactionDetials = Common.BusinessSession.MerchantNationalPaymentAmountViewModel;
            DB.KiiPayBusinessLocalTransaction trans = new DB.KiiPayBusinessLocalTransaction()
            {
                AmountSent = transactionDetials.FaxingAmount,
                PaymentReference = transactionDetials.PaymentReference,
                PayedFromKiiPayBusinessInformationId = SenderMerchantDetails.KiiPayBusinessInformationId,
                PayedFromKiiPayBusinessWalletInformationId = SenderMerchantDetails.Id,
                PayedToKiiPayBusinessInformationId = ReceiverDetails.KiiPayBusinessInformationId,
                PayedToKiiPayBusinessWalletInformationId = ReceiverDetails.MFBCCardID,
                TransactionDate = DateTime.Now

            };

            var result = _merchantNationalPaymentServices.SaveTransaction(trans);

            int PayedFromMFBCCardId = trans.PayedFromKiiPayBusinessWalletInformationId ?? 0;

            if (vm.SendSms == true )
            {
                smsFee = Common.Common.GetSmsFee(SenderMerchantDetails.Country);               
            }
            if  (SenderMerchantDetails.CurrentBalance < trans.AmountSent + smsFee)
            {
                smsFee = 0;
                vm.SendSms = false;
            }
            bool deductCreditOfSender = _CommonServices.DeductTheCreditOnCard(PayedFromMFBCCardId, trans.AmountSent + smsFee);

            bool IncreseReceiverCredit = _CommonServices.IncreaseTheCreditBalanceonMFBCCard(trans.PayedToKiiPayBusinessWalletInformationId, trans.AmountSent);
            var receiverMerchantDetials = _CommonServices.GetMFBCCardInformationByKiiPayBusinessInformationId(trans.PayedToKiiPayBusinessInformationId);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationPaymentServiceProvider?SenderName=" +
               SenderMerchantDetails.FirstName + " " + SenderMerchantDetails.MiddleName + " " + SenderMerchantDetails.LastName +
               "&ReceivingAmount=" + trans.AmountSent + "&ReceiverBusinessName=" + receiverMerchantDetials.KiiPayBusinessInformation.BusinessName
               + "&ReceiverCountry=" + Common.Common.GetCountryName(receiverMerchantDetials.Country));

            mail.SendMail(SenderMerchantDetails.Email, "Confirmation of Payment to Service Provider ", body);

            if (vm.SendSms == true)
            {
                SmsApi smsApiServices = new SmsApi();
                string senderName = SenderMerchantDetails.FirstName + " " + SenderMerchantDetails.MiddleName + " " + SenderMerchantDetails.LastName;
                string businessAccounntNo = receiverMerchantDetials.KiiPayBusinessInformation.BusinessMobileNo;
                string businessName = receiverMerchantDetials.KiiPayBusinessInformation.BusinessName;
                string amount = Common.Common.GetCurrencySymbol(SenderMerchantDetails.Country) + trans.AmountSent;
                string paymentReference = trans.PaymentReference;
                string reveingAmount = Common.Common.GetCurrencySymbol(SenderMerchantDetails.Country) + trans.AmountSent;
                string message = smsApiServices.GetBusinessPaymentMessage(senderName, businessAccounntNo, businessName, amount, paymentReference, reveingAmount);
                string senderPhoneNumber = Common.Common.GetCountryPhoneCode(SenderMerchantDetails.Country) + SenderMerchantDetails.PhoneNumber;
                string receiverPhoneNumber = Common.Common.GetCountryPhoneCode(SenderMerchantDetails.Country) + receiverMerchantDetials.PhoneNumber;
                smsApiServices.SendSMS(senderPhoneNumber, message);
                smsApiServices.SendSMS(receiverPhoneNumber, message);

            }

            return RedirectToAction("PaymentSuccessful");

        }

        public ActionResult PaymentSuccessful()
        {

            var ReceiverDetails = Common.BusinessSession.MerchantDetialsViewModel;
            MerchantNationalPaymentSuccessfullViewModel vm = new MerchantNationalPaymentSuccessfullViewModel()
            {

                MerchantAccountNo = ReceiverDetails.MerchantAccountNo,
                MerchantName = ReceiverDetails.MerchantName,
                MerchantCountry = ReceiverDetails.Country,
                PaymentReference = Common.BusinessSession.MerchantNationalPaymentAmountViewModel.PaymentReference,
                TotalAmount = Common.BusinessSession.MerchantNationalPaymentAmountViewModel.FaxingAmount.ToString(),
                ReceiveOption = "MFBC Card withdrawal"
            };
            Session.Remove("MerchantDetialsViewModel");
            Session.Remove("MerchantNationalPaymentAmountViewModel");
            Session.Remove("MerchantAccountNumber");
            Session.Remove("TransactionSummaryURL");
            return View(vm);
        }
    }
}