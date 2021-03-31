using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class AdminMFBCCardRefundRecieptController : Controller
    {
        // GET: EmailTemplate/AdminMFBCCardRefundReciept
        public ActionResult Index(string ReceiptNumber, string MerchantAccNo, string Date, string Time, 
            string MerchantName, string MFBCardNumber, string MerchantCountry, string MerchantCity, string MFBCCardUserName,
            string RefundingStaffName, string RefundingStaffCode, string RefundedAmount , string RefundingType)
        {
            ViewBag.ReceiptNumber = ReceiptNumber;
            ViewBag.MerchantAccNo = MerchantAccNo;
            ViewBag.Date = Date;
            ViewBag.Time = Time;
            ViewBag.MerchantName = MerchantName;
            ViewBag.MFBCardNumber = MFBCardNumber;
            ViewBag.MerchantCountry = MerchantCountry;
            ViewBag.MerchantCity = MerchantCity;
            ViewBag.MFBCCardUserName = MFBCCardUserName;
            ViewBag.RefundingStaffName = RefundingStaffName;
            ViewBag.RefundingStaffCode = RefundingStaffCode;
            ViewBag.RefundedAmount = RefundedAmount;

            string val = "";
            if (!string.IsNullOrEmpty(RefundingType)) {

                val = RefundingType.Substring(RefundingType.Length - 3, 3); 
            }
            ViewBag.RefundingType = "Admin-" + val;
            return View();
        }
    }
}