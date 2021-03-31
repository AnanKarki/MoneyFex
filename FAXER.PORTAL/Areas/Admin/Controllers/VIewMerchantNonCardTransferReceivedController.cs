using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class VIewMerchantNonCardTransferReceivedController : Controller
    {


        Services.CommonServices CommonService = null;
        Services.VIewMerchantNonCardTransferReceivedServices ViewMerchantNonCardTransferReceivedServices = null;
        public VIewMerchantNonCardTransferReceivedController()
        {
            CommonService = new Services.CommonServices();
            ViewMerchantNonCardTransferReceivedServices = new Services.VIewMerchantNonCardTransferReceivedServices();

        }
        // GET: Admin/VIewMerchantNonCardTransferReceived
        public ActionResult Index(string CountryCode , string City)
        {

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            var cities = SCity.GetCities(DB.Module.BusinessMerchant, CountryCode);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
            ViewBag.Country = CountryCode;
            ViewBag.City = City;
            var vm = ViewMerchantNonCardTransferReceivedServices.GetTransactionDetails(CountryCode, City);

            return View(vm);
            
        }

        public ActionResult ViewMore(int Id) {

            var vm = ViewMerchantNonCardTransferReceivedServices.GetAdditonalInformatinViewModel(Id);
            return View(vm);
        }
        
    }
}