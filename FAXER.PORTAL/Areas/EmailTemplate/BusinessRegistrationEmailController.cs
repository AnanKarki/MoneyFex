using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate
{
    public class BusinessRegistrationEmailController : Controller
    {
        // GET: EmailTemplate/BusinessRegistrationEmail
        public ActionResult Index(string BusinessName , string Link)
        {
            ViewBag.BusinessName = BusinessName;
            ViewBag.Link = Link;
            return View();
        }
    }
}