using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ConfirmationOfMerchantPaymentWithReceiptFaxerController : Controller
    {
        // GET: EmailTemplate/ConfirmationOfMerchantPaymentWithReceiptFaxer
        public ActionResult Index(string FaxerName,string MerchantBusinessName, string PayForGoodsAbroad)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.MerchantBusinessName = MerchantBusinessName;
            ViewBag.PayForGoodsAbroad = PayForGoodsAbroad;
            return View();
        }
    }
}