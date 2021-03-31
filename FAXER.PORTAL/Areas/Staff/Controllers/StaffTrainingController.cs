using FAXER.PORTAL.Areas.Staff.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Staff.Controllers
{
    public class StaffTrainingController : Controller
    {
        // GET: Staff/StaffTraining
        StaffMessageServices Service = new StaffMessageServices();
        public ActionResult Index()
        {
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            return View();
        }
        public ActionResult Training() {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            Services.StaffTrainingServices trainingServices = new Services.StaffTrainingServices();

            var result = trainingServices.GetStaffTrainings();
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            return View(result);
        }
    }
}