using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class PayGoodsAndServicesFaxerReceiptController : Controller
    {
        // GET: EmailTemplate/PayGoodsAndServicesFaxerReceipt
        public ActionResult Index(string ReceiptNumber, string Date, string Time, string FaxerFullName , string FaxerCountry 
           , string BusinessMerchantName,string BusinessMFCode, string BusinessCountry , string BusinessCity , string AmountPaid, string ExchangeRate, string AmountInLocalCurrency
           , string Fee , string FaxerCurrency , string ReceiverCurrreny)
        {
            ViewBag.ReceiptNumber = ReceiptNumber;
            ViewBag.Date = Date;
            ViewBag.Time = Time;
            ViewBag.FaxerFullName = FaxerFullName;
            ViewBag.FaxerCountry = FaxerCountry;
            ViewBag.BusinessMerchantName = BusinessMerchantName;
            ViewBag.BusinessMFCode = BusinessMFCode;
            ViewBag.BusinessCountry = BusinessCountry;
            ViewBag.BusinessCity = BusinessCity;
            ViewBag.AmountPaid = (int.Parse(AmountPaid) + int.Parse(Fee) ) + FaxerCurrency;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.AmountInLocalCurrency = AmountInLocalCurrency + ReceiverCurrreny;
            ViewBag.Fee = Fee + " " + FaxerCurrency;
            ViewBag.TotalSentAmount = AmountPaid + " " + FaxerCurrency;
            return View();
        }
    }
}