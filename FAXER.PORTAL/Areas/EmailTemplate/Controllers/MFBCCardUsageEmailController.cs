using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class MFBCCardUsageEmailController : Controller
    {
        // GET: EmailTemplate/MFBCCardUsageEmail
        public ActionResult Index(string NameOfBusinessUser, string MFBCCardNumber, string CardUserName, string CardUserCountry, string CardUserCity, string CityofPayingAgentOrRegisteredMerchant, string BalanceOnCard)
        {
            ViewBag.NameOfBusinessUser = NameOfBusinessUser;
            ViewBag.MFBCCardNumber = MFBCCardNumber;
            ViewBag.CarduserName = CardUserName;
            ViewBag.CardUserCountry = CardUserCountry;
            ViewBag.CardUserCity = CardUserCity;
            ViewBag.CityofPayingAgentOrRegisteredMerchant = CityofPayingAgentOrRegisteredMerchant;
            ViewBag.BalanceOnCard = BalanceOnCard;
            return View();
        }
    }
}