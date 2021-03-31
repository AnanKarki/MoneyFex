using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class EmailToHRAdminAboutJobApplicantController : Controller
    {
        // GET: EmailTemplate/EmailToHRAdminAboutJobApplicant
        public ActionResult Index(string CandidateName , string Telephone , string Email , string country , string City , string JobTitle)
        {

            ViewBag.CandidateName = CandidateName;
            ViewBag.Telephone = Telephone;
            ViewBag.Email = Email;
            ViewBag.Country = country;
            ViewBag.City = City;
            ViewBag.JobTitle = JobTitle;
            return View();
        }
    }
}