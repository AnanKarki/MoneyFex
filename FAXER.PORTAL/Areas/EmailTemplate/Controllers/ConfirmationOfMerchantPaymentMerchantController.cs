using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ConfirmationOfMerchantPaymentMerchantController : Controller
    {
        // GET: EmailTemplate/ConfirmationOfMerchantPaymentMerchant
        public ActionResult Index(string BusinessMerchantName , string FaxerName)
        {

            ViewBag.BusinessMerchantName = BusinessMerchantName;
            ViewBag.FaxerName = FaxerName;
            return View();
        }
    }
}