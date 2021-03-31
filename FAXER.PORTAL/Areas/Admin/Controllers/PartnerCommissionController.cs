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
    public class PartnerCommissionController : Controller
    {
        CommonServices _commonServices = new CommonServices();
        // GET: Admin/PartnerCommission
        public ActionResult CurrentCommission(string CountryCode = "", string City = "", string Partner = "", int Service = 0, int Type = 0)
        {

            List<PartnerCommissionViewModel> vm = new List<PartnerCommissionViewModel>();
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            SetViewBagForAgents(CountryCode, City);
            ViewBag.Partner = Partner;

            if (!string.IsNullOrEmpty(Partner))
            {
                Partner = Partner.Trim();
            }
            if (Service > 0)
            {
            }
            if (Type > 0)
            {
            }

            return View(vm);
        }

        public ActionResult SetCurrentCommission(string CountryCode = "", string City = "")
        {
            SetViewBagForCountries();
            SetViewBagForCities(CountryCode);
            SetViewBagForAgents(CountryCode, City);
            PartnerCurrentCommisionViewModel vm = new PartnerCurrentCommisionViewModel();
            return View(vm);
        }
        [HttpPost]
        public ActionResult SetCurrentCommission([Bind(Include = PartnerCurrentCommisionViewModel.BindProperty)] PartnerCurrentCommisionViewModel model)
        {
            SetViewBagForCountries();
            SetViewBagForCities(model.Country);
            SetViewBagForAgents(model.Country, model.City);
            PartnerCurrentCommisionViewModel vm = new PartnerCurrentCommisionViewModel();
            return View(vm);
        }

        public ActionResult History(string CountryCode = "", string City = "", string Partner = "", int Service = 0, int Type = 0)
        {
            List<PartnerCommissionViewModel> vm = new List<PartnerCommissionViewModel>();
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            SetViewBagForAgents(CountryCode, City);
            ViewBag.Partner = Partner;
            if (!string.IsNullOrEmpty(Partner))
            {
                Partner = Partner.Trim();
            }
            if (Service > 0)
            {
            }
            if (Type > 0)
            {
            }
            return View(vm);

        }

        private void SetViewBagForCountries()
        {
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public void SetViewBagForCities(string Country = "")
        {
            ViewAlertsServices Service = new ViewAlertsServices();

            var cities = Service.GetCities();
            if (!(string.IsNullOrEmpty(Country)))
            {
                cities = Service.GetCitiesFromCountry(Country);
            }
            ViewBag.Cities = new SelectList(cities, "City", "City");
        }

        public void SetViewBagForAgents(string countryCode = "", string city = "")
        {
            ViewAlertsServices Service = new ViewAlertsServices();
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