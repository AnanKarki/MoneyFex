using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class MFTCCardDeletionController : Controller
    {
        // GET: EmailTemplate/MFTCCardDeletion
        public ActionResult Index(string NameOfDeleter, string MFTCNo, string NameOnMFTCard, string DOBOfCardUser, string CardUserHistory, string CardUserCity, string AmountOnCard)
        {
            ViewBag.NameOfDeleter = NameOfDeleter;
            ViewBag.MFTCNo = MFTCNo;
            ViewBag.NameOnMFTCard = NameOnMFTCard;
            ViewBag.DOBOfCardUser = DOBOfCardUser;
            ViewBag.CardUserHistory = CardUserHistory;
            ViewBag.CardUserCity = CardUserCity;
            ViewBag.AmountOnCard = AmountOnCard;

            return View();
        }
    }
}