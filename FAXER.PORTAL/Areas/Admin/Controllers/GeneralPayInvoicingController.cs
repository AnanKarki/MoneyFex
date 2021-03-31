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
    public class GeneralPayInvoicingController : Controller
    {
        // GET: Admin/GeneralPayInvoicing

        ViewAlertsServices Service = new ViewAlertsServices();
        CommonServices _commonServices = new CommonServices();
        public ActionResult Index(string CountryCode = "", string City = "", int GeneralId= 0, string Date = "", string message = "")
        {
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            //treated agent view bag as general
            SetViewBagForAgents(CountryCode, City);
            List<InvoicingViewModel> vm = new List<InvoicingViewModel>();
            return View(vm);
        }
        public ActionResult GeneralPayInvoice()
        {
            AgentPayInvoiceViewModel vm = new AgentPayInvoiceViewModel();
            return View(vm);
        }
        public ActionResult Pay()
        {
            return RedirectToAction("GeneralPayAnInvoiceSucess");
        }
        public ActionResult GeneralPayAnInvoiceSucess()
        {
            AgentPayaAnInvoiceSuccessVm vm = new AgentPayaAnInvoiceSuccessVm();
            return View(vm);
        }
        public ActionResult SeeInvoice()
        {
            CreateAgentInvoicingViewModel vm = new CreateAgentInvoicingViewModel();
            List<ItemListVm> Item = new List<ItemListVm>();
            vm.Item = Item;
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