using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class MFTCCardTopUpController : Controller
    {
        // GET: EmailTemplate/MFTCCardTopUp
        public ActionResult Index(string FaxerName, string CardUserCountry, string TopUpMoneyfaxCard, string SetAutoTopUp)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.CardUserCountry = Common.Common.GetCountryName(CardUserCountry);
            ViewBag.TopUpMoneyfaxCard = TopUpMoneyfaxCard;
            ViewBag.SetAutoTopUp = SetAutoTopUp;
            return View();
        }
    }
}