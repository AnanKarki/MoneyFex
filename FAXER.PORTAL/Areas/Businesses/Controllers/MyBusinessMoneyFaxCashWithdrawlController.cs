using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class MyBusinessMoneyFaxCashWithdrawlController : Controller
    {
        // GET: Businesses/MyBusinessMoneyFaxCashWithdrawl
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CashWithdrawlSheet(string FromDate , string ToDate)
        {
            if (Common.BusinessSession.LoggedBusinessMerchant == null)
            {

                return RedirectToAction("Login", "BusinessLogin");
            }
            Services.MFBCCardWithdrawlServices services = new Services.MFBCCardWithdrawlServices();
           
            var model = new List<ViewModels.MFBCCardWithdrawlViewModel>();


            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                var fromDate = Convert.ToDateTime(FromDate);
                var toDate = Convert.ToDateTime(ToDate);
                model = services.GetMFBCCardWithdrawlDetailsByFilterDate(fromDate, toDate);

            }
            else
            {
                 model = services.GetMFBCCardWithdrawlDetails();
            }
            return View(model);

        }
    }
}