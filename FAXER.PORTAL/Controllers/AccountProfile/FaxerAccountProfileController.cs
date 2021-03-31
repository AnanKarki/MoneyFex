using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.AccountProfile
{
    public class FaxerAccountProfileController : Controller
    {
        private DB.FAXEREntities context = null;
        private SFaxerAccountProfileServices _faxerAccountProfile = null;

        public FaxerAccountProfileController()
        {
            context = new FAXEREntities();
            _faxerAccountProfile = new SFaxerAccountProfileServices();
        }
        // GET: FaxerAccountProfile
        public ActionResult Index()
        {
            if ((FaxerSession.LoggedUser == null))
            {
                FaxerSession.FromUrl = "/FaxerAccountProfile";
                return RedirectToAction("Login", "FaxerAccount");
            }
            return View();
        }
        public ActionResult FaxerInformation()
        {
            Services.SFaxerSignUp service = new Services.SFaxerSignUp();
            var model = service.GetInformation(Common.FaxerSession.LoggedUserName);


            FaxerSession.ResetEmail = Common.FaxerSession.LoggedUserName;
            ViewBag.Countries = new SelectList(context.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            ViewBag.IdCardType = new SelectList(context.IdentityCardType.OrderBy(x => x.CardType), "CardType", "CardType");
            if (!string.IsNullOrEmpty(model.IdCardNumber))
            {
                int IDCardNumberCount = model.IdCardNumber.Count();
                model.IdCardNumber = "****" + model.IdCardNumber.Substring(IDCardNumberCount - 2, 2);

                model.IdCardType = "*****";
            }

            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {

                model.PhoneNumber = "*****" + model.PhoneNumber.Substring(model.PhoneNumber.Length - 4, 4);

            }

            model.IssuingCountry = Common.Common.GetCountryName(model.IssuingCountry);
            Common.FaxerSession.IDhasbeenExpired = false;
            return View(model);
        }

        [HttpPost]
        public ActionResult FaxerInformation(FaxerInformation model)
        {
            ViewBag.Countries = new SelectList(context.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            ViewBag.IdCardType = new SelectList(context.IdentityCardType.OrderBy(x => x.CardType), "CardType", "CardType");
            bool ISValid = true;

            if (string.IsNullOrEmpty(Common.FaxerSession.UserEnterAccountVerficationCode))
            {

                ModelState.AddModelError("VerificationError", "Please enter your address");
                TempData["InvalidInformation"] = true;
                return View(model);
            }
            if (Common.FaxerSession.UserEnterAccountVerficationCode.Trim() != Common.FaxerSession.AccountVerificationCode.Trim())
            {

                ModelState.AddModelError("VerificationError", "Please enter the verification code sent to your email address");
                TempData["InvalidInformation"] = true;
                return View(model);
            }
            if (string.IsNullOrEmpty(model.Address1))
            {

                ModelState.AddModelError("Address1", "Please enter your address");
                ISValid = false;
            }
            if (string.IsNullOrEmpty(model.City))
            {

                ModelState.AddModelError("City", "Please enter a City");
                ISValid = false;
            }
            if (string.IsNullOrEmpty(model.State))
            {

                ModelState.AddModelError("State", "Please enter your state");
                ISValid = false;
            }
            if (string.IsNullOrEmpty(model.PostalCode))
            {

                ModelState.AddModelError("PostalCode", "Please enter your postal code");
                ISValid = false;
            }
            if (string.IsNullOrEmpty(model.Country))
            {

                ModelState.AddModelError("Country", "Please select your country");
                ISValid = false;
            }

            if (ISValid)
            {
                var data = context.FaxerInformation.Find(model.Id);

                // Old value has been added on the Dictionnary to check the previous value has been changed or not campare to the newval
                var oldvaldict = new Dictionary<string, string>() {
                {"Address1", data.Address1} ,
                { "Address2", data.Address2},
                { "City",    data.City }  ,
                { "State",data.State    }  ,
                { "Country", data.Country}
                };

                data.Address1 = model.Address1;
                data.Address2 = model.Address2;
                data.City = model.City;
                data.State = model.State;
                data.Country = model.Country;

                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();

                var newValdict = new Dictionary<string, string>()
                {
                {"Address1", data.Address1} ,
                { "Address2", data.Address2},
                { "City",    data.City }  ,
                { "State",data.State    }  ,
                { "Country", data.Country}
                };

                foreach (var item in oldvaldict)
                {
                    string newValue = "";
                    try
                    {

                        newValue = newValdict[item.Key];

                    }
                    catch (Exception)
                    {

                    }

                    if (item.Value != newValue)
                    {

                        // Account profile update log 
                        UpdateLogHistory(item.Key, "FaxerInformation", item.Value, newValdict[item.Key]);
                    }

                }
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string body = "";

                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxerEmailUpdateEmail?SenderName=" + data.FirstName + " " + data.MiddleName + " " + data.LastName
                    + "&UpdatedPara= Address");
                mail.SendMail(data.Email, "Your Address Type has been Updated", body);
                return RedirectToAction("FaxerInformation");
            }
            else
            {
                TempData["InvalidInformation"] = true;
                return View(model);
            }
        }

        public ActionResult UpdateIDCardType(string IDCardType, int Id)
        {

            if (Id != 0)
            {


                var data = context.FaxerInformation.Find(Id);
                string OldValue = data.IdCardType;
                data.IdCardType = IDCardType;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();
                // Account Profile Update log 
                UpdateLogHistory("IdCardType", "FaxerInformation", OldValue, data.IdCardType);
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string body = "";

                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxerEmailUpdateEmail?SenderName=" + data.FirstName + " " + data.MiddleName + " " + data.LastName
                    + "&UpdatedPara= Id Card Type");
                mail.SendMail(data.Email, "Your Id Card Type has been Updated", body);

            }
            return RedirectToAction("FaxerInformation", "FaxerAccountProfile");
        }

        public ActionResult UpdateIdCardExpiryDate(string IdCardExpiryDate, int Id)
        {


            DateTime ExpiryDate = Convert.ToDateTime(IdCardExpiryDate);
            if (Id != 0)
            {
                var data = context.FaxerInformation.Find(Id);

                string OldValue = data.IdCardExpiringDate.ToString();
                data.IdCardExpiringDate = ExpiryDate;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();


                // Account Profile Update log 
                UpdateLogHistory("IdCardExpiringDate", "FaxerInformation", OldValue, data.IdCardExpiringDate.ToString());
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string body = "";

                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxerEmailUpdateEmail?SenderName=" + data.FirstName + " " + data.MiddleName + " " + data.LastName
                    + "&UpdatedPara= Id Card Expiry Date");
                mail.SendMail(data.Email, "Your Id Card Expiry Date has been Updated", body);
            }
            return RedirectToAction("FaxerInformation", "FaxerAccountProfile");
        }

        public ActionResult UpdateIDCardIssuingCountry(string IssuingCountry, int Id)
        {

            if (Id != 0)
            {
                var data = context.FaxerInformation.Find(Id);
                string OldValue = data.IssuingCountry;
                data.IssuingCountry = IssuingCountry;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();



                // Account Profile Update log 
                UpdateLogHistory("IssuingCountry", "FaxerInformation", OldValue, data.IssuingCountry);
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string body = "";

                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxerEmailUpdateEmail?SenderName=" + data.FirstName + " " + data.MiddleName + " " + data.LastName
                    + "&UpdatedPara= Id Card Issuing Country");
                mail.SendMail(data.Email, "Your Id Card Issuing Country has been Updated", body);
            }
            return RedirectToAction("FaxerInformation", "FaxerAccountProfile");

        }

        public ActionResult UpdateIDCardNumber(string IdCardNumber, int Id)
        {
            if (Id != 0)
            {
                var data = context.FaxerInformation.Find(Id);
                string OldValue = data.IdCardNumber;
                data.IdCardNumber = IdCardNumber;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();


                // Account Profile Update log 
                UpdateLogHistory("IdCardNumber", "FaxerInformation", OldValue, data.IdCardNumber);
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string body = "";

                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxerEmailUpdateEmail?SenderName=" + data.FirstName + " " + data.MiddleName + " " + data.LastName
                    + "&UpdatedPara= ID Card Number");
                mail.SendMail(data.Email, "Your Id Card Number has been Updated", body);
            }
            return RedirectToAction("FaxerInformation", "FaxerAccountProfile");


        }
        public ActionResult UpdateFaxerPhone(string PhoneNumber, int Id)
        {
            if (Id != 0)
            {

                var data = context.FaxerInformation.Find(Id);
                string OldValue = data.PhoneNumber;
                if (!string.IsNullOrEmpty(PhoneNumber))
                {


                    data.PhoneNumber = PhoneNumber.FormatPhoneNo();
                    context.Entry(data).State = EntityState.Modified;
                    context.SaveChanges();
                }


                // Account Profile Update log 
                UpdateLogHistory("PhoneNumber", "FaxerInformation", OldValue, data.PhoneNumber);

                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string body = "";

                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxerEmailUpdateEmail?SenderName=" + data.FirstName + " " + data.MiddleName + " " + data.LastName
                    + "&UpdatedPara= phone number");
                mail.SendMail(data.Email, "Your phone Number has been Updated", body);
            }
            return RedirectToAction("FaxerInformation", "FaxerAccountProfile");
        }
        [HttpPost]
        public ActionResult UpdateImageUpload()
        {

            string CardUrl = "";
            string fileName = "";
            if (Request.Files.Count > 0)
            {

                var upload = Request.Files[0];

                string directory = Server.MapPath("/Documents");
                fileName = "";
                if (upload != null && upload.ContentLength > 0)
                {
                    fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1]; //Path.GetFileName(upload.FileName);
                    upload.SaveAs(Path.Combine(directory, fileName));
                    CardUrl = "/Documents/" + fileName;
                }
                if (fileName == "")
                {


                    TempData["ChooseCard"] = "Please choose a file to uplaod ";

                    return RedirectToAction("FaxerInformation", "FaxerAccountProfile");
                }
                else
                {

                    int FaxerId = Common.FaxerSession.LoggedUser.Id;
                    var data = context.FaxerInformation.Find(FaxerId);
                    string OldValue = data.CardUrl;
                    data.CardUrl = CardUrl;
                    context.SaveChanges();



                    // Account Profile Update log 
                    UpdateLogHistory("CardUrl", "FaxerInformation", OldValue, data.CardUrl);
                    MailCommon mail = new MailCommon();
                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    string body = "";

                    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxerEmailUpdateEmail?SenderName=" + data.FirstName + " " + data.MiddleName + " " + data.LastName
                        + "&UpdatedPara= ID Card Photo ");
                    mail.SendMail(data.Email, "ID Card Photo has been Updated", body);
                }
            }

            return RedirectToAction("FaxerInformation", "FaxerAccountProfile");
        }

        public ActionResult UpdateFaxerEmail(string Email, int Id)
        {
            if (Id != 0)
            {
                string OldEmail = "";
                var data = context.FaxerInformation.Find(Id);
                OldEmail = data.Email;
                data.Email = Email;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();

                var login = context.FaxerLogin.Where(x => x.FaxerId == data.Id).FirstOrDefault();
                login.UserName = Email;
                context.Entry(login).State = EntityState.Modified;
                context.SaveChanges();
                FaxerSession.LoggedUserName = Email;





                // Account Profile Update log 
                UpdateLogHistory("Email", "FaxerInformation", OldEmail, data.Email);

                UpdateLogHistory("UserName", "FaxerLogin", OldEmail, data.Email);

                MailCommon mail = new MailCommon();

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string body = "";

                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxerEmailUpdateEmail?SenderName=" + data.FirstName + " " + data.MiddleName + " " + data.LastName
                    + "&UpdatedPara= Email Address");
                mail.SendMail(OldEmail, "Email Address Updated", body);

            }
            return RedirectToAction("FaxerInformation", "FaxerAccountProfile");
        }

        public ActionResult RegisteredTopUpCards()
        {
            int faxerId = FaxerSession.LoggedUser.Id;
            List<Models.RegisteredMoneyFaxTopUpCardsViewModel> data = new List<Models.RegisteredMoneyFaxTopUpCardsViewModel>();
            data = (from c in context.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false).ToList()
                    join d in context.SenderKiiPayPersonalAccount on c.Id equals d.KiiPayPersonalWalletId
                    select new Models.RegisteredMoneyFaxTopUpCardsViewModel()
                    {
                        Id = c.Id,
                        CardUsersName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                        MoneyFaxTopUpCard = c.MobileNo.Decrypt().GetVirtualAccountNo()
                    }).ToList();
            Common.FaxerSession.BackButtonURL = Request.Url.ToString();

            return View(data);
        }

        [HttpPost]
        public ActionResult DeleteRegisteredTopUpCards(int MFTCId)
        {
            if (MFTCId != 0)
            {
                var MFTCCard = context.KiiPayPersonalWalletInformation.Find(MFTCId);
                MFTCCard.IsDeleted = true;
                MFTCCard.CardStatus = CardStatus.IsDeleted;
                context.Entry(MFTCCard).State = EntityState.Modified;
                context.SaveChanges();


                DeletedMFTCCards data1 = new DeletedMFTCCards()
                {
                    MoblieNumber = MFTCCard.MobileNo,
                    FaxerId = FaxerSession.LoggedUser.Id,
                    DeletedBy = 0,
                    Date = DateTime.Now.Date,
                    Time = DateTime.Now.TimeOfDay
                };
                context.DeletedMFTCCards.Add(data1);
                context.SaveChanges();



                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                string body = "";
                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                string RegisterMFTCLink = baseUrl + "/TopUpRegestration/UserContactDetails";
                string CardUserCountry = Common.Common.GetCountryName(MFTCCard.CardUserCountry);
                string CardUserCurrency = Common.Common.GetCountryCurrency(MFTCCard.CardUserCountry);
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardDeletionEmail?FaxerName=" + FaxerName +
                        "&MFTCCardNumber=" + MFTCCard.MobileNo.Decrypt() + "&CardUserName=" + MFTCCard.FirstName + " " + MFTCCard.MiddleName + " " + MFTCCard.LastName +
                       "&CardUserDOB=" + MFTCCard.CardUserDOB.ToString("dd/MM/yyyy") + "&CardUserPhoneNumber=" + MFTCCard.CardUserTel
                       + "&CardUserEmailAddress=" + MFTCCard.CardUserEmail +
                       "&CardUserCountry=" + CardUserCountry + "&CardUserCity=" + MFTCCard.CardUserCity +
                       "&RegisterMFTC=" + RegisterMFTCLink);
                mail.SendMail(FaxerEmail, "MoneyFax Top-Up Card - Deletion", body);

                string body2 = "";
                body2 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardDeletion?NameOfDeleter" + FaxerName + "&MFTCNo=" + MFTCCard.MobileNo.Decrypt() +
                    "&NameOnMFTCard=" + MFTCCard.FirstName + "&DOBOfCardUser=" + MFTCCard.CardUserDOB.ToString("dd/MM/yyy") + "&CardUserHistory=" + CardUserCountry +
                    "&CardUserCity=" + MFTCCard.CardUserCity + "&AmountOnCard=" + MFTCCard.CurrentBalance + " " + CardUserCurrency);

                string MFTCCardNumber = "(" + MFTCCard.MobileNo.Decrypt() + ")";
                //mail.SendMail("mftcdelete@moneyfex.com", "Alert - MFTC  " + MFTCCardNumber + "DELETED", body2);

                mail.SendMail("vadeleted@moneyfex.com", "Alert - MFTC  " + MFTCCardNumber + "DELETED", body2);


                return RedirectToAction("RegisteredTopUpCards", "FaxerAccountProfile");
            }
            return View();
        }


        public JsonResult SendVerificationCode()
        {

            string Email = Common.FaxerSession.LoggedUser.UserName;

            Services.SFaxerSignUp service = new Services.SFaxerSignUp();
            var faxerInformation = service.GetInformation(Common.FaxerSession.LoggedUserName);
            MailCommon mailCommon = new MailCommon();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string VerificationCode = Common.Common.GenerateRandomDigit(8);

            Common.FaxerSession.AccountVerificationCode = VerificationCode;
            //string body = "Your Account verification code" + VerificationCode;

            //mailCommon.SendMail(Email, "Moneyfex Acount Verification Code", body);

            SmsApi smsApiServices = new SmsApi();
            var message = smsApiServices.GetAddressUpdateMessage(VerificationCode);
            string phoneNo = Common.Common.GetCountryName(faxerInformation.Country) + "" + faxerInformation.PhoneNumber;
            smsApiServices.SendSMS(phoneNo, message);


            TempData["AccountVerificationCodeSend"] = true;

            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VerifyAccountToEdit(string verificationCode)
        {


            bool AccountVerified = false;
            if (Common.FaxerSession.AccountVerificationCode == verificationCode.Trim())
            {

                // when verification code entered by user is valid the temp value 1
                AccountVerified = true;
                Common.FaxerSession.UserEnterAccountVerficationCode = verificationCode;
            }

            return Json(new
            {
                AccountVerified = AccountVerified
            }, JsonRequestBehavior.AllowGet
            );
        }
        [HttpGet]
        public ActionResult UpdateRegisteredTopUpCards(int MFTCId)
        {
            if (MFTCId == 0)
            {

                TempData["NoCardSelected"] = "Please a select virtual account to update";
                return RedirectToAction("DetailsOnMoneyFaxTopUpCard", "FaxerMoneyFaxTopUpCard");
            }
            if (Request.UrlReferrer != null)
            {

                Common.FaxerSession.BackButtonURL = Request.UrlReferrer.ToString();
            }
            if (MFTCId != 0)
            {
                MFTCCardInformationUpdateViewModel vm = new MFTCCardInformationUpdateViewModel();
                var data = (from c in context.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCId).ToList()
                            join d in context.Country on c.CardUserCountry equals d.CountryCode
                            select new MFTCCardInformationUpdateViewModel()
                            {
                                Id = c.Id,
                                TopUpUserFirstName = c.FirstName,
                                TopUpUserMiddleName = c.MiddleName,
                                TopUpUserLastName = c.LastName,
                                TopUpUserDateOfBirth = c.CardUserDOB.ToString("MM/dd/yyyy") + " " + "(MM/dd/yyyy)",
                                Address1 = c.Address1,
                                Address2 = c.Address2,
                                State = c.CardUserState,
                                PostalCode = c.CardUserPostalCode,
                                City = c.CardUserCity,
                                Country = d.CountryName,
                                PhoneNumber = c.CardUserTel,
                                EmailAddress = c.CardUserEmail
                            }).FirstOrDefault();

                ViewBag.MFTCCardId = data.Id;
                return View(data);

            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateRegisteredTopUpCards([Bind(Include = MFTCCardInformationUpdateViewModel.BindProperty)] MFTCCardInformationUpdateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var data = context.KiiPayPersonalWalletInformation.Where(x => x.Id == vm.Id).FirstOrDefault();
                data.Address1 = vm.Address1;
                data.Address2 = vm.Address2;
                data.CardUserState = vm.State;
                data.CardUserPostalCode = vm.PostalCode;
                data.CardUserCity = vm.City;
                data.CardUserTel = vm.PhoneNumber;
                data.CardUserEmail = vm.EmailAddress;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();

                var CardUserInformation = context.KiiPayPersonalUserInformation.Where(x => x.KiiPayPersonalWalletInformationId == vm.Id).FirstOrDefault();

                if (CardUserInformation != null)
                {
                    CardUserInformation.EmailAddress = vm.EmailAddress;
                    context.Entry(CardUserInformation).State = EntityState.Modified;


                    var CardUserLogin = context.KiiPayPersonalUserLogin.Where(x => x.KiiPayPersonalUserInformationId == CardUserInformation.Id).FirstOrDefault();
                    CardUserLogin.Email = vm.EmailAddress;
                    context.Entry(CardUserLogin).State = EntityState.Modified;
                    context.SaveChanges();
                }
                MailCommon mail = new MailCommon();

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                //string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/CardUserInformationUpdatedEmail?FaxerName="
                //    + data.FaxerInformation.FirstName + " " + data.FaxerInformation.MiddleName + " " + data.FaxerInformation.LastName +
                //    "&CardUserName=" + data.FirstName + " " + data.MiddleName + " " + data.LastName);

                //mail.SendMail(data.FaxerInformation.Email, "Details on MoneyFex Card update has been updated", body);
                //mail.SendMail(data.CardUserEmail, "Details on MoneyFex Card update has been updated", body);

                //mail.SendMail("anankarki97@gmail.com", "Details on MoneyFex Card update has been updated", body);

                return RedirectToAction("RegisteredTopUpCardsUpdateSuccessfullyMessage", "FaxerAccountProfile");
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult RegisteredTopUpCardsUpdateSuccessfullyMessage()
        {
            return View();
        }

        public ActionResult SavedCreditDebitCard()
        {
            var data = _faxerAccountProfile.GetSavedCreditDebitCards();

            ViewBag.CardCount = data.Count();
            //if (data.Count == 0)
            //{
            //    TempData["CreditDebitCardCount"] = 0;
            //    return RedirectToAction("Index", "SenderDashBoard");
            //}
            //else
            //{
            //    TempData["CreditDebitCardCount"] = data.Count;
            //}

            return View(data);
        }

        public ActionResult AddADebitCreditCard()
        {
            if (FaxerSession.LoggedUser == null)
            {

                var Url = Request.Url.ToString();
                string[] tokens = Url.Split('/');
                Common.FaxerSession.FromUrl = "/" + tokens[3] + "/" + tokens[4];

                return RedirectToAction("Login", "FaxerAccount");

            }
            var data = _faxerAccountProfile.GetSavedCreditDebitCards();
            ViewBag.Countries = new SelectList(context.Country.ToList(), "CountryCode", "CountryName");
            SenderAddMoneyCardViewModel vm = new SenderAddMoneyCardViewModel();
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            vm.Address = senderCommonFunc.GetSenderAddress();
            return View(vm);
        }


        [HttpPost]
        public ActionResult AddADebitCreditCard([Bind(Include = SenderAddMoneyCardViewModel.BindProperty)]SenderAddMoneyCardViewModel model)
        {
            bool valid = true;
            Services.SFaxerSignUp faService = new Services.SFaxerSignUp();

            ViewBag.Countries = new SelectList(context.Country.ToList(), "CountryCode", "CountryName");

            //var countryCode = context.Country.Where(x => x.CountryName.ToLower() == model.County.ToLower()).FirstOrDefault();
            //if (countryCode == null)
            //{
            //    model.County = "";
            //}
            //else
            //    model.County = countryCode.CountryCode;
            var result = Common.Common.ValidationOfSenderMoneyCardCard(model);
            if (result.Data == false)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
            var faInformation = faService.GetInformation(FaxerSession.LoggedUser.UserName);
            var CurrentYearToString = DateTime.Now.Year.ToString();
            var CurrentYear = Convert.ToInt32(CurrentYearToString.Substring(2, 2));
            var CurrentMonth = DateTime.Now.Month;
            string NameOfFaxer = Regex.Replace(faService.GetFaxerNameByEmail(FaxerSession.LoggedUser.UserName), @"\s+", "");

            //string CardName = Regex.Replace(model.CardHolderName, @"\s+", "");
            //Regex regex = new Regex(@"^\d$");

            //var c  = regex.Match(model.Num);
            //if (c.Success == false) {

            //    ModelState.AddModelError("Num", "Please enter a valid credit card number");
            //    valid = false;

            //}

            //string[] splittedDate = model.ExpiringDateYear.Split('-');
            //model.ExpiringDateMonth = splittedDate[1];
            //model.ExpiringDateYear = splittedDate[0];

            //if (CardName.ToLower() != NameOfFaxer.ToLower())
            //{

            //    ModelState.AddModelError("CardName", "Please enter your account registered name ");
            //    valid = false;
            //}
            //if (Convert.ToInt32(model.ExpiringDateMonth) > 12 || Convert.ToInt32(model.ExpiringDateMonth) < 0)
            //{

            //    ModelState.AddModelError("EMonth", "Please enter a valid month");
            //    valid = false;
            //}
            //if (Convert.ToInt32(model.ExpiringDateYear) < CurrentYear)
            //{

            //    ModelState.AddModelError("EYear", "Your Card Has been expired");
            //    valid = false;
            //}
            //if ((Convert.ToInt32(model.ExpiringDateYear) == CurrentYear) && Convert.ToInt32(model.ExpiringDateMonth) < CurrentMonth)
            //{
            //    ModelState.AddModelError("EYear", "Your Card has been expired");
            //    valid = false;
            //}
            //if (model.Address.ToLower() != faInformation.Address1.ToLower())
            //{
            //    ModelState.AddModelError("Address", "Address line one does not match with your registered address,");
            //    valid = false;
            //}

            #region  Strip portion
            var DebitCreditNumber = model.CardNumber.Split(' ');
            model.CardNumber = string.Join("", DebitCreditNumber);
            StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");
            StripeResultIsValidCardVm StripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                Number = model.CardNumber,
                ExpirationMonth = model.ExpiringDateMonth,
                ExpiringYear = model.ExpiringDateYear,
                SecurityCode = model.SecurityCode,
                billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                CurrencyCode = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode)

            };

            var Striperesult = StripServices.IsValidCardNo(StripeResultIsValidCardVm);

            if (Striperesult.IsValid == false)
            {
                ModelState.AddModelError("ErrorMessage", Striperesult.Message);
                valid = false;
            }

            //var stripeTokenCreateOptions = new StripeTokenCreateOptions
            //{
            //    Card = new StripeCreditCardOptions
            //    {
            //        Number = model.CardNumber,
            //        ExpirationMonth = int.Parse(model.ExpiringDateMonth),
            //        ExpirationYear = int.Parse(model.ExpiringDateYear),
            //        Cvc = model.SecurityCode,
            //        Name = NameOfFaxer
            //    }
            //};

            //string token = "";
            //var tokenService = new StripeTokenService();

            //StripeResponse stripeResponse = new StripeResponse();
            //try
            //{
            //    var stripeToken = tokenService.Create(stripeTokenCreateOptions);

            //    token = stripeToken.Id;
            //}
            //catch (Exception ex)
            //{

            //    ModelState.AddModelError("ErrorMessage", ex.Message);
            //    valid = false;

            //    //Log.Write("Merchant Auto Payment Exception : " + ex.Message);
            //}
            #endregion
            if (valid == true)
            {
                var savedCard = _faxerAccountProfile.AddCard(model);
                //if (savedCard != null)
                //{
                //    MailCommon mail = new MailCommon();
                //    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                //    string body = "";
                //    string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                //    string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                //    string CardNumber = "xxxx-xxxx-xxxx-" + savedCard.Num.Decrypt().Right(4);
                //    string TopUpMoneyfaxCard = baseUrl + "/StartFaxingMoney/Index?mFTCCardInformationId=";
                //    string PayForGoodsAbroad = baseUrl + "/PayForGoodsAndServicesAbroad/MerchantAccountNumber";
                //    string SetAutoTopUp = baseUrl + "/FaxerAutoPayments/MoneyFaxCardAutoTopUp?mftcCardInformationId=";

                //    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/NewCreditDebitCardAddedEmail?FaxerName=" + FaxerName
                //           + "&LastForDigitOfCreditOrDebitCard=" + CardNumber + "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard
                //           + "&SetAutoTopUp=" + SetAutoTopUp + "&PayForGoodsAbroad=" + PayForGoodsAbroad);
                //    mail.SendMail(FaxerEmail, "New Credit/Debit Card Added ", body);
                //    if (FaxerSession.FromUrl == "/CardPayment/TopUpUsingSavedCreditDebit")
                //    {
                //        return RedirectToAction("TopUpUsingSavedCreditDebit", "CardPayment");
                //    }
                //    else if (FaxerSession.FromUrl == "/CardPayment/NonCardReceiverTransferUsingSavedCreditDebitCard")
                //    {
                //        return RedirectToAction("NonCardReceiverTransferUsingSavedCreditDebitCard", "CardPayment");
                //    }
                //    else if (FaxerSession.FromUrl == "/CardPayment/MerchantPaymentUsingSavedCreditDebitCard")
                //    {
                //        return RedirectToAction("MerchantPaymentUsingSavedCreditDebitCard", "CardPayment");
                //    }
                //    else if (FaxerSession.FromUrl == "/TopUpSomeoneElseMFTCCard/TopUpSomeoneElseCardUsingSavedCreditDebitCard")
                //    {
                //        return RedirectToAction("TopUpSomeoneElseCardUsingSavedCreditDebitCard", "TopUpSomeoneElseMFTCCard");
                //    }
                //    else if (FaxerSession.FromUrl == "/CardPayment/ReceiverTransferUsingSavedCreditDebitCard")
                //    {

                //        return RedirectToAction("ReceiverTransferUsingSavedCreditDebitCard", "CardPayment");

                //    }

                //    return RedirectToAction("SaveCardMessage", "FaxerAccountProfile");
                //}
                //else
                //{
                //    return RedirectToAction("OutOfLimit", "FaxerAccountProfile");
                //}
                return RedirectToAction("SaveCardMessage", "FaxerAccountProfile");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteDebitCreditCard(int CardId)
        {
            if (CardId != 0)
            {
                var card = _faxerAccountProfile.RemoveCard(CardId);


                //MailCommon mail = new MailCommon();
                //var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                //string body = "";
                //string FaxerName = Common.FaxerSession.LoggedUser.FirstName;
                //string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                //string CardNumber = "xxxx-xxxx-xxxx-" + card.Num.Decrypt().Right(4);
                //string AddNewCreditOrDebitCard = baseUrl + "/FaxerAccountProfile/AddADebitCreditCard";
                //body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/CreditDebitCardDeletionEmail?FaxerName="
                //    + FaxerName + "&CreditDebitCardLast4Digits=" + CardNumber + "&AddNewCreditOrDebitCard=" + AddNewCreditOrDebitCard);
                //mail.SendMail(FaxerEmail, "Credit/Debit card deleted", body);

                Areas.Admin.Services.EmailServices _emailServices = new Areas.Admin.Services.EmailServices();
                _emailServices.CardDeletionEmail(card);
                return RedirectToAction("SavedCreditDebitCard", "FaxerAccountProfile");
            }
            return View();
        }



        [HttpGet]
        public ActionResult UpdateDebitCreditCard(int CardId)
        {
            if (FaxerSession.LoggedUser == null)
            {

                var Url = Request.Url.ToString();
                string[] tokens = Url.Split('/');
                Common.FaxerSession.FromUrl = "/" + tokens[3] + "/" + tokens[4];

                return RedirectToAction("Login", "FaxerAccount");

            }


            var updatedata = _faxerAccountProfile.List(CardId);
            SenderAddMoneyCardViewModel vm = new SenderAddMoneyCardViewModel();
            if (updatedata != null)
            {
                vm.CardNumber = updatedata.CardNo;
                vm.ExpiringDateMonth = updatedata.EndMonth;
                vm.ExpiringDateYear = updatedata.EndYear;
                vm.SecurityCode = updatedata.SecurityCode;
                vm.SelectCard = updatedata.CardType;
            }


            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateDebitCreditCard([Bind(Include = SenderAddMoneyCardViewModel.BindProperty)]SenderAddMoneyCardViewModel vm, int cardId)
        {
            #region  Strip portion
            StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");
            var result = Common.Common.ValidationOfSenderMoneyCardCard(vm);
            if (result.Data == false)
            {
                ModelState.AddModelError("", result.Message);
                return View(vm);
            }

            var stripeTokenCreateOptions = new StripeTokenCreateOptions
            {
                Card = new StripeCreditCardOptions
                {
                    Number = vm.CardNumber,
                    ExpirationMonth = int.Parse(vm.ExpiringDateMonth),
                    ExpirationYear = int.Parse(vm.ExpiringDateYear),
                    Cvc = vm.SecurityCode,
                }
            };

            string token = "";
            var tokenService = new StripeTokenService();

            StripeResponse stripeResponse = new StripeResponse();
            try
            {
                var stripeToken = tokenService.Create(stripeTokenCreateOptions);

                token = stripeToken.Id;
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("SavedCreditDebitCard", "FaxerAccountProfile");
                //Log.Write("Merchant Auto Payment Exception : " + ex.Message);
            }
            #endregion

            if (ModelState.IsValid)
            {
                //DB.FAXEREntities db = new DB.FAXEREntities();
                //var data = context.SavedCard.Find(Id);
                //data.Num = CardNumber.Encrypt();
                //data.EMonth = Month.Encrypt();
                //data.EYear = Year.Encrypt();
                //data.ClientCode = SecurityCode.Encrypt();

                //db.Entry(data).State = EntityState.Modified;
                //db.SaveChanges();
                var data = _faxerAccountProfile.UpdateCard(vm, cardId);
                return RedirectToAction("SavedCreditDebitCard", "FaxerAccountProfile");
            }
            return View();
        }

        public ActionResult SaveCardMessage()
        {
            return View();
        }

        public ActionResult OutOfLimit()
        {
            return View();
        }


        private bool UpdateLogHistory(string ColumnName, string TableName, string OldValue, string NewValue)
        {

            FaxerAccountProfileUpdatedLogHistory updatedLogHistory = new FaxerAccountProfileUpdatedLogHistory()
            {

                UpdateByEmailAddress = Common.FaxerSession.LoggedUser.UserName,
                UpdatedById = Common.FaxerSession.LoggedUser.Id,
                UpdatedColumn = ColumnName,
                TableName = TableName,
                UpdatedDateTime = DateTime.Now,
                OldValue = OldValue,
                NewValue = NewValue,

            };

            context.FaxerAccountProfileUpdatedLogHistory.Add(updatedLogHistory);
            context.SaveChanges();
            return true;
        }


    }
}