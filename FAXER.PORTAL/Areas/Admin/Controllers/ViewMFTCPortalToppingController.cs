using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Controllers;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewMFTCPortalToppingController : Controller
    {
        Services.ViewMFTCPortalToppingServices Service = new Services.ViewMFTCPortalToppingServices();
        CommonServices CommonService = new CommonServices();
        // GET: Admin/ViewMFTCPortalTopping
        [HttpGet]
        public ActionResult Index(string MFTC = "", int Id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (MFTC == "transactionSuccess")
            {
                ViewBag.TransactionMessage = "Transaction Completed Successfully !";
                MFTC = "";
            }
            else if (MFTC == "invalid")
            {
                ViewBag.TransactionMessage = "Invalid Card Number. Please Try Again !";
                MFTC = "";
            }

            ViewBag.Countries = new SelectList(CommonService.GetCountries(), "Code", "Name");
            ViewMFTCPortalToppingViewModel data = new ViewMFTCPortalToppingViewModel();
            if (MFTC != "" || Id != 0)
            {
                if (MFTC != "")
                {
                    bool check = Service.checkMFTCCard(MFTC);
                    if (check == false)
                    {
                        return RedirectToAction("Index", new { @MFTC = "invalid" });
                    }
                    data = Service.getFaxerAndCardUserInfo(MFTC);

                }
                else if (Id != 0)
                {
                    data = Service.getFaxerAndCardUserInfo(Service.MFTC(Id));

                }
                if (data != null)
                {
                    var MFTCCards = Service.getMFTCCardNumbersWithFormating(data.ViewMFTCPortalFaxer.FaxerId);

                    ViewBag.MFTCCards = new SelectList(MFTCCards, "Id", "MFTCCardNumber", data.ViewMFTCPortalCardUser.CardUserId);
                    FaxerSession.FaxerCountry = data.ViewMFTCPortalFaxer.FaxerCountryCode;
                    FaxerSession.MFTCCard = MFTC;
                    FaxerSession.MFTCCardInformationId = data.ViewMFTCPortalCardUser.CardUserId;
                    return View(data);
                }
            }

            var MFTCCardss = new List<DropDownMFTCCardsViewModel>();
            ViewBag.MFTCCards = new SelectList(MFTCCardss, "Id", "MFTCCardNumber");
            return View();
        }
        public ActionResult idToCard(int id)
        {

            var data = Service.MFTC(id);
            return RedirectToAction("Index", new { MFTC = data });


        }

        [HttpPost]
        public ActionResult Index([Bind(Include = ViewMFTCPortalToppingViewModel.BindProperty)]ViewMFTCPortalToppingViewModel model)
        {

            // Admin Result Status Object 
            AdminResult adminResult = new AdminResult();
            // The viewbag is used in Layout page 
            ViewBag.AdminResult = adminResult;
            ViewBag.Countries = new SelectList(CommonService.GetCountries(), "Code", "Name");
            if (model.ViewMFTCPortalFaxer.FaxerId != 0)
            {
                var MFTCCards = Service.getMFTCCardNumbersWithFormating(model.ViewMFTCPortalFaxer.FaxerId);
                ViewBag.MFTCCards = new SelectList(MFTCCards, "Id", "MFTCCardNumber");
            }
            if (model.ViewMFTCPortalFaxer.FaxerId == 0)
            {
                var MFTCCardss = Service.getMFTCCardNumbersWithFormating(0);
                ViewBag.MFTCCards = new SelectList(MFTCCardss, "Id", "MFTCCardNumber");
            }

           


            if ((model.isCardAvailable == true) && (model.bankToBankPayment == false))
            {
                if (model.ToppingPayment.FaxingAmount == 0)
                {
                    ModelState.AddModelError("FaxingAmount", "Paying Amount must be greater than 0 !");
                    return View(model);
                }
                if (model.ToppingPayment.NameOnCard == null)
                {
                    ModelState.AddModelError("NameOnCard", "Please enter your name on the Card");
                    return View(model);
                }
                if (model.ToppingPayment.CardNumber == null)
                {
                    ModelState.AddModelError("CardNumber", "Card Number can't be empty while Card is chosen !");
                    return View(model);
                }
                if (model.ToppingPayment.EndMonth == 0)
                {
                    ModelState.AddModelError("EndMonth", "Please enter the End Month !");
                    return View(model);
                }
                else if (model.ToppingPayment.EndYear == 0)
                {
                    ModelState.AddModelError("EndYear", "Please enter the End Year !");
                    return View(model);
                }
                else
                {
                    if ((model.ToppingPayment.EndYear) == int.Parse(DateTime.Now.ToString("yy")))
                    {
                        if ((model.ToppingPayment.EndMonth) < int.Parse(DateTime.Now.ToString("mm")))
                        {
                            ModelState.AddModelError("EndMonth", "Please enter Valid Date !");
                            return View(model);
                        }
                    }
                    else if ((model.ToppingPayment.EndYear) < int.Parse(DateTime.Now.ToString("yy")))
                    {
                        ModelState.AddModelError("EndYear", "Please enter the Valid Date !");
                        return View(model);
                    }
                }


                if (string.IsNullOrEmpty(model.ToppingPayment.SecurityCode))
                {
                    ModelState.AddModelError("SecurityCode", "Please enter the Security Code !");
                    return View(model);
                }


                bool valid = true;
                if (model.ToppingBillingAddress.Addressline1.ToLower() != model.ViewMFTCPortalFaxer.FaxerAddress.ToLower())
                {
                    ModelState.AddModelError("Addressline1", "Address line one does not match with your registered address,");
                    valid = false;
                }
                if (model.ToppingBillingAddress.City.ToLower() != model.ViewMFTCPortalFaxer.FaxerCity.ToLower())
                {
                    ModelState.AddModelError("City", "City Is Not Matched!!");
                    valid = false;
                }
                if (model.ToppingBillingAddress.ZipCode != model.ViewMFTCPortalFaxer.FaxerPostalCode)
                {
                    ModelState.AddModelError("ZipCode", "Postal Code is not matched !");
                    valid = false;
                }
                if (model.ToppingBillingAddress.Country.ToLower() != model.ViewMFTCPortalFaxer.FaxerCountryCode.ToLower())
                {
                    ModelState.AddModelError("Country", "Country is not matched !");
                    valid = false;
                }
                if (model.ToppingPayment.FaxingAmount != model.CalculateFaxingFee.TotalAmountIncludingFees)
                {
                    ModelState.AddModelError("FaxingAmount", "Paying Amount and Deposit Amount do not match !");
                    valid = false;
                }

                if (model.checkTermsAndConditions == false)
                {
                    ModelState.AddModelError("checkTermsAndConditions", "Please Confirm Before Continuing!");
                    valid = false;
                }
                if (valid == false)
                {
                    return View(model);
                }
            }

            
            if (((model.isCardAvailable == true) && (model.bankToBankPayment == true)) || ((model.isCardAvailable == false) && (model.bankToBankPayment == false)))
            {
                ModelState.AddModelError("bankToBankPayment", "You can either choose Credit/Debit Card or Bank to Bank Payment method. Not both of them !");
                return View(model);
            }



            if ((model.isCardAvailable == false) && (model.bankToBankPayment == true))
            {
                if (model.CalculateFaxingFee.TopUpAmount == 0)
                {
                    ModelState.AddModelError("TopUpAmount", "Please enter the amount to deposit ");
                    return View(model);
                }



            }

            //if (((model.isCardAvailable == true) && (model.bankToBankPayment == false)) || ((model.isCardAvailable == false) && (model.bankToBankPayment == true)))
            //{

            //}
            var country = Service.GetFaxerCountry(model.ViewMFTCPortalFaxer.FaxerCountryCode);
            if (country != null)
            {

                if ((model.isCardAvailable == true) && (model.bankToBankPayment == false))
                {
                    #region  Strip portion
                    // StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");
                    StripeConfiguration.SetApiKey(System.Configuration.ConfigurationManager.AppSettings["StripeApiLiveKey"]);

                    var stripeTokenCreateOptions = new StripeTokenCreateOptions
                    {
                        Card = new StripeCreditCardOptions
                        {
                            Number = model.ToppingPayment.CardNumber,
                            ExpirationMonth = model.ToppingPayment.EndMonth,
                            ExpirationYear = model.ToppingPayment.EndYear,
                            Cvc = model.ToppingPayment.SecurityCode,
                            Name = model.ToppingPayment.NameOnCard
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
                        adminResult.Message = ex.Message;
                        adminResult.Status = AdminResultStatus.Warning;
                        ViewBag.AdminResult = adminResult;
                        //ModelState.AddModelError("ErrorMessage", ex.Message);
                        return View(model);

                    }

                    var stripeTransaction = Common.FaxerStripe.CreateTransaction(
                        model.CalculateFaxingFee.TotalAmountIncludingFees
                        , Common.Common.GetCountryCurrency(model.ViewMFTCPortalFaxer.FaxerCountryCode),
                        model.ToppingPayment.NameOnCard,
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
                }

                var result = Service.saveTopUpData(model, FaxerSession.MFTCCardInformationId);
                if (result)
                {

                    adminResult.Message = "Payment Completed Successfully";
                    adminResult.Status = AdminResultStatus.OK;
                    ViewBag.AdminResult = adminResult;
                  

                    ModelState.Clear();
                    return View();
                    //return RedirectToAction("Index", new { @MFTC = "transactionSuccess" });
                }
            }

            else
            {
                ModelState.AddModelError("bankToBankPayment", "You can either choose Credit/Debit Card or Bank to Bank Payment method. Not both of them !");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult getFaxingSummary(decimal TopUpAmount = 0, int mFTCCardInformationId = 0)
        {
            FaxerSession.MFTCCardInformationId = mFTCCardInformationId;
            var faxingSummary = new CalculateFaxingFeeVm();

            faxingSummary = Service.getCalculateSummary(TopUpAmount, FaxerSession.FaxerCountry, FaxerSession.MFTCCardInformationId);

            if (faxingSummary == null)
            {
                return Json(new
                {
                    NoExchangeRateSet = true,
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new CalculateFaxingFeeVm
            {
                TopUpAmount = faxingSummary.TopUpAmount,
                TopUpFees = faxingSummary.TopUpFees,
                TotalAmountIncludingFees = faxingSummary.TotalAmountIncludingFees,
                ExchangeRate = faxingSummary.ExchangeRate,
                ReceivingAmount = faxingSummary.ReceivingAmount,
                CurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.FaxerCountry),
                Currency = CommonService.getCurrency(FaxerSession.FaxerCountry),
                ReceivingCurrency = faxingSummary.ReceivingCurrency,
                ReceivingCurrencySymbol = faxingSummary.ReceivingCurrencySymbol
            }, JsonRequestBehavior.AllowGet);
        }
    }
}