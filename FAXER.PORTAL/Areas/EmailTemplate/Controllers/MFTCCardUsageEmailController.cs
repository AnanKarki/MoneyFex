using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class MFTCCardUsageEmailController : Controller
    {
        // GET: EmailTemplate/MFTCCardUsageEmail
        public ActionResult Index(string FaxerName, string MFTCCardNumber, string CardUserName, string CardUserCountry, string CardUserCity, string CityOfPayingAgentOrRegisteredMerchant, string BalanceOnCard , string TopUpMoneyfaxCard , string StopAlert)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.MFTCCardNumber = Common.Common.GetVirtualAccountNo(MFTCCardNumber);
            ViewBag.CardUserName = CardUserName;
            ViewBag.CardUserCountry = CardUserCountry;
            ViewBag.CardUserCity = CardUserCity;
            ViewBag.CityOfPayingAgentOrRegisteredMerchant = CityOfPayingAgentOrRegisteredMerchant;
            ViewBag.BalanceOnCard = BalanceOnCard + Common.Common.GetCountryCodeByName(CardUserName);
            ViewBag.TopUpMoneyfaxCard = TopUpMoneyfaxCard;
            ViewBag.StopAlert = StopAlert;
            return View();
        }
    }
}