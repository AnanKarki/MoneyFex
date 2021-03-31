using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Announcements
{
    public class CardDeletionEmailController : Controller
    {
        // GET: EmailTemplate/CardDeletionEmail
        public ActionResult Index(string cardOwnerFirstName="" , string formattedCardNumber ="")
        {
            ViewBag.CardOwnerFirstName = cardOwnerFirstName;
            ViewBag.FormattedCardNumber = formattedCardNumber;
            return View();
        }
    }
}