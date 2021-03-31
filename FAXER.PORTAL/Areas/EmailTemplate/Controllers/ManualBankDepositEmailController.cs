using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ManualBankDepositEmailController : Controller
    {
        // GET: EmailTemplate/ManualBankDepositEmail
        public ActionResult Index(string senderName = "", string receiverName = "", string receiptNo = "",
                                                   decimal SendingAmount = 0, decimal ReceivingAmount = 0, string CountryCode = "",
                                                   string receivingCountry = "", string ReceiveingCurrency = "", string sendingCurrency = "",
                                                   string bankName = "", decimal fee = 0, string receiverAccountNo = "", string bankCode = "",
                                                   string receiverFirstName = "", string paymentReference = "", SenderPaymentMode SenderPaymentMode = SenderPaymentMode.Cash
            ,bool IsTransactionHeld= false)
        {
            ViewBag.FirstName = senderName;
            ViewBag.receiverFullName = receiverName;
            ViewBag.transactionNo = receiptNo;
            ViewBag.SendingAmount = SendingAmount;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.CountryCode = CountryCode;
            ViewBag.receivingCountry = receivingCountry;
            ViewBag.ReceiveingCurrency = ReceiveingCurrency;
            ViewBag.sendingCurrency = sendingCurrency;
            ViewBag.BankName = bankName;
            ViewBag.fee = fee;
            ViewBag.bankCode = bankCode;
            ViewBag.bankAccount = receiverAccountNo;
            ViewBag.receiverFirstName = receiverFirstName;
            ViewBag.paymentReference = paymentReference;
            ViewBag.SenderPaymentMode = SenderPaymentMode;
            ViewBag.IsTransactionHeld = IsTransactionHeld;
            return View();
        }
    }
}