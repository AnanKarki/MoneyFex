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
    public class FormFaxerMerchantPaymentsController : Controller
    {
        FormFaxerMerchantPaymentsServices Service = new FormFaxerMerchantPaymentsServices();
        CommonServices CommonService = new CommonServices();
        // GET: Admin/FormFaxerMerchantPayments
        public ActionResult Index(string accountNumber = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (accountNumber == "transactionSuccess")
            {
                ViewBag.startMessage = "Payment completed successfully !";
                ViewBag.Value = 1;
                accountNumber = "";
            }
            ViewBag.Countries = new SelectList(CommonService.GetCountries(), "Code", "Name");
            AdminResult adminResult = new AdminResult();
            ViewBag.AdminResult = adminResult;
            var savedCards = new List<DropDownSavedCardsViewModel>();
            if (string.IsNullOrEmpty(accountNumber) == false)
            {

                var vm = Service.getDetails(accountNumber);
                if (vm != null)
                {
                    if (vm.FormFaxerDetails != null)
                    {
                        savedCards = Service.getSavedCards(vm.FormFaxerDetails.FaxerId);
                        ViewBag.savedCards = new SelectList(savedCards, "Id", "CardNameOnCard");

                        return View(vm);
                    }
                    else
                    {
                        ViewBag.startMessage = "Please enter a valid account Number!";
                        ViewBag.Value = 0;
                        accountNumber = "";
                    }
                }

            }

            ViewBag.savedCards = new SelectList(savedCards, "Id", "CardNameOnCard");
            FormFaxerMerchantPaymentsViewModel model = new FormFaxerMerchantPaymentsViewModel();
            model.isCardAvailable = false;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = FormFaxerMerchantPaymentsViewModel.BindProperty)]FormFaxerMerchantPaymentsViewModel model)
        {

            AdminResult adminResult = new AdminResult();
            ViewBag.AdminResult = adminResult;
            ViewBag.Countries = new SelectList(CommonService.GetCountries(), "Code", "Name");
            if (model != null)
            {
                var savedCards = new List<DropDownSavedCardsViewModel>();

                var vm = Service.getDetails(model.FaxerAccountNo);

                if (model.FormFaxerDetails.FaxerId > 0)
                {
                    savedCards = Service.getSavedCards(vm.FormFaxerDetails.FaxerId);
                }
                ViewBag.savedCards = new SelectList(savedCards, "Id", "CardNameOnCard");

                if (model.FormFaxerDetails.FaxerId == 0)
                {
                    ModelState.AddModelError("FaxerId", "Please provide Faxer Account Number first !");
                    adminResult.Message = "Please provide Faxer Account Number first !";
                    adminResult.Status = AdminResultStatus.Warning;
                    ViewBag.AdminResult = adminResult;
                    return View(model);
                }



                if (string.IsNullOrEmpty(model.FormBusinessDetails.BusinessAccountNo))
                {
                    ModelState.AddModelError("BusinessMobileNo", "Please provide the MFS Code first !");
                    adminResult.Message = "Please provide the MFS Code first !";
                    adminResult.Status = AdminResultStatus.Warning;
                    ViewBag.AdminResult = adminResult;
                    return View(model);
                }
                if (model.FormBusinessDetails.BusinessConfirm == false)
                {
                    ModelState.AddModelError("BusinessConfirm", "Please confirm  all the merchant details are correct before proceeding !");
                    adminResult.Message = "Please confirm buiness merchant  details are correct before proceeding !";
                    adminResult.Status = AdminResultStatus.Warning;
                    ViewBag.AdminResult = adminResult;
                    return View(model);
                }
                bool valid = true;
                if (model.FormPaymentDetails.PaymentTopUpAmount == 0)
                {
                    ModelState.AddModelError("PaymentTopUpAmount", "The PaymentTopUpAmount field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.PaymentReference))
                {
                    ModelState.AddModelError("PaymentReference", "The PaymentReference field can't be blank !");
                    valid = false;
                }
                if ((model.isCardAvailable == true && model.BankPayment == true) || (model.isCardAvailable == false && model.BankPayment == false))
                {
                    ModelState.AddModelError("BankPayment", "You can either choose Card Payment option or Bank Payment option. Not both !");
                    valid = false;
                }
                if (model.isCardAvailable == true)
                {
                    if (model.FormCreditCardDetails.CardAmount == 0)
                    {
                        ModelState.AddModelError("CardAmount", "The CardAmount field can't be blank !");
                        valid = false;
                    }
                    if (model.FormCreditCardDetails.CardAmount != model.FormPaymentDetails.PaymentAmountIncludingFee)
                    {
                        ModelState.AddModelError("CardAmount", "This amount should be equal to the Payment Top-Up Amount !");
                        valid = false;
                    }
                    if (string.IsNullOrEmpty(model.FormCreditCardDetails.CardNameOnCard))
                    {
                        ModelState.AddModelError("CardNameOnCard", "This Credit/Debit Name field can't be blank !");
                        valid = false;
                    }
                    if (string.IsNullOrEmpty(model.FormCreditCardDetails.CardNumber))
                    {
                        ModelState.AddModelError("CardNumber", "This Credit/Debit CardNumber field can't be blank !");
                        valid = false;
                    }
                    if (model.FormCreditCardDetails.CardEndMonth == 0)
                    {
                        ModelState.AddModelError("CardEndMonth", "This Credit/Debit CardEndMonth field can't be blank !");
                        valid = false;
                    }
                    if (model.FormCreditCardDetails.CardEndYear == 0)
                    {
                        ModelState.AddModelError("CardEndYear", "This Credit/Debit CardEndYear field can't be blank !");
                        valid = false;
                    }
                    if (model.FormCreditCardDetails.CardEndYear == int.Parse(DateTime.Now.ToString("yy")))
                    {
                        if (model.FormCreditCardDetails.CardEndMonth < int.Parse(DateTime.Now.ToString("MM")))
                        {
                            ModelState.AddModelError("CardEndMonth", "Please enter a valid Credit/Debit date !");
                            valid = false;

                        }
                    }
                    if (model.FormCreditCardDetails.CardEndYear < int.Parse(DateTime.Now.ToString("yy")))
                    {
                        ModelState.AddModelError("CardEndYear", " Please enter a valid Credit/Debit date !");
                        valid = false;
                    }
                    if (string.IsNullOrEmpty(model.FormCreditCardDetails.CardSecurityNo))
                    {
                        ModelState.AddModelError("CardSecurityNo", "This CardSecurityNo field can't be blank !");
                        valid = false;
                    }
                    if (string.IsNullOrEmpty(model.FormBillingDetails.BillingAddress1))
                    {
                        ModelState.AddModelError("BillingAddress1", "This BillingAddress1 field can't be blank !");
                        valid = false;
                    }
                    else if ((model.FormBillingDetails.BillingAddress1.ToLower()).Trim() != (model.FormFaxerDetails.FaxerAddress.ToLower()).Trim())
                    {
                        ModelState.AddModelError("BillingAddress1", "The BillingAddress1 must match with the faxer detail address ");
                        valid = false;
                    }
                    if (string.IsNullOrEmpty(model.FormBillingDetails.BillingCity))
                    {
                        ModelState.AddModelError("BillingCity", "This BillingCity field can't be blank !");
                        valid = false;
                    }
                    else if ((model.FormBillingDetails.BillingCity.ToLower()).Trim() != (model.FormFaxerDetails.FaxerCity.ToLower()).Trim())
                    {
                        ModelState.AddModelError("BillingCity", "The BillingCity field must match with the faxer city !");
                        valid = false;
                    }
                    if (string.IsNullOrEmpty(model.FormBillingDetails.BillingPostalCode))
                    {
                        ModelState.AddModelError("BillingPostalCode", "This BillingPostalCode field can't be blank !");
                        valid = false;
                    }
                    if (string.IsNullOrEmpty(model.FormBillingDetails.BillingCountry))
                    {
                        ModelState.AddModelError("BillingCountry", "This BillingCountry field can't be blank !");
                        valid = false;
                    }
                    else if ((model.FormBillingDetails.BillingCountry.ToLower()).Trim() != (model.FormFaxerDetails.FaxerCountryCode.ToLower()).Trim())
                    {
                        ModelState.AddModelError("BillingCountry", "The country must match with the faxer Country !");
                        valid = false;
                    }
                }
                if (valid == true)
                {
                    if (model.AcceptTerms == false)
                    {
                        ModelState.AddModelError("AcceptTerms", "You must accept the Terms And Conditions before proceeding !");
                        return View(model);
                    }


                    if (model.isCardAvailable == true)
                    {

                        #region  Strip portion
                        //StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");

                        StripeConfiguration.SetApiKey(System.Configuration.ConfigurationManager.AppSettings["StripeApiLiveKey"]);
                        var stripeTokenCreateOptions = new StripeTokenCreateOptions
                        {
                            Card = new StripeCreditCardOptions
                            {
                                Number = model.FormCreditCardDetails.CardNumber,
                                ExpirationMonth = model.FormCreditCardDetails.CardEndMonth,
                                ExpirationYear = model.FormCreditCardDetails.CardEndYear,
                                Cvc = model.FormCreditCardDetails.CardSecurityNo,
                                Name = model.FormCreditCardDetails.CardNameOnCard
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
                            return View(model);

                        }

                        var stripeTransaction = Common.FaxerStripe.CreateTransaction(
                            model.FormPaymentDetails.PaymentAmountIncludingFee
                            , Common.Common.GetCountryCurrency(model.FormFaxerDetails.FaxerCountryCode),
                            model.FormCreditCardDetails.CardNameOnCard,
                            Sourcetoken
                            );


                        if (string.IsNullOrEmpty(Sourcetoken))
                        {
                            var chargeOptions = new StripeChargeCreateOptions()
                            {
                                Amount = (int)model.FormPaymentDetails.PaymentAmountIncludingFee * 100,
                                Currency = Common.Common.GetCountryCurrency(model.FormFaxerDetails.FaxerCountryCode),
                                Description = "Charge for " + model.FormCreditCardDetails.CardNameOnCard,
                                //SourceTokenOrExistingSourceId = "tok_mastercard",// obtained with Stripe.js
                                SourceTokenOrExistingSourceId = Sourcetoken
                            };
                            var chargeService = new StripeChargeService();
                            StripeCharge charge = chargeService.Create(chargeOptions);


                        }
                    }
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

                    //sending to stripe
                    //string currency = Service.getCurrency(model.FormFaxerDetails.FaxerCC);
                    //StripeCharge charge = FaxerStripe.CreateTransaction((Int32)model.FormCreditCardDetails.CardAmount, currency, model.FormCreditCardDetails.CardNameOnCard);
                    //TODO : Stripe Success Validation
                    bool result = Service.SaveFaxerMerchantPayments(model);
                    if (result)
                    {
                        // Sms Function  
                      
                        return RedirectToAction("Index", new { @accountNumber = "transactionSuccess" });
                    }
                }

            }
            // To get first modelstate error and pass to admin result message object 
            // which has been used in admin layout page
            var modelErrors = new List<string>();
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

        public ActionResult getSavedCardInfo(int id)
        {
            var cardDetails = Service.getCardDetails(id);
            return Json(new DropDownSavedCardsViewModel()
            {
                CardNameOnCard = cardDetails.CardNameOnCard,
                CardNumber = cardDetails.CardNumber,
                CardEndMonth = cardDetails.CardEndMonth,
                CardEndYear = cardDetails.CardEndYear

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getMerchantInfo(string mfsCode)
        {
            var merchantDetails = Service.getBusinessDetails(mfsCode);



            if (merchantDetails == null)
            {

                return Json(new
                {
                    InvalidMerchant = true
                }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var CardStatus = Service.GetMerchantcardStatus(merchantDetails.KiiPayBusinessInformationId);
                string ErrorMessage = "";
                string StatusOfCard = "";
                switch (CardStatus)
                {
                    case null:
                        ErrorMessage = "Business Merchant do not have MFBC Card to accept payment!";
                        break;
                    case DB.CardStatus.Active:
                        break;
                    case DB.CardStatus.InActive:
                    case DB.CardStatus.IsDeleted:
                    case DB.CardStatus.IsRefunded:

                        if (CardStatus == DB.CardStatus.InActive)
                        {
                            StatusOfCard = "Deactivated";
                        }
                        else if (CardStatus == DB.CardStatus.IsDeleted)
                        {
                            StatusOfCard = "Deleted";
                        }
                        else if (CardStatus == DB.CardStatus.IsRefunded)
                        {

                            StatusOfCard = "Refunded";
                        }
                        ErrorMessage = "Business Merchant MFBC Card has been " + StatusOfCard + ", you cannot proceed your payment";
                        break;
                    default:
                        break;
                }
                if (CardStatus == DB.CardStatus.Active)
                {
                    return Json(new FormBusinessDetailsViewModel()
                    {
                        KiiPayBusinessInformationId = merchantDetails.KiiPayBusinessInformationId,
                        BusinessMobileNo = merchantDetails.BusinessMobileNo,
                        BusinessMerchantName = merchantDetails.BusinessMerchantName,
                        BusinessAccountNo = merchantDetails.BusinessAccountNo,
                        BusinessCity = merchantDetails.BusinessCity,
                        BusinessCountry = merchantDetails.BusinessCountry,
                        BusinessCC = merchantDetails.BusinessCC
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    return Json(new
                    {

                        InvalidCard = true,
                        ErrorMessage = ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult getFaxingCalc(decimal faxingAmount = 0, string faxingCC = "", string receivingCC = "", decimal receivingAmount = 0, bool isFaxingFeeIncluded = true)
        {

            var faxingSummary = Service.getFaxingCalculations(faxingAmount, faxingCC, receivingCC, receivingAmount, isFaxingFeeIncluded);
            return Json(new FormPaymentDetailsViewModel()
            {
                PaymentTopUpAmount = faxingSummary.TopUpAmount,
                PaymentFaxingFee = faxingSummary.TopUpFees,
                PaymentAmountIncludingFee = faxingSummary.TotalAmountIncludingFees,
                PaymentExchangeRate = faxingSummary.ExchangeRate,
                PaymentAmountToBeReceived = faxingSummary.ReceivingAmount,
                PaymentReceivingCountry = receivingCC,
                SenderCurrency = CommonService.getCurrency(faxingCC),
                SenderCurrencySymbol = CommonService.getCurrencySymbol(faxingCC),
                ReceiverCurrency = CommonService.getCurrency(receivingCC),
                ReceiverCurrencySymbol = CommonService.getCurrencySymbol(receivingCC)

            }, JsonRequestBehavior.AllowGet);
        }
    }
}