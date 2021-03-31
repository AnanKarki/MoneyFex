using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class NonCardMoneyTransferConfirmationToReceiverController : Controller
    {
        // GET: EmailTemplate/NonCardMoneyTransferConfirmationToReceiver
        public ActionResult Index(string ReceiverName, string ReceiverCountry, string SenderCountry)
        {

            ViewBag.ReceiverName = ReceiverName;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.SenderCountry = SenderCountry;
            return View();
        }
    }
}