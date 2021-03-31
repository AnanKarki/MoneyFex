using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ExchangeRateSettingController : Controller

    {
        
        Services.CommonServices CommonService = new Services.CommonServices();
        Services.ExchangeRateSettingServices service = new Services.ExchangeRateSettingServices();
        // GET: Admin/ExchangeRateSetting
        public ActionResult Index(string SourceCountryCode = "", string DestinationCountryCode = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            //get country list
            var countries = CommonService.GetCountries();

            //source country list with selected value
            ViewBag.SourceCountries = new SelectList(countries, "Code", "Name", SourceCountryCode);
            //Destination country list with selected value
            ViewBag.DestinationCountries = new SelectList(countries, "Code", "Name", DestinationCountryCode);


            //get rates for selected countries
            //check for valid countries
            var viewmodel = new ViewModels.ExchangeRateSettingViewModel();
            if (string.IsNullOrEmpty(SourceCountryCode) == false && string.IsNullOrEmpty(DestinationCountryCode) == false)
            {
                //get rates
                viewmodel = service.ShowRate(SourceCountryCode, DestinationCountryCode);
            }
            return View(viewmodel);
        }
        [HttpGet]
        public ActionResult SetNewRate(string SourceCountryCode = "", string DestinationCountryCode = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var countries = CommonService.GetCountries();

            ViewBag.SourceCountries = new SelectList(countries, "Code", "Name", SourceCountryCode);
            //Destination country list with selected value
            ViewBag.DestinationCountries = new SelectList(countries, "Code", "Name", DestinationCountryCode);
            var viewmodel = new ViewModels.ExchangeRateSettingViewModel();
            SetNewRateViewModel setNewRateVm = new SetNewRateViewModel();
            if (string.IsNullOrEmpty(SourceCountryCode) == false && string.IsNullOrEmpty(DestinationCountryCode) == false)
            {
                //get rates
                viewmodel = service.ShowRate(SourceCountryCode, DestinationCountryCode);
                setNewRateVm.Rate = viewmodel.ExchangeRate;
            }
            return View(setNewRateVm);
        }
        [HttpPost]
        public ActionResult SetNewRate([Bind(Include = SetNewRateViewModel.BindProperty)]SetNewRateViewModel model)
        {
            if (model.DestinationCountryCode != "" && model.SourceCountryCode != "" && model.Rate != 0)
            {

               bool result= service.SetNewRate(model.SourceCountryCode,model.DestinationCountryCode,model.Rate);

               if(result)
                {
                    return RedirectToAction("SetNewRate", "ExchangeRateSetting");
                }
                else
                {
                    return RedirectToAction("SetNewRate", "ExchangeRateSetting");
                }
            }


            return RedirectToAction("SetNewRate", "ExchangeRateSetting");
        }


        public ActionResult DeleteRate([Bind(Include = SetNewRateViewModel.BindProperty)]SetNewRateViewModel model)
        {
            if (model.DestinationCountryCode != "" && model.SourceCountryCode != ""&& model.Id !=0)
            {
                bool result = service.deleteRate(model.Id);
                if (result)
                {
                    return RedirectToAction("Index", "ExchangeRateSetting");
                }
            }
            return RedirectToAction("Index", "ExchangeRateSetting");
        }

    }
}