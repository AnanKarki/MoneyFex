using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts
{
    public class MFBCCardUserWithdrawlReceiptController : Controller
    {
        // GET: EmailTemplate/MFBCCardUserWithdrawlReceipt
        public ActionResult Index(string MFReceiptNumber, string TransactionDate, string TransactionTime, string BusinessMerchantName,string BusinessCountry,
            string  BusinessCity, string MFBCCardNumber, string BusinessCardUserFullName
      , string Telephone, string AgentName, string AgentCode, string AmountRequested, string ExchangeRate, string AmountWithdrawn, string Currency
            , string AgentCountry , string AgentCity , string AgentTelephoneNumber)
        {
            ViewBag.MFReceiptNumber = MFReceiptNumber;
            ViewBag.TransactionDate = TransactionDate;
            ViewBag.TransactionTime = TransactionTime;
            ViewBag.BusinessMerchantName = BusinessMerchantName;
            ViewBag.BusinessCountry = BusinessCountry;
            ViewBag.BusinessCity = BusinessCity;
            ViewBag.MFBCCardNumber = MFBCCardNumber;
            ViewBag.BusinessCardUserFullName = BusinessCardUserFullName;
            ViewBag.Telephone = Telephone;
            ViewBag.AgentName = AgentName;
            ViewBag.AgentCode = AgentCode;
            ViewBag.AmountRequested = AmountRequested + " " +  Currency;
            ViewBag.ExchangeRate = "1.0";
            ViewBag.AmountWithdrawn = AmountWithdrawn + " " +  Currency;
            ViewBag.AgentCountry = AgentCountry;
            ViewBag.AgentCity = AgentCity;
            ViewBag.AgentTelephoneNumber = AgentTelephoneNumber;
            return View();
        }
    }
}