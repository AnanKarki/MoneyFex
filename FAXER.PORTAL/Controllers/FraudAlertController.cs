using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class FraudAlertController : Controller
    {
        // GET: FraudAlert
        public ActionResult Index( string FormURL="" , string BackUrl = "")
        {
            if (Common.FaxerSession.LoggedUser == null) {

                if (string.IsNullOrEmpty(FormURL) && string.IsNullOrEmpty(BackUrl)) {
                    return RedirectToAction("Login" , "FaxerAccount");
                }

                //Common.FaxerSession.FromUrl = "/FraudAlert/Index?FormURL=" + FormURL + "&BackUrl=" + BackUrl;
                Common.FaxerSession.FromUrl = FormURL;

                Common.FaxerSession.BackUrl = BackUrl;


            }
            //Common.FaxerSession.BackUrl = "/FraudAlert/Index?FormURL=" + FormURL + "&BackUrl=" + BackUrl;
            Common.FaxerSession.FromUrl = FormURL;

            Common.FaxerSession.BackUrl = BackUrl;
            ViewBag.Url = FormURL;
            ViewBag.BackUrl = BackUrl;

            return Redirect(FormURL);
            return View();
        }
    }
}