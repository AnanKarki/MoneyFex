using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class NonCardMoneyFaxController : Controller
    {
        // GET: NonCardMoneyFax
        private DB.FAXEREntities dbContext = null;
        private CommonServices CommonService = null;
        public NonCardMoneyFaxController()
        {
            dbContext = new DB.FAXEREntities();
            CommonService = new CommonServices();
        }
        public ActionResult Index(string recCountryCode = "")
        {
            if ((FaxerSession.LoggedUser == null))
            {
                FaxerSession.FromUrl = "/NonCardMoneyFax";
                return RedirectToAction("Login", "FaxerAccount");
            }

            ViewBag.ReceivingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");

            NonCardMoneyFaxViewModel model = new NonCardMoneyFaxViewModel();
            model.SendingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode);
            model.SendingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode);
            if (!string.IsNullOrEmpty(recCountryCode))
            {
                FaxerSession.ReceivingCountry = recCountryCode;
                model.ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(recCountryCode);
                model.ReceivingCurrencySymbol = CommonService.getCurrencySymbol(recCountryCode);
                model.ReceivingCountry = recCountryCode;
            }

            if (Common.FaxerSession.FaxingAmountSummary != null)
            {
                model.ReceivingCountry = Common.FaxerSession.ReceivingCountry;
                if (Common.FaxerSession.FaxingAmountSummary.IncludingFaxingFee == true)
                {
                    model.FaxingAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount;
                }
                else
                {
                    model.FaxingAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount;
                }
                model.IncludeFaxingFee = Common.FaxerSession.FaxingAmountSummary.IncludingFaxingFee;

                ModelState.AddModelError("CountryError", "Please note: Changing the receiver's Country may also change the receiving amount and fees.");

                return View(model);
            }
            else if (Common.FaxerSession.ReceivingCountry != null)
            {

                model.ReceivingCountry = Common.FaxerSession.ReceivingCountry;
                return View(model);

            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = NonCardMoneyFaxViewModel.BindProperty)] NonCardMoneyFaxViewModel model)
        {
            ViewBag.ReceivingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            var FaxerCardPhoto = dbContext.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            //If User has entered a receiving amount then we should
            // take receiving Country else sending country 
            string SendORReceivingCountry = model.ReceivingAmount > 0 ? model.ReceivingCountry : Common.FaxerSession.LoggedUser.CountryCode;

            if (model.ReceivingCountry == "" || model.ReceivingCountry == null)
            {
                ModelState.AddModelError("CountryError", "Receiving Country is Required.");
            }
            else if (model.FaxingAmount == 0 && model.ReceivingAmount == 0)
            {
                ModelState.AddModelError("Error", "Please enter an amount to proceed");
            }

            else if (string.IsNullOrEmpty(FaxerCardPhoto.CardUrl)
                && (Common.Common.IsValidAmountToTransfer(model.FaxingAmount, model.ReceivingAmount, SendORReceivingCountry) == false))
            {


                Common.FaxerSession.ReceivingCountry = model.ReceivingCountry;
                Common.FaxerSession.NonCardMoneyFaxViewModel = model;
                Common.FaxerSession.ToUrl = "/NonCardMoneyFax/NonCardFaxingSummary";
                ViewBag.ToUrl = "/NonCardMoneyFax/NonCardFaxingSummary";
                ModelState.AddModelError("CardURLError",
                    "You are about to make a transfer of over {currency} 1000," +
                    " to comply with anti-money laundering regulations, MoneyFex is required by law to ask for a copy of /n " +
                    "your Photo Identification Document (ID).Please upload a copy of your ID to proceed with this Transaction.");


            }
            else
            {
                if ((Common.FaxerSession.ReceivingCountry != null) && Common.FaxerSession.ReceivingCountry != model.ReceivingCountry)
                {
                    Session.Remove("TransactionSummaryUrl");
                }
                Common.FaxerSession.ReceivingCountry = model.ReceivingCountry;
                Common.FaxerSession.NonCardMoneyFaxViewModel = model;
                return RedirectToAction("NonCardFaxingSummary");
            }
            return View(model);
        }

        public ActionResult NonCardFaxingSummary()
        {
            var model = Common.FaxerSession.NonCardMoneyFaxViewModel;
            model.ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.ReceivingCountry);
            model.SendingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode);
            model.SendingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode);
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

                string FaxingCountryCode = FaxerSession.FaxerCountry;
                string ReceivingCountryCode = model.ReceivingCountry;
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

                    TempData["ExchangeRate"] = "We are yet to start operations to this country, please try again later!";
                    return RedirectToAction("Index", new { recCountryCode = ReceivingCountryCode });
                }
                //   exchangeRate = Convert.ToDouble(dbContext.ExchangeRates.Where(x => x.CountryCode1 == ReceivingCountryCode && x.CountryCode2==FaxingCountryCode).Select(x => x.CountryRate2).FirstOrDefault());
                if (model.ReceivingAmount > 0)
                {
                    model.FaxingAmount = model.ReceivingAmount;
                }
                //var feeSummary = Services.SEstimateFee.CalculateFaxingFee(model.FaxingAmount, model.IncludeFaxingFee == "Yes", model.ReceivingAmount > 0, exchangeRateObj.CountryRate1, exchangeRateObj.FaxingFee1 ?? 0);
                var feeSummary = Services.SEstimateFee.CalculateFaxingFee(model.FaxingAmount, model.IncludeFaxingFee, model.ReceivingAmount > 0, exchangeRate, Services.SEstimateFee.GetFaxingCommision(FaxingCountryCode));

                Common.FaxerSession.FaxingAmountSummary = feeSummary;
                EstimateFaxingFeeSummary vm = new EstimateFaxingFeeSummary();
                vm = Common.FaxerSession.FaxingAmountSummary;
                vm.FaxingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode);
                vm.FaxingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode);
                vm.ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.ReceivingCountry);
                vm.ReceivingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.ReceivingCountry);

                return View(vm);
            }
            return View();
        }
        [HttpPost]
        public ActionResult NonCardFaxingSummary([Bind(Include = EstimateFaxingFeeSummary.BindProperty)] EstimateFaxingFeeSummary model)
        {
            if (!string.IsNullOrEmpty(Common.FaxerSession.TransactionSummaryUrl))
            {

                return Redirect(Common.FaxerSession.TransactionSummaryUrl);
            }
            if (Common.FaxerSession.NonCardReceiverId > 0)
            {

                return RedirectToAction("NonCardReceiversDetails", new { id = Common.FaxerSession.NonCardReceiverId });
            }
            return RedirectToAction("NonCardReceiversDetails", 0);
        }



        [HttpGet]
        public ActionResult NonCardReceiversDetails(int id = 0)
        {

            string receivingCountryCode = FaxerSession.ReceivingCountry;
            ViewBag.RecvingCountry = dbContext.Country.Where(x => x.CountryCode.Equals(receivingCountryCode)).Select(x => x.CountryName).FirstOrDefault();

            var ReceiverList = (from c in dbContext.ReceiversDetails.Where(x => x.FaxerID == FaxerSession.LoggedUser.Id).ToList()
                                select new PreviousReceiversDropDown()
                                {
                                    Id = c.Id,
                                    FullName = c.FirstName + " " + c.MiddleName + " " + c.LastName

                                }).ToList();
            //ViewBag.PreviousReceivers = new SelectList(dbContext.ReceiversDetails.Where(x => x.FaxerID == FaxerSession.LoggedUser.Id).OrderBy(x => x.FirstName), "ID", "FirstName");
            ViewBag.PreviousReceivers = new SelectList(ReceiverList, "Id", "FullName");
            NonCardReceiversDetailsViewModel model = new NonCardReceiversDetailsViewModel();
            model.CountryPhoneCode = CommonService.getPhoneCodeFromCountry(receivingCountryCode);
            if ((Common.FaxerSession.NonCardReceiversDetails != null) && int.Parse(Common.FaxerSession.NonCardReceiversDetails.PreviousReceivers) == id)
            {

                model = Common.FaxerSession.NonCardReceiversDetails;
                return View(model);
            }
            if (id > 0)
            {
                Common.FaxerSession.NonCardReceiverId = id;
                var data = (from c in dbContext.ReceiversDetails.Where(x => x.Id == id).ToList()
                            select new NonCardReceiversDetailsViewModel()
                            {
                                ReceiverFirstName = c.FirstName,
                                ReceiverMiddleName = c.MiddleName,
                                ReceiverLastName = c.LastName,
                                ReceiverCity = c.City,
                                ReceiverCountry = c.Country,
                                ReceiverPhoneNumber = c.PhoneNumber,
                                ReceiverEmailAddress = c.EmailAddress,
                                CountryPhoneCode = Common.Common.GetCountryPhoneCode(c.Country),
                                PreviousReceivers = c.Id.ToString()
                            }).FirstOrDefault();
                if (data.ReceiverCountry != receivingCountryCode)
                {
                    ModelState.AddModelError("PreviousReceivers", "Receiver and Receiving countries do not match");
                    return View();
                }
                return View(data);
            }
            else
            {
                model.PreviousReceivers = "0";

            }
            return View(model);
        }
        [HttpPost]
        public ActionResult NonCardReceiversDetails([Bind(Include = Models.NonCardReceiversDetailsViewModel.BindProperty)] Models.NonCardReceiversDetailsViewModel model)
        {
            string receivingCountryCode = FaxerSession.ReceivingCountry;
            ViewBag.RecvingCountry = dbContext.Country.Where(x => x.CountryCode.Equals(receivingCountryCode)).Select(x => x.CountryName).FirstOrDefault();
            var ReceiverList = (from c in dbContext.ReceiversDetails.Where(x => x.FaxerID == FaxerSession.LoggedUser.Id).ToList()
                                select new PreviousReceiversDropDown()
                                {
                                    Id = c.Id,
                                    FullName = c.FirstName + " " + c.MiddleName + " " + c.LastName

                                }).ToList();
            //ViewBag.PreviousReceivers = new SelectList(dbContext.ReceiversDetails.Where(x => x.FaxerID == FaxerSession.LoggedUser.Id).OrderBy(x => x.FirstName), "ID", "FirstName");
            ViewBag.PreviousReceivers = new SelectList(ReceiverList, "Id", "FullName");


            if (ModelState.IsValid)
            {
                if (model.PreviousReceivers == null)
                {
                    model.PreviousReceivers = "0";
                    var EmailExist = dbContext.ReceiversDetails.Where(x => x.EmailAddress == model.ReceiverEmailAddress && x.FaxerID == FaxerSession.LoggedUser.Id).FirstOrDefault();
                    if (EmailExist != null)
                    {
                        ModelState.AddModelError("ReceiverEmailAddress", "  This Receiver's email address is already registered in the system, please either use a different email address or select Receiver's information from the list ");
                        model.PreviousReceivers = EmailExist.Id.ToString();
                        return View(model);

                    }
                }
                FaxerSession.NonCardReceiversDetails = model;

                return RedirectToAction("NonCardPaymentMethodDetails");
            }

            return View(model);
        }

        public ActionResult NonCardPaymentMethodDetails()
        {


            if (!string.IsNullOrEmpty(Common.FaxerSession.TransactionSummaryUrl))
            {
                return Redirect(Common.FaxerSession.TransactionSummaryUrl);
            }
            return RedirectToAction("Index", "FraudAlert", new { FormURL = "/FaxMoney/ChoosePaymentOption", BackUrl = "/NonCardMoneyFax/NonCardReceiversDetails?id=" + Common.FaxerSession.NonCardReceiverId });
            //return RedirectToAction("ChoosePaymentOption", "FaxMoney");
        }

    }

    public class PreviousReceiversDropDown
    {
        public int Id { get; set; }

        public string FullName { get; set; }
    }
}