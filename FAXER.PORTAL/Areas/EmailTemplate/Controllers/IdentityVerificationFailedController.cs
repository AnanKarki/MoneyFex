using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class IdentityVerificationFailedController : Controller
    {
        // GET: EmailTemplate/IdentityVerificationFailed
        public ActionResult Index(string senderFirstname = "", string Reason = "")
        {
            ViewBag.SenderName = senderFirstname;
            ViewBag.Reason = Reason;
            return View();
        }
    }
}