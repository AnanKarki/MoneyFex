using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class RefundRequestByAdminEmailController : Controller
    {
        // GET: EmailTemplate/RefundRequestByAdminEmail
        public ActionResult Index(string FaxerName , string MFCNNumber , string FaxedAmount , string ReceiverName ,string ReceiverCountry , string ReceiverCity , string FaxedDate , string NameOfAdminRefundRequester)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.MFCNNumber = MFCNNumber;
            ViewBag.FaxedAmount = FaxedAmount;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.ReceiverCountry = ReceiverCountry;
            ViewBag.ReceiverCity = ReceiverCity;
            ViewBag.FaxedDate = FaxedDate;
            ViewBag.NameOfAdminRefundRequester = NameOfAdminRefundRequester;
            return View();
        }
    }
}