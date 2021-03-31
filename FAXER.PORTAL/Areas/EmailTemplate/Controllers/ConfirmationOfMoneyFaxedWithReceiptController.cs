using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ConfirmationOfMoneyFaxedWithReceiptController : Controller
    {
        // GET: EmailTemplate/ConfirmationOfMoneyFaxedWithReceipt
        public ActionResult Index(string FaxerName , string ReceiverName , string ReceiverCity , string MFCN , string FaxAmount , string RegisterMFTC, string FaxerCountry)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.ReceiverCity = ReceiverCity;
            ViewBag.MFCN = MFCN;
            ViewBag.FaxAmount = FaxAmount;
            ViewBag.RegisterMFTC = RegisterMFTC;
            ViewBag.FaxerCountry = FaxerCountry;
            return View();
        }
    }
}