using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts.MerchantAndCardUser
{
    public class MFTCCardTopUpconfirmationPaymentReceiptController : Controller
    {
        // GET: EmailTemplate/MFTCCardTopUpconfirmationPaymentReceipt
        public ActionResult Index(string ReceiptNumber , string Date , string Time , string SenderName , string MFTCCardNo , string MFTCCardName, string TopUpAmount
            , string Fees , string ExchangeRate , string TotalAmount , string AmountInLocalCurrency , string SendingCurrency , string ReceivingCurrency)
        {

            ViewBag.MFReceiptNumber = ReceiptNumber;
            ViewBag.TransactionDate = Date;
            ViewBag.TransactionTime = Time;
            ViewBag.SenderName = SenderName;
            ViewBag.MFTCCardNumber = Common.Common.GetVirtualAccountNo(MFTCCardNo);
            ViewBag.MFTCCardName = MFTCCardName;
            ViewBag.TopUpAmount = TopUpAmount;
            ViewBag.Fees = Fees;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.TotalAmount = TotalAmount;
            ViewBag.AmountInLocalCurrency = AmountInLocalCurrency;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            return View();
        }
    }
}