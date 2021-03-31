using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.CardUsers.Services;
using FAXER.PORTAL.Areas.CardUsers.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class MFTCCardPaymentbyCardUserController : Controller
    {
        MFTCCardPaymentByCardUserServices _mFTCCardPaymentByCardUserServices = null;
        CardUserCommonServices _cardUserCommonServices = null;
        int MFTCCardID = Common.CardUserSession.LoggedCardUserViewModel == null ? 0 : Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;


        public MFTCCardPaymentbyCardUserController()
        {
            _cardUserCommonServices = new CardUserCommonServices();
            _mFTCCardPaymentByCardUserServices = new MFTCCardPaymentByCardUserServices();
        }
        // GET: CardUsers/MFTCCardPaymentbyCardUser
        public ActionResult Index()
        {

            if (!string.IsNullOrEmpty(Common.CardUserSession.MFTCCardNo))
            {

                ViewBag.MFTCCardNO = Common.CardUserSession.MFTCCardNo;
            }
            return View();
        }

        [HttpGet]
        public ActionResult MFTCCardHoderDetials(string MFTCCardNo = "")
        {

            MFTCCardHolderDetailsViewModel vm = new MFTCCardHolderDetailsViewModel();
            if (!string.IsNullOrEmpty(MFTCCardNo))
            {


                string[] tokens = MFTCCardNo.Split('-');
                var MFTCCardDetails = new DB.KiiPayPersonalWalletInformation();
                if (tokens.Length < 2)
                {
                    MFTCCardDetails = _mFTCCardPaymentByCardUserServices.GetCardInformationByCardNumber(MFTCCardNo.Trim());

                }
                else
                {
                    MFTCCardDetails = _mFTCCardPaymentByCardUserServices.GetMFTCCardInformationByCardNumber(MFTCCardNo);
                }



                if (MFTCCardDetails != null)
                {

                    if (MFTCCardDetails.Id == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId)
                    {
                        TempData["InvalidCardNumber"] = "Please enter a valid virtual account no";

                        return RedirectToAction("Index");
                    }
                    else if (MFTCCardDetails.CardStatus == DB.CardStatus.InActive)
                    {
                        TempData["InvalidCardNumber"] = "Virtual Account Deactivated, please contact MoneyFex Support";

                        return RedirectToAction("Index");
                    }
                    else if (MFTCCardDetails.CardStatus == DB.CardStatus.IsDeleted)
                    {
                        TempData["InvalidCardNumber"] = "Virtual Account Deleted, please contact MoneyFex Support";

                        return RedirectToAction("Index");
                    }
                    else if (MFTCCardDetails.CardStatus == DB.CardStatus.IsRefunded)
                    {
                        TempData["InvalidCardNumber"] = "Virtual Account Refunded, please contact MoneyFex Support";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        CommonServices _adminCardServices = new CommonServices();
                        vm = new MFTCCardHolderDetailsViewModel()
                        {
                            MFTCCardId = MFTCCardDetails.Id,
                            CardUserName = MFTCCardDetails.FirstName + " " + MFTCCardDetails.MiddleName + " " + MFTCCardDetails.LastName,
                            CardUserCity = MFTCCardDetails.CardUserCity,
                            CardUserCountry = Common.Common.GetCountryName(MFTCCardDetails.CardUserCountry),
                            CardUserMFTCCardNumber = MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo(),
                            Confirm = false,
                            Address = MFTCCardDetails.Address1,
                            CountryPhoneCode = _adminCardServices.getPhoneCodeFromCountry(MFTCCardDetails.CardUserCountry),
                            Phone = MFTCCardDetails.CardUserTel,
                            Email = MFTCCardDetails.CardUserEmail
                        };

                        Common.CardUserSession.ReceivingCountry = MFTCCardDetails.CardUserCountry;
                        Common.CardUserSession.ReceivingCurrency = Common.Common.GetCountryCurrency(MFTCCardDetails.CardUserCountry);
                        Common.CardUserSession.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(MFTCCardDetails.CardUserCountry);
                        Common.CardUserSession.MFTCCardHolderDetailsViewModel = vm;
                        Common.CardUserSession.MFTCCardNo = vm.CardUserMFTCCardNumber;

                    }
                }
                else
                {
                    TempData["InvalidCardNumber"] = "Please enter a valid account no.";

                    return RedirectToAction("Index");
                }
            }
            else
            {

                TempData["InvalidCardNumber"] = "Please enter a valid account no.";

                return RedirectToAction("Index");
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult MFTCCardHoderDetials([Bind(Include = MFTCCardHolderDetailsViewModel.BindProperty)]MFTCCardHolderDetailsViewModel vm)
        {
            if (vm.Confirm == false)
            {

                ModelState.AddModelError("Confirm", "Please confirm the account details ");

            }
            else
            {

                if (!string.IsNullOrEmpty(Common.CardUserSession.TransactionSummaryURL))
                {
                    return Redirect(Common.CardUserSession.TransactionSummaryURL);
                }
                else if (vm.CardUserCountry.ToLower() == Common.Common.GetCountryName(Common.CardUserSession.LoggedCardUserViewModel.Country).ToLower())
                {

                    return RedirectToAction("TopUpAmount", "MFTCCardLocalTopupByCardUser");
                }
                else
                {

                    return RedirectToAction("PayingAmount");
                }
            }

            return View(vm);
        }
        [HttpGet]
        public ActionResult PayingAmount()
        {
            CardUser_MFTCCardPaymentPayingAmountViewModel vm = new CardUser_MFTCCardPaymentPayingAmountViewModel();
            if (Common.CardUserSession.CardUser_MFTCCardPaymentPayingAmountViewModel != null)
            {

                vm = Common.CardUserSession.CardUser_MFTCCardPaymentPayingAmountViewModel;

            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult PayingAmount([Bind(Include = CardUser_MFTCCardPaymentPayingAmountViewModel.BindProperty)]CardUser_MFTCCardPaymentPayingAmountViewModel model)
        {
            CardUser_MFTCCardPaymentPayingAmountViewModel vm = new CardUser_MFTCCardPaymentPayingAmountViewModel();
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
                string FaxingCountryCode = Common.CardUserSession.FaxingCountry;
                string ReceivingCountryCode = Common.CardUserSession.ReceivingCountry;

                Common.CardUserSession.CardUser_MFTCCardPaymentPayingAmountViewModel = vm;
                
                decimal exchangeRate =  SExchangeRate.GetExchangeRateValue(FaxingCountryCode , ReceivingCountryCode); 

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
                return RedirectToAction("PaymentDetails");
            }
            return View();

        }
        public ActionResult PaymentDetails()
        {

            decimal CurrentBalanceOnCard = _cardUserCommonServices.getCurrentBalanceOnCard(MFTCCardID);

            EstimateFaxingFeeSummary faxingSummarry = new EstimateFaxingFeeSummary();
            faxingSummarry = Common.CardUserSession.FaxingAmountSummary;
            if (faxingSummarry.TotalAmount > CurrentBalanceOnCard)
            {

                TempData["InSufficientBalance"] = "Insufficient balance in account";
                return RedirectToAction("PayingAmount");
            }
            CardUser_MFTCCardPaymentDetailsViewModel model = new CardUser_MFTCCardPaymentDetailsViewModel()
            {
                FaxingFee = faxingSummarry.FaxingFee,
                AmountToBeReceived = faxingSummarry.ReceivingAmount,
                CurrentExchangeRate = faxingSummarry.ExchangeRate,
                faxingAmount = faxingSummarry.FaxingAmount,
                PaymentReference = Common.CardUserSession.CardUser_MFTCCardPaymentPayingAmountViewModel.PaymentReference,
                TotalAmountIncludingFee = faxingSummarry.TotalAmount,

            };
            return View(model);
            return View();
        }

        public ActionResult FraudAlertProtection()
        {

            return View();
        }

        [HttpGet]
        public ActionResult MFTCPaymentTransactionSummary()
        {

            // If user try to go back after the payment completion 
            // the user will  be redirected to there  dasboard
            if (Common.CardUserSession.MFTCCardPaymentByCardUserSuccessful == true)
            {

                return RedirectToAction("Index", "CardUserHome");
            }

            CardUser_MFTCPaymentTransactionSummaryViewModel vm = new CardUser_MFTCPaymentTransactionSummaryViewModel();
            #region Receiver Detials 
            var receiverDetails = Common.CardUserSession.MFTCCardHolderDetailsViewModel;
            vm.ReceiverName = receiverDetails.CardUserName;
            vm.ReceiverMFTCCardNumber = receiverDetails.CardUserMFTCCardNumber;
            #endregion

            #region Sender Details 
            var senderDetails = _cardUserCommonServices.GetMFTCCardUserInformation(MFTCCardID);
            vm.CardUserName = senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName;
            vm.CardUserEmail = senderDetails.CardUserEmail;
            vm.CardUserCountry = Common.Common.GetCountryName(senderDetails.CardUserCountry);
            vm.CardUserPhoneNumber = senderDetails.CardUserTel;
            vm.City = senderDetails.CardUserCity;
            vm.streetAddress = senderDetails.Address1;
            vm.State = senderDetails.CardUserState;
            vm.PostalCode = senderDetails.CardUserPostalCode;
            vm.SenderMFTCCardNumber = senderDetails.MobileNo.Decrypt().GetVirtualAccountNo();

            #endregion

            #region Transaction Details 

            var PaymentDetails = Common.CardUserSession.FaxingAmountSummary;

            vm.SentAmount = PaymentDetails.FaxingAmount.ToString();
            vm.Fees = PaymentDetails.FaxingFee.ToString();
            vm.TotalAmount = PaymentDetails.TotalAmount.ToString();
            vm.TotalReceiveAmount = PaymentDetails.ReceivingAmount.ToString();

            #endregion

            Common.CardUserSession.TransactionSummaryURL = "/CardUsers/MFTCCardPaymentbyCardUser/MFTCPaymentTransactionSummary";
            return View(vm);
        }

        [HttpPost]
        public ActionResult MFTCPaymentTransactionSummary([Bind(Include = CardUser_MFTCPaymentTransactionSummaryViewModel.BindProperty)]CardUser_MFTCPaymentTransactionSummaryViewModel vm)
        {

            var paymentDetails = Common.CardUserSession.FaxingAmountSummary;
            var receiverDetails = Common.CardUserSession.MFTCCardHolderDetailsViewModel;
            var senderDetails = _cardUserCommonServices.GetMFTCCardUserInformation(MFTCCardID);

            DB.KiiPayPersonalWalletPaymentByKiiPayPersonal transaction = new DB.KiiPayPersonalWalletPaymentByKiiPayPersonal()
            {

                SenderId = Common.CardUserSession.LoggedCardUserViewModel.id,
                SenderWalletId = senderDetails.Id,
                ReceiverWalletId = receiverDetails.MFTCCardId,
                ReceivingMobileNumber = Common.CardUserSession.MFTCCardNo.GetVirtualAccountNo(),
                FaxingAmount = paymentDetails.FaxingAmount,
                RecievingAmount = paymentDetails.ReceivingAmount,
                ExchangeRate = paymentDetails.ExchangeRate,
                FaxingFee = paymentDetails.FaxingFee,
                TotalAmount = paymentDetails.TotalAmount,
                PaymentReference = Common.CardUserSession.CardUser_MFTCCardPaymentPayingAmountViewModel.PaymentReference,
                TransactionDate = DateTime.Now,
                ReceiptNumber = _cardUserCommonServices.ReceiptNoForMFTCPayment(),
                PaymentType = DB.PaymentType.International
            };

            var result = _mFTCCardPaymentByCardUserServices.SaveTransaction(transaction);

            // deduct credit on card 
            //_cardUserCommonServices.DeductCreditOnCard(result.PayedFromMFTCCardId, result.TotalAmount);
            //_cardUserCommonServices.IncreaseCreditOnMFTCCard(result.PayedToMFTCCardId, result.RecievingAmount);


            var ReceiverDetails = _cardUserCommonServices.GetMFTCCardUserInformation(transaction.ReceiverWalletId);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCTopUpConfirmation?CardUserName="
                + senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName +
                "&ReceiverName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + ReceiverDetails.LastName +
                "&ReceiverCountry=" + Common.Common.GetCountryName(ReceiverDetails.CardUserCountry)
                + "&AccountNo=" + ReceiverDetails.MobileNo.Decrypt().GetVirtualAccountNo() + "&FaxingCurrency=" + Common.Common.GetCurrencySymbol(ReceiverDetails.CardUserCountry)
                + "&Amount=" + transaction.TotalAmount + "&Fee=" + transaction.FaxingFee);


            string URL = baseUrl + "/EmailTemplate/MFTCCardTopUpconfirmationPaymentReceipt?ReceiptNumber=" + transaction.ReceiptNumber
                + "&Date=" + transaction.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + transaction.TransactionDate.ToString("HH:mm") + "&SenderName=" +
                senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName
                + "&MFTCCardNo=" + ReceiverDetails.MobileNo.Decrypt().GetVirtualAccountNo()
                + "&MFTCCardName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName +
                "&TopUpAmount=" + transaction.FaxingAmount + "&Fees=" + transaction.FaxingFee +
                "&ExchangeRate=" + transaction.ExchangeRate + "&TotalAmount=" + transaction.TotalAmount + "&AmountInLocalCurrency=" + transaction.RecievingAmount
                + "&SendingCurrency=" + Common.Common.GetCountryCurrency(senderDetails.CardUserCountry) + "&ReceivingCurrency=" + Common.Common.GetCountryName(receiverDetails.CardUserCountry);
            var Receipt = Common.Common.GetPdf(URL);
            mail.SendMail(senderDetails.CardUserEmail, "Virtual Account Deposit Confirmation ", body, Receipt);


            //SMS Function
            SmsApi smsApiServices = new SmsApi();
            string senderName = senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName;
            string virtualAccounntNo = ReceiverDetails.MobileNo.Decrypt().GetVirtualAccountNo();
            string amount = Common.Common.GetCurrencySymbol(senderDetails.CardUserCountry) + transaction.TotalAmount;
            string receivingAmount = Common.Common.GetCurrencySymbol(ReceiverDetails.CardUserCountry) + transaction.RecievingAmount;
            //string paymentReference = vm.PaymentReference;

            string message = smsApiServices.GetVirtualAccountDepositMessage(senderName, virtualAccounntNo, amount, receivingAmount);
            string phoneNumber = Common.Common.GetCountryPhoneCode(senderDetails.CardUserCountry) + senderDetails.CardUserTel;
            smsApiServices.SendSMS(phoneNumber, message);


            // Check  Auto Top-Up is enable for The Logged In CardUser or Not 

            if (senderDetails.CurrentBalance == 0)
            {

                _cardUserCommonServices.SendMailWhenBalanceISZero(senderDetails.Id);
                if (senderDetails.AutoTopUp == true)
                {

                    _cardUserCommonServices.AutoTopUp(senderDetails.Id);
                }
            }


            return RedirectToAction("PaymentSuccessful");
            return View();
        }
        public ActionResult PaymentSuccessful()
        {


            var paymentDetails = Common.CardUserSession.FaxingAmountSummary;
            var receiverDetails = Common.CardUserSession.MFTCCardHolderDetailsViewModel;
            MFTCCardPaymentByCardUserSuccessfulViewModel vm = new MFTCCardPaymentByCardUserSuccessfulViewModel()
            {

                ReceiverName = receiverDetails.CardUserName,
                ReceiverCardNumber = receiverDetails.CardUserMFTCCardNumber.GetVirtualAccountNo(),
                ReceiverCountry = receiverDetails.CardUserCountry,
                ExchangeRate = paymentDetails.ExchangeRate.ToString(),
                ReceiveAmount = paymentDetails.ReceivingAmount.ToString(),
                TopUpAmount = paymentDetails.FaxingAmount.ToString(),
                Receiveoption = "E-Card withdrawal"
            };

            Session.Remove("TransactionSummaryURL");
            Session.Remove("MFTCCardHolderDetailsViewModel");
            Session.Remove("FaxingAmountSummary");
            Session.Remove("CardUser_MFTCCardPaymentPayingAmountViewModel");
            Common.CardUserSession.MFTCCardPaymentByCardUserSuccessful = true;
            return View(vm);
        }


    
    }
}