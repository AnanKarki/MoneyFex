using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class RefundIssuedController : Controller
    {
        // GET: EmailTemplate/RefundIssued
        public ActionResult Index(string SenderFristName = "", string SendingCurrency = "", string RefundAmount = "",
            string TransactionNumber = "", string SendingAmount = "", string SendingCountry = "", string Fee = "", string ReceiverFirstName = "",
            string ReceivingCurrency = "", string ReceivingAmount = "", string BankName = "", string BankAccount = "", string BranchCode = "",
            TransactionServiceType transactionServiceType = TransactionServiceType.BankDeposit, string WalletName = "", string MobileNo = "")
        {
            ViewBag.SenderFristName = SenderFristName;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.RefundAmount = RefundAmount;
            ViewBag.TransactionNumber = TransactionNumber;
            ViewBag.SendingAmount = SendingAmount;
            ViewBag.SendingCountry = SendingCountry;
            ViewBag.Fee = Fee;
            ViewBag.ReceiverFirstName = ReceiverFirstName;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.BankName = BankName;
            ViewBag.BankAccount = BankAccount;
            ViewBag.BranchCode = BranchCode;

            ViewBag.TransactionServiceType = transactionServiceType;
            ViewBag.WalletName = WalletName;
            ViewBag.MobileNo = MobileNo;

            return View();
        }
    }
}