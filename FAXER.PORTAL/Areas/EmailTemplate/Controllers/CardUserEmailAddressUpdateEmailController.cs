using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class CardUserEmailAddressUpdateEmailController : Controller
    {
        // GET: EmailTemplate/CardUserEmailAddressUpdateEmail
        public ActionResult Index(string SenderName, string CardUserName, string UpdatedProperty, string CardNumber)
        {
            ViewBag.SenderName = SenderName;
            ViewBag.CardUserName = CardUserName;
            ViewBag.UpdatedProperty = UpdatedProperty;
            ViewBag.CardNumber = CardNumber;
            return View();
        }
    }
}