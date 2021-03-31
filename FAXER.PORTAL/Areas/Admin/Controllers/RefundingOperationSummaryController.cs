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
    public class RefundingOperationSummaryController : Controller
    {
        ViewAlertsServices Service = new ViewAlertsServices();
        CommonServices _commonServices = new CommonServices();
        // GET: Admin/RefundingOperationSummary
        public ActionResult TotalPrefundingAmount(string SendingCountry="",string Date="")
        {
            List<TotalPrefundingAmontViewModel> vm = new List<TotalPrefundingAmontViewModel>();
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            return View(vm);
        }
        public ActionResult PrefundigStatement(string CountryCode = "", string City="",int AgentId=0,string Date = "")
        {
            List<PrefundingStatementViewModel> vm = new List<PrefundingStatementViewModel>();

            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            SetViewBagForAgents(CountryCode, City);
            return View(vm);
        }


        private void SetViewBagForCountries()
        {
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public void SetViewBagForCities(string Country = "")
        {

            var cities = Service.GetCities();
            if (!(string.IsNullOrEmpty(Country)))
            {
                cities = Service.GetCitiesFromCountry(Country);
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
            var agents = Service.GetEmptyAgentsList();
            if ((string.IsNullOrEmpty(countryCode) == false) && (string.IsNullOrEmpty(city) == false))
            {
                agents = Service.GetAgentsFromCountryCity(countryCode, city);
            }
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
        }
        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Agent, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }

    }
}