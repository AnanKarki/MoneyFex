using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.MoblieTransferApi.Models;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransferZero.Sdk.Model;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentMobileMoneyTransferController : Controller
    {
        Admin.Services.CommonServices common = new Admin.Services.CommonServices();
        SAgentMobileTransferWallet _sAgentMobileTransferWalletServices = new SAgentMobileTransferWallet();
        SCashPickUpTransferService _cashPickUp = new SCashPickUpTransferService();
        SSenderMobileMoneyTransfer _mobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
        SAgentInformation _sFaxerInfromationServices = null;

        // GET: Agent/AgentMobileMoneyTransfer
        [HttpGet]
        public ActionResult WalletInformation(string AccountNoORPhoneNo = "")
        {
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            Common.FaxerSession.TransferMethod = "otherwallet";

            AgentResult agentResult = new AgentResult();
            string agencyName = agentInfo.Name;
            string agencyMFSCode = agentInfo.AccountNo;
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }

            #region old desin
            var countries = common.GetCountries();
            var identifyCardType = common.GetCardType();

            ViewBag.countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType");

            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();
            vm = _cashPickUp.GetCashPickupInformationViewModel();
            if (AccountNoORPhoneNo != "")
            {
                AccountNoORPhoneNo = Common.Common.IgnoreZero(AccountNoORPhoneNo);
                var result = _cashPickUp.getFaxer(AccountNoORPhoneNo, vm);

                if (result != null)
                {
                    ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType", result.IdType);
                    string FaxerCountryPhoneCode = Common.Common.GetCountryPhoneCode(result.Country);
                    if (result.Id != 0)
                    {
                        ViewBag.AgentResult = agentResult;
                        return View(result);
                    }
                }
                else
                {
                    //agentResult.Message = "Account does not exist";
                    //agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("Invalid", "Account does not exist");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }


            }
            vm.Country = agentInfo.CountryCode;
            ViewBag.AgentResult = agentResult;
            #endregion
            return View(vm);

        }

        [HttpPost]
        public ActionResult WalletInformation([Bind(Include = CashPickupInformationViewModel.BindProperty)] CashPickupInformationViewModel vm)
        {
            #region old design

            //var identifyCardType = common.GetCardType();
            //string agentCountry = Common.AgentSession.AgentInformation.CountryCode;

            //AgentResult agentResult = new AgentResult();
            //var countries = common.GetCountries();
            //ViewBag.countries = new SelectList(countries, "Code", "Name");
            //ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType");
            //vm.SenderCountryCode = Common.Common.GetCountryPhoneCode(vm.IssuingCountry);
            //var CurrentYear = DateTime.Now.Year;
            //if (vm.Country != agentCountry)
            //{
            //    ModelState.AddModelError("InvalidCountry", "This transaction was not from your country, please direct the customer to their respective country.");
            //    return View(vm);
            //}
            //if (vm.DOB == null)
            //{
            //    ModelState.AddModelError("", "Enter Date Of Birth .");
            //    ViewBag.AgentResult = agentResult;
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
            //        //    agentResult.Message = "Sender's Identity card has been expired.";
            //        //    agentResult.Status = ResultStatus.Warning;
            //        ModelState.AddModelError("IDExpired", "Expired ID");
            //        ViewBag.AgentResult = agentResult;
            //        return View(vm);
            //    }

            //    _cashPickUp.SetCashPickupInformationViewModel(vm);
            //    return RedirectToAction("MobileMoneyTransferEnterAmount");
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

        [HttpGet]
        public ActionResult ReceiverDetailsInformation(string Country = "")
        {
            var paymentInfo = _sAgentMobileTransferWalletServices.GetMobileMoneyTransferEnterAmountViewModel();
            Country = Common.Common.GetCountryCodeByCurrency(paymentInfo.ReceivingCurrencyCode);
            ReceiverDetailsInformationViewModel vm = _sAgentMobileTransferWalletServices.GetReceiverDetailsInformation();
            int AgentId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            var recentlyPaidNumbers = _mobileMoneyTransferServices.GetRecentlyPaidNumbersForAgent(AgentId, DB.Module.Agent, Country);
            var wallets = _mobileMoneyTransferServices.GetWallets().Where(x => x.CountryCode == Country).ToList();
            var countries = common.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name", Country);
            ViewBag.MobileWalletProviders = new SelectList(wallets, "Id", "Name", Country);
            ViewBag.PreviousMobileNumbers = new SelectList(recentlyPaidNumbers, "Code", "Name", Country);
            vm.MobileCode = Common.Common.GetCountryPhoneCode(Country);
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            var Data = _kiiPaytrasferServices.GetCommonEnterAmount();
            if (Data != null && Data.ReceivingAmount != 0 && Data.SendingAmount != 0 && Data.Fee != 0 && Data.ExchangeRate != 0 && Data.TotalAmount != 0)
            {
                vm.Country = Data.ReceivingCountryCode;
                vm.MobileCode = Common.Common.GetCountryPhoneCode(Data.ReceivingCountryCode);
                return View(vm);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult ReceiverDetailsInformation([Bind(Include = ReceiverDetailsInformationViewModel.BindProperty)] ReceiverDetailsInformationViewModel model)
        {
            int AgentId = Common.AgentSession.LoggedUser.PayingAgentStaffId;

            var recentlyPaidNumbers = _mobileMoneyTransferServices.GetRecentlyPaidNumbersForAgent(AgentId, DB.Module.Agent, model.Country);
            var wallets = _mobileMoneyTransferServices.GetWallets().Where(x => x.CountryCode == model.Country).ToList();
            var countries = common.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            ViewBag.MobileWalletProviders = new SelectList(wallets, "Id", "Name");
            ViewBag.PreviousMobileNumbers = new SelectList(recentlyPaidNumbers, "Code", "Name");
            model.MobileCode = Common.Common.GetCountryPhoneCode(model.Country);
            if (model.ReasonForTransfer == PORTAL.Models.ReasonForTransfer.Non)
            {
                ModelState.AddModelError("Reason", "Select Reason");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                bool IsValidReceiver = Common.Common.IsValidBankDepositReceiver(model.MobileNumber, Service.MobileWallet);

                if (IsValidReceiver == false)
                {
                    ModelState.AddModelError("InvalidReceiver", " Receiver is banned");
                    return View(model);
                }

                string CountryName = Common.Common.GetCountryName(model.Country);


                var paymentInfo = _sAgentMobileTransferWalletServices.GetMobileMoneyTransferEnterAmountViewModel();

                var agentInfo = Common.AgentSession.AgentInformation;
                var transactionTransferType = TransactionTransferType.Agent;
                if (agentInfo.IsAUXAgent)
                {
                    transactionTransferType = TransactionTransferType.AuxAgent;
                }
                var Apiservice = Common.Common.GetApiservice(agentInfo.CountryCode, model.Country,
                     paymentInfo.SendingAmount, TransactionTransferMethod.OtherWallet, transactionTransferType, agentInfo.Id);
                if (Apiservice == null)
                {
                    ModelState.AddModelError("ServiceNotAvialable", "Service Not Avialable");
                    return View(model);
                }


                SmsApi smsApi = new SmsApi();
                var IsValidMobileNo = smsApi.IsValidMobileNo(model.MobileCode + "" + model.MobileNumber);

                if (IsValidMobileNo == false)
                {
                    ModelState.AddModelError("", "Enter Valid Number");
                    return View(model);
                }
                var IsValidAccount = _mobileMoneyTransferServices.IsValidMobileAccount(new SenderMobileMoneyTransferVM()
                {

                    CountryCode = model.Country,
                    WalletId = model.MobileWalletProvider,
                    MobileNumber = model.MobileNumber,
                    ReceiverName = model.ReceiverName,
                    CountryPhoneCode = Common.Common.GetCountryPhoneCode(model.Country),
                }, paymentInfo.SendingAmount, Common.AgentSession.AgentInformation.CountryCode, TransactionTransferType.Agent);

                if (IsValidAccount.Data == false)
                {

                    ModelState.AddModelError("", IsValidAccount.Message);
                    return View(model);

                }



                _sAgentMobileTransferWalletServices.SetReceiverDetailsInformation(model);

                return RedirectToAction("TransactionSummary");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult MobileMoneyTransferEnterAmount()
        {
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();
            var agentInfo = Common.AgentSession.AgentInformation;
            int AgentId = agentInfo.Id;
            ViewBag.AgentId = agentInfo.Id;
            ViewBag.IsAuxAgent = agentInfo.IsAUXAgent;
            ViewBag.TransferType = 2;
            if (agentInfo.IsAUXAgent)
            {
                ViewBag.TransferType = 4;
            }
            MobileMoneyTransferEnterAmountViewModel vm = new MobileMoneyTransferEnterAmountViewModel();
            var agentCountry = getAgentCountryCode(AgentId);
            var paymentInfo = _sAgentMobileTransferWalletServices.GetMobileMoneyTransferEnterAmountViewModel();
            if (paymentInfo != null)
            {
                vm.SendingAmount = paymentInfo.SendingAmount;
                vm.ReceivingAmount = paymentInfo.ReceivingAmount;
                vm.ReceivingCurrencyCode = paymentInfo.ReceivingCurrencyCode == null ? "NGN" : paymentInfo.ReceivingCurrencyCode;
                vm.SendingCurrencyCode = Common.Common.GetCountryCurrency(agentCountry);
                vm.SendingCountry = agentCountry;
                vm.Fee = paymentInfo.Fee;
                vm.IsConfirm = paymentInfo.IsConfirm;
                vm.AgentCommission = paymentInfo.AgentCommission;
                vm.TotalAmount = paymentInfo.TotalAmount;
            }
            return View(vm);

        }
        [HttpPost]
        public ActionResult MobileMoneyTransferEnterAmount([Bind(Include = MobileMoneyTransferEnterAmountViewModel.BindProperty)] MobileMoneyTransferEnterAmountViewModel Vm)
        {
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();
            //int AgentId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            //string agentCountry = Common.AgentSession.AgentInformation.CountryCode;
            //var sender = _cashPickUp.GetCashPickupInformationViewModel();
            //var mobileTransfer = _sAgentMobileTransferWalletServices.GetReceiverDetailsInformation();
            //var paymentInfo = _sAgentMobileTransferWalletServices.GetMobileMoneyTransferEnterAmountViewModel();
            var agentInfo = Common.AgentSession.AgentInformation;
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();
            Vm.ReceivingCurrencyCode = paymentInfo.ReceivingCurrency;
            Vm.SendingCurrencyCode = paymentInfo.SendingCurrency;
            Vm.SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol;
            Vm.ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol;
            Vm.SendingCountry = paymentInfo.SendingCountryCode;
            Vm.ReceivingCountry = paymentInfo.ReceivingCountryCode;
            Vm.SendingAmount = paymentInfo.SendingAmount;
            Vm.ReceivingAmount = paymentInfo.ReceivingAmount;
            ViewBag.AgentId = agentInfo.Id;
            ViewBag.IsAuxAgent = agentInfo.IsAUXAgent;
            ViewBag.TransferType = TransactionTransferType.Agent;
            if (agentInfo.IsAUXAgent)
            {
                ViewBag.TransferType = TransactionTransferType.AuxAgent;
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
                _sAgentMobileTransferWalletServices.SetMobileMoneyTransferEnterAmountViewModel(Vm);
                return RedirectToAction("ReceiverDetailsInformation", "AgentMobileMoneyTransfer");
            }
            else
            {
                ModelState.AddModelError("IsConfirm", "Please Confirm.");
                return View(Vm);
            }

        }

        public ActionResult TransactionSummary()
        {
            var paymentInfo = _sAgentMobileTransferWalletServices.GetMobileMoneyTransferEnterAmountViewModel();

            var receiverinfo = _sAgentMobileTransferWalletServices.GetReceiverDetailsInformation();

            string ReceiverFirstname = receiverinfo.ReceiverName.Split(' ')[0];
            CommonTransactionSummaryViewModel vm = new CommonTransactionSummaryViewModel()
            {
                Fee = paymentInfo.Fee,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                ReceivingCurrecyCode = paymentInfo.ReceivingCurrencyCode,
                ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCurrencyCode = paymentInfo.SendingCurrencyCode,
                SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol,
                ReceiverFirstName = ReceiverFirstname,
                TotalAmount = paymentInfo.Fee + paymentInfo.SendingAmount
            };

            return View(vm);
        }
        [HttpPost]
        public ActionResult TransactionSummary([Bind(Include = CommonTransactionSummaryViewModel.BindProperty)]CommonTransactionSummaryViewModel vm)
        {
            _sAgentMobileTransferWalletServices.SetTransactionSummary();


            TransferForAllAgentServices transferForAllAgentServices = new TransferForAllAgentServices();
            int TransactionId = transferForAllAgentServices.CompleteTransaction(TransactionTransferMethod.OtherWallet);
            //int TransactionId = _sAgentMobileTransferWalletServices.TransactionCompleted();

            var agentInfo = Common.AgentSession.AgentInformation;


            int senderId = _sAgentMobileTransferWalletServices.GetSenderFromMobileWallet(TransactionId);
            Common.AgentSession.SenderId = senderId;
            var SenderDocumentApprovalStatus = Common.Common.GetSenderIdentificationStatus(senderId);

            switch (SenderDocumentApprovalStatus)
            {
                case null:
                    return RedirectToAction("IdentityVerification", "AgentBankAccountDeposit");
                case DocumentApprovalStatus.Approved:
                    break;
                case DocumentApprovalStatus.Disapproved:
                    return RedirectToAction("IdentityVerification", "AgentBankAccountDeposit");
                case DocumentApprovalStatus.InProgress:
                    return RedirectToAction("IdentityVerificationInProgress", "AgentBankAccountDeposit");
                default:
                    break;
            }

            return RedirectToAction("MobileMoneyTransferSuccess", "AgentMobileMoneyTransfer", new { Id = TransactionId });
        }

        [HttpGet]
        public ActionResult MobileMoneyTransferSuccess(int Id)
        {
            var result = _sAgentMobileTransferWalletServices.GetMobileMoneyTransferEnterAmountViewModel();
            MobileMoneyTransferSuccessViewModel Vm = new MobileMoneyTransferSuccessViewModel()
            {
                SendingAmount = result.SendingAmount,
                SendingCurrency = result.SendingCurrencySymbol,
                WalletName = result.ReceiverName,
                TransactionId = Id,
            };
            AgentCommonServices cs = new AgentCommonServices();
            //cs.ClearOtherMobileWallerTransfer();
            return View(Vm);
        }



        public void PrintReceipt(int TransactionId)
        {

            var CarduserInformation = _mobileMoneyTransferServices.MobileMoneyTransferInfo(TransactionId);

            var agentInformation = Common.AgentSession.AgentInformation;

            var ReceiverInfo = _sAgentMobileTransferWalletServices.GetReceiverDetailsInformation(); ;

            var senderInfo = _cashPickUp.GetCashPickupInformationViewModel();

            // Need Changes Here  Because the sender info is conditional now 
            //the Kiipay Personal might or might not be associated with the faxer

            //var faxerInformation = dbContext.FaxerInformation.Where(x => x.Id == CarduserInformation.FaxerId).FirstOrDefault();

            string CardUserCurrency = Common.Common.GetCountryCurrency(CarduserInformation.SendingCountry);
            string SenderPhoneCode = Common.Common.GetCountryPhoneCode(CarduserInformation.SendingCountry);
            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(ReceiverInfo.Country);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardUserReceiverReceipt/KiiPayWallet?MFReceiptNumber=" + CarduserInformation.ReceiptNo +
             "&TransactionDate=" + CarduserInformation.TransactionDate.ToString("dd/MM/yyyy") +
             "&TransactionTime=" + CarduserInformation.TransactionDate.ToString("HH:mm") +
             "&SenderFullName=" + senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName +
             "&SenderEmail=" + senderInfo.Email +
             "&SenderTelephone=" + SenderPhoneCode + " " + senderInfo.MobileNo +
             "&SenderDOB=" + senderInfo.DOB.ToFormatedString("dd/MM/yyyy") +
             "&ReceiverFullName=" + ReceiverInfo.ReceiverName +

             "&ReceiverTelephone=" + ReceiverPhoneCode + " " + ReceiverInfo.MobileNumber +
             "&AgentName=" + agentInformation.Name +
             "&AgentAcountNumber=" + agentInformation.AccountNo +
             "&StaffName=" + agentInformation.ContactPerson +
             "&AgentCity=" + agentInformation.City +
             "&AgentCountry=" + Common.Common.GetCountryName(agentInformation.CountryCode) +
             "&TotalAmount=" + CardUserCurrency + "" + CarduserInformation.TotalAmount +
             "&Fee=" + CardUserCurrency + "" + CarduserInformation.Fee +
             "&SendingAmount=" + CardUserCurrency + "" + CarduserInformation.SendingAmount +
             "&ExchangeRate=" + CarduserInformation.ExchangeRate +
             "&SendingCurrency= " + Common.Common.GetCountryCurrency(senderInfo.Country) +
             "&ReceivingCurrency= " + Common.Common.GetCountryCurrency(ReceiverInfo.Country) +
             "&ReceivingAmount= " + CarduserInformation.ReceivingAmount +
             "&PaymentMethod= " + CarduserInformation.PaymentType;
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
        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount, bool IsReceivingAmount, string receiverCountry)
        {

            int AgentId = Common.AgentSession.AgentInformation.Id;

            var agentCountry = getAgentCountryCode(AgentId);

            var enterAmountData = _sAgentMobileTransferWalletServices.GetMobileMoneyTransferEnterAmountViewModel();

            var ReceivingCountry = receiverCountry;
            var SendingCurrencySymbol = Common.Common.GetCurrencySymbol(agentCountry);
            var SendingCurrencyCode = Common.Common.GetCurrencyCode(agentCountry);
            var RecevingCurrencySymbol = Common.Common.GetCurrencySymbol(receiverCountry);
            var ReceivingCurrencyCode = Common.Common.GetCurrencyCode(receiverCountry);
            if (IsReceivingAmount)
            {


                SendingAmount = ReceivingAmount;
            }


            //var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
            //    enterAmountData.ExchangeRate, SEstimateFee.GetFaxingCommision(agentCountry));

            //var AgentCommission = Common.Common.GetAgentSendingCommission(TransferService.OtherWallet, AgentId, result.FaxingAmount, result.FaxingFee);

            //// Rewrite session with additional value 
            //enterAmountData.Fee = result.FaxingFee;
            //enterAmountData.SendingAmount = result.FaxingAmount;
            //enterAmountData.ReceivingAmount = result.ReceivingAmount;
            //enterAmountData.TotalAmount = result.TotalAmount;
            //enterAmountData.AgentCommission = AgentCommission;

            //_sAgentMobileTransferWalletServices.SetMobileMoneyTransferEnterAmountViewModel(enterAmountData);
            //return Json(new
            //{
            //    Fee = result.FaxingFee,
            //    TotalAmount = result.TotalAmount,
            //    ReceivingAmount = result.ReceivingAmount,
            //    SendingAmount = result.FaxingAmount,
            //    AgentCommission = AgentCommission

            //}, JsonRequestBehavior.AllowGet);

            var feeInfo = SEstimateFee.GetTransferFee(agentCountry, ReceivingCountry, TransactionTransferMethod.OtherWallet, SendingAmount,
                TransactionTransferType.Agent, Common.AgentSession.LoggedUser.Id, Common.AgentSession.AgentInformation.IsAUXAgent);
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
                    ReceivingCurrencyCode = ReceivingCurrencyCode,

                }, JsonRequestBehavior.AllowGet);
            }
            var result = new EstimateFaxingFeeSummary();

            result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                SExchangeRate.GetExchangeRateValue(agentCountry, ReceivingCountry, TransactionTransferMethod.OtherWallet, AgentId,
                TransactionTransferType.Agent, Common.AgentSession.AgentInformation.IsAUXAgent),
                feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(agentCountry, ReceivingCountry, result.FaxingAmount
                , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, IsReceivingAmount, TransactionTransferMethod.OtherWallet, AgentId, true);

            if (introductoryRateResult != null)
            {

                result = introductoryRateResult;
            }


            //var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
            //    enterAmountData.ExchangeRate, SEstimateFee.GetFaxingCommision(agentCountry));

            var AgentCommission = Common.Common.GetAgentSendingCommission(TransferService.OtherWallet, AgentId, result.FaxingAmount, result.FaxingFee);


            // Rewrite session with additional value 
            enterAmountData.Fee = result.FaxingFee;
            enterAmountData.SendingAmount = result.FaxingAmount;
            enterAmountData.ReceivingAmount = result.ReceivingAmount;
            enterAmountData.TotalAmount = result.TotalAmount;
            enterAmountData.AgentCommission = AgentCommission;
            enterAmountData.ExchangeRate = result.ExchangeRate;

            _sAgentMobileTransferWalletServices.SetMobileMoneyTransferEnterAmountViewModel(enterAmountData);

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
        public JsonResult GetRecentlyPaidNumberInfo(string mobileNumber)
        {
            var list = _mobileMoneyTransferServices.list().Data.Where(x => x.PaidToMobileNo == mobileNumber).FirstOrDefault();
            if (list != null)
            {
                return Json(new
                {
                    ReceiverName = list.ReceiverName,
                    MobileNumber = mobileNumber,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    ReceiverName = "",
                    MobileNumber = "",
                }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult GetCountryPhone(string countryCode)
        {
            if (!string.IsNullOrEmpty(countryCode))
            {
                var phoneCode = Common.Common.GetCountryPhoneCode(countryCode);
                return Json(new
                {
                    countryPhoneCode = phoneCode,
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                countryPhoneCode = "",
            }, JsonRequestBehavior.AllowGet);
        }
    }
}