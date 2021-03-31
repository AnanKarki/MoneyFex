using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class MFTCCardRegistrationEmailToCardUserController : Controller
    {
        // GET: EmailTemplate/MFTCCardRegistrationEmailToCardUser
        public ActionResult Index(string CardUserName , string MFTCCardNumber , string SenderName , string SenderCountry , string CardUserCountry)
        {

            ViewBag.CardUserName = CardUserName;
            ViewBag.MFTCCardNumber = MFTCCardNumber;
            ViewBag.SenderName = SenderName;
            ViewBag.SenderCountry = SenderCountry;
            ViewBag.CardUserCountry = CardUserCountry;
            return View();
        }
    }
}