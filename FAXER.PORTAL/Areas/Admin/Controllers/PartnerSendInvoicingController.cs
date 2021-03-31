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
    public class PartnerSendInvoicingController : Controller
    {
        ViewAlertsServices Service = new ViewAlertsServices();
        CommonServices _commonServices = new CommonServices();
      
        // GET: Admin/PartnerSendInvoicing
        public ActionResult Index(string CountryCode = "", string City = "", int PartnerId = 0, string Date = "", string message = "")
        {
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            //treated as partner View Bag
            SetViewBagForAgents(CountryCode, City);
            List<InvoicingViewModel> vm = new List<InvoicingViewModel>();
            return View(vm);

        }

        [HttpGet]
        public ActionResult PartnerCreateInvoice(int Id = 0, string Country = "", string City = "")
        {
            SetViewBagForCountries();
            SetViewBagForCities(Country);
            SetViewBagForAgents(Country, City);
            CreateAgentInvoicingViewModel vm = new CreateAgentInvoicingViewModel();
            List<ItemListVm> Item = new List<ItemListVm>();
            vm.Item = Item;
            return View(vm);
        }
        [HttpPost]
        public ActionResult PartnerCreateInvoice([Bind(Include = CreateAgentInvoicingViewModel.BindProperty)]CreateAgentInvoicingViewModel model)
        {
            SetViewBagForCountries();
            SetViewBagForCities(model.Country);
            SetViewBagForAgents(model.Country, model.City);
            CreateAgentInvoicingViewModel vm = new CreateAgentInvoicingViewModel();
            return RedirectToAction("PartnerPayAnInvoiceSuccess", vm);
        }

        public ActionResult PartnerPayAnInvoiceSuccess()
        {
            AgentPayaAnInvoiceSuccessVm vm = new AgentPayaAnInvoiceSuccessVm();
            vm.Name = "Ganesh";
            vm.InvoiceNo = "9844442720";
            return View(vm);
        }

        public ActionResult InvoicePreview()
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