using FAXER.PORTAL.Areas.Businesses.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class MFTCCardLocalTopupController : Controller
    {
        Services.CommonServices _CommonServices = new Services.CommonServices();
        Services.MFTCCardPaymentByMerchantServices _mFTCCardPaymentByMerchantServices = new Services.MFTCCardPaymentByMerchantServices();
        // GET: Businesses/MFTCCardLocalTopup
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult TopUpAmount()
        {

            MFTCCardLocalTopUpByMerchantViewModel vm = new ViewModels.MFTCCardLocalTopUpByMerchantViewModel();
            if (Common.BusinessSession.MFTCCardLocalTopUpByMerchantViewModel != null)
            {
                vm = Common.BusinessSession.MFTCCardLocalTopUpByMerchantViewModel;
            }
            Services.BusinessCardServices cardServices = new Services.BusinessCardServices();
            ViewBag.CreditonCard = cardServices.GetCreditOnCard() + " " + Common.Common.GetCountryCurrency(Common.BusinessSession.LoggedBusinessMerchant.CountryCode);

            return View(vm);

        }
        [HttpPost]
        public ActionResult TopUpAmount([Bind(Include = MFTCCardLocalTopUpByMerchantViewModel.BindProperty)]MFTCCardLocalTopUpByMerchantViewModel vm)
        {

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
                Common.BusinessSession.MFTCCardLocalTopUpByMerchantViewModel = vm;
                return RedirectToAction("TransactionSummary");
            }

            return View();
            
        }
        [HttpGet]
        public ActionResult TransactionSummary()
        {


            MFTCCardTopUpTransactionSummaryViewModel vm = new MFTCCardTopUpTransactionSummaryViewModel();
            #region MFTC Card User Details
            var CardUserDetails = _mFTCCardPaymentByMerchantServices.GetCardInformationByCardNumber(Common.BusinessSession.MFTCCardNumber);
            vm.MFTCCardNumber = CardUserDetails.MobileNo.Decrypt().GetVirtualAccountNo();
            vm.CardUserId = CardUserDetails.Id;
            vm.CardUsername = CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName;
            #endregion
            #region Merchant Information 
            var MerchantCardDetiails = _mFTCCardPaymentByMerchantServices.GetBusinessInformation();
            vm.MerchantMFBCCardNumber = MerchantCardDetiails.MobileNo.Decrypt().FormatMFBCCard();
            vm.MerchantCardID = MerchantCardDetiails.Id;
            vm.KiiPayBusinessInformationId = MerchantCardDetiails.KiiPayBusinessInformationId;
            vm.MerchantName = MerchantCardDetiails.KiiPayBusinessInformation.BusinessName;
            vm.MerchantEmail = MerchantCardDetiails.KiiPayBusinessInformation.Email;
            vm.MerchantPhoneNumber = Common.Common.GetCountryPhoneCode(MerchantCardDetiails.Country) + " " + MerchantCardDetiails.KiiPayBusinessInformation.PhoneNumber;
            vm.CountryOfBirth = Common.Common.GetCountryName(MerchantCardDetiails.KiiPayBusinessInformation.BusinessOperationCountryCode);
            vm.streetAddress = MerchantCardDetiails.KiiPayBusinessInformation.BusinessOperationAddress1;
            vm.State = MerchantCardDetiails.KiiPayBusinessInformation.BusinessOperationState;
            vm.PostalCode = MerchantCardDetiails.KiiPayBusinessInformation.BusinessOperationPostalCode;
            //vm.CardExpriyDate = MerchantCardDetiails.

            #endregion

            #region Faxing Amount summary

            vm.TotalAmount = Common.BusinessSession.MFTCCardLocalTopUpByMerchantViewModel.FaxingAmount.ToString();
            #endregion

            ViewBag.SendSMS = _CommonServices.CheckBalanceForMessage(Convert.ToDecimal(Common.BusinessSession.MFTCCardLocalTopUpByMerchantViewModel.FaxingAmount));
            Common.BusinessSession.TransactionSummaryURL = "/Businesses/MFTCCardLocalTopup/TransactionSummary";
            return View(vm);
            
        }
        [HttpPost]
        [PreventSpam(DelayRequest = 60, ErrorMessage = "You can only create a new transaction every 60 seconds.")]
        public   ActionResult TransactionSummary([Bind(Include = MFTCCardTopUpTransactionSummaryViewModel.BindProperty)]MFTCCardTopUpTransactionSummaryViewModel vm)
        {

            // If user request multiple time then the request will not be valid 
            // and return to the payment successful page as their first request 
            if (!ModelState.IsValid)
            {
                return RedirectToAction("PaymentSuccessFul");
            }
            decimal smsFee = 0;
            ViewBag.SendSMS = _CommonServices.CheckBalanceForMessage(Convert.ToDecimal(Common.BusinessSession.MFTCCardLocalTopUpByMerchantViewModel.FaxingAmount));
            var faxingDetials = Common.BusinessSession.MFTCCardLocalTopUpByMerchantViewModel;
            KiiPayPersonalWalletPaymentByKiiPayBusiness trans = new KiiPayPersonalWalletPaymentByKiiPayBusiness()
            {
                KiiPayBusinessWalletInformationId = vm.MerchantCardID,
                KiiPayBusinessInformationId = vm.KiiPayBusinessInformationId,
                KiiPayPersonalWalletInformationId = vm.CardUserId,
                ExchangeRate = 0,
                PayingAmount = faxingDetials.FaxingAmount,
                Fee = 0,
                RecievingAmount = faxingDetials.FaxingAmount,
                TotalAmount = faxingDetials.FaxingAmount,
                PaymentReference = faxingDetials.PaymentReference,
                TransactionDate = DateTime.Now,
                ReceiptNumber = _CommonServices.ReceiptNoForMFTCPayment(),
                PaymentType = DB.PaymentType.Local

            };
            var result = _mFTCCardPaymentByMerchantServices.SaveTransaction(trans);

            var ReceiverDetails = _CommonServices.GetMFTCCardUserInformation(trans.KiiPayPersonalWalletInformationId);

            var MFBCCardCardDetials = _CommonServices.GetMFBCCardInformationByMFBCCardID(result.KiiPayBusinessWalletInformationId);


            if (vm.SendSms == true)
            {
                smsFee = Common.Common.GetSmsFee(MFBCCardCardDetials.Country);
            }
            if (MFBCCardCardDetials.CurrentBalance < trans.TotalAmount + smsFee)
            {
                smsFee = 0;
                vm.SendSms = false;
            }
            var DeductTheCredit = _CommonServices.DeductTheCreditOnCard(trans.KiiPayBusinessWalletInformationId, trans.TotalAmount + smsFee);

            var IncreaseBalanceonMFTC = _CommonServices.IncreaseTheCreditBalanceonMFTC(trans.KiiPayPersonalWalletInformationId, trans.TotalAmount);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);



            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCTopUpConfirmation?CardUserName="
                + MFBCCardCardDetials.FirstName + " " + MFBCCardCardDetials.MiddleName + " " + MFBCCardCardDetials.LastName +
                "&ReceiverName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName +
                "&ReceiverCountry=" + Common.Common.GetCountryName(ReceiverDetails.CardUserCountry) +
                    "&AccountNo=" + ReceiverDetails.MobileNo.Decrypt().GetVirtualAccountNo() + "&FaxingCurrency=" + Common.Common.GetCurrencySymbol(MFBCCardCardDetials.Country)
                + "&Amount=" + trans.TotalAmount + "&Fee=" + trans.Fee + "&IsLocalPayment=" + true);
            


            //string URL = baseUrl + "/EmailTemplate/MFTCCardTopUpconfirmationPaymentReceipt?ReceiptNumber=" + trans.ReceiptNumber
            //    + "&Date=" + trans.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + trans.TransactionDate.ToString("HH:mm") + "&SenderName=" +
            //    MFBCCardCardDetials.CardUserFullName + " " + MFBCCardCardDetials.CardUserMiddleName + " " + MFBCCardCardDetials.CardUserLastName
            //    + "&MFTCCardNo=" + ReceiverDetails.MFTCCardNumber.Decrypt()
            //    + "&MFTCCardName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName +
            //    "&TopUpAmount=" + trans.FaxingAmount + "&Fees=" + trans.FaxingFee +
            //    "&ExchangeRate=" + trans.ExchangeRate + "&TotalAmount=" + trans.TotalAmount + "&AmountInLocalCurrency=" + trans.RecievingAmount;
            //var Receipt = Common.Common.GetPdf(URL);


            mail.SendMail(MFBCCardCardDetials.Email, "Virtual Account Deposit Confirmation ", body);

            if(vm.SendSms == true)
            {
                SmsApi smsApiServices = new SmsApi();
                string senderName = MFBCCardCardDetials.FirstName + " " + MFBCCardCardDetials.MiddleName + " " + MFBCCardCardDetials.LastName;
                string virtualAccountNo = ReceiverDetails.MobileNo.Decrypt().GetVirtualAccountNo();
                string amount = Common.Common.GetCurrencySymbol(MFBCCardCardDetials.Country) + trans.TotalAmount;
                string receivingAmount = Common.Common.GetCurrencySymbol(MFBCCardCardDetials.Country) + trans.RecievingAmount;

                string message = smsApiServices.GetVirtualAccountDepositMessage(senderName, virtualAccountNo, amount,receivingAmount);
                string senderPhoneNumber = Common.Common.GetCountryPhoneCode(MFBCCardCardDetials.Country) + MFBCCardCardDetials.PhoneNumber;
                string receiverPhoneNumber = Common.Common.GetCountryPhoneCode(MFBCCardCardDetials.Country) + ReceiverDetails.CardUserTel;
                smsApiServices.SendSMS(senderPhoneNumber, message);
                smsApiServices.SendSMS(receiverPhoneNumber, message);

            }
            return RedirectToAction("PaymentSuccessFul");
            
        }

        public ActionResult PaymentSuccessful()
        {

            var faxingDetials = Common.BusinessSession.MFTCCardLocalTopUpByMerchantViewModel;
            var CardUserDetails = _mFTCCardPaymentByMerchantServices.GetCardInformationByCardNumber(Common.BusinessSession.MFTCCardNumber);
            MFTCCardTopUpByMerchantSuccessfulViewModel vm = new MFTCCardTopUpByMerchantSuccessfulViewModel()
            {
                FaxingAmount = faxingDetials.FaxingAmount,
                CardUserCountry = Common.Common.GetCountryName(CardUserDetails.CardUserCountry),
                CardUserName = CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName,
                MFTCCardNumber = CardUserDetails.MobileNo.Decrypt().GetVirtualAccountNo(),
                ReceiveOption = "E-Card Withdrawal"

            };
            Session.Remove("MFTCCardLocalTopUpByMerchantViewModel");
            Session.Remove("MFTCCardNumber");
            Session.Remove("TransactionSummaryURL");


            return View(vm);
        }
    }
}