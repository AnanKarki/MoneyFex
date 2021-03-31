using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class BankToBankMFTCCardTopUpController : Controller
    {
        // GET: EmailTemplate/BankToBankMFTCCardTopUp
        public ActionResult Index(string FaxerName , string CardUserName , string CardNumber , string Amount 
            , string PaymentReference, string BankReference , string FaxerAccountNo, string CardUserCountry , string CardUserCity , string ReceivingAmount )
        {
 
            ViewBag.FaxerName = FaxerName;
            ViewBag.CardUserName = CardUserName;
            ViewBag.CardNumber = CardNumber;
            ViewBag.CardUserCountry = CardUserCountry;
            ViewBag.CardUserCity = CardUserCity;
            ViewBag.Amount = Amount;
            ViewBag.PaymentReference = PaymentReference;
            ViewBag.BankReference = BankReference;
            ViewBag.FaxerAccountNo = FaxerAccountNo;
            ViewBag.ReceivingAmount = ReceivingAmount;

            return View();
        }
    }
}