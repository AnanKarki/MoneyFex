using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class BalanceonMFTCCardZeroEmailController : Controller
    {
        // GET: EmailTemplate/BalanceonMFTCCardZeroEmail
        public ActionResult Index(string FaxerName, string MFTCCardNumber, string CardUserName, string SenderCountryCode, string SenderCurrency
            , string CardUserCountryCode, string CardUserCurrency, string TopUpMoneyfaxCard, string SetAutoTopUp , string ExchangeRate)
        {

            ViewBag.FaxerName = FaxerName;
            ViewBag.MFTCCardNumber = MFTCCardNumber;
            ViewBag.CardUserName = CardUserName;
            ViewBag.SenderCountryCode = SenderCountryCode;
            ViewBag.SenderCurrency = SenderCurrency;
            ViewBag.CardUserCountryCode = CardUserCountryCode;
            ViewBag.CardUserCurrency = CardUserCurrency;
            ViewBag.TopUpMoneyfaxCard = TopUpMoneyfaxCard;
            ViewBag.SetAutoTopUp = SetAutoTopUp;
            ViewBag.ExchangeRate = ExchangeRate;

            return View();
        }
    }
}