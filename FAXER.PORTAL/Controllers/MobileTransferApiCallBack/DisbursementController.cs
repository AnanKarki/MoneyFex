using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.MobileTransferApiCallBack
{
    public class DisbursementController : Controller
    {
        // GET: Disbursement
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Callback()
        {
            return View();
        }
    }
}