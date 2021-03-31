using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate
{
    public class ManualBankDepositSuccessEmailController : Controller
    {
        // GET: EmailTemplate/ManualBankDepositSuccessEmail
        public ActionResult Index(string senderName = "", string fee = "", string sendingAmount = "",
                                                   string receiverAccountNo = "" , string receivingAmount = "", string receiptNo = "",
                                                   string bankName = "", string receiverName = "", string bankCode = "",
                                                   string receivingCountry = "", string receiverFirstName = "")
        {
            ViewBag.FirstName = senderName;
            ViewBag.receiverFullName = receiverName;
            ViewBag.transactionNo = receiptNo;
            ViewBag.SendingAmount = sendingAmount;
            ViewBag.ReceivingAmount = receivingAmount;
            //ViewBag.CountryCode = CountryCode;
            //ViewBag.ReceiveingCurrency = ReceiveingCurrency;
            //ViewBag.sendingCurrency = sendingCurrency;
            ViewBag.BankName = bankName;
            ViewBag.fee = fee;
            ViewBag.bankCode = bankCode;
            ViewBag.bankAccount = receiverAccountNo;
            ViewBag.receiverFirstName = receiverFirstName;
            ViewBag.receivingCountry = receivingCountry;
            return View();
        }

    }
}