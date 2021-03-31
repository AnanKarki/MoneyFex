using FAXER.PORTAL.Areas.Staff.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Staff.Controllers
{
    public class StaffHomeController : Controller
    {
        StaffMessageServices Service = new StaffMessageServices();
        // GET: Staff/StaffHome
        public ActionResult Index()
        {

            DemoLoginModel model = new DemoLoginModel();
            model.UserName = "Demo";
            model.Password = "Demo123@";
            Common.FaxerSession.DemoLoginModel = model;
            //StaffInformation staffInformation = Common.StaffSession.StaffInformation ?? new StaffInformation();
            var staffInformation = Common.StaffSession.LoggedStaff;
            if (staffInformation != null)
            {
                ViewBag.Count = Service.getInboxCount();
                ViewBag.DraftCount = Service.getDraftCount();
                Services.StaffNoticeboardServices noticeboardServices = new Services.StaffNoticeboardServices();
                var result = noticeboardServices.GetNoticeList();
                return View(result);
            }
            
           
            return RedirectToAction("StaffMainLogin" , "StaffLogin");
        }
    }
}