using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentDashboardController : Controller
    {
        DailyTransactionStatementServices _dailyTransactionStatementServices = null;
        AgentInformation agentInfo = null;
        AgentCommonServices CommonServices = null;
        SAgentKiiPayWalletTransferServices _KiiPayWalletTransferServices = null;
        PayAReceiverControllerServices _payaReceiverServices = null;
        SAgentMobileTransferWallet _sAgentMobileTransferWallet = null;
        SCashPickUpTransferService _sCashPickUpTransferService = null;
        SAgentBankAccountDeposit _sAgentBankAccountDeposit = null;
        public AgentDashboardController()
        {
            _dailyTransactionStatementServices = new DailyTransactionStatementServices();
            agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            CommonServices = new AgentCommonServices();
            _KiiPayWalletTransferServices = new SAgentKiiPayWalletTransferServices();
            _payaReceiverServices = new PayAReceiverControllerServices();
            _sAgentMobileTransferWallet = new SAgentMobileTransferWallet();
            _sCashPickUpTransferService = new SCashPickUpTransferService();
            _sAgentBankAccountDeposit = new SAgentBankAccountDeposit();
        }
        // GET: Agent/AgentDashboard
        public ActionResult Index()
        {

            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }

            string firstLogin = Common.AgentSession.FirstLogin;
            AgentServices.AlertServices alertServices = new AgentServices.AlertServices();
            AgentDashBoardViewModel vm = new AgentDashBoardViewModel();
            vm.AlersViewModel = alertServices.GetAlerts();
            ViewBag.AgentMoneyFaxAcNo = agentInfo.AccountNo;
            ViewBag.FirstLogin = firstLogin;
            ViewBag.Count = vm.AlersViewModel.Count();

            vm.AgentCurrencySymbol = Common.Common.GetCurrencySymbol(agentInfo.CountryCode);
            vm.AgentCurrency = Common.Common.GetCountryCurrency(agentInfo.CountryCode);

            vm.Commission = CommonServices.Get30DaysAgentCommission(Common.AgentSession.LoggedUser.PayingAgentStaffId);
            ViewBag.AgentType = Common.AgentSession.LoggedUser.AgentType;
            return View(vm);
        }

        public ActionResult GoToDashboard()
        {

            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            CommonServices.ClearAgentKiiPayTransferSession();
            CommonServices.ClearAgentBankAccountDeposit();
            CommonServices.ClearAgentCashPickUpTransfer();
            CommonServices.ClearOtherMobileWallerTransfer();
            CommonServices.ClearAgentPayBillsMonthly();
            CommonServices.ClearAgentPayBillsTopUp();
            CommonServices.ClearPayAReceiverCashPickUp();
            CommonServices.ClearPayAReceiverKiiPay();
            CommonServices.ClearCommonEnterAmount();
            CommonServices.ClearSenderId();
            CommonServices.ClearAgentTransactionSummaryVm();
            if (!agentInfo.IsAUXAgent)
            {
                return RedirectToAction("Index", "AgentDashboard");
            }
            else
            {
                return RedirectToAction("AUXAgentDashboard", "AgentDashboard");
            }
        }

        public ActionResult AUXAgentDashboard()
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }

            AgentDashBoardViewModel vm = new AgentDashBoardViewModel();
            vm.AgentCurrency = Common.Common.GetCountryCurrency(agentInfo.CountryCode);
            vm.AgentCurrencySymbol = Common.Common.GetCurrencySymbol(agentInfo.CountryCode);
            vm.AUXAgentAccountBalance = CommonServices.getAuxAgentAccountBalance(agentInfo.Id, Common.AgentSession.LoggedUser.PayingAgentStaffId);
            vm.ExchangeRate = CommonServices.GetExchanegRateList(agentInfo.Id);

            return View(vm);
        }
        public ActionResult ClearAlert()
        {

            Session.Remove("FirstLogin");
            return RedirectToAction("Index");
        }

        public JsonResult GetAlertsFullDetails(int id)
        {

            AgentServices.AlertServices alertServices = new AgentServices.AlertServices();
            var result = alertServices.GetAlertsDetialsById(id);
            return Json(new
            {
                AlertHeading = result.Heading,
                AlertFullMessage = result.FullMessage,
                AlertPhoto = result.PhotoUrl
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GoToTransactionSatement()
        {
            if (!agentInfo.IsAUXAgent)
            {
                return RedirectToAction("DailyTransactionStatement", "AgentDashboard");
            }
            else
            {
                return RedirectToAction("DailyAuxTransactionStatement", "AgentDashboard");
            }
        }
        public ActionResult DailyTransactionStatement(string referenceNumber = "", int day = 0, int month = 0, int year = 0, int transactionType = 0, int hideButton = 0)
        {

            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            if (Common.AgentSession.AgentStaffLogin.AgentStaff.AgentStaffType == StaffType.Transaction)
            {
                return RedirectToAction("Index", "AgentDashboard");
            }
            int PayingStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId;

            ViewBag.Days = new SelectList(Enumerable.Range(1, 32));
            ViewBag.Years = new SelectList(Enumerable.Range(2018, 10));
            DailyTransactionStatementViewModel vm = new DailyTransactionStatementViewModel();
            vm.AccountBalance = _dailyTransactionStatementServices.getAgentAccountBalance(agentInfo.Id, PayingStaffId);
            vm.AgentCurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(Common.AgentSession.AgentInformation.CountryCode);
            vm.TransactionList = _dailyTransactionStatementServices.getDailyTransactionStatementList(PayingStaffId);
            vm.Commission = CommonServices.Get30DaysAgentCommission(Common.AgentSession.LoggedUser.PayingAgentStaffId);

            if (!string.IsNullOrEmpty(referenceNumber))
            {
                vm.ReferenceNumber = referenceNumber;
            }
            else
            {
                vm.ReferenceNumber = _dailyTransactionStatementServices.generateReferenceNumber();
            }
            if (day != 0)
            {
                vm.Day = day;
                vm.TransactionList = vm.TransactionList.Where(x => x.DateAndTime.Day == day).ToList();
            }
            if (month != 0)
            {
                vm.Month = (Month)month;
                vm.TransactionList = vm.TransactionList.Where(x => x.DateAndTime.Month == month).ToList();
            }
            if (year != 0)
            {
                vm.Year = year;
                vm.TransactionList = vm.TransactionList.Where(x => x.DateAndTime.Year == year).ToList();
            }
            if (transactionType != 0)
            {
                vm.TransactionType = (TransactionType)transactionType;
                vm.TransactionList = vm.TransactionList.Where(x => x.TransactionType == (TransactionType)transactionType).ToList();
            }

            for (int i = 0; i < vm.TransactionList.Count(); i++)
            {
                vm.TransactionList[i].FormatedDate = vm.TransactionList[i].DateAndTime.ToString("dd/MM/yyyy HH:mm");
            }
            vm.TransactionList.OrderBy(x => x.DateAndTime);
            if (hideButton == 1)
            {
                ViewBag.HideButton = 1;
            }
            else
            {
                ViewBag.HideButton = 0;
            }
            return View(vm);
        }
        public ActionResult DailyAuxTransactionStatement(string receivingCountry = "", string status = "", string DateRange = "",
             int? page = null, int PageSize = 10, int CurrentpageCount = 0)
       {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            if (Common.AgentSession.AgentStaffLogin.AgentStaff.AgentStaffType == StaffType.Transaction)
            {
                return RedirectToAction("GoToDashboard", "AgentDashboard");
            }
            int PayingStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            var ReceivingCountries = Common.Common.GetReceivingCountries();
            ViewBag.ReceivingCountries = new SelectList(ReceivingCountries, "CountryCode", "CountryName", receivingCountry);

            DailyTransactionStatementViewModel vm = new DailyTransactionStatementViewModel();
            SenderTransactionSearchParamVm searchparam = new SenderTransactionSearchParamVm()
            {
                senderId = PayingStaffId,
                ReceivingCountry = receivingCountry,
                Status = status,
                DateRange = DateRange,
                PageSize = PageSize,
                PageNum = page ?? 1,
            };
            ViewBag.PageSize = PageSize;
            ViewBag.DateRange = DateRange;
            ViewBag.CurrentpageCount = CurrentpageCount;
            ViewBag.NumberOfPage = page ?? 1;
            ViewBag.PageNumber = page ??1;
            ViewBag.ButtonCount = 0;
            vm.TransactionList = _dailyTransactionStatementServices.GetAuxAgentDailyTransactionStatement(searchparam);
            if (vm.TransactionList.Count != 0)
            {
                var TotalCount = vm.TransactionList.FirstOrDefault().TotalCount;
                int NumberOfPage = Common.Common.GetNumberOfPage(TotalCount, PageSize);
                ViewBag.NumberOfPage = NumberOfPage;
                var numberofbuttonshown = NumberOfPage - CurrentpageCount;
                ViewBag.ButtonCount = numberofbuttonshown;
            }
            return View(vm);
        }


        public ActionResult AgentTransactionDetail(int id, TransactionType transactionService, Models.Type Type, bool isSystemSummary = false)
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }

            int AgentId = agentInfo.Id;
            ViewBag.AgentId = AgentId;
            ViewBag.IsSystemSummary = isSystemSummary;
            int agentStaffId = _dailyTransactionStatementServices.GetPayingStaffId(AgentId);
            AgentTransactionHistoryViewModel vm = _dailyTransactionStatementServices.GetTransactionHistories(transactionService, agentStaffId);
            vm.TransactionHistoryList = vm.TransactionHistoryList.Where(x => x.Id == id).ToList();
            vm.FilterKey = transactionService;
            vm.FilterType = Type;
            string fullName = vm.TransactionHistoryList.Where(x => x.Id == id).Select(x => x.ReceiverName).FirstOrDefault();
            string firstName = "";
            if (!string.IsNullOrEmpty(fullName))
            {
                var names = fullName.Split(' ');
                firstName = names[0];
            }

            ViewBag.ReceiverFirstName = firstName;
            return View(vm);
        }

        public ActionResult RepeatCashPickUpTransfer(string ReceiptNumber, string MobileNo)
        {
            KiiPayReceiverDetailsInformationViewModel vm = new KiiPayReceiverDetailsInformationViewModel();
            SendMoneToKiiPayWalletViewModel vm2 = new SendMoneToKiiPayWalletViewModel();
            vm2 = _KiiPayWalletTransferServices.getFaxerInfo(MobileNo, vm2);
            vm = _KiiPayWalletTransferServices.GetReceiverDetailsFromReceiptNumber(ReceiptNumber);
            _KiiPayWalletTransferServices.SetKiiPayReceiverDetailsInformationViewModel(vm);
            _KiiPayWalletTransferServices.SetSendMoneToKiiPayWalletViewModel(vm2);
            return RedirectToAction("SendMoneyToKiiPayEnterAmount", "AgentKiiPayWalletTransfer");
        }
        public ActionResult RepeatOtherMobileWallets(string ReceiptNumber, string MobileNo)
        {
            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();
            ReceiverDetailsInformationViewModel vm2 = new ReceiverDetailsInformationViewModel();

            vm = _sCashPickUpTransferService.getFaxer(MobileNo, vm);
            vm2 = _sAgentMobileTransferWallet.GetReceiverDetailsFromReceiptNumber(ReceiptNumber);
            _sCashPickUpTransferService.SetCashPickupInformationViewModel(vm);
            _sAgentMobileTransferWallet.SetReceiverDetailsInformation(vm2);
            return RedirectToAction("MobileMoneyTransferEnterAmount", "AgentMobileMoneyTransfer");
        }
        public ActionResult BankAccountDeposit(string ReceiptNumber, string MobileNo)
        {
            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();
            SenderBankAccountDepositVm vm2 = new SenderBankAccountDepositVm();

            vm = _sCashPickUpTransferService.getFaxer(MobileNo, vm);
            vm2 = _sAgentBankAccountDeposit.GetReceiverDetailsFromReceiptNumber(ReceiptNumber);
            _sCashPickUpTransferService.SetCashPickupInformationViewModel(vm);
            _sAgentBankAccountDeposit.SetAgentBankAccountDeposit(vm2);
            return RedirectToAction("BankDepositAbroadEnterAmount", "AgentBankAccountDeposit");
        }
        public ActionResult CashPickUpTransfer(string ReceiptNumber, string MobileNo)
        {
            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();
            CashPickUpReceiverDetailsInformationViewModel vm2 = new CashPickUpReceiverDetailsInformationViewModel();
            vm = _sCashPickUpTransferService.getFaxer(MobileNo, vm);
            vm2 = _sCashPickUpTransferService.GetReceiverDetailsFromReceiptNumber(ReceiptNumber);
            _sCashPickUpTransferService.SetCashPickupInformationViewModel(vm);
            _sCashPickUpTransferService.SetCashPickUpReceiverInfoViewModel(vm2);
            return RedirectToAction("CashPickUpEnterAmount", "AgentCashPickUpTransfer");
        }
        public FileContentResult DownloadStatement(string referenceNumber = "", int day = 0, int month = 0, int year = 0, int transactionType = 0)
        {
            int PayingStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var statementURL = baseUrl + "/EmailTemplate/AgentTransactionStatement/TransactionStatment?referenceNumber=" + referenceNumber + "&day=" + day + "&month=" + month + "&year=" + year + "&transactionType=" + transactionType + "&hideButton=1" + "&agentId=" + agentInfo.Id + "&PayingAgentStaffId=" + Common.AgentSession.LoggedUser.PayingAgentStaffId;
            var statementPDF = Common.Common.GetPdf(statementURL);
            byte[] bytes = statementPDF.Save();
            string mimeType = "Application/pdf";
            //Response.Buffer = true;
            //Response.Clear();
            //Response.ContentType = mimeType;
            //Response.OutputStream.Write(bytes, 0, bytes.Length);
            //Response.Flush();
            //Response.End();
            decimal accountBalance = _dailyTransactionStatementServices.getAgentAccountBalance(agentInfo.Id, PayingStaffId);
            _dailyTransactionStatementServices.saveAgentDailyTransactionStatementDetails(agentInfo.Id, referenceNumber, day, month, year, (TransactionType)transactionType, accountBalance);
            return File(bytes, "application/pdf", DateTime.Now + " " + "MoneyFex Agent Daily Transaction Statement.pdf");
        }
        public void PrintReceiptOfBankDeposit(int TransactionId, TransactionType transactionService, int AgentId = 0)
        {
            int agentStaffId = _dailyTransactionStatementServices.GetPayingStaffId(AgentId);
            AgentTransactionHistoryViewModel vm = _dailyTransactionStatementServices.GetTransactionHistories(transactionService, agentStaffId);
            var transactionHistory = vm.TransactionHistoryList.Where(x => x.Id == TransactionId).FirstOrDefault();

            if (transactionHistory == null)
            {
                transactionHistory = _dailyTransactionStatementServices.GetManualBankAccountDepositTrasactionDetails(TransactionId).FirstOrDefault();

            }


            string name = transactionHistory.ReceiverName;
            string firstname = name.Split(' ').First();
            string ReceiverPhoneNo = Common.Common.GetCountryPhoneCode(Common.Common.GetCountryCodeByCountryName(transactionHistory.ReceiverCountry)) + " " + transactionHistory.ReceiverNumber;
            string SenderPhoneNo = Common.Common.GetCountryPhoneCode(Common.Common.GetCountryCodeByCountryName(transactionHistory.SenderCountry)) + " " + transactionHistory.SenderNumber;

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptURL = baseUrl + "/EmailTemplate/AgentTransactionStatement/PrintBankDepositStatement?MFReceiptNumber=" + transactionHistory.ReceiptNumber +
             "&TransactionDate=" + transactionHistory.TransactionDate +
             "&TransactionTime=" + transactionHistory.TransactionDate +
             "&SenderFullName=" + transactionHistory.SenderName +
             "&SenderEmail=" + transactionHistory.SenderEmail +
             "&SenderTelephone=" + SenderPhoneNo +
             "&SenderDOB=" + transactionHistory.SenderDOB +
             "&ReceiverFullName=" + transactionHistory.ReceiverName +
             "&ReceiverAccount=" + transactionHistory.AccountNumber +
             "&ReceiverTelephone=" + ReceiverPhoneNo +
             "&AgentName=" + transactionHistory.AgentName +
             "&AgentAcountNumber=" + transactionHistory.AgentNumber +
             "&StaffName=" + transactionHistory.TransactionStaff +

             "&ReceiverCountry=" + transactionHistory.ReceiverCountry +
             "&TotalAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.AmountPaid +
             "&Fee=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.Fee +
             "&SendingAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.AmountSent +
             "&ReceivingAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.ReceivingAmount + transactionHistory.SendingCurrency +

             "&ExchangeRate=" + transactionHistory.ExchangeRate +
             "&SendingCurrency=" + transactionHistory.SendingCurrency +
             "&ReceivingCurrency=" + transactionHistory.ReceivingCurrrency +
             "&ReceiverName=" + firstname +
             "&PaymentMethod=" + transactionHistory.PaymentMethod +
             "&BankName=" + transactionHistory.BankName +
             "&BankBranch=" + transactionHistory.BankBranch;


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
        public void PrintReceiptOfCashPickUp(int TransactionId, TransactionType transactionService, int AgentId = 0)
        {
            int agentStaffId = _dailyTransactionStatementServices.GetPayingStaffId(AgentId);

            AgentTransactionHistoryViewModel vm = _dailyTransactionStatementServices.GetTransactionHistories(transactionService, agentStaffId);
            var transactionHistory = vm.TransactionHistoryList.Where(x => x.Id == TransactionId).FirstOrDefault();

            string name = transactionHistory.ReceiverName;
            string firstname = name.Split(' ').First();



            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptURL = baseUrl + "/EmailTemplate/AgentTransactionStatement/PrintCashPickUpStatement?MFReceiptNumber=" + transactionHistory.ReceiptNumber +
             "&TransactionDate=" + transactionHistory.TransactionDate +
             "&TransactionTime=" + transactionHistory.TransactionDate +
             "&SenderFullName=" + transactionHistory.SenderName +
             "&SenderEmail=" + transactionHistory.SenderEmail +
             "&SenderTelephone=" + Common.Common.GetCountryPhoneCode(transactionHistory.ReceiverCountry) + " " + transactionHistory.SenderNumber +
             "&SenderDOB=" + transactionHistory.SenderDOB +
             "&ReceiverFullName=" + transactionHistory.ReceiverName +
             "&ReceiverAccount=" + transactionHistory.MFCN +
             "&ReceiverTelephone=" + Common.Common.GetCountryPhoneCode(transactionHistory.ReceiverCountry) + " " + transactionHistory.ReceiverNumber +
             "&AgentName=" + transactionHistory.AgentName +
             "&AgentAcountNumber=" + transactionHistory.AgentNumber +
             "&StaffName=" + transactionHistory.TransactionStaff +

             "&ReceiverCountry=" + transactionHistory.ReceiverCountry +
             "&TotalAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.AmountPaid +
             "&Fee=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.Fee +
             "&SendingAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.AmountSent +
             "&ReceivingAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.ReceivingAmount + transactionHistory.SendingCurrency +

             "&ExchangeRate=" + transactionHistory.ExchangeRate +
             "&SendingCurrency=" + transactionHistory.SendingCurrency +
             "&ReceivingCurrency=" + transactionHistory.ReceivingCurrrency +
             "&ReceiverName=" + firstname +
             "&PaymentMethod=" + transactionHistory.PaymentMethod;

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
        public void PrintReceiptOfKiiPayWallet(int TransactionId, TransactionType transactionService, int AgentId = 0)
        {
            int agentStaffId = _dailyTransactionStatementServices.GetPayingStaffId(AgentId);


            AgentTransactionHistoryViewModel vm = _dailyTransactionStatementServices.GetTransactionHistories(transactionService, agentStaffId);
            var transactionHistory = vm.TransactionHistoryList.Where(x => x.Id == TransactionId).FirstOrDefault();

            string name = transactionHistory.ReceiverName;
            string firstname = name.Split(' ').First();



            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptURL = baseUrl + "/EmailTemplate/AgentTransactionStatement/PrintKiiPayStatement?MFReceiptNumber=" + transactionHistory.ReceiptNumber +
             "&TransactionDate=" + transactionHistory.TransactionDate +
             "&TransactionTime=" + transactionHistory.TransactionDate +
             "&SenderFullName=" + transactionHistory.SenderName +
             "&SenderEmail=" + transactionHistory.SenderEmail +
             "&SenderTelephone=" + Common.Common.GetCountryPhoneCode(transactionHistory.ReceiverCountry) + " " + transactionHistory.SenderNumber +
             "&SenderDOB=" + transactionHistory.ReceiverDOB +
             "&ReceiverFullName=" + transactionHistory.ReceiverName +
             "&ReceiverEmail=" + transactionHistory.ReceiverEmail +
             "&ReceiverAccount=" + transactionHistory.ReceiverNumber +
             "&ReceiverTelephone=" + Common.Common.GetCountryPhoneCode(transactionHistory.ReceiverCountry) + " " + transactionHistory.ReceiverNumber +
             "&AgentName=" + transactionHistory.AgentName +
             "&AgentAcountNumber=" + transactionHistory.AgentNumber +
             "&StaffName=" + transactionHistory.TransactionStaff +

             "&ReceiverCountry=" + transactionHistory.ReceiverCountry +
             "&TotalAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.AmountPaid +
             "&Fee=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.Fee +
             "&SendingAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.AmountSent +
             "&ReceivingAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.ReceivingAmount + transactionHistory.SendingCurrency +

             "&ExchangeRate=" + transactionHistory.ExchangeRate +
             "&SendingCurrency=" + transactionHistory.SendingCurrency +
             "&ReceivingCurrency=" + transactionHistory.ReceivingCurrrency +
             "&ReceiverName=" + firstname +
             "&PaymentMethod=" + transactionHistory.PaymentMethod;

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
        public void PrintReceiptOfOtherWallet(int TransactionId, TransactionType transactionService, int AgentId = 0)
        {
            int agentStaffId = _dailyTransactionStatementServices.GetPayingStaffId(AgentId);


            AgentTransactionHistoryViewModel vm = _dailyTransactionStatementServices.GetTransactionHistories(transactionService, agentStaffId);
            var transactionHistory = vm.TransactionHistoryList.Where(x => x.Id == TransactionId).FirstOrDefault();
            string name = transactionHistory.ReceiverName;
            string firstname = name.Split(' ').First();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptURL = baseUrl + "/EmailTemplate/AgentTransactionStatement/PrintOtherWalletStatement?MFReceiptNumber=" + transactionHistory.ReceiptNumber +
             "&TransactionDate=" + transactionHistory.TransactionDate +
             "&TransactionTime=" + transactionHistory.TransactionDate +
             "&SenderFullName=" + transactionHistory.SenderName +
             "&SenderEmail=" + transactionHistory.SenderEmail +
             "&SenderTelephone=" + Common.Common.GetCountryPhoneCode(transactionHistory.ReceiverCountry) + " " + transactionHistory.SenderNumber +
             "&SenderDOB=" + transactionHistory.SenderDOB +
             "&ReceiverFullName=" + transactionHistory.ReceiverName +
             "&ReceiverTelephone=" + Common.Common.GetCountryPhoneCode(transactionHistory.ReceiverCountry) + " " + transactionHistory.ReceiverNumber +
             "&AgentName=" + transactionHistory.AgentName +
             "&AgentAcountNumber=" + transactionHistory.AgentNumber +
             "&StaffName=" + transactionHistory.TransactionStaff +
             "&ReceiverCountry=" + transactionHistory.ReceiverCountry +
             "&TotalAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.AmountPaid +
             "&Fee=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.Fee +
             "&SendingAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.AmountSent +
             "&ReceivingAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.ReceivingAmount + transactionHistory.ReceivingCurrrency +
             "&ExchangeRate=" + transactionHistory.ExchangeRate +
             "&SendingCurrency=" + transactionHistory.SendingCurrency +
             "&ReceivingCurrency=" + transactionHistory.ReceivingCurrrency +
             "&ReceiverName=" + firstname +
             "&PaymentMethod=" + transactionHistory.PaymentMethod;

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
        public void PrintReceiptOfPayBillTopUp(int TransactionId, TransactionType transactionService, int AgentId = 0)
        {
            int agentStaffId = _dailyTransactionStatementServices.GetPayingStaffId(AgentId);


            AgentTransactionHistoryViewModel vm = _dailyTransactionStatementServices.GetTransactionHistories(transactionService, agentStaffId);
            var transactionHistory = vm.TransactionHistoryList.Where(x => x.Id == TransactionId).FirstOrDefault();
            string name = transactionHistory.ReceiverName;
            string firstname = name.Split(' ').First();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptURL = baseUrl + "/EmailTemplate/AgentTransactionStatement/PrintPayBillTopUpStatement?MFReceiptNumber=" + transactionHistory.ReceiptNumber +
             "&TransactionDate=" + transactionHistory.TransactionDate +
             "&Name=" + transactionHistory.ReceiverName +
             "&WalletNumber=" + transactionHistory.ReceiverNumber +
             "&CustomerAccountNumber=" + transactionHistory.AccountNumber +
             "&AgentName=" + transactionHistory.AgentName +
             "&AgentAcountNumber=" + transactionHistory.AgentNumber +
             "&StaffName=" + transactionHistory.TransactionStaff +
             "&TotalAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.AmountPaid +
             "&Fee=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.Fee +
             "&SendingAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.AmountSent +
             "&ReceivingAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.ReceivingAmount + transactionHistory.SendingCurrency +
             "&ExchangeRate=" + transactionHistory.ExchangeRate +
             "&SendingCurrency=" + transactionHistory.SendingCurrency +
             "&ReceivingCurrency=" + transactionHistory.ReceivingCurrrency +
             "&ReceiverName=" + firstname +
             "&PaymentMethod=" + transactionHistory.PaymentMethod;

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


    }
}