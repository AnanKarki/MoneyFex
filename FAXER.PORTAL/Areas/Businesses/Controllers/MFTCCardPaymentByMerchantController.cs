using FAXER.PORTAL.Areas.Businesses.Services;
using FAXER.PORTAL.Areas.Businesses.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class MFTCCardPaymentByMerchantController : Controller
    {
        DB.FAXEREntities context = null;
        MFTCCardPaymentByMerchantServices _mFTCCardPaymentByMerchantServices = null;
        CommonServices _commonServices = null;
        public MFTCCardPaymentByMerchantController()
        {
            context = new DB.FAXEREntities();
            _mFTCCardPaymentByMerchantServices = new MFTCCardPaymentByMerchantServices();
            _commonServices = new CommonServices();
        }
        // GET: Businesses/MFTCCardPaymentByMerchant
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(Common.BusinessSession.MFTCCardNumber))
            {

                ViewBag.MFTCCardNumber = Common.BusinessSession.MFTCCardNumber;
            }
            return View();
        }

        [HttpGet]
        public ActionResult SearchMFTCCardUser(string MFTCCardNumber = "")
        {
            ViewModels.MFTCCarduserDetailsViewModel vm = new MFTCCarduserDetailsViewModel();




            if (!string.IsNullOrEmpty(MFTCCardNumber))
            {



                var CardUserDetails = new DB.KiiPayPersonalWalletInformation();

                CardUserDetails = _mFTCCardPaymentByMerchantServices.GetCardInformationByCardNumber(MFTCCardNumber.Trim());



                //var CardUserDetails = _mFTCCardPaymentByMerchantServices.GetCardUserDetials(MFTCCardNumber);

                if (CardUserDetails == null)
                {

                    TempData["NoCardFound"] = "Please enter valid a virtual account no";
                    return RedirectToAction("Index");
                }
                if (CardUserDetails.CardStatus == DB.CardStatus.InActive)
                {

                    TempData["NoCardFound"] = "Virtual account  is currently deactivated";
                    return RedirectToAction("Index");
                }
                else if (CardUserDetails.CardStatus == DB.CardStatus.IsDeleted)
                {

                    TempData["NoCardFound"] = "Virtual account  has been Deleted";
                    return RedirectToAction("Index");
                }
                else if (CardUserDetails.CardStatus == DB.CardStatus.IsRefunded)
                {

                    TempData["NoCardFound"] = "Virtual account  has been refunded";
                    return RedirectToAction("Index");
                }
                else
                {
                    vm = new MFTCCarduserDetailsViewModel()
                    {
                        Id = CardUserDetails.Id,
                        CardUserName = CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName,
                        MFTCCardNo = CardUserDetails.MobileNo.Decrypt().GetVirtualAccountNo(),
                        CarduserCity = CardUserDetails.CardUserCity,
                        CardUserCountry = Common.Common.GetCountryName(CardUserDetails.CardUserCountry),
                        Phone = CardUserDetails.CardUserTel,
                        Email = CardUserDetails.CardUserEmail
                    };
                    Common.BusinessSession.ReceivingCurrency = Common.Common.GetCountryCurrency(CardUserDetails.CardUserCountry);
                    Common.BusinessSession.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(CardUserDetails.CardUserCountry);
                    Common.BusinessSession.ReceivingCountry = CardUserDetails.CardUserCountry;
                    Common.BusinessSession.MFTCCardNumber = vm.MFTCCardNo;
                }
            }
            else
            {

                TempData["NoCardFound"] = "Please enter a virtual account no";
                return RedirectToAction("Index");
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult SearchMFTCCardUser([Bind(Include = MFTCCarduserDetailsViewModel.BindProperty)]MFTCCarduserDetailsViewModel vm)
        {

            if (ModelState.IsValid)
            {

                if (vm.Confirm == false)
                {

                    ModelState.AddModelError("Confirm", "Please confirm the account detials");
                    return View(vm);
                }
                if (!string.IsNullOrEmpty(Common.BusinessSession.TransactionSummaryURL))
                {

                    return Redirect(Common.BusinessSession.TransactionSummaryURL);

                }
                if (vm.CardUserCountry.ToLower() == Common.Common.GetCountryName(Common.BusinessSession.LoggedBusinessMerchant.CountryCode).ToLower())
                {

                    return RedirectToAction("TopUpAmount", "MFTCCardLocalTopup");
                }
                return RedirectToAction("TopUpAmount");
            }

            return View(vm);

        }
        [HttpGet]
        public ActionResult TopUpAmount()
        {

            TopUpAmountViewModel vm = new TopUpAmountViewModel();

            ViewBag.CreditonCard = Common.Common.GetCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.CountryCode) + " " + _commonServices.GetCurrentBalanceOnCard();
            if (Common.BusinessSession.TopUpAmountViewModel != null)
            {

                vm = Common.BusinessSession.TopUpAmountViewModel;
                return View(vm);
            }
            return View();
        }
        [HttpPost]
        public ActionResult TopUpAmount([Bind(Include = TopUpAmountViewModel.BindProperty)]TopUpAmountViewModel model)
        {

            // set Previous model value to vm
            TopUpAmountViewModel vm = new TopUpAmountViewModel();
            vm = model;
            if (model.TopUpAmount == 0 && model.ReceivingAmount == 0)
            {
                ModelState.AddModelError("TopUpAmount", "Please enter an amount to proceed");
            }
            else if (model.PaymentReference == null)
            {
                ModelState.AddModelError("PaymentReference", "Payment Reference is Required");
            }
            else
            {
                string FaxingCountryCode = Common.BusinessSession.FaxingCountry;
                string ReceivingCountryCode = Common.BusinessSession.ReceivingCountry;
                decimal exchangeRate = 0;
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
                Common.BusinessSession.TopUpAmountViewModel = vm;
                return RedirectToAction("TopUpDetails");
            }
            return View();
        }

        public ActionResult TopUpDetails()
        {
            decimal CurrentBalanceOnCard = _commonServices.GetCurrentBalanceOnCard();

            EstimateFaxingFeeSummary faxingSummarry = new EstimateFaxingFeeSummary();
            faxingSummarry = Common.BusinessSession.FaxingAmountSummary;
            if (faxingSummarry.TotalAmount > CurrentBalanceOnCard)
            {

                TempData["InSufficientBalance"] = "Insufficient balance on card";
                return RedirectToAction("TopUpAmount");
            }
            TopUpDetailsViewModel model = new TopUpDetailsViewModel()
            {
                FaxingFee = faxingSummarry.FaxingFee,
                AmountToBeReceived = faxingSummarry.ReceivingAmount,
                CurrentExchangeRate = faxingSummarry.ExchangeRate,
                faxingAmount = faxingSummarry.FaxingAmount,
                PaymentReference = Common.BusinessSession.TopUpAmountViewModel.PaymentReference,
                TotalAmountIncludingFee = faxingSummarry.TotalAmount,

            };
            return View(model);
        }

        public ActionResult FraudAlertPage()
        {

            return View();
        }
        [HttpGet]
        public ActionResult MFTCCardTopUpTransactionSummary()
        {
            MFTCCardTopUpTransactionSummaryViewModel vm = new MFTCCardTopUpTransactionSummaryViewModel();
            #region MFTC Card User Details
            var CardUserDetails = _mFTCCardPaymentByMerchantServices.GetCardInformationByCardNumber(Common.BusinessSession.MFTCCardNumber);
            vm.MFTCCardNumber = CardUserDetails.MobileNo.Decrypt();
            vm.CardUserId = CardUserDetails.Id;
            vm.CardUsername = CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName;
            #endregion
            #region Merchant Information 
            var MerchantCardDetiails = _mFTCCardPaymentByMerchantServices.GetBusinessInformation();
            vm.MerchantCardID = MerchantCardDetiails.Id;
            vm.KiiPayBusinessInformationId = MerchantCardDetiails.KiiPayBusinessInformationId;
            vm.MerchantName = MerchantCardDetiails.KiiPayBusinessInformation.BusinessName;
            vm.MerchantEmail = MerchantCardDetiails.KiiPayBusinessInformation.Email;
            vm.MerchantPhoneNumber = Common.Common.GetCountryPhoneCode(MerchantCardDetiails.Country) + " " + MerchantCardDetiails.KiiPayBusinessInformation.PhoneNumber;
            vm.CountryOfBirth = Common.Common.GetCountryName(MerchantCardDetiails.KiiPayBusinessInformation.BusinessOperationCountryCode);
            vm.streetAddress = MerchantCardDetiails.KiiPayBusinessInformation.BusinessOperationAddress1;
            vm.State = MerchantCardDetiails.KiiPayBusinessInformation.BusinessOperationState;
            vm.PostalCode = MerchantCardDetiails.KiiPayBusinessInformation.BusinessOperationPostalCode;
            vm.MerchantMFBCCardNumber = MerchantCardDetiails.MobileNo.Decrypt().FormatMFBCCard();
            //vm.CardExpriyDate = MerchantCardDetiails.

            #endregion

            #region Faxing Amount summary
            var faxingDetials = Common.BusinessSession.FaxingAmountSummary;
            vm.Fees = faxingDetials.FaxingFee.ToString();
            vm.TotalAmount = faxingDetials.TotalAmount.ToString();
            vm.TotalReceiveAmount = faxingDetials.ReceivingAmount.ToString();
            vm.SentAmount = faxingDetials.FaxingAmount.ToString();
            #endregion
            Common.BusinessSession.TransactionSummaryURL = "/Businesses/MFTCCardPaymentByMerchant/MFTCCardTopUpTransactionSummary";
            return View(vm);
        }
        [HttpPost]
        public ActionResult MFTCCardTopUpTransactionSummary([Bind(Include = MFTCCardTopUpTransactionSummaryViewModel.BindProperty)]MFTCCardTopUpTransactionSummaryViewModel vm)
        {
            var faxingDetials = Common.BusinessSession.FaxingAmountSummary;
            KiiPayPersonalWalletPaymentByKiiPayBusiness trans = new KiiPayPersonalWalletPaymentByKiiPayBusiness()
            {
                KiiPayBusinessWalletInformationId = vm.MerchantCardID,
                KiiPayBusinessInformationId = vm.KiiPayBusinessInformationId,
                KiiPayPersonalWalletInformationId = vm.CardUserId,
                ExchangeRate = faxingDetials.ExchangeRate,
                PayingAmount = faxingDetials.FaxingAmount,
                Fee = faxingDetials.FaxingFee,
                RecievingAmount = faxingDetials.ReceivingAmount,
                TotalAmount = faxingDetials.TotalAmount,
                PaymentReference = Common.BusinessSession.TopUpAmountViewModel.PaymentReference,
                TransactionDate = DateTime.Now,
                ReceiptNumber = _commonServices.ReceiptNoForMFTCPayment(),
                PaymentType = DB.PaymentType.International

            };
            var result = _mFTCCardPaymentByMerchantServices.SaveTransaction(trans);


            var DeductTheCredit = _commonServices.DeductTheCreditOnCard(trans.KiiPayBusinessWalletInformationId, trans.TotalAmount);

            var IncreaseBalanceonMFTC = _commonServices.IncreaseTheCreditBalanceonMFTC(trans.KiiPayPersonalWalletInformationId, trans.RecievingAmount);

            var ReceiverDetails = _commonServices.GetMFTCCardUserInformation(trans.KiiPayPersonalWalletInformationId);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var MFBCCardCardDetials = _commonServices.GetMFBCCardInformationByMFBCCardID(result.KiiPayBusinessWalletInformationId);


            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCTopUpConfirmation?CardUserName="
                + MFBCCardCardDetials.FirstName + " " + MFBCCardCardDetials.MiddleName + " " + MFBCCardCardDetials.LastName +
                "&ReceiverName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName +
                "&ReceiverCountry=" + Common.Common.GetCountryName(ReceiverDetails.CardUserCountry)
                + "&AccountNo=" + ReceiverDetails.MobileNo.Decrypt().GetVirtualAccountNo() + "&FaxingCurrency=" + Common.Common.GetCurrencySymbol(MFBCCardCardDetials.Country)
                + "&Amount=" + trans.TotalAmount + "&Fee=" + trans.Fee + "&IsLocalPayment=" + false);



            string URL = baseUrl + "/EmailTemplate/MFTCCardTopUpconfirmationPaymentReceipt?ReceiptNumber=" + trans.ReceiptNumber
                + "&Date=" + trans.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + trans.TransactionDate.ToString("HH:mm") + "&SenderName=" +
                MFBCCardCardDetials.FirstName + " " + MFBCCardCardDetials.MiddleName + " " + MFBCCardCardDetials.LastName
                + "&MFTCCardNo=" + ReceiverDetails.MobileNo.Decrypt().GetVirtualAccountNo()
                + "&MFTCCardName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName +
                "&TopUpAmount=" + trans.PayingAmount + "&Fees=" + trans.Fee +
                "&ExchangeRate=" + trans.ExchangeRate + "&TotalAmount=" + trans.TotalAmount + "&AmountInLocalCurrency=" + trans.RecievingAmount +
                 "&SendingCurrency=" + Common.Common.GetCountryCurrency(MFBCCardCardDetials.Country) + "&ReceivingCurrency=" + Common.Common.GetCountryName(ReceiverDetails.CardUserCountry);
            var Receipt = Common.Common.GetPdf(URL);


            mail.SendMail(MFBCCardCardDetials.Email, "Virtual account deposit confirmation ", body, Receipt);

            //Sms Function
            SmsApi smsApiServices = new SmsApi();
            string senderName = MFBCCardCardDetials.FirstName + " " + MFBCCardCardDetials.MiddleName + " " + MFBCCardCardDetials.LastName;
            string virtualAccounntNo = ReceiverDetails.MobileNo.Decrypt().GetVirtualAccountNo();
            string amount = Common.Common.GetCurrencySymbol(MFBCCardCardDetials.Country) + trans.TotalAmount;
            string receivingAmount = Common.Common.GetCountryName(ReceiverDetails.CardUserCountry) + trans.RecievingAmount;
            string message = smsApiServices.GetVirtualAccountDepositMessage(senderName,virtualAccounntNo,amount,receivingAmount);
            string phoneNumber = Common.Common.GetCountryPhoneCode(MFBCCardCardDetials.Country) + MFBCCardCardDetials.PhoneNumber;
            smsApiServices.SendSMS(phoneNumber, message);

            return RedirectToAction("PaymentSuccessFul");

        }
        public ActionResult PaymentSuccessFul()
        {
            var faxingDetials = Common.BusinessSession.FaxingAmountSummary;
            var CardUserDetails = _mFTCCardPaymentByMerchantServices.GetCardInformationByCardNumber(Common.BusinessSession.MFTCCardNumber);
            MFTCCardTopUpByMerchantSuccessfulViewModel vm = new MFTCCardTopUpByMerchantSuccessfulViewModel()
            {
                FaxingAmount = faxingDetials.FaxingAmount,
                ReceivingAmount = faxingDetials.ReceivingAmount,
                ExchangeRate = faxingDetials.ExchangeRate,
                CardUserCountry = Common.Common.GetCountryName(CardUserDetails.CardUserCountry),
                CardUserName = CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName,
                MFTCCardNumber = CardUserDetails.MobileNo.Decrypt().GetVirtualAccountNo(),
                ReceiveOption = "E-Card Withdrawal"

            };
            Session.Remove("FaxingAmountSummary");
            Session.Remove("TopUpAmountViewModel");
            Session.Remove("MFTCCardNumber");
            Session.Remove("TransactionSummaryURL");


            return View(vm);
        }
    }
}