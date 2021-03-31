using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAXER.PORTAL.Areas.CardUsers.ViewModels;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class MyBusinessMoneyFaxExpenditureSheetController : Controller
    {
        // GET: Businesses/MyBusinessMoneyFaxExpenditureSheet
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ExpenditureSheet(string FromDate , string ToDate , int Options = 0)
        {
            if (Common.BusinessSession.LoggedBusinessMerchant == null)
            {

                return RedirectToAction("Login", "BusinessLogin");
            }

            Services.MyBusinessMoneyFaxExpenditureSheetServices services = new Services.MyBusinessMoneyFaxExpenditureSheetServices();
            var model = new ViewModels.MyBusinessMoneyFaxExpenditureSheetViewModel();
            model.purchaseViewModels = new List<ViewModels.MyBusinessMoneyFaxPurchaseViewModel>();
            model.topUpViewModels = new List<ViewModels.MyBusinessMoneyFaxTopUpViewModel>();
            
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                var fromDate = Convert.ToDateTime(FromDate);
                var toDate = Convert.ToDateTime(ToDate);

                if (Options == 1)
                {
                    model.purchaseViewModels = services.FilterExpenditureByDate(fromDate, toDate);
                }else if(Options == 2)
                {
                    model.topUpViewModels = services.GetFilterTopUpDetails(fromDate, toDate);

                }
            }
            else {
                
                if (Options == 1)
                {
                    model.purchaseViewModels = services.GetMyBusinessMoneyFaxExpendituresDetials();
                    
                }
                else if (Options == 2)
                {
                    model.topUpViewModels = services.GetTopUpDetails();

                }
            }
            model.Options = (Options)Options;
            return View(model);
        }
    }
}