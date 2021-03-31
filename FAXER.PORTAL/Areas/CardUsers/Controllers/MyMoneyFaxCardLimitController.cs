using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class MyMoneyFaxCardLimitController : Controller
    {
        // GET: CardUsers/MyMoneyFaxCardLimit
        public ActionResult Index()
        {
            if (Common.CardUserSession.LoggedCardUserViewModel == null) {

                return RedirectToAction("Login" , "CardUserLogin");
            }
            Services.MyMoneyFaxCardLimitServices services = new Services.MyMoneyFaxCardLimitServices();
            var model = services.GetCardLimitDetails();
            return View(model);
        }

    }
}