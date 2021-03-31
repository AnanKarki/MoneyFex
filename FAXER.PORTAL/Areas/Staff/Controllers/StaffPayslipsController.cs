using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Areas.Staff.Services;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Staff.Controllers
{
    public class StaffPayslipsController : Controller
    {
        // GET: Staff/StaffPayslips
        StaffMessageServices Service = new StaffMessageServices();
        public ActionResult Index()
        {
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            return View();
        }
        public ActionResult Payslips(Month month = 0)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            ViewBag.Month = (Month)month;
            StaffPaysilpServices paysilpServices = new StaffPaysilpServices();
            var vm = paysilpServices.GetStaffPayslipsList(month);
            return View(vm);
        }
    }
}