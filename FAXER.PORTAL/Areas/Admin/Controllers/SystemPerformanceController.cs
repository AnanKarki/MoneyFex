using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class SystemPerformanceController : Controller
    {
        // GET: Admin/SystemPerformance
        public ActionResult PerformanceReport(string Date="",int Type=0,int Platform=0)
        {
            List<PerformanceReportViewModel> vm = new List<PerformanceReportViewModel>();
            return View(vm);
        }

        public ActionResult SecurityReport(string Date = "", int Type = 0, int Platform = 0)
        {
            List<SecurityReportViewModel> vm = new List<SecurityReportViewModel>();
            return View(vm);
        }
    }
}