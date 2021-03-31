using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts.MerchantAndCardUser
{
    public class ConfirmationofServiceProviderPaymentReceiptController : Controller
    {
        // GET: EmailTemplate/ConfirmationofServiceProviderPaymentReceipt
        public ActionResult Index(string ReceiptNumber, string Date, string Time, string SenderName, string ServiceProvideName, string BusinessMFCode, string TopUpAmount
      , string Fees, string ExchangeRate, string TotalAmount, string AmountInLocalCurrency , string SendingCurrency , string ReceivingCurrency
            , string SenderCountry , string BusinessCountry , string BusinessCity)
        {

            ViewBag.MFReceiptNumber = ReceiptNumber;
            ViewBag.TransactionDate = Date;
            ViewBag.TransactionTime = Time;
            ViewBag.SenderName = SenderName;
            ViewBag.ServiceProviderName = ServiceProvideName;
            ViewBag.BusinessMFCode = BusinessMFCode;
            ViewBag.TopUpAmount = TopUpAmount;
            ViewBag.Fees = Fees;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.TotalAmount = TotalAmount;
            ViewBag.AmountInLocalCurrency = AmountInLocalCurrency;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            ViewBag.SenderCountry = SenderCountry;
            ViewBag.BusinessCountry = BusinessCountry;
            ViewBag.BusinessCity = BusinessCity;

            return View();
        }
    }
}