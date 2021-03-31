using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class MoneyFexTransactionCancellationController : Controller
    {
        // GET: EmailTemplate/MoneyFexTransactionCancellation
        public ActionResult Index(string NameOfCancellar, string SenderName, string ReceiverName, string MFCN, string TransactionAmount)
        {
            ViewBag.NameOfCancellar = NameOfCancellar;
            ViewBag.SenderName = SenderName;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.MFCN = MFCN;
            ViewBag.TransactionAmount = TransactionAmount;
            return View();
        }
    }
}