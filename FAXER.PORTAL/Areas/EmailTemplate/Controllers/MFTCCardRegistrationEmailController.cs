using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class MFTCCardRegistrationEmailController : Controller
    {
        // GET: EmailTemplate/MFTCCardRegistrationEmail
        public ActionResult Index(string FaxerName , string MFTCCardNumber , string CardUserName , string CardUserCountry , string TopUpMoneyfaxCard)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.MFTCCardNumber = MFTCCardNumber;
            ViewBag.CardUserName = CardUserName;
            ViewBag.CardUserCountry = CardUserCountry;
            ViewBag.TopUpMoneyfaxCard = TopUpMoneyfaxCard;
            return View();
        }
    }
}