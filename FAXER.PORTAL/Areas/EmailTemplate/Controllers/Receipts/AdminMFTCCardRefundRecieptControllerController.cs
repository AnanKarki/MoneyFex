using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class AdminMFTCCardRefundRecieptControllerController : Controller
    {
        // GET: EmailTemplate/AdminMFTCCardRefundRecieptController
        public ActionResult Index(string ReceiptNumber, string CardLastFourDigits, string Date, string Time,
            string MFTCardRegistrar, string MFTCardNumber, string MFTCardUserName, string MFTCardUserCountry, string MFTCardUserCity,
            string RefundingStaffName, string RefundingStaffCode, string RefundedAmount , 
            string RefundingType , string BalanceOnCard)
        {
            ViewBag.ReceiptNumber = ReceiptNumber;
            ViewBag.CardLastFourDigits = CardLastFourDigits;
            ViewBag.Date = Date;
            ViewBag.Time = Time;
            ViewBag.MFTCardRegistrar = MFTCardRegistrar;
            ViewBag.MFTCardNumber = MFTCardNumber;
            ViewBag.MFTCardUserName = MFTCardUserName;
            ViewBag.MFTCardUserCountry = MFTCardUserCountry;
            ViewBag.MFTCardUserCity = MFTCardUserCity;
            ViewBag.RefundingStaffName = RefundingStaffName;
            ViewBag.RefundingStaffCode = RefundingStaffCode;
            ViewBag.RefundedAmount = RefundedAmount;

            string val = "";
            if (!string.IsNullOrEmpty(RefundingType)) {

                val = RefundingType.Substring(RefundingType.Length - 3, 3);
            }
            ViewBag.RefundingType = "Admin-" + val;

            ViewBag.BalanceOnCard = BalanceOnCard;
            return View();
        }
    }
}