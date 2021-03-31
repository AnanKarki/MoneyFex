using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class TransactionStillInProgressController : Controller
    {
        // GET: EmailTemplate/TransactionStillInProgress
        public ActionResult Index(string SenderFristName = "", string RecipentName = "", string ReceiverCountry = "",
            string TransactionNumber = "", string BankName = "", string BankAccount = "")
        {
            ViewBag.SenderFristName = SenderFristName;
            ViewBag.RecipentName = RecipentName;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.TransactionNumber = TransactionNumber;
            ViewBag.BankName = BankName;
            ViewBag.BankAccount = BankAccount;
            return View();
        }
    }
}