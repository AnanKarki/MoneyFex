using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Dashboard
{

    // Standing Order Payments
    public class FaxerAutoPaymentsController : Controller
    {
        int FaxerInformationId = 0;
        DB.FAXEREntities context = new DB.FAXEREntities();
        public FaxerAutoPaymentsController()
        {
            if (FaxerSession.LoggedUser != null)
            {
                FaxerInformationId = FaxerSession.LoggedUser.Id;
            }
        }
        // GET: FaxerAutoPayments
        public ActionResult Index()
        {
            if (Common.FaxerSession.LoggedUser == null)
            {

                return RedirectToAction("Login", "faxeraccount");
            }

            //ViewBag.MFTCAutoTopupCount = context.MFTCCardInformation.Where(x => x.FaxerId == FaxerInformationId && x.IsDeleted == false && x.AutoTopUp == true).Count();
            ViewBag.MerchantAutoPayment = context.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == FaxerInformationId && x.EnableAutoPayment == true).Count();
            ViewBag.OtherMFTCCardPayment = context.OtherMFTCCardAutoTopUpInformation.Where(x => x.FaxerId == FaxerInformationId && x.EnableAutoPayment == true).Count();
            return View();
        }
        public ActionResult MerchantAutoPayments(int faxMerchantPayInfoId = 0)
        {
            if (FaxerSession.LoggedUser == null)
            {
                var Url = Request.Url.ToString();
                string[] tokens = Url.Split('/');

                FaxerSession.FromUrl = "/" + tokens[3] + "/" + tokens[4];

                return RedirectToAction("Login", "FaxerAccount");
            }
            List<SenderKiiPayBusinessPaymentInformation> lst = context.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == FaxerInformationId).ToList();

            var list = (from c in context.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == FaxerInformationId)
                        select new MerchantInfoDropDowmViewModel()
                        {
                            Id = c.Id,
                            MerchantName = c.KiiPayBusinessInformation.BusinessName,

                        }
                        ).ToList();

            List<MerchantAutoPaymentsViewModel> gridLst = new List<MerchantAutoPaymentsViewModel>();
            if (faxMerchantPayInfoId == 0)
            {
                ViewBag.PreviousPayee = new SelectList(list, "Id", "MerchantName");
            }
            else
            {
                ViewBag.PreviousPayee = new SelectList(list, "Id", "MerchantName", faxMerchantPayInfoId);
                gridLst = (from c in context.FaxerMerchantPaymentInformation.Where(x => x.Id == faxMerchantPayInfoId).ToList()
                           join e in context.Country on c.KiiPayBusinessInformation.BusinessOperationCountryCode equals e.CountryCode
                           select new MerchantAutoPaymentsViewModel()
                           {
                               AccountNo = c.KiiPayBusinessInformation.BusinessMobileNo,
                               AutoAmount = c.AutoPaymentAmount.ToString(),
                               City = c.KiiPayBusinessInformation.BusinessOperationAddress1,
                               Country = e.CountryName,
                               Id = c.Id,
                               MerchantName = c.KiiPayBusinessInformation.BusinessName,
                               EnableAutoPayment = c.EnableAutoPayment == true ? "Yes" : "No",
                               PaymentFrequency = Enum.GetName(typeof(AutoPaymentFrequency), c.AutoPaymentFrequency),
                               Frequency = c.AutoPaymentFrequency,
                               FrequencyDetails = c.FrequencyDetails,
                               PaymentReference = c.PaymentRefrence
                           }).ToList();
                foreach (var freqencydetails in gridLst)
                {

                    var paymentDay = Convert.ToInt32(freqencydetails.FrequencyDetails);
                    if (freqencydetails.Frequency == AutoPaymentFrequency.Weekly)
                    {
                        freqencydetails.FrequencyDetails = Enum.GetName(typeof(DayOfWeek), paymentDay) + " every Week";
                    }
                    else if (freqencydetails.Frequency == AutoPaymentFrequency.Monthly)
                    {
                        string abbreviation = "";
                        if (paymentDay == 01 || paymentDay == 21 || paymentDay == 31)
                        {

                            abbreviation = "st";
                        }
                        else if (paymentDay == 02 || paymentDay == 22)
                        {
                            abbreviation = "nd";
                        }
                        else if (paymentDay == 03 || paymentDay == 23)
                        {
                            abbreviation = "rd";
                        }
                        else
                        {
                            abbreviation = "th";
                        }

                        freqencydetails.FrequencyDetails = paymentDay + abbreviation + " of the every Month";
                    }
                    else if (freqencydetails.Frequency == AutoPaymentFrequency.Yearly)
                    {
                        string PaymentDate = freqencydetails.FrequencyDetails;
                        int Month = int.Parse(PaymentDate.Substring(0, 2));
                        int Day = int.Parse(PaymentDate.Substring(2, 2));
                        string MonthName = Enum.GetName(typeof(Month), Month);
                        string abbreviation = "";
                        if (Day == 01 || Day == 21 || Day == 31)
                        {

                            abbreviation = "st";
                        }
                        else if (Day == 02 || Day == 22)
                        {
                            abbreviation = "nd";
                        }
                        else if (Day == 03 || Day == 23)
                        {
                            abbreviation = "rd";
                        }
                        else
                        {
                            abbreviation = "th";
                        }
                        freqencydetails.FrequencyDetails = MonthName + " " + Day + abbreviation + " of  the every Year";

                    }
                    else
                    {
                        freqencydetails.FrequencyDetails = "None";
                    }
                }
                FaxerSession.MerchantPayInfoId = faxMerchantPayInfoId;
            }
            return View(gridLst);
        }

        public ActionResult MerchantAutoPaymentAdd()
        {
            var SavedCard = context.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry);
            if (SavedCard == null)
            {

                @TempData["CardCount"] = 0;
                return RedirectToAction("MerchantAutoPayments");
                //ModelState.AddModelError("Error", "Please Add Creidt/Debit Card to Set AutoPayment for Merchant ");
                //return View();
            }
            var merchantPaymentInformation = context.FaxerMerchantPaymentInformation.Where(x => x.Id == FaxerSession.MerchantPayInfoId).FirstOrDefault();
            MerchantAutoPaymentAddViewModel model = new MerchantAutoPaymentAddViewModel();
            if (merchantPaymentInformation.EnableAutoPayment == true)
            {
                model.AutoPaymentAmount = merchantPaymentInformation.AutoPaymentAmount;
                model.AutoPaymentFrequency = merchantPaymentInformation.AutoPaymentFrequency;
                model.FrequencyDetails = merchantPaymentInformation.FrequencyDetails;
                model.PaymentReference = merchantPaymentInformation.PaymentRefrence;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult MerchantAutoPaymentAdd([Bind(Include = MerchantAutoPaymentAddViewModel.BindProperty)]MerchantAutoPaymentAddViewModel model)
        {
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry);
            if (model.AutoPaymentAmount != 0)
            {

                if (model.AutoPaymentFrequency == AutoPaymentFrequency.NoLimitSet)
                {
                    ModelState.AddModelError("AutoPaymentFrequency", "Please select payment frequency");
                    return View(model);
                }
                else if (string.IsNullOrEmpty(model.PaymentReference)) {

                    ModelState.AddModelError("PaymentReference", "Please enter the payment reference");
                    return View(model);
                }
                var SavedCard = context.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                if (SavedCard == null)
                {
                    ModelState.AddModelError("Error", "Please Add Creidt/Debit Card to Set AutoPayment for Merchant ");
                    return View(model);
                }
                var merchantPaymentInformation = context.FaxerMerchantPaymentInformation.Where(x => x.Id == FaxerSession.MerchantPayInfoId).FirstOrDefault();
                merchantPaymentInformation.AutoPaymentAmount = model.AutoPaymentAmount;
                merchantPaymentInformation.AutoPaymentFrequency = model.AutoPaymentFrequency;
                merchantPaymentInformation.EnableAutoPayment = true;
                merchantPaymentInformation.FrequencyDetails = model.FrequencyDetails;
                merchantPaymentInformation.PaymentRefrence = model.PaymentReference;
                context.Entry(merchantPaymentInformation).State = EntityState.Modified;
                context.SaveChanges();
                // Send email for confirmation of Auto Top Up Set up Merchant 
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string body = "";
                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                string FaxerCountry = context.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).Select(x => x.Country).FirstOrDefault();
                string CountryCurrency = Common.Common.GetCountryCurrency(FaxerCountry);
                var BusinessMerchantDetails = context.KiiPayBusinessInformation.Where(x => x.Id == merchantPaymentInformation.KiiPayBusinessInformationId).FirstOrDefault();
                var cardDetails = context.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                string CardNumber = "xxxx-xxxx-xxxx-" + cardDetails.Num.Decrypt().Right(4);
                string SetAutoPaymentLink = baseUrl + "/FaxerAutoPayments/MerchantAutoPayments?faxMerchantPayInfoId=" + merchantPaymentInformation.Id;
                string PayforGoodsBusinessMerchant = baseUrl + "/PayForGoodsAndServicesAbroad/MerchantDetails?MerchantACNumber=" + BusinessMerchantDetails.BusinessMobileNo;
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfAutoPaymentSetupFaxer/?FaxerName=" +
                    FaxerName + "&AutoPaymentAmount=" + merchantPaymentInformation.AutoPaymentAmount + "&CountryCurrencySymbol=" + CountryCurrency
                    + "&BusinessMerchantName=" + BusinessMerchantDetails.BusinessName + "&AutoPaymentFrequency=" + merchantPaymentInformation.AutoPaymentFrequency
                    + "&CreditORDebitCardlast4digits=" + CardNumber
                    + "&SetAutoPayment=" + SetAutoPaymentLink +
                    "&PayforGoodsBusinessMerchant=" + PayforGoodsBusinessMerchant);
                mail.SendMail(FaxerEmail, "Confirmation of Auto Payment Setup", body);
                // End
                return RedirectToAction("MerchantAutoPaymentSuccessfullySetup", "FaxerAutoPayments");
            }
            else
            {
                ModelState.AddModelError("Error", "Auto payment Amount is Required.");
            }
            return View();
        }
        public ActionResult MerchantAutoPaymentSuccessfullySetup()
        {
            ViewBag.faxMerchantPayInfoId = FaxerSession.MerchantPayInfoId;
            return View();
        }

        [HttpPost]
        public ActionResult DeleteMerchantAutoPayments(int merchantPaymentInformationId)
        {

            if (merchantPaymentInformationId != 0)
            {
                var merchantPaymentInformation = context.FaxerMerchantPaymentInformation.Where(x => x.Id == merchantPaymentInformationId).FirstOrDefault();
                string AutoPaymentAmount = merchantPaymentInformation.AutoPaymentAmount.ToString();
                merchantPaymentInformation.AutoPaymentAmount = 0;
                merchantPaymentInformation.AutoPaymentFrequency = 0;
                merchantPaymentInformation.EnableAutoPayment = false;
                merchantPaymentInformation.FrequencyDetails = null;
                context.Entry(merchantPaymentInformation).State = EntityState.Modified;
                context.SaveChanges();
                // Send email for Confirmation of Auto payment to Merchant deletion
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string body = "";
                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                string FaxerCountry = context.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).Select(x => x.Country).FirstOrDefault();
                string CountryCurrency = Common.Common.GetCountryCurrency(FaxerCountry);
                var BusinessMerchantDetials = context.KiiPayBusinessInformation.Where(x => x.Id == merchantPaymentInformation.KiiPayBusinessInformationId).FirstOrDefault();
                string SetAutoPaymentLink = baseUrl + "/FaxerAutoPayments/MerchantAutoPayments?faxMerchantPayInfoId=" + merchantPaymentInformation.Id;
                string PayforGoodsBusinessMerchant = baseUrl + "/PayForGoodsAndServicesAbroad/MerchantDetails?MerchantACNumber=" + BusinessMerchantDetials.BusinessMobileNo;
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfAutoPaymentDeletion/?FaxerName=" +
                    FaxerName + "&AutoTopUpAmount=" + AutoPaymentAmount + "&CountryCurrencySymbol=" + CountryCurrency
                    + "&BusinessMerchantName=" + BusinessMerchantDetials.BusinessName + "&SetAutoPayment=" + SetAutoPaymentLink +
                    "&PayforGoodsBusinessMerchant=" + PayforGoodsBusinessMerchant);
                mail.SendMail(FaxerEmail, "Confirmation of Auto payment to Merchant deletion", body);
                // End
                return RedirectToAction("MerchantAutoPayments", "FaxerAutoPayments");
            }
            return View();
        }

        #region MoneyFax Card Auto TopUp
        public ActionResult MoneyFaxCardAutoTopUp(int mftcCardInformationId = 0)
        {
            if (FaxerSession.LoggedUser == null)
            {

                var Url = Request.Url.ToString();
                string[] tokens = Url.Split('/');
                Common.FaxerSession.FromUrl = "/" + tokens[3] + "/" + tokens[4];

                return RedirectToAction("Login", "FaxerAccount");

            }

            FaxerSession.MFTCCardInformationId = mftcCardInformationId;

            var list = (from c in context.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false).ToList()
                        select new AutoTopUpMFTCCardList()
                        {

                            Id = c.Id,
                            FullName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                        }
                        );

            List<MoneyFaxCardAutoTopUpViewModel> vmList = new List<MoneyFaxCardAutoTopUpViewModel>();
            if (mftcCardInformationId == 0)
            {
                ViewBag.FirstName = new SelectList(list, "Id", "FullName");
            }
            else
            {
                ViewBag.FirstName = new SelectList(list, "Id", "FullName", mftcCardInformationId);
                vmList = (from c in context.KiiPayPersonalWalletInformation.Where(x => x.Id == mftcCardInformationId).ToList()
                          select new MoneyFaxCardAutoTopUpViewModel()
                          {
                              Id = c.Id,
                              AutoAmount = c.AutoTopUpAmount,
                              MFTCCardNo = c.MobileNo.Decrypt(),
                              NameonMFTCCard = c.FirstName + " " + c.MiddleName + "" + c.LastName,
                              AutoTopUp = c.AutoTopUp == true ? "Yes" : "No",
                          }).ToList();
                FaxerSession.AutoAmount = context.KiiPayPersonalWalletInformation.Where(x => x.Id == mftcCardInformationId).FirstOrDefault().AutoTopUpAmount;

            }
            return View(vmList);
        }
        public ActionResult AutoPaymentTopUp([Bind(Include = AutoPaymentTopUpViewModel.BindProperty)]AutoPaymentTopUpViewModel vm)
        {
            var SavedCard = context.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            if (SavedCard == null)
            {

                @TempData["CardCount"] = 0;
                return RedirectToAction("MoneyFaxCardAutoTopUp");

                //ModelState.AddModelError("Error", "Please Add Creidt/Debit Card to Set Auto Top-Up ");
                //return View(vm);
            }
            vm.TopUpAmount = FaxerSession.AutoAmount;
            return View(vm);
        }
        [HttpPost]
        public ActionResult AddAutoPaymentTopUp([Bind(Include = AutoPaymentTopUpViewModel.BindProperty)]AutoPaymentTopUpViewModel vm)
        {
            var SavedCard = context.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            if (SavedCard == null)
            {
                return RedirectToAction("AutoPaymentTopUp");
            }

            if (vm.TopUpAmount <= 0) {

                ModelState.AddModelError("Error", "Amount should be greater than 0");
                return View(vm);

            }
            int Id = FaxerSession.MFTCCardInformationId;
            var data = context.KiiPayPersonalWalletInformation.Find(Id);
            data.AutoTopUpAmount = vm.TopUpAmount;
            data.AutoTopUp = true;
            context.Entry(data).State = EntityState.Modified;
            context.SaveChanges();
            // Send email for confirmation of Auto Top Up Set Up
            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string FaxerName = Common.FaxerSession.LoggedUser.FullName;
            string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
            string CountryCurrency = Common.Common.GetCountryCurrency(data.CardUserCountry);
            string TopUpMoneyfaxCard = baseUrl + "/StartFaxingMoney/Index?mFTCCardInformationId=" + Id;
            string PayForGoodsAbroad = baseUrl + "/PayForGoodsAndServicesAbroad";
            string SetAutoTopUp = baseUrl + "/FaxerAutoPayments/MoneyFaxCardAutoTopUp?mftcCardInformationId=" + Id;

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfAutoPaymentMFTCTopUpSetupFaxer/?FaxerName=" +
                FaxerName + "&AutoTopUpAmount=" + vm.TopUpAmount + "&CountryCurrencySymbol=" + CountryCurrency
                + "&MFTCCardNumber=" + data.MobileNo.Decrypt()
                + "&CardUserName=" + data.FirstName + " " + data.MiddleName + " " + data.LastName +
                "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard + "&SetAutoTopUp=" + SetAutoTopUp +
                "&PayForGoodsAbroad=" + PayForGoodsAbroad);
            mail.SendMail(FaxerEmail, "Confirmation of Auto Top-Up Setup", body);
            // End 
            return RedirectToAction("AutoTopUpSuccessfullySetup", "FaxerAutoPayments");

        }
        public ActionResult AutoTopUpSuccessfullySetup()
        {
            ViewBag.MFTCCardId = FaxerSession.MFTCCardInformationId;
            return View();
        }
        [HttpPost]
        public ActionResult DeleteMoneyFaxCardAutoTopUp(int CardId)
        {
            if (CardId != 0)
            {
                var mftcCard = context.KiiPayPersonalWalletInformation.Find(CardId);
                string AutoTopUpAmount = mftcCard.AutoTopUpAmount.ToString();
                mftcCard.AutoTopUpAmount = 0;
                mftcCard.AutoTopUp = false;
                context.Entry(mftcCard).State = EntityState.Modified;
                context.SaveChanges();
                // Send email for confirmation of Auto Top Up Deletion
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string body = "";
                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                string CountryCurrency = Common.Common.GetCountryCurrency(mftcCard.CardUserCountry);
                string TopUpMoneyfaxCard = baseUrl + "/StartFaxingMoney/Index?mFTCCardInformationId=" + CardId;
                string PayForGoodsAbroad = baseUrl + "/PayForGoodsAndServicesAbroad/MerchantAccountNumber";
                string SetAutoTopUp = baseUrl + "/FaxerAutoPayments/MoneyFaxCardAutoTopUp?mftcCardInformationId=" + CardId;

                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfAutoPaymentMFTCTopUpDeletion/?FaxerName=" +
                    FaxerName + "&AutoTopUpAmount=" + AutoTopUpAmount + "&CountryCurrencySymbol=" + CountryCurrency
                    + "&MFTCCardNumber=" + mftcCard.MobileNo.Decrypt()
                    + "&CardUserName=" + mftcCard.FirstName + " " + mftcCard.MiddleName + " " + mftcCard.LastName +
                    "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard + "&SetAutoTopUp=" + SetAutoTopUp +
                "&PayForGoodsAbroad=" + PayForGoodsAbroad);
                mail.SendMail(FaxerEmail, "Confirmation of Auto Top-Up deletion", body);
                // End 
                return RedirectToAction("MoneyFaxCardAutoTopUp", "FaxerAutoPayments");
            }
            return View();
        }
        #endregion

        public ActionResult SomeoneElseMoneyFaxCardAutoTopUp(string MFTCCardNo = "")
        {
            int faxerId = Common.FaxerSession.LoggedUser.Id;

            if (string.IsNullOrEmpty(MFTCCardNo))
            {
                var vmList = (from c in context.OtherMFTCCardAutoTopUpInformation.Where(x => x.FaxerId == Common.FaxerSession.LoggedUser.Id).ToList()
                              select new SomeoneElseMoneyFexCardAutoTopUpViewModel()
                              {
                                  Id = c.Id,
                                  MFTCCardId = c.MFTCCardId,
                                  AutoAmount = c.AutoPaymentAmount,
                                  MFTCCardNo = c.MFTCCard.MobileNo.Decrypt(),
                                  NameonMFTCCard = c.MFTCCard.FirstName + " " + c.MFTCCard.MiddleName + "" + c.MFTCCard.LastName,
                                  AutoTopUp = c.EnableAutoPayment == true ? "Yes" : "No",
                                  PaymentFrequency = Enum.GetName(typeof(AutoPaymentFrequency), c.AutoPaymentFrequency),
                                  Frequency = c.AutoPaymentFrequency,
                                  FrequencyDetails = c.FrequencyDetails
                              }).ToList();

                foreach (var freqencydetails in vmList)
                {

                    var paymentDay = Convert.ToInt32(freqencydetails.FrequencyDetails);
                    if (freqencydetails.Frequency == AutoPaymentFrequency.Weekly)
                    {
                        freqencydetails.FrequencyDetails = Enum.GetName(typeof(DayOfWeek), paymentDay) + " every Week";
                    }
                    else if (freqencydetails.Frequency == AutoPaymentFrequency.Monthly)
                    {
                        string abbreviation = "";
                        if (paymentDay == 01 || paymentDay == 21 || paymentDay == 31)
                        {

                            abbreviation = "st";
                        }
                        else if (paymentDay == 02 || paymentDay == 22)
                        {
                            abbreviation = "nd";
                        }
                        else if (paymentDay == 03 || paymentDay == 23)
                        {
                            abbreviation = "rd";
                        }
                        else
                        {
                            abbreviation = "th";
                        }

                        freqencydetails.FrequencyDetails = paymentDay + abbreviation + " of the every Month";
                    }
                    else if (freqencydetails.Frequency == AutoPaymentFrequency.Yearly)
                    {
                        string PaymentDate = freqencydetails.FrequencyDetails;
                        int Month = int.Parse(PaymentDate.Substring(0, 2));
                        int Day = int.Parse(PaymentDate.Substring(2, 2));
                        string MonthName = Enum.GetName(typeof(Month), Month);
                        string abbreviation = "";
                        if (Day == 01 || Day == 21 || Day == 31)
                        {

                            abbreviation = "st";
                        }
                        else if (Day == 02 || Day == 22)
                        {
                            abbreviation = "nd";
                        }
                        else if (Day == 03 || Day == 23)
                        {
                            abbreviation = "rd";
                        }
                        else
                        {
                            abbreviation = "th";
                        }
                        freqencydetails.FrequencyDetails = MonthName + " " + Day + abbreviation + " of  the every Year";

                    }
                    else
                    {
                        freqencydetails.FrequencyDetails = "None";
                    }
                }



                return View(vmList);
            }
            else
            {
                var VmList = new List<SomeoneElseMoneyFexCardAutoTopUpViewModel>();
                var MFTCCard = MFTCCardNo.Encrypt();
                var MFTCCardDetails = context.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTCCard).FirstOrDefault();
                if (MFTCCardDetails != null)
                {
                    //if (faxerId == MFTCCardDetails.FaxerId)
                    //{
                    //    ModelState.AddModelError("CardInvalid", "This card is your own ");
                    //    return View(VmList);
                    //}

                    var data = context.OtherMFTCCardAutoTopUpInformation.Where(x => x.MFTCCardId == MFTCCardDetails.Id && x.FaxerId == faxerId).ToList();

                    if (data.Count > 0)
                    {
                        VmList = (from c in data
                                  select new SomeoneElseMoneyFexCardAutoTopUpViewModel()
                                  {
                                      Id = c.Id,
                                      MFTCCardId = c.MFTCCardId,
                                      AutoAmount = c.AutoPaymentAmount,
                                      MFTCCardNo = c.MFTCCard.MobileNo.Decrypt(),
                                      NameonMFTCCard = c.MFTCCard.FirstName + " " + c.MFTCCard.MiddleName + "" + c.MFTCCard.LastName,
                                      AutoTopUp = c.EnableAutoPayment == true ? "Yes" : "No",
                                      PaymentFrequency = Enum.GetName(typeof(AutoPaymentFrequency), c.AutoPaymentFrequency),
                                      Frequency = c.AutoPaymentFrequency,
                                      FrequencyDetails = c.FrequencyDetails
                                  }).ToList();

                        foreach (var freqencydetails in VmList)
                        {

                            var paymentDay = Convert.ToInt32(freqencydetails.FrequencyDetails);
                            if (freqencydetails.Frequency == AutoPaymentFrequency.Weekly)
                            {
                                freqencydetails.FrequencyDetails = Enum.GetName(typeof(DayOfWeek), paymentDay) + " every Week";
                            }
                            else if (freqencydetails.Frequency == AutoPaymentFrequency.Monthly)
                            {
                                string abbreviation = "";
                                if (paymentDay == 01 || paymentDay == 21 || paymentDay == 31)
                                {

                                    abbreviation = "st";
                                }
                                else if (paymentDay == 02 || paymentDay == 22)
                                {
                                    abbreviation = "nd";
                                }
                                else if (paymentDay == 03 || paymentDay == 23)
                                {
                                    abbreviation = "rd";
                                }
                                else
                                {
                                    abbreviation = "th";
                                }

                                freqencydetails.FrequencyDetails = paymentDay + abbreviation + " of the every Month";
                            }
                            else if (freqencydetails.Frequency == AutoPaymentFrequency.Yearly)
                            {
                                string PaymentDate = freqencydetails.FrequencyDetails;
                                int Month = int.Parse(PaymentDate.Substring(0, 2));
                                int Day = int.Parse(PaymentDate.Substring(2, 2));
                                string MonthName = Enum.GetName(typeof(Month), Month);
                                string abbreviation = "";
                                if (Day == 01 || Day == 21 || Day == 31)
                                {

                                    abbreviation = "st";
                                }
                                else if (Day == 02 || Day == 22)
                                {
                                    abbreviation = "nd";
                                }
                                else if (Day == 03 || Day == 23)
                                {
                                    abbreviation = "rd";
                                }
                                else
                                {
                                    abbreviation = "th";
                                }
                                freqencydetails.FrequencyDetails = MonthName + " " + Day + abbreviation + " of  the every Year";

                            }
                            else
                            {
                                freqencydetails.FrequencyDetails = "None";
                            }
                        }
                    }
                    else
                    {
                        VmList = (from c in context.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardDetails.Id).ToList()
                                  select new SomeoneElseMoneyFexCardAutoTopUpViewModel()
                                  {
                                      Id = 0,
                                      MFTCCardId = c.Id,
                                      AutoAmount = 0,
                                      MFTCCardNo = c.MobileNo.Decrypt(),
                                      NameonMFTCCard = c.FirstName + " " + c.MiddleName + "" + c.LastName,
                                      AutoTopUp = "No",
                                      PaymentFrequency = Enum.GetName(typeof(AutoPaymentFrequency), 0),
                                      Frequency = 0,
                                      FrequencyDetails = "",
                                  }).ToList();



                    }

                }
                return View(VmList);
            }


        }

        [HttpGet]
        public ActionResult AddSomeoneElseMoneyFaxCardAutoTopUp(int MFTCCardid)
        {
            Models.SomeoneElseAutoPaymentViewModel model = new SomeoneElseAutoPaymentViewModel();
            model.MFTCCardid = MFTCCardid;
            Common.FaxerSession.MFTCCard = context.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardid).FirstOrDefault().MobileNo.Decrypt();

            var data = context.OtherMFTCCardAutoTopUpInformation.Where(x => x.FaxerId == Common.FaxerSession.LoggedUser.Id && x.MFTCCardId == MFTCCardid).FirstOrDefault();

            if (data != null)
            {

                model.AutoPaymentAmount = data.AutoPaymentAmount;
                model.FrequencyDetails = data.FrequencyDetails;
                model.AutoPaymentFrequency = data.AutoPaymentFrequency;
                model.TopUpReference = data.TopUpReference;
            }
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
            return View(model);
        }
        [HttpPost]
        public ActionResult AddSomeoneElseMoneyFaxCardAutoTopUp([Bind(Include = SomeoneElseAutoPaymentViewModel.BindProperty)]SomeoneElseAutoPaymentViewModel vm)
        {


            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
            if (vm.AutoPaymentAmount <= 0)
            {
                ModelState.AddModelError("AutoPaymentAmount", "Please enter amount greater than 0");
                return View(vm);
            }
            if (vm.AutoPaymentFrequency == AutoPaymentFrequency.NoLimitSet)
            {
                ModelState.AddModelError("AutoPaymentFrequency", "Please select a payment frequency");
                return View(vm);

            }
            if (vm.TopUpReference == null)
            {
                ModelState.AddModelError("TopUpReference", "Please enter a payment Reference ");
                return View(vm);
            }
            int FaxerId = Common.FaxerSession.LoggedUser.Id;
            var data = context.OtherMFTCCardAutoTopUpInformation.Where(x => x.MFTCCardId == vm.MFTCCardid && x.FaxerId == FaxerId).FirstOrDefault();


            if (data != null)
            {

                data.AutoPaymentAmount = vm.AutoPaymentAmount;
                data.AutoPaymentFrequency = vm.AutoPaymentFrequency;
                data.EnableAutoPayment = true;
                data.FrequencyDetails = vm.FrequencyDetails;
                data.TopUpReference = vm.TopUpReference;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();

            }
            else
            {
                OtherMFTCCardAutoTopUpInformation autoTopUpInformation = new OtherMFTCCardAutoTopUpInformation()
                {

                    MFTCCardId = vm.MFTCCardid,
                    AutoPaymentAmount = vm.AutoPaymentAmount,
                    AutoPaymentFrequency = vm.AutoPaymentFrequency,
                    EnableAutoPayment = true,
                    FaxerId = Common.FaxerSession.LoggedUser.Id,
                    TopUpReference = vm.TopUpReference,
                    FrequencyDetails = vm.FrequencyDetails

                };
                context.OtherMFTCCardAutoTopUpInformation.Add(autoTopUpInformation);
                context.SaveChanges();

            }

            MailCommon mail = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string FaxerName = Common.FaxerSession.LoggedUser.FullName;
            string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
            string FaxerCountry = context.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).Select(x => x.Country).FirstOrDefault();
            string CountryCurrency = Common.Common.GetCountryCurrency(FaxerCountry);
            var cardDetails = context.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            var MFTCCardDetials = context.KiiPayPersonalWalletInformation.Where(x => x.Id == vm.MFTCCardid).FirstOrDefault();
            string CardNumber = "xxxx-xxxx-xxxx-" + cardDetails.Num.Decrypt().Right(4);
            string SetAutoPaymentLink = baseUrl + "/FaxerAutoPayments/AddSomeoneElseMoneyFaxCardAutoTopUp?MFTCCardid=" + vm.MFTCCardid;
            string TopUCard = baseUrl + "/TopUpSomeoneElseMFTCCard/MFTCCardAccountNo?MFTCCardNO=" + MFTCCardDetials.MobileNo.Decrypt();
            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfAutoPaymentSetupSomeoneElseMFTCCard/?FaxerName=" +
                FaxerName + "&AutoPaymentAmount=" + vm.AutoPaymentAmount + "&CountryCurrencySymbol=" + CountryCurrency
                + "&CardUserName=" + MFTCCardDetials.FirstName + " " + MFTCCardDetials.MiddleName + " " + MFTCCardDetials.LastName
                + "&AutoPaymentFrequency=" + vm.AutoPaymentFrequency
                + "&CreditORDebitCardlast4digits=" + CardNumber
                + "&SetAutoPayment=" + SetAutoPaymentLink +
                "&TopUPMFTCCard=" + TopUCard);
            mail.SendMail(FaxerEmail, "Confirmation of Auto Payment Setup to someone else's card", body);
            return RedirectToAction("AddSomeoneElseMoneyFaxCardAutoTopUpSuccesfull");
        }

        public ActionResult AddSomeoneElseMoneyFaxCardAutoTopUpSuccesfull()
        {

            ViewBag.MFTCCard = Common.FaxerSession.MFTCCard;
            return View();

        }

        public ActionResult DeleteSomeoneElseMoneyFaxCardAutoTopUp(int Id)
        {

            var data = context.OtherMFTCCardAutoTopUpInformation.Where(x => x.Id == Id).FirstOrDefault();

            data.EnableAutoPayment = false;
            data.AutoPaymentAmount = 0;
            data.AutoPaymentFrequency = 0;
            data.TopUpReference = "";

            context.Entry(data).State = EntityState.Modified;
            context.SaveChanges();

            return RedirectToAction("SomeoneElseMoneyFaxCardAutoTopUp");

        }
    }
}