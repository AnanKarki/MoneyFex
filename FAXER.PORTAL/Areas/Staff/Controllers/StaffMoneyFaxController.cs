using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Staff.Controllers
{
    public class StaffMoneyFaxController : Controller
    {
        // GET: Staff/StaffMoneyFax
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Moneyfax() {
            return View();
        }
    }
}