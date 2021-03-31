using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using Twilio.Rest.Trunking.V1;

namespace FAXER.PORTAL.Areas.Admin.Controllers.Staff_Transactions
{
    public class StaffTransactionUpdateController : Controller
    {
        // GET: Admin/StaffTransactionUpdate
        SCashPickUpTransferService _cashPickUp = null;
        AdminForAlTransferServices _adminAllTransferServices = null;
        StaffTransactionHistoryServices _Services = null;
        SSenderBankAccountDeposit _senderBankAccountDepositServices = null;
        CommonServices _commonServices = new CommonServices();
        public StaffTransactionUpdateController()
        {
            _Services = new StaffTransactionHistoryServices();
            _adminAllTransferServices = new AdminForAlTransferServices();
            _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            _cashPickUp = new SCashPickUpTransferService();
        }
        public ActionResult Index(int Services = 0, int Month = 0, int Year = 0, int Day = 0, string Country = "", string Receiver = "", string Identifier = "", int? page = null)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountry(Country);

            ViewBag.Years = new SelectList(Enumerable.Range(2018, 10));
            ViewBag.Receiver = Receiver;
            ViewBag.Identifier = Identifier;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<DailyTransactionStatementListVm> vm = _Services.GetStaffTransactionStatementList().ToPagedList(pageNumber, pageSize);

            if (Services != 0)
            {
                ViewBag.TransferMethod = Services;
                vm = vm.Where(x => x.TransactionType == (TransactionType)Services).ToPagedList(pageNumber, pageSize);
            }
            if (Month != 0)
            {
                ViewBag.Month = Month;
                vm = vm.Where(x => x.DateAndTime.Month == Month).ToPagedList(pageNumber, pageSize);
            }
            if (Day != 0)
            {
                ViewBag.Day = Day;
                vm = vm.Where(x => x.DateAndTime.Day == Day).ToPagedList(pageNumber, pageSize);
            }
            if (Year != 0)
            {
                ViewBag.Year = Year;
                vm = vm.Where(x => x.DateAndTime.Year == Year).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(Country))
            {
                ViewBag.Country = _commonServices.getCurrencyCodeFromCountry(Country);
                vm = vm.Where(x => x.ReceivingCountry == _commonServices.getCurrencyCodeFromCountry(Country)).ToPagedList(pageNumber, pageSize);
            }


            if (!string.IsNullOrEmpty(Receiver))
            {
                Receiver = Receiver.Trim();
                vm = vm.Where(x => x.ReceiverName.ToLower().Contains(Receiver.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(Identifier))
            {
                Identifier = Identifier.Trim();
                vm = vm.Where(x => x.TransactionIdentifier.ToLower().Contains(Identifier.ToLower())).ToPagedList(pageNumber, pageSize);
            }

            return View(vm);
        }
        public ActionResult UpdateTransaction(int id = 0, TransactionType transactionService = TransactionType.All)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            switch (transactionService)
            {
                case TransactionType.KiiPayWallet:

                    return RedirectToAction("UpdateKiiPayWallet", new { id = id });
                    break;
                case TransactionType.CashPickUp:
                    return RedirectToAction("UpdateCashPickUp", new { id = id });
                    break;
                case TransactionType.BankAccountDeposit:
                    return RedirectToAction("UpdateBankAccountDeposit", new { id = id });
                    break;
                case TransactionType.OtherWalletTransfer:
                    return RedirectToAction("UpdateOtherWalletTransfer", new { id = id });
                    break;
                default:
                    return View();
                    break;
            }

        }

        public ActionResult UpdateOtherWalletTransfer(int id = 0, string Country = "")
        {
            SetViewBagForCountry(Country);
            SetViewBagForMobileWalletProvider(Country);
            ViewBag.PhoneCode = Common.Common.GetCountryPhoneCode(Country);
            ReceiverDetailsInformationViewModel vm = new ReceiverDetailsInformationViewModel();
            vm.Id = id;
            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateOtherWalletTransfer([Bind(Include = ReceiverDetailsInformationViewModel.BindProperty)] ReceiverDetailsInformationViewModel model)
        {
            SetViewBagForCountry(model.Country);
            SetViewBagForMobileWalletProvider(model.Country);
            ViewBag.PhoneCode = Common.Common.GetCountryPhoneCode(model.Country);
            if (string.IsNullOrEmpty(model.Country))
            {
                ModelState.AddModelError("Country", "Please Select Country");

            }
            if (string.IsNullOrEmpty(model.MobileNumber))
            {
                ModelState.AddModelError("MobileNo", "Please Enter Mobile No");

            }
            if (string.IsNullOrEmpty(model.ReceiverName))
            {
                ModelState.AddModelError("ReceiverName", "Please Enter ReceiverName");

            }
            if (ModelState.IsValid)
            {
                var otherWallet = _adminAllTransferServices.UpdateOtherMobile(model);
                return RedirectToAction("StaffTransactionsUpdateSuccess", new { senderId = otherWallet.SenderId });
            }


            return View(model);
        }
        private void SetViewBagForMobileWalletProvider(string Country)
        {
            SSenderMobileMoneyTransfer _mobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            var wallets = _mobileMoneyTransferServices.GetWallets().Where(x => x.CountryCode == Country).ToList();
            ViewBag.MobileWalletProviders = new SelectList(wallets, "Id", "Name", Country);
        }

        [HttpGet]
        public ActionResult UpdateKiiPayWallet(int id = 0, string Country = "")
        {
            SetViewBagForCountry(Country);
            if (Country != "")
            {
                ViewBag.MobileCode = _commonServices.getPhoneCodeFromCountry(Country);
            }
            TransactionUpdateVm vm = new TransactionUpdateVm();
            vm.Id = id;

            vm.Country = Country;
            return View(vm);
        }

        [HttpPost]
        public ActionResult UpdateKiiPayWallet(TransactionUpdateVm model)
        {
            SetViewBagForCountry(model.Country);
            if (string.IsNullOrEmpty(model.Country))
            {
                ModelState.AddModelError("Country", "Please Select Country");

            }
            if (string.IsNullOrEmpty(model.MobileNo))
            {
                ModelState.AddModelError("MobileNo", "Please Enter Mobile No");

            }
            if (string.IsNullOrEmpty(model.ReceiverName))
            {
                ModelState.AddModelError("ReceiverName", "Please Enter ReceiverName");

            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("StaffTransactionsUpdateSuccess", new { senderId = 0 });
            }

            return View(model);
        }


        public ActionResult UpdateCashPickUp(int Id = 0, string Country = "")
        {
            SetViewBagForCountry(Country);
            CashPickUpReceiverDetailsInformationViewModel vm = new CashPickUpReceiverDetailsInformationViewModel();
            vm.MobileCode = _commonServices.getPhoneCodeFromCountry(Country);
            vm.Id = Id;
            return View(vm);

        }
        [HttpPost]
        public ActionResult UpdateCashPickUp([Bind(Include = CashPickUpReceiverDetailsInformationViewModel.BindProperty)] CashPickUpReceiverDetailsInformationViewModel model)
        {
            SetViewBagForCountry(model.Country);
            if (string.IsNullOrEmpty(model.MobileNo))
            {
                ModelState.AddModelError("MobileNo", "Please Enter Mobile No");

            }
            if (string.IsNullOrEmpty(model.ReceiverFullName))
            {
                ModelState.AddModelError("ReceiverName", "Please Enter OwnerName");

            }
            if (string.IsNullOrEmpty(model.Country))
            {
                ModelState.AddModelError("Country", "Please select Country");

            }
            if (ModelState.IsValid)
            {

                var CashPickUp = _adminAllTransferServices.UpdateCashPickUp(model);
                return RedirectToAction("StaffTransactionsUpdateSuccess", new { senderId = CashPickUp.SenderId });
            }

            return View(model);

        }
        [HttpGet]
        public ActionResult UpdateBankAccountDeposit(int id = 0)
        {
            var data = _adminAllTransferServices.getBankAccountDeposit(id);

            SetViewBagForBanks(data.ReceivingCountry);
            SenderBankAccountDepositVm vm = new SenderBankAccountDepositVm();
            vm.Id = data.Id;
            vm.CountryCode = data.ReceivingCountry;
            vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(data.ReceivingCountry);
            ViewBag.Branches = new SelectList(_senderBankAccountDepositServices.GetBranches(vm.BankId), "Code", "Name", vm.BranchCode);
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(data.ReceivingCountry);
            return View(vm);

        }
        [HttpPost]
        public ActionResult UpdateBankAccountDeposit([Bind(Include = SenderBankAccountDepositVm.BindProperty)] SenderBankAccountDepositVm model)
        {
            SetViewBagForBanks(model.CountryCode);
            SenderBankAccountDepositVm vm = new SenderBankAccountDepositVm();
            ViewBag.Branches = new SelectList(_senderBankAccountDepositServices.GetBranches(vm.BankId), "Code", "Name", vm.BranchCode);
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(model.CountryCode);

            if (string.IsNullOrEmpty(model.MobileNumber))
            {
                ModelState.AddModelError("MobileNo", "Please Enter Mobile No");

            }
            if (string.IsNullOrEmpty(model.AccountOwnerName))
            {
                ModelState.AddModelError("ReceiverName", "Please Enter OwnerName");

            }

            if (ModelState.IsValid)
            {

                var bankdeposit = _adminAllTransferServices.UpdateBankAccountDeposit(model);
                return RedirectToAction("StaffTransactionsUpdateSuccess", new { senderId = bankdeposit.SenderId });
            }

            return View(vm);

        }

        public ActionResult StaffTransactionsUpdateSuccess(int SenderId = 0)
        {
            ViewBag.SenderName = _commonServices.GetSenderName(SenderId);
            ViewBag.SenderCountry = _commonServices.getCountryNameFromCode(_commonServices.GetSenderCountry(SenderId));
            ViewBag.SenderAccountNo = _commonServices.GetSenderAccountNoBySenderId(SenderId);
            return View();
        }
        private void SetViewBagForCountry(string Country)
        {
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name", Country);
        }

        public void SetViewBagForBanks(string Country = "")
        {
            var banks = _commonServices.GetBanks(Country);
            ViewBag.BankNames = new SelectList(banks, "Id", "Name");
        }

    }
}