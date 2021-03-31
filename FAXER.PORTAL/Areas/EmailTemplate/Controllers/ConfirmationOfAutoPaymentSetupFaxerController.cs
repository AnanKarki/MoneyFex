using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ConfirmationOfAutoPaymentSetupFaxerController : Controller
    {
        // GET: EmailTemplate/ConfirmationOfAutoPaymentSetupFaxer
        public ActionResult Index(string FaxerName, string AutoPaymentAmount, string CountryCurrencySymbol, string BusinessMerchantName, string CreditORDebitCardlast4digits, string AutoPaymentFrequency , string SetAutoPayment , string PayforGoodsBusinessMerchant)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.AutoPaymentAmount = AutoPaymentAmount;
            ViewBag.CountryCurrencySymbol = CountryCurrencySymbol;
            ViewBag.BusinessMerchantName = BusinessMerchantName;
            ViewBag.CreditORDebitCardlast4digits = CreditORDebitCardlast4digits;
            ViewBag.AutoPaymentFrequency = AutoPaymentFrequency;
            ViewBag.SetAutoPayment = SetAutoPayment;
            ViewBag.PayforGoodsBusinessMerchant = PayforGoodsBusinessMerchant;
            return View();
        }
    }
}