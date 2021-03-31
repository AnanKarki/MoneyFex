using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate
{
    public class BusinessMerchantActivationEmailToAdminController : Controller
    {
        // GET: EmailTemplate/BusinessMerchantActivationEmailToAdmin
        public ActionResult Index(string MerchantName, string Telephone, string Email, string RegistrationCode)
        {

            ViewBag.MerchantName = MerchantName;
            ViewBag.Telephone = Telephone;
            ViewBag.Email = Email;
            ViewBag.RegistrationCode = RegistrationCode;

            return View();
        }
    }
}