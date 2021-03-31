using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ConfirmationOfAutoPaymentMFTCTopUpDeletionController : Controller
    {
        // GET: EmailTemplate/ConfirmationOfAutoPaymentMFTCTopUpDeletion
        public ActionResult Index(string FaxerName, string AutoTopUpAmount, string CountryCurrrencySymbol, string MFTCCardNumber, string CarduserName, string TopUpMoneyfaxCard , string SetAutoTopUp , string PayForGoodsAbroad)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.AutoTopUpAmount = AutoTopUpAmount;
            ViewBag.CountryCurrencySymbol = CountryCurrrencySymbol;
            ViewBag.MFTCCardNumber = MFTCCardNumber;
            ViewBag.CardUserName = CarduserName;
            ViewBag.TopUpMoneyfaxCard = TopUpMoneyfaxCard;
            ViewBag.SetAutoTopUp = SetAutoTopUp;
            ViewBag.PayForGoodsAbroad = PayForGoodsAbroad;
            return View();
        }   
    }
}