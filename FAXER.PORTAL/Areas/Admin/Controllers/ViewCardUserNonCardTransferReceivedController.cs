using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewCardUserNonCardTransferReceivedController : Controller
    {
        Services.CommonServices CommonService = null;
        Services.ViewCardUserNonCardTransferReceivedServices ViewCardUserNonCardTransferReceivedServices = null;
        public ViewCardUserNonCardTransferReceivedController()
        {
            CommonService = new Services.CommonServices();
            ViewCardUserNonCardTransferReceivedServices = new Services.ViewCardUserNonCardTransferReceivedServices();
        }
        // GET: Admin/ViewCardUserNonCardTransferReceived
        public ActionResult Index(string CountryCode = "", string City = "")
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            var cities = SCity.GetCities(DB.Module.CardUser, CountryCode);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
            ViewBag.Country = CountryCode;
            ViewBag.City = City;
            var vm = ViewCardUserNonCardTransferReceivedServices.GetTransactionDetails(CountryCode, City);

            return View(vm);
            
        }

        public ActionResult ViewMore(int TransactionId)
        {

            var vm = ViewCardUserNonCardTransferReceivedServices.GetAdditionalDetails(TransactionId);
            return View(vm);
        }
    }
}