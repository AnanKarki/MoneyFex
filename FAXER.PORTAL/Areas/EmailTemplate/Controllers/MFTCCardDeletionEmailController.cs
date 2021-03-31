using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class MFTCCardDeletionEmailController : Controller
    {
        // GET: EmailTemplate/MFTCCardDeletionEmail
        public ActionResult Index(string FaxerName, string MFTCCardNumber, string CardUserName, string CardUserDOB, string CardUserPhoneNumber, string CardUserEmailAddress, string CardUserCountry, string CardUserCity ,string RegisterMFTC)
        {
            ViewBag.Faxername = FaxerName;
            ViewBag.MFTCCardNumber = MFTCCardNumber;
            ViewBag.CardUserName = CardUserName;
            ViewBag.CardUserDOB = CardUserDOB;
            ViewBag.CardUserPhoneNumber = CardUserPhoneNumber;
            ViewBag.CardUserEmailAddress = CardUserEmailAddress;
            ViewBag.CardUserCountry = CardUserCountry;
            ViewBag.CardUserCity = CardUserCity;
            ViewBag.RegisterMFTC = RegisterMFTC;
            return View();
        }
    }
}