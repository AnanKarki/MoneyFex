using FAXER.PORTAL.Areas.CardUsers.Services;
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
    public class MFTCCardLocalTopupByCardUserController : Controller
    {
        Services.CardUserCommonServices _commonServices = null;
        MFTCCardPaymentByCardUserServices _mFTCCardPaymentByCardUserServices = null;
        public MFTCCardLocalTopupByCardUserController()
        {
            _commonServices = new Services.CardUserCommonServices();
            _mFTCCardPaymentByCardUserServices = new MFTCCardPaymentByCardUserServices();
        }
        // GET: CardUsers/MFTCCardLocalTopupByCardUser
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult TopUpAmount()
        {



            MFTCCardLocalTopupByCardUserViewModel vm = new ViewModels.MFTCCardLocalTopupByCardUserViewModel();
            if (Common.CardUserSession.MFTCCardLocalTopupByCardUserViewModel != null)
            {
                vm = Common.CardUserSession.MFTCCardLocalTopupByCardUserViewModel;
            }
            return View(vm);
        }
        public ActionResult TopUpAmount([Bind(Include = MFTCCardLocalTopupByCardUserViewModel.BindProperty)]MFTCCardLocalTopupByCardUserViewModel vm)
        {
            // Is Valid Amount to Transfer according to the withdrawal limit set by sender
            var ValidAmountAccordingToWithdrawalLimit = _commonServices.ValidAmountAccordingToWithdrawalLimit(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId, vm.FaxingAmount);
            if (ValidAmountAccordingToWithdrawalLimit == false) {

                ModelState.AddModelError("FaxingAmount", "Sorry! your cannot transfer more amount than your withdrawal limit ");
                return View(vm);


            }
            var currentbalance = _commonServices.getCurrentBalanceOnCard(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId);
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
                Common.CardUserSession.MFTCCardLocalTopupByCardUserViewModel = vm;
                return RedirectToAction("TransactionSummary");
            }

            return View();
            return View();
        }

        [HttpGet]
        public ActionResult TransactionSummary()
        {
            CardUser_MFTCPaymentTransactionSummaryViewModel vm = new CardUser_MFTCPaymentTransactionSummaryViewModel();
            #region Receiver Detials 
            var receiverDetails = Common.CardUserSession.MFTCCardHolderDetailsViewModel;
            vm.ReceiverName = receiverDetails.CardUserName;
            vm.ReceiverMFTCCardNumber = receiverDetails.CardUserMFTCCardNumber;
            #endregion

            #region Sender Details 
            var senderDetails = _commonServices.GetMFTCCardUserInformation(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId);
            vm.CardUserName = senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName;
            vm.CardUserEmail = senderDetails.CardUserEmail;
            vm.CardUserCountry = Common.Common.GetCountryName(senderDetails.CardUserCountry);
            vm.CardUserPhoneNumber = senderDetails.CardUserTel;
            vm.City = senderDetails.CardUserCity;
            vm.streetAddress = senderDetails.Address1;
            vm.State = senderDetails.CardUserState;
            vm.PostalCode = senderDetails.CardUserPostalCode;
            vm.SenderMFTCCardNumber = senderDetails.MobileNo.Decrypt().FormatMFTCCard();

            #endregion

            #region Transaction Details 

            var PaymentDetails = Common.CardUserSession.MFTCCardLocalTopupByCardUserViewModel;

            vm.TotalAmount = PaymentDetails.FaxingAmount.ToString();

            #endregion
            ViewBag.SendSMS = _commonServices.CheckBalanceForMessage(Convert.ToDecimal(vm.TotalAmount));
            Common.CardUserSession.TransactionSummaryURL = "/CardUsers/MFTCCardLocalTopupByCardUser/TransactionSummary";
            return View(vm);

        }

        [HttpPost]
        public ActionResult TransactionSummary([Bind(Include = CardUser_MFTCPaymentTransactionSummaryViewModel.BindProperty)]CardUser_MFTCPaymentTransactionSummaryViewModel vm)
        {
            decimal smsFee = 0;
            var paymentDetails = Common.CardUserSession.MFTCCardLocalTopupByCardUserViewModel;
            var receiverDetails = Common.CardUserSession.MFTCCardHolderDetailsViewModel;
            var senderDetails = _commonServices.GetMFTCCardUserInformation(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId);
            ViewBag.SendSMS = _commonServices.CheckBalanceForMessage(Convert.ToDecimal(paymentDetails.FaxingAmount));

            DB.KiiPayPersonalWalletPaymentByKiiPayPersonal transaction = new DB.KiiPayPersonalWalletPaymentByKiiPayPersonal()
            {
                SenderId = Common.CardUserSession.LoggedCardUserViewModel.id,
                SenderWalletId = senderDetails.Id,
                ReceiverWalletId = receiverDetails.MFTCCardId,
                FaxingAmount = paymentDetails.FaxingAmount,
                RecievingAmount = paymentDetails.FaxingAmount,
                ExchangeRate = 0,
                FaxingFee = 0,
                TotalAmount = paymentDetails.FaxingAmount,
                PaymentReference = paymentDetails.PaymentReference,
                TransactionDate = DateTime.Now,
                ReceiptNumber = _commonServices.ReceiptNoForMFTCPayment(),
                PaymentType = DB.PaymentType.Local
            };

            var result = _mFTCCardPaymentByCardUserServices.SaveTransaction(transaction);

            // deduct credit on card 

            if(vm.SendSms == true)
            {
                smsFee = Common.Common.GetSmsFee(senderDetails.CardUserCountry);
            }
            if (senderDetails.CurrentBalance < transaction.TotalAmount + smsFee)
            {
                smsFee = 0;
                vm.SendSms = false;
            }
            //_commonServices.DeductCreditOnCard(result.PayedFromMFTCCardId, result.TotalAmount + smsFee);
            //_commonServices.IncreaseCreditOnMFTCCard(result.PayedToMFTCCardId, result.TotalAmount);

            var ReceiverDetails = _commonServices.GetMFTCCardUserInformation(transaction.ReceiverWalletId);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCTopUpConfirmation?CardUserName="
                + senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName +
                "&ReceiverName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + ReceiverDetails.LastName +
                "&ReceiverCountry=" + Common.Common.GetCountryName(ReceiverDetails.CardUserCountry) +
                "&AccountNo=" + ReceiverDetails.MobileNo.Decrypt().GetVirtualAccountNo() + "&FaxingCurrency=" + Common.Common.GetCurrencySymbol(ReceiverDetails.CardUserCountry)
                + "&Amount=" + transaction.TotalAmount + "&Fee=" + 0  + "&IsLocalPayment=" + true);
           


            mail.SendMail(senderDetails.CardUserEmail, "Virtual Account Deposit  Confirmation ", body);
            if(vm.SendSms == true)
            {
                SmsApi smsApiServices = new SmsApi();
                string senderName = senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName;
                string amount = Common.Common.GetCurrencySymbol(senderDetails.CardUserCountry) + transaction.TotalAmount;
                string virtualAccountNo = ReceiverDetails.MobileNo.Decrypt().GetVirtualAccountNo();
                string receivingAmount = Common.Common.GetCurrencySymbol(senderDetails.CardUserCountry) + transaction.TotalAmount;

                string message = smsApiServices.GetVirtualAccountDepositMessage(senderName, virtualAccountNo,amount,receivingAmount);
                string senderPhoneNumber = Common.Common.GetCountryPhoneCode(senderDetails.CardUserCountry) + senderDetails.CardUserTel;
                string receiverPhoneNumber = Common.Common.GetCountryPhoneCode(ReceiverDetails.CardUserCountry) + ReceiverDetails.CardUserTel;
                smsApiServices.SendSMS(senderPhoneNumber, message);
                smsApiServices.SendSMS(receiverPhoneNumber, message);
            }

            // Check  Auto Top-Up is enable for The Logged In CardUser or Not 

            if (senderDetails.CurrentBalance == 0)
            {

                _commonServices.SendMailWhenBalanceISZero(senderDetails.Id);


                if (senderDetails.AutoTopUp == true)
                {

                    _commonServices.AutoTopUp(senderDetails.Id);
                }
            }
          

            return RedirectToAction("PaymentSuccessful");
            return View();
        }

        public ActionResult PaymentSuccessful() {
            var paymentDetails = Common.CardUserSession.MFTCCardLocalTopupByCardUserViewModel;
            var receiverDetails = Common.CardUserSession.MFTCCardHolderDetailsViewModel;
            MFTCCardPaymentByCardUserSuccessfulViewModel vm = new MFTCCardPaymentByCardUserSuccessfulViewModel()
            {

                ReceiverName = receiverDetails.CardUserName,
                ReceiverCardNumber = receiverDetails.CardUserMFTCCardNumber.GetVirtualAccountNo(),
                ReceiverCountry = receiverDetails.CardUserCountry,
                TopUpAmount = paymentDetails.FaxingAmount.ToString(),
                Receiveoption = "E-Card withdrawal"
            };
            Session.Remove("TransactionSummaryURL");
            Session.Remove("MFTCCardHolderDetailsViewModel");
            Session.Remove("MFTCCardLocalTopupByCardUserViewModel");
            return View(vm);

        }



    }
}