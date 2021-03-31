using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class PaymentMethodController : Controller
    {
        // GET: PaymentMethod
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        CommonServices CommonService = new CommonServices();
        public ActionResult Index()
        {
            try
            {
                if ((FaxerSession.LoggedUser == null))
                {
                    FaxerSession.FromUrl = "/PaymentMethod";
                    return RedirectToAction("Login", "FaxerAccount");
                }
                ViewBag.PaymentMethod = new SelectList(dbContext.PaymentMethods.OrderBy(x => x.PaymentMethodCode), "PaymentMethodCode", "PaymentMethodName");
            }
            catch (Exception)
            {


            }

            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = PaymentMethodViewModel.BindProperty)]PaymentMethodViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    FaxerSession.PaymentMethod = model.PaymentMethod;
                    switch (model.PaymentMethod)
                    {
                        case "PM001"://crdr card
                            return RedirectToAction("Index", "CardPayment");
                            break;
                        case "PM002"://saved crdrcard
                            return RedirectToAction("TopUpUsingSavedCreditDebit", "CardPayment");
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
        [HttpGet]
        public ActionResult MFTCTopUpUsingBankToBankPaymentMethod()
        {

            //Models.BankToBankTransferViewModel model = new Models.BankToBankTransferViewModel();
            //if (Common.FaxerSession.BankToBankTransfer != null)
            //{
            //    model = Common.FaxerSession.BankToBankTransfer;
            //}
            //model.SendingAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount;
            //model.SendingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.LoggedUser.CountryCode);
            //model.SendingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.LoggedUser.CountryCode);

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

            if (BankAccountInfo == null) {

                TempData["BankHasbeenSetuped"] = 0;
                return RedirectToAction("Index");
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
        public ActionResult MFTCTopUpUsingBankToBankPaymentMethod([Bind(Include = BankToBankTransferViewModel.BindProperty)]BankToBankTransferViewModel model)
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
                    ModelState.AddModelError("Confirm", "Please confirm to make Payment ");
                    return View(model);

                }
                if (model.Accept == false) {
                    ModelState.AddModelError("Accept", "Please accept our terms and condition to make payment");
                    return View(model);
                }
                Common.FaxerSession.BankToBankTransfer = model;

                ViewBag.ModelIsValid = 1;
                return View(model);
                //return RedirectToAction("ContactMoneyFex");
            }
            return View(model);

        }

        public ActionResult MFTCTopUpUsingBankToBankDepositConfirmation() {

            return View();
        }
        public ActionResult MFTCTopUpUsingBankToBankDepositConfirmation_Yes() {


            int MFTCCardId = int.Parse(Common.FaxerSession.TopUpCardId);
            var MFTCCardNumber = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault().MobileNo.Decrypt();

            DB.BankToBankMFTCCardTopUp MFTCCardTopUp = new DB.BankToBankMFTCCardTopUp()
            {

                FaxerId = Common.FaxerSession.LoggedUser.Id,
                MFTCCardId = int.Parse(Common.FaxerSession.TopUpCardId),
                MFTCCardNumber = MFTCCardNumber,
                PaymentReference = Common.FaxerSession.BankToBankTransfer.PaymentReference,
                TopUpAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount,
                TransactionDate = DateTime.Now,
                ExchangeRate = Common.FaxerSession.FaxingAmountSummary.ExchangeRate,
                FaxingFee = Common.FaxerSession.FaxingAmountSummary.FaxingFee,
                ReceivingAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount,
                TotalAmountImcludingFee = Common.FaxerSession.FaxingAmountSummary.TotalAmount

            };
            var obj = dbContext.BankToBankMFTCCardTopUp.Add(MFTCCardTopUp);
            dbContext.SaveChanges();


            var FaxerDetails = dbContext.FaxerInformation.Where(x => x.Id == obj.FaxerId).FirstOrDefault();
            var CardUserDetails = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == obj.MFTCCardId).FirstOrDefault();

            var FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string body = "";


            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BankToBankMFTCCardTopUp?FaxerName=" + FaxerDetails.FirstName + " " +
                FaxerDetails.MiddleName + " " + FaxerDetails.LastName +
                 "&CardUserName=" + CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName
                 + "&CardNumber=" + CardUserDetails.MobileNo.Decrypt().GetVirtualAccountNo()
                 + "&Amount=" + obj.TopUpAmount + " " + FaxerCurrency + "&BankReference=" + obj.PaymentReference);

            //mail.SendMail("noncardfex@moneyfex.com", "Bank to Bank Payment MFTC Card Top-Up", body);

            mail.SendMail("noncardpayment@moneyfex.com", "Bank to Bank transfer payment ", body);
            mail.SendMail(FaxerDetails.Email, "Bank to Bank transfer payment ", body);
            return RedirectToAction("BankToBankPaymentSuccessful");
           
        }
        public ActionResult MFTCTopUpUsingBankToBankDepositConfirmation_No() {

            return RedirectToAction("MFTCTopUpUsingBankToBankDepositConfirmation");
        }

        //[HttpGet]
        //public ActionResult ContactMoneyFex()
        //{
        //    int MFTCCardId = int.Parse(Common.FaxerSession.TopUpCardId);
        //    var MFTCCardNumber = dbContext.MFTCCardInformation.Where(x => x.Id == MFTCCardId).Select(x => x.MFTCCardNumber).FirstOrDefault();


        //    Models.ContactMoneyFexViewModel model = new Models.ContactMoneyFexViewModel();
        //    model.MFTCCardNumber = MFTCCardNumber.Decrypt();
        //    model.MoneyFexEmail = "noncardfex@moneyfex.com";
        //    model.Subject = "Top-Up payment bank transfer";

        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult ContactMoneyFex(Models.ContactMoneyFexViewModel model)
        //{
        //    if (string.IsNullOrEmpty(model.PaymentRefernce))
        //    {
        //        ModelState.AddModelError("PaymentRefernce", "Please enter a payment reference");
        //        return View(model);
        //    }
        //    DB.BankToBankMFTCCardTopUp MFTCCardTopUp = new DB.BankToBankMFTCCardTopUp()
        //    {

        //        FaxerId = Common.FaxerSession.LoggedUser.Id,
        //        MFTCCardId = int.Parse(Common.FaxerSession.TopUpCardId),
        //        MFTCCardNumber = model.MFTCCardNumber.Encrypt(),
        //        PaymentReference = model.PaymentRefernce,
        //        TopUpAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount,
        //        TransactionDate = DateTime.Now,
        //        ExchangeRate = Common.FaxerSession.FaxingAmountSummary.ExchangeRate,
        //        FaxingFee = Common.FaxerSession.FaxingAmountSummary.FaxingFee,
        //        ReceivingAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount,
        //        TotalAmountImcludingFee = Common.FaxerSession.FaxingAmountSummary.TotalAmount

        //    };
        //    var obj = dbContext.BankToBankMFTCCardTopUp.Add(MFTCCardTopUp);
        //    dbContext.SaveChanges();


        //    var FaxerDetails = dbContext.FaxerInformation.Where(x => x.Id == obj.FaxerId).FirstOrDefault();
        //    var CardUserDetails = dbContext.MFTCCardInformation.Where(x => x.Id == obj.MFTCCardId).FirstOrDefault();

        //    var FaxerCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
        //    MailCommon mail = new MailCommon();
        //    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

        //    string body = "";


        //    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BankToBankMFTCCardTopUp?FaxerName=" + FaxerDetails.FirstName + " " +
        //        FaxerDetails.MiddleName + " " + FaxerDetails.LastName +
        //         "&CardUserName=" + CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName
        //         + "&CardNumber=" + CardUserDetails.MFTCCardNumber.Decrypt()
        //         + "&Amount=" + obj.TopUpAmount + " " + FaxerCurrency + "&BankReference=" + obj.PaymentReference);

        //    //mail.SendMail("noncardfex@moneyfex.com", "Bank to Bank Payment MFTC Card Top-Up", body);

        //    mail.SendMail("noncardpayment@moneyfex.com", "Bank to Bank transfer payment ", body);


        //    return RedirectToAction("BankToBankPaymentSuccessful");
        //}

        public ActionResult BankToBankPaymentSuccessful()
        {

            return View();

        }


        [HttpGet]
        public ActionResult ReciverPaymentMethod()
        {
            try
            {
                if ((FaxerSession.LoggedUser == null))
                {
                    FaxerSession.FromUrl = "/PaymentMethod/ReciverPaymentMethod";
                    return RedirectToAction("Login", "FaxerAccount");
                }
                var Faxerdetails = dbContext.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                ViewBag.cardUrl = Faxerdetails.CardUrl;

                ViewBag.ToUrl = "/PaymentMethod/ReciverPaymentMethod";


                ViewBag.PaymentMethod = new SelectList(dbContext.PaymentMethods.OrderBy(x => x.PaymentMethodCode), "PaymentMethodCode", "PaymentMethodName");


            }
            catch (Exception)
            {


            }
            return View();
        }
        [HttpPost]
        public ActionResult ReciverPaymentMethod([Bind(Include = PaymentMethodViewModel.BindProperty)]PaymentMethodViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    FaxerSession.PaymentMethod = model.PaymentMethod;
                    switch (model.PaymentMethod)
                    {
                        case "PM001"://crdr card
                            return RedirectToAction("Receiver", "CardPayment");
                            break;
                        case "PM002"://saved crdrcard
                            return RedirectToAction("ReceiverTransferUsingSavedCreditDebitCard", "CardPayment");
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

        [HttpGet]
        public ActionResult ReceiverPaymentUsingBankToBank()
        {

            //Models.BankToBankTransferViewModel model = new Models.BankToBankTransferViewModel();
            //if (Common.FaxerSession.BankToBankTransfer != null)
            //{
            //    model = Common.FaxerSession.BankToBankTransfer;
            //}
            //model.SendingAmount = Common.FaxerSession.FaxingAmountSummary.FaxingAmount;

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
                return RedirectToAction("ReciverPaymentMethod");
            }
            if (BankAccountInfo != null)
            {
                model.AccountNumber = BankAccountInfo.AccountNo;
                model.LabelName = BankAccountInfo.LabelName;
                model.LabelValue = BankAccountInfo.LabelValue;

            }
            return View(model);

        }
        [HttpPost]
        public ActionResult ReceiverPaymentUsingBankToBank([Bind(Include = BankToBankTransferViewModel.BindProperty)]BankToBankTransferViewModel model)
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
                    ModelState.AddModelError("Confirm", "Please the payment condition");
                    return View(model);

                }
                if (model.Accept == false) {

                    ModelState.AddModelError("Accept", "Please accept the terms and condition");
                    return View(model);
                }
                //DB.ReceiversDetails receiversDetails = new DB.ReceiversDetails()
                //{

                //    FirstName = Common.FaxerSession.ReceiversDetails.ReceiverFirstName,
                //    MiddleName = Common.FaxerSession.ReceiversDetails.ReceiverMiddleName,
                //    LastName = Common.FaxerSession.ReceiversDetails.ReceiverLastName,
                //    City = Common.FaxerSession.ReceiversDetails.ReceiverCity,
                //    Country = Common.FaxerSession.ReceivingCountry,
                //    EmailAddress = Common.FaxerSession.ReceiversDetails.ReceiverEmailAddress,
                //    FaxerID = Common.FaxerSession.LoggedUser.Id,
                //    PhoneNumber = Common.FaxerSession.ReceiversDetails.ReceiverPhoneNumber,
                //    CreatedDate = DateTime.Now
                //};
                //dbContext.ReceiversDetails.Add(receiversDetails);
                //dbContext.SaveChanges();
                //Common.FaxerSession.NonCardReceiverId = receiversDetails.Id;


                Common.FaxerSession.BankToBankTransfer = model;
                return RedirectToAction("ReceiverPaymentUsingBankToBankDepositConfirmation");
                //return RedirectToAction("ReceiverPaymentContactMoneyFex");
            }
            return View(model);

        }


        [HttpGet]
        public ActionResult ReceiverPaymentUsingBankToBankDepositConfirmation()
        {

            return View();
        }

        public ActionResult ReceiverPaymentUsingBankToBankDepositConfirmation_Yes()
        {

            DB.ReceiversDetails receiversDetails = new DB.ReceiversDetails()
            {

                FirstName = Common.FaxerSession.ReceiversDetails.ReceiverFirstName,
                MiddleName = Common.FaxerSession.ReceiversDetails.ReceiverMiddleName,
                LastName = Common.FaxerSession.ReceiversDetails.ReceiverLastName,
                City = Common.FaxerSession.ReceiversDetails.ReceiverCity,
                Country = Common.FaxerSession.ReceivingCountry,
                EmailAddress = Common.FaxerSession.ReceiversDetails.ReceiverEmailAddress,
                FaxerID = Common.FaxerSession.LoggedUser.Id,
                PhoneNumber = Common.FaxerSession.ReceiversDetails.ReceiverPhoneNumber,
                CreatedDate = DateTime.Now
            };
            dbContext.ReceiversDetails.Add(receiversDetails);
            dbContext.SaveChanges();
            // Add City in City Table
            City newCity = new City()
            {
                CountryCode = receiversDetails.Country,
                Module = Module.Faxer,
                Name = receiversDetails.City
            };
            SCity.Save(newCity);
            // End 

            Common.FaxerSession.NonCardReceiverId = receiversDetails.Id;

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
            //mail.SendMail("anege1234@gmail.com", "Bank to Bank transfer payment", body);
            mail.SendMail("noncardpayment@moneyfex.com", "Bank to Bank transfer payment", body);


            return RedirectToAction("PaymentSuccessful");
            
        }

        public ActionResult ReceiverPaymentUsingBankToBankDepositConfirmation_No()
        {

            return RedirectToAction("ReceiverPaymentUsingBankToBank");
        }


        public ActionResult PaymentSuccessful()
        {

            return View();

        }
        // The below code has not been implemented as per the requirement was changed 
        [HttpGet]
        public ActionResult ReceiverPaymentContactMoneyFex()
        {

            Models.ContactMoneyFexViewModel model = new Models.ContactMoneyFexViewModel();
            model.ReceiverEmail = Common.FaxerSession.ReceiversDetails.ReceiverEmailAddress;
            model.MoneyFexEmail = "noncardfex@moneyfex.com";
            model.Subject = "Noncard transfer payment ";
            return View(model);
        }
        public ActionResult ReceiverPaymentContactMoneyFex([Bind(Include = ContactMoneyFexViewModel.BindProperty)]ContactMoneyFexViewModel model)
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
            //mail.SendMail("anege1234@gmail.com", "Bank to Bank transfer payment", body);
            mail.SendMail("noncardpayment@moneyfex.com", "Bank to Bank transfer payment", body);


            return RedirectToAction("PaymentSuccessful");
        }

       
    }
}
