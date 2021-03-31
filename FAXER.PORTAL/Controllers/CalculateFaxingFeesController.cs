using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class CalculateFaxingFeesController : Controller
    {
        private DB.FAXEREntities dbContext = null;
        private CommonServices CommonService = null;

        public CalculateFaxingFeesController()
        {

            dbContext = new FAXEREntities();
            CommonService = new CommonServices();

        }
        // GET: CalculateFaxingFees
        public ActionResult Index(decimal TopUpAmount = 0)
        {
            CalculateFaxingFeeVm vm = new CalculateFaxingFeeVm();
            string FaxingCountryCode = FaxerSession.FaxerCountry;
            var ReceivingCountryCode = FaxerSession.ReceivingCountry;
            vm.SendingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.FaxerCountry);
            vm.SendingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.FaxerCountry);
            vm.ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.ReceivingCountry);
            vm.ReceivingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.ReceivingCountry);

            if ((Common.FaxerSession.FaxingAmountSummary != null))
            {
                var feeSummary = Common.FaxerSession.FaxingAmountSummary;
                vm.ExchangeRate = feeSummary.ExchangeRate;
                vm.TopUpFees = feeSummary.FaxingFee;
                vm.TopUpAmount = feeSummary.FaxingAmount + feeSummary.FaxingFee;
                vm.ReceivingAmount = feeSummary.ReceivingAmount;
                vm.TotalAmountIncludingFees = feeSummary.FaxingFee + feeSummary.FaxingAmount;
                if (TopUpAmount == 0)
                {
                    return View(vm);
                }
            }


            if (TopUpAmount != 0)
            {

                var FaxerCardPhoto = dbContext.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();


                if (string.IsNullOrEmpty( FaxerCardPhoto.CardUrl )&&  (Common.Common.IsValidAmountToTransfer(TopUpAmount, 0, FaxingCountryCode) == false))
                {

                    ViewBag.ToUrl = "/CalculateFaxingFees/Index";
                    Common.FaxerSession.ToUrl = "/CalculateFaxingFees/Index";
                    ModelState.AddModelError("CardURLError", "You are about to make a transfer of over {currency} 1000, to comply with anti-money laundering regulations, MoneyFex is required by law to ask for a copy of /n " +
                        "your Photo Identification Document (ID).Please upload a copy of your ID to proceed with this Transaction.");
                    return View(vm);

                }
                var feeSummary = new EstimateFaxingFeeSummary();
                decimal exchangeRate = 0, faxingFee = 0;
                var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == FaxingCountryCode && x.CountryCode2 == ReceivingCountryCode).FirstOrDefault();
                if (exchangeRateObj == null)
                {
                    var exchangeRateobj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == ReceivingCountryCode && x.CountryCode2 == FaxingCountryCode).FirstOrDefault();
                    if (exchangeRateobj2 != null)
                    {
                        exchangeRateObj = exchangeRateobj2;
                        exchangeRate = Math.Round(1 / exchangeRateObj.CountryRate1, 6, MidpointRounding.AwayFromZero);
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
                    return View();
                }

                //feeSummary = Services.SEstimateFee.CalculateFaxingFee(TopUpAmount, true, false, exchangeRateObj.CountryRate1, Services.SEstimateFee.GetFaxingCommision(FaxingCountryCode));
                feeSummary = Services.SEstimateFee.CalculateFaxingFee(TopUpAmount, true, false, exchangeRate, Services.SEstimateFee.GetFaxingCommision(FaxingCountryCode));
                Common.FaxerSession.FaxingAmountSummary = feeSummary;
                vm.ExchangeRate = feeSummary.ExchangeRate;
                vm.Exchange = feeSummary.ExchangeRate.ToString();
                vm.TopUpFees = feeSummary.FaxingFee;
                vm.TopUpAmount = feeSummary.FaxingAmount;
                vm.ReceivingAmount = feeSummary.ReceivingAmount;
                vm.TotalAmountIncludingFees = feeSummary.FaxingFee + feeSummary.FaxingAmount;
             
            }
            ViewBag.ReceivingCountryCode = ReceivingCountryCode;
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = CalculateFaxingFeeVm.BindProperty)] CalculateFaxingFeeVm model)
        {
            if (model.TopUpAmount != 0)
            {



                if (!string.IsNullOrEmpty(Common.FaxerSession.TransactionSummaryUrl))
                {
                    return Redirect(Common.FaxerSession.TransactionSummaryUrl);
                }
                var FaxerCardPhoto = dbContext.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                if (FaxerCardPhoto.CardUrl == null && (model.TopUpAmount >= 1000))
                {

                    ViewBag.ToUrl = "/CalculateFaxingFees/Index";
                    Common.FaxerSession.ToUrl = "/CalculateFaxingFees/Index";
                    ModelState.AddModelError("CardURLError", "You are about to make a transfer of over {currency} 1000, to comply with anti-money laundering regulations, MoneyFex is required by law to ask for a copy of /n " +
                        "your Photo Identification Document (ID).Please upload a copy of your ID to proceed with this Transaction.");

                    return View(model);

                }
                return RedirectToAction("Index", "FraudAlert", new { FormURL = "/PaymentMethod/Index", BackUrl = "/CalculateFaxingFees/Index" });
            }
            else
            {
                ModelState.AddModelError("Error", "TopUp Amount is Required.");
            }
            return View();
        }

        public ActionResult TopUpPaymentMethod()
        {
            return View();
        }
    }
    public class CalculateFaxingFeeVm
    {

        public const string BindProperty = "TopUpAmount,TopUpFees,TotalAmountIncludingFees,ExchangeRate,Exchange," +
            "ReceivingAmount,IncludeFaxingFee,CurrencySymbol,Currency,SendingCurrency,SendingCurrencySymbol,ReceivingCurrency,ReceivingCurrencySymbol";
        public decimal TopUpAmount { get; set; }
        public decimal TopUpFees { get; set; }
        public decimal TotalAmountIncludingFees { get; set; }
        [Range(typeof(decimal), "0", "6")]
        public decimal ExchangeRate { get; set; }
        public string Exchange { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string IncludeFaxingFee { get; set; }
        public string CurrencySymbol { get; set; }
        public string Currency { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
    }
}