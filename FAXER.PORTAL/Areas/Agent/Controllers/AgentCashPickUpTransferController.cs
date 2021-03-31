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
    public class AgentCashPickUpTransferController : Controller
    {
        Admin.Services.CommonServices common = new Admin.Services.CommonServices();

        SCashPickUpTransferService _cashPickUp = new SCashPickUpTransferService();
        AgentCommonServices _commonServices = new AgentCommonServices();


        // GET: Agent/AgentCashPickUpTransfer
        [HttpGet]
        public ActionResult CashPickupInformation(string AccountNoORPhoneNo = "")
        {
            Common.FaxerSession.TransferMethod = "cashpickup";
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();

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

            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();
            vm = _cashPickUp.GetCashPickupInformationViewModel();

            if (AccountNoORPhoneNo != "")
            {
                ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType", vm.IdType);
                var result = _cashPickUp.getFaxer(AccountNoORPhoneNo, vm);


                if (result != null)
                {
                    string FaxerCountryPhoneCode = Common.Common.GetCountryPhoneCode(result.Country);
                    if (result.Id != 0)
                    {
                        ViewBag.AgentResult = agentResult;
                        return View(result);
                    }
                }
                else
                {

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
        public ActionResult CashPickupInformation([Bind(Include = CashPickupInformationViewModel.BindProperty)] CashPickupInformationViewModel vm)
        {
            #region old
            //List<Admin.Services.DropDownViewModel> countries = common.GetCountries();
            //List<Admin.Services.DropDownCardTypeViewModel> identifyCardType = common.GetCardType();

            //AgentResult agentResult = new AgentResult();

            //ViewBag.countries = new SelectList(countries, "Code", "Name");
            //ViewBag.IDTypes = new SelectList(identifyCardType, "Id", "CardType");
            //vm.SenderCountryCode = Common.Common.GetCountryPhoneCode(vm.IssuingCountry);
            //var CurrentYear = DateTime.Now.Year;
            //string agentCountry = Common.AgentSession.AgentInformation.CountryCode;

            //if (vm.Country != agentCountry)
            //{
            //    ModelState.AddModelError("InvalidCountry", "This transaction was not from your country, please direct the customer to their respective country.");
            //    ViewBag.AgentResult = agentResult;
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
            //        ViewBag.AgentResult = agentResult;
            //        return View(vm);
            //    }
            //    if (vm.ExpiryDate < DateTime.Now)
            //    {
            //        //agentResult.Message = "Sender's Identity card has been expired.";
            //        //agentResult.Status = ResultStatus.Warning;
            //        ModelState.AddModelError("IDExpired", "Expired ID");
            //        ViewBag.AgentResult = agentResult;
            //        return View(vm);
            //    }
            //    _cashPickUp.SetCashPickupInformationViewModel(vm);
            //    return RedirectToAction("CashPickUpEnterAmount");
            //}
            //ViewBag.AgentResult = agentResult;
            //return View(vm);
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
        public ActionResult CashPickUpReceiverDetailsInformation(string Country = "")
        {
            var paymentInfo = _cashPickUp.GetCashPickUpEnterAmount();
            Country = paymentInfo.ReceivingCountry;
            int Id = _cashPickUp.GetCashPickupInformationViewModel().Id;
            var countries = common.GetCountries();
            var IdCardType = common.GetCardType();

            ViewBag.Countries = new SelectList(countries, "Code", "Name", Country);
            ViewBag.IdCardTypes = new SelectList(IdCardType, "Id", "CardType");


            var existingReceiver = _cashPickUp.getExistingReceiver(Id, Country);

            if (Id != 0)
            {
                ViewBag.existingReceiver = new SelectList(existingReceiver, "Id", "FirstName");
            }
            else
            {
                ViewBag.existingReceiver = new SelectList(GetPreviousReceivers(), "Code", "Name");

            }
            CashPickUpReceiverDetailsInformationViewModel vm = _cashPickUp.GetCashPickUpReceiverInfoViewModel();
            vm.MobileCode = Common.Common.GetCountryPhoneCode(Country);
            vm.Country = Country;
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
        public ActionResult CashPickUpReceiverDetailsInformation([Bind(Include = CashPickUpReceiverDetailsInformationViewModel.BindProperty)] CashPickUpReceiverDetailsInformationViewModel Vm)
        {
            var paymentInfo = _cashPickUp.GetCashPickUpEnterAmount();
            int Id = _cashPickUp.GetCashPickupInformationViewModel().Id;
            Vm.MobileCode = Common.Common.GetCountryPhoneCode(paymentInfo.ReceivingCountry);
            var countries = common.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            var IdCardType = common.GetCardType();
            ViewBag.IdCardTypes = new SelectList(IdCardType, "Id", "CardType");
            var existingReceiver = _cashPickUp.getExistingReceiver(Id, paymentInfo.ReceivingCountry);
            if (Id != 0)
            {
                ViewBag.existingReceiver = new SelectList(existingReceiver, "Id", "FirstName");
            }
            else
            {
                ViewBag.existingReceiver = new SelectList(GetPreviousReceivers(), "Code", "Name");

            }

            if (Vm.ReasonForTransfer == PORTAL.Models.ReasonForTransfer.Non)
            {
                ModelState.AddModelError("Reason", "Select Reason");
                return View(Vm);
            }
            if (ModelState.IsValid)
            {
                bool IsValidReceiver = Common.Common.IsValidBankDepositReceiver(Vm.MobileNo, Service.CashPickUP);

                if (IsValidReceiver == false)
                {
                    ModelState.AddModelError("InvalidReceiver", " Receiver is banned");
                    return View(Vm);
                }

                if (Vm.Country == "MA")
                {
                    var agentInfo = Common.AgentSession.AgentInformation;
                    var transactionTransferType = TransactionTransferType.Agent;
                    if (agentInfo.IsAUXAgent)
                    {
                        transactionTransferType = TransactionTransferType.AuxAgent;
                    }
                    var Apiservice = Common.Common.GetApiservice(agentInfo.CountryCode, Vm.Country, paymentInfo.SendingAmount,
                        TransactionTransferMethod.CashPickUp, transactionTransferType, agentInfo.Id);

                    if (Apiservice == null)
                    {

                        ModelState.AddModelError("ServiceNotAvialable", "Service Not Available");
                        return View(Vm);

                    }
                    if (Vm.IdenityCardId < 0)
                    {
                        ModelState.AddModelError("IdenityCardId", "Select Id card type");
                        return View(Vm);
                    }
                    if (string.IsNullOrEmpty(Vm.IdentityCardNumber))
                    {
                        ModelState.AddModelError("IdentityCardNumber", "Enter card number");
                        return View(Vm);
                    }

                    SmsApi smsApi = new SmsApi();
                    var IsValidMobileNo = smsApi.IsValidMobileNo(Vm.MobileCode + "" + Vm.MobileNo);
                    if (IsValidMobileNo == false)
                    {
                        ModelState.AddModelError("", "Enter Valid Number");
                        return View(Vm);
                    }
                }

                _cashPickUp.SetCashPickUpReceiverInfoViewModel(Vm);
                return RedirectToAction("TransactionSummary");
            }

            return View(Vm);
        }


        public List<ReasonToTransferVm> GetReasonForTransfers()
        {
            var result = new List<ReasonToTransferVm>();
            return result;
        }
        public List<PreviousReceiverVm> GetPreviousReceivers()
        {
            var result = new List<PreviousReceiverVm>();
            return result;
        }
        [HttpGet]
        public ActionResult CashPickUpEnterAmount()
        {


            AgentResult agentResult = new AgentResult();

            CashPickUpEnterAmountViewModel Vm = new CashPickUpEnterAmountViewModel();
            //var PaymentSummary = _cashPickUp.GetCashPickUpEnterAmount();
            //string SendingCountry = Common.AgentSession.CashPickupInformationViewModel.Country;
            //string ReceivingCountry = Common.AgentSession.CashPickUpReceiverDetailsInformationViewModel.Country;
            //string agentCountry = Common.AgentSession.AgentInformation.CountryCode;

            //Vm.ExchangeRate = SExchangeRate.GetExchangeRateValue(agentCountry, ReceivingCountry, TransactionTransferMethod.CashPickUp, Common.AgentSession.AgentInformation.Id, TransactionTransferType.Agent);
            //Vm.ReceivingAmount = PaymentSummary.ReceivingAmount;
            //Vm.SendingAmount = PaymentSummary.SendingAmount;
            //Vm.SendingCurrency = Common.Common.GetCountryCurrency(agentCountry);
            //Vm.ReceivingCurrency = Common.Common.GetCountryCurrency(ReceivingCountry);
            //Vm.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(agentCountry);
            //Vm.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(ReceivingCountry);
            //Vm.WalletName = Common.AgentSession.CashPickUpReceiverDetailsInformationViewModel.ReceiverFullName;
            //_commonServices.SetAgentPaymentSummarySession(ReceivingCountry, TransactionTransferMethod.CashPickUp);
            //_cashPickUp.SetCashPickUpEnterAmount(Vm);
            //ViewBag.AgentResult = agentResult;

            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();
            var paymentInfo = _cashPickUp.GetCashPickUpEnterAmount();
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
                Vm.SendingAmount = paymentInfo.SendingAmount;
                Vm.ReceivingAmount = paymentInfo.ReceivingAmount;
                Vm.ReceivingCurrency = paymentInfo.ReceivingCurrency == null ? "NGN" : paymentInfo.ReceivingCurrency;
                Vm.SendingCurrency = Common.Common.GetCountryCurrency(agentCountry);
                Vm.SendingCountry = agentCountry;
                Vm.Fee = paymentInfo.Fee;
                Vm.IsConfirm = paymentInfo.IsConfirm;
                Vm.TheyReceive = paymentInfo.TheyReceive;
                Vm.AgentCommission = paymentInfo.AgentCommission;
                Vm.TotalAmount = paymentInfo.TotalAmount;
                return View(Vm);
            }



            return View(Vm);

        }
        [HttpPost]
        public ActionResult CashPickUpEnterAmount([Bind(Include = CashPickUpEnterAmountViewModel.BindProperty)] CashPickUpEnterAmountViewModel Vm)
        {
            AgentResult agentResult = new AgentResult();

            var agentInfo = Common.AgentSession.AgentInformation;
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
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
            ViewBag.TransferType = TransactionTransferType.Agent;
            if (agentInfo.IsAUXAgent)
            {
                ViewBag.TransferType = TransactionTransferType.AuxAgent;
            }
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
                if (string.IsNullOrEmpty((Vm.Fee).ToString()))
                {
                    ModelState.AddModelError("FaxingFee", "Please calculate estimated fee ");
                    ViewBag.AgentResult = agentResult;
                    return View(Vm);
                }
                if (Vm.IsConfirm == false)
                {
                    ModelState.AddModelError("IsConfirm", "Please Confirm.");
                    return View(Vm);
                }
                if (agentInfo.IsAUXAgent)
                {
                    AgentCommonServices _AgentCommonServices = new AgentCommonServices();
                    decimal AUXAgentAccountBalance = _AgentCommonServices.getAuxAgentAccountBalance(agentInfo.Id, Common.AgentSession.LoggedUser.PayingAgentStaffId);
                    if (AUXAgentAccountBalance <= Vm.TotalAmount)
                    {
                        ModelState.AddModelError("Insufficientfund", "Insufficient account balance");
                        ViewBag.AgentResult = agentResult;
                        return View(Vm);
                    }
                }
                _cashPickUp.SetCashPickUpEnterAmount(Vm);
                return RedirectToAction("CashPickUpReceiverDetailsInformation", "AgentCashPickUpTransfer");


            }
            ViewBag.AgentResult = agentResult;
            return View(Vm);
        }
        public ActionResult TransactionSummary()
        {
            var paymentInfo = _cashPickUp.GetCashPickUpEnterAmount();

            var receiverinfo = _cashPickUp.GetCashPickUpReceiverInfoViewModel();

            string ReceiverFirstname = receiverinfo.ReceiverFullName.Split(' ')[0];
            CommonTransactionSummaryViewModel vm = new CommonTransactionSummaryViewModel()
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

            return View(vm);
        }
        [HttpPost]
        public ActionResult TransactionSummary([Bind(Include = CommonTransactionSummaryViewModel.BindProperty)]CommonTransactionSummaryViewModel vm)
        {
            var paymentInfo = _cashPickUp.GetCashPickUpEnterAmount();
            _cashPickUp.SetTransactionSummary();
            TransferForAllAgentServices transferForAllAgentServices = new TransferForAllAgentServices();
            int transactionId = transferForAllAgentServices.CompleteTransaction(TransactionTransferMethod.CashPickUp);
            //var nonCardTransaction = _cashPickUp.FaxNonCardTransactionByAgent(paymentInfo);
            if (transactionId > 0)
            {
                var agentInfo = Common.AgentSession.AgentInformation;
                if (agentInfo.IsAUXAgent)
                {
                    int senderId = Common.AgentSession.SenderId;
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
                }
                return RedirectToAction("CashPickUpSuccess", "AgentCashPickUpTransfer", new { Id = transactionId });
            }
            else
            {
                ModelState.AddModelError("", "Error Occured While Doing Transaction");
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult CashPickUpSuccess(int Id)
        {
            CashPickUpSuccessViewModel Vm = new CashPickUpSuccessViewModel();

            TransferForAllAgentServices transferForAllAgentServices = new TransferForAllAgentServices();
            var transferSummary = transferForAllAgentServices.GetTransactionSummary();
            Vm.WalletName = transferSummary.RecipientDetails.ReceiverName;
            Vm.SendingCurrency = transferSummary.PaymentSummary.SendingCurrency;
            Vm.SendingAmount = transferSummary.PaymentSummary.SendingAmount;
            Vm.TransactionId = Id;
            return View(Vm);
        }

        public void PrintReceipt(int TransactionId)
        {

            var FaxingNonCardInfo = _cashPickUp.FaxingNonCardInfo(TransactionId);

            var agentInformatioin = Common.AgentSession.AgentInformation;

            var ReceiverInfo = _cashPickUp.GetCashPickUpReceiverInfoViewModel();

            var senderInfo = _cashPickUp.GetCashPickupInformationViewModel();

            // Need Changes Here  Because the sender info is conditional now 
            //the Kiipay Personal might or might not be associated with the faxer

            //var faxerInformation = dbContext.FaxerInformation.Where(x => x.Id == CarduserInformation.FaxerId).FirstOrDefault();

            string CardUserCurrency = Common.Common.GetCountryCurrency(FaxingNonCardInfo.SendingCountry);
            string SenderPhoneCode = Common.Common.GetCountryPhoneCode(FaxingNonCardInfo.SendingCountry);
            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(ReceiverInfo.Country);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);


            var ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardUserReceiverReceipt/PrintReceipt?MFReceiptNumber=" + FaxingNonCardInfo.ReceiptNumber +
                "&TransactionDate=" + FaxingNonCardInfo.TransactionDate.ToString("dd/MM/yyyy") +
                "&TransactionTime=" + FaxingNonCardInfo.TransactionDate.ToString("HH:mm") +
                "&SenderFullName=" + senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName +
                "&SenderEmail=" + senderInfo.Email +
                "&SenderTelephone=" + SenderPhoneCode + " " + senderInfo.MobileNo +
                "&SenderDOB=" + senderInfo.DOB.ToFormatedString("dd/MM/yyyy") +
                "&ReceiverFullName=" + ReceiverInfo.ReceiverFullName +
                "&ReceiverEmail=" + ReceiverInfo.Email +
                "&ReceiverTelephone=" + ReceiverPhoneCode + " " + ReceiverInfo.MobileNo +
                "&AgentName=" + agentInformatioin.Name +
                "&AgentAcountNumber=" + agentInformatioin.AccountNo +
                "&StaffName=" + agentInformatioin.ContactPerson +
                "&AgentCity=" + agentInformatioin.City +
                "&AgentCountry=" + Common.Common.GetCountryName(agentInformatioin.CountryCode) +
                "&TotalAmount=" + CardUserCurrency + " " + FaxingNonCardInfo.TotalAmount +
                "&Fee=" + CardUserCurrency + " " + FaxingNonCardInfo.FaxingFee +
                "&SendingAmount=" + CardUserCurrency + " " + FaxingNonCardInfo.FaxingAmount +
                "&ExchangeRate=" + FaxingNonCardInfo.ExchangeRate +
                "&SendingCurrency= " + Common.Common.GetCountryCurrency(senderInfo.Country) +
                "&ReceivingCurrency= " + Common.Common.GetCountryCurrency(ReceiverInfo.Country) +
                "&MFCN=" + FaxingNonCardInfo.MFCN +
                "&ReceivingAmount=" + FaxingNonCardInfo.ReceivingAmount +
                "&PaymentMethod=" + FaxingNonCardInfo.PaymentMethod;

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
            SAgentInformation _sFaxerInfromationServices = new SAgentInformation();
            var result = _sFaxerInfromationServices.list().Data.Where(x => x.Id == agentId).Select(x => x.CountryCode).FirstOrDefault();
            return result;
        }
        public ActionResult GetReceiverDetails(int id, string RecevingCountry)
        {
            var receiverDetails = _cashPickUp.getReceiverDetails(id);

            if (receiverDetails.Country.ToLower() != RecevingCountry.ToLower())
            {
                return Json(new
                {
                    InvalidReceivingCountry = true
                }, JsonRequestBehavior.AllowGet);

            }
            var CountryPhoneCode = Common.Common.GetCountryPhoneCode(receiverDetails.Country);
            return Json(new
            {
                ReceiverFullName = receiverDetails.FirstName + " " + receiverDetails.MiddleName + " " + receiverDetails.LastName,
                Email = receiverDetails.EmailAddress,
                MobileNo = receiverDetails.PhoneNumber,
                MobileCode = CountryPhoneCode,
                ReceiverSelected = true,

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getCalculatedDetails(string FaxerCountry, string ReceiverCountry, Decimal FaxAmount, decimal ReceivingAmount)
        {
            //Decimal FaxAmount = 0;
            //string ReceiverCountry = "";

            FaxerCountry = Common.AgentSession.AgentInformation.CountryCode;

            var result = _cashPickUp.getCalculateDetails(FaxerCountry, ReceiverCountry, FaxAmount, ReceivingAmount);

            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(ReceiverCountry);
            if (result != null)
            {
                return Json(new
                {
                    SendingAmount = result.FaxingAmount,
                    Fee = result.FaxingFee,
                    ExchangeRate = result.ExchangeRate,
                    TotalAmount = Math.Round(result.TotalAmount),
                    ReceivingAmount = result.ReceivingAmount,
                    ReceivingCurrency = common.getCurrencyCodeFromCountry(ReceiverCountry),
                    ReceivingCurrencySymbol = common.getCurrencySymbol(ReceiverCountry),
                    SendingCurrency = common.getCurrencyCodeFromCountry(FaxerCountry),
                    SendingCurrencySymbol = common.getCurrencySymbol(FaxerCountry),
                    ReceiverPhoneCode = ReceiverPhoneCode,
                }, JsonRequestBehavior.AllowGet);


            }
            else
            {

                return Json(new
                {
                    NoExchangeRateSetup = true,
                    SendingAmount = 0,
                    Fee = 0,
                    ExchangeRate = 0,
                    TotalAmount = 0,
                    ReceivingAmount = 0,
                    ReceivingCurrency = "",
                    ReceivingCurrencySymbol = "",
                    SendingCurrency = "",
                    SendingCurrencySymbol = "",


                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount, bool IsReceivingAmount, string receiverCountry)
        {
            int AgentId = Common.AgentSession.AgentInformation.Id;
            var agentCountry = getAgentCountryCode(AgentId);
            var enterAmountData = _cashPickUp.GetCashPickUpEnterAmount();

            var senderInfo = _cashPickUp.GetCashPickupInformationViewModel();
            var SendingCurrencySymbol = Common.Common.GetCurrencySymbol(agentCountry);
            var SendingCurrencyCode = Common.Common.GetCurrencyCode(agentCountry);
            var RecevingCurrencySymbol = Common.Common.GetCurrencySymbol(receiverCountry);
            var ReceivingCurrencyCode = Common.Common.GetCurrencyCode(receiverCountry);

            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }
            //var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
            //    paymentSummary.ExchangeRate, SEstimateFee.GetFaxingCommision(agentCountry));

            //var AgentCommission = Common.Common.GetAgentSendingCommission(TransferService.CahPickUp, AgentId, result.FaxingAmount, result.FaxingFee);

            //// Rewrite session with additional value 
            //paymentSummary.Fee = result.FaxingFee;
            //paymentSummary.SendingAmount = result.FaxingAmount;
            //paymentSummary.ReceivingAmount = result.ReceivingAmount;
            //paymentSummary.TotalAmount = result.TotalAmount;
            //paymentSummary.AgentCommission = AgentCommission;

            //_cashPickUp.SetCashPickUpEnterAmount(paymentSummary);
            //return Json(new
            //{
            //    Fee = result.FaxingFee,
            //    TotalAmount = result.TotalAmount,
            //    ReceivingAmount = result.ReceivingAmount,
            //    SendingAmount = result.FaxingAmount,
            //    AgentCommission = AgentCommission
            //}, JsonRequestBehavior.AllowGet);


            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }

            var feeInfo = SEstimateFee.GetTransferFee(agentCountry, receiverCountry, TransactionTransferMethod.CashPickUp, SendingAmount,
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
                    ReceivingCurrency = ReceivingCurrencyCode,

                }, JsonRequestBehavior.AllowGet);
            }
            var result = new EstimateFaxingFeeSummary();

            result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                SExchangeRate.GetExchangeRateValue(agentCountry, receiverCountry, TransactionTransferMethod.CashPickUp, AgentId, TransactionTransferType.Agent, Common.AgentSession.AgentInformation.IsAUXAgent), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(agentCountry, receiverCountry, result.FaxingAmount
                , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, IsReceivingAmount, TransactionTransferMethod.CashPickUp, AgentId, true);

            if (introductoryRateResult != null)
            {

                result = introductoryRateResult;
            }


            //var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
            //    enterAmountData.ExchangeRate, SEstimateFee.GetFaxingCommision(agentCountry));
            var AgentCommission = Common.Common.GetAgentSendingCommission(TransferService.CahPickUp, AgentId, result.FaxingAmount, result.FaxingFee);
            // Rewrite session with additional value 
            enterAmountData.Fee = result.FaxingFee;
            enterAmountData.SendingAmount = result.FaxingAmount;
            enterAmountData.ReceivingAmount = result.ReceivingAmount;
            enterAmountData.TotalAmount = result.TotalAmount;
            enterAmountData.AgentCommission = AgentCommission;
            enterAmountData.ExchangeRate = result.ExchangeRate;


            _cashPickUp.SetCashPickUpEnterAmount(enterAmountData);

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

        public ActionResult GetPhoneCode(string countryCode)
        {
            var phoneCode = common.getPhoneCodeFromCountry(countryCode);
            return Json(new
            {
                MobileCode = phoneCode
            }, JsonRequestBehavior.AllowGet);
        }
    }
}