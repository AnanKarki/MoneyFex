using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ConfirmationOfAutoPaymentDeletionController : Controller
    {
        // GET: EmailTemplate/ConfirmationOfAutoPaymentDeletion
        public ActionResult Index(string FaxerName, string AutoTopUpAmount, string CountryCurrencySymbol, string BusinessMerchantName , string SetAutoPayment , string PayforGoodsBusinessMerchant)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.AutoTopUpAmount = AutoTopUpAmount;
            ViewBag.CountryCurrencySymbol = CountryCurrencySymbol;
            ViewBag.BusinessMerchantName = BusinessMerchantName;
            ViewBag.SetAutoPayment = SetAutoPayment;
            ViewBag.PayforGoodsBusinessMerchant = PayforGoodsBusinessMerchant;
            return View();
        }
    }
}