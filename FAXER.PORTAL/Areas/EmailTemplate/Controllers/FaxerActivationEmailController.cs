using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class FaxerActivationEmailController : Controller
    {
        // GET: EmailTemplate/FaxerActivationEmail
        public ActionResult Index(string FaxerName,string guId)
        {
            ViewBag.FaxerName = FaxerName;
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            ViewBag.link = string.Format("{0}/FaxerAccount/Activate/{1}", baseUrl, guId);
            return View();
        }
    }
}