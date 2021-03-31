using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Announcements
{
    public class AppAvailableEmailController : Controller
    {
        // GET: EmailTemplate/AppAvailableEmail
        public ActionResult Index(string senderFirstName = "")
        {
            ViewBag.SenderFirstName = senderFirstName;
            return View();
        }
    }
}