using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class FaxingAndPayFeeCalculationsController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        Services.CommonServices CommonService = new Services.CommonServices();
        // GET: Admin/FaxingAndPayFeeCalculations
        public ActionResult Index()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = CommonService.GetCountries();

            ViewBag.SourceCountries = new SelectList(countries, "Code", "Name");
            ViewBag.DestinationCountries = new SelectList(countries, "Code", "Name");

            var viewmodel = new ViewModels.FaxingAndPayFeeCalculationsViewModel();
            return View(viewmodel);
        }




        [HttpPost]
        public ActionResult Index([Bind(Include = FaxingAndPayFeeCalculationsViewModel.BindProperty)]FaxingAndPayFeeCalculationsViewModel model)
        {
            var countries = CommonService.GetCountries();

            ViewBag.SourceCountries = new SelectList(countries, "Code", "Name", model.FaxingCountry);
            ViewBag.DestinationCountries = new SelectList(countries, "Code", "Name", model.ReceivingCountry);

            string FaxingCountryCode = model.FaxingCountry;
            string ReceivingCountryCode = model.ReceivingCountry;

            model.FaxingCurrency = CommonService.getCurrencyCodeFromCountry(model.FaxingCountry);
            model.ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(model.ReceivingCountry);
            model.FaxingCurrencySymbol = CommonService.getCurrencySymbol(model.FaxingCountry);
            model.ReceivingCurrencySymbol = CommonService.getCurrencySymbol(model.ReceivingCountry);

            decimal exchangeRate = 0, faxingFee = 0;
            var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == FaxingCountryCode && x.CountryCode2 == ReceivingCountryCode).FirstOrDefault();
            if (exchangeRateObj == null)
            {
                var exchangeRateobj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == ReceivingCountryCode && x.CountryCode2 == FaxingCountryCode).FirstOrDefault();
                if (exchangeRateobj2 != null)
                {
                    exchangeRateObj = new DB.ExchangeRate();
                    exchangeRateObj.CountryCode1 = exchangeRateobj2.CountryCode2;
                    exchangeRateObj.CountryCode2 = exchangeRateobj2.CountryCode1;
                    exchangeRateObj.FaxingFee2 = exchangeRateobj2.FaxingFee1;
                    exchangeRateObj.FaxingFee1 = exchangeRateobj2.FaxingFee2;
                    exchangeRateObj.CountryRate1 = Math.Round(1 / exchangeRateobj2.CountryRate1 , 6 , MidpointRounding.AwayFromZero);
                    exchangeRateObj.CountryRate2 = exchangeRateobj2.CountryRate1;

                }
                else
                {

                    return View(model);
                }
            }
            exchangeRate = exchangeRateObj.CountryRate1;
            //   exchangeRate = Convert.ToDouble(dbContext.ExchangeRates.Where(x => x.CountryCode1 == ReceivingCountryCode && x.CountryCode2==FaxingCountryCode).Select(x => x.CountryRate2).FirstOrDefault());
            if (model.AmountToBeReceived > 0)
            {
                model.FaxingAmount = model.AmountToBeReceived;
            }
           
            var feeSummary = FAXER.PORTAL.Services.SEstimateFee.CalculateFaxingFee(model.FaxingAmount, model.IncludeFaxingFee, model.AmountToBeReceived > 0, exchangeRateObj.CountryRate1, FAXER.PORTAL.Services.SEstimateFee.GetFaxingCommision(FaxingCountryCode));




            model.AmountToBeReceived = feeSummary.ReceivingAmount;
            model.CurExchangeRate = feeSummary.ExchangeRate;

            model.FaxingAmount = feeSummary.FaxingAmount;
            model.FaxingFee = feeSummary.FaxingFee;
            model.TotalAmount = feeSummary.TotalAmount;

            return View(model);
        }


    }
}