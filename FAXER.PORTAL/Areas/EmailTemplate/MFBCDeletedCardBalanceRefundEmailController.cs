using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate
{
    public class MFBCDeletedCardBalanceRefundEmailController : Controller
    {
        // GET: EmailTemplate/MFBCDeletedCardBalanceRefundEmail
        public ActionResult Index(string MerchantName, string RefundAmount, string MerchantAccountNo, string Deleter, string MFBCNo, string CardUserName, string CardUserDOB, string CardUserCountry, string CardUserCity)
        {
            ViewBag.MerchantName = MerchantName;
            ViewBag.RefundAmount = RefundAmount;
            ViewBag.MerchantAccountNo = MerchantAccountNo;
            ViewBag.Deleter = Deleter;
            ViewBag.MFBCNo = MFBCNo;
            ViewBag.CardUserName = CardUserName;
            ViewBag.CardUserDOB = CardUserDOB;
            ViewBag.CardUserCountry = CardUserCountry;
            ViewBag.CardUserCity = CardUserCity;
            
            return View();
        }
    }
}