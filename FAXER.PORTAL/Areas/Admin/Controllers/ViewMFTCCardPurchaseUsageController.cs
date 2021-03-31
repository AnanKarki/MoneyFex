using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewMFTCCardPurchaseUsageController : Controller
    {
        Services.ViewMFTCCardPurchaseUsageServices Service = new Services.ViewMFTCCardPurchaseUsageServices();
        Services.CommonServices CommonService = new Services.CommonServices();
        // GET: Admin/ViewMFTCCardPurchaseUsage
        public ActionResult Index(string CountryCode = "", string City = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            var vm = Service.getList(CountryCode, City);
            if (vm != null)
            {
                ViewBag.Country = CountryCode;
            }
            return View(vm);
        }

        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.CardUser, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
    }
}