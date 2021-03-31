using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class FaxMoneyController : Controller
    {
        private DB.FAXEREntities dbContext = null;
        private CommonServices CommonService = null;

        public FaxMoneyController()
        {
            dbContext = new FAXEREntities();
            CommonService = new CommonServices();
        }
        // GET: FaxMoney

        public ActionResult Index()
        {
            Session.Remove("ReceivingCountry");
            Session.Remove("NonCardReceiverId");

            Common.FaxerSession.BackButtonURL = Request.Url.ToString();

            Common.FaxerSession.BackButtonURLMyMoneyFex = Request.Url.ToString();
            return View();
        }
        public ActionResult DoesReceiverHaveACard()
        {
            Session.Remove("ReceivingCountry");
            Session.Remove("NonCardReceiverId");
            return View();
        }
        public ActionResult ChoosePaymentOption()
        {
            if (Common.FaxerSession.LoggedUser == null)
            {
                return RedirectToAction("Login", "faxerAccount");
            }
            ViewBag.PaymentMethod = new SelectList(dbContext.PaymentMethods.OrderBy(x => x.PaymentMethodCode), "PaymentMethodCode", "PaymentMethodName");
            return View();

        }
        [HttpPost]
        public ActionResult ChoosePaymentOption([Bind(Include = Models.PaymentMethodViewModel.BindProperty)] FAXER.PORTAL.Models.PaymentMethodViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    FaxerSession.PaymentMethod = model.PaymentMethod;
                    switch (model.PaymentMethod)
                    {
                        case "PM001"://crdr card
                            return RedirectToAction("NonCardReciver", "CardPayment");
                            break;
                        case "PM002"://saved crdrcard
                            return RedirectToAction("NonCardReceiverTransferUsingSavedCreditDebitCard", "CardPayment");
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
                ViewBag.PaymentMethod = new SelectList(dbContext.PaymentMethods.OrderBy(x => x.PaymentMethodCode), "PaymentMethodCode", "PaymentMethodName");
                return View();
            }
            return View();
        }
        public ActionResult UsingCreditDebitCard()
        {
            return View();
        }
        public ActionResult UsingSavedCreditDebitCard()
        {
            return View();
        }




        [HttpGet]
        public ActionResult UsingBankToBank()
        {

            Models.BankToBankTransferViewModel model = new Models.BankToBankTransferViewModel();

            if (Common.FaxerSession.BankToBankTransfer != null)
            {

                model = Common.FaxerSession.BankToBankTransfer;
            }
            model.SendingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode);
            model.SendingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode);
            model.SendingAmount = Common.FaxerSession.FaxingAmountSummary.TotalAmount;
            model.SenderSurname = Common.Common.GetSenderLastName();
            var BankAccountInfo = Common.Common.GetBankAccountInfo(FaxerSession.LoggedUser.CountryCode);
            model.PaymentReference = model.SenderSurname + "-" + model.SendingAmount;
            if (BankAccountInfo == null)
            {

                @TempData["BankHasbeenSetuped"] = 0;
                return RedirectToAction("ChoosePaymentOption");
            }
            if (BankAccountInfo != null)
            {
                model.AccountNumber = BankAccountInfo.AccountNo;
                model.LabelName = BankAccountInfo.LabelName + ":";
                model.LabelValue = BankAccountInfo.LabelValue;

            }
            return View(model);
        }
        [HttpPost]
        public ActionResult UsingBankToBank([Bind(Include = Models.BankToBankTransferViewModel.BindProperty)] Models.BankToBankTransferViewModel model)
        {
            if (ModelState.IsValid)
            {

                //var Country = dbContext.Country.Where(x => x.CountryName.ToLower() == model.Country.ToLower()).FirstOrDefault();
                //if (Country == null)
                //{
                //    ModelState.AddModelError("Country", "Country Name Does not match");
                //    return View(model);
                //}
                //model.Country = Country.CountryCode;
                //var FaxerInformation = dbContext.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
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
                    ModelState.AddModelError("Accept", "Please accept the terms and condition to make Payment ");
                    return View(model);

                }
                //if (int.Parse(Common.FaxerSession.NonCardReceiversDetails.PreviousReceivers) == 0)
                //{
                //    DB.ReceiversDetails receiversDetails = new DB.ReceiversDetails()
                //    {

                //        FirstName = Common.FaxerSession.NonCardReceiversDetails.ReceiverFirstName,
                //        MiddleName = Common.FaxerSession.NonCardReceiversDetails.ReceiverMiddleName,
                //        LastName = Common.FaxerSession.NonCardReceiversDetails.ReceiverLastName,
                //        City = Common.FaxerSession.NonCardReceiversDetails.ReceiverCity,
                //        Country = Common.FaxerSession.ReceivingCountry,
                //        EmailAddress = Common.FaxerSession.NonCardReceiversDetails.ReceiverEmailAddress,
                //        FaxerID = Common.FaxerSession.LoggedUser.Id,
                //        PhoneNumber = Common.FaxerSession.NonCardReceiversDetails.ReceiverPhoneNumber,
                //        CreatedDate = DateTime.Now
                //    };
                //    dbContext.ReceiversDetails.Add(receiversDetails);
                //    dbContext.SaveChanges();
                //    Common.FaxerSession.NonCardReceiverId = receiversDetails.Id;

                //}
                //else
                //{
                //    Common.FaxerSession.NonCardReceiverId = int.Parse(Common.FaxerSession.NonCardReceiversDetails.PreviousReceivers);

                //}
                Common.FaxerSession.BankToBankTransfer = model;

                //return RedirectToAction("ContactMoneyFex");
                ViewBag.ModelIsValid = 1;
                return View(model);
                //return RedirectToAction("BankToBankDepositConfirmation");
            }
            return View(model);

        }

        [HttpGet]
        public ActionResult BankToBankDepositConfirmation()
        {

            return View();
        }

        [PreventSpam]
        public ActionResult BankToBankDepositConfirmation_Yes()
        {

            if (int.Parse(Common.FaxerSession.NonCardReceiversDetails.PreviousReceivers) == 0)
            {
                DB.ReceiversDetails receiversDetails = new DB.ReceiversDetails()
                {

                    FirstName = Common.FaxerSession.NonCardReceiversDetails.ReceiverFirstName,
                    MiddleName = Common.FaxerSession.NonCardReceiversDetails.ReceiverMiddleName,
                    LastName = Common.FaxerSession.NonCardReceiversDetails.ReceiverLastName,
                    City = Common.FaxerSession.NonCardReceiversDetails.ReceiverCity,
                    Country = Common.FaxerSession.ReceivingCountry,
                    EmailAddress = Common.FaxerSession.NonCardReceiversDetails.ReceiverEmailAddress,
                    FaxerID = Common.FaxerSession.LoggedUser.Id,
                    PhoneNumber = Common.FaxerSession.NonCardReceiversDetails.ReceiverPhoneNumber,
                    CreatedDate = DateTime.Now
                };
                dbContext.ReceiversDetails.Add(receiversDetails);
                dbContext.SaveChanges();

                City newCity = new City()
                {
                    CountryCode = receiversDetails.Country,
                    Module = Module.Faxer,
                    Name = receiversDetails.City
                };
                SCity.Save(newCity);
                Common.FaxerSession.NonCardReceiverId = receiversDetails.Id;

            }
            else
            {
                Common.FaxerSession.NonCardReceiverId = int.Parse(Common.FaxerSession.NonCardReceiversDetails.PreviousReceivers);

            }
            DB.BankToBankFaxingNonCardTransaction faxingNonCardTransaction = new DB.BankToBankFaxingNonCardTransaction()
            {

                FaxerId = Common.FaxerSession.LoggedUser.Id,
                ReceiverId = Common.FaxerSession.NonCardReceiverId,
                AmountSent = Common.FaxerSession.FaxingAmountSummary.FaxingAmount,
                ExchangeRate = Common.FaxerSession.FaxingAmountSummary.ExchangeRate,
                FaxingFee = Common.FaxerSession.FaxingAmountSummary.FaxingFee,
                ReceivingAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount,
                TotalAmountImcludingFee = Common.FaxerSession.FaxingAmountSummary.TotalAmount,
                PaymentReference = Common.FaxerSession.BankToBankTransfer.PaymentReference,
                TransactionDate = DateTime.Now
            };



            var obj = dbContext.BankToBankFaxingNonCardTransaction.Add(faxingNonCardTransaction);
            dbContext.SaveChanges();
            var ReceiverDetials = dbContext.ReceiversDetails.Where(x => x.Id == obj.ReceiverId).FirstOrDefault();
            var FaxerDetails = dbContext.FaxerInformation.Where(x => x.Id == obj.FaxerId).FirstOrDefault();

            var ReceiverCountry = Common.Common.GetCountryName(ReceiverDetials.Country);
            var FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string body = "";

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BankToBankNonCardTransfer?FaxerName=" + FaxerDetails.FirstName + " " + FaxerDetails.MiddleName + " " + FaxerDetails.LastName
                + "&FaxerAccountNo=" + FaxerDetails.AccountNo + "&ReceiverFullName=" + ReceiverDetials.FirstName +
                " " + ReceiverDetials.MiddleName + " " + ReceiverDetials.LastName + "&ReceiverCountry=" + ReceiverCountry +
                "&ReceiverEmail=" + ReceiverDetials.EmailAddress +
                "&ReceiverTelephone=" + ReceiverDetials.PhoneNumber + "&ReceiverCity=" + ReceiverDetials.City + "&Amount=" + obj.AmountSent + " " + FaxerCurrency
                + "&BankReference=" + obj.PaymentReference);

            //mail.SendMail("noncardfex@moneyfex.com", "Bank to Bank  Non Card MoneyFex", body);
            //mail.SendMail("moneyfaxer@gmail.com", "Bank to bank transfer ", body);
            mail.SendMail("noncardpayment@moneyfex.com", "Bank to bank transfer", body);
            mail.SendMail(FaxerDetails.Email, "Bank to Bank transfer payment", body);

            return RedirectToAction("PaymentSuccessful");

        }
        public ActionResult BankToBankDepositConfirmation_No()
        {

            return RedirectToAction("UsingBankToBank");
        }


        [HttpGet]
        public ActionResult ContactMoneyFex()
        {

            Models.ContactMoneyFexViewModel model = new Models.ContactMoneyFexViewModel();
            model.ReceiverEmail = Common.FaxerSession.NonCardReceiversDetails.ReceiverEmailAddress;
            model.MoneyFexEmail = "noncardfex@moneyfex.com";
            model.Subject = "Noncard transfer payment";
            return View(model);
        }
        public ActionResult ContactMoneyFex([Bind(Include = Models.ContactMoneyFexViewModel.BindProperty)] Models.ContactMoneyFexViewModel model)
        {
            if (string.IsNullOrEmpty(model.PaymentRefernce))
            {
                ModelState.AddModelError("PaymentRefernce", "Please enter a Payment Reference ");
                return View(model);
            }


            DB.BankToBankFaxingNonCardTransaction faxingNonCardTransaction = new DB.BankToBankFaxingNonCardTransaction()
            {

                FaxerId = Common.FaxerSession.LoggedUser.Id,
                ReceiverId = Common.FaxerSession.NonCardReceiverId,
                AmountSent = Common.FaxerSession.FaxingAmountSummary.FaxingAmount,
                ExchangeRate = Common.FaxerSession.FaxingAmountSummary.ExchangeRate,
                FaxingFee = Common.FaxerSession.FaxingAmountSummary.FaxingFee,
                ReceivingAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount,
                TotalAmountImcludingFee = Common.FaxerSession.FaxingAmountSummary.TotalAmount,
                PaymentReference = model.PaymentRefernce,
                TransactionDate = DateTime.Now
            };



            var obj = dbContext.BankToBankFaxingNonCardTransaction.Add(faxingNonCardTransaction);
            dbContext.SaveChanges();
            var ReceiverDetials = dbContext.ReceiversDetails.Where(x => x.Id == obj.ReceiverId).FirstOrDefault();
            var FaxerDetails = dbContext.FaxerInformation.Where(x => x.Id == obj.FaxerId).FirstOrDefault();

            var ReceiverCountry = Common.Common.GetCountryName(ReceiverDetials.Country);
            var FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string body = "";

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BankToBankNonCardTransfer?FaxerName=" + FaxerDetails.FirstName + " " + FaxerDetails.MiddleName + " " + FaxerDetails.LastName
                + "&FaxerAccountNo=" + FaxerDetails.AccountNo + "&ReceiverFullName=" + ReceiverDetials.FirstName +
                " " + ReceiverDetials.MiddleName + " " + ReceiverDetials.LastName + "&ReceiverCountry=" + ReceiverCountry +
                "&ReceiverEmail=" + ReceiverDetials.EmailAddress +
                "&ReceiverTelephone=" + ReceiverDetials.PhoneNumber + "&ReceiverCity=" + ReceiverDetials.City + "&Amount=" + obj.AmountSent + " " + FaxerCurrency
                + "&BankReference=" + obj.PaymentReference);

            //mail.SendMail("noncardfex@moneyfex.com", "Bank to Bank  Non Card MoneyFex", body);
            //mail.SendMail("moneyfaxer@gmail.com", "Bank to bank transfer ", body);
            mail.SendMail("noncardpayment@moneyfex.com", "Bank to bank transfer", body);
            //mail.SendMail("anankarki97@gmail.com", "Bank to Bank transfer payment", body);


            return RedirectToAction("PaymentSuccessful");
        }

        public ActionResult PaymentSuccessful()
        {
            Session.Remove("BankToBankTransfer");
            return View();

        }
    }
}