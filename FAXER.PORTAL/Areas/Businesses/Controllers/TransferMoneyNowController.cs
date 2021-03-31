using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class TransferMoneyNowController : Controller
    {
        // GET: Businesses/TransferMoneyNow
        public ActionResult Index()
        {

            if (Common.BusinessSession.MerchantHasMFBCCard == false)
            {

                return RedirectToAction("Index", "BusinessHome");
            }
            return View();
        }
    }
}