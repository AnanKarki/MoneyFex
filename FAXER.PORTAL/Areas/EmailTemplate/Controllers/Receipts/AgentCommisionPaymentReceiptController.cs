using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class AgentCommisionPaymentReceiptController : Controller
    {
        // GET: EmailTemplate/AgentCommisionPaymentReceipt
        public ActionResult Index(string ReceiptNumber , string AgentName , string AgentMFCode , string TransferredCommision 
            , string ReceivedCommission, string TotalCommission , string StaffName , string Date , string Time, string AgentCurrency ,string CommisionPaymentType)
        {
            ViewBag.ReceiptNumber = ReceiptNumber;
            ViewBag.AgentName = AgentName;
            ViewBag.AgentMFCode = AgentMFCode;
            ViewBag.TransferredCommision = TransferredCommision + " " + AgentCurrency;
            ViewBag.ReceivedCommision = ReceivedCommission + " " + AgentCurrency;
            ViewBag.TotalCommision = TotalCommission + " " + AgentCurrency;
            ViewBag.StaffName = StaffName;
            ViewBag.Date = Date;
            ViewBag.Time = Time;
            string val = "";
            if (!string.IsNullOrEmpty(CommisionPaymentType)) {
                val = CommisionPaymentType.Substring(CommisionPaymentType.Length - 3, 3);
            }

            ViewBag.CommisionPaymentType = "Admin-" +  val;
            return View();
        }
    }
}