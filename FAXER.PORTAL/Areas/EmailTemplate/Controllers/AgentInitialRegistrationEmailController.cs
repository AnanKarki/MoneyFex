using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class AgentInitialRegistrationEmailController : Controller
    {
        // GET: EmailTemplate/AgentInitialRegistrationEmail
        public ActionResult Index(string ContactPerson, string RegistrationCode)
        {
            ViewBag.ContactPerson = ContactPerson;
            ViewBag.RegistrationCode = RegistrationCode;
            return View();
        }
    }
}