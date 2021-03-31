using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Businesses.Services;
using FAXER.PORTAL.Areas.Businesses.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class BusinessRegistrationController : Controller
    {
        DB.FAXEREntities db = null;
        BecomeAMerchantServices becomeAMerchantServices = null;
        BusinessSignUpServices businessSignUpServices = null;
        BusinessLoginServices businessLoginServices = null;

        public BusinessRegistrationController()
        {
            db = new DB.FAXEREntities();
             becomeAMerchantServices = new BecomeAMerchantServices();
            businessSignUpServices = new BusinessSignUpServices();
            businessLoginServices = new BusinessLoginServices();
        }

        // GET: Businesses/BusinessRegistration
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            ViewBag.Country = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");
            if(Common.BusinessResistrationSession.BusinessRegistrationsSignUpVm != null)
            {
                var data = Common.BusinessResistrationSession.BusinessRegistrationsSignUpVm;
                return View(data);
            }
            return View();
        }

        [HttpPost]
        public ActionResult SignUp([Bind(Include = BusinessRegistrationsSignUpVm.BindProperty)]BusinessRegistrationsSignUpVm vm)
        {
            ViewBag.Country = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");

            if (ModelState.IsValid)
            {
                if (!Common.Common.ValidatePassword(vm.Password))
                {

                    ModelState.AddModelError("Password", "Password Should Contain Atleast 1 UpperCase, 1 number and 1 Special Character");
                    return View(vm);
                }

                
                if (mailExists(vm.Email) == true)
                {
                    ModelState.AddModelError("Email", "This email address is already taken !");
                    return View(vm);
                }

                if (vm.BusinessType == BusinessType.Non)
                {
                    ModelState.AddModelError("BusinessType", "This field is required");
                    return View(vm);
                }

                Common.BusinessResistrationSession.BusinessRegistrationsSignUpVm = vm;
                return RedirectToAction("BusinessContactDetails");
            }
            else
            {
                return View(vm);
            }
        }

        private bool mailExists(string email)
        {
            ViewRegisterBusinessServices viewRegisterBusinessServices = new ViewRegisterBusinessServices();
            var mailExists = viewRegisterBusinessServices.CheckMerchantEmail(email);
            return mailExists;
        }

        [HttpGet]
        public ActionResult BusinessContactDetails()
        {
            ViewBag.Country = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");
            if(Common.BusinessResistrationSession.BusinessContactDetails != null)
            {
                var data = Common.BusinessResistrationSession.BusinessContactDetails;
                return View(data);
            }
            return View();
        }

        [HttpPost]
        public ActionResult BusinessContactDetails([Bind(Include = BusinessOwnerInformationVm.BindProperty)]BusinessOwnerInformationVm vm)
        {
            ViewBag.Country = new SelectList(db.Country.ToList(), "CountryCode", "CountryName");
            if (ModelState.IsValid)
            {
                Common.BusinessResistrationSession.BusinessContactDetails = vm;
                return RedirectToAction("PaymentServiceAgreement");
            }
            else
            {



                return View(vm);
            }

        }
        [HttpGet]
        public ActionResult PaymentServiceAgreement()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PaymentServiceAgreement([Bind(Include = PaymentServiceAgreementVm.BindProperty)]PaymentServiceAgreementVm vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.IhaveRead == false)
                {
                    ModelState.AddModelError("IhaveRead", "This field must be checked");
                    return View(vm);
                }
                if (vm.IAccept == false)
                {
                    ModelState.AddModelError("IAccept", "This field must be checked");
                    return View(vm);
                }
                Services.BusinessSignUpServices businessSignUpServices = new Services.BusinessSignUpServices();
                var RegistrationNumber = businessSignUpServices.GetResgistrationNumber(7);
                string activationCode = Guid.NewGuid().ToString();
                string BMFSCode = businessSignUpServices.GetMFSCode(10);
                var merchantVm = Common.BusinessResistrationSession.BusinessContactDetails;
                var businessInfo = Common.BusinessResistrationSession.BusinessRegistrationsSignUpVm;


                DB.KiiPayBusinessInformation business = new KiiPayBusinessInformation()
                {
                    BusinessOperationAddress1 = businessInfo.AddressLine1,
                    BusinessOperationAddress2 = businessInfo.AddressLine2,
                    BusinessLicenseNumber = businessInfo.RegistrationNumber,
                    BusinessMobileNo = BMFSCode,
                    BusinessName = businessInfo.BusinessName,
                    BusinessOperationCity = businessInfo.City,
                    ContactPerson = businessInfo.ContactPersonName,
                    BusinessOperationCountryCode = businessInfo.Country,
                    Email = businessInfo.Email,
                    FaxNumber = "",
                    PhoneNumber = businessInfo.MobileNumber.FormatPhoneNo(),
                    BusinessOperationPostalCode = businessInfo.ZipCode,
                    RegistrationNumBer = RegistrationNumber,
                    BusinessOperationState = businessInfo.State,
                    Website = "",

                };
                DB.BecomeAMerchant merchant = new DB.BecomeAMerchant()
                {
                    ActivationCode = activationCode,
                    Address1 = merchantVm.AddressLine1,
                    Address2 = merchantVm.AddressLine2,
                    BusinessEmailAddress = merchantVm.EmailAddress,
                    BusinessLicenseRegistrationNumber = businessInfo.RegistrationNumber,
                    BusinessType = businessInfo.BusinessType,
                    City = merchantVm.City,
                    CompanyBusinessName = businessInfo.BusinessName,
                    ContactPhone = merchantVm.ContactNo.FormatPhoneNo(),
                    CountryCode = merchantVm.CountryCode,
                    FaxNo = "",
                    FirstName = merchantVm.FirstName,
                    LastName = merchantVm.LastName,
                    PostZipCode = merchantVm.ZipCode,
                    RegistrationNumber = RegistrationNumber,
                    StateProvince = merchantVm.State,
                    Street = "",
                    Website = "",
                    Id = merchantVm.Id,
                };

                string loginCode = businessSignUpServices.GetNewLoginCode(6);
                var savedMerahnt = becomeAMerchantServices.AddBusinessMerchant(merchant);
                var savedBusiness = businessSignUpServices.SaveBusinessInformation(business);
                DB.KiiPayBusinessLogin login = new KiiPayBusinessLogin()
                {
                    ActivationCode = activationCode,
                    IsActive = true,
                    KiiPayBusinessInformationId = savedBusiness.Id,
                    IsDeleted = false,
                    LoginFailCount = 0,
                    Password = businessInfo.Password.Encrypt(),
                    Username = businessInfo.Email,
                    LoginCode = loginCode,
                };
                var savedLogin = businessSignUpServices.SaveBusinessLogin(login);

                string verificationCode = Common.Common.GenerateVerificationCode(6);

                RegistrationCodeVerificationViewModel regverificationCodeVm = new RegistrationCodeVerificationViewModel()
                {
                    Country = savedBusiness.BusinessOperationCountryCode,
                    UserId = savedBusiness.Id,
                    //IsExpired = false,
                    PhoneNo = Common.Common.GetCountryPhoneCode(savedBusiness.BusinessOperationCountryCode) + " " + savedBusiness.PhoneNumber,
                    RegistrationOf = RegistrationOf.KiiPayBusiness,
                    VerificationCode = verificationCode,
                    
                };

                Common.FaxerSession.RegistrationCodeVerificationViewModel = regverificationCodeVm;
                SRegistrationVerificationCode registrationVerificationCodeServices = new SRegistrationVerificationCode();
                registrationVerificationCodeServices.Add(regverificationCodeVm);


                // Sms Function Execute Here 
                SmsApi smsApiServices = new SmsApi();
                string message =  smsApiServices.GetRegistrationMessage(verificationCode);

                smsApiServices.SendSMS(regverificationCodeVm.PhoneNo, message);
                //Email 

                ViewRegisterBusinessServices viewRegisterBusinessServices = new ViewRegisterBusinessServices();
                //var mailExists = viewRegisterBusinessServices.CheckMerchantEmail(businessInfo.Email);
                //if(mailExists == true)
                //{
                //    ModelState.AddModelError("Email", "This email address is already taken !");
                //    return View(vm);
                //}

                string msg = "";
                MailCommon mail = new MailCommon();
                var baseUrl = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
                var link = string.Format("{0}/Businesses/BecomeAMerchant/ValidateLink?activationCode={1}", baseUrl, activationCode);
                string body = "";
                try
                {
                    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BusinessRegistrationEmail?BusinessName=" + businessInfo.BusinessName + "&Link=" + link);
                    mail.SendMail(businessInfo.Email, "Business Registration Information - Link", body);

                    msg = "Registraion Completed";
                }
                catch (Exception)
                {
                    mail.SendMail(businessInfo.Email, "Business Registration Information - Link", body);
                    msg = "Problem finding mail";
                }
                Common.BusinessResistrationSession.BusinessRegistrationsSignUpVm = null;
                Common.BusinessResistrationSession.BusinessContactDetails = null;                
                return RedirectToAction("Index", "SenderRegistrationCodeVerification", new { area = "" });
               
            }
            else
            {
                return View(vm);
            }
        }

        public JsonResult GetAddressDetails()
        {
            BusinessRegistrationsSignUpVm data = Common.BusinessResistrationSession.BusinessRegistrationsSignUpVm;
            if (data != null)
            {
                return Json(new
                {
                    AddressLine1 = data.AddressLine1,
                    AddressLine2 = data.AddressLine2,
                    City = data.City,
                    ContactNo = data.ContactNo,
                    CountryCode = data.Country,
                    State = data.State,
                    ZipCode = data.ZipCode
                }, JsonRequestBehavior.AllowGet);
            }

            //return Json(new
            //{
            //    AddressLine1 = data.AddressLine1,
            //    AddressLine2 = data.AddressLine2,
            //    City = data.City,
            //    ContactNo = data.ContactNo,
            //    CountryId = data.Country,
            //    State = data.State,
            //    ZipCode = data.ZipCode
            //}, JsonRequestBehavior.AllowGet);

            return null;
        }


    }
}