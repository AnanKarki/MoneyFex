using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class EmailToJobApplicantController : Controller
    {
        // GET: EmailTemplate/EmailToJobApplicant
        public ActionResult Index(string JobApplicantFirstName , string JobTitle)
        {
            ViewBag.FirstName = JobApplicantFirstName;
            ViewBag.JobTitle = JobTitle;
            return View();
        }
    }
}