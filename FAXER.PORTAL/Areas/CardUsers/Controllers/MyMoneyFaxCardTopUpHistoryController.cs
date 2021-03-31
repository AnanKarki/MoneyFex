using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class MyMoneyFaxCardTopUpHistoryController : Controller
    {
        // GET: CardUsers/MyMoneyFaxCardTopUpHistory
        public ActionResult Index(string FromDate , string ToDate)
        {
            if (Common.CardUserSession.LoggedCardUserViewModel == null)
            {

                return RedirectToAction("Login", "CardUserLogin");
            }
            Services.MyMoneyFaxCardTopUpHistoryServices services = new Services.MyMoneyFaxCardTopUpHistoryServices();
            var model = new List<ViewModels.MyMoneyFaxCardTopUpHistoryViewModel>();


            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                var fromDate = Convert.ToDateTime(FromDate);
                var toDate = Convert.ToDateTime(ToDate);
                model = services.FilterTopUpCardHistoryDetails(fromDate, toDate);

            }
            else
            {
                model = services.GetAllTopUpCardHistoryDetails();
            }
            return View(model);
        }

    }
}