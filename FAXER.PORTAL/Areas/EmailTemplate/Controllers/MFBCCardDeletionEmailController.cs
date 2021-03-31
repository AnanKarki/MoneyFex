using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class MFBCCardDeletionEmailController : Controller
    {
        // GET: EmailTemplate/MFBCCardDeletionEmail
        public ActionResult Index(string NameOfBusinessContactPerson , string MFBCCardNumber , string CardUserName , string CardUserDOB , string BusinessPhoneNumber, string CardUserEmail , string CardUserCountry , string CardUserCity ,string RegisterBusinessCard)
        {
            ViewBag.NameOfBusinessContactPerson = NameOfBusinessContactPerson;
            ViewBag.MFBCCardNumber = MFBCCardNumber;
            ViewBag.CardUserName =CardUserName;
            ViewBag.CardUserDOB = CardUserDOB;
            ViewBag.BusinessPhoneNumber = BusinessPhoneNumber;
            ViewBag.CardUserEmail = CardUserEmail;  
            ViewBag.CardUserCountry = CardUserCountry;
            ViewBag.CardUserCity = CardUserCity;
            ViewBag.RegisterBusinessCard = RegisterBusinessCard;
            return View();
        }
    }
}