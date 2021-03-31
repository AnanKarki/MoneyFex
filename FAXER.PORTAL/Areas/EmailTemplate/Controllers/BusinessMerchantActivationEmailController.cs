using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class BusinessMerchantActivationEmailController : Controller
    {
        // GET: EmailTemplate/BusinessMerchantActivationEmail
        public ActionResult Index(string NameOfContactPerson, string EmailAddress, string MerchantLoginCode, string Link)
        {
            ViewBag.NameOfContactPerson = NameOfContactPerson;
            ViewBag.EmailAddress = EmailAddress;
            ViewBag.MerchantLoginCode = MerchantLoginCode;
            ViewBag.Link = Link;
            return View();
        }
    }
}