using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class NonCardUserWithrawlReceiptController : Controller
    {
        // GET: EmailTemplate/NonCardUserWithrawlReceipt
        public ActionResult Index(string MFReceiptNumber, string TransactionDate, string TransactionTime, string FaxerFullName, string MFCN, string ReceiverFullName
            , string Telephone, string AgentName, string AgentCode, string AmountSent, string ExchangeRate, string Fee, string AmountReceived, string SendingCurrency, string ReceivingCurrency,
            string AgentCountry , string AgentCity , string AgentTelephoneNumber)
        {
            ViewBag.MFReceiptNumber = MFReceiptNumber;
            ViewBag.TransactionDate = TransactionDate;
            ViewBag.TransactionTime = TransactionTime;
            ViewBag.FaxerFullName = FaxerFullName;
            ViewBag.MFCN = MFCN;
            ViewBag.ReceiverFullName = ReceiverFullName;
            ViewBag.Telephone = Telephone;
            ViewBag.AgentName = AgentName;
            ViewBag.AgentCode = AgentCode;
            ViewBag.AmountSent = AmountSent + " " + SendingCurrency;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.Fee = Fee + " "  +SendingCurrency;
            ViewBag.AmountReceived = AmountReceived + " " + ReceivingCurrency;
            ViewBag.AgentCountry = AgentCountry;
            ViewBag.AgentCity = AgentCity;
            ViewBag.AgentTelephoneNumber = AgentTelephoneNumber;

            return View();
        }
    }
}