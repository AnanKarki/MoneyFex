using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class AgentMoneySenderReceiptController : Controller
    {
        // GET: EmailTemplate/AgentMoneySenderReceipt
        public ActionResult Index(string MFReceiptNumber, string TransactionDate, string TransactionTime, string FaxerFullName, string MFCN, string ReceiverFullName
       , string Telephone, string AgentName, string AgentCode, string AmountSent, string ExchangeRate, string Fee, string AmountReceived , string TotalAmountSentAndFee
            ,string  SendingCurrency , string ReceivingCurrency)
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
            ViewBag.AmountSent = AmountSent;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.Fee = Fee;
            ViewBag.AmountReceived = AmountReceived;
            ViewBag.TotalAmountSentAndFee = TotalAmountSentAndFee;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            return View();
        }
    }
}