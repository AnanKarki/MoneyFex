using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class DemoLoginController : Controller
    {
        // GET: DemoLogin
        public ActionResult Index()
        {
            return PartialView("_Demo_Login");
        }

        [HttpPost]
        public ActionResult Index(string UserName, string Password)
        {
            string validUserName = Common.Common.DemoUserName.GetAppSettingValue();
            string ValidPasword = Common.Common.DemoPassword.GetAppSettingValue();
            if (UserName.ToLower() == validUserName.ToLower() && Password == ValidPasword)
            {
                Common.DemoLoginModel model = new Common.DemoLoginModel() { Password = Password, UserName = UserName };
                Common.FaxerSession.DemoLoginModel = model;
                Common.FaxerSession.AreaName = "EmailTemplate";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Message = "Invalid Login";
            return PartialView("_Demo_Login");
        }
    }
}