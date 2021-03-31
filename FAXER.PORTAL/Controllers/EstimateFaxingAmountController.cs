using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class EstimateFaxingAmountController : Controller
    {
        // GET: EstimateFaxingAmount      
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        CommonServices CommonService = new CommonServices();
        public ActionResult Index()
        {
            FAXER.PORTAL.Models.EstimateFaxingAmount model = new Models.EstimateFaxingAmount();
            model.FaxingCurrency = CommonService.getCurrencyCodeFromCountry(Common.FaxerSession.FaxingCountry);
            model.FaxingCurrencySymbol = CommonService.getCurrencySymbol(Common.FaxerSession.FaxingCountry);
            model.ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(Common.FaxerSession.ReceivingCountry);
            model.ReceivingCurrencySymbol = CommonService.getCurrencySymbol(Common.FaxerSession.ReceivingCountry);
            if (Session["SummaryFaxingAmount"] != null)
            {
                model.FaxingAmount = Convert.ToDecimal(Session["SummaryFaxingAmount"]);
            }
            if (Common.FaxerSession.FaxingAmountSummary != null)
            {
                if (Common.FaxerSession.FaxingAmountSummary.IncludingFaxingFee == true)
                {
                    model.FaxingAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount ;
                }
                else
                {
                    model.FaxingAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount - Common.FaxerSession.FaxingAmountSummary.FaxingFee;
                }
            }
            return View(model);
        }
        public ActionResult LoadFaxingSummary()
        {
            FAXER.PORTAL.Models.EstimateFaxingFeeSummary faxingAmount = new Models.EstimateFaxingFeeSummary();

            faxingAmount.FaxingAmount = 100;
            faxingAmount.FaxingFee = 100;

            return PartialView(faxingAmount);

        }

        [HttpPost]
        public ActionResult Index([Bind(Include = EstimateFaxingAmount.BindProperty)]EstimateFaxingAmount model)
        {
            if (model.FaxingAmount == 0 && model.ReceivingAmount == 0)
            {
                ModelState.AddModelError("Error", "Please enter an amount to proceed");
            }
            else
            {
                /*Faxing amount 

                Does this include fee?

                Yes 

                Faxing amount - Commission fee (in %)

                (instant faxing = 5%) - Fax money without using a card

                (Card faxing (top-up Card = 6%) - Fax money using a card


                ---------------------------------------------------------
                Faxing amount 

                Does this include fee?

                No 

                Faxing amount + Commission fee (in %)

                (instant faxing = 5%) - Fax money without using a card

                (Card faxing (top-up Card = 6%) - Fax money using a card

                ----------------------------------------------------------

                Note:

                No conversion with fee?




                Nepal --> UK 

                100/Exchange Rate = Amount to be received

                UK --> Nepal 



                100*Exchange Rate = Amount to be Received



                Amount to be Received: 

                Nepal --> UK 

                £100

                Behind the scene ->

                £100 * Exchange Rate  = Rupees*/

                string FaxingCountryCode = Common.FaxerSession.FaxingCountry;
                string ReceivingCountryCode = Common.FaxerSession.ReceivingCountry;

                decimal exchangeRate = 0, faxingFee = 0;
                //var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == FaxingCountryCode && x.CountryCode2 == ReceivingCountryCode).FirstOrDefault();
                //if (exchangeRateObj == null)
                //{
                //    var exchangeRateobj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == ReceivingCountryCode && x.CountryCode2 == FaxingCountryCode).FirstOrDefault();
                //    if (exchangeRateobj2 != null)
                //    {
                //        exchangeRateObj = new DB.ExchangeRate();
                //        exchangeRateObj.CountryCode1 = exchangeRateobj2.CountryCode2;
                //        exchangeRateObj.CountryCode2 = exchangeRateobj2.CountryCode1;
                //        exchangeRateObj.FaxingFee2 = exchangeRateobj2.FaxingFee1;
                //        exchangeRateObj.FaxingFee1 = exchangeRateobj2.FaxingFee2;
                //        exchangeRateObj.CountryRate1 = exchangeRateobj2.CountryRate2;
                //        exchangeRateObj.CountryRate2 = exchangeRateobj2.CountryRate1;
                //    }
                //    else
                //    {

                //        return View(model);
                //    }
                //}
                var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == FaxingCountryCode && x.CountryCode2 == ReceivingCountryCode).FirstOrDefault();
                if (exchangeRateObj == null)
                {
                    var exchangeRateobj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == ReceivingCountryCode && x.CountryCode2 == FaxingCountryCode).FirstOrDefault();
                    if (exchangeRateobj2 != null)
                    {
                        exchangeRateObj = exchangeRateobj2;
                        exchangeRate = Math.Round( 1 / exchangeRateObj.CountryRate1 , 6 , MidpointRounding.AwayFromZero);
                       
                    }

                }
                else
                {
                    exchangeRate = exchangeRateObj.CountryRate1;
                }
                if (ReceivingCountryCode == FaxingCountryCode)
                {

                    exchangeRate = 1m;

                }
                if (exchangeRate == 0)
                {

                    ViewBag.ExchangeRate = "We are yet to start operations to this country, please try again later!";
                    return View(model);
                }

                if (model.ReceivingAmount > 0)
                {
                    model.FaxingAmount = model.ReceivingAmount;
                }
                //   exchangeRate = Convert.ToDouble(dbContext.ExchangeRates.Where(x => x.CountryCode1 == ReceivingCountryCode && x.CountryCode2==FaxingCountryCode).Select(x => x.CountryRate2).FirstOrDefault());
                if (model.ReceivingAmount > 0)
                {
                    model.FaxingAmount = model.ReceivingAmount;
                }

                //var feeSummary = Services.SEstimateFee.CalculateFaxingFee(model.FaxingAmount, model.IncludeFaxingFee, model.ReceivingAmount > 0, exchangeRateObj.CountryRate1, Services.SEstimateFee.GetFaxingCommision(FaxingCountryCode));

                var feeSummary = Services.SEstimateFee.CalculateFaxingFee(model.FaxingAmount, model.IncludeFaxingFee, model.ReceivingAmount > 0, exchangeRate, Services.SEstimateFee.GetFaxingCommision(FaxingCountryCode));
                feeSummary.IncludingFaxingFee = model.IncludeFaxingFee;
                Session["FaxingDetails"] = feeSummary;
                Common.FaxerSession.FaxingAmountSummary = feeSummary;
                Common.FaxerSession.FaxingAmountSummary.FaxingCurrencySymbol = model.FaxingCurrencySymbol;
                Common.FaxerSession.FaxingAmountSummary.FaxingCurrency = model.FaxingCurrency;
                return RedirectToAction("Index", "FaxingAmountSummary", feeSummary);
                #region old code for fax calculation
                //Models.EstimateFaxingFeeSummary FeeSummary = new Models.EstimateFaxingFeeSummary();
                //decimal faxingAmount = 0;

                //var isFromReceivingAmount = model.ReceivingAmount > 0;
                //if (model.ReceivingAmount == 0 && model.FaxingAmount > 0)
                //{
                //    faxingAmount = model.FaxingAmount;
                //}
                //if (model.FaxingAmount == 0 && model.ReceivingAmount > 0)
                //{
                //    //receiving amount is in destination country currency
                //    //for india is destination amount should be multiplied by exchange rate
                //    faxingAmount = model.ReceivingAmount * exchangeRate;
                //}
                //faxingFee = faxingAmount * ((exchangeRateObj.FaxingFee1 ?? 0.00m));

                //if (model.IncludeFaxingFee == "Yes" || isFromReceivingAmount)
                //{

                //    FeeSummary.FaxingAmount = faxingAmount - faxingFee;
                //    FeeSummary.FaxingFee = faxingFee;
                //    FeeSummary.TotalAmount = FeeSummary.FaxingAmount + FeeSummary.FaxingFee;
                //    FeeSummary.ExchangeRate = exchangeRate;
                //    FeeSummary.ReceivingAmount = FeeSummary.FaxingAmount * exchangeRate;


                //}
                //else
                //{

                //    FeeSummary.FaxingAmount = faxingAmount;
                //    FeeSummary.ReceivingAmount = faxingAmount * exchangeRate;
                //    FeeSummary.FaxingFee = faxingFee;
                //    FeeSummary.ExchangeRate = exchangeRate;
                //    FeeSummary.TotalAmount = faxingAmount + faxingFee;

                //}
                //Session["FaxingDetails"] = FeeSummary;


                //return RedirectToAction("Index", "FaxingAmountSummary", FeeSummary); 
                #endregion

            }
            return View();
        }
    }
}