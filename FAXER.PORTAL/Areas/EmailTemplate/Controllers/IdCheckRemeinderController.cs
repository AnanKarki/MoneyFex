using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class IdCheckRemeinderController : Controller
    {
        // GET: EmailTemplate/IdCheckRemeinder
        public ActionResult Index(string SenderFristName = "", string ReceiverFirstName = "", string TransactionNumber = "",
            string SendingCurrency = "", string SendingAmount = "", string SendingCountry = "", string Fee = "",
            string ReceivingCurrency = "", string ReceivingAmount = "", string BankName = "", string BankAccount = "", string BranchCode = "")
        {
            ViewBag.SenderFristName = SenderFristName;
            ViewBag.ReceiverFirstName = ReceiverFirstName;
            ViewBag.TransactionNumber = TransactionNumber;
            ViewBag.SendingCurrency = SendingCurrency;
            ViewBag.SendingAmount = SendingAmount;
            ViewBag.SendingCountry = SendingCountry;
            ViewBag.Fee = Fee;
            ViewBag.ReceivingCurrency = ReceivingCurrency;
            ViewBag.ReceivingAmount = ReceivingAmount;
            ViewBag.BankName = BankName;
            ViewBag.BankAccount = BankAccount;
            ViewBag.BranchCode = BranchCode;
            return View();
        }
    }
}