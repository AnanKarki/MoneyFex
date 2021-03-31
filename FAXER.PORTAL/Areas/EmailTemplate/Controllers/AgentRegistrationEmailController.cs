using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class AgentRegistrationEmailController : Controller
    {
        // GET: EmailTemplate/AgentRegistrationEmail
        public ActionResult Index(string AgentName , string Link)
        {
            ViewBag.AgentName = AgentName;
            ViewBag.Link = Link;
             return View();
        }
    }
}