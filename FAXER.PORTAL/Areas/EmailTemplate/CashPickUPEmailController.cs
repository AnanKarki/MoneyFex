using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate
{
    public class CashPickUPEmailController : Controller
    {
        // GET: EmailTemplate/CashPickUPEmail
        public ActionResult Index(string senderName = "", string receiverName = "", string receiverFirstName = "",
                                            string MFCN = "", string sendingAmount = "", string ReceivingAmount = "",
                                            string Fee = "", string receivingCountry = "", string PaymentReference = "", SenderPaymentMode SenderPaymentMode = SenderPaymentMode.Cash)
        {
            ViewBag.SenderFirstName = senderName;
            ViewBag.receiverFullName = receiverName;
            ViewBag.ReceiverFirstName = receiverFirstName;
            ViewBag.MFCN = MFCN;
            ViewBag.sendingAmount = sendingAmount;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.Fee = Fee;
            ViewBag.receivingCountry = receivingCountry;
            ViewBag.PaymentReference = PaymentReference;
            ViewBag.SenderPaymentMode = SenderPaymentMode;
            return View();
        }
    }
}