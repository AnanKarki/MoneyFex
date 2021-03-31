using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        LoginServices Service = new LoginServices();
        // GET: Admin/Login
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Forgot()
        {
            return View();
        }

        public ActionResult NewPassword()
        {
            return View();
        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AdminLogOut()
        {
            bool result = Service.SaveStaffLoginHistory();
            if (result)
            {
                //clear your session here;
                AdminSession.Clear();
                return Redirect("/Staff/StaffLogin/AdminPortalStaffLogin");
            }
            return RedirectToAction("Index");
            
        }


    }
}