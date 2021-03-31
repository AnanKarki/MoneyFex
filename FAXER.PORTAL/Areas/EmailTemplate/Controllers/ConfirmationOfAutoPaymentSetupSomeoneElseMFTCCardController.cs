using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ConfirmationOfAutoPaymentSetupSomeoneElseMFTCCardController : Controller
    {
        // GET: EmailTemplate/ConfirmationOfAutoPaymentSetupSomeoneElseMFTCCard
        public ActionResult Index(string FaxerName, string AutoPaymentAmount, string CountryCurrencySymbol, string CardUserName, string CreditORDebitCardlast4digits, string AutoPaymentFrequency, string SetAutoPayment, string TopUPMFTCCard)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.AutoPaymentAmount = AutoPaymentAmount;
            ViewBag.CountryCurrencySymbol = CountryCurrencySymbol;
            ViewBag.CardUserName = CardUserName;
            ViewBag.CreditORDebitCardlast4digits = CreditORDebitCardlast4digits;
            ViewBag.AutoPaymentFrequency = AutoPaymentFrequency;
            ViewBag.SetAutoPayment = SetAutoPayment;
            ViewBag.TopUPMFTCCard = TopUPMFTCCard;
            return View();
        }
    }
}