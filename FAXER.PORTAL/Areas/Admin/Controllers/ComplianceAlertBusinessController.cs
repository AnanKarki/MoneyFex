using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ComplianceAlertBusinessController : Controller
    {
        // GET: Admin/ComplianceAlertBusiness
        public ActionResult Index()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            return View();
        }
        public ActionResult BunisessSendAlert()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            return View();
        }
        public ActionResult DeleteAlert()
        {

            return RedirectToAction("Index", "ComplianceAlertBusiness");
        }
    }
}