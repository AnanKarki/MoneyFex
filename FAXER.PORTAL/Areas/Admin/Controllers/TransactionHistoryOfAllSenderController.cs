using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Controllers;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.MoblieTransferApi.Models;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using PagedList;
using PagedList.Mvc;
using System.Linq;
using System.Web.Mvc;
using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using TransferZero.Sdk.Model;
using System.Collections.Generic;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class TransactionHistoryOfAllSenderController : Controller
    {
        CommonServices _CommonServices = null;
        SSenderTransactionHistory _senderTransactionHistoryServices = null;

        public TransactionHistoryOfAllSenderController()
        {
            _CommonServices = new CommonServices();
            _senderTransactionHistoryServices = new SSenderTransactionHistory();

        }
        // GET: Admin/TransactionHistoryOfAllSender
        public ActionResult Index(int? page = null, int PageSize = 10, int CurrentpageCount = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var Country = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(Country, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Country, "Code", "Name");
            var currencies = _CommonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(currencies, "Code", "Name");


            ViewBag.TransferMethod = "0";
            ViewBag.PageSize = 10;
            SenderTransactionActivityWithSenderDetails vm = new SenderTransactionActivityWithSenderDetails();
            vm.SenderTransactionStatement = new List<SenderTransactionActivityVm>();
            int pageSize = PageSize;
            int pageNumber = page ?? 1;
            ViewBag.NumberOfPage = page ?? 1;
            ViewBag.CurrentpageCount = CurrentpageCount;
            ViewBag.ButtonCount = 0;
            ViewBag.PageNumber = pageNumber;

            SenderTransactionSearchParamVm searchParamVm = new SenderTransactionSearchParamVm()
            {
                TransactionServiceType = 0,
                senderId = 0,
                PageSize = PageSize,
                PageNum = page ?? 1,
                IsBusiness = false
            };
            vm = _senderTransactionHistoryServices.GetSenderTransactionAndDetails(searchParamVm);

            if (vm.SenderTransactionStatement.Count != 0)
            {
                var TotalCount = vm.SenderTransactionStatement.FirstOrDefault().TotalCount;
                int NumberOfPage = Common.Common.GetNumberOfPage(TotalCount, searchParamVm.PageSize);
                ViewBag.NumberOfPage = NumberOfPage;
                ViewBag.ButtonCount = NumberOfPage > 10 ? 10 : NumberOfPage;

            }

            return View(vm);

        }
        [HttpPost]
        public ActionResult Index(SenderTransactionSearchParamVm searchParamVm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var Country = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(Country, "Code", "Name", searchParamVm.SendingCountry);
            ViewBag.ReceivingCountries = new SelectList(Country, "Code", "Name", searchParamVm.ReceivingCountry);
            var currencies = _CommonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(currencies, "Code", "Name", searchParamVm.SendingCurrency);

            ViewBag.TransferMethod = searchParamVm.TransactionServiceType;
            ViewBag.DateRange = searchParamVm.DateRange;
            ViewBag.SenderId = searchParamVm.senderId;
            ViewBag.Status = searchParamVm.Status;
            ViewBag.SearchString = searchParamVm.searchString;
            ViewBag.SenderName = searchParamVm.SenderName;
            ViewBag.ReceiverName = searchParamVm.ReceiverName;
            ViewBag.PhoneNumber = searchParamVm.PhoneNumber;
            ViewBag.TransactionWithAndWithoutFee = searchParamVm.TransactionWithAndWithoutFee;
            ViewBag.ResponsiblePerson = searchParamVm.ResponsiblePerson;
            ViewBag.SearchByStatus = searchParamVm.SearchByStatus;
            ViewBag.MFCode = searchParamVm.MFCode;
            ViewBag.SenderEmail = searchParamVm.SenderEmail;
            ViewBag.PageSize = searchParamVm.PageSize;
            searchParamVm.IsBusiness = false;
            SenderTransactionActivityWithSenderDetails vm = _senderTransactionHistoryServices.GetSenderTransactionAndDetails(searchParamVm);
            ViewBag.PageNumber = searchParamVm.PageNum;
            ViewBag.NumberOfPage = 0;
            ViewBag.CurrentpageCount = searchParamVm.CurrentpageCount;
            ViewBag.ButtonCount = 0;
            if (vm.SenderTransactionStatement.Count != 0)
            {
                var TotalCount = vm.SenderTransactionStatement.FirstOrDefault().TotalCount;
                int NumberOfPage = Common.Common.GetNumberOfPage(TotalCount, searchParamVm.PageSize);
                ViewBag.NumberOfPage = NumberOfPage;
                var numberofbuttonshown = NumberOfPage - searchParamVm.CurrentpageCount;

                ViewBag.ButtonCount = numberofbuttonshown;

            }
            return View(vm);
        }



        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SendEMail(EmailModel emailModel)
        {
            EmailServices _emailServices = new EmailServices();

            var model = _emailServices.GetAllTransactionDetailsForEmail(emailModel);
            foreach (var item in model.Transactions)
            {
                _emailServices.SendEmailToSender(item, emailModel.EmailType);
            }



            return Json(new { Data = true }, JsonRequestBehavior.AllowGet);
        }





        [HttpPost]
        public JsonResult SaveNote(TransactionStatementNoteViewModel vm)
        {
            SSenderTransactionHistory _senderTransactionHistoryServices = new SSenderTransactionHistory();
            vm.NoteType = NoteType.TransactionStatementNote;
            _senderTransactionHistoryServices.SaveNotes(vm);

            return Json(new { Data = true }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult AgentDetails(int? AgentStaffId)
        {


            SAgentInformation agentInformationServices = new SAgentInformation();
            var data = agentInformationServices.GetAgentStaffInfo().Data.Where(x => x.Id == AgentStaffId).FirstOrDefault();
            AgentDetialsVm vm = new AgentDetialsVm()
            {

                AgentCountry = Common.Common.GetCountryName(data.Country),
                AgentName = data.Agent.Name,
                AgentNumber = data.Agent.AccountNo,
                AgentPhoneNo = Common.Common.GetCountryPhoneCode(data.Country) + data.Agent.PhoneNumber,
                StaffAccountNo = data.AgentMFSCode,
                StaffEmail = data.EmailAddress,
                TransactionStaffName = data.FirstName + " " + data.LastName + " " + data.LastName
            };
            return View(vm);
        }
        public ActionResult ReInitialCashPickUpTransfer(int Id)
        {
            SSenderCashPickUp _senderCashPickupServices = new SSenderCashPickUp();

            var data = _senderCashPickupServices.List().Data.Where(x => x.Id == Id).FirstOrDefault();
            try
            {
                SenderDocumentationServices senderDocumentationServices = new SenderDocumentationServices();
                senderDocumentationServices.ReInitialCashPickUpTransaction(data);
                TempData["message"] = "Transaction has been re-initialized";
                TempData["status"] = true;
            }
            catch (Exception ex)
            {
                TempData["message"] = "Cannot Re-Initialize the transaction";
                TempData["status"] = true;
            }
            return RedirectToAction("Index");
        }
        public ActionResult ReInitialMobileTransfer(int Id)
        {
            SSenderMobileMoneyTransfer _senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();

            var data = _senderMobileMoneyTransferServices.list().Data.Where(x => x.Id == Id).FirstOrDefault();

            TempData["message"] = "Cannot Re-Initialize the transaction";
            TempData["status"] = true;
            if (data.Status == DB.MobileMoneyTransferStatus.Abnormal)
            {

                SenderDocumentationServices senderDocumentationServices = new SenderDocumentationServices();
                senderDocumentationServices.ReInitialMobileDepositTransaction(data);

                TempData["message"] = "Transaction has been re-initialized";
                TempData["status"] = true;
            }
            return RedirectToAction("Index");
        }

        public ActionResult ReInitialBankDepositTransfer(int Id)
        {
            SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            var data = senderBankAccountDepositServices.List().Data.Where(x => x.Id == Id).FirstOrDefault();

            TempData["message"] = "Cannot Re-Initialize the transaction";
            TempData["status"] = true;
            if (data.Status == DB.BankDepositStatus.ReInitialise)
            {

                SenderDocumentationServices senderDocumentationServices = new SenderDocumentationServices();
                senderDocumentationServices.ReInitialBankDepositTransaction(data);

                TempData["message"] = "Transaction has been re-initialized";
                TempData["status"] = true;
            }
            return RedirectToAction("Index");
        }
        public ActionResult CancelBankTransaction(int Id)
        {
            SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            var data = senderBankAccountDepositServices.List().Data.Where(x => x.Id == Id).FirstOrDefault();

            TempData["message"] = "Cannot cancel the transaction";
            TempData["status"] = true;
            if (data.Status == DB.BankDepositStatus.Held || data.Status == DB.BankDepositStatus.Held || data.Status == DB.BankDepositStatus.Incomplete
                || data.Status == DB.BankDepositStatus.IdCheckInProgress)
            {
                //SenderDocumentationServices senderDocumentationServices = new SenderDocumentationServices();
                //senderDocumentationServices.ReInitialBankDepositTransaction(data);
                //CancelTransaction(data.TransferZeroSenderId);

                TransferZeroWebHookController controller = new TransferZeroWebHookController();
                //  controller.CancelTransactionByReceiptNo(data.ReceiptNo);
                TempData["message"] = "Transaction has been canceled";
                TempData["status"] = true;
            }
            return RedirectToAction("Index");
        }
        public ActionResult CancelMobileTransaction(int Id)
        {
            SSenderMobileMoneyTransfer _senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            var data = _senderMobileMoneyTransferServices.list().Data.Where(x => x.Id == Id).FirstOrDefault();

            TempData["message"] = "Cannot cancel the transaction";
            TempData["status"] = true;
            if (data.Status == DB.MobileMoneyTransferStatus.InProgress || data.Status == DB.MobileMoneyTransferStatus.IdCheckInProgress)
            {
                //CancelTransaction(data.TransferZeroSenderId);

                TempData["message"] = "Transaction has been canceled";
                TempData["status"] = true;
            }
            return RedirectToAction("Index");
        }
        public ActionResult ConfirmMobileTransferTransaction(int Id)
        {
            SSenderMobileMoneyTransfer _senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            var data = _senderMobileMoneyTransferServices.list().Data.Where(x => x.Id == Id).FirstOrDefault();
            if (data.ReceivingCountry == "CM")
            {
                SSenderTransactionHistory _senderTransactionHistoryServices = new SSenderTransactionHistory();

                //data.Status = MobileMoneyTransferStatus.Paid;
                //_senderTransactionHistoryServices.UpdateStatusOfMobileTransfer(data);
                _senderMobileMoneyTransferServices.ConfirmMobilePayment(data);

            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult ConfirmMobileTransfer(string refno)
        {
            SSenderMobileMoneyTransfer _senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            var data = _senderMobileMoneyTransferServices.list().Data.Where(x => x.ReceiptNo == refno).FirstOrDefault();
            
                SSenderTransactionHistory _senderTransactionHistoryServices = new SSenderTransactionHistory();

                _senderTransactionHistoryServices.UpdateStatusOfMobileTransfer(data);

            _senderMobileMoneyTransferServices.ConfirmMobilePayment(data);
            return Json(new
            {
                Data= "Confirm Successfully"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransactionNote(int TransactionId)
        {
            SSenderTransactionHistory _senderTransactionHistoryServices = new SSenderTransactionHistory();

            var result = _senderTransactionHistoryServices.TransactionStatementNote(TransactionId);
            _senderTransactionHistoryServices.UpdateTransactionStatementNote(TransactionId);
            return Json(new
            {
                result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApproveHoldTransaction(int Id, TransactionServiceType method)
        {

            var result = new ServiceResult<bool>();
            var exchangeRateType = ExchangeRateType.TransactionExchangeRate;

            switch ((TransactionServiceType)method)
            {
                case TransactionServiceType.All:
                    break;
                case TransactionServiceType.MobileWallet:
                    SSenderMobileMoneyTransfer senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
                    var data = senderMobileMoneyTransferServices.list().Data.Where(x => x.Id == Id).FirstOrDefault();
                    if (data.Status == DB.MobileMoneyTransferStatus.IdCheckInProgress)
                    {
                        result.Data = false;
                        result.Status = ResultStatus.Error;
                        result.Message = "Approve the identification document process first";
                        return Json(new
                        {
                            result
                        }, JsonRequestBehavior.AllowGet);
                    }


                    try
                    {
                        exchangeRateType = Common.Common.SystemExchangeRateType(data.SendingCountry, data.ReceivingCountry, TransactionTransferMethod.OtherWallet);
                    }
                    catch (Exception)
                    {

                    }

                    if (exchangeRateType == ExchangeRateType.CurrentExchangeRate)
                    {

                        var newExchange = Common.Common.GetModifedAmount(data.SendingCountry, data.ReceivingCountry,
                            TransactionTransferMethod.OtherWallet, data.SendingAmount, TransactionTransferType.Admin);
                        data.ReceivingAmount = newExchange.ReceivingAmount;
                        data.ExchangeRate = newExchange.ExchangeRate;
                        data.Fee = newExchange.Fee;
                        data.TotalAmount = newExchange.TotalAmount;

                    }
                    // senderMobileMoneyTransferServices.CreateTransactionToApi(data);
                    var MobileMoneyTransactionResult = new MTNCameroonResponseParamVm();
                    SSenderMobileMoneyTransfer _senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
                    if (data.IsComplianceApproved == false)
                    {
                        data.IsComplianceApproved = true;
                        data.ComplianceApprovedBy = StaffSession.LoggedStaff.StaffId;
                        data.ComplianceApprovedDate = DateTime.Now;
                        var transferApiResponse = _senderMobileMoneyTransferServices.CreateTransactionToApi(data, TransactionTransferType.Online);
                        Log.Write(data.ReceiptNo + "Mobile Transaction Api Success ");
                        data.Status = transferApiResponse.status;
                        MobileMoneyTransactionResult = transferApiResponse.response;
                        senderMobileMoneyTransferServices.Update(data);

                        SMobileMoneyTransferResopnseStatus _sMobileMoneyResposeStatus = new SMobileMoneyTransferResopnseStatus();
                        _sMobileMoneyResposeStatus.AddLog(MobileMoneyTransactionResult, data.Id);
                        Log.Write(data.ReceiptNo + "Mobile Transaction Api Successful ");
                        _senderMobileMoneyTransferServices.SendEmailAndSms(data);
                    }
                    break;
                case TransactionServiceType.KiiPayWallet:
                    break;
                case TransactionServiceType.BillPayment:
                    break;
                case TransactionServiceType.ServicePayment:
                    break;
                case TransactionServiceType.CashPickUp:
                    SSenderCashPickUp senderCashPickUpServices = new SSenderCashPickUp();
                    var cashTrans = senderCashPickUpServices.List().Data.Where(x => x.Id == Id).FirstOrDefault();
                    if (cashTrans.FaxingStatus == FaxingStatus.IdCheckInProgress)
                    {

                        result.Data = false;
                        result.Status = ResultStatus.Error;
                        result.Message = "Approve the identification document process first";
                        return Json(new
                        {
                            result
                        }, JsonRequestBehavior.AllowGet);
                    }


                    exchangeRateType = Common.Common.SystemExchangeRateType(cashTrans.SendingCountry, cashTrans.ReceivingCountry, TransactionTransferMethod.CashPickUp);

                    if (exchangeRateType == ExchangeRateType.CurrentExchangeRate)
                    {

                        var newExchange = Common.Common.GetModifedAmount(cashTrans.SendingCountry, cashTrans.ReceivingCountry,
                            TransactionTransferMethod.CashPickUp, cashTrans.FaxingAmount, TransactionTransferType.Admin);
                        cashTrans.ReceivingAmount = newExchange.ReceivingAmount;
                        cashTrans.ExchangeRate = newExchange.ExchangeRate;
                        cashTrans.FaxingFee = newExchange.Fee;
                        cashTrans.TotalAmount = newExchange.TotalAmount;

                    }


                    if (cashTrans.IsComplianceApproved == false)
                    {
                        //item.IsComplianceApproved = true;
                        //item.ComplianceApprovedBy = StaffSession.LoggedStaff.StaffId;
                        //item.ComplianceApprovedDate = DateTime.Now;

                        cashTrans.FaxingStatus = FaxingStatus.NotReceived;
                        cashTrans.IsComplianceApproved = true;
                        cashTrans.ComplianceApprovedBy = StaffSession.LoggedStaff.StaffId;
                        cashTrans.ComplianceApprovedDate = DateTime.Now;
                        senderCashPickUpServices.Update(cashTrans);

                        var CashPickUpTransactionResult = new BankDepositResponseVm();
                        var transResponse = senderCashPickUpServices.CreateCashPickTransactionToApi(cashTrans);
                        cashTrans.FaxingStatus = transResponse.CashPickUp.FaxingStatus;
                        CashPickUpTransactionResult = transResponse.BankDepositApiResponseVm;
                        senderCashPickUpServices.Update(cashTrans);
                        senderCashPickUpServices.AddResponseLog(CashPickUpTransactionResult, cashTrans.Id);
                        senderCashPickUpServices.SendEmailAndSms(cashTrans);
                    }
                    Log.Write(cashTrans.MFCN + "Cash pick up Transaction Api Successful ");
                    break;
                case TransactionServiceType.BankDeposit:
                    SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();

                    var item = senderBankAccountDepositServices.List().Data.Where(x => x.Id == Id).FirstOrDefault();
                    var bankdepositTransactionResult = new BankDepositResponseVm();

                    if (item.Status == DB.BankDepositStatus.IdCheckInProgress)
                    {
                        result.Data = false;
                        result.Status = ResultStatus.Error;
                        result.Message = "Approve the identification document process first";
                        return Json(new
                        {
                            result
                        }, JsonRequestBehavior.AllowGet);
                    }
                    exchangeRateType = Common.Common.SystemExchangeRateType(item.SendingCountry, item.ReceivingCountry, TransactionTransferMethod.BankDeposit);

                    if (exchangeRateType == ExchangeRateType.CurrentExchangeRate)
                    {

                        var newExchange = Common.Common.GetModifedAmount(item.SendingCountry, item.ReceivingCountry,
                            TransactionTransferMethod.BankDeposit, item.SendingAmount, TransactionTransferType.Admin);
                        item.ReceivingAmount = newExchange.ReceivingAmount;
                        item.ExchangeRate = newExchange.ExchangeRate;
                        item.Fee = newExchange.Fee;
                        item.TotalAmount = newExchange.TotalAmount;

                    }
                    SSenderBankAccountDeposit senderBankAccountDeposit = new SSenderBankAccountDeposit();
                    if (item.IsComplianceApproved == false)
                    {
                        item.IsComplianceApproved = true;
                        item.ComplianceApprovedBy = StaffSession.LoggedStaff.StaffId;
                        item.ComplianceApprovedDate = DateTime.Now;

                        var transResponse = senderBankAccountDeposit.CreateBankTransactionToApi(item);
                        item.Status = transResponse.BankAccountDeposit.Status;
                        bankdepositTransactionResult = transResponse.BankDepositApiResponseVm;
                        senderBankAccountDepositServices.Update(item);
                        SBankDepositResponseStatus sBankDepositResponseStatus = new SBankDepositResponseStatus();
                        sBankDepositResponseStatus.AddLog(bankdepositTransactionResult, item.Id);
                        senderBankAccountDeposit.SendEmailAndSms(item);
                    }
                    break;
                default:
                    break;
            }
            result.Data = true;
            result.Message = "Approve Successfully";
            result.Status = ResultStatus.OK;
            return Json(new
            {
                result
            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ManualApproveTransaction(string refNo)
        {
            SSenderBankAccountDeposit senderBankAccountDeposit = new SSenderBankAccountDeposit();

            var data = senderBankAccountDeposit.List().Data.Where(x => x.ReceiptNo == refNo.Trim()).FirstOrDefault();
            data.IsManualDeposit = true;
            data.ManuallyApproved = true;
            data.IsManualApproveNeeded = false;
            data.Status = BankDepositStatus.Confirm;

            senderBankAccountDeposit.Update(data);
            senderBankAccountDeposit.SendEmailAndSms(data);
            return Json(new
            {
                Data ="Payment Confirmed successfully!"
            }, JsonRequestBehavior.AllowGet);




        }
        public JsonResult UpdateStatus()
        {

            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckPGStatus(string refno, TransactionServiceType transferMethod)
        {

            PGTransactionResultVm result = _senderTransactionHistoryServices.CheckPGStatus(refno, transferMethod);

            return Json(new
            {
                Data = result
            }, JsonRequestBehavior.AllowGet);
        }

        //public FileResult ExportExcelOfTransactionStatement(SenderTransactionSearchParamVm searchParamVm)
        //{
        //    var transactionStatements = _senderTransactionHistoryServices.GetTransactionDetailsForExcel(searchParamVm);
        //    Common.Utilities _utilities = new Utilities();
        //    var fsr = _utilities.CreateExcelWorkSheet(transactionStatements, "TransactionStatement");
        //    return fsr;
        //}
        public FileResult ExportExcelOfTransactionStatement(string DateRange = "", int TransferMethod = 0, string SendingCountry = "",
            string ReceivingCountry = "", string searchParam = "", string senderName = "", string receiverName = "", string Status = "",
            string telephone = "", string SendingCurrency = "", string transactionWithAndWithoutFee = "", string ResponsiblePerson = "",
             string SearchByStatus = "", string MFCode = "", string SenderEmail = "", int PageNum = 0, int PageSize = 0)
        {
            SenderTransactionSearchParamVm searchParamVm = new SenderTransactionSearchParamVm()
            {
                DateRange = DateRange,
                MFCode = MFCode,
                PageNum = PageNum,
                PageSize = PageSize,
                PhoneNumber = telephone,
                ReceiverName = receiverName,
                ReceivingCountry = ReceivingCountry,
                ResponsiblePerson = ResponsiblePerson,
                SearchByStatus = SearchByStatus,
                searchString = searchParam,
                SenderName = senderName,
                SenderEmail = SenderEmail,
                SendingCountry = SendingCountry,
                SendingCurrency = SendingCurrency,
                Status = Status,
                TransactionServiceType = TransferMethod,
                TransactionWithAndWithoutFee = transactionWithAndWithoutFee,
                IsBusiness = false
            };
            var transactionStatements = _senderTransactionHistoryServices.GetTransactionDetailsForExcel(searchParamVm);
            Common.Utilities _utilities = new Utilities();
            var fsr = _utilities.CreateExcelWorkSheet(transactionStatements, "TransactionStatement");
            return fsr;
        }

    }


    public class AgentDetialsVm
    {

        public string AgentName { get; set; }

        public string AgentNumber { get; set; }

        public string AgentCountry { get; set; }
        public string AgentPhoneNo { get; set; }

        public string TransactionStaffName { get; set; }
        public string StaffAccountNo { get; set; }
        public string StaffEmail { get; set; }

    }

    public class EmailTransactionList
    {

        public int TransactionId { get; set; }
        public string RecevingCountry { get; set; }
        public string Method { get; set; }
        public int SenderId { get; set; }


    }
    public class EmailModel
    {

        public string EmailType { get; set; }
        public System.Collections.Generic.List<EmailTransactionList> Transactions { get; set; }
    }
}