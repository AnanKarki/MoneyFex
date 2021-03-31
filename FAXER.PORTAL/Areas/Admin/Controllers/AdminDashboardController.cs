using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AdminDashboardController : Controller
    {
        AdminDashboardServices Service = new AdminDashboardServices();
        CommonServices CommonServices = new CommonServices();
        // GET: Admin/AdminDashboard
        public ActionResult Index(string Country = "", string City = "", string Year = "", int Month = 0)
        {
            if (Common.AdminSession.StaffId == 0)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            Common.MiscSession.CountryCode = Country;
            Common.MiscSession.City = City;
            Common.StaffSession.IsFromAuxAgnet = false;
            var countries = CommonServices.GetCountries();

            ViewBag.Country = new SelectList(countries, "Code", "Name");

            var cities = CommonServices.GetCities(Country);
            ViewBag.Cities = new SelectList(cities, "City", "City");

            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));

            var vm = Service.GetCounts(Country, City, Year, Month);
            if (!string.IsNullOrEmpty(Country))
            {
                vm.CountryCode = Country;
            }
            if (!string.IsNullOrEmpty(City))
            {
                vm.City = City;
            }
            if (!string.IsNullOrEmpty(Year))
            {
                vm.Year = Year;
            }
            if (Month != 0)
            {
                vm.Month = (Month)Month;
            }

            return View(vm);
        }
    }
}