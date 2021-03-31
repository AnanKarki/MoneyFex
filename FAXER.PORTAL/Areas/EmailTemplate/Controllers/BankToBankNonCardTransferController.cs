using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class BankToBankNonCardTransferController : Controller
    {
        // GET: EmailTemplate/BankToBankNonCardTransfer
        public ActionResult Index(string FaxerName , string  FaxerAccountNo ,string ReceiverFullName,string ReceiverCountry, string ReceiverEmail 
            , string ReceiverCity , string ReceiverTelephone, string Amount , string BankReference )
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.FaxerAccountNo = FaxerAccountNo;
            ViewBag.ReceiverFullName = ReceiverFullName;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.ReceiverEmail = ReceiverEmail;
            ViewBag.ReceiverTelephone = ReceiverTelephone;
            ViewBag.ReceiverCity = ReceiverCity;
            ViewBag.Amount = Amount;
            ViewBag.BankReference = BankReference;

            return View();
        }
    }
}