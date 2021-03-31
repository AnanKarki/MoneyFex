
using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.SecureTradingPaymentGateway;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Twilio.TwiML.Voice;

namespace FAXER.PORTAL.Controllers.Transfer
{
    public class SenderBankAccountDepositController : Controller
    {
        SSenderBankAccountDeposit _senderBankAccountDepositServices = null;

        SSenderKiiPayWalletTransfer _kiiPaytrasferServices = null;
        public SenderBankAccountDepositController()
        {
            _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
        }
        // GET: SenderBankAccountDepositDebit

        #region international bank account deposit


        public JsonResult GetBankCode(int BankId)
        {
            var bankCode = _senderBankAccountDepositServices.GetBankCode(BankId);
            var branches = _senderBankAccountDepositServices.GetBranches(BankId);
            return Json(new
            {
                BranchCode = bankCode.Code,
                Branches = branches
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCountryPhonCode(string CountryCode)
        {

            var CountryPhoneCode = Common.Common.GetCountryPhoneCode(CountryCode);
            return Json(new
            {
                CountryCode = CountryPhoneCode
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Index(string Country = "", string RecentAcccountNo = "")
        {
            SenderBankAccountDepositVm vm = new SenderBankAccountDepositVm();
            var amountSummary = _kiiPaytrasferServices.GetCommonEnterAmount(); // GetSummary set in Session 

            var country = Common.Common.GetCountries();

            var recentAccountNumbers = new List<Common.DropDownViewModel>();

            vm = _senderBankAccountDepositServices.GetSenderBankAccountDeposit();

            string RepeatedTransactionCountry = Country;
            if (string.IsNullOrEmpty(Country))
            {
                Country = vm.CountryCode;
            }
            if (Common.FaxerSession.IsTransferFromHomePage || Common.FaxerSession.IsCommonEstimationPage)
            {
                Country = amountSummary.ReceivingCountryCode;
                ViewBag.ReceivingCountryCurrency = amountSummary.ReceivingCurrency;
                ViewBag.TransferMethod = "Bank Deposit";
                ViewBag.SendingCountryCurrency = amountSummary.SendingCurrency;
                ViewBag.SendingAmount = amountSummary.SendingAmount;
                ViewBag.ReceivingCountry = amountSummary.ReceivingCountryCode.ToLower();
            }
            var senderStatus = Common.Common.SenderStatus(Common.FaxerSession.LoggedUser.Id);

            if (string.IsNullOrEmpty(Country))
            {
                Country = RepeatedTransactionCountry;
            }
            if (!string.IsNullOrEmpty(Country))
            {
                recentAccountNumbers = _senderBankAccountDepositServices.GetRecentAccountNumbers(amountSummary.ReceivingCurrency
                    ,amountSummary.ReceivingCountryCode)
                                        .Where(x => x.CountryCode == Country).ToList();
            }
            ViewBag.RecentAccountNumbers = new SelectList(recentAccountNumbers, "Code", "Name", RecentAcccountNo);
            vm.CountryCode = Country;
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName", Country);
            vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Country);
            SetViewBagForBanks(Country, _kiiPaytrasferServices.GetCommonEnterAmount().ReceivingCurrency);
            getRecentAccountno(ref vm, RecentAcccountNo, Country);
            ViewBag.Branches = new SelectList(_senderBankAccountDepositServices.GetBranches(vm.BankId), "Code", "Name", vm.BranchCode);
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Country);
            if (Common.FaxerSession.IsTransferFromHomePage
               && amountSummary.SendingCountryCode != Common.FaxerSession.LoggedUser.CountryCode)
            {
                return RedirectToAction("Index", "SenderTransferMoneyNow");
            }
            var isServiceAvailable = Common.Common.
                              GetTransferServices(Common.FaxerSession.LoggedUser.CountryCode, Country
                              ).Where(x => x.ServiceType == DB.TransferService.BankDeposit).FirstOrDefault();
            if (isServiceAvailable == null)
            {
                ModelState.AddModelError("", "Bank Deposit Service is not available");
                return View(vm);
            }

            //vm.IsEuropeTransfer = IsEuropeTransfer(Country);
            //vm.IsSouthAfricaTransfer = _senderBankAccountDepositServices.IsSouthAfricanTransfer(Country);
            //vm.IsEuropeTransfer = _senderBankAccountDepositServices.IsWestAfricanTransfer(Country);

            return View(vm);
        }


        public void getRecentAccountno(ref SenderBankAccountDepositVm vm, string accountNo, string Country = "")
        {

            var accountData = _senderBankAccountDepositServices.
                GetAccountInformationFromAccountNumber(accountNo, Country);
            if (accountData != null)
            {

                if (Country.ToLower() != accountData.CountryCode.ToLower())
                {
                    vm.AccountOwnerName = "";
                    vm.MobileNumber = "";
                    vm.AccountNumber = "";
                    vm.BankId = 0;
                    vm.BranchCode = "";
                }
                else
                {
                    vm.AccountOwnerName = accountData.AccountOwnerName;
                    vm.MobileNumber = accountData.MobileNumber;
                    vm.AccountNumber = accountData.AccountNumber;
                    vm.BankId = accountData.BankId;
                    vm.BranchCode = accountData.BranchCode;
                    vm.ReceiverCity = accountData.ReceiverCity;
                    vm.ReceiverEmail = accountData.ReceiverEmail;
                    vm.ReceiverPostalCode = accountData.ReceiverPostalCode;
                    vm.ReceiverStreet = accountData.ReceiverStreet;
                }

            }

            vm.IsEuropeTransfer = IsEuropeTransfer(Country);
            vm.IsSouthAfricaTransfer = _senderBankAccountDepositServices.IsSouthAfricanTransfer(Country);
            vm.IsWestAfricaTransfer = _senderBankAccountDepositServices.IsWestAfricanTransfer(Country);

        }

        private bool IsEuropeTransfer(string Country)
        {

            var result = Common.Common.IsEuropeTransfer(Country);
            return result;
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = SenderBankAccountDepositVm.BindProperty)] SenderBankAccountDepositVm model)
        {

            var isServiceAvailable = Common.Common.
                              GetTransferServices(Common.FaxerSession.LoggedUser.CountryCode, model.CountryCode
                              ).Where(x => x.ServiceType == DB.TransferService.BankDeposit).FirstOrDefault();
            if (isServiceAvailable == null)
            {
                ModelState.AddModelError("", "Bank Deposit Service is not available");
                return View(model);
            }
            model.IsEuropeTransfer = IsEuropeTransfer(model.CountryCode);
            model.IsSouthAfricaTransfer = _senderBankAccountDepositServices.IsSouthAfricanTransfer(model.CountryCode);
            model.IsWestAfricaTransfer = _senderBankAccountDepositServices.IsWestAfricanTransfer(model.CountryCode);

            var country = Common.Common.GetCountries();

            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();


            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(model.CountryCode);
            var recentAccountNumbers = _senderBankAccountDepositServices.
                GetRecentAccountNumbers(_kiiPaytrasferServices.GetCommonEnterAmount().ReceivingCurrency ,
                model.CountryCode).Where(x => x.CountryCode 
                != model.CountryCode).ToList();
            ViewBag.RecentAccountNumbers = new SelectList(recentAccountNumbers, "Code", "Name");
            SetViewBagForBanks(model.CountryCode);
            ViewBag.Branches = new SelectList(_senderBankAccountDepositServices.GetBranches(model.BankId), "Code", "Name", model.BranchCode);
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName", model.CountryCode);
            ViewBag.ReceivingCountryCurrency = _kiiPaytrasferServices.GetCommonEnterAmount().ReceivingCurrency;
            ViewBag.TransferMethod = "Bank Deposit";
            ViewBag.SendingCountryCurrency = _kiiPaytrasferServices.GetCommonEnterAmount().SendingCurrency;
            ViewBag.SendingAmount = _kiiPaytrasferServices.GetCommonEnterAmount().SendingAmount;
            ViewBag.ReceivingCountry = _kiiPaytrasferServices.GetCommonEnterAmount().ReceivingCountryCode.ToLower();

            if (ModelState.IsValid)

            {
                if (model.ReasonForTransfer == ReasonForTransfer.Non)
                {
                    ModelState.AddModelError("ReasonForTransfer", "Select Reason for Transfer");
                    return View(model);
                }
                if (!string.IsNullOrEmpty(model.AccountOwnerName))
                {
                    var AccountOwnerFullname = model.AccountOwnerName.Split(' ');
                    if (AccountOwnerFullname.Count() < 2)
                    {
                        ModelState.AddModelError("AccountOwnerName", "Enter recipient full name");
                        return View(model);
                    }
                }

                if (!string.IsNullOrEmpty(model.AccountOwnerName))
                {
                    var AccountOwnerFullname = model.AccountOwnerName.Split(' ');
                    if (AccountOwnerFullname.Count() < 1)
                    {
                        ModelState.AddModelError("AccountOwnerName", "Enter recipient full name");
                        return View(model);
                    }

                }
                if (!IsEuropeTransfer(model.CountryCode)) //have to select bank if it is not europe transfer  
                {

                    if (model.BankId == 0)
                    {

                        ModelState.AddModelError("BankId", "Select Bank");
                        return View(model);
                    }
                }
                else
                { // Valid Bank Name if user is performing bank transfer to europe

                    if (string.IsNullOrEmpty(model.BankName))
                    {

                        ModelState.AddModelError("BankName", "Enter Bank Name");
                        return View(model);
                    }
                }
                if (model.IsWestAfricaTransfer)
                {
                    model.BranchCode = Common.Common.getBank(model.BankId).Code;
                }
                if (model.CountryCode == "NG")
                {
                    model.BranchCode = Common.Common.getBank(model.BankId).Code;
                }

                if (model.IsSouthAfricaTransfer)
                {
                    // validate
                    if (string.IsNullOrEmpty(model.ReceiverStreet))
                    {
                        ModelState.AddModelError("ReceiverStreet", "Enter Address");
                        return View(model);
                    }
                    if (string.IsNullOrEmpty(model.ReceiverPostalCode))
                    {
                        ModelState.AddModelError("ReceiverPostalCode", "Enter Postcode");
                        return View(model);
                    }
                    if (string.IsNullOrEmpty(model.ReceiverCity))
                    {
                        ModelState.AddModelError("ReceiverCity", "Enter City");
                        return View(model);
                    }
                    if (string.IsNullOrEmpty(model.ReceiverEmail))
                    {
                        ModelState.AddModelError("ReceiverEmail", "Enter Email");
                        return View(model);
                    }

                }



                bool IsValidBankDepositReceiver = Common.Common.IsValidBankDepositReceiver(model.AccountNumber, Service.BankAccount);
                if (IsValidBankDepositReceiver == false)
                {

                    ModelState.AddModelError("", "Account no. not accepted");
                    return View(model);
                }
                //var HasExceededBankDepositReceiverLimit = Common.Common.HasExceededBankDepositReceiverLimit(
                //    model.CountryCode, model.AccountNumber, model.BankId);
                //if (HasExceededBankDepositReceiverLimit)
                //{

                //    ModelState.AddModelError("", "Recipient daily transaction limit exceeded");
                //    return View(model);
                //}
                //var HasExceededTransactionLimit = Common.Common.
                //    HasExceededBankDepositLimit(Common.FaxerSession.LoggedUser.Id);
                //if (HasExceededTransactionLimit)
                //{

                //    ModelState.AddModelError("", "MoneyFex account daily transaction limit exceeded");
                //    return View(model);
                //}


                model.ReceipientId = _senderBankAccountDepositServices.GetRecipientId(model.BankId, model.AccountNumber);
                bool HasRecipientTransacionExceedLimit = Common.Common.HasExceededReceiverLimit(Common.FaxerSession.SenderId,
                    model.ReceipientId, Common.FaxerSession.LoggedUser.CountryCode, model.CountryCode, TransactionTransferMethod.BankDeposit);
                if (HasRecipientTransacionExceedLimit)
                {
                    ModelState.AddModelError("", "Transaction for Recipient limit exceeded");
                    return View(model);
                }
                bool HasSenderTransacionExceedLimit = Common.Common.HasExceededSenderTransactionLimit(Common.FaxerSession.SenderId, Common.FaxerSession.LoggedUser.CountryCode, model.CountryCode, TransactionTransferMethod.BankDeposit);
                if (HasSenderTransacionExceedLimit)
                {
                    ModelState.AddModelError("", "Sender daily transaction limit exceeded");
                    return View(model);
                }
                //bool HasTransacionAmountExceedLimit = Common.Common.HasExceededAmountLimit(Common.FaxerSession.SenderId, Common.FaxerSession.LoggedUser.CountryCode,
                //    model.CountryCode, _kiiPaytrasferServices.GetCommonEnterAmount().SendingAmount, Module.Faxer);
                //if (HasTransacionAmountExceedLimit)
                //{
                //    ModelState.AddModelError("", "MoneyFex account daily transaction limit exceeded");
                //    return View(model);
                //}

                bool IsManualDeposit = Common.Common.IsManualDeposit(Common.FaxerSession.LoggedUser.CountryCode, model.CountryCode);
                bool IsValidateAccountNo = true;



                if (!IsManualDeposit)
                {

                    decimal SendingAmount = _kiiPaytrasferServices.GetCommonEnterAmount().SendingAmount;

                    var Apiservice = Common.Common.GetApiservice(Common.FaxerSession.LoggedUser.CountryCode, model.CountryCode, SendingAmount,
                        TransactionTransferMethod.BankDeposit, TransactionTransferType.Online);

                    if (Apiservice == null)
                    {
                        ModelState.AddModelError("ServiceNotAvialable", "Service Not Available");
                        return View(model);
                    }
                    else
                    {
                        if (Apiservice == DB.Apiservice.FlutterWave)
                        {
                            if (string.IsNullOrEmpty(model.ReceiverEmail))
                            {
                                ModelState.AddModelError("ReceiverEmail", "Enter Email");
                                return View(model);
                            }
                        }
                    }

                    var IsValidAccountNo = _senderBankAccountDepositServices.IsValidBankAccount(model, SendingAmount, FaxerSession.LoggedUser.CountryCode);

                    if (IsValidAccountNo.Data == false)
                    {

                        ModelState.AddModelError("", IsValidAccountNo.Message);
                        return View(model);
                    }
                }

                bool IsValidDigitCount = Common.Common.IsValidDigitCount(model.CountryCode, model.AccountNumber);
                if (!IsValidDigitCount)
                {
                    ModelState.AddModelError("AccountNumber", "Enter 10 digit account number");
                    return View(model);
                }
                else if (IsValidateAccountNo == true)
                {
                    model.IsManualDeposit = IsManualDeposit;
                    model.SendingCurrency = _kiiPaytrasferServices.GetCommonEnterAmount().SendingCurrency;
                    model.ReceivingCurrency = _kiiPaytrasferServices.GetCommonEnterAmount().ReceivingCurrency;

                    _senderBankAccountDepositServices.SetSenderBankAccountDeposit(model);


                    //for enter amount Page

                    var loggedInSenderData = _senderBankAccountDepositServices.GetLoggedUserData();

                    var receiverInformation = _senderBankAccountDepositServices.GetReceiverInformationFromAccountNumnber(model.AccountNumber);



                    // Update Payment Summary Session 
                    _kiiPaytrasferServices.setPaymentSummary(TransactionTransferMethod.BankDeposit);

                    var estimatedSummary = _kiiPaytrasferServices.GetCommonEnterAmount();

                    //Fetch receiver data later currenctly sender data is used
                    SenderBankAccoutDepositEnterAmountVm bankDepositEnterAmount = new SenderBankAccoutDepositEnterAmountVm()
                    {
                        ReceiverName = model.AccountOwnerName,
                        ReceiverId = 0,
                        //SendingCurrencySymbol =  Common.Common.GetCurrencySymbol(loggedInSenderData.CountryCode),
                        //SendingCurrencyCode = _kiiPaytrasferServices.GetCommonEnterAmount().SendingCurrency,
                        //ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(model.CountryCode),
                        //ReceivingCurrencyCode = _kiiPaytrasferServices.GetCommonEnterAmount().ReceivingCurrency,
                        //ExchangeRate = Common.Common.GetExchangeRate(loggedInSenderData.CountryCode, model.CountryCode),
                        //SendingCountryCode = loggedInSenderData.CountryCode,
                        //ReceivingCountryCode = model.CountryCode
                        SendingCurrencyCode = estimatedSummary.SendingCurrency,
                        SendingCurrencySymbol = estimatedSummary.SendingCurrencySymbol,
                        ReceivingCurrencySymbol = estimatedSummary.ReceivingCurrencySymbol,
                        ReceivingCurrencyCode = estimatedSummary.ReceivingCurrency,
                        ExchangeRate = estimatedSummary.ExchangeRate,
                        SendingCountryCode = estimatedSummary.SendingCountryCode,
                        ReceivingCountryCode = estimatedSummary.ReceivingCountryCode
                    };

                    //SSenderForAllTransfer.SetPaymentSummarySession(model.CountryCode);

                    _senderBankAccountDepositServices.SetSenderBankAccoutDepositEnterAmount(bankDepositEnterAmount);



                    return RedirectToAction("BankDepositAbroadSummary", "SenderBankAccountDeposit");
                }
                else
                {
                    ModelState.AddModelError("", "Enter a validate account number");
                    return View(model);
                }
            }
            else
            {
                //ModelState.AddModelError("", "");
                return View(model);
            }
        }

        private void SetViewBagForBanks(string Country)
        {
            var banks = _senderBankAccountDepositServices.getBanksList(Country);
            ViewBag.BankNames = new SelectList(banks, "Id", "Name");
        }
        private void SetViewBagForBanks(string Country, string currency)
        {
            var banks = _senderBankAccountDepositServices.getBanksByCurrency(Country, currency);
            ViewBag.BankNames = new SelectList(banks, "Id", "Name");
        }

        public ActionResult BankDepositAbroadEnterAmount()
        {
            var loggedInSenderData = _senderBankAccountDepositServices.GetLoggedUserData();
            var bankAccountDepositData = _senderBankAccountDepositServices.GetSenderBankAccountDeposit();
            var enterAmountData = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
            SenderBankAccoutDepositEnterAmountVm vm = new SenderBankAccoutDepositEnterAmountVm();
            enterAmountData.ReceiverName = bankAccountDepositData.AccountOwnerName;
            _senderBankAccountDepositServices.SetSenderBankAccoutDepositEnterAmount(enterAmountData);
            if (Common.FaxerSession.IsTransferFromHomePage == true)
            {

                return RedirectToAction("BankDepositAbroadSummary");
            }
            vm = enterAmountData;
            return View(vm);
        }

        [HttpPost]
        public ActionResult BankDepositAbroadEnterAmount([Bind(Include = SenderBankAccoutDepositEnterAmountVm.BindProperty)] SenderBankAccoutDepositEnterAmountVm vm)
        {
            if (ModelState.IsValid)
            {

                if (Common.FaxerSession.LoggedUser.CountryCode == "GB")
                {

                    if (vm.SendingAmount < 25 || vm.SendingAmount > 50000)
                    {

                        ModelState.AddModelError("SendingAmount", "Enter amount higher than 25 and less than 50,000");
                        return View(vm);
                    }
                }
                Common.FaxerSession.SenderBankAccoutDepositEnterAmount = vm;
                if (Common.FaxerSession.IsTransferFromHomePage == false)
                {

                    return RedirectToAction("InternationalPayNow");
                }
                return RedirectToAction("BankDepositAbroadSummary", "SenderBankAccountDeposit");
            }
            return View(vm);

        }

        public ActionResult BankDepositAbroadSummary()
        {
            //Fetching data for summary

            if (Common.FaxerSession.TransactionId != 0)
            {
                _senderBankAccountDepositServices.setAmount(Common.FaxerSession.TransactionId);

            }
            if (Common.FaxerSession.RecipientId != 0)
            {
                _senderBankAccountDepositServices.setReciverInfo(Common.FaxerSession.RecipientId);

            }

            var sendingAmountData = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
            ViewBag.ReceivingCountryCurrency = sendingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Bank Deposit";
            ViewBag.SendingCountryCurrency = sendingAmountData.SendingCurrencyCode;
            ViewBag.SendingAmount = sendingAmountData.SendingAmount;
            ViewBag.ReceivingCountry = sendingAmountData.ReceivingCountryCode == null ? "" : sendingAmountData.ReceivingCountryCode.ToLower();

            string fullName = sendingAmountData.ReceiverName;

            ViewBag.ReceiverName = fullName;

            var names = fullName.Split(' ');
            string firstName = names[0];
            SenderTransferSummaryVm vm = new SenderTransferSummaryVm()
            {
                Amount = sendingAmountData.SendingAmount,
                Fee = sendingAmountData.Fee,
                ReceivedAmount = sendingAmountData.ReceivingAmount,
                SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
                SendingCurrencyCode = sendingAmountData.SendingCurrencyCode,
                PaidAmount = sendingAmountData.TotalAmount,
                ReceivingCurrencySymbol = sendingAmountData.ReceivingCurrencySymbol,
                ReceivingCurrencyCode = sendingAmountData.ReceivingCurrencyCode,
                ReceiverName = firstName
            };
            _senderBankAccountDepositServices.SetTransactionSummary();

            return View(vm);
        }

        [HttpPost]
        public ActionResult BankDepositAbroadSummary([Bind(Include = SenderTransferSummaryVm.BindProperty)] SenderTransferSummaryVm model)
        {

            bool Isvalid = true;

            var paymentSummary = _kiiPaytrasferServices.GetCommonEnterAmount();

            var bankDepositModel = _senderBankAccountDepositServices.SaveIncompleteTransaction();
            Common.FaxerSession.TransactionId = bankDepositModel.Id;

            Common.FaxerSession.IsTransactionOnpending = true;

            TransactionPendingViewModel transactionPending = new TransactionPendingViewModel()
            {
                TransactionId = bankDepositModel.Id,
                IsTransactionPending = true,
                TransferMethod = TransactionServiceType.BankDeposit,
                Fee = bankDepositModel.Fee,
                ExchangeRate = bankDepositModel.ExchangeRate,
                ReceiverFullName = bankDepositModel.ReceiverName,
                ReceiptNumber = bankDepositModel.ReceiptNo,
                ReceivingCountry = Common.Common.GetCountryName(bankDepositModel.ReceivingCountry),
                Receivingurrency = paymentSummary.ReceivingCurrency,  //Common.Common.GetCountryCurrency(bankDepositModel.ReceivingCountry),
                SenderId = bankDepositModel.SenderId,
                SendingAmount = bankDepositModel.SendingAmount,
                SendingCurrency = paymentSummary.SendingCurrency,//Common.Common.GetCountryCurrency(bankDepositModel.SendingCountry),
                TransactionNumber = bankDepositModel.ReceiptNo,
                BankAccount = bankDepositModel.ReceiverAccountNo,
                BankCode = bankDepositModel.BankCode,
                BankName = _senderBankAccountDepositServices.GetBankName(bankDepositModel.BankId),


            };
            _senderBankAccountDepositServices.SetTransactionPendingViewModel(transactionPending);

            if (Common.FaxerSession.TransactionId == 0 && Common.FaxerSession.RecipientId == 0)
            {
                //    SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
                //   Common.FaxerSession.ReceiptNo = _senderForAllTransferServices.GenerateReceiptNoForBankDepoist(_senderForAllTransferServices.GetTransactionSummary().BankAccountDeposit.IsManualDeposit);

            }
            else
            {
                //Isvalid = _senderBankAccountDepositServices.RepeatTransaction(Common.FaxerSession.TransactionId, Common.FaxerSession.RecipientId);

            }
            if (!Isvalid)
            {
                string ErrorMessage = Common.FaxerSession.ErrorMessage;
                ModelState.AddModelError("", ErrorMessage);

                var sendingAmountData = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
                ViewBag.ReceivingCountryCurrency = sendingAmountData.ReceivingCurrencyCode;
                ViewBag.TransferMethod = "Bank Deposit";
                ViewBag.SendingCountryCurrency = sendingAmountData.SendingCurrencyCode;
                ViewBag.SendingAmount = Math.Round(sendingAmountData.SendingAmount, 2);
                ViewBag.ReceivingCountry = sendingAmountData.ReceivingCountryCode.ToLower();

                string fullName = sendingAmountData.ReceiverName;

                ViewBag.ReceiverName = fullName;

                return View(model);
            }
            return RedirectToAction("InternationalPayNow", "SenderBankAccountDeposit");
        }

        public ActionResult InternationalPayNow()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var vm = _senderBankAccountDepositServices.GetPaymentMethod();
            var sendsingAmountData = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
            ViewBag.ReceivingCountryCurrency = sendsingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Bank Deposit";
            ViewBag.SendingCountryCurrency = sendsingAmountData.SendingCurrencyCode;
            ViewBag.SendingAmount = sendsingAmountData.SendingAmount;
            ViewBag.ReceiverName = sendsingAmountData.ReceiverName;
            ViewBag.ReceivingCountry = sendsingAmountData.ReceivingCountryCode == null ? "" : sendsingAmountData.ReceivingCountryCode.ToLower();
            ViewBag.Fee = sendsingAmountData.Fee;
            //Credit/Debit card 
            //Fee: GBP 0.05
            //Manual Bank Deposit
            //Fee: GBP 0.79
            ViewBag.CreditDebitFee = new CreditDebitCardViewModel().CreditDebitCardFee;
            ViewBag.ManualBankDepositFee = new SenderMoneyFexBankDepositVM().BankFee;

            vm.TotalAmount = sendsingAmountData.TotalAmount;
            vm.SendingCurrencySymbol = sendsingAmountData.SendingCurrencySymbol;

            var senderwalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
            if (senderwalletInfo != null)
            {
                vm.KiipayWalletBalance = senderwalletInfo.CurrentBalance;
                vm.HasKiiPayWallet = true;
            }
            else
            {
                vm.HasKiiPayWallet = false;
            }

            vm.HasEnableMoneyFexBankAccount = senderCommonFunc.IsEnabledMoneyFexbankAccount(Common.FaxerSession.LoggedUser.CountryCode);
            vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails();

            return View(vm);
        }

        [HttpPost]
        public ActionResult InternationalPayNow([Bind(Include = PaymentMethodViewModel.BindProperty)] PaymentMethodViewModel vm)
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var sendsingAmountData = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
            ViewBag.ReceivingCountryCurrency = sendsingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Bank Deposit";
            ViewBag.SendingCountryCurrency = sendsingAmountData.SendingCurrencyCode;
            ViewBag.SendingAmount = sendsingAmountData.SendingAmount;
            ViewBag.ReceiverName = sendsingAmountData.ReceiverName;
            ViewBag.ReceivingCountry = sendsingAmountData.ReceivingCountryCode.ToLower();
            ViewBag.Fee = sendsingAmountData.Fee;

            vm.HasEnableMoneyFexBankAccount = senderCommonFunc.IsEnabledMoneyFexbankAccount(Common.FaxerSession.LoggedUser.CountryCode);
            //Credit/Debit card 
            //Fee: GBP 0.05
            //Manual Bank Deposit
            //Fee: GBP 0.79
            ViewBag.CreditDebitFee = new CreditDebitCardViewModel().CreditDebitCardFee;
            ViewBag.ManualBankDepositFee = new SenderMoneyFexBankDepositVM().BankFee;



            int selectedCardId = 0;
            string cardNumber = null;
            string CreditCardSecurityCode = "";
            if (vm.CardDetails != null)
            {
                foreach (var item in vm.CardDetails)
                {
                    if (item.IsChecked == true)
                    {
                        selectedCardId = item.CardId;
                        cardNumber = item.CardNumber;
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                        CreditCardSecurityCode = item.SecurityCode;
                    }
                }
            }

            if (vm.CardDetails == null)
            {

                vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails();
            }
            _senderBankAccountDepositServices.SetPaymentMethod(vm);



            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();


            // Bank Account Deposit Api Request 

            var bankAccountDeposit = _senderBankAccountDepositServices.GetSenderBankAccountDeposit();
            var HasExceededBankDepositReceiverLimit = Common.Common.HasExceededReceiverLimit(Common.FaxerSession.SenderId,
                    bankAccountDeposit.ReceipientId, Common.FaxerSession.LoggedUser.CountryCode, bankAccountDeposit.CountryCode, TransactionTransferMethod.BankDeposit);
            if (HasExceededBankDepositReceiverLimit)
            {
                ModelState.AddModelError("TransactionError", "Recipient daily transaction limit exceeded");
                return View(vm);
            }
            bool HasSenderTransacionExceedLimit = Common.Common.HasExceededSenderTransactionLimit(Common.FaxerSession.SenderId, Common.FaxerSession.LoggedUser.CountryCode, bankAccountDeposit.CountryCode, TransactionTransferMethod.BankDeposit);
            if (HasSenderTransacionExceedLimit)
            {
                ModelState.AddModelError("TransactionError", "Sender daily transaction limit exceeded");
                return View(vm);
            }
            //var HasExceededTransactionLimit = Common.Common.
            //    HasExceededAmountLimit(Common.FaxerSession.LoggedUser.Id, Common.FaxerSession.LoggedUser.CountryCode, bankAccountDeposit.CountryCode, sendsingAmountData.SendingAmount, Module.Faxer);
            //if (HasExceededTransactionLimit)
            //{
            //    ModelState.AddModelError("TransactionError", "MoneyFex account daily transaction limit exceeded");
            //    return View(vm);
            //}
            _senderBankAccountDepositServices.SetTransactionSummary();
            var transactionSummaryvm = _senderForAllTransferServices.GetTransactionSummary();

            switch (vm.SenderPaymentMode)
            {
                case SenderPaymentMode.SavedDebitCreditCard:

                    ViewBag.IsSavedCreditDebitCard = true;
                    SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
                    var cardInfo = _saveCreditDebitCard.GetCardInfo(selectedCardId);
                    transactionSummaryvm.CreditORDebitCardDetials = new CreditDebitCardViewModel()
                    {
                        CardNumber = cardInfo.Num.Decrypt(),
                        NameOnCard = cardInfo.CardName.Decrypt(),
                        EndMM = cardInfo.EMonth.Decrypt(),
                        EndYY = cardInfo.EYear.Decrypt(),
                        SecurityCode = CreditCardSecurityCode
                        //SecurityCode = cardInfo.ClientCode.Decrypt()
                    };


                    var validateCardresult = Common.Common.ValidateCreditDebitCard(transactionSummaryvm.CreditORDebitCardDetials);
                    if (validateCardresult.Data == false)
                    {
                        ModelState.AddModelError("TransactionError", validateCardresult.Message);
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                        return View(vm);
                    }
                    StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
                    {
                        CardName = Common.FaxerSession.LoggedUser.FullName,
                        ExpirationMonth = transactionSummaryvm.CreditORDebitCardDetials.EndMM,
                        ExpiringYear = transactionSummaryvm.CreditORDebitCardDetials.EndYY,
                        Number = transactionSummaryvm.CreditORDebitCardDetials.CardNumber,
                        SecurityCode = transactionSummaryvm.CreditORDebitCardDetials.SecurityCode,
                        billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                        billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                        CurrencyCode = Common.Common.GetCurrencyCode(Common.FaxerSession.LoggedUser.CountryCode),
                        Amount = transactionSummaryvm.CreditORDebitCardDetials.FaxingAmount,
                        SenderId = Common.FaxerSession.LoggedUser.Id

                    };

                    var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm, Common.FaxerSession.LoggedUser.CountryCode, bankAccountDeposit.CountryCode);
                    if (StripeResult.IsValid == false)
                    {
                        ModelState.AddModelError("TransactionError", StripeResult.Message);
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                        return View(vm);
                    }
                    _senderBankAccountDepositServices.SetDebitCreditCardDetail(transactionSummaryvm.CreditORDebitCardDetials);
                    //transactionSummaryvm.CreditORDebitCardDetials.StripeTokenID = result.StripeTokenId;

                    return RedirectToAction("DebitCreditCardDetails", new { IsFromSavedDebitCard = true });
                case SenderPaymentMode.CreditDebitCard:
                    return RedirectToAction("DebitCreditCardDetails");
                case SenderPaymentMode.KiiPayWallet:
                    var hasEnoughBal = senderCommonFunc.SenderHasEnoughWalletBaltoTransfer(transactionSummaryvm.KiiPayTransferPaymentSummary.TotalAmount, transactionSummaryvm.SenderAndReceiverDetail.SenderWalletId);
                    if (hasEnoughBal == false)
                    {
                        ModelState.AddModelError("TransactionError", "Your wallet doesn't have enough balance!");
                        vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails();
                        return View(vm);
                    }
                    _senderForAllTransferServices.CompleteTransaction(transactionSummaryvm);
                    break;
                case SenderPaymentMode.MoneyFexBankAccount:
                    return RedirectToAction("MoneyFexBankDeposit");
                default:
                    break;
            }
            return RedirectToAction("AddMoneyToWalletSuccess");

        }


        public ActionResult DebitCreditCardDetails(bool IsAddDebitCreditCard = false, bool IsFromSavedDebitCard = false)
        {
            var sendingAmountData = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();

            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            var Vm = _senderBankAccountDepositServices.GetDebitCreditCardDetail();

            // var paymentInfo = _senderForAllTransferServices.GetTransactionSummary().KiiPayTransferPaymentSummary;
            var paymentInfo = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
            //  Fee Added statically (Bank Fee) it will be added dynamically after payment setup 
            //Credit/Debit card 
            //Fee: GBP 0.80
            //Manual Bank Deposit
            //Fee: GBP 0.79
            decimal TotalAmount = paymentInfo.TotalAmount + Vm.CreditDebitCardFee;
            Vm.FaxingAmount = paymentInfo.TotalAmount + Vm.CreditDebitCardFee;
            Vm.FaxingCurrencySymbol = paymentInfo.SendingCurrencySymbol;
            Vm.FaxingCurrency = paymentInfo.SendingCurrencyCode;
            Vm.SaveCard = IsAddDebitCreditCard;
            Vm.ReceiverName = sendingAmountData.ReceiverName;
            ViewBag.HasOneSavedCard = Common.Common.HasOneCardSaved();
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            Vm.AddressLineOne = senderCommonFunc.GetSenderAddress();
            ViewBag.IsFromSavedDebitCard = IsFromSavedDebitCard;

            ViewBag.ReceivingCountryCurrency = sendingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Bank Deposit";
            ViewBag.SendingCountryCurrency = sendingAmountData.SendingCurrencyCode;
            ViewBag.SendingAmount = sendingAmountData.SendingAmount;
            ViewBag.ReceiverName = sendingAmountData.ReceiverName;
            ViewBag.ReceivingCountry = sendingAmountData.ReceivingCountryCode.ToLower();
            ViewBag.Fee = sendingAmountData.Fee;

            return View(Vm);

        }


        [HttpPost]
        public JsonResult ThreeDQuery([Bind(Include = CreditDebitCardViewModel.BindProperty)] CreditDebitCardViewModel vm)
        {

            var serviceResult = new ServiceResult<ThreeDRequestVm>();
            var bankAccountDeposit = _senderBankAccountDepositServices.GetSenderBankAccountDeposit();
            var HasExceededBankDepositReceiverLimit = Common.Common.HasExceededReceiverLimit(Common.FaxerSession.SenderId,
                    bankAccountDeposit.ReceipientId, Common.FaxerSession.LoggedUser.CountryCode, bankAccountDeposit.CountryCode, TransactionTransferMethod.BankDeposit);
            if (HasExceededBankDepositReceiverLimit)
            {
                serviceResult.Data = null;
                serviceResult.Status = ResultStatus.Error;
                serviceResult.Message = "Recipient daily transaction limit exceeded";
                //ModelState.AddModelError("TransactionError", "Recipient daily transaction limit exceeded");
                return Json(serviceResult, JsonRequestBehavior.AllowGet);

            }
            bool HasSenderTransacionExceedLimit = Common.Common.HasExceededSenderTransactionLimit(Common.FaxerSession.SenderId, Common.FaxerSession.LoggedUser.CountryCode, bankAccountDeposit.CountryCode, TransactionTransferMethod.BankDeposit);
            if (HasSenderTransacionExceedLimit)
            {
                serviceResult.Data = null;
                serviceResult.Status = ResultStatus.Error;
                serviceResult.Message = "Sender daily transaction limit exceeded";
                //ModelState.AddModelError("TransactionError", "MoneyFex account daily transaction limit exceeded");
                return Json(serviceResult, JsonRequestBehavior.AllowGet);
            }
            //var HasExceededTransactionLimit = Common.Common.
            //        HasExceededAmountLimit(Common.FaxerSession.LoggedUser.Id, Common.FaxerSession.LoggedUser.CountryCode, bankAccountDeposit.CountryCode,
            //        vm.FaxingAmount, Module.Faxer);

            //if (HasExceededTransactionLimit)
            //{
            //    serviceResult.Data = null;
            //    serviceResult.Status = ResultStatus.Error;
            //    serviceResult.Message = "MoneyFex account daily transaction limit exceeded";
            //    //ModelState.AddModelError("TransactionError", "MoneyFex account daily transaction limit exceeded");
            //    return Json(serviceResult, JsonRequestBehavior.AllowGet);
            //}

            if (string.IsNullOrEmpty(vm.CardNumber))
            {

                ModelState.AddModelError("", "Enter Card Number");
                serviceResult.Data = null;
                serviceResult.Message = "Enter Card Number";
                serviceResult.Status = ResultStatus.Error;
                return Json(serviceResult);
            }
            var number = vm.CardNumber.Split(' ');
            vm.CardNumber = string.Join("", number);
            var validateCardresult = Common.Common.ValidateCreditDebitCard(vm);
            if (validateCardresult.Data == false)
            {
                serviceResult.Data = null;
                serviceResult.Message = validateCardresult.Message;
                serviceResult.Status = ResultStatus.Error;
                return Json(serviceResult);
            }



            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                CardName = Common.FaxerSession.LoggedUser.FullName,
                ExpirationMonth = vm.EndMM,
                ExpiringYear = vm.EndYY,
                Number = vm.CardNumber,
                SecurityCode = vm.SecurityCode,
                billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                CurrencyCode = Common.Common.GetCurrencyCode(Common.FaxerSession.LoggedUser.CountryCode),
                Amount = vm.FaxingAmount,
                SenderId = Common.FaxerSession.LoggedUser.Id
            };

            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm, Common.FaxerSession.LoggedUser.CountryCode, bankAccountDeposit.CountryCode);

            string CardType = AgentSession.CardType;


            if (!StripeResult.IsValid)
            {
                serviceResult.Data = null;
                serviceResult.Message = StripeResult.Message;
                serviceResult.Status = ResultStatus.Error;
                serviceResult.IsCardUsageMsg = StripeResult.IsCardUsageMsg;
                serviceResult.CardUsageMessage = StripeResult.CardUsageMsg;
                return Json(serviceResult);
            }

            else

            {
                SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

                _senderBankAccountDepositServices.SetDebitCreditCardDetail(vm);


                //  Fee Added statically (Bank Fee) it will be added dynamically after payment setup 
                //Credit/Debit card 
                //Fee: GBP 0.80
                //Manual Bank Deposit
                //Fee: GBP 0.79 
                var paymentInfo = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
                decimal faxingAmount = paymentInfo.TotalAmount + new CreditDebitCardViewModel().CreditDebitCardFee;
                decimal TotalAmount = Common.Common.CreditTypeFee(CardType, faxingAmount);
                //_senderBankAccountDepositServices.SetSenderBankAccoutDepositEnterAmount(paymentInfo);
                var transferSummary = _senderForAllTransferServices.GetTransactionSummary();
                transferSummary.CreditORDebitCardDetials = vm;
                transferSummary.CreditORDebitCardDetials.FaxingAmount = TotalAmount;
                transferSummary.KiiPayTransferPaymentSummary.Fee = paymentInfo.Fee;
                transferSummary.KiiPayTransferPaymentSummary.TotalAmount = paymentInfo.TotalAmount;
                _senderForAllTransferServices.SetTransactionSummary(transferSummary);
                _senderBankAccountDepositServices.SetTransactionSummary();
                var senderInfo = Common.FaxerSession.LoggedUser;

                SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
                StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
                {
                    Amount = TotalAmount,
                    Currency = Common.Common.GetCountryCurrency(senderInfo.CountryCode),
                    NameOnCard = "Charge for " + stripeResultIsValidCardVm.CardName,
                    StripeTokenId = StripeResult.StripeTokenId,
                    CardNum = stripeResultIsValidCardVm.Number,
                    ReceivingCountry = transferSummary.SenderAndReceiverDetail.ReceiverCountry,
                    SendingCountry = transferSummary.SenderAndReceiverDetail.SenderCountry,
                    ExipiryDate = stripeResultIsValidCardVm.ExpirationMonth + "/" + stripeResultIsValidCardVm.ExpiringYear,
                    SecurityCode = stripeResultIsValidCardVm.SecurityCode,
                    termurl = "/SenderBankAccountDeposit/ThreeDQueryResponseCallBack",
                    billingpostcode = senderInfo.PostCode,
                    billingpremise = senderInfo.HouseNo,
                    ReceiptNo = Common.FaxerSession.ReceiptNo,
                    SenderFirstName = senderInfo.FirstName,
                    SenderLastName = Common.Common.GetSenderLastName(),
                    SenderEmail = Common.Common.GetSenderInfo(senderInfo.Id).Email,
                    SenderId = senderInfo.Id,
                    ReceivingCurrency = Common.Common.GetCountryCurrency(transferSummary.SenderAndReceiverDetail.ReceiverCountry),
                };
                try
                {
                    var resultThreedQuery = StripServices.CreateThreedQuery(stripeCreateTransaction, TransactionTransferType.Online, 
                        TransactionTransferMethod.BankDeposit);
                    serviceResult.Message = resultThreedQuery.Message;
                    serviceResult.Status = resultThreedQuery.Status;
                    serviceResult.Data = resultThreedQuery.Data;
                    serviceResult.IsGetType3dAuth = resultThreedQuery.IsGetType3dAuth;
                    if (serviceResult.Status == ResultStatus.OK)
                    {
                        vm.ThreeDEnrolled = serviceResult.Data.ThreeDEnrolled == "Y" ? true : false;
                        _senderBankAccountDepositServices.SetDebitCreditCardDetail(vm);
                    }

                    //var resultThreedQuery = Transact365Serivces.CreatePayment(stripeCreateTransaction);
                    //serviceResult.Message = resultThreedQuery.Message;
                    //serviceResult.Status = resultThreedQuery.Status;
                    //serviceResult.Data = resultThreedQuery.Data;
                    //if (resultThreedQuery.Status == ResultStatus.OK)
                    //{
                    //    vm.ThreeDEnrolled = resultThreedQuery.Data.ThreeDEnrolled == "Y" ? true : false;
                    //    _senderBankAccountDepositServices.SetDebitCreditCardDetail(vm);
                    //    serviceResult.IsGetType3dAuth = resultThreedQuery.IsGetType3dAuth;
                    //}

                    if (!vm.ThreeDEnrolled)
                    {
                        serviceResult.Data = null;
                        serviceResult.Message = resultThreedQuery.Message;
                        serviceResult.Status = ResultStatus.Error;
                        return Json(serviceResult);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex.Message, ErrorType.PaymentGateway, "SenderBankAccountDeposit/ThreeDQuery");
                }
            }
            return Json(serviceResult);

        }

        [HttpGet]
        public ActionResult ThreeDQueryResponseCallBack(string uid, string id)
        {
            try
            {
                CustomerResponseVm vm = new CustomerResponseVm();
                var result = Transact365Serivces.GetTransationDetails(uid,id);
                if (result.Status == ResultStatus.Error)
                {
                    return RedirectToAction("DebitCreditCardDetails");
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ErrorType.PaymentGateway, "SenderBankAccountDeposit/ThreeDQueryResponseCallBack");
                return RedirectToAction("DebitCreditCardDetails");
            }
        }



        [HttpPost]
        public ActionResult ThreeDQueryResponseCallBack([Bind(Include = CustomerResponseVm.BindProperty)] CustomerResponseVm responseVm)
        {
            try
            {
                return View(responseVm);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ErrorType.PaymentGateway, "SenderBankAccountDeposit/ThreeDQueryResponseCallBack");
                return RedirectToAction("DebitCreditCardDetails");
            }
        }


        [HttpPost]
        public ActionResult DebitCreditCardDetails([Bind(Include = CustomerResponseVm.BindProperty)] CustomerResponseVm responseVm)
        {


            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            var transferSummary = _senderForAllTransferServices.GetTransactionSummary();
            ViewBag.HasOneSavedCard = Common.Common.HasOneCardSaved();


            var vm = new CreditDebitCardViewModel();
            vm = transferSummary.CreditORDebitCardDetials;
            var senderInfo = Common.Common.GetSenderInfo(Common.FaxerSession.LoggedUser.Id);
            StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
            {
                Amount = vm.FaxingAmount,
                Currency = Common.Common.GetCurrencyCode(Common.FaxerSession.LoggedUser.CountryCode),
                md = responseVm.MD,
                pares = responseVm.PaRes,
                parenttransactionreference = responseVm.parenttransactionreference,
                ThreeDEnrolled = transferSummary.CreditORDebitCardDetials.ThreeDEnrolled,
                billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                ReceiptNo = Common.FaxerSession.ReceiptNo,
                SenderId = Common.FaxerSession.LoggedUser.Id,
                CardNum = vm.CardNumber,
                state = senderInfo.State,
                city = senderInfo.City
            };
            var cardProcessor = StripServices.GetCardProcessor(stripeCreateTransaction, TransactionTransferType.Online, TransactionTransferMethod.BankDeposit);
            transferSummary.CardProcessorApi = cardProcessor;
            _senderForAllTransferServices.SetTransactionSummary(transferSummary);

            switch (cardProcessor)
            {
                case CardProcessorApi.TrustPayment:
                    var transactionResult = new StripeResult();

                    transactionResult = StripServices.CreateTransaction(stripeCreateTransaction);
                    if (transactionResult == null)
                    {
                        ModelState.AddModelError("TransactionError", transactionResult.Message);
                        transferSummary.CreditORDebitCardDetials.SecurityCode = "";
                        return View(transferSummary.CreditORDebitCardDetials);
                    }
                    if (transactionResult.IsValid == false)
                    {
                        transferSummary.CreditORDebitCardDetials.IsCardUsageMsg = transactionResult.IsCardUsageMsg;
                        transferSummary.CreditORDebitCardDetials.CardUsageMsg = transactionResult.CardUsageMsg;
                        transferSummary.CreditORDebitCardDetials.ErrorMsg = transactionResult.Message;
                        ModelState.AddModelError("StripeError", transactionResult.Message);
                        return View(transferSummary.CreditORDebitCardDetials);
                    }
                    break;
                case CardProcessorApi.T365:
                    break;
            }




            //var Bankdepositdata = _senderBankAccountDepositServices.List().Data.Where(x => x.Id == Common.FaxerSession.TransactionId).FirstOrDefault();
            //if (Bankdepositdata != null)
            //{
            //    Bankdepositdata.Status = BankDepositStatus.ReInitialise;
            //    _senderBankAccountDepositServices.Update(Bankdepositdata);
            //}
            var SenderDocumentApprovalStatus = Common.Common.GetSenderIdentificationStatus(Common.FaxerSession.LoggedUser.Id);

            transferSummary.IsIdCheckInProgress = Common.Common.IsSenderIdCheckInProgress(Common.FaxerSession.LoggedUser.Id);
            //if (SenderDocumentApprovalStatus == null) {

            //    transferSummary.IsIdCheckInProgress = false;
            //}
            //if (SenderDocumentApprovalStatus != null && SenderDocumentApprovalStatus != DocumentApprovalStatus.Approved)
            //{


            //    transferSummary.IsIdCheckInProgress = false;
            //}

            _senderForAllTransferServices.CompleteTransaction(transferSummary);

            switch (SenderDocumentApprovalStatus)
            {
                case null:
                    return RedirectToAction("IdentificationInformation");
                case DocumentApprovalStatus.Approved:
                    break;
                case DocumentApprovalStatus.Disapproved:
                    return RedirectToAction("IdentificationInformation");
                case DocumentApprovalStatus.InProgress:
                    return RedirectToAction("IdentificationInformation");
                    break;
                default:
                    break;
            }
            if (transferSummary.IsLocalPayment == false)
            {

                return RedirectToAction("AddMoneyToWalletSuccess");
            }
            else
            {

                return RedirectToAction("LocalAddMoneyToWalletSuccess");
            }

        }


        [HttpGet]
        public ActionResult IdentificationInformation()
        {

            GetCountries();
            GetIdTypes();
            var vm = _senderBankAccountDepositServices.GetIdentificationDetailViewModel(Common.FaxerSession.LoggedUser.Id);
            if (vm != null)
            {
                if (vm.ExpiryDate.HasValue)
                {
                    bool isValidExpityDate = Common.DateUtilities.DateGreaterThanToday(vm.ExpiryDate);
                    if (!isValidExpityDate)
                    {
                        ModelState.AddModelError("", "ID has been Expired.Update Your Id");
                    }
                    else
                    {
                        if (vm.Status == DocumentApprovalStatus.InProgress || vm.Status == DocumentApprovalStatus.Approved)
                        {
                            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
                            var transferSummary = _senderForAllTransferServices.GetTransactionSummary();
                            var paymentMethod = transferSummary.PaymentMethodAndAutoPaymentDetail;
                            switch (transferSummary.TransferType)
                            {
                                case TransferType.BankDeposit:
                                    if (paymentMethod.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
                                    {
                                        return RedirectToAction("MoneyFexBankDeposit");
                                    }
                                    else
                                    {
                                        if (transferSummary.IsLocalPayment == false)
                                        {
                                            return RedirectToAction("AddMoneyToWalletSuccess");
                                        }
                                        else
                                        {
                                            return RedirectToAction("LocalAddMoneyToWalletSuccess");
                                        }
                                    }
                                case TransferType.CashPickup:
                                    if (paymentMethod.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
                                    {
                                        return RedirectToAction("MoneyFexBankDeposit", "SenderCashPickUp");
                                    }
                                    else
                                    {

                                        return RedirectToAction("CashPickUpSuccess", "SenderCashPickUp");

                                    }
                                case TransferType.MobileTransfer:
                                    if (paymentMethod.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
                                    {
                                        return RedirectToAction("MoneyFexBankDeposit", "SenderMobileMoneyTransfer");
                                    }
                                    else
                                    {
                                        if (transferSummary.IsLocalPayment == false)
                                        {
                                            return RedirectToAction("AddMoneyToWalletSuccess", "SenderMobileMoneyTransfer");
                                        }
                                        else
                                        {
                                            return RedirectToAction("LocalAddMoneyToWalletSuccess", "SenderMobileMoneyTransfer");
                                        }
                                    }
                            }
                        }
                    }

                }
            }
            else
            {
                vm = new IdentificationDetailModel();
            }
            return View(vm);
        }

        public void GetCountries()
        {

            var result = (from c in Common.Common.GetCountriesForDropDown()
                          select new IssuingCountryModel()
                          {
                              IssuingCountry = c.CountryCode,
                              IssuingCountryName = c.CountryName,
                          }).ToList();
            ViewBag.Countries = new SelectList(result, "IssuingCountry", "IssuingCountryName");
        }
        public void GetIdTypes()
        {
            var IdenticationTypes = (from c in Common.Common.GetIdTypes()
                                     select new IdentificationTypeModel()
                                     {

                                         IdentificationTypeId = c.Id,
                                         Name = c.CardType
                                     }).ToList();
            ViewBag.IdenticationTypes = new SelectList(IdenticationTypes.ToList(), "IdentificationTypeId", "Name");
        }
        [HttpPost]
        public ActionResult IdentificationInformation([Bind(Include = IdentificationDetailModel.BindProperty)] IdentificationDetailModel model)
        {


            GetCountries();
            GetIdTypes();

            if (ModelState.IsValid)
            {


                var ExpiryDate = new DateTime(model.Year, (int)model.Month, model.Day);
                if (ExpiryDate < DateTime.Now)
                {
                    ModelState.AddModelError("ExpiryDate", "ID has been expired");
                    return View(model);
                }

                if (Request.Files.Count == 0)
                {
                    ModelState.AddModelError("document", "Choose file to upload");
                    return View(model);
                }

                SenderDocumentationServices _senderDocumentationServices = new SenderDocumentationServices();


                string DocumentUrl = null;
                string DocumentUrlTwo = null;
                if (model.SenderBusinessDocumentationId > 0)
                {
                    var senderBusinessDocumentation = _senderDocumentationServices.GetDocumentDetails(model.SenderBusinessDocumentationId);
                    if (senderBusinessDocumentation != null)
                    {
                        DocumentUrl = senderBusinessDocumentation.DocumentPhotoUrl;
                        DocumentUrlTwo = senderBusinessDocumentation.DocumentPhotoUrlTwo;
                    }
                }
                else
                {
                    if (model.DocumentUrl == null)
                    {
                        ModelState.AddModelError("document", "Upload Id.");
                        return View(model);
                    }

                }
                if (Request.Files.Count < 1)
                {
                    var identificationdoc = Request.Files["DocumentUrl"];
                    var identificationdocTwo = Request.Files["DocumentUrlTwo"];
                }
                var IdentificationDoc = Request.Files["DocumentUrl"];
                var IdentificationDocTwo = Request.Files["DocumentUrlTwo"];
                var DocumentData = new ServiceResult<string>();

                if (IdentificationDoc != null && IdentificationDoc.ContentLength > 0)
                {
                    DocumentData = Common.Common.GetDocumentPath(IdentificationDoc);
                    if (DocumentData.Status == ResultStatus.OK)
                    {
                        model.DocumentUrl = DocumentData.Data;
                    }
                    else
                    {
                        ModelState.AddModelError("document", DocumentData.Message);
                        return View(model);
                    }
                }


                if (IdentificationDocTwo != null && IdentificationDocTwo.ContentLength > 0)
                {
                    DocumentData = Common.Common.GetDocumentPath(IdentificationDocTwo);
                    if (DocumentData.Status == ResultStatus.OK)
                    {
                        model.DocumentUrlTwo = DocumentData.Data;
                    }
                    else
                    {
                        ModelState.AddModelError("documentTwo", DocumentData.Message);
                        return View(model);
                    }
                }


                int senderId = Common.FaxerSession.LoggedUser.Id;
                string DocumentName = "";
                try
                {
                    DocumentName = IdentificationDoc.FileName.Split('.')[0];
                }
                catch (Exception)
                {

                }

                CommonServices _CommonServices = new CommonServices();
                var senderInfo = _CommonServices.GetSenderInfo(senderId);
                var senderDocumentation = _CommonServices.GetSenderDocumentation(senderId);
                if (senderDocumentation.Count > 0)
                {
                    if (model.DocumentUrl == null)
                    {
                        model.DocumentUrl = DocumentUrl;
                    }
                    if (model.DocumentUrlTwo == null)
                    {
                        model.DocumentUrlTwo = DocumentUrlTwo;
                    }

                    SenderDocumentationViewModel vm = (from c in senderDocumentation
                                                       select new SenderDocumentationViewModel()
                                                       {
                                                           AccountNo = c.AccountNo,
                                                           DocumentPhotoUrl = model.DocumentUrl,
                                                           DocumentPhotoUrlTwo = model.DocumentUrlTwo,
                                                           City = c.City,
                                                           Country = c.Country,
                                                           CreatedBy = c.CreatedBy,
                                                           CreatedDate = c.CreatedDate,
                                                           DocumentName = model.IdentityNumber,
                                                           DocumentExpires = DocumentExpires.Yes,
                                                           DocumentType = DocumentType.Compliance,
                                                           IssuingCountry = model.IssuingCountry,
                                                           SenderId = c.SenderId,
                                                           Status = DocumentApprovalStatus.InProgress,
                                                           Id = c.Id,
                                                           ExpiryDate = ExpiryDate,
                                                           IdentificationTypeId = model.IdentificationTypeId,
                                                           IdentityNumber = model.IdentityNumber
                                                       }).FirstOrDefault();
                    _senderDocumentationServices.UpdateDocument(vm);
                }
                else
                {
                    SenderDocumentationViewModel vm = new SenderDocumentationViewModel()
                    {
                        SenderId = senderId,
                        AccountNo = senderInfo.AccountNo,
                        City = senderInfo.City,
                        Country = senderInfo.Country,
                        CreatedDate = DateTime.Now,
                        DocumentName = model.IdentityNumber,
                        DocumentExpires = DocumentExpires.Yes,
                        DocumentType = DocumentType.Compliance,
                        DocumentPhotoUrl = model.DocumentUrl,
                        DocumentPhotoUrlTwo = model.DocumentUrlTwo,
                        SenderName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName,
                        Status = DocumentApprovalStatus.InProgress,
                        IsUploadedFromSenderPortal = true,
                        IssuingCountry = model.IssuingCountry,
                        IdentificationTypeId = model.IdentificationTypeId,
                        IdentityNumber = model.IdentityNumber,
                        ExpiryDate = ExpiryDate

                    };
                    _senderDocumentationServices.UploadDocument(vm);
                }

                _senderDocumentationServices.SendIdentiVerificationInProgressEmail(senderId);


                SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
                //var transferSummary = _senderForAllTransferServices.GetTransactionSummary();
                //transferSummary.IsIdCheckInProgress = true;
                //_senderForAllTransferServices.SetTransactionSummary(transferSummary);
                //_senderForAllTransferServices.CompleteTransaction(transferSummary);

                return RedirectToAction("IdentificationUploadSuccess");
            }

            return View(model);
        }


        public ActionResult IdentificationUploadSuccess()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            senderCommonFunc.ClearTransferBankDepositSession();

            return View();

        }

        public JsonResult IsCrebitCard(string cardNumber, string month, string year, string securityCode)
        {


            try
            {
                var number = cardNumber.Split(' ');
                cardNumber = string.Join("", number);
                StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
                {
                    CardName = Common.FaxerSession.LoggedUser.FullName,
                    ExpirationMonth = month,
                    ExpiringYear = year,
                    Number = cardNumber,
                    SecurityCode = securityCode,

                    billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                    billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                    CurrencyCode = Common.Common.GetCurrencyCode(Common.FaxerSession.LoggedUser.CountryCode),
                    Amount = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount().TotalAmount,
                    IsCreditDebitCardCheck = true,
                    SenderId = Common.FaxerSession.LoggedUser.Id
                };
                var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm, Common.FaxerSession.LoggedUser.CountryCode, _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount().ReceivingCountryCode);

                decimal amount = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount().TotalAmount;
                amount = (decimal)Math.Round((0.601 / 100), 2) * amount;
                return Json(new
                {
                    IsCrebitCard = StripeResult.IsCreditCard,
                    ExtraAmount = Common.Common.GetCurrencySymbol(Common.FaxerSession.LoggedUser.CountryCode) + amount
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    IsCrebitCard = false,
                    ExtraAmount = 0
                }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult MoneyFexBankDeposit()
        {
            SenderMoneyFexBankDepositVM vm = new SenderMoneyFexBankDepositVM();

            var sendingAmountData = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
            ViewBag.ReceivingCountryCurrency = sendingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Bank Deposit";
            ViewBag.SendingCountryCurrency = sendingAmountData.SendingCurrencyCode;
            ViewBag.SendingAmount = sendingAmountData.SendingAmount;
            ViewBag.ReceiverName = sendingAmountData.ReceiverName;
            ViewBag.ReceivingCountry = sendingAmountData.ReceivingCountryCode.ToLower();

            //  Fee Added statically (Bank Fee) it will be added dynamically after payment setup 
            //Credit/Debit card 
            //Fee: GBP 0.80
            //Manual Bank Deposit
            //Fee: GBP 0.79

            decimal addedBankFee = sendingAmountData.TotalAmount + vm.BankFee;
            vm.Amount = addedBankFee;
            vm.SendingCurrencyCode = sendingAmountData.SendingCurrencyCode;
            vm.SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol;
            _senderBankAccountDepositServices.SetMoneyFexBankAccountDeposit(vm);
            vm = _senderBankAccountDepositServices.GetMoneyFexBankAccountDeposit();

            return View(vm);
        }


        [HttpPost]
        public ActionResult MoneyFexBankDeposit([Bind(Include = SenderMoneyFexBankDepositVM.BindProperty)] SenderMoneyFexBankDepositVM model)
        {
            var sendingAmountData = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
            ViewBag.ReceivingCountryCurrency = sendingAmountData.ReceivingCurrencyCode;
            ViewBag.TransferMethod = "Bank Deposit";
            ViewBag.SendingCountryCurrency = sendingAmountData.SendingCurrencyCode;
            ViewBag.SendingAmount = sendingAmountData.SendingAmount;
            ViewBag.ReceiverName = sendingAmountData.ReceiverName;
            ViewBag.ReceivingCountry = sendingAmountData.ReceivingCountryCode.ToLower();
            ViewBag.Fee = sendingAmountData.Fee;
            if (ModelState.IsValid)
            {
                SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

                var transferSummary = _senderForAllTransferServices.GetTransactionSummary();

                var moneyFexBankAccountDeposit = _senderBankAccountDepositServices.GetMoneyFexBankAccountDeposit();
                moneyFexBankAccountDeposit.HasMadePaymentToBankAccount = model.HasMadePaymentToBankAccount;


                //_senderBankAccountDepositServices.SetSenderBankAccoutDepositEnterAmount(sendingAmountData);

                //_senderBankAccountDepositServices.SetMoneyFexBankAccountDeposit(moneyFexBankAccountDeposit);


                _senderBankAccountDepositServices.SetTransactionSummary();



                var SenderDocumentApprovalStatus = Common.Common.GetSenderIdentificationStatus(Common.FaxerSession.LoggedUser.Id);

                transferSummary.IsIdCheckInProgress = Common.Common.IsSenderIdCheckInProgress(Common.FaxerSession.LoggedUser.Id);
                //if (SenderDocumentApprovalStatus != null && SenderDocumentApprovalStatus == DocumentApprovalStatus.Approved)
                //{


                //    transferSummary.IsIdCheckInProgress = false;
                //}

                _senderForAllTransferServices.CompleteTransaction(transferSummary);

                switch (SenderDocumentApprovalStatus)
                {
                    case null:
                        return RedirectToAction("IdentificationInformation");
                    case DocumentApprovalStatus.Approved:
                        break;
                    case DocumentApprovalStatus.Disapproved:
                        return RedirectToAction("IdentificationInformation");
                    case DocumentApprovalStatus.InProgress:
                        return RedirectToAction("IdentificationInformation");
                        break;
                    default:
                        break;
                }

                return RedirectToAction("Success", "MoneyFexBankDeposit");
            }

            return View(model);
        }


        public ActionResult AddMoneyToWalletSuccess()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var data = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
            SenderAddMoneySuccessVM result = new SenderAddMoneySuccessVM()
            {
                ReceiverName = data.ReceiverName,
                Amount = data.SendingAmount,
                Currnecy = data.SendingCurrencySymbol
            };

            SenderBankAccountDepositVm vm = new SenderBankAccountDepositVm();
            _senderBankAccountDepositServices.SetSenderBankAccountDeposit(vm);
            senderCommonFunc.ClearTransferBankDepositSession();
            Common.FaxerSession.IsTransferFromHomePage = false;
            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Payment Confirmation");
            return View(result);

        }

        #endregion

        #region Local Bank Deposit


        public ActionResult LocalBankAccountDeposit(string RecentAccountNo = "")
        {
            SenderBankAccountDepositVm vm = new SenderBankAccountDepositVm();

            vm = _senderBankAccountDepositServices.GetSenderBankAccountDeposit();

            var recentAccountNumbers = _senderBankAccountDepositServices.GetRecentAccountNumbers().Where(x => x.CountryCode ==
            Common.FaxerSession.LoggedUser.CountryCode).ToList();
            string CountryCode = Common.FaxerSession.LoggedUser.CountryCode;
            SetViewBagForBanks(CountryCode);

            var branches = _senderBankAccountDepositServices.GetBranches();

            ViewBag.RecentAccountNumbers = new SelectList(recentAccountNumbers, "Name", "Code", RecentAccountNo);
            ViewBag.Branches = new SelectList(branches, "Id", "Name");
            getRecentAccountno(ref vm, RecentAccountNo);
            return View(vm);
        }

        [HttpPost]
        public ActionResult LocalBankAccountDeposit([Bind(Include = SenderBankAccountDepositVm.BindProperty)] SenderBankAccountDepositVm model)
        {
            var recentAccountNumbers = _senderBankAccountDepositServices.GetRecentAccountNumbers().Where(x => x.CountryCode == Common.FaxerSession.LoggedUser.CountryCode).ToList();
            var branches = _senderBankAccountDepositServices.GetBranches();
            string CountryCode = Common.FaxerSession.LoggedUser.CountryCode;
            SetViewBagForBanks(CountryCode);
            model.CountryCode = CountryCode;
            ViewBag.RecentAccountNumbers = new SelectList(recentAccountNumbers, "Name", "Code");
            ViewBag.Branches = new SelectList(branches, "Id", "Name");

            _senderBankAccountDepositServices.SetSenderBankAccountDeposit(model);

            //for enter amount Page
            var loggedInSenderData = _senderBankAccountDepositServices.GetLoggedUserData();
            //var receiverInformation = _senderBankAccountDepositServices.GetReceiverInformationFromAccountNumnber(model.AccountNumber);
            SenderBankAccoutDepositEnterAmountVm localbankDepositEnterAmount = new SenderBankAccoutDepositEnterAmountVm()
            {
                ReceiverName = model.AccountOwnerName,
                SendingCurrencySymbol = Common.Common.GetCurrencySymbol(loggedInSenderData.CountryCode),
                SendingCurrencyCode = Common.Common.GetCountryCurrency(loggedInSenderData.CountryCode),
                ReceiverId = 0,
                Image = "",
                ReceivingCurrencyCode = Common.Common.GetCountryCurrency(loggedInSenderData.CountryCode),
                ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(loggedInSenderData.CountryCode),

            };
            //SSenderForAllTransfer.SetPaymentSummarySession(loggedInSenderData.CountryCode);
            _senderBankAccountDepositServices.SetSenderBankAccoutDepositEnterAmount(localbankDepositEnterAmount);
            if (ModelState.IsValid)
            {
                return RedirectToAction("LocalBankDepositEnterAmount", "SenderBankAccountDeposit");

            }

            return View(model);
        }


        public ActionResult LocalBankDepositEnterAmount()
        {


            var loggedUserData = _senderBankAccountDepositServices.GetLoggedUserData();
            var localBankAccountAmount = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();

            SenderBankAccoutDepositEnterAmountVm vm = new SenderBankAccoutDepositEnterAmountVm()
            {
                SendingCurrencySymbol = Common.Common.GetCurrencySymbol(loggedUserData.CountryCode),
                SendingCurrencyCode = Common.Common.GetCountryCurrency(loggedUserData.CountryCode),
                Image = localBankAccountAmount.Image,
                ReceiverName = localBankAccountAmount.ReceiverName,
                ReceiverId = localBankAccountAmount.ReceiverId,
            };
            _senderBankAccountDepositServices.SetSenderBankAccoutDepositEnterAmount(vm);

            return View(vm);
        }

        [HttpPost]
        public ActionResult LocalBankDepositEnterAmount([Bind(Include = SenderBankAccoutDepositEnterAmountVm.BindProperty)] SenderBankAccoutDepositEnterAmountVm model)
        {
            if (model.SendingAmount != 0)
            {
                var data = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
                data.SendingAmount = model.SendingAmount;
                data.TotalAmount = model.SendingAmount;
                data.ReceivingAmount = model.SendingAmount;
                data.ReceivingCurrencyCode = data.SendingCurrencyCode;
                data.ExchangeRate = 1M;

                if (data.SendSms == true)
                {
                    data.SmsCharge = Common.Common.GetSmsFee(Common.FaxerSession.LoggedUser.CountryCode);
                }
                data.TotalAmount = data.TotalAmount + data.SmsCharge;

                _senderBankAccountDepositServices.SetSenderBankAccoutDepositEnterAmount(data);
                return RedirectToAction("LocalBankDepositSummary", "SenderBankAccountDeposit");

            }
            return View(model);
        }

        public ActionResult LocalBankDepositSummary()
        {
            var sendingAmountData = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
            var loggedUSerData = _senderBankAccountDepositServices.GetLoggedUserData();
            string fullName = sendingAmountData.ReceiverName;
            var names = fullName.Split(' ');
            string firstName = names[0];

            SenderAccountPaymentSummaryViewModel vm = new SenderAccountPaymentSummaryViewModel()
            {
                Amount = sendingAmountData.SendingAmount,
                SendingCurrencyCode = sendingAmountData.SendingCurrencyCode,
                SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
                ReceivedAmount = sendingAmountData.SendingAmount,
                ReceiverName = firstName,
                LocalSmsCharge = sendingAmountData.SmsCharge,
                PaidAmount = sendingAmountData.TotalAmount,
                ReceivingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
                ReceivingCurrecyCoode = sendingAmountData.ReceivingCurrencyCode
            };


            return View(vm);

        }

        [HttpPost]
        public ActionResult LocalBankDepositSummary([Bind(Include = SenderAccountPaymentSummaryViewModel.BindProperty)] SenderAccountPaymentSummaryViewModel model)
        {
            return RedirectToAction("LocalPayment", "SenderBankAccountDeposit");
        }

        public ActionResult LocalPayment()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var vm = _senderBankAccountDepositServices.GetPaymentMethod();
            var sendsingAmountData = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
            vm.TotalAmount = sendsingAmountData.TotalAmount;
            vm.SendingCurrencySymbol = sendsingAmountData.SendingCurrencySymbol;

            var senderwalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
            if (senderwalletInfo != null)
            {

                vm.KiipayWalletBalance = senderwalletInfo.CurrentBalance;
                vm.HasKiiPayWallet = true;

            }
            else
            {
                vm.HasKiiPayWallet = false;
            }

            vm.CardDetails = senderCommonFunc.GetSavedDebitCreditCardDetails();

            return View(vm);
        }


        [HttpPost]
        public ActionResult LocalPayment([Bind(Include = PaymentMethodViewModel.BindProperty)] PaymentMethodViewModel vm)
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            int selectedCardId = 0;
            if (vm.CardDetails != null)
            {
                foreach (var item in vm.CardDetails)
                {
                    if (item.IsChecked == true)
                    {
                        selectedCardId = item.CardId;
                        vm.SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard;
                    }
                }
            }

            _senderBankAccountDepositServices.SetPaymentMethod(vm);



            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();


            _senderBankAccountDepositServices.SetTransactionSummary();

            var transactionSummaryvm = _senderForAllTransferServices.GetTransactionSummary();

            switch (vm.SenderPaymentMode)
            {
                case SenderPaymentMode.SavedDebitCreditCard:
                    SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
                    var cardInfo = _saveCreditDebitCard.GetCardInfo(selectedCardId);
                    transactionSummaryvm.CreditORDebitCardDetials = new CreditDebitCardViewModel()
                    {
                        CardNumber = cardInfo.Num.Decrypt(),
                        NameOnCard = cardInfo.CardName.Decrypt(),
                        EndMM = cardInfo.EMonth.Decrypt(),
                        EndYY = cardInfo.EYear.Decrypt(),
                        SecurityCode = cardInfo.ClientCode.Decrypt()
                    };

                    var result = _senderForAllTransferServices.ValidateTransactionUsingStripe(transactionSummaryvm);
                    if (result.IsValid == false)
                    {

                        ModelState.AddModelError("TransactionError", result.Message);
                        return View(vm);
                    }

                    transactionSummaryvm.CreditORDebitCardDetials.StripeTokenID = result.StripeTokenId;
                    _senderForAllTransferServices.CompleteTransaction(transactionSummaryvm);

                    break;
                case SenderPaymentMode.CreditDebitCard:


                    return RedirectToAction("DebitCreditCardDetails");

                case SenderPaymentMode.KiiPayWallet:

                    var hasEnoughBal = senderCommonFunc.SenderHasEnoughWalletBaltoTransfer(transactionSummaryvm.KiiPayTransferPaymentSummary.TotalAmount, transactionSummaryvm.SenderAndReceiverDetail.SenderWalletId);

                    if (hasEnoughBal == false)
                    {


                        ModelState.AddModelError("TransactionError", "Your wallet doesn't have enough balance!");
                        return View(vm);
                    }
                    _senderForAllTransferServices.CompleteTransaction(transactionSummaryvm);
                    break;
                case SenderPaymentMode.MoneyFexBankAccount:

                    return RedirectToAction("MoneyFexBankDeposit");

                default:
                    break;
            }

            return RedirectToAction("LocalAddMoneyToWalletSuccess");


        }




        //public ActionResult LocalMoneyFexBankDeposit()
        //{


        //    var sendingAmountData = _senderBankAccountDepositServices.GetSenderLocalBankAccountEnterAmount();
        //    SenderMoneyFexBankDepositVM vm = new SenderMoneyFexBankDepositVM()
        //    {
        //        Amount = sendingAmountData.Amount,
        //        SendingCurrencyCode = sendingAmountData.CurrencyCode,
        //        SendingCurrencySymbol = sendingAmountData.CurrencySymbol
        //    };

        //    return View(vm);
        //}

        //[HttpPost]
        //public ActionResult LocalMoneyFexBankDeposit(SenderMoneyFexBankDepositVM model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _senderBankAccountDepositServices.SetMoneyFexBankAccountDeposit(model);
        //        return RedirectToAction("LocalAddMoneyToWalletSuccess", "SenderBankAccountDeposit");
        //    }

        //    return View(model);
        //}



        public ActionResult LocalAddMoneyToWalletSuccess()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var data = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
            SenderAddMoneySuccessVM result = new SenderAddMoneySuccessVM()
            {
                ReceiverName = data.ReceiverName,
                Amount = data.SendingAmount,
                Currnecy = data.SendingCurrencySymbol
            };
            senderCommonFunc.ClearTransferBankDepositSession();
            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Payment Confirmation");
            return View(result);
        }







        #endregion

        //For  onchange Events

        public JsonResult GetAccountInformation(string accountNo, string CountryCode)
        {

            var accountData = _senderBankAccountDepositServices.GetAccountInformationFromAccountNumber(accountNo, CountryCode);
            if (accountData != null)
            {
                return Json(new
                {
                    AccountOwnerName = accountData.AccountOwnerName,
                    Country = accountData.CountryCode.ToUpper(),
                    CountryPhoneCode = accountData.CountryPhoneCode,
                    MobileNumber = accountData.MobileNumber,
                    AccountNumber = accountData.AccountNumber,
                    BankId = accountData.BankId,
                    BranchCode = accountData.BranchCode
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult IsCVVCodeValid(string securityCode = "", int cardId = 0)
        {
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            var bankAccountDeposit = _senderBankAccountDepositServices.GetSenderBankAccountDeposit();
            var HasExceededBankDepositReceiverLimit = Common.Common.HasExceededReceiverLimit(Common.FaxerSession.SenderId,
                    bankAccountDeposit.ReceipientId, Common.FaxerSession.LoggedUser.CountryCode, bankAccountDeposit.CountryCode, TransactionTransferMethod.BankDeposit);
            if (HasExceededBankDepositReceiverLimit)
            {
                return Json(new ServiceResult<CreditDebitCardViewModel>()
                {
                    Data = new CreditDebitCardViewModel(),
                    Status = ResultStatus.Error,
                    Message = "Recipient daily transaction limit exceeded"
                }, JsonRequestBehavior.AllowGet);
            }
            bool HasSenderTransacionExceedLimit = Common.Common.HasExceededSenderTransactionLimit(Common.FaxerSession.SenderId, Common.FaxerSession.LoggedUser.CountryCode, bankAccountDeposit.CountryCode, TransactionTransferMethod.BankDeposit);
            if (HasSenderTransacionExceedLimit)
            {
                return Json(new ServiceResult<CreditDebitCardViewModel>()
                {
                    Data = new CreditDebitCardViewModel(),
                    Status = ResultStatus.Error,
                    Message = "Sender daily transaction limit exceeded"
                }, JsonRequestBehavior.AllowGet);
            }

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            PaymentMethodViewModel vm = new PaymentMethodViewModel()
            {
                HasEnableMoneyFexBankAccount = senderCommonFunc.IsEnabledMoneyFexbankAccount(Common.FaxerSession.LoggedUser.CountryCode),
                SenderPaymentMode = SenderPaymentMode.SavedDebitCreditCard,
            };
            _senderBankAccountDepositServices.SetPaymentMethod(vm);
            _senderBankAccountDepositServices.SetTransactionSummary();
            var transactionSummaryvm = _senderForAllTransferServices.GetTransactionSummary();

            SSaveCreditDebitCardServices _saveCreditDebitCard = new SSaveCreditDebitCardServices();
            var cardInfo = _saveCreditDebitCard.GetCardInfo(cardId);
            transactionSummaryvm.CreditORDebitCardDetials = new CreditDebitCardViewModel()
            {
                CardNumber = cardInfo.Num.Decrypt(),
                NameOnCard = cardInfo.CardName.Decrypt(),
                EndMM = cardInfo.EMonth.Decrypt(),
                EndYY = cardInfo.EYear.Decrypt(),
                SecurityCode = securityCode
                //SecurityCode = cardInfo.ClientCode.Decrypt()
            };


            var validateCardresult = Common.Common.ValidateCreditDebitCard(transactionSummaryvm.CreditORDebitCardDetials);
            if (validateCardresult.Data == false)
            {
                ModelState.AddModelError("TransactionError", validateCardresult.Message);
                return Json(new ServiceResult<CreditDebitCardViewModel>()
                {
                    Data = new CreditDebitCardViewModel(),
                    Status = ResultStatus.Error,
                    Message = validateCardresult.Message
                }, JsonRequestBehavior.AllowGet);
            }
            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                CardName = Common.FaxerSession.LoggedUser.FullName,
                ExpirationMonth = transactionSummaryvm.CreditORDebitCardDetials.EndMM,
                ExpiringYear = transactionSummaryvm.CreditORDebitCardDetials.EndYY,
                Number = transactionSummaryvm.CreditORDebitCardDetials.CardNumber,
                SecurityCode = transactionSummaryvm.CreditORDebitCardDetials.SecurityCode,
                billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                CurrencyCode = Common.Common.GetCurrencyCode(Common.FaxerSession.LoggedUser.CountryCode),
                Amount = transactionSummaryvm.CreditORDebitCardDetials.FaxingAmount,
                SenderId = Common.FaxerSession.LoggedUser.Id
            };

            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm, Common.FaxerSession.LoggedUser.CountryCode, bankAccountDeposit.CountryCode);
            if (StripeResult.IsValid == false)
            {
                return Json(new ServiceResult<CreditDebitCardViewModel>()
                {
                    Data = new CreditDebitCardViewModel(),
                    Status = ResultStatus.Error,
                    Message = StripeResult.Message
                }, JsonRequestBehavior.AllowGet);
            }
            _senderBankAccountDepositServices.SetDebitCreditCardDetail(transactionSummaryvm.CreditORDebitCardDetials);
            return Json(new ServiceResult<CreditDebitCardViewModel>()
            {
                Data = transactionSummaryvm.CreditORDebitCardDetials,
                Status = ResultStatus.OK,
                Message = "Success"
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckifFlutterWaveApi(string CountryCode = "")
        {
            var isFlutterwaveApi = false;
            decimal SendingAmount = _kiiPaytrasferServices.GetCommonEnterAmount().SendingAmount;

            var Apiservice = Common.Common.GetApiservice(Common.FaxerSession.LoggedUser.CountryCode, CountryCode, SendingAmount,
                       TransactionTransferMethod.BankDeposit, TransactionTransferType.Online);
            if (Apiservice == DB.Apiservice.FlutterWave)
            {
                isFlutterwaveApi = true;
            }
            return Json(isFlutterwaveApi, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount, bool IsReceivingAmount)
        {

            var enterAmountData = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
            var loggedInSenderData = _senderBankAccountDepositServices.GetLoggedUserData();

            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }
            var feeInfo = SEstimateFee.GetTransferFee(loggedInSenderData.CountryCode, _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount().ReceivingCountryCode
                , TransactionTransferMethod.BankDeposit, enterAmountData.SendingAmount, TransactionTransferType.Online);


            var result = new EstimateFaxingFeeSummary();
            try
            {
                result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                enterAmountData.ExchangeRate, feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);

            }
            catch (Exception)
            {

                return Json(new
                {
                    Fee = result.FaxingFee,
                    TotalAmount = result.TotalAmount,
                    ReceivingAmount = result.ReceivingAmount,
                    SendingAmount = result.FaxingAmount,
                    ReceiveAmount = result.ReceivingAmount,
                    HasFeeSetup = false
                }, JsonRequestBehavior.AllowGet);
            }


            // Rewrite session with additional value 
            enterAmountData.Fee = result.FaxingFee;
            enterAmountData.SendingAmount = result.FaxingAmount;
            enterAmountData.ReceivingAmount = result.ReceivingAmount;
            enterAmountData.TotalAmount = result.TotalAmount;

            _senderBankAccountDepositServices.SetSenderBankAccoutDepositEnterAmount(enterAmountData);
            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount = result.FaxingAmount,
                ReceiveAmount = result.ReceivingAmount,
                HasFeeSetup = true
            }, JsonRequestBehavior.AllowGet);
        }
        //public void SetTransactionSummary()
        //{

        //    SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
        //    SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

        //    var sendingAmountData = _senderBankAccountDepositServices.GetSenderBankAccoutDepositEnterAmount();
        //    var bankAccountDeposit = _senderBankAccountDepositServices.GetSenderBankAccountDeposit();
        //    //Completing Transaction
        //    var loggedUserData = _senderBankAccountDepositServices.GetLoggedUserData();
        //    var paymentMethod = _senderBankAccountDepositServices.GetPaymentMethod();

        //    TransactionSummaryVM transactionSummaryVm = new TransactionSummaryVM();
        //    transactionSummaryVm.BankAccountDeposit = bankAccountDeposit;
        //    int SenderWalletId = 0;
        //    var senderWalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
        //    if (senderWalletInfo != null)
        //    {

        //        SenderWalletId = senderWalletInfo.Id;
        //    }
        //    transactionSummaryVm.SenderAndReceiverDetail = new SenderAndReceiverDetialVM()
        //    {
        //        SenderId = loggedUserData.Id,
        //        SenderCountry = loggedUserData.CountryCode,
        //        ReceiverCountry = bankAccountDeposit.CountryCode,
        //        SenderWalletId = SenderWalletId
        //        //ReceiverId = bankAccountDeposit.RecentReceiverId == null ? 0 : (int)bankAccountDeposit.RecentReceiverId

        //    };

        //    //Set Sms Fee 
        //    transactionSummaryVm.KiiPayTransferPaymentSummary = new KiiPayTransferPaymentSummary()
        //    {
        //        ReceiverName = sendingAmountData.ReceiverName,
        //        SendingCurrency = sendingAmountData.SendingCurrencyCode,
        //        SendingAmount = sendingAmountData.SendingAmount,
        //        ReceivingAmount = sendingAmountData.ReceivingAmount,
        //        TotalAmount = sendingAmountData.TotalAmount,
        //        ExchangeRate = sendingAmountData.ExchangeRate,
        //        Fee = sendingAmountData.Fee,
        //        PaymentReference = "",
        //        ReceivingCurrency = sendingAmountData.ReceivingCurrencyCode,
        //        ReceivingCurrencySymbol = sendingAmountData.ReceivingCurrencySymbol,
        //        SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
        //        SendSMS = true,
        //        SMSFee = 0,

        //    };


        //    transactionSummaryVm.PaymentMethodAndAutoPaymentDetail = new PaymentMethodViewModel()
        //    {
        //        TotalAmount = sendingAmountData.TotalAmount,
        //        SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
        //        SenderPaymentMode = paymentMethod.SenderPaymentMode,
        //        EnableAutoPayment = false,
        //    };

        //    //For DebitCreditCardDetail


        //    var debitCreditCardDetail = _senderBankAccountDepositServices.GetDebitCreditCardDetail();

        //    transactionSummaryVm.CreditORDebitCardDetials = debitCreditCardDetail;
        //    transactionSummaryVm.CreditORDebitCardDetials.FaxingAmount = sendingAmountData.TotalAmount;


        //    var moneyFexBankAccountDepositData = _senderBankAccountDepositServices.GetMoneyFexBankAccountDeposit();
        //    transactionSummaryVm.MoneyFexBankDeposit = moneyFexBankAccountDepositData;

        //    transactionSummaryVm.TransferType = TransferType.BankDeposit;
        //    if (transactionSummaryVm.SenderAndReceiverDetail.SenderCountry == transactionSummaryVm.SenderAndReceiverDetail.ReceiverCountry)
        //    {

        //        transactionSummaryVm.IsLocalPayment = true;

        //    }
        //    else
        //    {
        //        transactionSummaryVm.IsLocalPayment = false;
        //    }
        //    _senderForAllTransferServices.SetTransactionSummary(transactionSummaryVm);

        //    _senderForAllTransferServices.GenerateReceiptNoForBankDepoist(transactionSummaryVm.BankAccountDeposit.IsManualDeposit);
        //    // return transactionSummaryVm;

        //}
    }
}