using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.MyRegisteredKiiPayWallets
{
    public class SenderWithdrawMoneyFromWalletController : Controller
    {
        private SUserBankAccount _userBankAccountServices = null;

        public SenderWithdrawMoneyFromWalletController()
        {
            _userBankAccountServices = new SUserBankAccount();
        }

        // GET: SenderWithdrawMoneyFromWallet
        [HttpGet]
        public ActionResult SenderWithdrawMoneyFromWallet()
        {
            var LoggedUser = Common.FaxerSession.LoggedUser;
            SenderWithdrawMoneyFromWalletViewModel Vm = new SenderWithdrawMoneyFromWalletViewModel();

            var senderKiiPay = _userBankAccountServices.SenderKiiPayList().Data.Where(x => x.SenderId == LoggedUser.Id || x.KiiPayAccountIsOF == DB.KiiPayAccountIsOF.Sender).FirstOrDefault();

            Vm.Banks = (from c in _userBankAccountServices.List().Data.Where(x => x.UserId == LoggedUser.Id && x.UserType == DB.Module.Faxer).ToList()
                        select new SavedBankDetailsViewModel
                        {
                            AccountNumber = c.AccountNumber,
                            BankName = c.BankName,
                            BankId = c.Id,
                            FormattedAccNo = FormatAccNo(c.AccountNumber)

                        }).ToList();
            Vm.AvailableBalance = senderKiiPay.KiiPayPersonalWalletInformation.CurrentBalance;
            Vm.AvailableCurrency = Common.Common.GetCurrencySymbol(LoggedUser.CountryCode);
            Vm.CardHolderName = senderKiiPay.KiiPayPersonalWalletInformation.FirstName + " " + senderKiiPay.KiiPayPersonalWalletInformation.MiddleName + " " + senderKiiPay.KiiPayPersonalWalletInformation.LastName;
            return View(Vm);

        }


        [HttpPost]
        public ActionResult SenderWithdrawMoneyFromWallet([Bind(Include = SenderWithdrawMoneyFromWalletViewModel.BindProperty)]SenderWithdrawMoneyFromWalletViewModel model)
        {
            var LoggedUser = Common.FaxerSession.LoggedUser;

            model.Banks = (from c in _userBankAccountServices.List().Data.Where(x => x.UserId == LoggedUser.Id && x.UserType == DB.Module.Faxer).ToList()
                           select new SavedBankDetailsViewModel
                           {
                               AccountNumber = c.AccountNumber,
                               BankName = c.BankName,
                               BankId = c.Id,
                               FormattedAccNo = FormatAccNo(c.AccountNumber),
                               IsChecked = true,

                        }).ToList();
     
            if (ModelState.IsValid && model.Banks.Count > 0)
            {
                foreach (var item in model.Banks)
                {
                    if(item.IsChecked==false)
                    {
                        ModelState.AddModelError("IsChecked", "Please select one Bank");
                        return View(model);
                    }
                    else
                    {
                        _userBankAccountServices.SetBankId(model.Banks.Where(x => x.IsChecked == true).FirstOrDefault().BankId);
                        return RedirectToAction("SenderWithdrawMoneyFromWalletEnterAmount",model);
                    }
                }
                
                
            }

            return View(model);
        }
        private string FormatAccNo(string accountno)
        {
            // 12356456789
            string formattedAccNO = accountno;
            try
            {
                formattedAccNO = accountno.Substring(accountno.Length - 5, 5);
            }
            catch (Exception)
            {

            }

            return "XXX-" + formattedAccNO;

        }
        [HttpGet]
        public ActionResult SenderWithdrawMoneyFromWalletEnterAmount([Bind(Include = SenderWithdrawMoneyFromWalletViewModel.BindProperty)]SenderWithdrawMoneyFromWalletViewModel model)
        {
            var LoggedUser = Common.FaxerSession.LoggedUser;
            var Country = Common.FaxerSession.FaxerCountry;
            SenderWithdrawMoneyFromWalletEnterAmountViewModel Vm = new SenderWithdrawMoneyFromWalletEnterAmountViewModel();
            Vm.AvailableCurrency = model.AvailableCurrency;
            Vm.AvailableBalance = model.AvailableBalance;
            Vm.SendingCurrency = Common.Common.GetCountryCurrency(LoggedUser.CountryCode);
            Vm.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(LoggedUser.CountryCode);
            return View(Vm);
        }
        [HttpPost]
        public ActionResult SenderWithdrawMoneyFromWalletEnterAmount([Bind(Include = SenderWithdrawMoneyFromWalletEnterAmountViewModel.BindProperty)]SenderWithdrawMoneyFromWalletEnterAmountViewModel Vm)
        {
            _userBankAccountServices.SetAmount(Vm.Amount);

            if (ModelState.IsValid)
            {
                return RedirectToAction("SenderWithdrawMoneyFromWalletSummary");
            }
            return View(Vm);
        }
        [HttpGet]
        public ActionResult SenderWithdrawMoneyFromWalletSummary()
        {
            var LoggedUser = Common.FaxerSession.LoggedUser;
            var Country = Common.FaxerSession.FaxerCountry;
            int BankId = Common.FaxerSession.BankId;
            var Amount = Common.FaxerSession.AmountToBeTransferred;
            var SendToAccount = _userBankAccountServices.GetAccountNumber(BankId);
            SenderWithdrawMoneyFromWalletSummaryViewModel Vm = new SenderWithdrawMoneyFromWalletSummaryViewModel();
            Vm.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(LoggedUser.CountryCode);
            Vm.SendingCurrencyCode = Common.Common.GetCountryCurrency(LoggedUser.CountryCode);
            Vm.SendToName = _userBankAccountServices.GetBankName(BankId);
            Vm.SendFromName = "KiiPay Wallet";
            Vm.SendingBalance = _userBankAccountServices.GetAmount();
            Vm.ReceiveBalance = _userBankAccountServices.GetAmount();
            Vm.FormattedAccNo = FormatAccNo(SendToAccount);
            ViewBag.IsPinCodeSend = 0;
            return View(Vm);
        }
        [HttpPost]
        public ActionResult SenderWithdrawMoneyFromWalletSummary([Bind(Include = SenderWithdrawMoneyFromWalletSummaryViewModel.BindProperty)]SenderWithdrawMoneyFromWalletSummaryViewModel Vm)
        {
            ViewBag.IsPinCodeSend = 0;
            if (string.IsNullOrEmpty(Vm.UserEnterPinCode))
            {
                Vm.PinCode = GetMobilePin();
                ViewBag.IsPinCodeSend = 1;
                return View(Vm);
            }
            else
            {
                string sentPinCode = _userBankAccountServices.GetMobilePinCode();

                if (Vm.UserEnterPinCode != sentPinCode)
                {
                    ViewBag.IsPinCodeSend = 1;
                    ModelState.AddModelError("UserEnterPinCode", " Invalid Pincode");
                    return View(Vm);
                }
            }
            return RedirectToAction("SenderWithdrawMoneyFromWalletSuccess");


        }

        public string GetMobilePin()
        {
            //if session null generate code and return  else return value in session

            string code = "";
            if (Common.FaxerSession.SentMobilePinCode == null || Common.FaxerSession.SentMobilePinCode == "")
            {
                code = Common.Common.GenerateRandomDigit(6);

                _userBankAccountServices.SetMobilePinCode(code);

                SmsApi smsService = new SmsApi();
                var msg = smsService.GetPinCodeMsg(code);

                var PhoneNo = Common.FaxerSession.LoggedUser.CountryPhoneCode + Common.FaxerSession.LoggedUser.PhoneNo;
                smsService.SendSMS(PhoneNo, msg);
            }

            else
            {
                code = Common.FaxerSession.SentMobilePinCode;
            }

            string mobilePinCode = code;
            return mobilePinCode;
        }

        [HttpGet]
        public ActionResult SenderWithdrawMoneyFromWalletSuccess()
        {
            var LoggedUser = Common.FaxerSession.LoggedUser;
            var country = Common.FaxerSession.FaxerCountry;
            int BankId = Common.FaxerSession.BankId;
            var Amount = Common.FaxerSession.AmountToBeTransferred;

            SenderWithdrawMoneyFromWalletSuccessViewModel Vm = new SenderWithdrawMoneyFromWalletSuccessViewModel();
            Vm.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(LoggedUser.CountryCode);
            Vm.BankName = _userBankAccountServices.GetBankName(BankId);
            Vm.Amount = _userBankAccountServices.GetAmount();


            return View(Vm);
        }

        [HttpGet]
        public ActionResult SenderAddNewBankAccount()
        {
            ViewBag.Banks = new SelectList(GetBanks(), "Code", "Name");
            ViewBag.Addresses = new SelectList(GetAddresses(), "Code", "Name");
            ViewBag.Branches = new SelectList(GetBranches(), "Code", "Name");
            ViewBag.Countries = new SelectList(GetCountries(), "Code", "Name");
            return View();

        }
        [HttpPost]
        public ActionResult SenderAddNewBankAccount([Bind(Include = SenderAddNewBankVM.BindProperty)]SenderAddNewBankVM Vm)
        {
            ViewBag.Banks = new SelectList(GetBanks(), "Code", "Name");
            ViewBag.Addresses = new SelectList(GetAddresses(), "Code", "Name");
            ViewBag.Branches = new SelectList(GetBranches(), "Code", "Name");
            ViewBag.Countries = new SelectList(GetCountries(), "Code", "Name");
            if (ModelState.IsValid)
            {
                return RedirectToAction("SenderAddNewBankAccountSuccess");
            }
            return View(Vm);
        }
        [HttpGet]
        public ActionResult SenderAddNewBankAccountSuccess()
        {

            return View();

        }
        public List<BankVm> GetBanks()
        {

            var result = new List<BankVm>();
            return result;

        }
        public List<AddressVm> GetAddresses()
        {
            var result = new List<AddressVm>();
            return result;
        }
        public List<BranchVm> GetBranches()
        {
            var result = new List<BranchVm>();
            return result;
        }
        public List<CountryVM> GetCountries()
        {
            var result = new List<CountryVM>();
            return result;
        }

    }
}