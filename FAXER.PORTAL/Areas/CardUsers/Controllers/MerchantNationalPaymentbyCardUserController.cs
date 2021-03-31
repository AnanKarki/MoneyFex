using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.CardUsers.Services;
using FAXER.PORTAL.Areas.CardUsers.ViewModels;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class MerchantNationalPaymentbyCardUserController : Controller
    {
        DB.FAXEREntities context = new DB.FAXEREntities();
        CardUserCommonServices _cardUserCommonServices = null;
        CardUserMerchantNationalPaymentServices _cardUserMerchantNationalPaymentServices = null;
        int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel == null ? 0 : Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;

        public MerchantNationalPaymentbyCardUserController()
        {
            _cardUserCommonServices = new CardUserCommonServices();
            _cardUserMerchantNationalPaymentServices = new CardUserMerchantNationalPaymentServices();
        }
        // GET: CardUsers/MerchantNationalPaymentbyCardUser
        [HttpGet]
        public ActionResult Index()
        {
            CardUserPreviousPayeeViewModel vm = new CardUserPreviousPayeeViewModel();
            var PreviousPayee = _cardUserCommonServices.getPreviousPayees();
            ViewBag.PreviousPayee = new SelectList(PreviousPayee, "BusinessMFCode", "Name");
            Common.CardUserSession.BackButtonURL = "/CardUsers/MerchantNationalPaymentbyCardUser/Index";
            if (!string.IsNullOrEmpty(Common.CardUserSession.MerchantAccountNumber))
            {

                vm.BusinessMFCode = Common.CardUserSession.MerchantAccountNumber;
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index( [Bind(Include = CardUserPreviousPayeeViewModel.BindProperty)]CardUserPreviousPayeeViewModel vm)
        {
            if (!string.IsNullOrEmpty(vm.BusinessMFCode))
            {
                Common.CardUserSession.MerchantAccountNumber = vm.BusinessMFCode;
                return RedirectToAction("MerchantDetials", new { MerchantAccountNo = vm.BusinessMFCode });
            }
            else
            {

                return RedirectToAction("SearchMerchantByAccountNo");
            }
            
        }

        public ActionResult SearchMerchantByAccountNo(string MerchantAccountNo)
        {

            if (!string.IsNullOrEmpty(Common.CardUserSession.MerchantAccountNumber))
            {
                ViewBag.MerchantAccountNo = Common.CardUserSession.MerchantAccountNumber;
                ViewBag.BusinessName = Common.CardUserSession.MerchantName;
            }

            Common.CardUserSession.BackButtonURL = "/CardUsers/MerchantNationalPaymentbyCardUser/SearchMerchantByAccountNo";
            return View();
        }
        [HttpGet]
        public ActionResult getMerchants(string term)
        {
            var data = context.KiiPayBusinessInformation.Where(x => x.BusinessName.StartsWith(term) && x.BusinessOperationCountryCode == Common.CardUserSession.LoggedCardUserViewModel.Country);
            if (data.Count() > 0)
            {
                return Json(context.KiiPayBusinessInformation.Where(x => x.BusinessName.StartsWith(term) && x.BusinessOperationCountryCode == Common.CardUserSession.LoggedCardUserViewModel.Country).Select(a => new { label = a.BusinessName, id = a.BusinessMobileNo }),
                    JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(new { label = "No Result Found", id = 0 },
                JsonRequestBehavior.AllowGet);

            }
        }


        [HttpGet]
        public ActionResult MerchantDetials(string MerchantAccountNo = "")
        {

            ViewModels.MerchantDetailsViewModel_CardUserViewModel vm = new ViewModels.MerchantDetailsViewModel_CardUserViewModel();
            if (string.IsNullOrEmpty(MerchantAccountNo))
            {

                TempData["InvalidMFBCCard"] = "Plese enter a valid card number";
                return RedirectToAction("SearchMerchantByAccountNo");

            }
            else
            {
                var merchantDetails = _cardUserCommonServices.GetBusinessInformation(MerchantAccountNo);
                if (merchantDetails == null)
                {
                    TempData["InvalidMFBCCard"] = "Plese enter a valid card number";
                    return RedirectToAction("SearchMerchantByAccountNo");

                }
                var receiverCardDetails = _cardUserCommonServices.GetMFBCCardInformation(merchantDetails.Id);
                if (receiverCardDetails == null)
                {

                    TempData["InvalidMFBCCard"] = "This merchant does not have a withdrawal card to accept payments";
                    return RedirectToAction("SearchMerchantByAccountNo");

                }
                if (receiverCardDetails.CardStatus == DB.CardStatus.InActive)
                {

                    TempData["InvalidMFBCCard"] = "This card has been deactivated";
                    return RedirectToAction("SearchMerchantByAccountNo");


                }

                if (_cardUserCommonServices.GetMFBCCardInformation(merchantDetails.Id).Country != Common.CardUserSession.LoggedCardUserViewModel.Country)
                {

                    TempData["InvalidMFBCCard"] = "The service provider is not of your country , choose international payment option";
                    return RedirectToAction("SearchMerchantByAccountNo");

                }
                CommonServices _adminCommonServices = new CommonServices();
                vm = new ViewModels.MerchantDetailsViewModel_CardUserViewModel()
                {
                    KiiPayBusinessInformationId = merchantDetails.Id,
                    MerchantAccountNo = merchantDetails.BusinessMobileNo,
                    City = merchantDetails.BusinessOperationCity,
                    Country = Common.Common.GetCountryName(merchantDetails.BusinessOperationCountryCode),
                    MerchantName = merchantDetails.BusinessName,
                    MFBCCardID = _cardUserCommonServices.GetMFBCCardInformation(merchantDetails.Id).Id,
                    Address = merchantDetails.BusinessOperationAddress1,
                    Email = merchantDetails.Email,
                    Website = merchantDetails.Website,
                    PhoneNo = merchantDetails.PhoneNumber,
                    CountryPhoneCode = _adminCommonServices.getPhoneCodeFromCountry(merchantDetails.BusinessOperationCountryCode)
                };
                Common.CardUserSession.MerchantDetailsViewModel_CardUserViewModel = vm;
                Common.CardUserSession.MerchantAccountNumber = vm.MerchantAccountNo;
                Common.CardUserSession.MerchantName = vm.MerchantName;

            }
            return View(vm);
            return View();
        }
        [HttpPost]
        public ActionResult MerchantDetials([Bind(Include = MerchantDetailsViewModel_CardUserViewModel.BindProperty)] MerchantDetailsViewModel_CardUserViewModel vm)
        {

            if (ModelState.IsValid)
            {

                if (vm.confirm == false)
                {
                    ModelState.AddModelError("confirm", "Please accept the terms and condition");

                }
                else
                {

                    if (!string.IsNullOrEmpty(Common.CardUserSession.TransactionSummaryURL))
                    {
                        return Redirect(Common.CardUserSession.TransactionSummaryURL);
                    }
                    return RedirectToAction("PayingAmount");
                }
            }
            return View(vm);
            return View();
        }

        [HttpGet]
        public ActionResult PayingAmount()
        {
            MerchantNationalPayingAmount_CardUserViewModel vm = new MerchantNationalPayingAmount_CardUserViewModel();
            if (Common.CardUserSession.MerchantNationalPayingAmount_CardUserViewModel != null)
            {
                vm = Common.CardUserSession.MerchantNationalPayingAmount_CardUserViewModel;
            }
            return View(vm);
            return View();
        }
        [HttpPost]
        public ActionResult PayingAmount([Bind(Include = MerchantNationalPayingAmount_CardUserViewModel.BindProperty)]MerchantNationalPayingAmount_CardUserViewModel vm)
        {
            // Check whether the paying Amount is valid according to purchase Limit
            var validAmountAccordingToPurchaseLimit = _cardUserCommonServices.ValidAmountAccordingToPurchaseLimit(MFTCCardId, vm.FaxingAmount);

            if (validAmountAccordingToPurchaseLimit == false)
            {

                ModelState.AddModelError("FaxingAmount", "Sorry, You have exceeded your purchase limit");
                return View(vm);
            }

            // Check for sufficient balance
            var currentbalance = _cardUserCommonServices.getCurrentBalanceOnCard(MFTCCardId);
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
                Common.CardUserSession.MerchantNationalPayingAmount_CardUserViewModel = vm;
                return RedirectToAction("MerchantNationalPaymentTransactionSummary");
            }

            return View();
        }
        [HttpGet]
        public ActionResult MerchantNationalPaymentTransactionSummary()
        {
            CardUser_MerchantNationalPaymentTransactionSummaryViewModel vm = new CardUser_MerchantNationalPaymentTransactionSummaryViewModel();
            #region Receiver Merchant Details 
            var receiverDetails = Common.CardUserSession.MerchantDetailsViewModel_CardUserViewModel;
            vm.ReceiverName = receiverDetails.MerchantName;
            vm.ReceiverAccountNo = receiverDetails.MerchantAccountNo;
            #endregion

            #region Sender Card user Information 

            var SenderDetials = _cardUserCommonServices.GetMFTCCardUserInformation(MFTCCardId);
            vm.CardUserName = SenderDetials.FirstName + " " + SenderDetials.MiddleName + " " + SenderDetials.LastName;
            vm.CardUserPhoneNumber = Common.Common.GetCountryPhoneCode(SenderDetials.CardUserCountry) + " " + SenderDetials.CardUserTel;
            vm.CardUserEmail = SenderDetials.CardUserEmail;
            vm.City = SenderDetials.CardUserCity;
            vm.streetAddress = SenderDetials.Address1;
            vm.State = SenderDetials.CardUserState;
            vm.PostalCode = SenderDetials.CardUserPostalCode;
            vm.CardUserCountry = Common.Common.GetCountryName(SenderDetials.CardUserCountry);
            vm.SenderMFTCCardNumber = SenderDetials.MobileNo.Decrypt().FormatMFTCCard();

            #endregion

            #region Transaction Information 
            vm.TotalAmount = Common.CardUserSession.MerchantNationalPayingAmount_CardUserViewModel.FaxingAmount.ToString();
            #endregion
            ViewBag.SendSMS = _cardUserCommonServices.CheckBalanceForMessage(Common.CardUserSession.MerchantNationalPayingAmount_CardUserViewModel.FaxingAmount);
            Common.CardUserSession.TransactionSummaryURL = "/CardUsers/MerchantNationalPaymentbyCardUser/MerchantNationalPaymentTransactionSummary";
            return View(vm);


            return View();

        }

        public ActionResult MerchantNationalPaymentTransactionSummary([Bind(Include = CardUser_MerchantNationalPaymentTransactionSummaryViewModel.BindProperty)]CardUser_MerchantNationalPaymentTransactionSummaryViewModel vm)
        {

            var PaymentDetails = Common.CardUserSession.MerchantNationalPayingAmount_CardUserViewModel;
            var MerchantDetails = Common.CardUserSession.MerchantDetailsViewModel_CardUserViewModel;
            decimal smsFee = 0;
            ViewBag.SendSMS = _cardUserCommonServices.CheckBalanceForMessage(Convert.ToDecimal(PaymentDetails.FaxingAmount));
            DB.KiiPayPersonalNationalKiiPayBusinessPayment transaction = new DB.KiiPayPersonalNationalKiiPayBusinessPayment()
            {
                AmountSent = PaymentDetails.FaxingAmount,
                PaymentReference = PaymentDetails.PaymentReference,
                TransactionDate = DateTime.Now,
                KiiPayPersonalWalletInformationId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId,
                KiiPayBusinessWalletInformationId = MerchantDetails.MFBCCardID
            };

            var result = _cardUserMerchantNationalPaymentServices.SaveTransaction(transaction);


            var senderDetails = _cardUserCommonServices.GetMFTCCardUserInformation(transaction.KiiPayPersonalWalletInformationId);

            // Deduct credit of Card Holder

            if (vm.SendSms == true)
            {
                smsFee = Common.Common.GetSmsFee(result.KiiPayPersonalWalletInformation.CardUserCountry);
            }
            if (senderDetails.CurrentBalance < (transaction.AmountSent + smsFee))
            {
                smsFee = 0;
                vm.SendSms = false;
            }
            Common.Common.DeductCreditOnCard(transaction.KiiPayPersonalWalletInformationId, transaction.AmountSent + smsFee);
            // Increse the credit of merchant card
            Common.Common.IncreaseBalanceOnMFBCCard(transaction.KiiPayBusinessWalletInformationId, transaction.AmountSent);


           var ReceiverDetails = _cardUserCommonServices.GetMFTCCardInformationByCardid(transaction.KiiPayBusinessWalletInformationId);
         

            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationPaymentServiceProvider?SenderName=" +
             senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName +
             "&ReceivingAmount=" + transaction.AmountSent + "&ReceiverBusinessName=" + ReceiverDetails.KiiPayBusinessInformation.BusinessName
             + "&ReceiverCountry=" + Common.Common.GetCountryName(ReceiverDetails.Country) + "&ReceivingCurrency=" + Common.Common.GetCountryCurrency(ReceiverDetails.Country));


            // Check  Auto Top-Up is enable for The Logged In CardUser or Not 

            if (senderDetails.CurrentBalance == 0)
            {

                _cardUserCommonServices.SendMailWhenBalanceISZero(senderDetails.Id);

                if (senderDetails.AutoTopUp == true)
                {

                    _cardUserCommonServices.AutoTopUp(senderDetails.Id);
                }
            }
            

            mail.SendMail(senderDetails.CardUserEmail, "Confirmation of Payment to Service Provider ", body);
            if (vm.SendSms == true)
            {
                SmsApi smsApiServices = new SmsApi();
                string senderName = senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName;
                string businessAccounntNo = ReceiverDetails.KiiPayBusinessInformation.BusinessMobileNo;
                string businessName = ReceiverDetails.KiiPayBusinessInformation.BusinessName;
                string amount = Common.Common.GetCurrencySymbol(senderDetails.CardUserCountry) + transaction.AmountSent;
                string paymentReference = transaction.PaymentReference;
                string receivingAmount = Common.Common.GetCurrencySymbol(senderDetails.CardUserCountry) + transaction.AmountSent;

                string message = smsApiServices.GetBusinessPaymentMessage(senderName, businessAccounntNo, businessName, amount, paymentReference);
                string senderPhoneNumber = Common.Common.GetCountryPhoneCode(senderDetails.CardUserCountry) + senderDetails.CardUserTel;
                string receiverPhoneNumber = Common.Common.GetCountryPhoneCode(ReceiverDetails.Country) + ReceiverDetails.PhoneNumber;
                smsApiServices.SendSMS(senderPhoneNumber, message);
                smsApiServices.SendSMS(receiverPhoneNumber, message);
            }

            return RedirectToAction("PaymentSuccessful");



        }


        public ActionResult PaymentSuccessful()
        {
            var MerchantDetails = Common.CardUserSession.MerchantDetailsViewModel_CardUserViewModel;

            var paymentDetails = Common.CardUserSession.MerchantNationalPayingAmount_CardUserViewModel;
            MerchantInternationPaymentSuccessfulByCarduserViewModel vm = new MerchantInternationPaymentSuccessfulByCarduserViewModel()
            {
                MerchantName = MerchantDetails.MerchantName,
                MerchantAccountNo = MerchantDetails.MerchantAccountNo,
                Country = MerchantDetails.Country,
                PaymentReference = paymentDetails.PaymentReference,
                TotalAmount = paymentDetails.FaxingAmount.ToString(),
                ReceiveOption = "MFBC Card Withdrawl"

            };
            Session.Remove("MerchantNationalPayingAmount_CardUserViewModel");
            Session.Remove("MerchantDetailsViewModel_CardUserViewModel");
            Session.Remove("TransactionSummaryURL");
            Session.Remove("ReceivingCountry");
            Session.Remove("MerchantAccountNumber");
            Session.Remove("MerchantName");
            return View(vm);
        }

        [HttpGet]
        public JsonResult CheckBalanceForMessage(decimal trasnctionAmount)
        {
            bool data = _cardUserCommonServices.CheckBalanceForMessage(trasnctionAmount);
            return Json(new
            {
                send = data

            }, JsonRequestBehavior.AllowGet);
        }

    }
}