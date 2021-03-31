using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class FinishEstimatingFeeController : Controller
    {
        // GET: FinishEstimatingFee
        public ActionResult Index()
        {
            if ((FaxerSession.LoggedUser == null))
            {
                FaxerSession.FromUrl = "/PaymentMethod";
                return RedirectToAction("Login", "FaxerAccount");
            }
            return View();
        }
    }
}