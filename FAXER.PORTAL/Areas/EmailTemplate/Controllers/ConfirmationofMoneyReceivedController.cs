using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ConfirmationofMoneyReceivedController : Controller
    {
        // GET: EmailTemplate/ConfirmationofMoneyReceived
        public ActionResult Index(string FaxerName , string ReceiverName, string ReceiverCurrency,string AgentCity , string AgentCountry)
        {
            ViewBag.FaxerName = FaxerName;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.ReceiverCurrency = ReceiverCurrency;
            ViewBag.AgentCity = AgentCity;
            ViewBag.AgentCountry = AgentCountry;
            return View();
        }
    }
}