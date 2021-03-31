using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class TransactionCancelledController : Controller
    {
        // GET: EmailTemplate/TransactionCancelled
        public ActionResult Index(string SenderFristName = "", string TransactionNumber = "", string SendingAmount = "", string Receivingcountry = "", string Fee = "",
            string ReceiverName = "", string BankName = "", string BankAccount = "", string BankCode = "",
            TransactionServiceType TransactionServiceType = TransactionServiceType.All
            , string WalletName = "", string MFCN = "" , string SendingCurrency = "" , string ReceivingCurrency = "")
        {

            ViewBag.SenderFristName = SenderFristName;
            ViewBag.TransactionNumber = TransactionNumber;
            ViewBag.SendingAmount = SendingAmount;
            ViewBag.Receivingcountry = Receivingcountry;
            ViewBag.Fee = Fee;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.BankName = BankName;
            ViewBag.BankAccount = BankAccount;
            ViewBag.BankCode = BankCode;
            ViewBag.TransactionServiceType = TransactionServiceType;
            ViewBag.WalletName = WalletName;
            ViewBag.MFCN = MFCN;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            return View();
        }
    }
}