using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class AgentRefundReceiptController : Controller
    {
        // GET: EmailTemplate/AgentRefundReceipt
        public ActionResult Index(string ReceiptNumber , string TransactionReceiptNumber , string Date , string Time , string SenderFullName , string MFCN , string ReceiverFullName , string Telephone , string RefundingAgentName 
            , string RefundingAgentCode , string OrignalAmountSent , string RefundedAmount ,
            string AgentCountry , string AgentCity , string AgentTelephoneNumber)
        {
            ViewBag.ReceiptNumber = ReceiptNumber;
            ViewBag.TransactionReceiptNumber = TransactionReceiptNumber;
            ViewBag.Date = Date;
            ViewBag.Time = Time;    
            ViewBag.SenderFullName = SenderFullName;
            ViewBag.MFCN = MFCN;
            ViewBag.ReceiverFullName = ReceiverFullName;
            ViewBag.Telephone = Telephone;
            ViewBag.RefundingAgentName = RefundingAgentName;
            ViewBag.RefundingAgentCode = RefundingAgentCode;
            ViewBag.OriginalSentAmount = OrignalAmountSent;
            ViewBag.RefundedAmount = RefundedAmount;
            ViewBag.AgentCountry = AgentCountry;
            ViewBag.AgentCity = AgentCity;
            ViewBag.AgentTelephoneNumber = AgentTelephoneNumber;

            return View();
        }
    }
}