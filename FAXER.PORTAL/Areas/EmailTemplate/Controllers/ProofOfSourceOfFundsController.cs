using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class ProofOfSourceOfFundsController : Controller
    {
        // GET: EmailTemplate/ProofOfSourceOfFunds
        public ActionResult Index(string senderFirstname = "", string receiverFullName = "")
        {
            ViewBag.SenderFirstName = senderFirstname;
            ViewBag.ReceiverFullName = receiverFullName;
            return View();
        }
    }
}