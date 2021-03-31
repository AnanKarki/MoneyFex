using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.MerchantAndCardUser
{
    public class ConfirmationOfMoneyTransferEmailToSenderController : Controller
    {
        // GET: EmailTemplate/ConfirmationOfMoneyTransferEmailToSender
        public ActionResult Index(string CardUserName, string ReceiverName, string ReceiverCountry, string ReceiverCity, string MFCN, string TransferredAmount , string FaxingCurrency , bool IsLocalPayment =false)
        {
            ViewBag.CardUserName = CardUserName;
            ViewBag.ReceiverFullName = ReceiverName;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.ReceiverCity = ReceiverCity;
            ViewBag.MFCN = MFCN;
            ViewBag.TransferredAmount = TransferredAmount;
            ViewBag.FaxingCurrency = FaxingCurrency;

            ViewBag.IsLocalPayment = IsLocalPayment == true ? 0 : 1;
            return View();
        }
    }
}