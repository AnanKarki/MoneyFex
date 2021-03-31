using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers.Staff_Transactions
{
    public class StaffTransactionHistoryController : Controller
    {
        SCashPickUpTransferService _cashPickUp = new SCashPickUpTransferService();
        StaffTransactionHistoryServices _Services = null;
        public StaffTransactionHistoryController()
        {
            _Services = new StaffTransactionHistoryServices();
        }
        // GET: Admin/StaffTransactionHistory
        public ActionResult Index(int Services = 0,int Month=0,int Year=0,int Day=0, string StaffName = "", string Identifier = "", int? page=null)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }


            ViewBag.Years = new SelectList(Enumerable.Range(2018, 10));
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.StaffName = StaffName;
            ViewBag.Identifier = Identifier;
            IPagedList<DailyTransactionStatementListVm> vm = _Services.GetStaffTransactionStatementList().ToPagedList(pageNumber,pageSize);
            
            if(Services!=0)
            {
                ViewBag.TransferMethod = Services;
                vm=vm.Where(x => x.TransactionType == (TransactionType)Services).ToPagedList(pageNumber, pageSize);
            }
            if(Month!=0)
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
            if (!string.IsNullOrEmpty(StaffName))
            {
                StaffName = StaffName.Trim();
                vm = vm.Where(x => x.StaffName.ToLower().Contains(StaffName.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(Identifier))
            {
                Identifier = Identifier.Trim();
                vm = vm.Where(x => x.TransactionIdentifier.ToLower().Contains(Identifier.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            return View(vm);
        }
        public ActionResult TransactionDetails(int id, TransactionType transactionService, Agent.Models.Type Type)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            AgentTransactionHistoryViewModel vm = _Services.GetTransactionHistories(transactionService,id);
            vm.TransactionHistoryList = vm.TransactionHistoryList.Where(x => x.Id == id).ToList();
            vm.FilterKey = transactionService;
            vm.FilterType = Type;
            string fullName = vm.TransactionHistoryList.Where(x => x.Id == id).Select(x => x.ReceiverName).FirstOrDefault();
            string firstName = "";
            if (fullName != null)
            {
                var names = fullName.Split(' ');
                firstName = names[0];
            }
            ViewBag.ReceiverFirstName = firstName;
            return View(vm);
        }


        public ActionResult RepeatCashPickUpTransfer(string ReceiptNumber, string MobileNo)
        {    
            CashPickUpReceiverDetailsInformationViewModel vm = new CashPickUpReceiverDetailsInformationViewModel();
            CashPickupInformationViewModel vm2 = new CashPickupInformationViewModel();
            vm2 = _cashPickUp.getFaxer(MobileNo, vm2);
            vm = _cashPickUp.GetReceiverDetailsFromReceiptNumber(ReceiptNumber);
            _cashPickUp.SetStaffCashPickUpReceiverInfoViewModel(vm);
            _cashPickUp.SetStaffCashPickupInformationViewModel(vm2);
            return RedirectToAction("CashPickUpEnterAmount", "StaffCashPickUpTransfer");
        }
        public ActionResult RepeatOtherMobileWallets(string ReceiptNumber, string MobileNo)
        {
            SAgentMobileTransferWallet _sAgentMobileTransferWalletServices = new SAgentMobileTransferWallet();
            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();
            ReceiverDetailsInformationViewModel vm2 = new ReceiverDetailsInformationViewModel();

            vm = _cashPickUp.getFaxer(MobileNo, vm);
            vm2 = _sAgentMobileTransferWalletServices.GetReceiverDetailsFromReceiptNumber(ReceiptNumber);
            _cashPickUp.SetStaffCashPickupInformationViewModel(vm);
            _sAgentMobileTransferWalletServices.SetStaffReceiverDetailsInformation(vm2);
            return RedirectToAction("MobileWalletEnterAmount", "StaffOtherMobileWalletsTransfer");
        }
        public ActionResult RepeatBankAccountDeposit(string ReceiptNumber, string MobileNo)
        {
            SAgentBankAccountDeposit _sAgentBankAccountDeposit = new SAgentBankAccountDeposit();
            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();
            SenderBankAccountDepositVm vm2 = new SenderBankAccountDepositVm();

            vm = _cashPickUp.getFaxer(MobileNo, vm);
            vm2 = _sAgentBankAccountDeposit.GetReceiverDetailsFromReceiptNumber(ReceiptNumber);
            _cashPickUp.SetStaffCashPickupInformationViewModel(vm);
            _sAgentBankAccountDeposit.SetAgentBankAccountDeposit(vm2);
            return RedirectToAction("BankAccountDepositEnterAmount", "StaffBankAccountDeposit");
        }
        public ActionResult RepeatKiiPayWallet(string ReceiptNumber, string MobileNo)
        {
            SAgentKiiPayWalletTransferServices _kiiPayWalletTransfer = new SAgentKiiPayWalletTransferServices();
            SendMoneToKiiPayWalletViewModel vm = new SendMoneToKiiPayWalletViewModel();
            KiiPayReceiverDetailsInformationViewModel vm2 = new KiiPayReceiverDetailsInformationViewModel();
            vm = _kiiPayWalletTransfer.getFaxerInfo(MobileNo, vm);
            vm2 = _kiiPayWalletTransfer.GetReceiverDetailsFromReceiptNumber(ReceiptNumber);
            _kiiPayWalletTransfer.SetAdminSendMoneToKiiPayWalletViewModel(vm);
            _kiiPayWalletTransfer.SetAdminKiiPayReceiverDetailsInformationViewModel(vm2);
            return RedirectToAction("KiiPayWalletEnterAmount", "StaffKiiPayWallet");
        }


        public void PrintReceiptOfBankDeposit(int TransactionId, TransactionType transactionService)
        {
          
            AgentTransactionHistoryViewModel vm = _Services.GetTransactionHistories(transactionService, TransactionId);
            var transactionHistory = vm.TransactionHistoryList.Where(x => x.Id == TransactionId).FirstOrDefault();

          


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
        public void PrintReceiptOfCashPickUp(int TransactionId, TransactionType transactionService)
        {

            AgentTransactionHistoryViewModel vm = _Services.GetTransactionHistories(transactionService, TransactionId);
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
        public void PrintReceiptOfKiiPayWallet(int TransactionId, TransactionType transactionService)
        {
            
            AgentTransactionHistoryViewModel vm = _Services.GetTransactionHistories(transactionService, TransactionId);
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
        public void PrintReceiptOfOtherWallet(int TransactionId, TransactionType transactionService)
        {


            AgentTransactionHistoryViewModel vm = _Services.GetTransactionHistories(transactionService, TransactionId);
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
    }
}