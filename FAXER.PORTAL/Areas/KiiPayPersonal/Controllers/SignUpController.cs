using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Controllers
{
    public class SignUpController : Controller
    {
        KiiPayPersonalCommonServices _commonServices = null;
        SignUpServices _services = null;
        public SignUpController()
        {
            _services = new SignUpServices();
            _commonServices = new KiiPayPersonalCommonServices();
        }

        // GET: KiiPayPersonal/SignUp
        public ActionResult Index()
        {
            var vm = new SignUpViewModel();
            vm.CountryPhoneCode = "";
            SetViewBagForCountries();
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = SignUpViewModel.BindProperty)]SignUpViewModel model)
        {
            SetViewBagForCountries();
            if (ModelState.IsValid)
            {
                bool IsValidMobileNo = _services.IsValidMobileNo(model.MobileNumber);
                if (!IsValidMobileNo) {

                    ModelState.AddModelError("MobileNumber", "Mobile no already exists!");
                    return View(model);
                }
                bool emailAlreadyExists = _services.checkIfEmailAddressAlreadyExists(model.EmailAddress);
                if(emailAlreadyExists == true)
                {
                    ModelState.AddModelError("EmailAddress", "An user already exists with this Email Address !");
                    return View(model);
                }
                if(model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords don't match !");
                    return View(model);
                }
                if(model.Password.Length < 6)
                {
                    ModelState.AddModelError("Password", "Password must be at least 6 characters long !");
                    return View(model);
                }
                if(Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel != null)
                {
                    Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel = null;
                }

                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel = new KiiPayPersonalSignUpSessionViewModel()
                {
                    CountryCode = model.CountryCode,
                    PhoneNumber = model.MobileNumber,
                    EmailAddress = model.EmailAddress,
                    Password = model.Password
                };
                return RedirectToAction("PersonalDetails");
            }
            return View(model);
        }


        [HttpGet]
        public ActionResult PersonalDetails()
        {
            ViewBag.Days = new SelectList(Enumerable.Range(1, 32));
            return View();
        }

        [HttpPost]
        public ActionResult PersonalDetails([Bind(Include = PersonalDetailsViewModel.BindProperty)]PersonalDetailsViewModel model)
        {
            ViewBag.Years = new SelectList(Enumerable.Range(1970, 70));
            ViewBag.Days = new SelectList(Enumerable.Range(1, 32));
            if (ModelState.IsValid)
            {
                if(model.BirthDateMonth == (Month)0)
                {
                    ModelState.AddModelError("BirthDateMonth", "Month field is required !");
                    return View(model);
                }
                if(model.BirthDateYear.ToString().Length != 4)
                {
                    ModelState.AddModelError("BirthDateYear", "Please enter valid year !");
                    return View(model);
                }
                string fileName = "";
                string directory = Server.MapPath("/Documents");
                var Image = Request.Files["upload"];

                if (Image != null && Image.ContentLength > 0)
                {
                    fileName = Guid.NewGuid() + "." + Image.FileName.Split('.')[1];
                    Image.SaveAs(Path.Combine(directory, fileName));
                    model.PhotoUrl = "/Documents/" + fileName;
                }
               
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.PhotoUrl = model.PhotoUrl;
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.FirstName = model.FirstName;
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.MiddleName = model.MiddleName;
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.LastName = model.LastName;
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.BirthdateDay = model.BirthDateDay;
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.BirthdateMonth = (int)model.BirthDateMonth;
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.BirthdateYear = model.BirthDateYear;
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.Gender = model.Gender;
                return RedirectToAction("PersonalAddress");
            }
            return View();
        }

        [HttpGet]
        public ActionResult PersonalAddress()
        {
            var vm = new PersonalAddressViewModel()
            {
                CountryCode = Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.CountryCode
            };
            SetViewBagForCountries();
            return View(vm);

        }

        [HttpPost]
        public ActionResult PersonalAddress([Bind(Include = PersonalAddressViewModel.BindProperty)]PersonalAddressViewModel model)
        {
            SetViewBagForCountries();
            if (ModelState.IsValid)
            {
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.PersonalAddressCountry = model.CountryCode;
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.PersonalAddressCity = model.City;
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.PersonalAddressAddress1 = model.Address1;
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.PersonalAddressAddress2 = model.Address2;
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.PersonalAddressPostalCode = model.PostalCode;

                int kiiPayPersonalUserInformationId = _services.AddKiiPayPersonalUser();

                string verificationCode = Common.Common.GenerateVerificationCode(6);
                RegistrationCodeVerificationViewModel regverificationCodeVm = new RegistrationCodeVerificationViewModel()
                {
                    Country = Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.PersonalAddressCountry,
                    UserId = kiiPayPersonalUserInformationId,
                    //IsExpired = false,
                    PhoneNo = Common.Common.GetCountryPhoneCode(Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.PersonalAddressCountry) + " " + Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.PhoneNumber,
                    RegistrationOf = RegistrationOf.KiiPayPersonal,
                    VerificationCode = verificationCode,
                };
                Common.FaxerSession.RegistrationCodeVerificationViewModel = regverificationCodeVm;
                SRegistrationVerificationCode registrationVerificationCodeServices = new SRegistrationVerificationCode();
                registrationVerificationCodeServices.Add(regverificationCodeVm);
                SmsApi smsApiServices = new SmsApi();
                string message = smsApiServices.GetRegistrationMessage(verificationCode);

                smsApiServices.SendSMS(regverificationCodeVm.PhoneNo, message);
                return RedirectToAction("VerificationCode");
            }
            return View(model);

        }

        [HttpGet]
        public ActionResult VerificationCode()
        {
            return View();
        }

        [HttpPost]
        public ActionResult VerificationCode([Bind(Include = VerificationCodeViewModel.BindProperty)]VerificationCodeViewModel model)
        {
            if(ModelState.IsValid)
            {
 
                string verificationCodeString = model.Code1.ToString() + model.Code2.ToString() + model.Code3.ToString() + model.Code4.ToString() + model.Code5.ToString() + model.Code6.ToString(); 
                var data = new RegistrationCodeVerificationViewModel()
                {
                    UserId = Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.UserId,
                    PhoneNo = Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.PhoneNumber,
                    Country = Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.CountryCode,
                    Message = "",
                    RegistrationOf = RegistrationOf.KiiPayPersonal,
                    VerificationCode = verificationCodeString
                };
                SRegistrationVerificationCode _registrationVerification = new SRegistrationVerificationCode();
                bool isValidCode = _registrationVerification.IsValidVerificationCode(data);
                if(isValidCode == true)
                {
                    return RedirectToAction("VerifiedYourAccount");
                }

                ViewBag.IsValidCode = "false";
            }
            return View(model);
        }

        public ActionResult VerifiedYourAccount()
        {
            return View();
        }

        private void SetViewBagForCountries()
        {
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public JsonResult getCountryPhoneCode(string countryCode)
        {
            if(!string.IsNullOrEmpty(countryCode))
            {
                var phoneCode = _commonServices.getCountryPhoneCode(countryCode);
                return Json(new
                {
                    CountryPhoneCode = phoneCode
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResendVerification()
        {
            string verificationCode = Common.Common.GenerateVerificationCode(6);
            SRegistrationVerificationCode registrationVerificationCodeServices = new SRegistrationVerificationCode();
            int userId = Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.UserId;
            string oldVerificationCode = Common.FaxerSession.RegistrationCodeVerificationViewModel.VerificationCode;

            bool update = registrationVerificationCodeServices.UpdateVerificationCode(userId, oldVerificationCode, verificationCode);
            if(update == true)
            {
                SmsApi smsApiServices = new SmsApi();
                string message = smsApiServices.GetRegistrationMessage(verificationCode);
                string phoneNumber = Common.Common.GetCountryPhoneCode(Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.PersonalAddressCountry) + " " + Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.PhoneNumber;
                smsApiServices.SendSMS(phoneNumber, message);
                                     

            }
            return RedirectToAction("VerificationCode");


        }
    }
}