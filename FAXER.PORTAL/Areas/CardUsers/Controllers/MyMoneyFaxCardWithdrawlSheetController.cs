using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class MyMoneyFaxCardWithdrawlSheetController : Controller
    {
        // GET: CardUsers/MyMoneyFaxCardWithdrawlSheet
        public ActionResult Index(string FromDate, string ToDate)
        {
            if (Common.CardUserSession.LoggedCardUserViewModel == null) {

                return RedirectToAction("Login", "CardUserLogin");
            }

            Services.MyMoneyFaxCardWithdrawlSheetServices services = new Services.MyMoneyFaxCardWithdrawlSheetServices();
            var model = new List<ViewModels.MyMoneyFaxCardWithdrawlSheetViewModel>();


            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                var fromDate = Convert.ToDateTime(FromDate);
                var toDate = Convert.ToDateTime(ToDate);
                model = services.FilterAllDetails(fromDate, toDate);

            }
            else
            {
                model = services.GetAllDetails();
            }
            return View(model);
        }   
    }
}