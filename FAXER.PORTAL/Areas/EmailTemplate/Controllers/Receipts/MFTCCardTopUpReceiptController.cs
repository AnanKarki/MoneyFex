using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class MFTCCardTopUpReceiptController : Controller
    {
        // GET: EmailTemplate/MFTCCardTopUpReceipt
        public ActionResult Index(string ReceiptNumber , string Date , string Time , string FaxerFullName , string MFTCCardNumber
            , string CardUserFullName , string AmountTopUp , string ExchangeRate , string AmountInLocalCurrency 
            , string Fee , string BalanceOnCard , string TopupReference = "")
        {
            ViewBag.ReceiptNumber = ReceiptNumber;
            ViewBag.Date = Date;
            ViewBag.Time = Time;
            ViewBag.FaxerFullName  = FaxerFullName;
            ViewBag.MFTCCardNumber = MFTCCardNumber;
            ViewBag.CardUserFullName = CardUserFullName;
            ViewBag.AmountTopUp = AmountTopUp;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.AmountInLocalCurrency = AmountInLocalCurrency;
            ViewBag.Fee = Fee;
            ViewBag.BalanceOnCard = BalanceOnCard;
            ViewBag.TopUpReference = TopupReference;
            return View();
        }
    }
}