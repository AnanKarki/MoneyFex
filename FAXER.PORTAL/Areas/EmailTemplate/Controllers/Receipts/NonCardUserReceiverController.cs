using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class NonCardUserReceiverController : Controller
    {
        // GET: EmailTemplate/NonCardUserReceiver
        public ActionResult Index(string MFReceiptNumber ,string TransactionDate , string TransactionTime, string FaxerFullName, string MFCN, string ReceiverFullName
            ,string Telephone , string AgentName , string AgentCode,string AmountSent , string ExchangeRate , string Fee,string AmountReceived, string SendingCurrency, string ReceivingCurrency)
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
            ViewBag.AmountSent = AmountSent + " " +SendingCurrency;
            ViewBag.ExchangeRate ="1"+ " " + SendingCurrency +" = " + ExchangeRate + " " + ReceivingCurrency;
            ViewBag.Fee = Fee + " " + SendingCurrency;
            ViewBag.TotalSentAmount = (decimal.Parse(AmountSent) + decimal.Parse(Fee)).ToString() + " " + SendingCurrency;
            ViewBag.AmountReceived = AmountReceived + " " + ReceivingCurrency;
            return View();
        }
    }
}