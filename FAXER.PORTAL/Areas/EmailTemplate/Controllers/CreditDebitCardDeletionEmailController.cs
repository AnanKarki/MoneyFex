using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class CreditDebitCardDeletionEmailController : Controller
    {
        // GET: EmailTemplate/CreditDebitCardDeletionEmail
        public ActionResult Index(string FaxerName, string CreditDebitCardLast4Digits , string AddNewCreditOrDebitCard)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.CreditDebitCardLast4Digits = CreditDebitCardLast4Digits;
            ViewBag.AddNewCreditOrDebitCard = AddNewCreditOrDebitCard;
            return View();
        }
    }
}