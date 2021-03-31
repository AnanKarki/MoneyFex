using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class FaxCancellationEmailController : Controller
    {
        // GET: EmailTemplate/FaxCancellationEmail
        public ActionResult Index(string FaxerName, string MFCN, string SentAmount, string ReceiverName, string ReceiverCountry, string ReceiverCity, string SentDate)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.MFCN = MFCN;
            ViewBag.SentAmount = SentAmount;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.ReceiverCity = ReceiverCity;
            ViewBag.SentDate = SentDate;
            return View();
        }
    }
}