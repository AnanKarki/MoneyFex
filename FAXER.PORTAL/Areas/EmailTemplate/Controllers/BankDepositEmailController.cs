using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class BankDepositEmailController : Controller
    {
        // GET: EmailTemplate/BankDepositEmail
        public ActionResult Index(string senderName = "", string receiverName = "",
            string receiptNo = "", decimal SendingAmount = 0,
            decimal ReceivingAmount = 0, string CountryCode = "",
            string receivingCountry = "", string ReceiveingCurrency = "",
            string sendingCurrency = "", string bankName = "",
            decimal fee = 0, string receiverAccountNo = "", string bankCode = "" , BankDepositStatus status = 0  )
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
            ViewBag.Status = status;
            return View();
        }
        public ActionResult ManualbankDepositEmail(string senderName = "", string receiverName = "", string receiptNo = "",
                                                   decimal SendingAmount = 0, decimal ReceivingAmount = 0, string CountryCode = "",
                                                   string receivingCountry = "", string ReceiveingCurrency = "", string sendingCurrency = "",
                                                   string bankName = "", decimal fee = 0, string receiverAccountNo = "", string bankCode = "",
                                                   string receiverFirstName = "")
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
            return View();
        }

        public ActionResult ManualbankDepositSuccessEmail(string senderName = "", string receiverName = "", string receiptNo = "",
                                                   decimal SendingAmount = 0, decimal ReceivingAmount = 0, string CountryCode = "",
                                                   string receivingCountry = "", string ReceiveingCurrency = "", string sendingCurrency = "",
                                                   string bankName = "", decimal fee = 0, string receiverAccountNo = "", string bankCode = "",
                                                   string receiverFirstName = "")
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
            return View();
        }



        public ActionResult CashPickUPEmail(string senderName = "", string receiverName = "", string receiverFirstName = "",
                                            string MFCN = "", string sendingAmount = "", string ReceivingAmount = "",
                                            string Fee = "", string receivingCountry = "")
        {
            ViewBag.SenderFirstName = senderName;
            ViewBag.receiverFullName = receiverName;
            ViewBag.ReceiverFirstName = receiverFirstName;
            ViewBag.MFCN = MFCN;
            ViewBag.sendingAmount = sendingAmount;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.Fee = Fee;
            ViewBag.receivingCountry = receivingCountry;
            return View();
        }

        public ActionResult CashPickUPSuccessEmail(string senderName = "", string receiverName = "", string city = "",
                                           string sendingAmount = "", string receivingCountry = "")
        {

            ViewBag.SenderFirstName = senderName;
            ViewBag.receiverFullName = receiverName;
            ViewBag.sendingAmount = sendingAmount;
            ViewBag.City = city;
            ViewBag.receivingCountry = receivingCountry;
            return View();
        }






    }
}