using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransferZero.Sdk.Model;
using Twilio.Rest.Trunking.V1;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;
using DropDownViewModel = FAXER.PORTAL.Areas.Agent.Models.DropDownViewModel;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentBankAccountDepositController : Controller
    {
        Admin.Services.CommonServices common = new Admin.Services.CommonServices();
        SAgentBankAccountDeposit _agentBankAccountDepositServices = null;
        SCashPickUpTransferService _cashPickUp = null;
        SSenderBankAccountDeposit _senderBankAccountDepositServices = null;
        SAgentInformation _sFaxerInfromationServices = null;
        SSenderKiiPayWalletTransfer _kiiPaytrasferServices = null;
        AgentCommonServices _CommonServices = null;


        public AgentBankAccountDepositController()
        {
            _agentBankAccountDepositServices = new SAgentBankAccountDeposit();
            _cashPickUp = new SCashPickUpTransferService();
            _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            _CommonServices = new AgentCommonServices();

        }
        // GET: Agent/AgentBankAccountDeposit
        [HttpGet]
        public ActionResult BankAccountDeposit(string AccountNoORPhoneNo = "")
        {
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();

            Common.FaxerSession.TransferMethod = "bankdeposit";

            //  AgentResult agentResult = new AgentResult();
            string agencyName = agentInfo.Name;
            string agencyMFSCode = agentInfo.AccountNo;
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            #region Old Design
            var countries = common.GetCountries();
            var identifyCardType = common.GetCardType();

            ViewBag.countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType");

            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();
            vm = _cashPickUp.GetCashPickupInformationViewModel();

            if (!string.IsNullOrEmpty(AccountNoORPhoneNo))
            {
                AccountNoORPhoneNo = Common.Common.IgnoreZero(AccountNoORPhoneNo);
                var result = _cashPickUp.getFaxer(AccountNoORPhoneNo, vm);
                if (result != null)
                {
                    ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType", vm.IdType);
                    string FaxerCountryPhoneCode = Common.Common.GetCountryPhoneCode(result.Country);
                    if (result.Id != 0)
                    {
                        return View(result);
                    }
                }
                else
                {
                    //agentResult.Message = "Account does not exist";
                    //agentResult.Status = ResultStatus.Warning;
                    //ViewBag.AgentResult = agentResult;
                    ModelState.AddModelError("InvalidAccount", "Account does not exist");
                    return View(vm);
                }

            }
            vm.Country = agentInfo.CountryCode;
            //ViewBag.AgentResult = agentResult;
            #endregion 
            return View(vm);

        }

        [HttpPost]
        public ActionResult BankAccountDeposit([Bind(Include = CashPickupInformationViewModel.BindProperty)] CashPickupInformationViewModel vm)
        {
            #region old
            //List<Admin.Services.DropDownCardTypeViewModel> identifyCardType = common.GetCardType();
            //List<Admin.Services.DropDownViewModel> countries = common.GetCountries();
            //ViewBag.countries = new SelectList(countries, "Code", "Name");
            //ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType");
            //vm.SenderCountryCode = Common.Common.GetCountryPhoneCode(vm.IssuingCountry);


            //string agentCountry = Common.AgentSession.AgentInformation.CountryCode;
            //if (vm.Country != agentCountry)
            //{
            //    ModelState.AddModelError("InvalidCountry", "This transaction was not from your country, please direct the customer to their respective country.");
            //    return View(vm);
            //}
            //var CurrentYear = DateTime.Now.Year;
            //if (vm.DOB == null)
            //{
            //    ModelState.AddModelError("", "Enter Date Of Birth .");
            //    return View(vm);
            //}
            //var DOB = vm.DOB;
            //DateTime date = Convert.ToDateTime(DOB);
            //var DOByear = date.Year;
            //var Age = CurrentYear - DOByear;
            //if (ModelState.IsValid)
            //{
            //    if (Age <= 18)
            //    {
            //        //    agentResult.Message = "Sender's should be more than 18 years to do the transaction.";
            //        //    agentResult.Status = ResultStatus.Warning;
            //        ModelState.AddModelError("InvalidAge", "Sender's should be more than 18 years to do the transaction.");

            //        return View(vm);
            //    }
            //    if (vm.ExpiryDate < DateTime.Now)
            //    {
            //        ModelState.AddModelError("IdExpired", "Expired ID");
            //        return View(vm);
            //    }
            //    _cashPickUp.SetCashPickupInformationViewModel(vm);
            //    //return RedirectToAction("BankAccountDepositSecond");

            //    return RedirectToAction("BankDepositAbroadEnterAmount");
            //}
            #endregion
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();

            if (string.IsNullOrEmpty(vm.Search))
            {
                ModelState.AddModelError("Search", "Enter telePhone/email");
                return View(vm);
            }
            var AccountNoORPhoneNo = Common.Common.IgnoreZero(vm.Search);
            TransactionTransferType transferType = TransactionTransferType.Agent;
            if (agentInfo.IsAUXAgent)
            {
                transferType = TransactionTransferType.AuxAgent;
            }
            var SenderDetails = _cashPickUp.getFaxer(AccountNoORPhoneNo, agentInfo.Id, transferType);

            if (SenderDetails == null)
            {
                ModelState.AddModelError("InvalidAccount", "Account does not exist");
                return View(vm);
            }
            else
            {
                _cashPickUp.SetCashPickupInformationViewModel(SenderDetails);
                return RedirectToAction("SenderDetails", "AgentBankAccountDeposit");
            }
        }

        public ActionResult AddSenderDetails()
        {
            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();
            return View(vm);
        }
        [HttpPost]
        public ActionResult AddSenderDetails([Bind(Include = CashPickupInformationViewModel.BindProperty)] CashPickupInformationViewModel vm)
        {
            #region Validation of sender Details
            if (string.IsNullOrEmpty(vm.FirstName))
            {
                ModelState.AddModelError("FirstName", "Enter First name");
                return View(vm);
            }
            if (string.IsNullOrEmpty(vm.LastName))
            {
                ModelState.AddModelError("LastName", "Enter Last name");
                return View(vm);
            }
            if (vm.Day == 0)
            {
                ModelState.AddModelError("Day", "Enter Day");
                return View(vm);
            }
            if (vm.Day > 32)
            {

                ModelState.AddModelError("Day", "Enter Validate Date");
                return View(vm);
            }
            if (vm.Month == Month.Month)
            {
                ModelState.AddModelError("Month", "Select Month");
                return View(vm);
            }
            if (vm.Year == 0)
            {
                ModelState.AddModelError("Month", "Enter Year");
                return View(vm);
            }
            if (string.IsNullOrEmpty(vm.PostCode))
            {
                ModelState.AddModelError("PostCode", "Enter PostCode/Zip Code");
                return View(vm);
            }
            if (string.IsNullOrEmpty(vm.AddressLine1))
            {
                ModelState.AddModelError("AddressLine1", "Enter Address Line 1");
                return View(vm);
            }
            if (string.IsNullOrEmpty(vm.City))
            {
                ModelState.AddModelError("City", "Enter City");
                return View(vm);
            }
            if (string.IsNullOrEmpty(vm.MobileNo))
            {
                ModelState.AddModelError("MobileNo", "Enter MobileNo");
                return View(vm);
            }
            var date = new DateTime(vm.Year, (int)vm.Month, vm.Day);

            var DOB = date;
            vm.DOB = DOB;
            bool isValidAge = Common.DateUtilities.ValidateAge(DOB);
            bool isValidSender = Common.Common.SenderExist(vm.MobileNo, vm.Email);
            if (isValidAge == false)
            {
                ModelState.AddModelError("DOB", "Sender Should be atleast 18 years");
                return View(vm);
            }
            if (isValidSender == false)
            {
                ModelState.AddModelError("InValid User", "Phone number or Email is already in use");
                return View(vm);
            }
            #endregion

            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            vm.CountryCode = agentInfo.CountryCode;
            vm.Country = agentInfo.CountryCode;
            var transferMethod = Common.FaxerSession.TransferMethod.ToLower();
            _cashPickUp.SetCashPickupInformationViewModel(vm);
            switch (transferMethod)
            {
                case "bankdeposit":
                    return RedirectToAction("BankDepositAbroadEnterAmount", "AgentBankAccountDeposit");
                case "cashpickup":
                    return RedirectToAction("CashPickUpEnterAmount", "AgentCashPickUpTransfer");
                case "otherwallet":
                    return RedirectToAction("MobileMoneyTransferEnterAmount", "AgentMobileMoneyTransfer");
                case "kiipaywallet":
                    SAgentKiiPayWalletTransferServices _kiiPayWalletTransfer = new SAgentKiiPayWalletTransferServices();
                    SendMoneToKiiPayWalletViewModel senderDetails = _kiiPayWalletTransfer.SetSenderDetails(vm);
                    _kiiPayWalletTransfer.SetSendMoneToKiiPayWalletViewModel(senderDetails);
                    return RedirectToAction("SendMoneyToKiiPayEnterAmount", "AgentKiiPayWalletTransfer");
                default:
                    return RedirectToAction("Index", "AgentDashboard");
            }
            return RedirectToAction("BankDepositAbroadEnterAmount", "AgentBankAccountDeposit");
        }
        public ActionResult SenderDetails()
        {
            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();
            vm = _cashPickUp.GetCashPickupInformationViewModel();
            return View(vm);
        }
        [HttpPost]
        public ActionResult SenderDetails([Bind(Include = CashPickupInformationViewModel.BindProperty)] CashPickupInformationViewModel vm)
        {
            var transferMethod = Common.FaxerSession.TransferMethod.ToLower();
            switch (transferMethod)
            {
                case "bankdeposit":
                    return RedirectToAction("BankDepositAbroadEnterAmount", "AgentBankAccountDeposit");
                    break;
                case "cashpickup":
                    return RedirectToAction("CashPickUpEnterAmount", "AgentCashPickUpTransfer");
                    break;
                case "otherwallet":
                    return RedirectToAction("MobileMoneyTransferEnterAmount", "AgentMobileMoneyTransfer");
                    break;
                case "kiipaywallet":
                    return RedirectToAction("SendMoneyToKiiPayEnterAmount", "AgentKiiPayWalletTransfer");
                    break;
                default:

                    return RedirectToAction("Index", "AgentDashboard");
                    break;
            }

            return RedirectToAction("BankDepositAbroadEnterAmount", "AgentBankAccountDeposit");

        }
        [HttpGet]
        public ActionResult BankAccountDepositSecond(string RecentAcccountNo = "")
        {
            SenderBankAccountDepositVm model = new SenderBankAccountDepositVm();
            model = _agentBankAccountDepositServices.GetAgentBankAccountDeposit();
            string Country = _agentBankAccountDepositServices.GetBankDepositAbroadEnterAmount().ReceivingCountry;
            if (string.IsNullOrEmpty(Country))
            {
                Country = model.CountryCode;
            }


            var country = Common.Common.GetCountries();
            var recentAccountNumbers = new List<DropDownViewModel>();
            if (!string.IsNullOrEmpty(Country))
            {
                var senderInfo = _cashPickUp.GetCashPickupInformationViewModel();

                recentAccountNumbers = _agentBankAccountDepositServices.GetRecentAccountNumbers(senderInfo.Id).Where(x => x.CountryCode == Country).ToList();
            }

            ViewBag.RecentAccountNumbers = new SelectList(recentAccountNumbers, "Code", "Name", RecentAcccountNo);
            model.CountryCode = Country;
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName", Country);
            var paymentInfo = _agentBankAccountDepositServices.GetBankDepositAbroadEnterAmount();
            SetViewBagForBanks(Country, paymentInfo.ReceivingCurrency);
            getRecentAccountno(ref model, RecentAcccountNo, Country);
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Country);
            ViewBag.Branches = new SelectList(_senderBankAccountDepositServices.GetBranches(model.BankId), "Code", "Name", model.BranchCode);

            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            var Data = _kiiPaytrasferServices.GetCommonEnterAmount();
            if (Data != null && Data.ReceivingAmount != 0 && Data.SendingAmount != 0 && Data.Fee != 0 && Data.ExchangeRate != 0 && Data.TotalAmount != 0)
            {
                model.CountryCode = Data.ReceivingCountryCode;
                ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Data.ReceivingCountryCode);
                return View(model);
            }
            return View(model);
        }
        public void getRecentAccountno(ref SenderBankAccountDepositVm vm, string accountNo, string Country = "")
        {

            var accountData = _senderBankAccountDepositServices.GetAccountInformationFromAccountNumber(accountNo);

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
            vm.IsEuropeTransfer = Common.Common.IsEuropeTransfer(Country);
            vm.IsSouthAfricaTransfer = Common.Common.IsSouthAfricanTransfer(Country);
            vm.IsWestAfricaTransfer = Common.Common.IsWestAfricanTransfer(Country);

        }


        private void SetViewBagForBanks(string Country, string currency)
        {
            var banks = _senderBankAccountDepositServices.getBanksByCurrency(Country, currency);
            ViewBag.BankNames = new SelectList(banks, "Id", "Name");
        }

        [HttpPost]
        public ActionResult BankAccountDepositSecond([Bind(Include = SenderBankAccountDepositVm.BindProperty)] SenderBankAccountDepositVm model)
        {
            var country = Common.Common.GetCountries();
            var senderInfo = _cashPickUp.GetCashPickupInformationViewModel();
            var recentAccountNumbers = _agentBankAccountDepositServices.GetRecentAccountNumbers(senderInfo.Id);


            ViewBag.RecentAccountNumbers = new SelectList(recentAccountNumbers, "Name", "Name");
            var paymentInfo = _agentBankAccountDepositServices.GetBankDepositAbroadEnterAmount();
            SetViewBagForBanks(model.CountryCode, paymentInfo.ReceivingCurrency);
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName", model.CountryCode);
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(model.CountryCode);
            ViewBag.Branches = new SelectList(_senderBankAccountDepositServices.GetBranches(model.BankId), "Code", "Name", model.BranchCode);
            model.IsEuropeTransfer = Common.Common.IsEuropeTransfer(model.CountryCode);
            model.IsSouthAfricaTransfer = Common.Common.IsSouthAfricanTransfer(model.CountryCode);
            model.IsWestAfricaTransfer = Common.Common.IsWestAfricanTransfer(model.CountryCode);
            if (ModelState.IsValid)
            {

                //if (string.IsNullOrEmpty(model.BranchCode)) //have to select bank if it is not europe transfer  
                //{

                //    ModelState.AddModelError("BranchCode", "Enter code");
                //    return View(model);

                //}
                if (model.CountryCode == "NG")
                {
                    model.BranchCode = Common.Common.getBank(model.BankId).Code;
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
                bool IsManualDeposit = Common.Common.IsManualDeposit(Common.AgentSession.AgentInformation.CountryCode, model.CountryCode);
                bool IsValidateAccountNo = true;

                if (!IsManualDeposit)
                {

                    var bankDeposit = _agentBankAccountDepositServices.GetBankDepositAbroadEnterAmount();
                    var agentInfo = Common.AgentSession.AgentInformation;
                    var transferType = TransactionTransferType.Agent;
                    if (agentInfo.IsAUXAgent)
                    {
                        transferType = TransactionTransferType.AuxAgent;
                    }
                    var Apiservice = Common.Common.GetApiservice(Common.AgentSession.AgentInformation.CountryCode, model.CountryCode, bankDeposit.SendingAmount,
                       TransactionTransferMethod.BankDeposit, transferType, agentInfo.Id);
                    if (Apiservice == null)
                    {

                        ModelState.AddModelError("ServiceNotAvialable", "Service Not Avialable");
                        return View(model);
                    }
                    //switch (Apiservice)
                    //{
                    //    case DB.Apiservice.VGG:
                    //        // validate Bank Account
                    //        BankDepositApi api = new BankDepositApi();
                    //        var accessToken = api.Login<AccessTokenVM>();
                    //        Common.FaxerSession.BankAccessToken = accessToken.Result;
                    //        if (accessToken.Result == null)
                    //        {
                    //            ModelState.AddModelError("", "Receiver's bank account number validation is taking longer than expected, please try again later!");
                    //            return View(model);
                    //        }
                    //        if (accessToken.Result == null && string.IsNullOrEmpty(accessToken.Result.AccessToken))
                    //        {
                    //            ModelState.AddModelError("", "Receiver's bank account number validation is taking longer than expected, please try again later!");
                    //            return View(model);
                    //        }

                    //        var validateAccountNo = api.ValidateAccountNo<AccountNoLookUpResponse>(
                    //             model.BranchCode, model.AccountNumber, accessToken.Result);
                    //        IsValidateAccountNo = validateAccountNo.Result.status;
                    //        break;
                    //    case DB.Apiservice.TransferZero:
                    //        TransferZeroApi transferZeroApi = new TransferZeroApi();
                    //        if (model.CountryCode == "NG" || model.CountryCode == "GH")
                    //        {
                    //            AccountValidationRequest accountValidationRequest = new AccountValidationRequest(
                    //                        bankAccount: model.AccountNumber,
                    //                        bankCode: model.BranchCode,
                    //                        country: (AccountValidationRequest.CountryEnum)Common.Common.getAccountValidationCountryCodeForTZ(model.CountryCode),
                    //                        currency: (AccountValidationRequest.CurrencyEnum)Common.Common.getAccountValidationCountryCurrencyForTZ(model.CountryCode),
                    //                        method: AccountValidationRequest.MethodEnum.Bank
                    //         );
                    //            var result = transferZeroApi.ValidateAccountNo(accountValidationRequest);
                    //            IsValidateAccountNo = result.Meta == null ? true : false;
                    //        }
                    //        break;

                    //    default:
                    //        break;
                    //}
                    var IsValidAccountNo = _senderBankAccountDepositServices.IsValidBankAccount(model, bankDeposit.SendingAmount,
                        agentInfo.CountryCode, transferType, agentInfo.Id);
                    if (IsValidAccountNo.Data == false)
                    {
                        ModelState.AddModelError("", IsValidAccountNo.Message);
                        return View(model);
                    }
                }
                if (IsValidateAccountNo == true)
                {
                    model.IsManualDeposit = IsManualDeposit;
                    _agentBankAccountDepositServices.SetAgentBankAccountDeposit(model);
                    return RedirectToAction("TransactionSummary", "AgentBankAccountDeposit");
                }
                else
                {
                    ModelState.AddModelError("", "Enter a validate account number");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Please enter the required field");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult BankDepositAbroadEnterAmount()
        {
            //int AgentId = Common.AgentSession.AgentInformation.Id;

            //string agentCountry = getAgentCountryCode(AgentId);
            //var model = _agentBankAccountDepositServices.GetAgentBankAccountDeposit();
            //var receiverInformation = _agentBankAccountDepositServices.GetReceiverInformationFromAccountNumnber(model.AccountNumber);
            //BankDepositAbroadEnterAmountVM vm = new BankDepositAbroadEnterAmountVM
            //{
            //    ReceiverName = receiverInformation.ReceiverName,
            //    ReceiverId = receiverInformation.Id,
            //    SendingCurrencySymbol = Common.Common.GetCurrencySymbol(agentCountry),
            //    SendingCurrencyCode = Common.Common.GetCountryCurrency(agentCountry),

            //    ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(model.CountryCode),
            //    ReceivingCurrencyCode = Common.Common.GetCountryCurrency(model.CountryCode),
            //    ExchangeRate = SExchangeRate.GetExchangeRateValue(agentCountry, model.CountryCode,
            //    TransactionTransferMethod.BankDeposit, Common.AgentSession.AgentInformation.Id, TransactionTransferType.Agent)

            //};

            //_CommonServices.SetAgentPaymentSummarySession(model.CountryCode, TransactionTransferMethod.BankDeposit);
            //_agentBankAccountDepositServices.SetBankDepositAbroadEnterAmount(vm);
            //if (vm.SendingAmount == 0)
            //{

            //    vm.SendingAmount = 1;
            //}
            //return View(vm);
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();
            BankDepositAbroadEnterAmountVM vm = new BankDepositAbroadEnterAmountVM();
            var paymentInfo = _agentBankAccountDepositServices.GetBankDepositAbroadEnterAmount();

            var agentInfo = Common.AgentSession.AgentInformation;
            int AgentId = agentInfo.Id;
            var agentCountry = getAgentCountryCode(AgentId);
            ViewBag.AgentId = AgentId;
            ViewBag.IsAuxAgent = agentInfo.IsAUXAgent;
            ViewBag.TransferType = 2;
            if (agentInfo.IsAUXAgent)
            {
                ViewBag.TransferType = 4;
            }
            if (paymentInfo != null)
            {
                vm.SendingAmount = paymentInfo.SendingAmount;
                vm.ReceivingAmount = paymentInfo.ReceivingAmount;
                vm.ReceivingCurrency = paymentInfo.ReceivingCurrency == null ? "NGN" : paymentInfo.ReceivingCurrency;
                vm.SendingCurrency = Common.Common.GetCountryCurrency(agentCountry);
                vm.Fee = paymentInfo.Fee;
                vm.SendingCountry = agentCountry;
                vm.IsConfirm = paymentInfo.IsConfirm;
                vm.AgentCommission = paymentInfo.AgentCommission;
                vm.TotalAmount = paymentInfo.TotalAmount;

                return View(vm);
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult BankDepositAbroadEnterAmount([Bind(Include = BankDepositAbroadEnterAmountVM.BindProperty)] BankDepositAbroadEnterAmountVM Vm)
        {

            //int AgentId = Common.AgentSession.AgentInformation.Id;
            //string agentCountry = getAgentCountryCode(AgentId);
            //var sender = _cashPickUp.GetCashPickupInformationViewModel();

            //var BankDeposit = _agentBankAccountDepositServices.GetAgentBankAccountDeposit();

            //var paymentInfo = _agentBankAccountDepositServices.GetBankDepositAbroadEnterAmount();
            var agentInfo = Common.AgentSession.AgentInformation;
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();
            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();
            Vm.ReceivingCurrency = paymentInfo.ReceivingCurrency;
            Vm.SendingCurrency = paymentInfo.SendingCurrency;
            Vm.SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol;
            Vm.ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol;
            Vm.SendingCountry = paymentInfo.SendingCountryCode;
            Vm.ReceivingCountry = paymentInfo.ReceivingCountryCode;
            Vm.SendingAmount = paymentInfo.SendingAmount;
            Vm.ReceivingAmount = paymentInfo.ReceivingAmount;
            ViewBag.AgentId = agentInfo.Id;
            ViewBag.IsAuxAgent = agentInfo.IsAUXAgent;
            ViewBag.TransferType = 2;
            if (agentInfo.IsAUXAgent)
            {
                ViewBag.TransferType = 4;
            }
            if (Vm.SendingAmount == 0)
            {
                ModelState.AddModelError("SendingAmount", "Please Enter Sending Amount ");
                return View(Vm);
            }
            if (Vm.ReceivingAmount == 0)
            {
                ModelState.AddModelError("RecevingAmount", "Please Enter Sending Amount ");
                return View(Vm);
            }
            if (string.IsNullOrEmpty((Vm.Fee).ToString()))
            {
                ModelState.AddModelError("FaxingFee", "Please calculate estimated fee ");
                return View(Vm);
            }

            if (Vm.IsConfirm == true)
            {
                #region old
                //if (sender.Id == 0)
                //{
                //    SFaxerSignUp faxerSignUpService = new SFaxerSignUp();
                //    string accountNo = faxerSignUpService.GetNewAccount(10);
                //    DB.FaxerInformation FaxerDetails = new DB.FaxerInformation()
                //    {
                //        FirstName = sender.FirstName,
                //        MiddleName = sender.MiddleName,
                //        LastName = sender.LastName,
                //        Address1 = sender.AddressLine1,
                //        City = sender.City,
                //        Country = sender.Country,
                //        Email = sender.Email,
                //        PhoneNumber = sender.MobileNo,
                //        IdCardNumber = sender.IdNumber,
                //        IdCardType = sender.IdType.ToString(),
                //        IssuingCountry = sender.IssuingCountry,
                //        RegisteredByAgent = true,
                //        IsDeleted = false,
                //        IdCardExpiringDate = sender.ExpiryDate,
                //        AccountNo = accountNo
                //    };
                //    var SenderAddedNew = _senderBankAccountDepositServices.AddSender(FaxerDetails);
                //    sender.Id = SenderAddedNew.Data.Id;

                //}

                //var ApiService = Common.Common.GetApiservice(sender.Country,
                //               BankDeposit.CountryCode, paymentInfo.SendingAmount, TransactionTransferMethod.BankDeposit, TransactionTransferType.Agent);

                //BankAccountDeposit BankAccountDeposit = new BankAccountDeposit()
                //{
                //    ExchangeRate = paymentInfo.ExchangeRate,
                //    Fee = paymentInfo.Fee,
                //    PaidFromModule = Module.Agent,
                //    TransactionDate = DateTime.Now,
                //    TotalAmount = paymentInfo.TotalAmount,
                //    ReceiverAccountNo = BankDeposit.AccountNumber,
                //    SenderPaymentMode = PORTAL.Models.SenderPaymentMode.Cash,
                //    ReceivingAmount = paymentInfo.ReceivingAmount,
                //    SenderId = sender.Id,
                //    ReceivingCountry = BankDeposit.CountryCode,
                //    SendingAmount = paymentInfo.SendingAmount,
                //    SendingCountry = agentCountry,
                //    ReceiverName = paymentInfo.ReceiverName,
                //    ReceiverCountry = BankDeposit.CountryCode,
                //    ReceiptNo = BankDeposit.IsManualDeposit == false ? Common.Common.GenerateBankAccountDepositReceiptNoforAgnet(6) : Common.Common.GenerateManualBankAccountDepositReceiptNo(6),
                //    //ReceiptNo = Common.Common.GenerateBankAccountDepositReceiptNoforAgnet(6),
                //    PayingStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName,
                //    PayingStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                //    AgentCommission = paymentInfo.AgentCommission,
                //    BankId = BankDeposit.BankId,
                //    BankCode = BankDeposit.BranchCode,
                //    ReceiverMobileNo = BankDeposit.MobileNumber,
                //    IsManualDeposit = BankDeposit.IsManualDeposit,
                //    Status = BankDeposit.IsManualDeposit == true ? BankDepositStatus.Incomplete : BankDepositStatus.Confirm,
                //    Apiservice = ApiService

                //};
                //if (BankAccountDeposit.SendingCountry == BankAccountDeposit.ReceivingCountry)
                //{
                //    BankAccountDeposit.PaymentType = PaymentType.Local;
                //}
                //else
                //{
                //    BankAccountDeposit.PaymentType = PaymentType.International;

                //}
                //var obj = new BankAccountDeposit();
                //SSenderForAllTransfer senderForAllTransfer = new SSenderForAllTransfer();
                //string ReceiverFirstname = BankAccountDeposit.ReceiverName.Split(' ')[0];
                //if (BankAccountDeposit.IsManualDeposit == true)
                //{
                //    // Send Email And SMS 
                //    obj = _senderBankAccountDepositServices.Add(BankAccountDeposit).Data;

                //    senderForAllTransfer.SendManualBankDepositFirstSmsToSender(ReceiverFirstname, Common.Common.GetCurrencyCode(BankAccountDeposit.ReceivingCountry) + " " + BankAccountDeposit.ReceivingAmount,
                //                          Common.Common.GetCountryName(BankAccountDeposit.ReceivingCountry),
                //                          BankAccountDeposit.ReceiptNo, Common.Common.GetCountryPhoneCode(BankAccountDeposit.SendingCountry) + sender.MobileNo);

                //    string bankName = Common.Common.getBankName(BankAccountDeposit.BankId);
                //    DB.FAXEREntities dbContext = new FAXEREntities();
                //    string AgentPhoneNo = dbContext.ManualDepositEnable
                //                  .Where(x => x.PayingCountry == BankAccountDeposit.ReceivingCountry
                //                    && x.IsEnabled == true).Select(x => x.MobileNo).FirstOrDefault();

                //    senderForAllTransfer.SendManualBankDepositSmsToAgent(BankAccountDeposit.ReceiptNo, BankAccountDeposit.ReceiverName, bankName, BankAccountDeposit.ReceiverAccountNo,
                //                                    BankAccountDeposit.BankCode, BankAccountDeposit.SendingAmount + " " + Common.Common.GetCurrencyCode(BankAccountDeposit.ReceivingCountry),
                //                                    Common.Common.GetCountryPhoneCode(BankAccountDeposit.ReceivingCountry) + AgentPhoneNo);

                //    senderForAllTransfer.SendManualDepositInProgressEmail(sender.FirstName, BankAccountDeposit.Fee, BankAccountDeposit.SendingAmount, Common.Common.GetCurrencyCode(BankAccountDeposit.SendingCountry),
                //    BankAccountDeposit.ReceiverAccountNo, Common.Common.GetCurrencyCode(BankAccountDeposit.ReceivingCountry), BankAccountDeposit.ReceivingAmount, BankAccountDeposit.BankId,
                //    BankAccountDeposit.ReceiverName, BankAccountDeposit.SenderId, BankAccountDeposit.ReceiptNo, BankAccountDeposit.BankCode, BankAccountDeposit.ReceiverCountry, ReceiverFirstname
                //    );
                //}
                //else
                //{
                //    // Create bank Api response log 

                //    SBankDepositResponseStatus sBankDepositResponseStatus = new SBankDepositResponseStatus();

                //    var bankdepositTransactionResult = new BankDepositResponseVm();
                //    SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

                //    TransactionSummaryVM transactionSummaryvm = new TransactionSummaryVM();

                //    transactionSummaryvm.SenderAndReceiverDetail = _senderBankAccountDepositServices.GetSenderAndReceiverDetails();
                //    transactionSummaryvm.BankAccountDeposit = _senderBankAccountDepositServices.GetMobileMoneyTransferDetails(BankAccountDeposit);
                //    transactionSummaryvm.KiiPayTransferPaymentSummary = _senderBankAccountDepositServices.GetKiiPayTransferPaymentSummary(BankAccountDeposit);

                //    _senderForAllTransferServices.SetTransactionSummary(transactionSummaryvm);

                //    switch (ApiService)
                //    {
                //        case DB.Apiservice.VGG:
                //            bankdepositTransactionResult = _agentBankAccountDepositServices.GetBankApiPaymentResponse(BankAccountDeposit.ReceiptNo);
                //            var transcationStatus = bankdepositTransactionResult.result.transactionStatus;
                //            if (transcationStatus == 1)
                //            {
                //                BankAccountDeposit.Status = BankDepositStatus.Incomplete;
                //            }
                //            else if (transcationStatus == 2)
                //            {
                //                BankAccountDeposit.Status = BankDepositStatus.Confirm;
                //            }
                //            else if (transcationStatus == 3)
                //            {
                //                BankAccountDeposit.Status = BankDepositStatus.Failed;
                //            }
                //            else if (transcationStatus == 0)
                //            {
                //                BankAccountDeposit.Status = BankDepositStatus.Incomplete;
                //            }
                //            break;
                //        case DB.Apiservice.TransferZero:


                //            string TransactionId = Guid.NewGuid().ToString();
                //            var transferZeroResponse = _senderForAllTransferServices.GetBankDepositTransferZeroTransactionResponse(TransactionId);
                //            var status = Common.Common.GetTransferZeroTransactionStatus(transferZeroResponse);
                //            var transferZeroTransactionResult = transferZeroResponse;
                //            var responseModel = _senderForAllTransferServices.PrepareTransferZeroResponse(transferZeroTransactionResult);
                //            responseModel.result.beneficiaryAccountName = BankAccountDeposit.ReceiverAccountNo;
                //            responseModel.result.beneficiaryBankCode = BankAccountDeposit.BankCode;
                //            responseModel.result.beneficiaryAccountName = BankAccountDeposit.ReceiverName;
                //            responseModel.result.amountInBaseCurrency = BankAccountDeposit.SendingAmount;
                //            responseModel.result.targetAmount = BankAccountDeposit.ReceivingAmount;
                //            responseModel.result.partnerTransactionReference = BankAccountDeposit.ReceiptNo;
                //            bankdepositTransactionResult = responseModel;
                //            break;
                //        default:
                //            break;
                //    }

                //    #region old
                //    //var bankdepositTransactionResult = _agentBankAccountDepositServices.GetBankApiPaymentResponse(BankAccountDeposit.ReceiptNo);

                //    //if (bankdepositTransactionResult.result != null)
                //    //{
                //    //    var transcationStatus = bankdepositTransactionResult.result.transactionStatus;
                //    //    if (transcationStatus == 1)
                //    //    {
                //    //        BankAccountDeposit.Status = BankDepositStatus.Incomplete;
                //    //    }
                //    //    else if (transcationStatus == 2)
                //    //    {
                //    //        BankAccountDeposit.Status = BankDepositStatus.Confirm;
                //    //    }
                //    //    else if (transcationStatus == 3)
                //    //    {
                //    //        //BankAccountDeposit.Status = BankDepositStatus.Cancel;
                //    //        BankAccountDeposit.Status = BankDepositStatus.Failed;
                //    //    }
                //    //    else if (transcationStatus == 0)
                //    //    {
                //    //        //BankAccountDeposit.Status = BankDepositStatus.Failed;
                //    //        BankAccountDeposit.Status = BankDepositStatus.Incomplete;
                //    //    }
                //    //}
                //    //else
                //    //{
                //    //    BankAccountDeposit.Status = BankDepositStatus.Failed;
                //    //}
                //    #endregion

                //    obj = _senderBankAccountDepositServices.Add(BankAccountDeposit).Data;

                //    sBankDepositResponseStatus.AddLog(bankdepositTransactionResult, obj.Id);

                //    // Send Email And SMS 

                //    senderForAllTransfer.SendBankDepositSms(sender.FirstName, Common.Common.GetCurrencyCode(BankAccountDeposit.ReceivingCountry) + " " + BankAccountDeposit.ReceivingAmount
                //           , BankAccountDeposit.ReceiptNo, Common.Common.GetCountryPhoneCode(sender.Country) + sender.MobileNo, ReceiverFirstname, BankAccountDeposit.Status);

                //    senderForAllTransfer.SendMoneyTransferedEmail(sender.FirstName, BankAccountDeposit.Fee, BankAccountDeposit.SendingAmount, Common.Common.GetCurrencyCode(BankAccountDeposit.SendingCountry),
                //    BankAccountDeposit.ReceiverAccountNo, Common.Common.GetCurrencyCode(BankAccountDeposit.ReceivingCountry), BankAccountDeposit.ReceivingAmount, BankAccountDeposit.BankId,
                //    BankAccountDeposit.ReceiverName, BankAccountDeposit.SenderId, BankAccountDeposit.ReceiptNo, BankAccountDeposit.BankCode, BankAccountDeposit.ReceiverCountry, BankAccountDeposit.Status);

                //}
                #endregion
                //  return RedirectToAction("BankAccountDepositSuccess", "AgentBankAccountDeposit", new { Id = obj.Id });
                //Vm.ReceivingCountry = Common.Common.GetCountryCodeByCurrency(Vm.ReceivingCurrencyCode);

                _agentBankAccountDepositServices.SetBankDepositAbroadEnterAmount(Vm);

                if (agentInfo.IsAUXAgent)
                {
                    AgentCommonServices _AgentCommonServices = new AgentCommonServices();
                    decimal AUXAgentAccountBalance = _AgentCommonServices.getAuxAgentAccountBalance(agentInfo.Id, Common.AgentSession.LoggedUser.PayingAgentStaffId);
                    if (AUXAgentAccountBalance <= Vm.TotalAmount)
                    {
                        ModelState.AddModelError("Insufficientfund", "Insufficient account balance");

                        return View(Vm);
                    }
                }

                return RedirectToAction("BankAccountDepositSecond");
            }
            else
            {
                ModelState.AddModelError("IsConfirm", "Please Confirm.");
                return View(Vm);
            }
        }

        public void SendBankDepositSms(string firstName, string Amount, string reference, string PhoneNo, string receiverFirstName, DB.BankDepositStatus status)
        {

            SmsApi smsApi = new SmsApi();
            string msg = smsApi.GetBankAccountDepositMsg(firstName, Amount, reference, receiverFirstName, status);
            smsApi.SendSMS(PhoneNo, msg);
        }



        [HttpGet]
        public ActionResult TransactionSummary()
        {
            var paymentInfo = _agentBankAccountDepositServices.GetBankDepositAbroadEnterAmount();
            var ReceiverInfo = _agentBankAccountDepositServices.GetAgentBankAccountDeposit();
            string ReceiverFirstname = ReceiverInfo.AccountOwnerName.Split(' ')[0];
            CommonTransactionSummaryViewModel vm = new CommonTransactionSummaryViewModel()
            {
                Fee = paymentInfo.Fee,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                ReceivingCurrecyCode = paymentInfo.ReceivingCurrency,
                ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCurrencyCode = paymentInfo.SendingCurrency,
                SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol,
                ReceiverFirstName = ReceiverFirstname,
                TotalAmount = paymentInfo.SendingAmount + paymentInfo.Fee
            };


            return View(vm);
        }
        [HttpPost]
        public ActionResult TransactionSummary([Bind(Include = CommonTransactionSummaryViewModel.BindProperty)]CommonTransactionSummaryViewModel vm)
        {
            _agentBankAccountDepositServices.SetTransactionSummary();

            TransferForAllAgentServices transferForAllAgentServices = new TransferForAllAgentServices();
            int transactionId = transferForAllAgentServices.CompleteTransaction(TransactionTransferMethod.BankDeposit);

            //int id = _agentBankAccountDepositServices.TransctionCompleted();
            //var agentInfo = Common.AgentSession.AgentInformation;

            int senderId = _agentBankAccountDepositServices.GetSenderFromBankeposit(transactionId);
            Common.AgentSession.SenderId = senderId;
            var SenderDocumentApprovalStatus = Common.Common.GetSenderIdentificationStatus(senderId);
            switch (SenderDocumentApprovalStatus)
            {
                case null:
                    return RedirectToAction("IdentityVerification");
                case DocumentApprovalStatus.Approved:
                    break;
                case DocumentApprovalStatus.Disapproved:
                    return RedirectToAction("IdentityVerification");
                case DocumentApprovalStatus.InProgress:
                    return RedirectToAction("IdentityVerificationInProgress");
                default:
                    break;
            }
            return RedirectToAction("BankAccountDepositSuccess", "AgentBankAccountDeposit", new { @Id = transactionId });
        }

        public ActionResult IdentityVerification()
        {
            var countries = common.GetCountries();
            var identifyCardType = common.GetCardType();

            ViewBag.countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType");

            return View();

        }
        [HttpPost]
        public ActionResult IdentityVerification([Bind(Include = IdentificationDetailModel.BindProperty)]IdentificationDetailModel model)
        {

            var countries = common.GetCountries();
            var identifyCardType = common.GetCardType();

            ViewBag.countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType");


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

                if (Request.Files.Count < 1)
                {
                    var identificationdoc = Request.Files["Document"];
                }
                string identificationDocPath = "";
                string DocumentPhotoUrl = "";
                var IdentificationDoc = Request.Files["Document"];

                if (IdentificationDoc != null && IdentificationDoc.ContentLength > 0)
                {
                    var allowedExtensions = new string[] { "gif", "jpg", "png", "jpeg", "pdf" };
                    int fileLength = IdentificationDoc.FileName.Split('.').Length;
                    var extension = IdentificationDoc.FileName.Split('.')[fileLength - 1];
                    extension = extension.ToLower();
                    identificationDocPath = Guid.NewGuid() + "." + IdentificationDoc.FileName.Split('.')[fileLength - 1];

                    if (allowedExtensions.Contains(extension))
                    {
                        try
                        {
                            IdentificationDoc.SaveAs(Server.MapPath("~/Documents") + "\\" + identificationDocPath);
                        }
                        catch (Exception ex)
                        {

                        }
                        DocumentPhotoUrl = "/Documents/" + identificationDocPath;
                    }
                    else
                    {
                        ModelState.AddModelError("document", "File type not allowed to upload. ");
                        return View(model);
                    }

                }
                else
                {


                    ModelState.AddModelError("document", "Upload Id");
                    return View(model);
                }
                int senderId = Common.AgentSession.SenderId;
                string DocumentName = IdentificationDoc.FileName.Split('.')[0];
                CommonServices _CommonServices = new CommonServices();
                var senderInfo = _CommonServices.GetSenderInfo(senderId);
                var senderDocumentation = _CommonServices.GetSenderDocumentation(senderId);
                if (senderDocumentation.Count > 0)
                {
                    SenderDocumentationViewModel vm = (from c in senderDocumentation
                                                       select new SenderDocumentationViewModel()
                                                       {
                                                           AccountNo = c.AccountNo,
                                                           DocumentPhotoUrl = DocumentPhotoUrl,
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
                        DocumentPhotoUrl = DocumentPhotoUrl,
                        SenderName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName,
                        Status = DocumentApprovalStatus.InProgress,
                        IsUploadedFromSenderPortal = false,
                        IssuingCountry = model.IssuingCountry,
                        IdentificationTypeId = model.IdentificationTypeId,
                        IdentityNumber = model.IdentityNumber,
                        ExpiryDate = ExpiryDate,
                        AgentId = Common.AgentSession.LoggedUser.Id,
                        IsUploadedFromAgentPortal = true


                    };
                    _senderDocumentationServices.UploadDocument(vm);
                }

                _senderDocumentationServices.SendIdentiVerificationInProgressEmail(senderId);

                return RedirectToAction("IdentityVerificationInProgress");
            }

            return View(model);
        }

        public ActionResult IdentityVerificationInProgress()
        {
            return View();

        }

        public ActionResult BankAccountDepositSuccess(int Id)
        {
            var result = _agentBankAccountDepositServices.GetBankDepositAbroadEnterAmount();
            var ReceiverInfo = _agentBankAccountDepositServices.GetAgentBankAccountDeposit();
            string ReceiverFirstname = ReceiverInfo.AccountOwnerName.Split(' ')[0];

            BankAccountDepositSuccessVM model = new BankAccountDepositSuccessVM()
            {
                Amount = result.SendingAmount,
                Currency = result.SendingCurrencySymbol,
                ReceiverName = ReceiverFirstname,
                TransactionId = Id,
            };
            //_CommonServices.ClearAgentBankAccountDeposit();
            return View(model);

        }
        public void PrintReceipt(int TransactionId)
        {

            var CarduserInformation = _senderBankAccountDepositServices.bankAccountDepositInfo(TransactionId);

            var agentInformatioin = Common.AgentSession.AgentInformation;

            var cashPickupReceiver = _cashPickUp.GetCashPickUpReceiverInfoViewModel();

            var senderInfo = _cashPickUp.GetCashPickupInformationViewModel();
            var ReceiverInfo = _agentBankAccountDepositServices.GetAgentBankAccountDeposit();

            // Need Changes Here  Because the sender info is conditional now 
            //the Kiipay Personal might or might not be associated with the faxer

            //var faxerInformation = dbContext.FaxerInformation.Where(x => x.Id == CarduserInformation.FaxerId).FirstOrDefault();

            string CardUserCurrency = Common.Common.GetCountryCurrency(CarduserInformation.SendingCountry);
            string SenderPhoneCode = Common.Common.GetCountryPhoneCode(CarduserInformation.SendingCountry);
            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(ReceiverInfo.CountryCode);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardUserReceiverReceipt/PrintReceiptOfBankDeposit?MFReceiptNumber=" + CarduserInformation.ReceiptNo +
                 "&TransactionDate=" + CarduserInformation.TransactionDate.ToString("dd/MM/yyyy") +
                 "&TransactionTime=" + CarduserInformation.TransactionDate.ToString("HH:mm") +
                 "&SenderFullName=" + senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName +
                 "&SenderEmail=" + senderInfo.Email +
                 "&SenderTelephone=" + SenderPhoneCode + " " + senderInfo.MobileNo +
                 "&SenderDOB=" + senderInfo.DOB.ToFormatedString("dd/MM/yyyy") +
                 "&ReceiverFullName=" + ReceiverInfo.AccountOwnerName +
                 "&ReceiverAccount=" + ReceiverInfo.AccountNumber +
                 "&ReceiverTelephone=" + ReceiverPhoneCode + " " + ReceiverInfo.MobileNumber +
                 "&AgentName=" + agentInformatioin.Name +
                 "&AgentAcountNumber=" + agentInformatioin.AccountNo +
                 "&StaffName=" + agentInformatioin.ContactPerson +
                 "&AgentCity=" + agentInformatioin.City +
                 "&AgentCountry=" + Common.Common.GetCountryName(agentInformatioin.CountryCode) +
                 "&TotalAmount=" + CardUserCurrency + "" + CarduserInformation.TotalAmount +
                 "&Fee=" + CardUserCurrency + " " + CarduserInformation.Fee +
                 "&SendingAmount=" + CardUserCurrency + "" + CarduserInformation.SendingAmount +
                 "&ExchangeRate=" + CarduserInformation.ExchangeRate +
                 "&SendingCurrency= " + Common.Common.GetCountryCurrency(senderInfo.Country) +
                 "&ReceivingCurrency= " + Common.Common.GetCountryCurrency(ReceiverInfo.CountryCode) +
                 "&ReceivingAmount=" + CarduserInformation.ReceivingAmount +
                 "&BankName=" + Common.Common.getBankName(CarduserInformation.BankId) +
                 "&BankCode=" + CarduserInformation.BankCode +
                 "&ReceivingCountry=" + Common.Common.GetCountryName(CarduserInformation.ReceivingCountry);
            var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);
            //var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";
            //ReceiptPDF.Save(path);
            byte[] bytes = ReceiptPDF.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
            //return File(path, "application/pdf");

        }
        public string getAgentCountryCode(int agentId = 0)
        {
            _sFaxerInfromationServices = new SAgentInformation();
            var result = _sFaxerInfromationServices.list().Data.Where(x => x.Id == agentId).Select(x => x.CountryCode).FirstOrDefault();
            return result;
        }
        public JsonResult GetAccountInformation(string accountNo)
        {

            var accountData = _senderBankAccountDepositServices.GetAccountInformationFromAccountNumber(accountNo);

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
        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount, bool IsReceivingAmount, string receiverCountry)
        {

            int AgentId = Common.AgentSession.AgentInformation.Id;

            var agentCountry = getAgentCountryCode(AgentId);
            //var receiverCountry = _agentBankAccountDepositServices.GetAgentBankAccountDeposit().CountryCode;
            var enterAmountData = _agentBankAccountDepositServices.GetBankDepositAbroadEnterAmount();
            var SendingCurrencySymbol = Common.Common.GetCurrencySymbol(agentCountry);
            var SendingCurrencyCode = Common.Common.GetCurrencyCode(agentCountry);
            var RecevingCurrencySymbol = Common.Common.GetCurrencySymbol(receiverCountry);
            var ReceivingCurrencyCode = Common.Common.GetCurrencyCode(receiverCountry);


            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }

            var feeInfo = SEstimateFee.GetTransferFee(agentCountry, receiverCountry, TransactionTransferMethod.BankDeposit, SendingAmount,
                TransactionTransferType.Agent, AgentId, Common.AgentSession.AgentInformation.IsAUXAgent);
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
                    SendingCurrency = SendingCurrencyCode,
                    ReceivingCurrency = ReceivingCurrencyCode,


                }, JsonRequestBehavior.AllowGet);
            }
            var result = new EstimateFaxingFeeSummary();


            result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                SExchangeRate.GetExchangeRateValue(agentCountry, receiverCountry,
                TransactionTransferMethod.BankDeposit, AgentId, TransactionTransferType.Agent, Common.AgentSession.AgentInformation.IsAUXAgent), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);

            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(agentCountry, receiverCountry, result.FaxingAmount
                 , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, IsReceivingAmount, TransactionTransferMethod.BankDeposit, AgentId, true);



            if (introductoryRateResult != null)
            {

                result = introductoryRateResult;
            }
            //var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
            //    enterAmountData.ExchangeRate, SEstimateFee.GetFaxingCommision(agentCountry));
            var AgentCommission = Common.Common.GetAgentSendingCommission(TransferService.BankDeposit, AgentId, result.FaxingAmount, result.FaxingFee);
            // Rewrite session with additional value 
            enterAmountData.Fee = result.FaxingFee;
            enterAmountData.SendingAmount = result.FaxingAmount;
            enterAmountData.ReceivingAmount = result.ReceivingAmount;
            enterAmountData.TotalAmount = result.TotalAmount;
            enterAmountData.AgentCommission = AgentCommission;
            enterAmountData.ExchangeRate = result.ExchangeRate;
            enterAmountData.ReceivingCountry = receiverCountry;
            enterAmountData.SendingCurrencySymbol = SendingCurrencySymbol;
            enterAmountData.ReceivingCurrencySymbol = RecevingCurrencySymbol;
            enterAmountData.SendingCurrency = SendingCurrencyCode;
            enterAmountData.ReceivingCurrency = ReceivingCurrencyCode;

            _agentBankAccountDepositServices.SetBankDepositAbroadEnterAmount(enterAmountData);
            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount = result.FaxingAmount,
                AgentCommission = AgentCommission,
                ExchangeRate = result.ExchangeRate,
                SendingCurrencySymbol = SendingCurrencySymbol,
                RecevingCurrencySymbol = RecevingCurrencySymbol,
                SendingCurrencyCode = SendingCurrencyCode,
                ReceivingCurrencyCode = ReceivingCurrencyCode,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}