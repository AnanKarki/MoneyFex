using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class MFTCDeletedCardBalanceRefundEmailController : Controller
    {
        // GET: EmailTemplate/MFTCDeletedCardBalanceRefundEmail
        public ActionResult Index(string FaxerName, string RefundAmount, string FourDigitOfCard, string Deleter, string MFTCNo, string CardUserName, string CardUserDOB, string CardUserCountry, string CardUserCity, string TopUpMethod, string OriginalTopUpAmountSendCurrency)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.RefundAmount = RefundAmount;
            ViewBag.FourDigitOfCard = FourDigitOfCard;
            ViewBag.Deleter = Deleter;
            ViewBag.MFTCNo = MFTCNo;
            ViewBag.CardUserName = CardUserName;
            ViewBag.CardUserDOB = CardUserDOB;
            ViewBag.CardUserCountry = CardUserCountry;
            ViewBag.CardUserCity = CardUserCity;
            ViewBag.TopUpMethod = TopUpMethod;
            ViewBag.OriginalTopUpAmountSendCurrency = OriginalTopUpAmountSendCurrency;
            return View();
        }
    }
}