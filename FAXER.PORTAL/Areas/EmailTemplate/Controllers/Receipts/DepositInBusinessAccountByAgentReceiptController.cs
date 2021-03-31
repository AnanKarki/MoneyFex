using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class DepositInBusinessAccountByAgentReceiptController : Controller
    {
        // GET: EmailTemplate/DepositInBusinessAccountByAgentReceipt
        public ActionResult Index(string ReceiptNumber, string TransactionDate, string TransactionTime, string SenderFullName, string ReceiverFullName
       , string Telephone, string AgentName, string AgentCode, string AmountSent, string ExchangeRate, string Fee, string AmountReceived, string TotalAmountSentAndFee
            , string SendingCurrency, string ReceivingCurrency)
        {
            ViewBag.ReceiptNumber = ReceiptNumber;
            ViewBag.TransactionDate = TransactionDate;
            ViewBag.TransactionTime = TransactionTime;
            ViewBag.SenderFullName = SenderFullName;
            ViewBag.ReceiverFullName = ReceiverFullName;
            ViewBag.Telephone = Telephone;
            ViewBag.AgentName = AgentName;
            ViewBag.AgentCode = AgentCode;
            ViewBag.TotalAmountSentAndFee = TotalAmountSentAndFee;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.AmountSent = AmountSent;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.Fee = Fee;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.AmountReceived = AmountReceived;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            return View();
        }
    }
}