using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.MerchantAndCardUser
{
    public class ConfirmationOfMoneyTransferController : Controller
    {
        // GET: EmailTemplate/ConfirmationOfMoneyTransfer
        public ActionResult Index(string ReceiverName, string ReceiverCountry, string SenderCountry)
        {
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.SenderCountry = SenderCountry;
            return View();
        }
    }
}