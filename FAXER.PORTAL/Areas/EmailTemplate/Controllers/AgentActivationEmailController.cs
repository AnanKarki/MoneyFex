using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class AgentActivationEmailController : Controller
    {
        // GET: EmailTemplate/AgentActivationEmail
        public ActionResult Index(string NameOfContactPerson , string EmailAddress ,  string AgencyLoginCode , string StaffLoginCode , string NameOfBusiness, string Password , bool IsRegisteredByAdmin = false)
        {
            ViewBag.NameOfContactPerson = NameOfContactPerson;
            ViewBag.EmailAddress = EmailAddress;
            ViewBag.AgencyLoginCode = AgencyLoginCode;
            ViewBag.NameOfBusiness = NameOfBusiness;
            ViewBag.IsRegisteredByAdmin = IsRegisteredByAdmin;
            ViewBag.StaffLoginCode = StaffLoginCode;
            ViewBag.Password= Password;
            return View();
        }
    }
}