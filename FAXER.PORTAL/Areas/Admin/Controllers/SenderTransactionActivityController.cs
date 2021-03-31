using Antlr.Runtime.Tree;
using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;


namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class SenderTransactionActivityController : Controller
    {
        SSenderTransactionHistory _senderTransactionHistoryServices = null;
        SSenderBankAccountDeposit _senderBankAccountDeposit = null;
        public SenderTransactionActivityController()
        {
            _senderTransactionHistoryServices = new SSenderTransactionHistory();
            _senderBankAccountDeposit = new SSenderBankAccountDeposit();
        }

        // GET: Admin/SenderTransactionActivity
        public ActionResult Index(int SenderId = 0, int transactionServiceType = 0, int year = 0, int month = 0, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            ViewBag.TransferMethod = transactionServiceType;
            ViewBag.Month = month;
            ViewBag.SenderId = SenderId;
            //ViewBag.Day = Day;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            SSenderTransactionHistory _senderTransactionHistoryServices = new SSenderTransactionHistory();
            IPagedList<SenderTransactionActivityVm> result = (from c in _senderTransactionHistoryServices.GetTransactionHistories((TransactionServiceType)transactionServiceType, SenderId, year, month).TransactionHistoryList.ToList()
                                                              select new SenderTransactionActivityVm()
                                                              {
                                                                  TransactionId = c.Id,
                                                                  SenderId = SenderId,
                                                                  Amount = c.SendingCurrencySymbol + " " + c.GrossAmount,
                                                                  DateTime = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                                  Fee = c.SendingCurrencySymbol + " " + c.Fee,
                                                                  identifier = c.TransactionIdentifier,
                                                                  TransferMethod = Common.Common.GetEnumDescription(c.TransactionServiceType),
                                                                  TransferType = c.TransactionType,
                                                              }).ToPagedList(pageNumber, pageSize);
            return View(result);
        }

        [HttpGet]
        public ActionResult UpdateBankDepositProperty(int Id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {

                return RedirectToAction("Index", "staffLogin", new { @area = "staff" });
            }
            SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            var data = senderBankAccountDepositServices.List().Data.Where(x => x.Id == Id).FirstOrDefault();

            UpdateProperyViewModel vm = new UpdateProperyViewModel()
            {
                AccountNo = data.ReceiverAccountNo,
                BankCode = data.BankCode,
                ReceiptNo = data.ReceiptNo,
                ReceiverName = data.ReceiverName
            };
            return View(vm);

        }

        [HttpPost]
        public ActionResult UpdateBankDepositProperty(UpdateProperyViewModel vm)
        {

            SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            var data = senderBankAccountDepositServices.List().Data.Where(x => x.ReceiptNo == vm.ReceiptNo).FirstOrDefault();
            if (data != null)
            {

                data.ReceiverName = vm.ReceiverName;
                data.BankCode = vm.BankCode;
                data.ReceiverAccountNo = vm.AccountNo;
                data.IsComplianceNeededForTrans = true;
                data.IsComplianceApproved = false;
                data.Status = BankDepositStatus.Held;
                var TransactionLimitAmount = Common.Common.GetAppSettingValue("TransactionLimit");

                //var result =  senderBankAccountDepositServices.CreateBankTransactionToApi(data);
                //if (data.SendingAmount > decimal.Parse(TransactionLimitAmount))
                //{
                //    data.IsComplianceNeededForTrans = true;
                //    data.IsComplianceApproved = false;
                //    data.Status = BankDepositStatus.Held;
                //}
                //else {

                //    data.Status = BankDepositStatus.ReInitialise;
                //}
                senderBankAccountDepositServices.Update(data);
                return RedirectToAction("CashPickUpDetails", "SenderTransactionActivity",
                    new
                    {
                        id = data.Id,
                        transactionServiceType = TransactionServiceType.BankDeposit,
                        SenderId = data.SenderId
                    });
            }

            return View(vm);

        }


        [HttpGet]
        public ActionResult UpdatePropertyOfTransaction(int id, TransactionServiceType transactionServiceType)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "staffLogin", new { @area = "staff" });
            }
            var vm = _senderTransactionHistoryServices.GetDetailsForUpdateProperty(id, transactionServiceType);
            CommonServices commonServices = new CommonServices();
            var wallets = commonServices.GetWalletProvider();
            ViewBag.Wallets = new SelectList(wallets, "Id", "Name", vm.WalletId);
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.Country);
            GetBankViewBags(vm.BankId, vm.Country, vm.BankCode);
            return View(vm);
        }

        public void GetBankViewBags(int BankId, string Country, string BranchCode)
        {
            CommonServices commonServices = new CommonServices();
            var banks = commonServices.GetBanks(Country);
            ViewBag.Banks = new SelectList(banks, "Id", "Name", BankId);
            SSenderBankAccountDeposit _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            ViewBag.Branches = new SelectList(_senderBankAccountDepositServices.GetBranches(BankId), "Code", "Name", BranchCode);
        }

        [HttpPost]
        public ActionResult UpdatePropertyOfTransaction(UpdateProperyViewModel vm)
        {
            GetBankViewBags(vm.BankId, vm.Country, vm.BankCode);

            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.Country);
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(vm.ReceiverName))
                {
                    var AccountOwnerFullname = vm.ReceiverName.Split(' ');
                    if (AccountOwnerFullname.Count() < 2)
                    {
                        ModelState.AddModelError("ReceiverName", "Enter recipient full name");
                        return View(vm);
                    }
                }
                if (vm.TransactionServiceType == TransactionServiceType.BankDeposit)
                {
                    
                    if (vm.IsEuropeTransfer)
                    {
                        if (string.IsNullOrEmpty(vm.BankName))
                        {
                            ModelState.AddModelError("BankName", "Enter Bank Name");
                            return View(vm);
                        }
                    }

                    if (vm.IsSouthAfricaTransfer)
                    {
                        // validate
                        if (string.IsNullOrEmpty(vm.ReceiverStreet))
                        {
                            ModelState.AddModelError("ReceiverStreet", "Enter Address");
                            return View(vm);
                        }
                        if (string.IsNullOrEmpty(vm.ReceiverPostalCode))
                        {
                            ModelState.AddModelError("ReceiverPostalCode", "Enter Postcode");
                            return View(vm);
                        }
                        if (string.IsNullOrEmpty(vm.ReceiverCity))
                        {
                            ModelState.AddModelError("ReceiverCity", "Enter City");
                            return View(vm);
                        }
                        if (string.IsNullOrEmpty(vm.ReceiverEmail))
                        {
                            ModelState.AddModelError("ReceiverEmail", "Enter Email");
                            return View(vm);
                        }

                    }
                    if (!vm.IsEuropeTransfer) //have to select bank if it is not europe transfer  
                    {
                        if (vm.BankId == 0)
                        {
                            ModelState.AddModelError("BankId", "Select Bank");
                            return View(vm);
                        }
                        vm.BankCode = Common.Common.getBank(vm.BankId).Code;
                    }
                    if (vm.IsWestAfricaTransfer)
                    {
                        vm.BankCode = Common.Common.getBank(vm.BankId).Code;
                    }
                }
                if (vm.TransactionServiceType == TransactionServiceType.MobileWallet)
                {
                    if (vm.WalletId == 0)
                    {
                        ModelState.AddModelError("WalletId", "Select Wallet");
                        return View(vm);
                    }
                }
                if (vm.TransactionServiceType == TransactionServiceType.CashPickUp)
                {
                    if (vm.Country == "MA")
                    {
                        if (vm.IdenityCardId < 0)
                        {
                            ModelState.AddModelError("IdenityCardId", "Select Id card type");
                            return View(vm);
                        }
                        if (string.IsNullOrEmpty(vm.IdentityCardNumber))
                        {
                            ModelState.AddModelError("IdentityCardNumber", "Enter card number");
                            return View(vm);
                        }

                        SmsApi smsApi = new SmsApi();
                        var IsValidMobileNo = smsApi.IsValidMobileNo(ViewBag.CountryPhoneCode + "" + vm.PhoneNumber);
                        if (IsValidMobileNo == false)
                        {
                            ModelState.AddModelError("", "Enter Valid Number");
                            return View(vm);
                        }
                    }
                }

                int senderId = _senderTransactionHistoryServices.UpdatePropertyOfTransaction(vm);
                return RedirectToAction("CashPickUpDetails", "SenderTransactionActivity",
                new
                {
                    id = vm.TransactionId,
                    transactionServiceType = vm.TransactionServiceType,
                    SenderId = senderId
                });
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult RefundTransaction(SenderTransactionHistoryViewModel vm)
        {
            int senderId = 0;
            var bankdeposit = new BankAccountDeposit();
            var mobileWallet = new MobileMoneyTransfer();
            var cashPickUp = new FaxingNonCardTransaction();
            switch (vm.FilterKey)
            {
                case TransactionServiceType.BankDeposit:
                    bankdeposit = _senderTransactionHistoryServices.UpdateRefundStatusOfBank(vm);
                    senderId = bankdeposit.SenderId;
                    break;

                case TransactionServiceType.MobileWallet:
                    mobileWallet = _senderTransactionHistoryServices.UpdateRefundStatusOfMobile(vm);
                    senderId = mobileWallet.SenderId;
                    break;
                case TransactionServiceType.CashPickUp:
                    cashPickUp = _senderTransactionHistoryServices.UpdateRefundStatusOfCashPickUP(vm);
                    senderId = cashPickUp.SenderId;
                    break;
            }
            //Add log for Refund
            RefundHistory refundHistory = new RefundHistory()
            {
                ReceiptNo = vm.RefundTransactionViewModel.ReceiptNo,
                RefundedAmount = vm.RefundTransactionViewModel.RefundingAmount,
                RefundType = vm.RefundTransactionViewModel.RefundType,
                RefundedDate = DateTime.Now,
                TransactionId = vm.Id,
                RefundedBy = Common.StaffSession.LoggedStaff.StaffId,
                TransactionServiceType = vm.FilterKey,
            };
            _senderTransactionHistoryServices.AddRefundHistory(refundHistory);

            // sendEmail 
            switch (vm.FilterKey)
            {
                case TransactionServiceType.BankDeposit:
                    SSenderBankAccountDeposit sSenderBankAccountDeposit = new SSenderBankAccountDeposit();
                    sSenderBankAccountDeposit.SendEmailAndSms(bankdeposit);
                    break;
                case TransactionServiceType.MobileWallet:
                    SSenderMobileMoneyTransfer sSenderMobileMoneyTransfer = new SSenderMobileMoneyTransfer();
                    sSenderMobileMoneyTransfer.SendEmailAndSms(mobileWallet);
                    break;
                case TransactionServiceType.CashPickUp:
                    SSenderCashPickUp sSenderCashPickUp = new SSenderCashPickUp();
                    sSenderCashPickUp.SendEmailAndSms(cashPickUp);
                    break;
            }

            return RedirectToAction("CashPickUpDetails", "SenderTransactionActivity", new { @id = vm.Id, @transactionServiceType = vm.FilterKey, SenderId = senderId });
        }
        public ActionResult CashPickUpDetails(int id = 0, TransactionServiceType transactionServiceType = 0, int SenderId = 0, bool IsBusiness = false)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SenderTransactionHistoryViewModel vm = _senderTransactionHistoryServices
                .GetTransactionHistories((TransactionServiceType)transactionServiceType, SenderId, IsBusiness: IsBusiness);
            vm.TransactionHistoryList = vm.TransactionHistoryList.Where(x => x.Id == id).ToList();
            vm.FilterKey = transactionServiceType;
            ViewBag.SenderId = SenderId;
            ButtonVisibility();
            return View(vm);
        }

        public void ButtonVisibility()
        {

            bool VisibleReset = false;
            bool VisibleCancel = false;
            bool VisibleNewDetails = false;
            bool VisibleUpdateProperty = false;
            bool VisibleRefund = false;
            bool VisibleApprove = false;
            bool VisibleAmountAdjusment = false;
            var loginLevel = Common.StaffSession.LoggedStaff.SystemLoginLevel;
            switch (loginLevel)
            {
                case SystemLoginLevel.Level4:
                    VisibleReset = true;
                    VisibleCancel = true;
                    VisibleNewDetails = false;
                    VisibleUpdateProperty = true;
                    VisibleApprove = false;
                    VisibleRefund = false;
                    VisibleAmountAdjusment = false;
                    break;
                case SystemLoginLevel.Level3:
                    VisibleReset = true;
                    VisibleCancel = true;
                    VisibleNewDetails = true;
                    VisibleUpdateProperty = true;
                    VisibleApprove = true;
                    VisibleRefund = false;
                    VisibleAmountAdjusment = false;
                    break;
                case SystemLoginLevel.Level2:
                    VisibleReset = true;
                    VisibleCancel = true;
                    VisibleNewDetails = true;
                    VisibleUpdateProperty = true;
                    VisibleRefund = true;
                    VisibleApprove = true;
                    VisibleAmountAdjusment = false;
                    break;
                case SystemLoginLevel.Level1:
                    VisibleReset = true;
                    VisibleCancel = true;
                    VisibleNewDetails = true;
                    VisibleUpdateProperty = true;
                    VisibleRefund = true;
                    VisibleApprove = true;
                    VisibleAmountAdjusment = true;
                    break;
                default:
                    break;
            }
            ViewBag.VisibleReset = VisibleReset;
            ViewBag.VisibleCancel = VisibleCancel;
            ViewBag.VisibleNewDetails = VisibleNewDetails;
            ViewBag.VisibleUpdateProperty = VisibleUpdateProperty;
            ViewBag.VisibleRefund = VisibleRefund;
            ViewBag.VisibleApprove = VisibleApprove;
            ViewBag.VisibleAmountAdjusment = VisibleAmountAdjusment;
        }
        [HttpGet]
        public ActionResult CreateNewbankDeposit(int Id, TransactionServiceType TransactionServiceType)

        {
            var data = _senderTransactionHistoryServices.GetTransactiondetails(Id, TransactionServiceType);

            var recentAccountNumbers = _senderBankAccountDeposit.GetRecentAccountNumbers(data.senderId)
                                              .Where(x => x.CountryCode == data.ReceiverCountry).ToList();

            ViewBag.RecentAccountNumbers = ViewBag.RecentAccountNumbers = new SelectList(recentAccountNumbers,
                "Code", "Name", data.AccountNumber);

            var IdCardType = Common.Common.GetIdCardType();
            ViewBag.IdCardTypes = new SelectList(IdCardType, "Id", "Name");
            SSenderMobileMoneyTransfer _mobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            var walletRecentAccountNum = _mobileMoneyTransferServices.GetRecentlyPaidNumbers(data.senderId, DB.Module.Staff, data.WalletId);
            ViewBag.RecentlyWalletNo = new SelectList(walletRecentAccountNum, "Code", "Name");


            SSenderCashPickUp _cashPickUpServices = new SSenderCashPickUp();
            var recentReceivers = _senderTransactionHistoryServices.GetRecentReceivers(data.ReceiverCountry);
            ViewBag.RecentReceivers = new SelectList(recentReceivers, "Id", "ReceiverName");
            ViewBag.Wallets = new SelectList(_senderTransactionHistoryServices.GetWallets(data.ReceiverCountry), "Id", "Name", data.WalletId);
            ViewBag.RecentlyPaidNumbers = new SelectList(_senderTransactionHistoryServices.GetRecentPaidReceivers(data.senderId, DB.Module.Staff, data.WalletId, data.ReceiverCountry),
               "Code", "Name", data.MobileNo);
            SetViewBagForBanks(data.ReceiverCountry);
            ViewBag.Branches = new SelectList(_senderBankAccountDeposit.GetBranches(data.BankId), "Code", "Name", data.BankCode);

            SenderBankAccountDepositVm vm = new SenderBankAccountDepositVm()
            {
                AccountNumber = data.AccountNumber,
                AccountOwnerName = data.ReceiverName,
                BankId = data.BankId,
                BranchCode = data.BankCode,
                CountryCode = data.ReceiverCountry,
                CountryPhoneCode = Common.Common.GetCountryPhoneCode(data.ReceiverCountry),
                RecentAccountNumber = data.AccountNumber,
                MobileNumber = data.MobileNo,
                BankName = data.BankName,
                IsBusiness = data.IsBusiness,
                IsEuropeTransfer = data.IsEuropeTransfer,
                IsManualDeposit = data.IsManualBankDeposit,
                walletId = data.WalletId,
                IsSouthAfricaTransfer = Common.Common.IsSouthAfricanTransfer(data.ReceiverCountry),
                IsWestAfricaTransfer = Common.Common.IsWestAfricanTransfer(data.ReceiverCountry),
                ReceiverPostalCode = data.ReceiverPostalCode,
                ReceiverStreet = data.ReceiverStreet,
                ReceiverEmail = data.ReceiverEmail,
                ReceiverCity = data.ReceiverCity,
                IdenityCardId = data.RecipientIdenityCardId,
                IdentityCardNumber = data.RecipientIdentityCardNumber,
                SenderId = data.senderId,
                SendingCurrency = data.SendingCurrency,
                ReceivingCurrency = data.ReceivingCurrrency

            };
            Common.AdminSession.TransactionId = data.Id;
            Common.AdminSession.SenderBankAccountDeposit = vm;
            Common.AdminSession.TransactionServiceType = TransactionServiceType;
            _senderTransactionHistoryServices.SetSenderTransactionHistoryList(data);
            ViewBag.TransactionServiceType = TransactionServiceType;
            return View(vm);
        }
        [HttpPost]
        public ActionResult CreateNewbankDeposit([Bind(Include = SenderBankAccountDepositVm.BindProperty)] SenderBankAccountDepositVm model)
        {

            var data = _senderTransactionHistoryServices.GetSenderTransactionHistoryList();

            ViewBag.TransactionServiceType = data.TransactionServiceType;
            var recentAccountNumbers = _senderBankAccountDeposit.GetRecentAccountNumbers(data.senderId)
                                              .Where(x => x.CountryCode == data.ReceiverCountry).ToList();
            ViewBag.RecentAccountNumbers = ViewBag.RecentAccountNumbers = new SelectList(recentAccountNumbers,
                "Code", "Name", model.AccountNumber);
            SetViewBagForBanks(data.ReceiverCountry);
            var IdCardType = Common.Common.GetIdCardType();
            ViewBag.IdCardTypes = new SelectList(IdCardType, "Id", "Name");
            ViewBag.Wallets = new SelectList(_senderTransactionHistoryServices.GetWallets(data.ReceiverCountry), "Id", "Name", data.WalletId);
            ViewBag.Branches = new SelectList(_senderBankAccountDeposit.GetBranches(model.BankId), "Code", "Name", model.BranchCode);

            Common.AdminSession.SenderBankAccountDeposit = model;

            switch (data.TransactionServiceType)
            {
                case TransactionServiceType.BankDeposit:
                    model.IsSouthAfricaTransfer = Common.Common.IsSouthAfricanTransfer(data.ReceiverCountry);

                    if (string.IsNullOrEmpty(model.AccountOwnerName))
                    {
                        ModelState.AddModelError("AccountOwnerName", "Enter Account Owner Name");
                        return View(model);
                    }
                    if (string.IsNullOrEmpty(model.AccountNumber))
                    {
                        ModelState.AddModelError("AccountNumber", "Enter Account Number");
                        return View(model);
                    }
                    if (!model.IsEuropeTransfer) //have to select bank if it is not europe transfer  
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
                    if (model.CountryCode == "NG")
                    {
                        model.BranchCode = Common.Common.getBank(model.BankId).Code;
                    }
                    if (model.IsSouthAfricaTransfer)
                    {
                        if (string.IsNullOrEmpty(model.MobileNumber))
                        {
                            ModelState.AddModelError("MobileNumber", "Enter Mobile No");
                            return View(model);
                        }
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
                        if (string.IsNullOrEmpty(model.ReceiverEmail))
                        {
                            ModelState.AddModelError("ReceiverEmail", "Enter Email Address");
                            return View(model);
                        }
                    }
                    break;
                case TransactionServiceType.CashPickUp:
                    if (string.IsNullOrEmpty(model.AccountOwnerName))
                    {
                        ModelState.AddModelError("AccountOwnerName", "Enter Account Owner Name");
                        return View(model);
                    }
                    if (string.IsNullOrEmpty(model.MobileNumber))
                    {
                        ModelState.AddModelError("MobileNumber", "Enter Mobile No");
                        return View(model);
                    }
                    if (model.CountryCode == "MA")
                    {
                        if (model.IdenityCardId <= 0)
                        {
                            ModelState.AddModelError("IdenityCardId", "Select Id card type");
                            return View(model);
                        }
                        if (string.IsNullOrEmpty(model.IdentityCardNumber))
                        {
                            ModelState.AddModelError("IdentityCardNumber", "Enter card number");
                            return View(model);
                        }
                    }
                    break;
                case TransactionServiceType.MobileWallet:

                    if (model.walletId <= 0)
                    {
                        ModelState.AddModelError("walletId", "Select Wallet Provider");
                        return View(model);
                    }
                    if (string.IsNullOrEmpty(model.AccountOwnerName))
                    {
                        ModelState.AddModelError("AccountOwnerName", "Enter Account Owner Name");
                        return View(model);
                    }
                    if (string.IsNullOrEmpty(model.MobileNumber))
                    {
                        ModelState.AddModelError("MobileNumber", "Enter Mobile No");
                        return View(model);
                    }

                    break;
            }

            CashPickUpEnterAmountViewModel cashPickUpEnterAmountViewModel = new CashPickUpEnterAmountViewModel()
            {
                SendingCountry = data.FaxerCountry,
                ReceivingCountry = data.ReceiverCountry,
                ReceivingAmount = data.ReceivingAmount,
                SendingAmount = data.GrossAmount,
                ReceivingCurrency = data.ReceivingCurrrency,
                SendingCurrency = data.SendingCurrency,
                TotalAmount = data.TotalAmount,
                Fee = data.Fee,
                TheyReceive = data.ReceivingAmount,
                ReceivingCurrencySymbol = data.ReceivingCurrencySymbol,
                SendingCurrencySymbol = data.ReceivingCurrencySymbol,
                ExchangeRate = data.ExchangeRate
            };
            SCashPickUpTransferService SCashPickUpTransferService = new SCashPickUpTransferService();
            SCashPickUpTransferService.SetStaffCashPickUpEnterAmount(cashPickUpEnterAmountViewModel);

            return RedirectToAction("EnterAmmount");

        }
        public ActionResult EnterAmmount()
        {

            SCashPickUpTransferService SCashPickUpTransferService = new SCashPickUpTransferService();
            CashPickUpEnterAmountViewModel vm = new CashPickUpEnterAmountViewModel();

            var paymentInfo = SCashPickUpTransferService.GetStaffCashPickUpEnterAmount();

            vm.SendingCountry = paymentInfo.SendingCountry;
            vm.ReceivingCountry = paymentInfo.ReceivingCountry;
            vm.SendingCurrencyCode = paymentInfo.SendingCurrency;
            vm.ReceivingCurrencyCode = paymentInfo.ReceivingCurrency;
            vm.SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol;
            vm.ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol;
            vm.Fee = paymentInfo.Fee;
            vm.ReceivingAmount = paymentInfo.ReceivingAmount;
            vm.SendingAmount = paymentInfo.SendingAmount;
            vm.Fee = paymentInfo.Fee;
            vm.TotalAmount = paymentInfo.TotalAmount;
            vm.InculdeFee = true;
            var transactionDetails = _senderTransactionHistoryServices.GetSenderTransactionHistoryList();

            ViewBag.SenderName = transactionDetails.SenderName;
            ViewBag.SenderAccountNo = transactionDetails.FaxerAccountNo;
            ViewBag.SenderCountry = Common.Common.GetCountryName(paymentInfo.SendingCountry);
            ViewBag.ReceiverName = transactionDetails.ReceiverName;
            ViewBag.TransferMethod = (int)transactionDetails.TransactionServiceType;
            ViewBag.SenderId = (int)transactionDetails.senderId;
            return View(vm);

        }
        [HttpPost]

        public ActionResult EnterAmmount([Bind(Include = CashPickUpEnterAmountViewModel.BindProperty)] CashPickUpEnterAmountViewModel model)
        {

            SCashPickUpTransferService SCashPickUpTransferService = new SCashPickUpTransferService();
            if (model.InculdeFee == false)
            {
                model.Fee = 0;
            }
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();
            model.SendingCurrencyCode = paymentInfo.SendingCurrency;
            model.SendingCurrency = paymentInfo.SendingCurrency;
            model.ReceivingCurrencyCode = paymentInfo.ReceivingCurrency;
            model.ReceivingCurrency = paymentInfo.ReceivingCurrency;
            SCashPickUpTransferService.SetStaffCashPickUpEnterAmount(model);

            return RedirectToAction("PaymentSummary");

        }
        [HttpGet]
        public ActionResult PaymentSummary()
        {

            var transactionDetails = _senderTransactionHistoryServices.GetSenderTransactionHistoryList();


            SCashPickUpTransferService SCashPickUpTransferService = new SCashPickUpTransferService();
            var paymentInfo = SCashPickUpTransferService.GetStaffCashPickUpEnterAmount();

            SenderTransferSummaryVm vm = new SenderTransferSummaryVm()
            {
                Amount = paymentInfo.SendingAmount,
                Fee = paymentInfo.Fee,
                PaidAmount = paymentInfo.TotalAmount,
                PaymentReference = transactionDetails.PaymentReference,
                ReceivedAmount = paymentInfo.ReceivingAmount,
                ReceiverName = transactionDetails.ReceiverName,
                ReceivingCurrencyCode = transactionDetails.ReceivingCurrrency,
                ReceivingCurrencySymbol = transactionDetails.ReceivingCurrencySymbol,
                SendingCurrencyCode = transactionDetails.SendingCurrency,
                SendingCurrencySymbol = transactionDetails.SendingCurrencySymbol,

            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult PaymentSuccessfully()
        {
            SenderAddMoneySuccessVM vm = _senderTransactionHistoryServices.CompleteDuplicateTransaction();



            Common.AdminSession.TransactionId = 0;
            Common.AdminSession.SenderBankAccountDeposit = null;
            Common.AdminSession.TransactionServiceType = TransactionServiceType.All;

            return View(vm);
        }
        private void SetViewBagForBanks(string Country)
        {
            var banks = _senderBankAccountDeposit.getBanksList(Country);
            ViewBag.BankNames = new SelectList(banks, "Id", "Name");
        }
        public ActionResult HoldUnhold(int id, int SenderId)
        {
            SSenderBankAccountDeposit _bankdeposit = new SSenderBankAccountDeposit();
            if (id != 0)
            {
                var data = _bankdeposit.List().Data.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {

                    if (data.Status == BankDepositStatus.Held)
                    {
                        data.Status = BankDepositStatus.UnHold;
                    }
                    else if (data.Status == BankDepositStatus.UnHold)
                    {
                        data.Status = BankDepositStatus.Held;
                    }
                    else if (data.Status == BankDepositStatus.Incomplete)
                    {
                        data.Status = BankDepositStatus.Held;
                    }
                    else
                    {
                        data.Status = data.Status;
                    }

                }

                _bankdeposit.Update(data);

            }
            return RedirectToAction("CashPickUpDetails", "SenderTransactionActivity", new { @id = id, @transactionServiceType = TransactionServiceType.BankDeposit, @SenderId = SenderId });

        }

        public ActionResult Cancel(int id, int SenderId)
        {
            SSenderBankAccountDeposit _bankdeposit = new SSenderBankAccountDeposit();
            if (id != 0)
            {
                var data = _bankdeposit.List().Data.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {
                    data.Status = BankDepositStatus.Cancel;
                }

                _bankdeposit.Update(data);

            }
            return RedirectToAction("CashPickUpDetails", "SenderTransactionActivity", new { @id = id, @transactionServiceType = TransactionServiceType.BankDeposit, @SenderId = SenderId });

        }
        public ActionResult RetryBankTransaction(int transactionId, int SenderId = 0, int AgentId = 0)
        {
            SSenderForAllTransfer _services = new SSenderForAllTransfer();
            if (transactionId != 0)
            {
                _services.RetryBankTransaction(transactionId);
            }
            if (AgentId != 0)
            {
                return RedirectToAction("Index", "TranscationDetails", new { @id = transactionId, transactionService = TransactionType.BankAccountDeposit, AgentId = AgentId });

            }
            return RedirectToAction("CashPickUpDetails", "SenderTransactionActivity", new { @id = transactionId, @transactionServiceType = TransactionServiceType.BankDeposit, @SenderId = SenderId });

        }
        public JsonResult GetTransactionDetailsForRefund(int transactionId, TransactionServiceType transactionServiceType)
        {
            SSenderForAllTransfer _services = new SSenderForAllTransfer();
            var result = new RefundTransactionViewModel();
            if (transactionId != 0)
            {
                result = _services.GetTransactionDetailsForRefund(transactionId, transactionServiceType);
            }
            return Json(new
            {
                Data = result,
            }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult ResetTransaction(int TransactionId, TransactionServiceType transactionServiceType)
        {
            SSenderForAllTransfer _services = new SSenderForAllTransfer();
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            string StaffName = Common.StaffSession.LoggedStaff.FirstName + " " + Common.StaffSession.LoggedStaff.MiddleName + " " +
                 Common.StaffSession.LoggedStaff.LastName;

            var result = _services.ResetTransaction(TransactionId, StaffId, StaffName, transactionServiceType);
            return Json(new
            {
                Data = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RecheckTransaction(int transactionId, TransactionServiceType transactionServiceType)
        {
            SSenderForAllTransfer _services = new SSenderForAllTransfer();
            var result = new TransactionStatusApiVm();
            if (transactionId != 0)
            {
                result = _services.RecheckTransaction(transactionId, transactionServiceType);
            }
            return Json(new
            {
                Data = result,
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ApproveBankAccountDepositTransaction(int transactionId)
        {
            SSenderForAllTransfer _services = new SSenderForAllTransfer();
            var result = _services.ApproveTransaction(transactionId, TransactionServiceType.BankDeposit);
            return Json(new
            {
                Data = result,
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ApproveTransansation(int transactionId, TransactionServiceType transactionServiceType)
        {
            SSenderForAllTransfer _services = new SSenderForAllTransfer();
            var result = _services.ApproveTransaction(transactionId, transactionServiceType);
            return Json(new
            {
                Data = result,
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CancelTransaction(int transactionId, TransactionServiceType transactionServiceType)
        {
            SSenderForAllTransfer _services = new SSenderForAllTransfer();
            var result = _services.CancelTransaction(transactionId, transactionServiceType);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BanReceiverAndSender(int senderId, int recipientId)
        {
            _senderTransactionHistoryServices.BanReceiverAndSender(senderId, recipientId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult CancelBankDepositTransaction(int transactionId)
        //{
        //    SSenderForAllTransfer _services = new SSenderForAllTransfer();
        //    var result = _services.CancelBankDepositTransaction(transactionId);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult CancelCashPickUpTransaction(int transactionId)
        //{
        //    SSenderBankAccountDeposit _senderBankAccountDeposit = new SSenderBankAccountDeposit();
        //    SSenderForAllTransfer _services = new SSenderForAllTransfer();
        //    var result = _services.CancelCashPickUPTransaction(transactionId);

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult CancelMobileWalletTransaction(int transactionId)
        //{
        //    SSenderBankAccountDeposit _senderBankAccountDeposit = new SSenderBankAccountDeposit();
        //    SSenderForAllTransfer _services = new SSenderForAllTransfer();
        //    var result = _services.CancelMobileWalletTransaction(transactionId);

        //    return Json(result, JsonRequestBehavior.AllowGet);

        //}

        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount,
            bool IsReceivingAmount, string receiverCountry, string sendingCountry, bool IncludeFee = true)
        {
            SCashPickUpTransferService _cashPickUp = new SCashPickUpTransferService();
            var paymentSummary = _cashPickUp.GetStaffCashPickUpEnterAmount();

            var senderInfo = _cashPickUp.GetStaffCashPickupInformationViewModel();

            string ReceivingCountry = receiverCountry;
            var SendingCurrencySymbol = Common.Common.GetCurrencySymbol(sendingCountry);
            var SendingCurrencyCode = Common.Common.GetCurrencyCode(sendingCountry);
            var RecevingCurrencySymbol = Common.Common.GetCurrencySymbol(receiverCountry);
            var ReceivingCurrencyCode = Common.Common.GetCurrencyCode(receiverCountry);

            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }

            var feeInfo = SEstimateFee.GetTransferFee(sendingCountry, ReceivingCountry, TransactionTransferMethod.BankDeposit,
                SendingAmount, TransactionTransferType.Admin);

            if (feeInfo == null)
            {

                return Json(new
                {
                    Fee = 0,
                    TotalAmount = 0,
                    ReceivingAmount = 0,
                    SendingAmount = 0,
                    AgentCommission = 0,
                    SendingCurrencySymbol = SendingCurrencySymbol,
                    RecevingCurrencySymbol = RecevingCurrencySymbol,
                    SendingCurrencyCode = SendingCurrencyCode,
                    ReceivingCurrency = ReceivingCurrencyCode,

                }, JsonRequestBehavior.AllowGet);
            }
            var result = new EstimateFaxingFeeSummary();
            int AgentId = 0;
            result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                SExchangeRate.GetExchangeRateValue(sendingCountry, ReceivingCountry,
                TransactionTransferMethod.BankDeposit, AgentId, TransactionTransferType.Online),
                feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);



            //var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(sendingCountry, ReceivingCountry, result.FaxingAmount
            //    , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, IsReceivingAmount, TransactionTransferMethod.BankDeposit, AgentId, false);

            //if (introductoryRateResult != null)
            //{

            //    result = introductoryRateResult;
            //}


            paymentSummary.Fee = result.FaxingFee;
            paymentSummary.TotalAmount = result.TotalAmount;
            if (IncludeFee == false)
            {
                paymentSummary.Fee = 0;
                paymentSummary.TotalAmount = result.TotalAmount - result.FaxingFee;
            }
            paymentSummary.SendingAmount = result.FaxingAmount;
            paymentSummary.ReceivingAmount = result.ReceivingAmount;

            paymentSummary.ExchangeRate = result.ExchangeRate;

            _cashPickUp.SetStaffCashPickUpEnterAmount(paymentSummary);
            return Json(new
            {
                Fee = paymentSummary.Fee,
                TotalAmount = paymentSummary.TotalAmount,
                ReceivingAmount = paymentSummary.ReceivingAmount,
                SendingAmount = paymentSummary.SendingAmount,
                ExchangeRate = paymentSummary.ExchangeRate,
                SendingCurrencySymbol = SendingCurrencySymbol,
                RecevingCurrencySymbol = RecevingCurrencySymbol,
                SendingCurrencyCode = SendingCurrencyCode,
                ReceivingCurrency = ReceivingCurrencyCode,

            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetRecentMobileWalletAccountNo(int walletId = 0, int senderId = 0)
        {
            SSenderMobileMoneyTransfer _mobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            var data = _mobileMoneyTransferServices.GetRecentlyPaidNumbers(senderId, DB.Module.Faxer, walletId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRecentBankDetails(string accountNo = "")
        {
            SSenderBankAccountDeposit sSenderBankAccountDeposit = new SSenderBankAccountDeposit();
            var data = sSenderBankAccountDeposit.GetAccountInformationFromAccountNumberAndId(accountNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }

}