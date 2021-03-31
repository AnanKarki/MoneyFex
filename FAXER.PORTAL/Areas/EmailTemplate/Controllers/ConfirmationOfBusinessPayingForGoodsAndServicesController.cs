using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ConfirmationOfBusinessPayingForGoodsAndServicesController : Controller
    {
        // GET: EmailTemplate/ConfirmationOfBusinessPayingForGoodsAndServices
        public ActionResult Index(string BusinessUserName , string PayedToBusinessMerchantName)
        {
            ViewBag.BusinessUserName = BusinessUserName;
            ViewBag.PayedToBusinessMerchantName = PayedToBusinessMerchantName;
            return View();
        }
    }
}