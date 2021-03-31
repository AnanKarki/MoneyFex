using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts.MerchantAndCardUser
{
    public class NonCardTransferReceiptController : Controller
    {
        // GET: EmailTemplate/NonCardTransferReceipt
        public ActionResult Index(string MFReceiptNumber, string TransactionDate, string TransactionTime, string FaxerFullName, string MFCN, string ReceiverFullName
           , string Telephone, string AmountSent, string ExchangeRate, string Fee, string AmountReceived, string SendingCurrency, string ReceivingCurrency
            , string SenderTelephoneNo)
        {
            ViewBag.MFReceiptNumber = MFReceiptNumber;
            ViewBag.TransactionDate = TransactionDate;
            ViewBag.TransactionTime = TransactionTime;
            ViewBag.FaxerFullName = FaxerFullName;
            ViewBag.MFCN = MFCN;
            ViewBag.ReceiverFullName = ReceiverFullName;
            ViewBag.Telephone = Telephone;
            ViewBag.AmountSent = AmountSent + " " + SendingCurrency;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.Fee = Fee + " " + SendingCurrency;
            ViewBag.TotalSentAmount = (decimal.Parse(AmountSent) + decimal.Parse(Fee)).ToString() + " " + SendingCurrency;
            ViewBag.AmountReceived = AmountReceived + " " + ReceivingCurrency;
            ViewBag.SenderTelephoneNo = SenderTelephoneNo;

            return View();
        }
    }
}