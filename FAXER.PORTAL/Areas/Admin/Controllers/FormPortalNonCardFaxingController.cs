using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class FormPortalNonCardFaxingController : Controller
    {
        CommonServices CommonService = new CommonServices();
        FormPortalNonCardFaxingServices Service = new FormPortalNonCardFaxingServices();
        FormPortalNonCardFaxingViewModel vm;
        // GET: Admin/FormPortalNonCardFaxing
        [HttpGet]
        public ActionResult Index(string accountNumber = "", string message = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (message == "success")
            {
                ViewBag.Message = "Transaction Completed Successfully !";
                message = "";
            }

            // Admin Result Status Object 
            AdminResult adminResult = new AdminResult();
            // The viewbag is used in Layout page 
            ViewBag.AdminResult = adminResult;
            var receivers = Service.getReceiversList(accountNumber);
            SetViewBagForContries();
            if (!string.IsNullOrEmpty(accountNumber))
            {
                SetViewBagForContries();
                var faxer = Service.getFaxerInfo(accountNumber);

                ViewBag.receivers = new SelectList(receivers, "ReceiverId", "ReceiverName");


                if (faxer == null) {
                    adminResult.Message = "Please enter a valid account no.";
                    adminResult.Status = AdminResultStatus.Warning;
                    ViewBag.AdminResult = adminResult;
                    return View();
                }
                return View(faxer);
            }
            ViewBag.receivers = new SelectList(receivers, "ReceiverId", "ReceiverName");
            var vm = new FormPortalNonCardFaxingViewModel();
            vm.RecCurrency = "";
            vm.RecCurSymbol = "";
            vm.SendingCurrency = "";
            vm.SendingCurSymbol = "";
            vm.CurValue = "";

            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = FormPortalNonCardFaxingViewModel.BindProperty)]FormPortalNonCardFaxingViewModel model)
        {
            // Admin Result Status Object 
            AdminResult adminResult = new AdminResult();
            // The viewbag is used in Layout page 
            ViewBag.AdminResult = adminResult;
            // To get first modelstate error and pass to admin result message object 
            var modelErrors = new List<string>();
            if (model != null)
            {
                bool valid = true;
                if (model.PortalNonCardFaxerDetails.faxerFirstName == null)
                {
                    ModelState.AddModelError("faxerFirstName", "Please make the Faxer's Details available !");
                    valid = false;
                }
                else if (model.PortalNonCardReceiverDetails.receiverFirstName == null)
                {
                    ModelState.AddModelError("receiverFirstName", "This receiverFirstName field can't be blank !");
                    valid = false;
                }
                else if (model.PortalNonCardReceiverDetails.receiverLastName == null)
                {
                    ModelState.AddModelError("receiverLastName", "This receiverLastName field can't be blank !");
                    valid = false;
                }

                else if (model.PortalNonCardReceiverDetails.receiverCity == null)
                {
                    ModelState.AddModelError("receiverCity", "This receiverCity field can't be blank !");
                    valid = false;
                }
                else if (model.PortalNonCardReceiverDetails.receiverCountry == null)
                {
                    ModelState.AddModelError("receiverCountry", "This receiverCountry field can't be blank !");
                    valid = false;
                }
                else if (model.PortalNonCardReceiverDetails.receiverTelephone == null)
                {
                    ModelState.AddModelError("receiverTelephone", "This receiverTelephone field can't be blank !");
                    valid = false;
                }
                //else if (model.PortalNonCardReceiverDetails.receiverEmailAddress == null)
                //{
                //    ModelState.AddModelError("receiverEmailAddress", "This receiverEmailAddress field can't be blank !");
                //    valid = false;
                //}
                else if (model.PortalNonCardFaxingCalculations.faxingAmount == 0)
                {
                    ModelState.AddModelError("faxingAmount", "Please enter the faxing amount !");
                    valid = false;
                }
                else if (model.PortalNonCardFaxingCalculations.receivingAmount == 0)
                {
                    ModelState.AddModelError("receivingAmount", "Please make sure this receivingAmount field doesn't remain blank !");
                    valid = false;
                }
                else if (model.isCardAvailable == true)
                {
                    if (model.PortalNonCardFaxingPaymentDetails.faxingAmount == 0)
                    {
                        ModelState.AddModelError("faxingAmount", "This faxingAmount field can't be blank !");
                        valid = false;
                    }
                    else if (model.PortalNonCardFaxingPaymentDetails.faxingAmount != model.PortalNonCardFaxingCalculations.totalAmount)
                    {
                        ModelState.AddModelError("faxingAmount", "The Faxing Amount in the Faxing Calculations section and Payment section should match !");
                        valid = false;
                    }
                    else if (model.PortalNonCardFaxingPaymentDetails.nameOnCard == null)
                    {
                        ModelState.AddModelError("nameOnCard", "This nameOnCard field can't be blank !");
                        valid = false;
                    }
                    else if (model.PortalNonCardFaxingPaymentDetails.cardNumber == null)
                    {
                        ModelState.AddModelError("cardNumber", "This cardNumber field can't be blank !");
                        valid = false;
                    }
                    else if (model.PortalNonCardFaxingPaymentDetails.endMonth == 0)
                    {
                        ModelState.AddModelError("endMonth", "This endMonth field can't be blank !");
                        valid = false;
                    }
                    else if (model.PortalNonCardFaxingPaymentDetails.endYear == 0)
                    {
                        ModelState.AddModelError("endYear", "This endYear field can't be blank !");
                        valid = false;
                    }
                    else if (string.IsNullOrEmpty(model.PortalNonCardFaxingPaymentDetails.securityCode))
                    {
                        ModelState.AddModelError("securityCode", "This securityCode field can't be blank !");
                        valid = false;
                    }
                    else if (model.PortalNonCardFaxingPaymentDetails.address1 == null)
                    {
                        ModelState.AddModelError("address1", "This address1 field can't be blank !");
                        valid = false;
                    }
                    else if (model.PortalNonCardFaxingPaymentDetails.address1.Trim().ToLower() != model.PortalNonCardFaxerDetails.faxerAddress.Trim().ToLower())
                    {
                        ModelState.AddModelError("address1", "The Address in the Faxer's Details section and Payment section must match !");
                        valid = false;
                    }
                    else if (model.PortalNonCardFaxingPaymentDetails.city == null)
                    {
                        ModelState.AddModelError("city", "This city field can't be blank !");
                        valid = false;
                    }
                    else if (model.PortalNonCardFaxingPaymentDetails.city.Trim().ToLower() != model.PortalNonCardFaxerDetails.faxerCity.Trim().ToLower())
                    {
                        ModelState.AddModelError("city", "The city in the Faxer's Details section and Payment section must match !");
                        valid = false;
                    }
                    else if (model.PortalNonCardFaxingPaymentDetails.postalCode == null)
                    {
                        ModelState.AddModelError("postalCode", "This postalCode can't be blank !");
                        valid = false;
                    }
                    else if (model.PortalNonCardFaxingPaymentDetails.postalCode.Trim().ToLower() != model.PortalNonCardFaxerDetails.faxerPostalCode.Trim().ToLower())
                    {
                        ModelState.AddModelError("postalCode", "The Faxer's postal code doesn't match in the database !");
                        valid = false;
                    }
                    else if (model.PortalNonCardFaxingPaymentDetails.country == null)
                    {
                        ModelState.AddModelError("country", "country field can't be blank !");
                        valid = false;
                    }
                    else if (model.PortalNonCardFaxingPaymentDetails.country.Trim().ToLower() != model.PortalNonCardFaxerDetails.faxerCountryCode.Trim().ToLower())
                    {
                        ModelState.AddModelError("country", "country in the Faxer's Details section and Payment section must match !");
                        valid = false;
                    }

                    else if (model.checkTermsAndConditions == false)
                    {
                        ModelState.AddModelError("checkTermsAndConditions", "Please make sure you have agreed to accept our terms and conditions !");
                        valid = false;
                    }
                }

                else if (((model.isBankToBankPayment == true) && (model.isCardAvailable == true)) || ((model.isBankToBankPayment == false) && (model.isCardAvailable == false)))
                {
                    ModelState.AddModelError("isBankToBankPayment", "You can either choose Bank to Bank Payment or Card Payment. Not both of them !");
                    valid = false;
                }
                if (valid == false)
                {

                    // To get first modelstate error and pass to admin result message object 
                    // which has been used in admin layout page

                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var modelError in modelState.Errors)
                        {
                            modelErrors.Add(modelError.ErrorMessage);
                        }
                    }

                    adminResult.Message = modelErrors.FirstOrDefault();
                    adminResult.Status = AdminResultStatus.Warning;
                    ViewBag.AdminResult = adminResult;
                    SetViewBagForContries();
                    var receivers = Service.getReceiversList(model.faxerAccountNo);
                    ViewBag.receivers = new SelectList(receivers, "ReceiverId", "ReceiverName");
                    return View(model);
                }
                else if (valid == true)
                {

                    // If user is paying amount using credit/debit card then stripe is used
                    if (model.isCardAvailable == true)
                    {
                        #region  Strip portion
                        //  StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");
                        StripeConfiguration.SetApiKey(System.Configuration.ConfigurationManager.AppSettings["StripeApiLiveKey"]);

                        var stripeTokenCreateOptions = new StripeTokenCreateOptions
                        {
                            Card = new StripeCreditCardOptions
                            {
                                Number = model.PortalNonCardFaxingPaymentDetails.cardNumber,
                                ExpirationMonth = model.PortalNonCardFaxingPaymentDetails.endMonth,
                                ExpirationYear = model.PortalNonCardFaxingPaymentDetails.endYear,
                                Cvc = model.PortalNonCardFaxingPaymentDetails.securityCode,
                                Name = model.PortalNonCardFaxingPaymentDetails.nameOnCard
                            }
                        };

                        string Sourcetoken = "";

                        try
                        {
                            var stripeTokenService = Common.FaxerStripe.GetStripeToken(stripeTokenCreateOptions);
                            Sourcetoken = stripeTokenService.Id;


                        }
                        catch (Exception ex)
                        {


                            ModelState.AddModelError("ErrorMessage", ex.Message);
                            adminResult.Status = AdminResultStatus.Warning;
                            adminResult.Message = ex.Message;
                            ViewBag.AdminResult = adminResult;
                            SetViewBagForContries();
                            var receivers = Service.getReceiversList(model.faxerAccountNo);
                            ViewBag.receivers = new SelectList(receivers, "ReceiverId", "ReceiverName");
                            return View(model);

                        }

                        var stripeTransaction = Common.FaxerStripe.CreateTransaction(
                            model.PortalNonCardFaxingCalculations.totalAmount
                            , Common.Common.GetCountryCurrency(model.PortalNonCardFaxerDetails.faxerCountryCode),
                            model.PortalNonCardFaxingPaymentDetails.nameOnCard,
                            Sourcetoken
                            );


                        // StripeResponse stripeResponse = new StripeResponse();
                        //try
                        //{
                        //    var stripeToken = tokenService.Create(stripeTokenCreateOptions);

                        //    Sourcetoken = stripeToken.Id;
                        //}
                        //catch (Exception ex)
                        //{

                        //    ModelState.AddModelError("ErrorMessage", ex.Message);

                        //    return View(model);
                        //    //Log.Write("Merchant Auto Payment Exception : " + ex.Message);
                        //}
                        #endregion

                        model.StripeChargeId = stripeTransaction.Id;
                    }
                    var result = Service.saveNonCardFaxingDetails(model);
                    if (result)
                    {

                        adminResult.Message = "Payment Completed Successfully";
                        adminResult.Status = AdminResultStatus.OK;
                        ViewBag.AdminResult = adminResult;

                        ModelState.Clear();
                        SetViewBagForContries();
                        var receivers = Service.getReceiversList("00");
                        ViewBag.receivers = new SelectList(receivers, "ReceiverId", "ReceiverName");

                       



                        return View();
                        //return RedirectToAction("Index", new { @message = "success" });
                    }
                }

            }
            SetViewBagForContries();

            // To get first modelstate error and pass to admin result message object 
            // which has been used in admin layout page

            foreach (var modelState in ModelState.Values)
            {
                foreach (var modelError in modelState.Errors)
                {
                    modelErrors.Add(modelError.ErrorMessage);
                }
            }

            adminResult.Message = modelErrors.FirstOrDefault();
            adminResult.Status = AdminResultStatus.Warning;
            ViewBag.AdminResult = adminResult;

            return View(model);
        }

        public ActionResult getFaxingCalc(decimal faxingAmount = 0, string faxingCC = "", string receivingCC = "", decimal receivingAmount = 0)
        {
            if (receivingCC == "")
            {
                return RedirectToAction("Index");
            }
            var faxingSummary = Service.getFaxingCalculations(faxingAmount, faxingCC, receivingCC, receivingAmount);
            return Json(new FormPortalNonCardFaxingCalculationsViewModel()
            {
                faxingAmount = faxingSummary.TopUpAmount,
                faxingFee = faxingSummary.TopUpFees,
                totalAmount = faxingSummary.TotalAmountIncludingFees,
                exchangeRate = faxingSummary.ExchangeRate,
                receivingAmount = faxingSummary.ReceivingAmount,
                receivingCountry = receivingCC,
                receivingCurrency = faxingSummary.ReceivingCurrency,
                receivingCurrencySymbol = faxingSummary.ReceivingCurrencySymbol,
                sendingCurrency = faxingSummary.SendingCurrency,
                sendingCurrencySymbol = faxingSummary.SendingCurrencySymbol
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getReceiverDetails(int recId)
        {
            var recDetails = Service.getReceiverDetails(recId) ?? new FormPortalNonCardReceiverDetailsViewModel();
            return Json(new FormPortalNonCardReceiverDetailsViewModel()
            {
                receiverId = recDetails.receiverId,
                receiverFirstName = recDetails.receiverFirstName,
                receiverMiddleName = recDetails.receiverMiddleName,
                receiverLastName = recDetails.receiverLastName,
                receiverCity = recDetails.receiverCity,
                receiverCountry = recDetails.receiverCountry,
                receiverTelephone = recDetails.receiverTelephone,
                receiverEmailAddress = recDetails.receiverEmailAddress,
                receiversCurrency = recDetails.receiversCurrency,
                receiversCurSymbol = recDetails.receiversCurSymbol
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getExchangeDetails(string sendingCC, string receivingCC)
        {
            string[] exchangeDetails = Service.exchangeRateDetails(sendingCC, receivingCC);
            if (exchangeDetails == null)
            {

                ModelState.AddModelError("Error", "Exchange Rate has not been setup for this country");
                return Json(new
                {
                    CurValue = "",
                    RecCurrency = "",
                    RecCurSymbol = ""

                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                CurValue = exchangeDetails[0],
                RecCurrency = exchangeDetails[1],
                RecCurSymbol = exchangeDetails[2] + "  " + exchangeDetails[0]
            }, JsonRequestBehavior.AllowGet);
        }



        private void SetViewBagForContries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
    }
}