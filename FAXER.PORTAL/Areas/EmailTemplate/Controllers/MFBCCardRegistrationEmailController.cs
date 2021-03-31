using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class MFBCCardRegistrationEmailController : Controller
    {
        // GET: EmailTemplate/MFBCCardRegistrationEmail
        public ActionResult Index(string NameofBusinessMerchant, string MFBCCardNumber, string CardUserName, string CardUserDOB, string CardUserEmailAddress, string CardUserCountry, string CardUserCity )
        {
            ViewBag.NameOfBusinessMerchant = NameofBusinessMerchant;
            ViewBag.MFBCCardNumber = MFBCCardNumber;
            ViewBag.CardUserName = CardUserName;
            ViewBag.CardUserDOB = CardUserDOB;
            ViewBag.CardUserEmailAddress = CardUserEmailAddress;
            ViewBag.CardUserCountry = CardUserCountry;
            ViewBag.CardUserCity = CardUserCity;
            return View();
        }
    }
}