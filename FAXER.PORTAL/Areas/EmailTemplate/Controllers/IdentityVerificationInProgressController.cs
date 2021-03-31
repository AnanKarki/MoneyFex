using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class IdentityVerificationInProgressController : Controller
    {
        // GET: EmailTemplate/IdentityVerificationInProgress
        public ActionResult Index(string senderFirstname = "")
        {
            ViewBag.SenderName = senderFirstname;
            return View();
        }
    }
}