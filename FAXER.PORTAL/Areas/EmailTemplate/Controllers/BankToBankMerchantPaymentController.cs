using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class BankToBankMerchantPaymentController : Controller
    {
        // GET: EmailTemplate/BankToBankMerchantPayment
        public ActionResult Index(string PayerName , string PayerAccountNo,string MerchantName , string MerchantAccountNo , string Amount , string PaymentReference , string BankReference)
        {
            ViewBag.PayerName = PayerName;
            ViewBag.PayerAccountNo = PayerAccountNo;
            ViewBag.MerchantName = MerchantName;
            ViewBag.MerchantAccountNo = MerchantAccountNo;
            ViewBag.Amount = Amount;
            ViewBag.PaymentReference = PaymentReference;
            ViewBag.BankReference = BankReference;

            return View();
        }
    }
}