using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class SenderMobileMoneyTranferEmailController : Controller
    {
        // GET: EmailTemplate/SenderMobileMoneyTranferEmail
        public ActionResult Index(string ReceiverName, string ReceiptNo, string ReceivingCountry, string SendingAmount,
                                            string WalletName, string PaidToMobileNo, string Fee, string ReceiverFirstName, string ReceivingAmount , string SenderFirstName,
                                            string PaymentReference, SenderPaymentMode senderPaymentMode , bool IsPaid = true)
        {
            ViewBag.SenderFirstName = SenderFirstName;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.ReceiptNo = ReceiptNo;
            ViewBag.ReceivingCountry = ReceivingCountry;
            ViewBag.SendingAmount = SendingAmount;
            ViewBag.WalletName = WalletName;
            ViewBag.PaidToMobileNo = PaidToMobileNo;
            ViewBag.Fee = Fee;
            ViewBag.ReceiverFirstName = ReceiverFirstName;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.PaymentReference = PaymentReference;
            ViewBag.senderPaymentMode = senderPaymentMode;
            ViewBag.IsPaid = IsPaid;
            return View();
        }
    }
}