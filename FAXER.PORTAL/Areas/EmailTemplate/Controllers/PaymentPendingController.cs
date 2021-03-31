using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class PaymentPendingController : Controller
    {
        // GET: EmailTemplate/PaymentPending
        public ActionResult Index(string SenderFristName = "", string TransactionNumber = "", decimal SendingAmount = 0,
            string SendingCurrency = "", decimal ExchangeRate = 0, string Receivingurrency = "", string Receivingcountry = "", decimal Fee = 0,
            string ReceiverName = "", string BankName = "", string BankAccount = "", string BankCode = "",
            TransactionServiceType TransactionServiceType = TransactionServiceType.All, string WalletName = "", string MFCN = "", int TransactionId = 0
            , string MobileNo = "")

        {
            ViewBag.SenderFristName = SenderFristName;
            ViewBag.TransactionNumber = TransactionNumber;
            ViewBag.SendingAmount = SendingAmount;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ExchangeRate = ExchangeRate;
            ViewBag.Receivingurrency = Receivingurrency;
            ViewBag.Receivingcountry = Receivingcountry;
            ViewBag.Fee = Fee;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.BankName = BankName;
            ViewBag.BankAccount = BankAccount;
            ViewBag.BankCode = BankCode;
            ViewBag.TransactionServiceType = TransactionServiceType;
            ViewBag.WalletName = WalletName;
            ViewBag.MFCN = MFCN;
            ViewBag.TransactionId = TransactionId;
            ViewBag.MobileNo = MobileNo;
            return View();
        }
    }
}