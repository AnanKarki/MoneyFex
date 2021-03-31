using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AgentComplianceReportController : Controller
    {
        // GET: Admin/AgentComplianceReport
        CommonServices _CommonServices = null;

        public AgentComplianceReportController()
        {
            _CommonServices = new CommonServices();
        }
        public ActionResult Index(string Country = "", string City = "", int AgentId = 0,int? page=null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetViewBagForCountries();
            SetViewBagForSCities(Country);
            SetViewBagForAgents(Country, City);
         
            List<AgentComplianceReportViewModel> vm = new List<AgentComplianceReportViewModel>();
            int pageSize = 10;
            int PageNumber = (page ?? 1);
            return View(vm);
        }


        [HttpGet]
        public ActionResult UploadReport(int Id=0,string Country="",string City="")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForCities(Country);
            SetViewBagForAgents(Country, City);

            AgentComplianceReportViewModel vm = new AgentComplianceReportViewModel();
            return View(vm);
        }

        public ActionResult Delete(int Id)
        {
            return RedirectToAction("Index", "AgentComplianceReport");
        }
        private void SetViewBagForCountries()
        {
            var countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }




        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Staff, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
        public void SetViewBagForCities(string country = "")
        {
            ViewAlertsServices Service = new ViewAlertsServices();
            var cities = Service.GetCities();
            if (country != "")
            {
                cities = Service.GetCitiesFromCountry(country);
            }

            ViewBag.Cities = new SelectList(cities, "City", "City");
        }
        public void SetViewBagForAgents(string countryCode = "", string city = "")
        {
            if (string.IsNullOrEmpty(city) == false)
            {
                city = city.Trim();
                city = city.ToLower();
            }
            ViewAlertsServices Service = new ViewAlertsServices();
            var agents = Service.GetEmptyAgentsList();
            if ((string.IsNullOrEmpty(countryCode) == false) && (string.IsNullOrEmpty(city) == false))
            {
                agents = Service.GetAgentsFromCountryCity(countryCode, city);
            }
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
        }
    }
}