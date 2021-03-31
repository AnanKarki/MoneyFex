using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Controllers.Transfer
{
    public class SenderFamilyAndFriendsController : Controller
    {
        SSenderFamilyAndFriends _familyAndFriendsTransferServices = null;
        SSenderKiiPayWalletTransfer _kiiPayWalletTransferServices = null;
        public SenderFamilyAndFriendsController()
        {
            _familyAndFriendsTransferServices = new SSenderFamilyAndFriends();
            _kiiPayWalletTransferServices = new SSenderKiiPayWalletTransfer();
        }
        // GET: SenderFamilyAndFriends
        public ActionResult Index()
        {
            var registeredWallets = _familyAndFriendsTransferServices.GetRegisteredWallets();
            ViewBag.RegisteredWallets = new SelectList(registeredWallets, "Code", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = SearchKiiPayWalletVM.BindProperty)] SearchKiiPayWalletVM vm)
        {
            var registeredWallets = _familyAndFriendsTransferServices.GetRegisteredWallets();
            ViewBag.RegisteredWallets = new SelectList(registeredWallets, "Code", "Name");
            vm.CountryCode = Common.FaxerSession.LoggedUser.CountryCode;


            if (ModelState.IsValid)
            {


                var result = _kiiPayWalletTransferServices.ValidateMobileNo(vm.MobileNo,vm.CountryCode);
                if (result == null)
                {
                    ModelState.AddModelError("MobileNo", "Enter valid mobile no");

                    return View(vm);
                }

                // Set Sender And Receiver Details in session 

                SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
                _kiiPayWalletTransferServices.SetSenderAndReceiverDetails(new SenderAndReceiverDetialVM()
                {

                    ReceiverCountry = result.CardUserCountry,
                    ReceiverId = result.Id,
                    SenderCountry = Common.FaxerSession.LoggedUser.CountryCode,
                    SenderId = Common.FaxerSession.LoggedUser.Id,
                    ReceiverMobileNo = result.MobileNo,
                    SenderWalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id).Id
                });

                // set Sending Currency And Receiving Currency 
                string ReceivingCountry = result.CardUserCountry;
                string SendingCountry = Common.FaxerSession.LoggedUser.CountryCode;
                _kiiPayWalletTransferServices.SetKiiPayTransferPaymentSummary(new KiiPayTransferPaymentSummary()
                {
                    ReceiverName = result.FirstName + " " + result.MiddleName + " " + result.LastName,
                    ReceivingCurrency = Common.Common.GetCountryCurrency(ReceivingCountry),
                    ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(ReceivingCountry),
                    SendingCurrency = Common.Common.GetCountryCurrency(SendingCountry),
                    SendingCurrencySymbol = Common.Common.GetCurrencySymbol(SendingCountry),
                    ExchangeRate = SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry),
                });

                _kiiPayWalletTransferServices.SetSearchKiiPayWallet(vm);

                
                if (ReceivingCountry.ToLower() == SendingCountry.ToLower()) {
                    return RedirectToAction("LocalEnterAmount", "SenderKiiPayWalletTransfer");

                }

                return RedirectToAction("InternationalEnterAmount", "SenderKiiPayWalletTransfer");
            }
            //if (ModelState.IsValid)
            //{
            //    _familyAndFriendsTransferServices.SetSenderTransferFamilyAndFriends(model);


            //    var receiversData = _familyAndFriendsTransferServices.GetReceiversDataFromWalletId(model.WalletId);
            //    //SetData in EnterAmount Session

            //    var loggedInSenderData = _familyAndFriendsTransferServices.GetLoggedUserData();
            //    SenderMobileEnrterAmountVm enterAmountData = new SenderMobileEnrterAmountVm()
            //    {
            //        ReceiverName = loggedInSenderData.FullName,
            //        ReceiverId = loggedInSenderData.Id,
            //        SendingCurrencySymbol = Common.Common.GetCurrencySymbol(loggedInSenderData.CountryCode),
            //        SendingCurrencyCode = Common.Common.GetCountryCurrency(loggedInSenderData.CountryCode),

            //        //Use receivers country code later


            //        ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol("IND"),
            //        ReceivingCurrencyCode = Common.Common.GetCountryCurrency("IND"),
            //        ExchangeRate = Common.Common.GetExchangeRate(loggedInSenderData.CountryCode, "IND")
            //    };
            //    _familyAndFriendsTransferServices.SetSenderTransferFamilyAndFriendsEnrterAmount(enterAmountData);

            //    return RedirectToAction("TransferFamilyFriendsEnterAmount", "SenderFamilyAndFriends");

            //}
            return View(vm);
        }


        public ActionResult TransferFamilyFriendsEnterAmount()
        {
            var enterAmountData = _familyAndFriendsTransferServices.GetSenderTransferFamilyAndFriendsEnrterAmount();
            SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm()
            {
                ReceiverName = enterAmountData.ReceiverName,
                ReceiverId = enterAmountData.ReceiverId,
                SendingCurrencySymbol = enterAmountData.SendingCurrencySymbol,
                SendingCurrencyCode = enterAmountData.SendingCurrencyCode,
                ReceivingCurrencySymbol = enterAmountData.ReceivingCurrencySymbol,
                ReceivingCurrencyCode = enterAmountData.ReceivingCurrencyCode,
                ExchangeRate = enterAmountData.ExchangeRate,
                PaymentReference = enterAmountData.PaymentReference
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult TransferFamilyFriendsEnterAmount([Bind(Include = SenderMobileEnrterAmountVm.BindProperty)] SenderMobileEnrterAmountVm model)
        {
            if (ModelState.IsValid)
            {
                _familyAndFriendsTransferServices.SetSenderTransferFamilyAndFriendsEnrterAmount(model);
                return RedirectToAction("WalletPaymentSummary", "SenderFamilyAndFriends");
            }
            return View(model);
        }


        public ActionResult WalletPaymentSummary()
        {
            var sendingAmountData = _familyAndFriendsTransferServices.GetSenderTransferFamilyAndFriendsEnrterAmount();

            SenderAccountPaymentSummaryViewModel vm = new SenderAccountPaymentSummaryViewModel()
            {
                Amount = sendingAmountData.SendingAmount,
                Fee = sendingAmountData.Fee,
                ReceivedAmount = sendingAmountData.ReceivingAmount,
                SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
                SendingCurrencyCode = sendingAmountData.SendingCurrencyCode,
                PaidAmount = sendingAmountData.TotalAmount,
                PaymentReference = sendingAmountData.PaymentReference,
                ReceiverName= sendingAmountData.ReceiverName
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult WalletPaymentSummary([Bind(Include = SenderAccountPaymentSummaryViewModel.BindProperty)] SenderAccountPaymentSummaryViewModel model)
        {
            return RedirectToAction("InternationalPayNow", "SenderFamilyAndFriends");

        }

        public ActionResult InternationalPayNow()
        {
            var vm = _familyAndFriendsTransferServices.GetPaymentMethod();
            var sendsingAmountData = _familyAndFriendsTransferServices.GetSenderTransferFamilyAndFriendsEnrterAmount();
            vm.TotalAmount = sendsingAmountData.TotalAmount;
            vm.SendingCurrencySymbol = sendsingAmountData.SendingCurrencySymbol;
            return View(vm);
        }


        [HttpPost]
        public ActionResult InternationalPayNow([Bind(Include = PaymentMethodViewModel.BindProperty)] PaymentMethodViewModel vm)
        {

            if (ModelState.IsValid)
            {

                _familyAndFriendsTransferServices.SetPaymentMethod(vm);

                if (vm.SenderPaymentMode == SenderPaymentMode.CreditDebitCard)
                {
                    return RedirectToAction("DebitCreditCardDetails", "SenderFamilyAndFriends");
                }

                if (vm.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
                {
                    return RedirectToAction("MoneyFexBankDeposit", "SenderFamilyAndFriends");
                }
                return RedirectToAction("WalletPaymentSuccess", "SenderFamilyAndFriends");
            }

            return View(vm);
        }


        public ActionResult DebitCreditCardDetails()
        {
            var addresses = _familyAndFriendsTransferServices.GetAddress();
            ViewBag.Address = new SelectList(addresses, "Name", "Name");

            CreditDebitCardViewModel vm = new CreditDebitCardViewModel();
            var sendingAmountData = _familyAndFriendsTransferServices.GetSenderTransferFamilyAndFriendsEnrterAmount();
            vm.FaxingAmount = sendingAmountData.TotalAmount;
            vm.FaxingCurrencySymbol = sendingAmountData.SendingCurrencySymbol;
            vm.FaxingCurrency = sendingAmountData.SendingCurrencyCode;

            return View(vm);
        }


        [HttpPost]
        public ActionResult DebitCreditCardDetails([Bind(Include = CreditDebitCardViewModel.BindProperty)] CreditDebitCardViewModel model)
        {
            var addresses = _familyAndFriendsTransferServices.GetAddress();
            ViewBag.Address = new SelectList(addresses, "Name", "Name");

            if (ModelState.IsValid)
            {
                _familyAndFriendsTransferServices.SetDebitCreditCardDetail(model);

                return RedirectToAction("WalletPaymentSuccess", "SenderFamilyAndFriends");
            }
            return View(model);
        }


        public ActionResult MoneyFexBankDeposit()
        {
            SenderMoneyFexBankDepositVM vm = new SenderMoneyFexBankDepositVM();

            var sendingAmountData = _familyAndFriendsTransferServices.GetSenderTransferFamilyAndFriendsEnrterAmount();

            vm.Amount = sendingAmountData.SendingAmount;
            vm.SendingCurrencyCode = sendingAmountData.SendingCurrencyCode;
            vm.SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol;

            return View(vm);
        }

        [HttpPost]
        public ActionResult MoneyFexBankDeposit([Bind(Include = SenderMoneyFexBankDepositVM.BindProperty)]SenderMoneyFexBankDepositVM model)
        {
            if (ModelState.IsValid)
            {
                _familyAndFriendsTransferServices.SetMoneyFexBankAccountDeposit(model);
                return RedirectToAction("WalletPaymentSuccess", "SenderFamilyAndFriends");
            }

            return View(model);
        }

        public ActionResult WalletPaymentSuccess()
        {
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();

            // Data to show on success

            var sendingAmountData = _familyAndFriendsTransferServices.GetSenderTransferFamilyAndFriendsEnrterAmount();
            var transferFamilyFriends = _familyAndFriendsTransferServices.GetSenderTransferFamilyAndFriends();

            SenderCashPickUpSuccessVm vm = new SenderCashPickUpSuccessVm()
            {
                SentAmount = sendingAmountData.SendingAmount,
                SendingCurrency = sendingAmountData.SendingCurrencySymbol,
                ReceiverName = sendingAmountData.ReceiverName,
            };

            //Completing Transaction
            var loggedUserData = _familyAndFriendsTransferServices.GetLoggedUserData();
            var paymentMethod = _familyAndFriendsTransferServices.GetPaymentMethod();

            TransactionSummaryVM transactionSummaryVm = new TransactionSummaryVM();


            // Get receiver Data

            transactionSummaryVm.SenderAndReceiverDetail = new SenderAndReceiverDetialVM()
            {
                SenderId = loggedUserData.Id,
                SenderCountry = loggedUserData.CountryCode,
                ReceiverCountry = "",
                ReceiverId = sendingAmountData.ReceiverId
            };

            //Set Sms Fee 
            transactionSummaryVm.KiiPayTransferPaymentSummary = new KiiPayTransferPaymentSummary()
            {
                ReceiverName = sendingAmountData.ReceiverName,
                SendingCurrency = sendingAmountData.SendingCurrencyCode,
                SendingAmount = sendingAmountData.SendingAmount,
                ReceivingAmount = sendingAmountData.ReceivingAmount,
                TotalAmount = sendingAmountData.TotalAmount,
                ExchangeRate = sendingAmountData.ExchangeRate,
                Fee = sendingAmountData.Fee,
                PaymentReference = "",
                ReceivingCurrency = sendingAmountData.ReceivingCurrencyCode,
                ReceivingCurrencySymbol = sendingAmountData.ReceivingCurrencySymbol,
                SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
                SendSMS = true,
                SMSFee = 0,
            };


            transactionSummaryVm.PaymentMethodAndAutoPaymentDetail = new PaymentMethodViewModel()
            {
                TotalAmount = sendingAmountData.TotalAmount,
                SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
                SenderPaymentMode = paymentMethod.SenderPaymentMode,
                EnableAutoPayment = false
            };

            //For DebitCreditCardDetail

            if (paymentMethod.SenderPaymentMode == SenderPaymentMode.CreditDebitCard)
            {
                var debitCreditCardDetail = _familyAndFriendsTransferServices.GetDebitCreditCardDetail();

                transactionSummaryVm.CreditORDebitCardDetials = debitCreditCardDetail;
            }

            if (paymentMethod.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
            {

                var moneyFexBankAccountDepositData = _familyAndFriendsTransferServices.GetMoneyFexBankAccountDeposit();
                transactionSummaryVm.MoneyFexBankDeposit = moneyFexBankAccountDepositData;

            }

            transactionSummaryVm.TransferType = TransferType.CashPickup;
            transactionSummaryVm.IsLocalPayment = false;

            _senderForAllTransferServices.CompleteTransaction(transactionSummaryVm);

            senderCommonFunc.ClearFamilyAndFriendSession();

            return View(vm);
        }


        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount)
        {
            bool IsReceivingAmount = false;
            var enterAmountData = _familyAndFriendsTransferServices.GetSenderTransferFamilyAndFriendsEnrterAmount();
            var loggedInSenderData = _familyAndFriendsTransferServices.GetLoggedUserData();
            if ((SendingAmount > 0 && ReceivingAmount > 0) && enterAmountData.ReceivingAmount != ReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
                IsReceivingAmount = true;
            }

            if (SendingAmount == 0)
            {

                IsReceivingAmount = true;
                SendingAmount = ReceivingAmount;
            }
            var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                enterAmountData.ExchangeRate, SEstimateFee.GetFaxingCommision(loggedInSenderData.CountryCode));

            // Rewrite session with additional value 
            enterAmountData.Fee = result.FaxingFee;
            enterAmountData.SendingAmount = result.FaxingAmount;
            enterAmountData.ReceivingAmount = result.ReceivingAmount;
            enterAmountData.TotalAmount = result.TotalAmount;

            _familyAndFriendsTransferServices.SetSenderTransferFamilyAndFriendsEnrterAmount(enterAmountData);
            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount = result.FaxingAmount
            }, JsonRequestBehavior.AllowGet);
        }

    }
}