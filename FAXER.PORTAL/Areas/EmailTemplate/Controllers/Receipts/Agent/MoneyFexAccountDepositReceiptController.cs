using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts.Agent
{
    public class MoneyFexAccountDepositReceiptController : Controller
    {
        // GET: EmailTemplate/MoneyFexAccountDepositReceipt
        public ActionResult Index(string ReceiptNo , string Date , string Time , string AgentName , string AgentCode , string NameOfUpdater , string DepositedAmount , string Currency)
        {

            ViewBag.ReceiptNo = ReceiptNo;
            ViewBag.Date = Date;
            ViewBag.Time = Time;
            ViewBag.AgentName = AgentName;
            ViewBag.AgentCode = AgentCode;
            ViewBag.NameOfUpdater = NameOfUpdater;
            ViewBag.DepositedAmount = DepositedAmount;
            ViewBag.Currency = Currency;
            return View();
        }
    }
}