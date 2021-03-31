using FAXER.PORTAL.Areas.CardUsers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class MyMoneyFaxCardPurchaseSheetController : Controller
    {
        // GET: CardUsers/MyMoneyFaxCardPurchaseSheet
        public ActionResult Index(string FromDate , string ToDate , int Option= 0)
        {
            if (Common.CardUserSession.LoggedCardUserViewModel == null) {

                return RedirectToAction("Login", "CardUserLogin");
            }
            Services.MyMoneyFaxCardPurchaseSheetServices services = new Services.MyMoneyFaxCardPurchaseSheetServices();
            
            //return View(result);
            var model = new ViewModels.MyMoneyFaxCardPurchaseSheetAndTopupViewModel();
            model.topUpSheetViewModels = new List<ViewModels.MyMoneyFaxCardTopUpSheetViewModel>();
            model.purchaseSheetViewModels = new List<ViewModels.MyMoneyFaxCardPurchaseSheetViewModel>();

            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                var fromDate = Convert.ToDateTime(FromDate);
                var toDate = Convert.ToDateTime(ToDate);
                if (Option == 1)
                {
                    model.purchaseSheetViewModels = services.FilterAllDetails(fromDate, toDate);
                    
                }
                else if(Option == 2){
                    model.topUpSheetViewModels = services.FilterGetTopUpDetails(fromDate, toDate);
                    
                }

            }
            else
            {
                if (Option == 1)
                {
                    //model.purchaseSheetViewModels = services.FilterAllDetails(fromDate, toDate);
                    model.purchaseSheetViewModels = services.GetAllDetails();
                    
                }
                else if (Option == 2)
                {

                    model.topUpSheetViewModels = services.GetTopUpDetails();

                    
                }
               

                
            }
            model.Options = (Options)Option; 
            return View(model);

        }
    }
}