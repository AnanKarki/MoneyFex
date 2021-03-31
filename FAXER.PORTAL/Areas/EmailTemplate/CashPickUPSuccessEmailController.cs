using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate
{
    public class CashPickUPSuccessEmailController : Controller
    {
        // GET: EmailTemplate/CashPickUPSuccessEmail
        public ActionResult Index(string senderName = "", string receiverName = "", string city = "",
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