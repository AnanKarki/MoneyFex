using FAXER.PORTAL.Areas.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AUXAgentDashBoardController : Controller
    {
        AuxAgentDocumentationServices _auxAgnetDoumentationServices = null;
        public AUXAgentDashBoardController()
        {
            _auxAgnetDoumentationServices = new AuxAgentDocumentationServices();
        }
        // GET: Admin/AUXAgentDashBoard
        public ActionResult Index()
        {
            Common.StaffSession.IsFromAuxAgnet = true;
            ViewBag.NumberOfRegisteredAuxAgnet = _auxAgnetDoumentationServices.GetNumberOfRegisteredAuxAgnet();
            ViewBag.NumberOfSenderRegisterdByAgent = _auxAgnetDoumentationServices.GetNumberOfSenderRegisteredByAuxAgnet();
            return View();
        }
    }
}