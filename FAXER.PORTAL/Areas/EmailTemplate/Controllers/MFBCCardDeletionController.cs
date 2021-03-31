using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class MFBCCardDeletionController : Controller
    {
        // GET: EmailTemplate/MFBCCardDeletion
        public ActionResult Index(string NameOfDeleter, string MFBCNo, string NameOnMFBCard, string MerchantName, string MerchantAccNo, string DOBOfCardUser, string CardUserHistory, string CardUserCity, string AmountOnCard )
        {
            ViewBag.NameOfDeleter = NameOfDeleter;
            ViewBag.MFBCNo = MFBCNo;
            ViewBag.NameOnMFBCard = NameOnMFBCard;
            ViewBag.MerchantName = MerchantName;
            ViewBag.MerchantAccNo = MerchantAccNo;
            ViewBag.DOBOfCardUser = DOBOfCardUser;
            ViewBag.CardUserHistory = CardUserHistory;
            ViewBag.CardUserCity = CardUserCity;
            ViewBag.AmountOnCard = AmountOnCard;

            return View();
        }
    }
}