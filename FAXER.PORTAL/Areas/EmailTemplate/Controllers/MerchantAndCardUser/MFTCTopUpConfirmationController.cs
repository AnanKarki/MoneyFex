using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.MerchantAndCardUser
{
    public class MFTCTopUpConfirmationController : Controller
    {
        // GET: EmailTemplate/MFTCTopUpConfirmation
        public ActionResult Index(string CardUserName, string ReceiverName, string ReceiverCountry ,  string AccountNo , string FaxingCurrency , string Amount ,string Fee  ,bool IsLocalPayment = false)
        {
            ViewBag.CardUserName = CardUserName;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.IsLocalPayment = IsLocalPayment == true ? 1 : 0;
            ViewBag.AccountNo = AccountNo;
            ViewBag.FaxingCurrency = FaxingCurrency;
            ViewBag.Amount = Amount;
            ViewBag.Fee = Fee;
             return View();
        }
    }
}