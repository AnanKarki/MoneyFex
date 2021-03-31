using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Dashboard
{
    public class PayForGoodsAndServicesAbroadController : Controller
    {
        DB.FAXEREntities context = new DB.FAXEREntities();
        CommonServices CommmonService = new CommonServices();
        // GET: PayForGoodsAndServicesAbroad
        [HandleError]
        public ActionResult Index()
        {
            string loginFaxerCountryCode = "";
            if (FaxerSession.LoggedUser != null)
            {
                int FaxerId = FaxerSession.LoggedUser.Id;
                loginFaxerCountryCode = context.FaxerInformation.Where(x => x.Id == FaxerId).FirstOrDefault().Country;
                ViewBag.CountryCode = loginFaxerCountryCode;
            }
            else
            {

                var Url = Request.Url.ToString();
                string[] tokens = Url.Split('/');
                FaxerSession.FromUrl = "/" + tokens[3];
            }
            var countries = context.Country.OrderBy(x => x.CountryName).Select(c => new SelectListItem()
            {
                Text = c.CountryName,
                Value = c.CountryCode
            }).ToList();
            ViewBag.Faxing = countries;
            ViewBag.Receiving = new SelectList(context.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            EstimateFaxingFee model = new EstimateFaxingFee();
            model.Faxing = loginFaxerCountryCode;
            if (!string.IsNullOrEmpty(Common.FaxerSession.FaxingCountry) && !string.IsNullOrEmpty(Common.FaxerSession.ReceivingCountry))
            {
                model.Faxing = Common.FaxerSession.FaxingCountry;
                model.Receiving = Common.FaxerSession.ReceivingCountry;
            }
            return View(model);
        }
        [HttpPost]
        [HandleError]
        public ActionResult Index([Bind(Include = EstimateFaxingFee.BindProperty)]EstimateFaxingFee model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Common.FaxerSession.FaxingCountry = model.Faxing;
                    Common.FaxerSession.ReceivingCountry = model.Receiving;

                    return RedirectToAction("MerchantAccountNumber");
                }
                ViewBag.Faxing = new SelectList(context.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
                ViewBag.Receiving = new SelectList(context.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return View();
        }

        [HttpGet]
        public ActionResult PreviousPayee()

        {

            if (Common.FaxerSession.LoggedUser == null) {

                return RedirectToAction("Login", "FaxerAccount");
            }
            var data = (from c in context.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == Common.FaxerSession.LoggedUser.Id)
                        join d in context.KiiPayBusinessInformation on c.KiiPayBusinessInformationId equals d.Id
                        select d).ToList();

            ViewBag.PreviousPayee = new SelectList(data, "BusinessMobileNo", "BusinessName");
            PreviousPayeeViewModel vm = new PreviousPayeeViewModel();
            if (Common.FaxerSession.MerchantACNumber != null)
            {

                vm.BusinessMFCode = Common.FaxerSession.MerchantACNumber;

            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult PreviousPayee([Bind(Include = PreviousPayeeViewModel.BindProperty)]PreviousPayeeViewModel vm)
        {

            if (!string.IsNullOrEmpty(vm.BusinessMFCode))
            {
                Common.FaxerSession.PayGoodsAndServicesBackURL = Request.Url.ToString();
                return RedirectToAction("MerchantDetails", new { MerchantACNumber = vm.BusinessMFCode });
            }
            else
            {

                return RedirectToAction("MerchantAccountNumber");
            }
            return View();
        }

        public ActionResult MerchantAccountNumber()
        {
            if (FaxerSession.LoggedUser == null)
            {

                var Url = Request.Url.ToString();
                string[] tokens = Url.Split('/');
                if (FaxerSession.FromUrl == "")
                {
                    FaxerSession.FromUrl = "/" + tokens[3] + "/" + tokens[4];

                    return RedirectToAction("Login", "FaxerAccount");
                }
            }
            MerchantAccountNoViewModel vm = new MerchantAccountNoViewModel();
            if (!string.IsNullOrEmpty(FaxerSession.MerchantACNumber) && !string.IsNullOrEmpty(FaxerSession.BusinessName))
            {


                vm.BusinessName = FaxerSession.BusinessName;
                vm.BusinessMobileNo = FaxerSession.MerchantACNumber;
                ViewBag.AccountNo = FaxerSession.MerchantACNumber;
                ViewBag.ReceivingCountry = FaxerSession.ReceivingCountry;
            }

            ViewBag.Countries = new SelectList(context.Country.ToList(), "CountryCode", "CountryName");
            Common.FaxerSession.PayGoodsAndServicesBackURL = Request.Url.ToString();
            return View(vm);
        }
        public ActionResult MerchantDetails(string MerchantACNumber = "")
        {


            if (FaxerSession.LoggedUser == null)
            {
                var Url = Request.Url.ToString();
                string[] tokens = Url.Split('/');
                if (FaxerSession.FromUrl != "/PayForGoodsAndServicesAbroad")
                {
                    FaxerSession.FromUrl = "/" + tokens[3] + "/" + tokens[4];
                    return RedirectToAction("Login", "FaxerAccount");
                }
            }

            if (string.IsNullOrEmpty(MerchantACNumber))
            {
                TempData["MerchantACNumber"] = "  Invalid  Business Virtual Account Number, please enter a valid account number!";
                return RedirectToAction("MerchantAccountNumber");
            }
            Common.FaxerSession.MerchantACNumber = MerchantACNumber;
            MerchantDetailsViewModel vm = new MerchantDetailsViewModel();
            vm = (from c in context.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo == MerchantACNumber).ToList()
                  join d in context.Country on c.BusinessOperationCountryCode equals d.CountryCode
                  select new MerchantDetailsViewModel()
                  {
                      Id = c.Id,
                      NameOfMerchant = c.BusinessName,
                      MoneyFaxAccountNumber = c.BusinessMobileNo,
                      RegisteredCity = c.BusinessOperationCity,
                      RegisteredCountry = d.CountryName,
                      BusinessName = c.BusinessName,
                      Telephone = c.PhoneNumber,
                      PhoneCode = Common.Common.GetCountryPhoneCode(c.BusinessOperationCountryCode),
                      Email = c.Email,
                      Website = c.Website
                  }).FirstOrDefault();
            if (vm == null)
            {
                TempData["MerchantACNumber"] = "Invalid  Business Virtual Account Number, please enter a valid account number!";
                return RedirectToAction("MerchantAccountNumber");
            }
            if (vm != null)
            {
                var MerChantMFTCCardDetails = context.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == vm.Id).ToList().LastOrDefault();
                if (MerChantMFTCCardDetails == null)
                {
                    TempData["MerchantACNumber"] = "Business Merchant  Doesn't have Business E-Card";
                    return RedirectToAction("MerchantAccountNumber");
                }
                if (MerChantMFTCCardDetails != null)
                {
                    if (MerChantMFTCCardDetails.CardStatus == DB.CardStatus.InActive)
                    {
                        TempData["MerchantACNumber"] = "Business Merchant Business E-Card has been deactivated";
                        return RedirectToAction("MerchantAccountNumber");
                    }
                    if (MerChantMFTCCardDetails.CardStatus == DB.CardStatus.IsDeleted || MerChantMFTCCardDetails.CardStatus == DB.CardStatus.IsRefunded)
                    {
                        TempData["MerchantACNumber"] = "Business Merchant Business E-Card has been deleted";
                        return RedirectToAction("MerchantAccountNumber");
                    }

                }
            }
            FaxerSession.BusinessInformationId = vm.Id;
            FaxerSession.MerchantACNumber = MerchantACNumber;
            FaxerSession.BusinessName = vm.NameOfMerchant;

            string ReceivingCountry = context.KiiPayBusinessInformation.Where(x => x.Id == vm.Id).Select(x => x.BusinessOperationCountryCode).FirstOrDefault();
            if (Common.FaxerSession.LoggedUser != null)
            {
                Common.FaxerSession.FaxingCountry = Common.FaxerSession.LoggedUser.CountryCode;

            }
            Common.FaxerSession.ReceivingCountry = ReceivingCountry;
            return View(vm);
        }
        [HttpPost]
        public ActionResult MerchantDetails([Bind(Include = MerchantDetailsViewModel.BindProperty)]MerchantDetailsViewModel model)
        {
            if (model.Confirm)
            {
                if (!string.IsNullOrEmpty(Common.FaxerSession.TransactionSummaryUrl))
                {
                    return Redirect(Common.FaxerSession.TransactionSummaryUrl);
                }
                //TODO:
                return RedirectToAction("MerchantAmount");
            }
            else
            {
                ModelState.AddModelError("Confirm", "Please Confirm Before Continue.");
            }
            return View(model);
        }
        public ActionResult MerchantAmount()
        {
            PayForGoodsAndServicesAbroadEstimateFaxingAmountViewModel model = new PayForGoodsAndServicesAbroadEstimateFaxingAmountViewModel();
            model.FaxingCurrency = CommmonService.getCurrencyCodeFromCountry(FaxerSession.FaxingCountry);
            model.FaxingCurrencySymbol = CommmonService.getCurrencySymbol(FaxerSession.FaxingCountry);
            model.ReceivingCurrency = CommmonService.getCurrencyCodeFromCountry(FaxerSession.ReceivingCountry);
            model.ReceivingCurrencySymbol = CommmonService.getCurrencySymbol(FaxerSession.ReceivingCountry);
            if (Common.FaxerSession.FaxingAmountSummary != null)
            {
                model.FaxingAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount - Common.FaxerSession.FaxingAmountSummary.FaxingFee;
                model.PaymentReference = Common.FaxerSession.PaymentRefrence;

            }
            return View(model);
        }
        [HttpPost]
        public ActionResult MerchantAmount([Bind(Include = PayForGoodsAndServicesAbroadEstimateFaxingAmountViewModel.BindProperty)]FAXER.PORTAL.Models.PayForGoodsAndServicesAbroadEstimateFaxingAmountViewModel model)
        {
            var FaxerCardPhoto = context.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            //If User has entered a receiving amount then we should
            // take receiving Country else sending country 
            string SendORReceivingCountry = model.ReceivingAmount > 0 ? Common.FaxerSession.ReceivingCountry : Common.FaxerSession.LoggedUser.CountryCode;
            if (model.FaxingAmount == 0 && model.ReceivingAmount == 0)
            {
                ModelState.AddModelError("Error", "Please enter an amount to proceed");
            }
            else if (model.PaymentReference == null)
            {
                ModelState.AddModelError("PaymentReference", "Payment Reference is Required");
            }
            else if (string.IsNullOrEmpty(FaxerCardPhoto.CardUrl)
                && (Common.Common.IsValidAmountToTransfer(model.FaxingAmount, model.ReceivingAmount, SendORReceivingCountry) == false))
            {


                ViewBag.ToUrl = "/PayForGoodsAndServicesAbroad/MerchantAmount";
                Common.FaxerSession.ToUrl = "/PayForGoodsAndServicesAbroad/MerchantAmount";
                ModelState.AddModelError("CardURLError", "You are about to make a transfer of over {currency} 1000," +
                    " to comply with anti-money laundering regulations, " +
                    "MoneyFex is required by law to ask for a copy of /n " +
                    "your Photo Identification Document (ID).Please upload a copy of your ID to proceed with this Transaction.");


            }
            else
            {
                string FaxingCountryCode = Common.FaxerSession.FaxingCountry;
                string ReceivingCountryCode = Common.FaxerSession.ReceivingCountry;

                decimal exchangeRate = 0, faxingFee = 0;
                var exchangeRateObj = context.ExchangeRate.Where(x => x.CountryCode1 == FaxingCountryCode &&
                                                            x.CountryCode2 == ReceivingCountryCode).FirstOrDefault();
                if (exchangeRateObj == null)
                {
                    var exchangeRateobj2 = context.ExchangeRate.Where(x => x.CountryCode1 == ReceivingCountryCode
                                                                 && x.CountryCode2 == FaxingCountryCode).FirstOrDefault();
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
                    return View(model);
                }

                if (model.ReceivingAmount > 0)
                {
                    model.FaxingAmount = model.ReceivingAmount;
                }
                var feeSummary = Services.SEstimateFee.CalculateFaxingFee(model.FaxingAmount,
                                                                          model.IncludeFaxingFee,
                                                                          model.ReceivingAmount > 0,
                                                                          exchangeRate,
                                                                          Services.SEstimateFee.GetFaxingCommision(FaxingCountryCode));
                Common.FaxerSession.FaxingAmountSummary = feeSummary;
                Common.FaxerSession.PaymentRefrence = model.PaymentReference;
                return RedirectToAction("MerchantPayingDetails", new { model = model.PaymentReference });
            }
            return View();
        }
        public ActionResult MerchantPayingDetails(string model)
        {
            if (Common.FaxerSession.FaxingAmountSummary != null)
            {
                var faxingSummarry = Common.FaxerSession.FaxingAmountSummary;
                MerchantPayingDetailsViewModel vm = new MerchantPayingDetailsViewModel()
                {
                    PaymentFee = faxingSummarry.FaxingFee,
                    AmountToBeReceivedByMerchant = faxingSummarry.ReceivingAmount,
                    CurrentExchangeRate = faxingSummarry.ExchangeRate,
                    PayingAmount = faxingSummarry.FaxingAmount,
                    PaymentReference = model,
                    TotalAmountIncludingFee = faxingSummarry.TotalAmount,
                    FaxingCurrency = CommmonService.getCurrencyCodeFromCountry(FaxerSession.FaxingCountry),
                    FaxingCurrencySymbol = CommmonService.getCurrencySymbol(FaxerSession.FaxingCountry),
                    ReceivingCurrency = CommmonService.getCurrencyCodeFromCountry(FaxerSession.ReceivingCountry),
                    ReceivingCurrencySymbol = CommmonService.getCurrencySymbol(FaxerSession.ReceivingCountry)

                };
                return View(vm);
            }
            return RedirectToAction("MerchantDetails", Common.FaxerSession.MerchantACNumber);
        }
        [HttpPost]
        public ActionResult MerchantPayingDetails()
        {

            if (Common.FaxerSession.LoggedUser == null)
            {
                Common.FaxerSession.FromUrl = "/FraudAlert/Index?FormURL=/PayForGoodsAndServicesAbroad/MerchantPaymentMethod&BackUrl=/PayForGoodsAndServicesAbroad/MerchantPayingDetails?model=" + Common.FaxerSession.PaymentRefrence;

                return RedirectToAction("MerchantLoginMessage", "PayForGoodsAndServicesAbroad");

            }

            if (!string.IsNullOrEmpty(Common.FaxerSession.TransactionSummaryUrl))
            {
                return Redirect(Common.FaxerSession.TransactionSummaryUrl);
            }

            return RedirectToAction("Index", "FraudAlert", new { FormURL = "/PayForGoodsAndServicesAbroad/MerchantPaymentMethod", BackUrl = "/PayForGoodsAndServicesAbroad/MerchantPayingDetails?model=" + Common.FaxerSession.PaymentRefrence });
        }
        public ActionResult MerchantLoginMessage()
        {
            return View();
        }
        public ActionResult MerchantPaymentMethod()
        {
            if ((FaxerSession.LoggedUser == null))
            {
                FaxerSession.FromUrl = "/PayForGoodsAndServicesAbroad/MerchantPaymentMethod";
                return RedirectToAction("MerchantLoginMessage");
            }
            ViewBag.PaymentMethod = new SelectList(context.PaymentMethods.OrderBy(x => x.PaymentMethodCode), "PaymentMethodCode", "PaymentMethodName");
            return View();
        }
        [HttpPost]
        public ActionResult MerchantPaymentMethod([Bind(Include = PaymentMethodViewModel.BindProperty)]FAXER.PORTAL.Models.PaymentMethodViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    FaxerSession.PaymentMethod = model.PaymentMethod;
                    switch (model.PaymentMethod)
                    {
                        case "PM001"://crdr card
                            return RedirectToAction("MerchantCardPayment", "CardPayment");
                            break;
                        case "PM002"://saved crdrcard
                            return RedirectToAction("MerchantPaymentUsingSavedCreditDebitCard", "CardPayment");
                            break;//bank to bank
                        case "PM003":
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                ViewBag.PaymentMethod = new SelectList(context.PaymentMethods.OrderBy(x => x.PaymentMethodCode), "PaymentMethodCode", "PaymentMethodName");
                return View();
            }
            return View();
        }

        [HttpGet]
        public ActionResult MerchantPaymentUsingBankToBankPaymentMethod()
        {

            //Models.BankToBankTransferViewModel model = new Models.BankToBankTransferViewModel();
            //if (Common.FaxerSession.BankToBankTransfer != null)
            //{
            //    model = Common.FaxerSession.BankToBankTransfer;
            //}
            //model.SendingAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount;
            //model.SendingCurrency = CommmonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode);
            //model.SendingCurrencySymbol = CommmonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode);

            Models.BankToBankTransferViewModel model = new Models.BankToBankTransferViewModel();

            if (Common.FaxerSession.BankToBankTransfer != null)
            {

                model = Common.FaxerSession.BankToBankTransfer;
            }
            model.SendingCurrency = CommmonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode);
            model.SendingCurrencySymbol = CommmonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode);
            model.SendingAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount;
            model.SenderSurname = Common.Common.GetSenderLastName();
            var BankAccountInfo = Common.Common.GetBankAccountInfo(FaxerSession.LoggedUser.CountryCode);
            model.PaymentReference = model.SenderSurname + "-" + model.SendingAmount;
            if (BankAccountInfo == null)
            {

                @TempData["BankHasbeenSetuped"] = 0;
                return RedirectToAction("MerchantPaymentMethod");
            }
            if (BankAccountInfo != null)
            {
                model.AccountNumber = BankAccountInfo.AccountNo;
                model.LabelName = BankAccountInfo.LabelName + " :";
                model.LabelValue = BankAccountInfo.LabelValue;

            }
            return View(model);

        }

        public ActionResult MerchantPaymentUsingBankToBankPaymentMethod([Bind(Include = BankToBankTransferViewModel.BindProperty)]BankToBankTransferViewModel model)
        {


            if (ModelState.IsValid)
            {

                //var Country = context.Country.Where(x => x.CountryName.ToLower() == model.Country.ToLower()).FirstOrDefault();
                //if (Country == null)
                //{
                //    ModelState.AddModelError("Country", "Country Name Does not match");
                //    return View(model);
                //}
                //model.Country = Country.CountryCode;
                //var FaxerInformation = context.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                //if (model.Address1.ToLower() != FaxerInformation.Address1.ToLower())
                //{
                //    ModelState.AddModelError("Address1", "Address does not match");
                //    return View(model);

                //}
                //if (model.City.ToLower() != FaxerInformation.City.ToLower())
                //{
                //    ModelState.AddModelError("City", "City does not match");
                //    return View(model);
                //}
                //if (model.PostalCode != FaxerInformation.PostalCode)
                //{
                //    ModelState.AddModelError("PostalCode", "PostalCode does not match");
                //    return View(model);
                //}
                //if (model.Country.ToLower() != FaxerInformation.Country.ToLower())
                //{
                //    ModelState.AddModelError("Country", "country doesnot match");
                //    return View(model);
                //}
                if (model.Confirm == false)
                {
                    ModelState.AddModelError("Confirm", "Please confirm the payment ");
                    return View(model);

                }
                if (model.Accept == false)
                {
                    ModelState.AddModelError("Accept", "Please accept our terms and condition to make payment ");
                    return View(model);
                }
                Common.FaxerSession.BankToBankTransfer = model;

                ViewBag.ModelIsValid = 1;
                return View(model);
                //return RedirectToAction("ContactMoneyFex");
            }
            return View(model);




        }

        [HttpGet]
        public ActionResult MerchantPaymentUsingBankToBankDepositConfirmation()
        {

            return View();

        }

        [HttpGet]
        public ActionResult MerchantPaymentUsingBankToBankPaymentMethod_Yes()
        {


            var MerchantAccountNumber = context.KiiPayBusinessInformation.
                                        Where(x => x.Id == Common.FaxerSession.BusinessInformationId).
                                        FirstOrDefault().BusinessMobileNo;
            DB.BankToBankFaxerMerchantPayment FaxerMerchantPayment = new DB.BankToBankFaxerMerchantPayment()
            {

                FaxerId = Common.FaxerSession.LoggedUser.Id,
                KiiPayBusinessInformationId = Common.FaxerSession.BusinessInformationId,
                AmountPaid = Common.FaxerSession.FaxingAmountSummary.FaxingAmount,
                BusinessMerchantAccountNo = MerchantAccountNumber,
                PaymentRefernce = Common.FaxerSession.BankToBankTransfer.PaymentReference,
                TransactionDate = DateTime.Now,
                ExchangeRate = Common.FaxerSession.FaxingAmountSummary.ExchangeRate,
                FaxingFee = Common.FaxerSession.FaxingAmountSummary.FaxingFee,
                ReceivingAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount,
                TotalAmountImcludingFee = Common.FaxerSession.FaxingAmountSummary.TotalAmount

            };
            var obj = context.BankToBankFaxerMerchantPayment.Add(FaxerMerchantPayment);
            context.SaveChanges();


            var FaxerDetails = context.FaxerInformation.Where(x => x.Id == obj.FaxerId).FirstOrDefault();
            var MerchantDetails = context.KiiPayBusinessInformation.Where(x => x.Id == obj.KiiPayBusinessInformationId).FirstOrDefault();
            var FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string MerchantPaymentReference = Common.FaxerSession.PaymentRefrence;
            string body = "";
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BankToBankMerchantPayment?PayerName=" + FaxerDetails.FirstName
                                                      + " " + FaxerDetails.MiddleName + " " + FaxerDetails.LastName +
                                                      "&PayerAccountNo=" + FaxerDetails.AccountNo +
                                                      "&MerchantName=" + MerchantDetails.BusinessName
                                                      + "&MerchantAccountNo=" + obj.BusinessMerchantAccountNo +
                                                      "&Amount=" + obj.AmountPaid + " " + FaxerCurrency +
                                                      "&PaymentReference=" + MerchantPaymentReference + "&BankReference=" + obj.PaymentRefernce);


            mail.SendMail("noncardpayment@moneyfex.com", "Bank to bank transfer payment ", body);
            mail.SendMail(FaxerDetails.Email, "Bank to bank transfer payment ", body);




            return RedirectToAction("BankToBankPaymentSuccessful");
            //return View();
        }

        [HttpGet]
        public ActionResult MerchantPaymentUsingBankToBankPaymentMethod_No()
        {


            return RedirectToAction("MerchantPaymentUsingBankToBankPaymentMethod");
        }



        [HttpGet]
        public ActionResult ContactMoneyFex()
        {

            var KiiPayBusinessInformationId = Common.FaxerSession.BusinessInformationId;

            var BusinessAccountNo = context.KiiPayBusinessInformation.Where(x => x.Id == KiiPayBusinessInformationId).Select(x => x.BusinessMobileNo).FirstOrDefault();

            Models.ContactMoneyFexViewModel model = new Models.ContactMoneyFexViewModel();
            model.BusinessMerchantAccountNumber = BusinessAccountNo;
            model.MoneyFexEmail = "noncardfex@moneyfex.com";
            model.Subject = "Merchant Payment ";

            return View(model);
        }
        [HttpPost]
        public ActionResult ContactMoneyFex([Bind(Include = ContactMoneyFexViewModel.BindProperty)]ContactMoneyFexViewModel model)
        {
            if (string.IsNullOrEmpty(model.PaymentRefernce))
            {
                ModelState.AddModelError("PaymentRefernce", "Please enter a payment reference");
                return View(model);
            }
            DB.BankToBankFaxerMerchantPayment FaxerMerchantPayment = new DB.BankToBankFaxerMerchantPayment()
            {

                FaxerId = Common.FaxerSession.LoggedUser.Id,
                KiiPayBusinessInformationId = Common.FaxerSession.BusinessInformationId,
                AmountPaid = Common.FaxerSession.FaxingAmountSummary.FaxingAmount,
                BusinessMerchantAccountNo = model.BusinessMerchantAccountNumber,
                PaymentRefernce = model.PaymentRefernce,
                TransactionDate = DateTime.Now,
                ExchangeRate = Common.FaxerSession.FaxingAmountSummary.ExchangeRate,
                FaxingFee = Common.FaxerSession.FaxingAmountSummary.FaxingFee,
                ReceivingAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount,
                TotalAmountImcludingFee = Common.FaxerSession.FaxingAmountSummary.TotalAmount

            };
            var obj = context.BankToBankFaxerMerchantPayment.Add(FaxerMerchantPayment);
            context.SaveChanges();


            var FaxerDetails = context.FaxerInformation.Where(x => x.Id == obj.FaxerId).FirstOrDefault();
            var MerchantDetails = context.KiiPayBusinessInformation.Where(x => x.Id == obj.KiiPayBusinessInformationId).FirstOrDefault();
            var FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string MerchantPaymentReference = Common.FaxerSession.PaymentRefrence;
            string body = "";
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BankToBankMerchantPayment?PayerName=" + FaxerDetails.FirstName
                                                     + " " + FaxerDetails.MiddleName + " " + FaxerDetails.LastName +
                                                     "&PayerAccountNo=" + FaxerDetails.AccountNo + "&MerchantName=" + MerchantDetails.BusinessName 
                                                     + "&MerchantAccountNo=" + obj.BusinessMerchantAccountNo +
                                                     "&Amount=" + obj.AmountPaid + " " + FaxerCurrency + 
                                                     "&PaymentReference=" + MerchantPaymentReference + "&BankReference=" + obj.PaymentRefernce);


            mail.SendMail("noncardpayment@moneyfex.com", "Bank to bank transfer payment ", body);



            return RedirectToAction("BankToBankPaymentSuccessful");
        }

        public ActionResult BankToBankPaymentSuccessful()
        {

            return View();

        }

        public ActionResult getMerchants(string term, string Country)
        {
            return Json(context.KiiPayBusinessInformation.Where(x => x.BusinessName.StartsWith(term)
                                                     && x.BusinessOperationCountryCode.ToLower() == Country.ToLower()).
                                                     Select(a => new { label = a.BusinessName, id = a.BusinessMobileNo }),
                JsonRequestBehavior.AllowGet);
        }
    }
}