using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class MFTCCardUserReceiverReceiptController : Controller
    {
        // GET: EmailTemplate/MFTCCardUserReceiverReceipt
        public ActionResult Index(string MFReceiptNumber, string TransactionDate, string TransactionTime, string FaxerFullName, string FaxerCountry,
            string FaxerCity, string MFTCCardNumber, string CardUserFullName, string CardUserCountry, string CardUserCity
            , string Telephone, string AgentName, string AgentCode, string AmountRequested, string ExchangeRate, string Fee, string AmountWithdrawn, string Currency,
            string AgentCountry, string AgentCity, string AgentTelphoneNumber)
        {
            ViewBag.MFReceiptNumber = MFReceiptNumber;
            ViewBag.TransactionDate = TransactionDate;
            ViewBag.TransactionTime = TransactionTime;
            ViewBag.FaxerFullName = FaxerFullName;
            ViewBag.FaxerCountry = FaxerCountry;
            ViewBag.FaxerCity = FaxerCity;
            ViewBag.MFTCCardNumber = MFTCCardNumber;
            ViewBag.CardUserFullName = CardUserFullName;
            ViewBag.CardUserCountry = CardUserCountry;
            ViewBag.CardUserCity = CardUserCity;
            ViewBag.Telephone = Telephone;
            ViewBag.AgentName = AgentName;
            ViewBag.AgentCode = AgentCode;
            ViewBag.AmountRequested = AmountRequested;
            //ViewBag.ExchangeRate = "1.0";
            ViewBag.Fee = Fee;
            ViewBag.AmountWithdrawn = AmountWithdrawn;
            ViewBag.Currency = Currency;
            ViewBag.AgentCountry = AgentCountry;
            ViewBag.AgentCity = AgentCity;
            ViewBag.AgentTelephoneNumber = AgentTelphoneNumber;

            return View();
        }


        public ActionResult KiiPayWallet(string MFReceiptNumber, string TransactionDate, string TransactionTime, string SenderFullName,
        string SenderEmail, string SenderTelephone, string SenderDOB, string ReceiverFullName, string ReceiverTelephone,
        string AgentName, string AgentAcountNumber, string StaffName, string AgentCity, string AgentCountry, string TotalAmount, string Fee,
        string SendingAmount, string ExchangeRate, string SendingCurrency, string ReceivingCurrency, string ReceivingAmount, string PaymentMethod)
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
            ViewBag.AgentCity = AgentCity;
            ViewBag.AgentCountry = AgentCountry;
            ViewBag.Fee = Fee;
            ViewBag.SendingAmount = TotalAmount;
            ViewBag.TotalAmount = SendingAmount;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.PaymentMethod = PaymentMethod;
            return View();
        }
        public ActionResult PrintReceipt(string MFReceiptNumber, string TransactionDate, string TransactionTime, string SenderFullName,
        string SenderEmail, string SenderTelephone, string SenderDOB, string ReceiverFullName, string ReceiverEmail, string ReceiverTelephone,
        string AgentName, string AgentAcountNumber, string StaffName, string AgentCity, string AgentCountry, string TotalAmount, string Fee,
        string SendingAmount, string ExchangeRate, string SendingCurrency, string ReceivingCurrency, string MFCN, string ReceivingAmount, string PaymentMethod)
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
            ViewBag.ReceiverTelephone = ReceiverTelephone;
            ViewBag.AgentName = AgentName;
            ViewBag.AgentAcountNumber = AgentAcountNumber;
            ViewBag.StaffName = StaffName;
            ViewBag.AgentCity = AgentCity;
            ViewBag.AgentCountry = AgentCountry;
            ViewBag.Fee = Fee;
            ViewBag.SendingAmount = TotalAmount;
            ViewBag.TotalAmount = SendingAmount;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            ViewBag.MFCN = MFCN;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.PaymentMethod = PaymentMethod;
            return View();
        }

        public ActionResult PrintReceiptOfBankDeposit(string MFReceiptNumber, string TransactionDate, string TransactionTime, string SenderFullName,
        string SenderEmail, string SenderTelephone, string SenderDOB, string ReceiverFullName, string ReceiverAccount, string ReceiverTelephone,
        string AgentName, string AgentAcountNumber, string StaffName, string AgentCity, string AgentCountry, string TotalAmount, string Fee,
        string SendingAmount, string ExchangeRate, string SendingCurrency, string ReceivingCurrency, string ReceivingAmount , string BankName , string BankCode , string ReceivingCountry)
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
            ViewBag.AgentCity = AgentCity;
            ViewBag.AgentCountry = AgentCountry;
            ViewBag.Fee = Fee;
            ViewBag.SendingAmount = TotalAmount;
            ViewBag.TotalAmount = SendingAmount;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.BankName = BankName;
            ViewBag.BankCode = BankCode;
            ViewBag.ReceivingCountry = ReceivingCountry;

            return View();
        }

    }
}