using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class AgentTransactionStatementController : Controller
    {
        DailyTransactionStatementServices _dailyTransactionStatementServices = new DailyTransactionStatementServices();
        // GET: EmailTemplate/AgentTransactionStatement


        public ActionResult Index(string referenceNumber = "", int day = 0, int month = 0, int year = 0, int transactionType = 0, int hideButton = 0, int agentId = 0)
        {


            var agentInfo = _dailyTransactionStatementServices.GetAgentInformation(agentId);
            int PayingAgentId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            ViewBag.Days = new SelectList(Enumerable.Range(1, 31));
            ViewBag.Years = new SelectList(Enumerable.Range(2019, 10));
            DailyTransactionStatementViewModel vm = new DailyTransactionStatementViewModel();
            vm.AccountBalance = _dailyTransactionStatementServices.getAgentAccountBalance(agentId, PayingAgentId);
            vm.AgentCurrencySymbol = Common.Common.GetCurrencySymbol(agentInfo.CountryCode);
           
            vm.TransactionList = _dailyTransactionStatementServices.getDailyTransactionStatementList(PayingAgentId);
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

            ViewBag.AgentName = agentInfo.Name;
            ViewBag.Address1 = agentInfo.Address1;
            ViewBag.Address2 = agentInfo.Address2;
            ViewBag.PhoneNo = Common.Common.GetCountryPhoneCode(agentInfo.CountryCode) + " " + agentInfo.PhoneNumber;
            ViewBag.Email = agentInfo.Email;
            ViewBag.AccountNo = agentInfo.AccountNo;
            //ViewBag.Website = agentInfo.Website;
            if (day == 0)
            {
                ViewBag.Day = "All";
            }
            else
            {
                ViewBag.Day = day;
            }
            if (month == 0)
            {
                ViewBag.Month = "All";
            }
            else
            {
                ViewBag.Month = (Month)month;
            }
            if (year == 0)
            {
                ViewBag.Year = "All";
            }
            else
            {
                ViewBag.Year = year;
            }
            if (transactionType == 0)
            {
                ViewBag.TransactionType = "All";
            }
            else
            {
                ViewBag.TransactionTypeName = Common.Common.GetEnumDescription(vm.TransactionType);
            }

            return View(vm);
        }

        public ActionResult TransactionStatment(string referenceNumber = "", int day = 0, int month = 0, int year = 0, int transactionType = 0, int hideButton = 0, int agentId = 0, int PayingAgentStaffId = 0)
        {




            var agentInfo = _dailyTransactionStatementServices.GetAgentInformation(agentId);
            ViewBag.Days = new SelectList(Enumerable.Range(1, 31));
            ViewBag.Years = new SelectList(Enumerable.Range(2019, 10));
            DailyTransactionStatementViewModel vm = new DailyTransactionStatementViewModel();
            vm.AccountBalance = _dailyTransactionStatementServices.getAgentAccountBalance(agentId , PayingAgentStaffId);
            vm.AgentCurrencySymbol = Common.Common.GetCurrencySymbol(agentInfo.CountryCode);
            vm.TransactionList = _dailyTransactionStatementServices.getDailyTransactionStatementList(PayingAgentStaffId);
            if (!string.IsNullOrEmpty(referenceNumber))
            {
                vm.ReferenceNumber = referenceNumber;
            }
            else
            {
                vm.ReferenceNumber = _dailyTransactionStatementServices.generateReferenceNumber(agentInfo.AccountNo);
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

            ViewBag.AgentName = agentInfo.Name;
            ViewBag.Address1 = agentInfo.Address1;
            ViewBag.Address2 = agentInfo.Address2;
            ViewBag.PhoneNo = Common.Common.GetCountryPhoneCode(agentInfo.CountryCode) + " " + agentInfo.PhoneNumber;
            ViewBag.Email = agentInfo.Email;
            ViewBag.AccountNo = agentInfo.AccountNo;
            //ViewBag.Website = agentInfo.Website;
            if (day == 0)
            {
                ViewBag.Day = "All";
            }
            else
            {
                ViewBag.Day = day;
            }
            if (month == 0)
            {
                ViewBag.Month = "All";
            }
            else
            {
                ViewBag.Month = (Month)month;
            }
            if (year == 0)
            {
                ViewBag.Year = "All";
            }
            else
            {
                ViewBag.Year = year;
            }
            if (transactionType == 0)
            {
                ViewBag.TransactionType = "All";
            }
            else
            {
                ViewBag.TransactionTypeName = Common.Common.GetEnumDescription(vm.TransactionType);
            }

            return View(vm);
        }

        public ActionResult PrintBankDepositStatement(string MFReceiptNumber, string TransactionDate, string TransactionTime, string SenderFullName,
        string SenderEmail, string SenderTelephone, string SenderDOB, string ReceiverFullName, string ReceiverAccount, string ReceiverTelephone,
        string AgentName, string AgentAcountNumber, string StaffName, string ReceiverCountry, string TotalAmount, string Fee,
        string SendingAmount, string ReceivingAmount, string ExchangeRate, string SendingCurrency, string ReceivingCurrency, string ReceiverName , 
        string PaymentMethod , string BankName , string BankBranch)
        {

            ViewBag.MFReceiptNumber = MFReceiptNumber;
            ViewBag.TransactionDate = TransactionDate;
            ViewBag.TransactionTime = TransactionTime;
            ViewBag.SenderFullName = SenderFullName;
            ViewBag.SenderEmail = SenderEmail;
            ViewBag.SenderTelephone = SenderTelephone;
            ViewBag.SenderDOB = SenderDOB;
            ViewBag.ReceiverFullName = ReceiverFullName;
            ViewBag.ReceiverAccount = ReceiverAccount;
            ViewBag.ReceiverTelephone = ReceiverTelephone;
            ViewBag.AgentName = AgentName;
            ViewBag.AgentAcountNumber = AgentAcountNumber;
            ViewBag.StaffName = StaffName;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.Fee = Fee;
            ViewBag.SendingAmount = TotalAmount;
            ViewBag.TotalAmount = SendingAmount;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            ViewBag.ReceiverFirstName = ReceiverName;
            ViewBag.PaymentMethod = PaymentMethod;
            ViewBag.BankName = BankName;
            ViewBag.bankBranch = BankBranch;


            return View();
        }

        public ActionResult PrintCashPickUpStatement(string MFReceiptNumber, string TransactionDate, string TransactionTime, string SenderFullName,
         string SenderEmail, string SenderTelephone, string SenderDOB, string ReceiverFullName, string ReceiverAccount, string ReceiverTelephone,
         string AgentName, string AgentAcountNumber, string StaffName, string ReceiverCountry, string TotalAmount, string Fee,
         string SendingAmount, string ReceivingAmount, string ExchangeRate, string SendingCurrency, string ReceivingCurrency, string ReceiverName ,string PaymentMethod)
        {

            ViewBag.MFReceiptNumber = MFReceiptNumber;
            ViewBag.TransactionDate = TransactionDate;
            ViewBag.TransactionTime = TransactionTime;
            ViewBag.SenderFullName = SenderFullName;
            ViewBag.SenderEmail = SenderEmail;
            ViewBag.SenderTelephone = SenderTelephone;
            ViewBag.SenderDOB = SenderDOB;
            ViewBag.ReceiverFullName = ReceiverFullName;
            ViewBag.ReceiverAccount = ReceiverAccount;
            ViewBag.ReceiverTelephone = ReceiverTelephone;
            ViewBag.AgentName = AgentName;
            ViewBag.AgentAcountNumber = AgentAcountNumber;
            ViewBag.StaffName = StaffName;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.Fee = Fee;
            ViewBag.SendingAmount = TotalAmount;
            ViewBag.TotalAmount = SendingAmount;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            ViewBag.ReceiverFirstName = ReceiverName;
            ViewBag.PaymentMethod = PaymentMethod;
            return View();
        }

        public ActionResult PrintKiiPayStatement(string MFReceiptNumber, string TransactionDate, string TransactionTime, string SenderFullName,
         string SenderEmail, string SenderTelephone, string SenderDOB, string ReceiverFullName, string ReceiverEmail, string ReceiverAccount, string ReceiverTelephone,
         string AgentName, string AgentAcountNumber, string StaffName, string ReceiverCountry, string TotalAmount, string Fee,
         string SendingAmount, string ReceivingAmount, string ExchangeRate, string SendingCurrency, string ReceivingCurrency, string ReceiverName , string PaymentMethod)
        {

            ViewBag.MFReceiptNumber = MFReceiptNumber;
            ViewBag.TransactionDate = TransactionDate;
            ViewBag.TransactionTime = TransactionTime;
            ViewBag.SenderFullName = SenderFullName;
            ViewBag.SenderEmail = SenderEmail;
            ViewBag.SenderTelephone = SenderTelephone;
            ViewBag.SenderDOB = SenderDOB;
            ViewBag.ReceiverFullName = ReceiverFullName;
            ViewBag.ReceiverEmail = ReceiverEmail;
            ViewBag.ReceiverAccount = ReceiverAccount;
            ViewBag.ReceiverTelephone = ReceiverTelephone;
            ViewBag.AgentName = AgentName;
            ViewBag.AgentAcountNumber = AgentAcountNumber;
            ViewBag.StaffName = StaffName;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.Fee = Fee;
            ViewBag.SendingAmount = TotalAmount;
            ViewBag.TotalAmount = SendingAmount;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            ViewBag.ReceiverFirstName = ReceiverName;
            ViewBag.PaymentMethod = PaymentMethod;
            return View();
        }

        public ActionResult PrintOtherWalletStatement(string MFReceiptNumber, string TransactionDate, string TransactionTime, string SenderFullName,
         string SenderEmail, string SenderTelephone, string SenderDOB, string ReceiverFullName, string ReceiverTelephone,
         string AgentName, string AgentAcountNumber, string StaffName, string ReceiverCountry, string TotalAmount, string Fee,
         string SendingAmount, string ReceivingAmount, string ExchangeRate, string SendingCurrency, string ReceivingCurrency, string ReceiverName, string PaymentMethod)
        {

            ViewBag.MFReceiptNumber = MFReceiptNumber;
            ViewBag.TransactionDate = TransactionDate;
            ViewBag.TransactionTime = TransactionTime;
            ViewBag.SenderFullName = SenderFullName;
            ViewBag.SenderEmail = SenderEmail;
            ViewBag.SenderTelephone = SenderTelephone;
            ViewBag.SenderDOB = SenderDOB;
            ViewBag.ReceiverFullName = ReceiverFullName;

            ViewBag.ReceiverTelephone = ReceiverTelephone;
            ViewBag.AgentName = AgentName;
            ViewBag.AgentAcountNumber = AgentAcountNumber;
            ViewBag.StaffName = StaffName;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.Fee = Fee;
            ViewBag.SendingAmount = TotalAmount;
            ViewBag.TotalAmount = SendingAmount;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            ViewBag.ReceiverFirstName = ReceiverName;
            ViewBag.PaymentMethod = PaymentMethod;
            return View();
        }

        public ActionResult PrintPayBillTopUpStatement(string MFReceiptNumber, string TransactionDate, string Name,
        string WalletNumber, string CustomerAccountNumber, string AgentName, string AgentAcountNumber, string StaffName,
        string TotalAmount, string Fee, string SendingAmount, string ReceivingAmount, string ExchangeRate, string SendingCurrency,
        string ReceivingCurrency, string ReceiverName, string PaymentMethod)
        {

            ViewBag.MFReceiptNumber = MFReceiptNumber;
            ViewBag.TransactionDate = TransactionDate;
            ViewBag.Name = Name;
            ViewBag.WalletNumber = WalletNumber;
            ViewBag.CustomerAccountNumber = CustomerAccountNumber;
            ViewBag.AgentName = AgentName;
            ViewBag.AgentAcountNumber = AgentAcountNumber;
            ViewBag.StaffName = StaffName;
            ViewBag.Fee = Fee;
            ViewBag.SendingAmount = TotalAmount;
            ViewBag.TotalAmount = SendingAmount;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            ViewBag.ReceiverFirstName = ReceiverName;
            ViewBag.PaymentMethod = PaymentMethod;
            return View();
        }

    }
}