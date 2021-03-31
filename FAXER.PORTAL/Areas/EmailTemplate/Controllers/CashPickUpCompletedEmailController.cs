using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class CashPickUpCompletedEmailController : Controller
    {
        // GET: EmailTemplate/CashPickUpCompletedEmail
        public ActionResult Index(string SenderName= "", string receiverName= "", string MFCN = "",
             string SentAmount = "" , string Fee= "", string receiverFirstName = "" ,
             string receivingAmount = "" , string ReceivingCountry= "" , string ReceivingCur= "",
             string SendingCur =""   , bool IsInProgress = false)
        {


            ViewBag.SenderName = SenderName;
            ViewBag.ReceiverName = receiverName;
            ViewBag.MFCN = MFCN;
            ViewBag.SentAmount = SentAmount;
            ViewBag.SendingCur = SendingCur;
            ViewBag.Fee = Fee;
            ViewBag.ReceivingCountry = ReceivingCountry;
            ViewBag.receiverFirstName = receiverFirstName;
            ViewBag.receivingAmount = receivingAmount;
            ViewBag.ReceivingCur = ReceivingCur;
            ViewBag.IsInProgress = IsInProgress;

            return View();
        }
    }
}