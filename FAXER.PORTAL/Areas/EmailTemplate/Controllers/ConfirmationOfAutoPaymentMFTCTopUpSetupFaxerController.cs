using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ConfirmationOfAutoPaymentMFTCTopUpSetupFaxerController : Controller
    {
        // GET: EmailTemplate/ConfirmationOfAutoPaymentMFTCTopUpSetupFaxer
        public ActionResult Index(string FaxerName, string AutoTopUpAmount, string CountryCurrencySymbol, string MFTCCardNumber, string CardUserName, string TopUpMoneyfaxCard, string SetAutoTopUp, string PayForGoodsAbroad)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.AutoTopUpAmount = AutoTopUpAmount;
            ViewBag.CountryCurrencySymbol = CountryCurrencySymbol;
            ViewBag.MFTCCardNumber = MFTCCardNumber;
            ViewBag.CardUserName = CardUserName;
            ViewBag.TopUpMoneyfaxCard = TopUpMoneyfaxCard;
            ViewBag.SetAutoTopUp = SetAutoTopUp;
            ViewBag.PayForGoodsAbroad = PayForGoodsAbroad;
            return View();
        }
    }
}