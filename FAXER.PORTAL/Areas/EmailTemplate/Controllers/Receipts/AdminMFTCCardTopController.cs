using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class AdminMFTCCardTopController : Controller
    {
        // GET: EmailTemplate/AdminMFTCCardTop
        public ActionResult Index(string ReceiptNumber, string Date, string Time, string FaxerFullName, string MFTCCardNumber
            , string CardUserFullName, string AmountTopUp, string ExchangeRate, string AmountInLocalCurrency
            , string Fee, string BalanceOnCard, string StaffName, string StaffCode, string SendingCurrency, string ReceivingCurrency
            , string DepositType , string FaxerCountry , string CardUserCountry , string CardUserCity)
        {
            ViewBag.ReceiptNumber = ReceiptNumber;
            ViewBag.Date = Date;
            ViewBag.Time = Time;
            ViewBag.FaxerFullName = FaxerFullName;
            ViewBag.MFTCCardNumber = MFTCCardNumber;
            ViewBag.CardUserFullName = CardUserFullName;
            ViewBag.StaffName = StaffName;
            ViewBag.StaffCode = StaffCode;
            ViewBag.AmountTopUp = AmountTopUp;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.AmountInLocalCurrency = AmountInLocalCurrency;
            ViewBag.Fee = Fee;
            ViewBag.BalanceOnCard = BalanceOnCard;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;

            ViewBag.TotalAmount = decimal.Parse(AmountTopUp) + decimal.Parse(Fee);
            string val = "";
            if (!string.IsNullOrEmpty(DepositType))
            {

                val = DepositType.Substring(DepositType.Length - 3, 3);
            }
            ViewBag.DepositType = "Admin-" + val;
            ViewBag.FaxerCountry = FaxerCountry;
            ViewBag.CardUserCountry = CardUserCountry;
            ViewBag.CardUserCity = CardUserCity;
            return View();
        }
    }
}