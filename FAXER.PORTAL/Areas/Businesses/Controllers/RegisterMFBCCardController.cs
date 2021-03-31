using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class RegisterMFBCCardController : Controller
    {
        // GET: Businesses/RegisterMFTCCard
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult RegisterBusinessCard() {
            if (Common.BusinessSession.LoggedBusinessMerchant == null)
            {
                var Url = Request.Url.ToString();
                string[] tokens = Url.Split('/');
                Common.BusinessSession.FormURL = "/" + tokens[3] + "/" + tokens[4] + "/" + tokens[5];

                return RedirectToAction("Login", "BusinessLogin");
            }
            Services.BusinessCardServices cardServices = new Services.BusinessCardServices();
            var CardInformation = cardServices.GetMFBCCardInformation(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId);
            ViewModels.MFBCCardUserDetailsViewModel model = new ViewModels.MFBCCardUserDetailsViewModel();

            if (CardInformation != null)
            {


                ModelState.AddModelError("DateOfBirth", "You are only allowed to register one card");
                model.CardId = CardInformation.Id;

            }
            else
            {
                model.CardId = 0;
            }
            return View(model);

        }
        [HttpPost]
        public ActionResult RegisterBusinessCard([Bind(Include = ViewModels.MFBCCardUserDetailsViewModel.BindProperty)]ViewModels.MFBCCardUserDetailsViewModel model)
        {
            //Services.BusinessCardServices cardServices = new Services.BusinessCardServices();
            Services.BusinessCardServices cardServices = new Services.BusinessCardServices();
            var CardInformation = cardServices.GetMFBCCardInformation(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId);
            if (CardInformation != null)
            {

                ModelState.AddModelError("DateOfBirth", "You are only allowed to register one card");
                return View();
            }
            if (ModelState.IsValid) {
                if (model.Gender == null ) {

                    ModelState.AddModelError("Gender", "Choose Your Gender");
                    return View(model);
                }
                if (model.DateOfBirth == default(DateTime)) {
                    ModelState.AddModelError("DateOfBirth", "Please Enter a Valid Date");
                    return View(model);
                }

                bool isValidAge = Common.DateUtilities.ValidateAge(model.DateOfBirth);
                if (isValidAge == false)
                {
                    ModelState.AddModelError("DateOfBirth", "You must be 18 years of age or above to sign up to our services");
                    return View(model);
                }
               
                Common.BusinessSession.BusinessCardUserDetails = model;
                Session["FirstName"] = model.FirstName;
                Session["MiddleName"] = model.MiddleName;
                Session["LastName"] = model.LastName;
                Session["gander"] = model.Gender;
                Session["DateOdBirth"] = model.DateOfBirth;
                return RedirectToAction("CardUserContactDetails");
            }
            return View();

        }
        [HttpGet]
        public ActionResult CardUserContactDetails() {
            if (Common.BusinessSession.BusinessCardUserDetails != null)
            {
                Services.CommonServices common = new Services.CommonServices();
                var countries = common.GetCountries();
                ViewBag.Countries = new SelectList(countries, "Code", "Name");
                ViewModels.MFBCCardUserContactDetailsViewModel vm = new ViewModels.MFBCCardUserContactDetailsViewModel();
                vm.Country = Common.BusinessSession.LoggedBusinessMerchant.CountryCode;
                vm.CountryName = Common.BusinessSession.LoggedBusinessMerchant.CountryCode;
                return View(vm);
            }
            return RedirectToAction("RegisterBusinessCard");
        }
        [HttpPost]
        public ActionResult CardUserContactDetails([Bind(Include = ViewModels.MFBCCardUserContactDetailsViewModel.BindProperty)]ViewModels.MFBCCardUserContactDetailsViewModel model)
        {
            Services.CommonServices common = new Services.CommonServices();
            var countries = common.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (ModelState.IsValid) {

                Services.BusinessCardServices cardServices = new Services.BusinessCardServices();
                bool EmailExist = cardServices.EmailExist(model.EmailAddress);
                if (EmailExist == true) {
                    ModelState.AddModelError("EmailAddress", "Email Already Exist");
                    return View(model);
                }
                Common.BusinessSession.BusinessCardUserContactDetails = model;
                Session["Address1"] = model.Address1;
                Session["Address2"] = model.Address2;
                Session["City"] = model.City;
                Session["Country"] = model.Country;
                Session["State"] = model.State;
                Session["PostalCode"] = model.PostalCode;
                Session["PhoneNumber"] = model.PhoneNumber;
                Session["EmailAddress"] = model.EmailAddress;

                return RedirectToAction("CardUserIdentification");
            }
            
            return View(model); ;
        }
        [HttpGet]
        public ActionResult CardUserIdentification() {
            if (Common.BusinessSession.BusinessCardUserContactDetails == null)
            {

                return RedirectToAction("CardUserContactDetails");
            }
            return View();
        }
        [HttpPost]
        public ActionResult CardUserIdentification([Bind(Include = ViewModels.MFBCCardUserIdentification.BindProperty)]ViewModels.MFBCCardUserIdentification model)
        {
            var fileName = "";
            if (Request.Files.Count > 0)
            {
                string directory = Server.MapPath("/Documents");
                var upload = Request.Files[0];
                fileName = "";
                if (upload != null && upload.ContentLength > 0)
                {
                    fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1]; //Path.GetFileName(upload.FileName);
                    upload.SaveAs(Server.MapPath("~/Documents") + "\\" + fileName);

                }
            }
            model.CardPhotoURL = "/Documents/" + fileName;

            var CardUserDetails = Common.BusinessSession.BusinessCardUserDetails;
            var CardUserContactDetails = Common.BusinessSession.BusinessCardUserContactDetails;
            Services.BusinessCardServices cardServices = new Services.BusinessCardServices();
            string MFBCNumber = cardServices.GetNewMFBCCardNumber();
            string MFBCCard = MFBCNumber + "-" + CardUserDetails.FirstName.ToUpper();

            DB.KiiPayBusinessWalletInformation mFBCCardInformation = new DB.KiiPayBusinessWalletInformation() {
                FirstName = CardUserDetails.FirstName,
                MiddleName = CardUserDetails.MiddleName,
                LastName = CardUserDetails.LastName,
                Gender = (Gender)CardUserDetails.Gender,
                DOB = CardUserDetails.DateOfBirth,
                AddressLine1 = CardUserContactDetails.Address1,
                AddressLine2 = CardUserContactDetails.Address2,
                City = CardUserContactDetails.City,
                Country = CardUserContactDetails.Country,
                State = CardUserContactDetails.State,
                Email = CardUserContactDetails.EmailAddress,
                PhoneNumber = CardUserContactDetails.PhoneNumber,
                KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId,
                KiiPayUserPhoto = model.CardPhotoURL,
                MFBCardPhoto =model.CardPhotoURL,
                PostalCode = CardUserContactDetails.PostalCode,
                MobileNo = MFBCCard.Encrypt() 
            };
            var result = cardServices.RegisterMFBCCard(mFBCCardInformation);
            Common.BusinessSession.MerchantHasMFBCCard = true;
            SCity.Save(mFBCCardInformation.City, mFBCCardInformation.Country, DB.Module.BusinessMerchant);
            if (result != null) {

                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                try
                {
                    string BusinessName = Common.BusinessSession.LoggedBusinessMerchant.BusinessName;   
                    string body = "";
                    string CardUserCountry = Common.Common.GetCountryName(result.Country);
                    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFBCCardRegistrationEmail?NameofBusinessMerchant=" + BusinessName + "&MFBCCardNumber=" + result.MobileNo.Decrypt() + 
                        "&CardUserName=" + result.FirstName + " " + result.LastName + "&CardUserDOB=" + result.DOB.ToString("dd/MM/yyyy") + 
                        "&CardUserEmailAddress=" + result.Email + "&CardUserCountry=" +  CardUserCountry + "&CardUserCity=" + result.City);

                    //mail.SendMail(business.Email, "Business Merchant Registration Confirmation ", body);

                    mail.SendMail(Common.BusinessSession.LoggedBusinessMerchant.BusinessEmailAddress, "MoneyFax Business Card registration - confirmation", body);
                    //mail.SendMail("anankarki97@gmail.com", "Business Merchant Registration Confirmation ", body);
                    
                    
                }
                catch (Exception)
                {

                    throw;
                }
                return RedirectToAction("MFBCRegistrationConfirmation");

            }
            return View();
        }
        public ActionResult MFBCRegistrationConfirmation() {
            Session.Remove("BusinessCardUserDetails");
            Session.Remove("BusinessCardUserContactDetails");
            return View();
        }
    }
}