using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class PasswordSecirityCodeEmailController : Controller
    {
        // GET: EmailTemplate/PasswordSecirityCodeEmail
        public ActionResult Index( string UserName , string SecurityCode)
        {
            ViewBag.UserName = UserName;
            ViewBag.SecurityCode = SecurityCode;
            return View();
        }
    }
}