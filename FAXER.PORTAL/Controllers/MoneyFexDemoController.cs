using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class MoneyFexDemoController : Controller
    {
        // GET: MoneyFexDemo
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string UserName, string Password, string Link)
        {
            string validUserName = Common.Common.MoneyFexDemoUserName.GetAppSettingValue();
            string ValidPasword = Common.Common.MoneyFexDemoPassword.GetAppSettingValue();
            if (UserName.ToLower() == validUserName.ToLower() && Password == ValidPasword)
            {
                //Common.DemoLoginModel model = new Common.DemoLoginModel() { Password = Password, UserName = UserName };
                //Common.FaxerSession.DemoLoginModel = model;
                //Common.FaxerSession.AreaName = "EmailTemplate";

                return Redirect(Link);
            }
            ViewBag.Message = "Invalid Login";
            if (string.IsNullOrEmpty(Link) )
            {

                TempData["URl"] = "http://birgunjnews.com/moneyfax/agent/";
            }
            else {
                TempData["URl"] = Link;
            }
            return View("");
        }


        public ActionResult Agent() {


            TempData["URl"]  = "http://birgunjnews.com/moneyfax/agent/";
            return RedirectToAction("Index");
        }

        public ActionResult Business() {


            TempData["URl"] = "http://birgunjnews.com/moneyfax/businesses/";
            return RedirectToAction("Index");
        }


        public ActionResult Sender()
        {


            TempData["URl"] = "http://birgunjnews.com/moneyfax/faxer/";
            return RedirectToAction("Index");
        }
        public ActionResult CardUser()
        {


            TempData["URl"] = "http://birgunjnews.com/moneyfax/card-user/index.php";
            return RedirectToAction("Index");
        }



    }
}