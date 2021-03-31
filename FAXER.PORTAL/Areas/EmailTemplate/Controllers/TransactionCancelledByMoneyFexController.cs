using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class TransactionCancelledByMoneyFexController : Controller
    {
        // GET: EmailTemplate/TransactionCancelledByMoneyFex
        public ActionResult Index(string SenderFristName = "", string TransactionNumber = "", string RecipentName = "",
            string BankName = "", string BankAccount = "", string ReceiverCountry = "", string receivingCurrency = "",
            string sendingCurrency = "", decimal exchangeRate = 0, decimal fee = 0, decimal receivingAmount = 0
            , decimal sendingAmount = 0, string bankCode = "", DB.TransactionTransferMethod transferMethod = DB.TransactionTransferMethod.BankDeposit,
            string walletName = "", string mobileNo = "")
        {
            ViewBag.SenderFristName = SenderFristName;
            ViewBag.TransactionNumber = TransactionNumber;
            ViewBag.RecipentName = RecipentName;
            ViewBag.BankName = BankName;
            ViewBag.BankAccount = BankAccount;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.ReceivingCurrency = receivingCurrency;
            ViewBag.SendingCurrrency = sendingCurrency;
            ViewBag.ExchangeRate = exchangeRate;
            ViewBag.Fee = fee;
            ViewBag.ReceivingAmount = receivingAmount;
            ViewBag.SendingAmount = sendingAmount;
            ViewBag.BankCode = bankCode;
            ViewBag.TransferMethod = transferMethod;
            ViewBag.WalletName = walletName;
            ViewBag.MobileNo = mobileNo;
            return View();
        }
    }
}