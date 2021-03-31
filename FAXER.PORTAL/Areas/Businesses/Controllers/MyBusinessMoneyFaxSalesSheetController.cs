using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class MyBusinessMoneyFaxSalesSheetController : Controller
    {
        // GET: Businesses/MyBusinessMoneyFaxSalesSheet
        public ActionResult Index()
        {   
            return View();
        }
        public ActionResult SalesSheet(string FromDate, string ToDate) {

            if (Common.BusinessSession.LoggedBusinessMerchant == null)
            {

                return RedirectToAction("Login", "BusinessLogin");
            }
            Services.MyBusinessMoneyFaxSalesSheetServices services = new Services.MyBusinessMoneyFaxSalesSheetServices();
            var model = new List<ViewModels.MyBusinessMoneyFaxSalesSheetViewModel>();


            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                var fromDate = Convert.ToDateTime(FromDate);
                var toDate = Convert.ToDateTime(ToDate);
                model = services.GetSalesSheetDetialsByFilterDate(fromDate, toDate);

            }
            else
            {
                model = services.GetSalesSheetDetials();
            }
            
            return View(model);

        }
    }
}