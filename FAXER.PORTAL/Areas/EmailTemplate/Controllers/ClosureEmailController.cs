using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ClosureEmailController : Controller
    {
        // GET: EmailTemplate/ClosureEmail
        public ActionResult Index(string senderFirstName)
        {
            ViewBag.SenderFirstName = senderFirstName;
            return View();
        }
    }
}