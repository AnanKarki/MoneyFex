using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentKiiPayWalletTransferController : Controller
    {
        Admin.Services.CommonServices common = new Admin.Services.CommonServices();
        SAgentKiiPayWalletTransferServices _kiiPayWalletTransfer = new SAgentKiiPayWalletTransferServices();
        AgentCommonServices _commonServices = new AgentCommonServices();
        // GET: Agent/AgentKiiPayWalletTransfer
        [HttpGet]
        public ActionResult SendMoneToKiiPayWallet(string AccountNoORPhoneNo = "")
        {
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            Common.FaxerSession.TransferMethod = "kiipaywallet";
            AgentResult agentResult = new AgentResult();
            string agencyName = agentInfo.Name;
            string agencyMFSCode = agentInfo.AccountNo;
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }

            #region old design
            var countries = common.GetCountries();
            var identifyCardType = common.GetCardType();

            ViewBag.countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType");
            SendMoneToKiiPayWalletViewModel vm = new SendMoneToKiiPayWalletViewModel();
            if (AccountNoORPhoneNo != "")
            {

                var result = _kiiPayWalletTransfer.getFaxerInfo(AccountNoORPhoneNo, vm);

                if (result != null)
                {
                    ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType", result.IdType);
                    ViewBag.countries = new SelectList(countries, "Code", "Name", result.Country);
                    string FaxerCountryPhoneCode = Common.Common.GetCountryPhoneCode(result.Country);
                    if (result.Id != 0)
                    {

                        ViewBag.AgentResult = agentResult;
                        return View(result);
                    }
                }
                else
                {
                    //    agentResult.Message = "Account does not exist";
                    //    agentResult.Status = ResultStatus.Warning;
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
        public ActionResult SendMoneToKiiPayWallet([Bind(Include = SendMoneToKiiPayWalletViewModel.BindProperty)]SendMoneToKiiPayWalletViewModel Vm)
        {
            #region old design
            //AgentResult agentResult = new AgentResult();
            //string agentCountry = Common.AgentSession.AgentInformation.CountryCode;

            //var countries = common.GetCountries();
            //var identifyCardType = common.GetCardType();

            //ViewBag.countries = new SelectList(countries, "Code", "Name");
            //ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType");
            //var CurrentYear = DateTime.Now.Year;
            //if (Vm.Country != agentCountry)
            //{
            //    ModelState.AddModelError("InvalidCountry", "This transaction was not from your country, please direct the customer to their respective country.");
            //    ViewBag.AgentResult = agentResult;
            //    return View(Vm);
            //}
            //if (Vm.DOB == null)
            //{
            //    ModelState.AddModelError("", "Enter Date Of Birth .");
            //    ViewBag.AgentResult = agentResult;
            //    return View(Vm);
            //}
            //var DOB = Vm.DOB;
            //DateTime date = Convert.ToDateTime(DOB);
            //var DOByear = date.Year;
            //var Age = CurrentYear - DOByear;
            //Vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Vm.IssuingCountry);
            //if (ModelState.IsValid)
            //{
            //    if (Age <= 18)
            //    {
            //        //    agentResult.Message = "Sender's should be more than 18 years to do the transaction.";
            //        //    agentResult.Status = ResultStatus.Warning;
            //        ModelState.AddModelError("InvalidAge", "Sender's should be more than 18 years to do the transaction.");
            //        ViewBag.AgentResult = agentResult;
            //        return View(Vm);
            //    }
            //    if (Vm.ExpiryDate < DateTime.Now)
            //    {
            //        //agentResult.Message = "Sender's Identity card has been expired.";
            //        //agentResult.Status = ResultStatus.Warning;

            //        ModelState.AddModelError("IDExpired", "Expired ID");

            //        ViewBag.AgentResult = agentResult;
            //        return View(Vm);
            //    }
            //    var result = _kiiPayWalletTransfer.getFaxerInfo(AccountNoORPhoneNo, Vm);
            //    _kiiPayWalletTransfer.SetSendMoneToKiiPayWalletViewModel(Vm);
            //    return RedirectToAction("SendMoneyToKiiPayEnterAmount");
            //}
            //ViewBag.AgentResult = agentResult;
            //return View(Vm);
            #endregion

            if (string.IsNullOrEmpty(Vm.Search))
            {
                ModelState.AddModelError("Search", "Enter telePhone/email");
                return View(Vm);
            }
            var AccountNoORPhoneNo = Common.Common.IgnoreZero(Vm.Search);
            var SenderDetails = _kiiPayWalletTransfer.getFaxerInfo(AccountNoORPhoneNo, Vm);
            if (SenderDetails == null)
            {
                ModelState.AddModelError("InvalidAccount", "Account does not exist");
                return View(Vm);
            }
            else
            {

                SCashPickUpTransferService _cashPickUp = new SCashPickUpTransferService();
                CashPickupInformationViewModel model = new CashPickupInformationViewModel();

                model.Search = Vm.Search;

                var senderInfo = _cashPickUp.getFaxer(AccountNoORPhoneNo, model);
                _cashPickUp.SetCashPickupInformationViewModel(senderInfo);

                _kiiPayWalletTransfer.SetSendMoneToKiiPayWalletViewModel(SenderDetails);


                return RedirectToAction("SenderDetails", "AgentBankAccountDeposit");
            }
        }

        [HttpGet]
        public ActionResult KiiPayReceiverDetailsInformation(string Country = "")
        {
            AgentResult agentResult = new AgentResult();
            var paymentInfo = _kiiPayWalletTransfer.GetKiiPayEnterAmount();
            Country = Common.Common.GetCountryCodeByCurrency(paymentInfo.ReceivingCurrency);
            var countries = common.GetCountries();
            ViewBag.countries = new SelectList(countries, "Code", "Name", Country);
            SetViewBagForPhoneNumbers(Country);
            ViewBag.AgentResult = agentResult;

            KiiPayReceiverDetailsInformationViewModel vm = _kiiPayWalletTransfer.GetKiiPayReceiverDetailsInformationViewModel();
            vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Country);
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            var Data = _kiiPaytrasferServices.GetCommonEnterAmount();
            if (Data != null && Data.ReceivingAmount != 0 && Data.SendingAmount != 0 && Data.Fee != 0 && Data.ExchangeRate != 0 && Data.TotalAmount != 0)
            {
                vm.Country = Data.ReceivingCountryCode;
                vm.CountryPhoneCode = Common.Common.GetCountryPhoneCode(Data.ReceivingCountryCode);
                return View(vm);
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult KiiPayReceiverDetailsInformation([Bind(Include = KiiPayReceiverDetailsInformationViewModel.BindProperty)]KiiPayReceiverDetailsInformationViewModel Vm, string Country = "")
        {
            AgentResult agentResult = new AgentResult();
            var countries = common.GetCountries();
            ViewBag.countries = new SelectList(countries, "Code", "Name");

            SetViewBagForPhoneNumbers(Country);
            if (ModelState.IsValid && Vm.ReasonForTransfer != ReasonForTransfer.Non)
            {
                var result = _kiiPayWalletTransfer.getReceiverDetails(Vm.MobileNo);
                if (result == null)
                {
                    //    agentResult.Message = "Mobile Number Doesnot exist";
                    //    agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("InvalidNumber", "Mobile Number Doesnot exist");
                    ViewBag.AgentResult = agentResult;
                    return View(Vm);
                }

                bool IsValidReceiver = Common.Common.IsValidBankDepositReceiver(Vm.MobileNo, Service.KiiPayWallet);

                if (IsValidReceiver == false)
                {
                    ModelState.AddModelError("InvalidReceiver", " Receiver is banned");
                    ViewBag.AgentResult = agentResult;
                    return View(Vm);
                }
                _kiiPayWalletTransfer.SetKiiPayReceiverDetailsInformationViewModel(Vm);
                return RedirectToAction("TransactionSummary");
            }
            if (Vm.ReasonForTransfer == PORTAL.Models.ReasonForTransfer.Non)
            {
                ModelState.AddModelError("Reason", "Select Reason");
            }
            ViewBag.AgentResult = agentResult;
            return View(Vm);
        }

        private void SetViewBagForPhoneNumbers(string Country)
        {
            var PayingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId;

            var phoneNumbers = _kiiPayWalletTransfer.getRecentNumbers(Country, PayingAgentStaffId);
            ViewBag.PhoneNumbers = new SelectList(phoneNumbers, "Code", "Name");
        }

        [HttpGet]
        public ActionResult SendMoneyToKiiPayEnterAmount()

        {
            AgentResult agentResult = new AgentResult();
            SendMoneyToKiiPayEnterAmountViewModel Vm = new SendMoneyToKiiPayEnterAmountViewModel();

            var paymentInfo = _kiiPayWalletTransfer.GetKiiPayEnterAmount();
            if (paymentInfo != null)
            {
                Vm.SendingAmount = paymentInfo.SendingAmount;
                Vm.ReceivingAmount = paymentInfo.ReceivingAmount;
                Vm.ReceivingCurrency = paymentInfo.ReceivingCurrency;
                Vm.SendingCurrency = paymentInfo.SendingCurrency;
                Vm.Fee = paymentInfo.Fee;
                Vm.PaymentReference = paymentInfo.PaymentReference;
                Vm.TheyReceive = paymentInfo.TheyReceive;
                Vm.AgentCommission = paymentInfo.AgentCommission;
                Vm.TotalAmount = paymentInfo.TotalAmount;

                ViewBag.AgentResult = agentResult;
                return View(Vm);
            }

            //string agentCountry = Common.AgentSession.AgentInformation.CountryCode;


            ////string SendingCountry = Common.AgentSession.SendMoneToKiiPayWalletViewModel.Country;
            ////string ReceivingCountry = Common.AgentSession.KiiPayReceiverDetailsInformationViewModel.Country;

            //string SendingCountry = _kiiPayWalletTransfer.GetSendMoneToKiiPayWalletViewModel().Country;
            //string ReceivingCountry = _kiiPayWalletTransfer.GetKiiPayReceiverDetailsInformationViewModel().Country;

            //Vm.ExchangeRate = SExchangeRate.GetExchangeRateValue(agentCountry, ReceivingCountry
            //    , TransactionTransferMethod.KiiPayWallet, Common.AgentSession.AgentInformation.Id, TransactionTransferType.Agent);
            //Vm.SendingCurrency = Common.Common.GetCountryCurrency(agentCountry);
            //Vm.ReceivingCurrency = Common.Common.GetCountryCurrency(ReceivingCountry);
            //Vm.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(agentCountry);
            //Vm.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(ReceivingCountry);
            //Vm.WalletName = Common.AgentSession.KiiPayReceiverDetailsInformationViewModel.ReceiverFullName;

            //_commonServices.SetAgentPaymentSummarySession(ReceivingCountry, TransactionTransferMethod.KiiPayWallet);
            //_kiiPayWalletTransfer.SetKiiPayEnterAmount(Vm);

            ViewBag.AgentResult = agentResult;
            return View(Vm);

        }
        [HttpPost]
        public ActionResult SendMoneyToKiiPayEnterAmount([Bind(Include = SendMoneyToKiiPayEnterAmountViewModel.BindProperty)]SendMoneyToKiiPayEnterAmountViewModel Vm)
        {
            AgentResult agentResult = new AgentResult();

            if (ModelState.IsValid)
            {
                if (Vm.SendingAmount == 0)
                {
                    ModelState.AddModelError("SendingAmount", "Please Enter Sending Amount ");
                    ViewBag.AgentResult = agentResult;
                    return View(Vm);
                }
                if (Vm.ReceivingAmount == 0)
                {
                    ModelState.AddModelError("RecevingAmount", "Please Enter Sending Amount ");
                    ViewBag.AgentResult = agentResult;
                    return View(Vm);
                }

                if (Vm.IsConfirm == false)
                {
                    ModelState.AddModelError("IsConfirm", "Please Confirm.");
                    ViewBag.AgentResult = agentResult;
                    return View(Vm);
                }
                var agentInfo = Common.AgentSession.AgentInformation;
                if (agentInfo.IsAUXAgent)
                {
                    AgentCommonServices _AgentCommonServices = new AgentCommonServices();
                    decimal AUXAgentAccountBalance = _AgentCommonServices.getAuxAgentAccountBalance(agentInfo.Id, Common.AgentSession.LoggedUser.PayingAgentStaffId);
                    if (AUXAgentAccountBalance <= Vm.TotalAmount)
                    {
                        ModelState.AddModelError("Insufficientfund", "Insufficient Fund");
                        ViewBag.AgentResult = agentResult;
                        return View(Vm);
                    }
                }
                _kiiPayWalletTransfer.SetKiiPayEnterAmount(Vm);
                ViewBag.AgentResult = agentResult;
                return RedirectToAction("KiiPayReceiverDetailsInformation");
            }

            ViewBag.AgentResult = agentResult;
            return View(Vm);

        }


        [HttpGet]
        public ActionResult TransactionSummary()
        {
            var paymentInfo = _kiiPayWalletTransfer.GetKiiPayEnterAmount();

            var receiverinfo = _kiiPayWalletTransfer.GetKiiPayReceiverDetailsInformationViewModel();

            string ReceiverFirstname = receiverinfo.ReceiverFullName.Split(' ')[0];
            CommonTransactionSummaryViewModel Vm = new CommonTransactionSummaryViewModel()
            {

                Fee = paymentInfo.Fee,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                ReceivingCurrecyCode = paymentInfo.ReceivingCurrency,
                ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCurrencyCode = paymentInfo.SendingCurrency,
                SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol,
                ReceiverFirstName = ReceiverFirstname
            };

            return View(Vm);
        }
        [HttpPost]
        public ActionResult TransactionSummary([Bind(Include = CommonTransactionSummaryViewModel.BindProperty)] CommonTransactionSummaryViewModel vm)
        {
            var PaymentInfo = _kiiPayWalletTransfer.GetKiiPayEnterAmount();
            var TopUpSomeoneElseCardTransaction = _kiiPayWalletTransfer.TopUpSomeoneElseCardTransaction(PaymentInfo);
            if (TopUpSomeoneElseCardTransaction != null)
            {
                var agentInfo = Common.AgentSession.AgentInformation;

                if (agentInfo.IsAUXAgent)
                {
                    Common.AgentSession.SenderId = TopUpSomeoneElseCardTransaction.FaxerId;
                    var SenderDocumentApprovalStatus = Common.Common.GetSenderIdentificationStatus(TopUpSomeoneElseCardTransaction.FaxerId);

                    switch (SenderDocumentApprovalStatus)
                    {
                        case null:
                            return RedirectToAction("IdentityVerification", "AgentBankAccountDeposit");
                            break;
                        case DocumentApprovalStatus.Approved:
                            break;
                        case DocumentApprovalStatus.Disapproved:
                            return RedirectToAction("IdentityVerification", "AgentBankAccountDeposit");
                            break;
                        case DocumentApprovalStatus.InProgress:
                            return RedirectToAction("IdentityVerificationInProgress", "AgentBankAccountDeposit");
                            break;
                        default:
                            break;
                    }
                }
                return RedirectToAction("SendMoneyToKiiPaySuccess", "AgentKiiPayWalletTransfer", new { Id = TopUpSomeoneElseCardTransaction.Id });
            }
            else
            {
                AgentResult agentResult = new AgentResult();

                agentResult.Message = "Error Occured Wile Doing Transaction";
                agentResult.Status = ResultStatus.Warning;
                ViewBag.AgentResult = agentResult;
                return View(vm);
            }

        }

        [HttpGet]
        public ActionResult SendMoneyToKiiPaySuccess(int Id)
        {
            SendMoneyToKiiPaySuccessViewModel Vm = new SendMoneyToKiiPaySuccessViewModel();
            //string SendingCountry = Common.AgentSession.SendMoneToKiiPayWalletViewModel.Country;
            //Vm.WalletName = Common.AgentSession.KiiPayReceiverDetailsInformationViewModel.ReceiverFullName;
            string SendingCountry = _kiiPayWalletTransfer.GetSendMoneToKiiPayWalletViewModel().Country;
            Vm.SendingCurrency = Common.Common.GetCurrencySymbol(SendingCountry);


            Vm.SendingAmount = _kiiPayWalletTransfer.GetKiiPayEnterAmount().SendingAmount;

            Vm.WalletName = _kiiPayWalletTransfer.GetKiiPayReceiverDetailsInformationViewModel().ReceiverFullName;
            Vm.TransactionId = Id;
            // _commonServices.ClearAgentKiiPayTransferSession();


            return View(Vm);
        }




        public void PrintReceipt(int TransactionId)
        {

            var CarduserInformation = _kiiPayWalletTransfer.TopUpSomeoneElseCardTransactionInfo(TransactionId);
            var KiiPayPersonalInfo = _kiiPayWalletTransfer.kiiPayPersonalWalletInfo(TransactionId);
            var agentInformation = Common.AgentSession.AgentInformation;
            var senderInfo = _kiiPayWalletTransfer.GetSendMoneToKiiPayWalletViewModel();
            var ReceiverInfo = _kiiPayWalletTransfer.GetKiiPayReceiverDetailsInformationViewModel();

            // Need Changes Here  Because the sender info is conditional now 
            //the Kiipay Personal might or might not be associated with the faxer

            //var faxerInformation = dbContext.FaxerInformation.Where(x => x.Id == CarduserInformation.FaxerId).FirstOrDefault();

            string CardUserCurrency = Common.Common.GetCountryCurrency(CarduserInformation.SendingCountry);
            string SenderPhoneCode = Common.Common.GetCountryPhoneCode(CarduserInformation.SendingCountry);
            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(ReceiverInfo.Country);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardUserReceiverReceipt/KiiPayWallet?MFReceiptNumber=" + CarduserInformation.ReceiptNumber +
              "&TransactionDate=" + CarduserInformation.TransactionDate.ToString("dd/MM/yyyy") +
              "&TransactionTime=" + CarduserInformation.TransactionDate.ToString("HH:mm") +
              "&SenderFullName=" + senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName +
              "&SenderEmail=" + senderInfo.Email +
              "&SenderTelephone=" + SenderPhoneCode + " " + senderInfo.MobileNo +
              "&SenderDOB=" + senderInfo.DOB.ToFormatedString("dd/MM/yyyy") +
              "&ReceiverFullName=" + ReceiverInfo.ReceiverFullName +

              "&ReceiverTelephone=" + ReceiverPhoneCode + " " + ReceiverInfo.MobileNo +
              "&AgentName=" + agentInformation.Name +
              "&AgentAcountNumber=" + agentInformation.AccountNo +
              "&StaffName=" + agentInformation.ContactPerson +
              "&AgentCity=" + agentInformation.City +
              "&AgentCountry=" + Common.Common.GetCountryName(agentInformation.CountryCode) +
              "&TotalAmount=" + CardUserCurrency + " " + CarduserInformation.TotalAmount +
              "&Fee=" + CardUserCurrency + " " + CarduserInformation.FaxingFee +
              "&SendingAmount=" + CardUserCurrency + " " + CarduserInformation.FaxingAmount +
              "&ExchangeRate=" + CarduserInformation.ExchangeRate +
              "&SendingCurrency= " + Common.Common.GetCountryCurrency(senderInfo.Country) +
              "&ReceivingCurrency= " + Common.Common.GetCountryCurrency(ReceiverInfo.Country) +
              "&ReceivingAmount=" + CarduserInformation.RecievingAmount +
              "&PaymentMethod=" + CarduserInformation.PaymentMethod; ;


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



        public JsonResult GetCountryPhoneCode(string CountryCode)
        {

            string PhoneCode = Common.Common.GetCountryPhoneCode(CountryCode);
            return Json(new
            {

                CountryPhoneCode = PhoneCode
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNumberName(string PhoneNumber)
        {
            var receivername = _kiiPayWalletTransfer.getReceiverDetails(PhoneNumber).FirstName;
            if (receivername == null)
            {
                return Json(new
                {
                    phoneTextBox = "",
                    ReceiverFullName = "",
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    phoneTextBox = PhoneNumber,
                    ReceiverFullName = receivername
                }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount, bool IsReceivingAmount, string receiverCountry)
        {

            var paymentSummary = _kiiPayWalletTransfer.GetKiiPayEnterAmount();
            int AgentId = Common.AgentSession.LoggedUser.Id;

            var senderInfo = _kiiPayWalletTransfer.GetSendMoneToKiiPayWalletViewModel();
            string agentCountry = Common.AgentSession.AgentInformation.CountryCode;
            string ReceivingCountry = receiverCountry;
            var SendingCurrencySymbol = Common.Common.GetCurrencySymbol(agentCountry);
            var SendingCurrencyCode = Common.Common.GetCurrencyCode(agentCountry);
            var RecevingCurrencySymbol = Common.Common.GetCurrencySymbol(receiverCountry);
            var ReceivingCurrencyCode = Common.Common.GetCurrencyCode(receiverCountry);

            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }


            //var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
            //    paymentSummary.ExchangeRate, SEstimateFee.GetFaxingCommision(senderInfo.Country));
            //var Agentcommission = Common.Common.GetAgentSendingCommission(TransferService.KiiPayWallet, AgentId, result.FaxingAmount, result.FaxingFee);
            //// Rewrite session with additional value 
            //paymentSummary.Fee = result.FaxingFee;
            //paymentSummary.SendingAmount = result.FaxingAmount;
            //paymentSummary.ReceivingAmount = result.ReceivingAmount;
            //paymentSummary.TotalAmount = result.TotalAmount;
            //paymentSummary.AgentCommission = Agentcommission;
            //_kiiPayWalletTransfer.SetKiiPayEnterAmount(paymentSummary);

            //return Json(new
            //{
            //    Fee = result.FaxingFee,
            //    TotalAmount = result.TotalAmount,
            //    ReceivingAmount = result.ReceivingAmount,
            //    SendingAmount = result.FaxingAmount,
            //    AgentCommission = Agentcommission

            //}, JsonRequestBehavior.AllowGet);





            var feeInfo = SEstimateFee.GetTransferFee(agentCountry, ReceivingCountry, TransactionTransferMethod.KiiPayWallet, SendingAmount, TransactionTransferType.Agent,
                Common.AgentSession.LoggedUser.Id, Common.AgentSession.AgentInformation.IsAUXAgent);
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

            result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                SExchangeRate.GetExchangeRateValue(agentCountry, ReceivingCountry, TransactionTransferMethod.KiiPayWallet, AgentId,
                TransactionTransferType.Agent, Common.AgentSession.AgentInformation.IsAUXAgent), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(agentCountry, ReceivingCountry, result.FaxingAmount
                , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, IsReceivingAmount, TransactionTransferMethod.KiiPayWallet, AgentId, true);

            if (introductoryRateResult != null)
            {

                result = introductoryRateResult;
            }


            //var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
            //    enterAmountData.ExchangeRate, SEstimateFee.GetFaxingCommision(agentCountry));
            var AgentCommission = Common.Common.GetAgentSendingCommission(TransferService.KiiPayWallet, AgentId, result.FaxingAmount, result.FaxingFee);
            // Rewrite session with additional value 
            paymentSummary.Fee = result.FaxingFee;
            paymentSummary.SendingAmount = result.FaxingAmount;
            paymentSummary.ReceivingAmount = result.ReceivingAmount;
            paymentSummary.TotalAmount = result.TotalAmount;
            paymentSummary.AgentCommission = AgentCommission;
            paymentSummary.ExchangeRate = result.ExchangeRate;

            _kiiPayWalletTransfer.SetKiiPayEnterAmount(paymentSummary);

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
                ReceivingCurrency = ReceivingCurrencyCode,

            }, JsonRequestBehavior.AllowGet);
        }


    }
}