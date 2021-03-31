using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class StaffActivationEmailController : Controller
    {
        // GET: EmailTemplate/StaffActivationEmail
        public ActionResult Index(string NameOfContactPerson, string EmailAddress, string LoginCode , string Link)
        {
            ViewBag.NameOfContactPerson = NameOfContactPerson;
            ViewBag.EmailAddress = EmailAddress;
            ViewBag.LoginCode = LoginCode;
            ViewBag.Link = Link;
            return View();
        }
    }
}