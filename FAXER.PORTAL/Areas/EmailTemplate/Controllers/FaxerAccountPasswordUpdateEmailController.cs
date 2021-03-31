using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class FaxerAccountPasswordUpdateEmailController : Controller
    {
        // GET: EmailTemplate/FaxerAccountPasswordUpdateEmail
        public ActionResult Index(string SenderName)
        {
            ViewBag.SenderName = SenderName;

            return View();
        }
    }
}