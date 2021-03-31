using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class FaxerAccountReactivationRequestController : Controller
    {
        // GET: EmailTemplate/FaxerAccountReactivationRequest
        public ActionResult Index(string FullName, string Country , string City , string Email , string MFNumber)
        {
            ViewBag.FullName = FullName;
            ViewBag.Country = Country;
            ViewBag.City = City;
            ViewBag.Email = Email;
            ViewBag.MFNumber = MFNumber;
            return View();
        }
    }
}