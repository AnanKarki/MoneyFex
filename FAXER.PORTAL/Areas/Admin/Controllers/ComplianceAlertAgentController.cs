using FAXER.PORTAL.Areas.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ComplianceAlertAgentController : Controller
    {
        CommonServices _CommonServices = null;
        public ComplianceAlertAgentController()
        {
            _CommonServices = new CommonServices();

        }
        // GET: Admin/ComplianceAlertAgent
        public ActionResult Index()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountryViewBag();
            return View();
        }
        public ActionResult AgentSendAlert(int id = 0)
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
            return RedirectToAction("Index", "ComplianceAlertAgent");
        }

        public void SetCountryViewBag()
        {
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
        }
    }
}