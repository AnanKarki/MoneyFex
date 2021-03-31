using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class TransactionPausedController : Controller
    {
        // GET: EmailTemplate/TransactionPaused
        public ActionResult Index(string SenderFristName = "", string TransactionNumber = "", string SendingAmount = "", string ReceivingAmount = "", string ReceivingCountry = "", string Fee = "",
              string ReceiverFirstName = "", string BankName = "", string BankAccount = "", string BankCode = "", TransactionServiceType TransactionServiceType = TransactionServiceType.All,
              string WalletName = "", string MFCN = "")
        {
            ViewBag.SenderFristName = SenderFristName;
            ViewBag.TransactionNumber = TransactionNumber;
            ViewBag.SendingAmount = SendingAmount;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.Receivingcountry = ReceivingCountry;
            ViewBag.Fee = Fee;
            ViewBag.ReceiverFirstName = ReceiverFirstName;
            ViewBag.BankName = BankName;
            ViewBag.BankAccount = BankAccount;
            ViewBag.BankCode = BankCode;
            ViewBag.TransactionServiceType = TransactionServiceType;
            ViewBag.WalletName = WalletName;
            ViewBag.MFCN = MFCN;
            return View();
        }
    }
}