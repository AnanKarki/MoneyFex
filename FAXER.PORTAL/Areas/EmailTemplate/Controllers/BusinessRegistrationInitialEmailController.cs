using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class BusinessRegistrationInitialEmailController : Controller
    {
        // GET: EmailTemplate/BusinessRegistrationInitialEmail
        public ActionResult Index(string ContactPerson , string RegistrationCode)
        {
            ViewBag.ContactPerson = ContactPerson;
            ViewBag.RegistrationCode = RegistrationCode;

            return View();
        }
    }
}