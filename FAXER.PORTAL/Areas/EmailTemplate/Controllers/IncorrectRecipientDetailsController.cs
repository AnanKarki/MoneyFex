using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class IncorrectRecipientDetailsController : Controller
    {
        // GET: EmailTemplate/IncorrectRecipientDetails
        public ActionResult Index(string SenderFristName = "", string ReceiverFullName = "", string BankName = ""
            , string BankAccount = "", string Country = "")
        {
            ViewBag.SenderFristName = SenderFristName;
            ViewBag.ReceiverFullName = ReceiverFullName;
            ViewBag.BankName = BankName;
            ViewBag.BankAccount = BankAccount;
            ViewBag.Country = Country;
            return View();
        }
    }
}