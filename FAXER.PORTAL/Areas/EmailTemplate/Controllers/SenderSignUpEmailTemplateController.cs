using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class SenderSignUpEmailTemplateController : Controller
    {
        // GET: EmailTemplate/SenderSignUpEmailTemplate
        public ActionResult Index(string FirstName, string Email, string MobileNo, string CustomerNo , string VerificationCode)
        {
            ViewBag.FirstName = FirstName;
            ViewBag.Email = Email;
            ViewBag.MobileNo = MobileNo;
            ViewBag.CustomerNo = CustomerNo;
            ViewBag.VerificationCode = VerificationCode;
            return View();
        }
    }
}