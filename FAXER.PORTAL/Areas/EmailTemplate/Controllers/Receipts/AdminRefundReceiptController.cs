using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class AdminRefundReceiptController : Controller
    {
        // GET: EmailTemplate/AdminRefundReceipt
        public ActionResult Index(string ReceiptNumber, string TransactionReceiptNumber, string Date, string Time, string SenderFullName, 
            string MFCN, string ReceiverFullName, string Telephone, string RefundingAdminName, string RefundingAdminCode, 
            string OrignalAmountSent, string RefundedAmount, string SendingCurrency, string ReceivingCurrency,
            string ReceiverCountry , string ReceiverCity , string RefundingType)
        {
            ViewBag.ReceiptNumber = ReceiptNumber;
            ViewBag.TransactionReceiptNumber = TransactionReceiptNumber;
            ViewBag.Date = Date;
            ViewBag.Time = Time;
            ViewBag.SenderFullName = SenderFullName;
            ViewBag.MFCN = MFCN;
            ViewBag.ReceiverFullName = ReceiverFullName;
            ViewBag.Telephone = Telephone;
            ViewBag.RefundingAdminName = RefundingAdminName;
            ViewBag.RefundingAdminCode = RefundingAdminCode;
            ViewBag.OriginalSentAmount = OrignalAmountSent + " " +  SendingCurrency;
            ViewBag.RefundedAmount = RefundedAmount +  " " +   SendingCurrency;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.ReceiverCity = ReceiverCity;

            string val = "";
            if (!string.IsNullOrEmpty(RefundingType)) {

                val = RefundingType.Substring(RefundingType.Length - 3, 3);
            }
            ViewBag.RefundingType = "Admin-" + val;

            return View();
        }
    }
}