using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.MerchantAndCardUser
{
    public class ConfirmationPaymentServiceProviderController : Controller
    {
        // GET: EmailTemplate/ConfirmationPaymentServiceProvider
        public ActionResult Index(string SenderName , string ReceivingAmount , string ReceiverBusinessName , string ReceiverCountry , string ReceivingCurrency)
        {

            ViewBag.SenderName = SenderName;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.ReceiverBusinessName = ReceiverBusinessName;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.ReceivingCurrency = ReceivingCurrency;

            return View();
        }
    }
}