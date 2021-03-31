using FAXER.PORTAL.Areas.Businesses.Services;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class BusinessLoginController : Controller
    {

        CommonServices commonServices = new CommonServices();
        // GET: Businesses/BusinessLogin
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            if (FAXER.PORTAL.Common.BusinessSession.LoggedBusinessMerchant != null)
            {
                return RedirectToAction("Index", "BusinessHome");
            }

            return View();
        }
        [HttpPost]
        public ActionResult Login([Bind(Include = ViewModels.BusinessLoginViewModel.BindProperty)]ViewModels.BusinessLoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.EmailAddress))
            {
                ModelState.AddModelError("EmailAddress", "The Field Is Required");
                return View(model);
            }
            //if (string.IsNullOrEmpty(model.LoginCode))
            //{
            //    ModelState.AddModelError("LoginCode", "The Field Is Required");
            //    return View(model);
            //}
            if (string.IsNullOrEmpty(model.password))
            {
                ModelState.AddModelError("password", "The Field Is Required");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                Services.BusinessLoginServices services = new Services.BusinessLoginServices();
                
                var LoginAccess = services.Login(model);
                if (LoginAccess != null)
                {
                    if (LoginAccess.IsActive == true)
                    {
                        string decriptedPassword = Common.Common.Decrypt(LoginAccess.Password);
                        bool isValidBusinessMerchant = (decriptedPassword == model.password);
                        if (isValidBusinessMerchant)
                        {

                            var logincount = services.UpdateLoginFailureCount(model.EmailAddress, 0, true);
                            Services.BusinessSignUpServices businessSignUpServices = new Services.BusinessSignUpServices();
                            var BusinessInformation = businessSignUpServices.GetBusinessInformation(LoginAccess.Username);

                            string FullName = businessSignUpServices.getBecomeaMerchantInformation(BusinessInformation.RegistrationNumBer);

                            var MFBCDetails = commonServices.GetMFBCCardInformationByKiiPayBusinessInformationId(BusinessInformation.Id);
                            ViewModels.LoggedBusinessMerchant loggedBusiness = new ViewModels.LoggedBusinessMerchant()
                            {
                                BusinessName = BusinessInformation.BusinessName,
                                LoginCode = LoginAccess.LoginCode,
                                KiiPayBusinessInformationId = BusinessInformation.Id,
                                BusinessMobileNo = BusinessInformation.BusinessMobileNo,
                                BusinessEmailAddress = BusinessInformation.Email,
                                CountryCode = BusinessInformation.BusinessOperationCountryCode,
                                FullName = FullName,
                                MFBCCardNo = commonServices.GetMFBCCardInformationByKiiPayBusinessInformationId(BusinessInformation.Id) == null ? "" : commonServices.GetMFBCCardInformationByKiiPayBusinessInformationId(BusinessInformation.Id).MobileNo.Decrypt(),
                                CardUserName = MFBCDetails == null ? "" : MFBCDetails.FirstName + " " + MFBCDetails.MiddleName + " " + MFBCDetails.LastName,
                          
                            };

                            Common.BusinessSession.FaxingCountry = loggedBusiness.CountryCode;
                            Common.BusinessSession.FaxingCurrency = Common.Common.GetCountryCurrency(loggedBusiness.CountryCode);
                            Common.BusinessSession.FaxingCurrencySymbol = Common.Common.GetCurrencySymbol(loggedBusiness.CountryCode);
                            Common.BusinessSession.LoggedBusinessMerchant = loggedBusiness;
                            Common.BusinessSession.MerchantHasMFBCCard = businessSignUpServices.MerchantHasMFBCCard(loggedBusiness.KiiPayBusinessInformationId);
                            Services.BusinessCardServices cardServices = new Services.BusinessCardServices();
                            Common.BusinessSession.LoggedBusinessMerchant.CurrentBalanceOnCard = cardServices.GetCreditOnCard();
                            Common.BusinessSession.FirstLogin = "FirstLogin";

                            if (Common.BusinessSession.FormURL != "")
                            {
                                return Redirect(Common.BusinessSession.FormURL);
                            }
                            return RedirectToAction("Index", "BusinessHome");
                        }
                        else
                        {
                            if (LoginAccess.LoginFailCount == 0)
                            {
                                if (LoginAccess.IsActive)
                                {
                                    var loginCount = services.UpdateLoginFailureCount(LoginAccess.Username, 1, true);
                                }
                                ModelState.AddModelError("password", "Login Credential did not match , You have two more attempt");
                                return View(model);

                            }
                            else if (LoginAccess.LoginFailCount == 1)
                            {
                                if (LoginAccess.IsActive)
                                {

                                    var loginCount = services.UpdateLoginFailureCount(LoginAccess.Username, 2, true);
                                }

                                ModelState.AddModelError("password", "Login Credential did not match , You have one more attempt");
                                return View(model);
                            }
                            else if (LoginAccess.LoginFailCount == 2)
                            {
                                if (LoginAccess.IsActive)
                                {
                                    var loginCount = services.UpdateLoginFailureCount(LoginAccess.Username, 2, false);
                                }
                                ModelState.AddModelError("password", "Your account has been deactivated ,contact the admin");
                                return View(model);
                            }
                        }
                    }
                    else
                    {

                        ModelState.AddModelError("password", "Your Account has been deactivated ,Please contact the moneyfax support team");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("password", "Your Login Credential Did Not Match");
                    return View(model);
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();

        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            Services.BusinessSignUpServices services = new Services.BusinessSignUpServices();
            var data = services.GetBusinessInformation(email);
            if (data != null)
            {
                MailCommon mail = new MailCommon();
                string passwordSecurityCode = Common.Common.GenerateRandomDigit(8);
                Common.BusinessSession.PasswordSecurityCode = passwordSecurityCode;
                Common.BusinessSession.EmailAddress = email;
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                //mail.SendMail(email, "Business Password Security Code.", "Your Password Security Code is " + passwordSecurityCode);
                string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/PasswordSecirityCodeEmail?UserName=" + data.BusinessName +
                    "&SecurityCode=" + passwordSecurityCode);

                mail.SendMail(email, "MoneyFex Password Reset Key", body);

                SmsApi smsApiServices = new SmsApi();
                var message = smsApiServices.GetPasswordResetMessage(passwordSecurityCode);
                string PhoneNo = Common.Common.GetCountryPhoneCode(data.BusinessOperationCountryCode) + "" + data.PhoneNumber;
                smsApiServices.SendSMS(PhoneNo, message);

                return RedirectToAction("SecurityCode");

            }
            else
            {
                TempData["InValid"] = "Please enter valid Email";
                return View();

            }

        }
        [HttpGet]
        public ActionResult SecurityCode()
        {

            return View();
        }
        [HttpPost]
        public ActionResult SecurityCode(string Securitycode)
        {
            var passwordSecuritycode = Common.BusinessSession.PasswordSecurityCode;
            if (Securitycode == passwordSecuritycode)
            {
                return RedirectToAction("PasswordReset");
            }
            else
            {
                TempData["InValid"] = "Please enter valid Security Code";
                return View();
            }

        }
        [HttpGet]
        public ActionResult PasswordReset()
        {

            return View();
        }
        [HttpPost]
        public ActionResult PasswordReset([Bind(Include = ViewModels.ConfirmPasswordResetViewModel.BindProperty)]ViewModels.ConfirmPasswordResetViewModel model)
        {
            var email = Common.BusinessSession.EmailAddress;
            Services.BusinessLoginServices services = new Services.BusinessLoginServices();
            if (ModelState.IsValid)
            {
                if (Common.Common.ValidatePassword(model.NewPassword))
                {
                    if (model.NewPassword != model.ConfirmPassword)
                    {
                        ModelState.AddModelError("ConfirmPassword", "The password didnot Match");
                        return View();
                    }
                    var passwordReset = services.PasswordReset(email, model);
                    if (passwordReset != null)
                    {
                        MailCommon mail = new MailCommon();
                        try
                        {
                            string mailMessage = string.Format("Contact Your Administration to activate your account. You Password Has been changed at", System.DateTime.Now);
                            mail.SendMail(passwordReset.Username, "Password Changed.", mailMessage);

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        return RedirectToAction("SuccessReset");
                    }
                }
                else
                {
                    ModelState.AddModelError("NewPassword", "Password Should Contain Atleast 1 UpperCase, 1 number and 1 Special Character");
                }
            }
            return View(model);

        }

        public ActionResult SuccessReset()
        {
            Session.Clear();
            Session.Abandon();
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Home", "BusinessHome");
        }
    }
}