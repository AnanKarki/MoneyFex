using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class AdminPayGoodsAndServicesReceiptController : Controller
    {
        // GET: EmailTemplate/AdminPayGoodsAndServicesReceipt
        public ActionResult Index(string ReceiptNumber, string Date, string Time, string FaxerFullName
         , string BusinessMerchantName, string BusinessMFCode, string AmountPaid, string ExchangeRate, string AmountInLocalCurrency
         , string Fee , string StaffName , string StaffCode, string SendingCurrency, string ReceivingCurrency ,
            string Faxercountry , string BusinessCountry , string BusinessCity , string DepositType)
        {
            ViewBag.ReceiptNumber = ReceiptNumber;
            ViewBag.Date = Date;
            ViewBag.Time = Time;
            ViewBag.FaxerFullName = FaxerFullName;
            ViewBag.BusinessMerchantName = BusinessMerchantName;
            ViewBag.BusinessMFCode = BusinessMFCode;
            ViewBag.StaffName = StaffName;
            ViewBag.StaffCode = StaffCode;
            ViewBag.AmountPaid =  (decimal.Parse(AmountPaid) - decimal.Parse(Fee)) +  " " +  SendingCurrency;

            ViewBag.TotalAmount = AmountPaid + " " + SendingCurrency;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.AmountInLocalCurrency =  AmountInLocalCurrency + " " +  ReceivingCurrency;
            ViewBag.Fee = Fee + " " +  SendingCurrency;
            //ViewBag.SendingCurrency = SendingCurrency;
            //ViewBag.ReceivingCurrency = ReceivingCurrency;

            ViewBag.FaxerCountry = Faxercountry;
            ViewBag.BusinessCountry = BusinessCountry;
            ViewBag.BusinessCity = BusinessCity;
            string val = "";
            if (!string.IsNullOrEmpty(DepositType)) {

                val = DepositType.Substring(DepositType.Length - 3, 3);
            }
            ViewBag.DepositType = val;

            return View();
        }
    }
}