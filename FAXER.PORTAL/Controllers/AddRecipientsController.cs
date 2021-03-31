using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class AddRecipientsController : Controller
    {
        SSenderBankAccountDeposit _senderBankAccountDepositServices = null;
        SSenderMobileMoneyTransfer _mobileMoneyTransferServices = null;
        RecipientServices _services = null;


        public AddRecipientsController()
        {
            _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            _mobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            _services = new RecipientServices();
        }
        // GET: AddRecipients
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RecipientsBankAccount(int Id = 0)
        {
            var country = Common.Common.GetCountries();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");

            var banks = _senderBankAccountDepositServices.getBanksList();
            ViewBag.BankNames = new SelectList(banks, "Id", "Name");

            SenderBankAccountDepositVm vm = new SenderBankAccountDepositVm();
            if (Id != 0)
            {

                vm = _services.GetBankRecipients(Id);
                ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);
                vm.IsSouthAfricaTransfer = Common.Common.IsSouthAfricanTransfer(vm.CountryCode);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult RecipientsBankAccount([Bind(Include = SenderBankAccountDepositVm.BindProperty)]SenderBankAccountDepositVm vm)
        {
            int senderId = Common.FaxerSession.LoggedUser.Id;
            var country = Common.Common.GetCountries();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");

            var banks = _senderBankAccountDepositServices.getBanksList(vm.CountryCode);
            ViewBag.BankNames = new SelectList(banks, "Id", "Name");
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);

            vm.IsSouthAfricaTransfer = Common.Common.IsSouthAfricanTransfer(vm.CountryCode);
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(vm.AccountOwnerName))
                {
                    var AccountOwnerFullname = vm.AccountOwnerName.Trim().Split(' ');
                    if (AccountOwnerFullname.Count() < 2)
                    {
                        ModelState.AddModelError("AccountOwnerName", "Enter recipient full name");
                        return View(vm);
                    }


                    if (!vm.IsBusiness)
                    {
                        //var regex = @"^[a-zA-Z]*$";
                        //Regex re = new Regex(regex);
                        //var check = re.IsMatch(vm.AccountOwnerName);
                        //if (!re.IsMatch(vm.AccountOwnerName))
                        //{
                        //    ModelState.AddModelError("AccountOwnerName", "No numbers and special characters allowed");
                        //    return View(vm);
                        //}
                    }
                }
                bool IsValidDigitCount = Common.Common.IsValidDigitCount(vm.CountryCode, vm.AccountNumber);
                if (!IsValidDigitCount)
                {

                    ModelState.AddModelError("AccountNumber", "Enter 10 digit account number");
                    return View(vm);
                }
                if (vm.CountryCode != "NG")
                {
                    if (string.IsNullOrEmpty(vm.BranchCode))
                    {

                        ModelState.AddModelError("BranchCode", "Enter code");
                        return View(vm);

                    }
                }
                if (vm.CountryCode == "ZA")
                {
                    if (string.IsNullOrEmpty(vm.MobileNumber))
                    {
                        ModelState.AddModelError("MobileNumber", "Enter Mobile Number");
                        return View(vm);
                    }
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
                        ModelState.AddModelError("ReceiverEmail", "Enter Email Address");
                        return View(vm);
                    }

                }
                if (vm.Id == 0)
                {
                    RecipientsViewModel model = new RecipientsViewModel()
                    {
                        Country = vm.CountryCode,
                        MobileNo = vm.MobileNumber,
                        AccountNo = vm.AccountNumber,
                        BankId = vm.BankId,
                        BranchCode = vm.BranchCode,
                        Reason = vm.ReasonForTransfer,
                        Service = Service.BankAccount,
                        ReceiverName = vm.AccountOwnerName,
                        SenderId = senderId,
                        MobileWalletProvider = 0,
                        IBusiness = vm.IsBusiness,
                        ReceiverEmail = vm.ReceiverEmail,
                        ReceiverPostalCode = vm.ReceiverPostalCode,
                        ReceiverStreet = vm.ReceiverStreet,
                        ReceiverCity = vm.ReceiverCity,
                    };
                    _services.AddReceipients(model);
                    return RedirectToAction("RecipientsAddedSuccessfully", "AddRecipients", new { @ReceiverName = vm.AccountOwnerName });

                }
                else
                {
                    var data = _services.RecipientsList().Where(x => x.Id == vm.Id).FirstOrDefault();
                    data.Country = vm.CountryCode;
                    data.MobileNo = vm.MobileNumber;
                    data.AccountNo = vm.AccountNumber;
                    data.BankId = vm.BankId;
                    data.BranchCode = vm.BranchCode;
                    data.Reason = vm.ReasonForTransfer;
                    data.Service = Service.BankAccount;
                    data.ReceiverName = vm.AccountOwnerName;
                    data.SenderId = senderId;
                    data.MobileWalletProvider = 0;
                    data.IBusiness = vm.IsBusiness;
                    data.PostalCode = vm.ReceiverPostalCode;
                    data.City = vm.ReceiverCity;
                    data.Email = vm.ReceiverEmail;
                    data.Street = vm.ReceiverStreet;
                    _services.UpdateReceipts(data);
                    return RedirectToAction("RecipientsUpdatedSuccessfully", "AddRecipients", new { @ReceiverName = vm.AccountOwnerName });

                }
            }

            return View(vm);
        }

        public ActionResult RecipientsMobileAccount(int Id = 0)
        {
            var country = Common.Common.GetCountries();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");
            int senderId = Common.FaxerSession.LoggedUser.Id;

            var wallets = _services.GetWallets();
            ViewBag.Wallets = new SelectList(wallets, "Id", "Name");

            SenderMobileMoneyTransferVM vm = new SenderMobileMoneyTransferVM();
            if (Id != 0)
            {
                vm = _services.GetWalletRecipients(Id);
                ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);

            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult RecipientsMobileAccount([Bind(Include = SenderMobileMoneyTransferVM.BindProperty)]SenderMobileMoneyTransferVM vm)
        {
            int senderId = Common.FaxerSession.LoggedUser.Id;
            var country = Common.Common.GetCountries();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");

            var wallets = _services.GetWallets();
            ViewBag.Wallets = new SelectList(wallets, "Id", "Name");
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(vm.ReceiverName))
                {
                    var AccountOwnerFullname = vm.ReceiverName.Trim().Split(' ');
                    if (AccountOwnerFullname.Count() < 2)
                    {
                        ModelState.AddModelError("ReceiverName", "Enter recipient full name");
                        return View(vm);
                    }
                }

                bool IsValidDigitCount = Common.Common.IsValidDigitCount(vm.CountryCode, vm.MobileNumber);
                if (!IsValidDigitCount)
                {

                    ModelState.AddModelError("MobileNumber", "Enter 10 digit moblie number");
                    return View(vm);
                }
                if (vm.Id == 0)
                {
                    RecipientsViewModel model = new RecipientsViewModel()
                    {
                        Country = vm.CountryCode,
                        MobileNo = vm.MobileNumber,
                        Service = Service.MobileWallet,
                        ReceiverName = vm.ReceiverName,
                        SenderId = senderId,
                        MobileWalletProvider = vm.WalletId,
                        BankId = 0

                    };
                    _services.AddReceipients(model);
                    return RedirectToAction("RecipientsAddedSuccessfully", "AddRecipients", new { @ReceiverName = vm.ReceiverName });
                }
                else
                {
                    var data = _services.RecipientsList().Where(x => x.Id == vm.Id).FirstOrDefault();
                    data.Country = vm.CountryCode;
                    data.MobileNo = vm.MobileNumber;
                    data.ReceiverName = vm.ReceiverName;
                    data.MobileWalletProvider = vm.WalletId;
                    _services.UpdateReceipts(data);
                    return RedirectToAction("RecipientsUpdatedSuccessfully", "AddRecipients", new { @ReceiverName = vm.ReceiverName });
                }



            }
            return View(vm);
        }
        public ActionResult RecipientsCashPickup(int Id = 0)
        {
            var country = Common.Common.GetCountries();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");

            var Identities = Common.Common.GetIdCardType();
            ViewBag.IdentityCards = new SelectList(Identities, "Id", "Name");

            SenderCashPickUpVM vm = new SenderCashPickUpVM();
            if (Id != 0)
            {
                vm = _services.GetCashPickUpRecipients(Id);
                ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult RecipientsCashPickup([Bind(Include = SenderCashPickUpVM.BindProperty)] SenderCashPickUpVM vm)
        {
            int senderId = Common.FaxerSession.LoggedUser.Id;
            var country = Common.Common.GetCountries();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);
            var Identities = Common.Common.GetIdCardType();
            ViewBag.IdentityCards = new SelectList(Identities, "Id", "Name");

            if (string.IsNullOrEmpty(vm.CountryCode))
            {
                ModelState.AddModelError("CountryCode", "Select Country");
                return View(vm);
            }
            if (string.IsNullOrEmpty(vm.FullName))
            {
                ModelState.AddModelError("FullName", "Enter Receipient Full Name");
                return View(vm);
            }
            if (!string.IsNullOrEmpty(vm.FullName))
            {
                var AccountOwnerFullname = vm.FullName.Trim().Split(' ');
                if (AccountOwnerFullname.Count() < 2)
                {
                    ModelState.AddModelError("FullName", "Enter recipient full name");
                    return View(vm);
                }
            }
            if (string.IsNullOrEmpty(vm.MobileNumber))
            {
                ModelState.AddModelError("MobileNumber", "Enter Mobile Number");
                return View(vm);
            }

            bool IsValidDigitCount = Common.Common.IsValidDigitCount(vm.CountryCode, vm.MobileNumber);
            if (!IsValidDigitCount)
            {
                ModelState.AddModelError("MobileNumber", "Enter 10 digit moblie number");
                return View(vm);
            }
            if (vm.CountryCode == "MA")
            {

                if (vm.IdenityCardId < 0)
                {
                    ModelState.AddModelError("IdenityCardId", "Select Id Card Type");
                    return View(vm);
                }
                if (string.IsNullOrEmpty(vm.IdentityCardNumber))
                {
                    ModelState.AddModelError("IdentityCardNumber", "Enter Id Number");
                    return View(vm);
                }

            }

            if (vm.Id == 0)
            {
                RecipientsViewModel model = new RecipientsViewModel()
                {
                    Country = vm.CountryCode,
                    MobileNo = vm.MobileNumber,
                    Service = Service.CashPickUP,
                    ReceiverName = vm.FullName,
                    SenderId = senderId,
                    MobileWalletProvider = 0,
                    BankId = 0,
                    IdentityCardNumber = vm.IdentityCardNumber,
                    IdentityCardId = vm.IdenityCardId,

                };
                _services.AddReceipients(model);
                return RedirectToAction("RecipientsAddedSuccessfully", "AddRecipients", new { @ReceiverName = vm.FullName });
            }
            else
            {
                var data = _services.RecipientsList().Where(x => x.Id == vm.Id).FirstOrDefault();
                data.Country = vm.CountryCode;
                data.MobileNo = vm.MobileNumber;
                data.ReceiverName = vm.FullName;
                data.IdentificationNumber = vm.IdentityCardNumber;
                data.IdentificationTypeId = vm.IdenityCardId;
                _services.UpdateReceipts(data);
                return RedirectToAction("RecipientsUpdatedSuccessfully", "AddRecipients", new { @ReceiverName = vm.FullName });
            }


        }
        public ActionResult RecipientsKiipayWallet(int Id = 0)
        {
            var country = Common.Common.GetCountries();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");
            SearchKiiPayWalletVM vm = new SearchKiiPayWalletVM();
            if (Id != 0)
            {
                vm = _services.GetKiiPayWalletRecipients(Id);
                ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);

            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult RecipientsKiipayWallet([Bind(Include = SearchKiiPayWalletVM.BindProperty)] SearchKiiPayWalletVM vm)
        {
            int senderId = Common.FaxerSession.LoggedUser.Id;
            var country = Common.Common.GetCountries();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);
            if (ModelState.IsValid)
            {

                if (string.IsNullOrEmpty(vm.ReceiverName))
                {
                    ModelState.AddModelError("ReceiverName", "Enter Receipient Full Name");
                    return View(vm);
                }
                if (!string.IsNullOrEmpty(vm.ReceiverName))
                {
                    var AccountOwnerFullname = vm.ReceiverName.Trim().Split(' ');
                    if (AccountOwnerFullname.Count() < 2)
                    {
                        ModelState.AddModelError("ReceiverName", "Enter recipient full name");
                        return View(vm);
                    }
                }
                bool IsValidDigitCount = Common.Common.IsValidDigitCount(vm.CountryCode, vm.MobileNo);
                if (!IsValidDigitCount)
                {
                    ModelState.AddModelError("MobileNo", "Enter 10 digit moblie number");
                    return View(vm);
                }

                if (vm.Id == 0)
                {
                    RecipientsViewModel model = new RecipientsViewModel()
                    {
                        Country = vm.CountryCode,
                        MobileNo = vm.MobileNo,
                        Service = Service.KiiPayWallet,
                        ReceiverName = vm.ReceiverName,
                        SenderId = senderId,
                        MobileWalletProvider = 0,
                        BankId = 0,

                    };
                    _services.AddReceipients(model);
                    return RedirectToAction("RecipientsAddedSuccessfully", "AddRecipients", new { @ReceiverName = vm.ReceiverName });
                }
                else
                {
                    var data = _services.RecipientsList().Where(x => x.Id == vm.Id).FirstOrDefault();
                    data.Country = vm.CountryCode;
                    data.MobileNo = vm.MobileNo;
                    data.ReceiverName = vm.ReceiverName;
                    _services.UpdateReceipts(data);
                    return RedirectToAction("RecipientsUpdatedSuccessfully", "AddRecipients", new { @ReceiverName = vm.ReceiverName });
                }


            }
            return View(vm);
        }
        public ActionResult RecipientsAddedSuccessfully(string ReceiverName = "")
        {
            ViewBag.ReceiverName = ReceiverName;
            return View();
        }
        public ActionResult RecipientsUpdatedSuccessfully(string ReceiverName = "")
        {
            ViewBag.ReceiverName = ReceiverName;
            return View();
        }
        public ActionResult Delete(int Id)
        {
            _services.DeleteReceipts(Id);
            return RedirectToAction("Index", "SenderTransferMoneyNow");

        }

        public JsonResult GetsenderByCountry(string Country = "")
        {
            var data = _senderBankAccountDepositServices.getBanksList(Country);
            string mobilecode = Common.Common.GetCountryPhoneCode(Country);

            return Json(new
            {
                Data = data,
                MobileCode = mobilecode
            }, JsonRequestBehavior.AllowGet);
        }

        private bool IsValidDigitCount(string CountryCode, string accountNo)
        {

            switch (CountryCode)
            {
                case "NG":
                    if (accountNo.Length < 10)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }

            return true;
        }
        public JsonResult GetWalletsByCountry(string Country = "")
        {
            var data = _services.GetWallets(Country);
            string mobilecode = Common.Common.GetCountryPhoneCode(Country);
            return Json(new
            {
                Data = data,
                MobileCode = mobilecode
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCountryPhoneCode(string CountryCode)
        {
            string CountryPhoneCode = Common.Common.GetCountryPhoneCode(CountryCode);
            return Json(new
            {

                CountryPhoneCode = CountryPhoneCode
            }, JsonRequestBehavior.AllowGet);
        }
    }
}