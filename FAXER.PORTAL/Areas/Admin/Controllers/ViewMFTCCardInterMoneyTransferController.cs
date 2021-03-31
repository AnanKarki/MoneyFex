using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewMFTCCardInterMoneyTransferController : Controller
    {
        ViewMFTCCardInterMoneyTransferServices service = new ViewMFTCCardInterMoneyTransferServices();
        CommonServices CommonService = new CommonServices();
        // GET: Admin/ViewMFTCCardInterMoneyTransfer
        public ActionResult Index(string CountryCode = "", string City = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);


            var vm = service.getList(CountryCode, City);
            ViewBag.Country = CountryCode;
            return View(vm);
        }



        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Faxer, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
    }
}