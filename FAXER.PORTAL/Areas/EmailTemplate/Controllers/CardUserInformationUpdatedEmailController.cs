using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class CardUserInformationUpdatedEmailController : Controller
    {
        // GET: EmailTemplate/CardUserInformationUpdatedEmail
        public ActionResult Index(string FaxerName ,string CardUserName)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.CardUserName = CardUserName;
            return View();
        }
    }
}