using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ConfirmationOfCardUserPayingForGoodsAndServicesController : Controller
    {
        // GET: EmailTemplate/ConfirmationOfCardUserPayingForGoodsAndServices
        public ActionResult Index(string CardUserName , string BusinessMerchantName)
        {
            @ViewBag.CardUserName = CardUserName;
            @ViewBag.BusinessMerchantName = BusinessMerchantName;
            return View();
        }
    }
}