using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewStaffLogController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        ViewStaffLogServices Service = new ViewStaffLogServices();

        // GET: Admin/ViewStaffLog
        public ActionResult Index(string CountryCode = "", string City = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            var vm = Service.getStaffLogList(CountryCode, City);
            if (vm != null)
            {
                ViewBag.Country = CountryCode;
            }
            return View(vm);
        }



        public ActionResult ViewLogs(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (id != 0)
            {
                var vm = Service.getStaffLog(id);
                return View(vm);
            }
            return View();
        }

        public ActionResult DeleteStaffLog(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (id != 0)
            {
                var result = Service.deleteStaffLog(id);

                return RedirectToAction("ViewLogs", "ViewStaffLog", new { @id = result });

            }
            return null;
        }
        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Staff, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }

        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

    }


}