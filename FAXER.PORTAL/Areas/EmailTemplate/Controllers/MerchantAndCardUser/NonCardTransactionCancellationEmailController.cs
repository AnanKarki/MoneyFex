using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.MerchantAndCardUser
{
    public class NonCardTransactionCancellationEmailController : Controller
    {
        // GET: EmailTemplate/NonCardTransactionCancellationEmail
        public ActionResult Index(string SenderName , string MFCN , string Amount , string ReceiverName , string ReceiverCity , string ReceiverCountry, string TransactionDate)
        {

            ViewBag.SenderName = SenderName;
            ViewBag.MFCN = MFCN;
            ViewBag.Amount = Amount;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.Country = ReceiverCountry;
            ViewBag.City = ReceiverCity;
            ViewBag.TransactionDate = TransactionDate;
            return View();
        }
    }
}