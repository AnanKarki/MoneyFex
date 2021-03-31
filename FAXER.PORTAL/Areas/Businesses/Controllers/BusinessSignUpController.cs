using FAXER.PORTAL.Areas.Businesses.Services;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class BusinessSignUpController : Controller
    {
        // GET: Businesses/BusinessSignUp
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult BusinessDetails(string RegistrationNumber)
        {
            Services.BusinessSignUpServices signUpServices = new BusinessSignUpServices();
            var Merchantdetails = signUpServices.GetBecomeAMerchantInformation(RegistrationNumber);
            var model = new ViewModels.BusinessDetailsViewModel();
            model.BusinessName = Merchantdetails.CompanyBusinessName;
            model.BusinessLicenseNumber = Merchantdetails.BusinessLicenseRegistrationNumber;
            model.EmailAddress = Merchantdetails.BusinessEmailAddress;
            model.NameOfContactPerson = Merchantdetails.FirstName + " " + Merchantdetails.LastName;
            model.RegistrationNumber = Merchantdetails.RegistrationNumber;
            return View(model);
        }
        [HttpPost]

        public ActionResult BusinessDetails([Bind(Include = ViewModels.BusinessDetailsViewModel.BindProperty)]ViewModels.BusinessDetailsViewModel model)
        {

            if (ModelState.IsValid)
            {

                Services.BusinessSignUpServices services = new BusinessSignUpServices();
                var emailExist = services.GetBusinessInformation(model.EmailAddress);
                if (emailExist != null)
                {
                    ModelState.AddModelError("EmailAddress", "Email Address Already Exist");
                    return View(model);
                }

                Common.BusinessSession.BusinessDetails = model;

                Session["BusinessName"] = model.BusinessName;
                Session["MiddleName"] = model.MiddleName;
                Session["BusinessLicenseNumber"] = model.BusinessLicenseNumber;
                Session["EmailAddress"] = model.EmailAddress;
                Session["NameOfContactPerson"] = model.NameOfContactPerson;
                Session["RegistrationNumber"] = model.RegistrationNumber;
                return RedirectToAction("ContactDetails");


            }

            return View(model);
        }
        [HttpGet]
        public ActionResult ContactDetails()
        {
            if (Common.BusinessSession.BusinessDetails == null)
            {

                return RedirectToAction("BusinessDetails");
            }
            CommonServices common = new CommonServices();
            var countries = common.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            Services.BusinessSignUpServices signUpServices = new BusinessSignUpServices();
            var Merchantdetails = signUpServices.GetBecomeAMerchantInformation(Common.BusinessSession.BusinessDetails.RegistrationNumber);
            var model = new ViewModels.BusinessContactDetailsViewModel();
            model.Address1 = Merchantdetails.Address1;
            model.Address2 = Merchantdetails.Address2;
            model.City = Merchantdetails.City;
            model.Country = Merchantdetails.CountryCode;
            model.FaxNo = Merchantdetails.FaxNo;
            model.PhoneNumber = Merchantdetails.ContactPhone;
            model.PostalCode = Merchantdetails.PostZipCode;
            model.State = Merchantdetails.StateProvince;
            model.Website = Merchantdetails.Website;

            return View(model);
        }
        [HttpPost]
        public ActionResult ContactDetails([Bind(Include = ViewModels.BusinessContactDetailsViewModel.BindProperty)]ViewModels.BusinessContactDetailsViewModel model)
        {

            CommonServices common = new CommonServices();
            var countries = common.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            if (model.IsConfirmed == false)
            {
                ModelState.AddModelError("IsConfirmed", "Please Confirm the Check box");
                return View(model);

            }

            if (ModelState.IsValid)
            {
                Services.BusinessSignUpServices signUpServices = new Services.BusinessSignUpServices();
                var businessDetails = Common.BusinessSession.BusinessDetails;
                var MFSCode = signUpServices.GetMFSCode(10);
                DB.KiiPayBusinessInformation business = new DB.KiiPayBusinessInformation()
                {
                    BusinessName = businessDetails.BusinessName,
                    RegistrationNumBer = businessDetails.RegistrationNumber,
                    BusinessLicenseNumber = businessDetails.BusinessLicenseNumber,
                    Email = businessDetails.EmailAddress,
                    BusinessOperationAddress1 = model.Address1,
                    BusinessOperationAddress2 = model.Address2,
                    BusinessMobileNo = MFSCode,
                    ContactPerson = businessDetails.NameOfContactPerson,
                    BusinessOperationCity = model.City,
                    BusinessOperationState = model.State,
                    BusinessOperationPostalCode = model.PostalCode,
                    PhoneNumber = model.PhoneNumber,
                    Website = model.Website,
                    FaxNumber = model.FaxNo,
                    BusinessOperationCountryCode = model.Country


                };
                var result = signUpServices.SaveBusinessInformation(business);
                if (result != null)
                {
                    var guId = Guid.NewGuid().ToString();
                    var LoginCode = signUpServices.GetNewLoginCode(6);
                    DB.KiiPayBusinessLogin businessLogin = new DB.KiiPayBusinessLogin()
                    {

                        Username = businessDetails.EmailAddress,
                        Password = null,
                        LoginCode = LoginCode,
                        ActivationCode = guId,
                        KiiPayBusinessInformationId = result.Id,
                        LoginFailCount = 0,
                        IsActive = false,
                    };
                    var Loginresult = signUpServices.SaveBusinessLogin(businessLogin);
                    if (Loginresult != null)
                    {

                        MailCommon mail = new MailCommon();
                        var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                        var link = string.Format("{0}/Businesses/BusinessSignUp/FirstLogin?ActivationCode={1}", baseUrl, Loginresult.ActivationCode);
                        try
                        {
                            string body = "";
                            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BusinessMerchantActivationEmail?NameOfContactPerson=" + business.ContactPerson + "&EmailAddress=" + business.Email + "&MerchantLoginCode=" + businessLogin.LoginCode + "&Link=" + link);

                            mail.SendMail(business.Email, "Business Merchant Registration Confirmation ", body);

                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        return RedirectToAction("RegistrationMessage");
                    }
                }
            }
            return View(model);

        }
        public ActionResult RegistrationMessage()
        {
            Session.Clear();
            Session.Abandon();
            return View();
        }
        [HttpGet]
        public ActionResult FirstLogin(string ActivationCode)
        {
            Services.BusinessSignUpServices services = new BusinessSignUpServices();

            var LoginData = services.GetBusinessLoginDetails(ActivationCode);
            if (LoginData != null)
            {
                ViewModels.BusinessLoginViewModel model = new ViewModels.BusinessLoginViewModel();
                model.EmailAddress = LoginData.Username;
                model.LoginCode = LoginData.LoginCode;
                return View(model);
            }
            return View();
        }
        [HttpPost]
        public ActionResult FirstLogin([Bind(Include = ViewModels.BusinessLoginViewModel.BindProperty)]ViewModels.BusinessLoginViewModel model)
        {

            Services.BusinessSignUpServices services = new BusinessSignUpServices();

            if (string.IsNullOrEmpty(model.EmailAddress))
            {
                ModelState.AddModelError("EmailAddress", "The Field is required");
                return View(model);

            }
            if (string.IsNullOrEmpty(model.LoginCode))
            {
                ModelState.AddModelError("LoginCode", "The Field is required");
                return View(model);

            }

            if (string.IsNullOrEmpty(model.password))
            {

                ModelState.AddModelError("password", "The Field is required");
                return View(model);

            }

            if (string.IsNullOrEmpty(model.ConfirmPassword))
            {
                ModelState.AddModelError("ConfirmPassword", "The Field is required");
                return View(model);
            }


            if (Common.Common.ValidatePassword(model.password))
            {
                if (model.password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password Did Not Match");
                    return View(model);
                }

                var LoginInformation = services.IsExist(model.EmailAddress);
                if ((LoginInformation != null) && model.LoginCode == LoginInformation.LoginCode)
                {


                    var result = services.UpdateBusinessLogin(model);
                    if (result == true)
                    {
                        return RedirectToAction("Login", "BusinessLogin");
                    }
                    else
                    {

                        ModelState.AddModelError("OldPassword", "The password is incorrect");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("ConfirmPassword", "Your Login Credential does not match");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("password", "Password Should Contain Atleast 1 UpperCase, 1 number and 1 Special Character");
                return View(model);
            }
            
        }
    }
}