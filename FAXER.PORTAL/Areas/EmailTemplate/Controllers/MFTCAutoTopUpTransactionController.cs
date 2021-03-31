using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class MFTCAutoTopUpTransactionController : Controller
    {
        // GET: EmailTemplate/MFTCAutoTopUpTransaction
        public ActionResult Index(string FaxerName, string CardUserCountry, string CardUserName , string CardNumber , string CardUserCity, string TopUpAmount)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.CardUserCountry = CardUserCountry;
            ViewBag.CardUserName = CardUserName;
            ViewBag.CardNumber = CardNumber;
            ViewBag.CardUserCity = CardUserCity;
            ViewBag.TopUpAmount = TopUpAmount;
            return View();
        }
    }
}