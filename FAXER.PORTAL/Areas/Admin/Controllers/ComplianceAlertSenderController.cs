using FAXER.PORTAL.Areas.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ComplianceAlertSenderController : Controller
    {
        CommonServices _CommonServices = null;
        public ComplianceAlertSenderController()
        {
            _CommonServices = new CommonServices();

        }
        // GET: Admin/ComplianceAlertSender
        public ActionResult Index()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            return View();
        }
        public ActionResult SenderSendAlert(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountryViewBag();
            return View();
        }
        public ActionResult DeleteAlert(int id = 0)
        {
            return RedirectToAction("Index", "ComplianceAlertSender");
        }
        public void SetCountryViewBag()
        {
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
        }
    }
}