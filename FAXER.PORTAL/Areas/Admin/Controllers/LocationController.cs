using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class LocationController : Controller
    {
        CommonServices CommonService = new CommonServices();
        LocationServices Service = new LocationServices();
        // GET: Admin/Location
        public ActionResult Index(string message = "",string CountryCode="",string City="",int AgentId=0,int? page=null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            setViewBagForAgents(CountryCode, City);
            if (message == "success")
            {
                ViewBag.Message = "Location Saved Successfully !";
                message = "";
            }
            else if (message == "messageDeleted")
            {
                ViewBag.Message = "Message Successfully Deleted !";
                message = "";
            }
            else if (message == "messageNotDeleted")
            {
                ViewBag.Message = "Something went wrong. Please try again !";
                message = "";
            }
            if (message == "successEdit")
            {
                ViewBag.Message = "Location update Successfully !";
                message = "";
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<LocationViewModel> vm = Service.getView().ToPagedList(pageNumber,pageSize);
            if (!string.IsNullOrEmpty(CountryCode))
            {

                vm = vm.Where(x => CommonService.getCurrencyCodeFromCountry(x.Country) == CountryCode).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(City))
            {

                vm = vm.Where(x => x.City == City).ToPagedList(pageNumber, pageSize);
            }
            if (AgentId!=0)
            {

                vm = vm.Where(x => x.AgentId == AgentId).ToPagedList(pageNumber, pageSize);
            }
            if (CountryCode!=null)
            {
                ViewBag.Country = CountryCode;
            }
            if (City != null)
            {
                ViewBag.City = City;
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddNewLocation(int id=0,string CountryCode = "", string City = "", int agentType = 0, int agentId = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            AddNewLocationViewModel vm = new AddNewLocationViewModel();
            SetViewBagForCountries();
            SetViewBagForCities(CountryCode);
            setViewBagForAgents(CountryCode, City,agentType);
            if (!string.IsNullOrEmpty(CountryCode))
            {
                vm.Country = CountryCode;
            }
            if (!string.IsNullOrEmpty(City))
            {
                vm.City = City;
            }

            if (agentId != 0)
            {
                vm.AgentId = agentId;
            }
            if (agentId != 0)
            {
                vm.Address = Service.getAddress(agentType, agentId);
                vm.ContactNo = Service.getContactNo(agentType, agentId);
            }
            vm.AgentType = (AgentType)agentType;

            if(id!=0)
            {
                vm=Service.GetLocation(id);
                setViewBagForAgents(vm.Country, vm.City, (int)vm.AgentType);
            }
            return View(vm);
        }


        [HttpPost]
        public ActionResult AddNewLocation([Bind(Include = AddNewLocationViewModel.BindProperty)]AddNewLocationViewModel model)
        {
            if (model != null)
            {
                bool valid = true;
                if (string.IsNullOrEmpty(model.Country))
                {
                    ModelState.AddModelError("Country", "Please select Country !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.City))
                {
                    ModelState.AddModelError("City", "Please select City !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.ContactNo))
                {
                    ModelState.AddModelError("ContactNo", "This field can't be blank !");
                    valid = false;
                }
            
                if (model.AgentId == 0)
                {
                    ModelState.AddModelError("AgentId", "This field can't be blank !");
                    valid = false;
                }
                if (valid == true)
                {
                    if(model.Id>0)
                    {
                        bool result = Service.saveEditLocation(model);
                        return RedirectToAction("Index", new { @message = "successEdit" });
                    }
                    else
                    {
                        bool result = Service.saveLocation(model);
                        return RedirectToAction("Index", new { @message = "success" });
                    }
   
                }
            }
            SetViewBagForCountries();
            SetViewBagForCities(model.Country);
            setViewBagForAgents(model.Country, model.City,(int)model.AgentType);
            return View(model);
        }

        public ActionResult DeleteLocation (int id)
        {
            if (id != 0)
            {
                bool result = Service.deleteLocation(id);
                if (result)
                {
                    return RedirectToAction("Index", new { @message="messageDeleted"});
                }
            }
            return RedirectToAction("Index", new { @message="messageNotDeleted"});
        }
        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
        private void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCitiesBoth(Country);
            ViewBag.SCities = new SelectList(cities, "Name", "Name");
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

        public void setViewBagForAgents(string countryCode = "", string city = "",int agentType=0)
        {
            ViewAlertsServices Servicee = new ViewAlertsServices();
            if (string.IsNullOrEmpty(city) == false)
            {
                city = city.Trim();
                city = city.ToLower();
            }
            var agents = Servicee.GetEmptyAgentsList();
            if ((string.IsNullOrEmpty(countryCode) == false) && (string.IsNullOrEmpty(city) == false) && agentType!=0)
            {
                agents = Service.getAgentList(countryCode, city,agentType);
            }
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
        }

    }
}