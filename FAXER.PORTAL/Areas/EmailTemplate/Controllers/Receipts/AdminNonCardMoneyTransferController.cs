using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class AdminNonCardMoneyTransferController : Controller
    {
        // GET: EmailTemplate/AdminNonCardMoneyTransfer
        public ActionResult Index(string MFReceiptNumber, string TransactionDate, string TransactionTime, string FaxerFullName,  string MFCN, string ReceiverFullName
       , string Telephone, string StaffName, string StaffCode, string AmountSent, string ExchangeRate,
            string Fee, string AmountReceived, string TotalAmountSentAndFee, string SendingCurrency, string ReceivingCurrency,
            string SenderPhoneNo , string PaymentType)
        {
            ViewBag.MFReceiptNumber = MFReceiptNumber;
            ViewBag.TransactionDate = TransactionDate;
            ViewBag.TransactionTime = TransactionTime;
            ViewBag.FaxerFullName = FaxerFullName;
            ViewBag.MFCN = MFCN;
            ViewBag.ReceiverFullName = ReceiverFullName;
            ViewBag.Telephone = Telephone;
            ViewBag.StaffName = StaffName;
            ViewBag.StaffCode = StaffCode;
            ViewBag.AmountSent = AmountSent + SendingCurrency;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.Fee = Fee + SendingCurrency;
            ViewBag.AmountReceived = AmountReceived + ReceivingCurrency;
            ViewBag.TotalAmountSentAndFee = TotalAmountSentAndFee + SendingCurrency;
            string val = "";
            if (!string.IsNullOrEmpty(PaymentType))
            {

                val = PaymentType.Substring(PaymentType.Length - 3, 3);
            }
            ViewBag.PaymentType = "Admin - " + val;
            ViewBag.SenderTelephone = SenderPhoneNo;
            return View();
        }
    }
}