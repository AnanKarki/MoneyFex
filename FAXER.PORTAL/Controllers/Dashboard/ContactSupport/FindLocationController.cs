using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Dashboard.ContactSupport
{
    public class FindLocationController : Controller
    {

        private DB.FAXEREntities dbContext = null;

        private Areas.Admin.Services.CommonServices CommonServices = null;

        private SFindLocation findLocationServices = null;

        public FindLocationController()
        {
            dbContext = new DB.FAXEREntities();
            CommonServices = new Areas.Admin.Services.CommonServices();
            findLocationServices = new SFindLocation();

        }
        // GET: FindLocation
        public ActionResult Index(int? ServiceType, string Country = "", string City = "", string PostalCode = "", string SearchText = "", bool ViewAll = false)
        {

            var countries = CommonServices.GetCountries();

            ViewBag.Country = new SelectList(countries, "Code", "Name");

            var cities = CommonServices.GetCities(Country);
            ViewBag.Cities = new SelectList(cities, "City", "City");

            Models.FindLocationVm vm = new Models.FindLocationVm();
            if (string.IsNullOrEmpty(Country))
            {

                vm.Country = Country;
            }
            if (string.IsNullOrEmpty(City))
            {

                vm.City = City;
            }

            vm.ServiceTypeDetails = findLocationServices.GetServiceTypeDetails(ServiceType, Country, City, PostalCode);

            if (!ViewAll)
            {
                if (vm.ServiceTypeDetails.Count() > 10)
                {
                    vm.ServiceTypeDetails = vm.ServiceTypeDetails.Take(9).ToList();

                }
            }

            if (!string.IsNullOrEmpty(SearchText))
            {

                vm.ServiceTypeDetails = (from c in vm.ServiceTypeDetails
                                         where c.BusinessName.Contains(SearchText) || c.BusinessType.Contains(SearchText)
                                         select c).ToList();


            }
            
            return View(vm);
        }


        [HttpPost]
        public ActionResult Index([Bind(Include = Models.FindLocationVm.BindProperty)] Models.FindLocationVm vm)
        {

            return View(vm);
        }

    }




}