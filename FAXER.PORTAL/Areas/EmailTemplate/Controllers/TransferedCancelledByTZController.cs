using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class TransferedCancelledByTZController : Controller
    {
        // GET: EmailTemplate/TransferedCancelledByTZ
        public ActionResult Index(string SenderFristName = "", string TransactionNumber = "", string RecipentName = "",
            string BankName = "", string BankAccount = "", string ReceiverCountry = "",
            DB.TransactionTransferMethod transferMethod = DB.TransactionTransferMethod.BankDeposit, string WalletName = "", string MobileNo = "")
        {
            ViewBag.SenderFristName = SenderFristName;
            ViewBag.TransactionNumber = TransactionNumber;
            ViewBag.RecipentName = RecipentName;
            ViewBag.BankName = BankName;
            ViewBag.BankAccount = BankAccount;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.TransferMethod = transferMethod;
            ViewBag.WalletName = WalletName;
            ViewBag.MobileNo = MobileNo;
            return View();
        }
    }
}