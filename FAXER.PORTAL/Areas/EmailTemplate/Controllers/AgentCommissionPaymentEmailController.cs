using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class AgentCommissionPaymentEmailController : Controller
    {
        // GET: EmailTemplate/AgentCommissionPaymentEmail
        public ActionResult Index(string NameOfAgent, string AgentMFCode, string FaxedAndRcVedComm, string TotalSendPayment, string TotalRcVedPayment)
        {
            ViewBag.NameOfAgent = NameOfAgent;
            ViewBag.AgentMFCode = AgentMFCode;
            ViewBag.FaxedAndRcVedComm = FaxedAndRcVedComm;
            ViewBag.TotalSendPayment = TotalSendPayment;
            ViewBag.TotalRCVedPayment = TotalRcVedPayment;
            return View();
        }
    }
}