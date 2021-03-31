using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class NewCreditDebitCardAddedEmailController : Controller
    {
        // GET: EmailTemplate/NewCreditDebitCardAddedEmail
        public ActionResult Index(string FaxerName, string LastForDigitOfCreditOrDebitCard , string TopUpMoneyfaxCard , string SetAutoTopUp , string PayForGoodsAbroad)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.LastForDigitOfCreditOrDebitCard = LastForDigitOfCreditOrDebitCard;
            ViewBag.TopUpMoneyfaxCard = TopUpMoneyfaxCard;
            ViewBag.SetAutoTopUp = SetAutoTopUp;
            ViewBag.PayForGoodsAbroad = PayForGoodsAbroad;
            return View();
        }
    }
}