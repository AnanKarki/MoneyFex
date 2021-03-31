using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class AgentRegistrationEmailToAdminController : Controller
    {
        // GET: EmailTemplate/AgentRegistrationEmailToAdmin
        public ActionResult Index(string AgentName , string AgentTelephone , string AgentEmail , string RegistrationCode)
        {

            ViewBag.AgentName = AgentName;
            ViewBag.AgentTelephone = AgentTelephone;
            ViewBag.AgentEmail = AgentEmail;
            ViewBag.RegistrationCode = RegistrationCode;

            return View();
        }
    }
}