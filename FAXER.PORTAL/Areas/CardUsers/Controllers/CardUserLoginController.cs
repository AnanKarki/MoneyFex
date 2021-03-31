using FAXER.PORTAL.Areas.CardUsers.Services;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class CardUserLoginController : Controller
    {
        CardUserCommonServices cardUserCommonServices = new CardUserCommonServices();
        // GET: CardUsers/CardUserLogin
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login([Bind(Include = ViewModels.CardUserLoginViewModel.BindProperty)]ViewModels.CardUserLoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                Services.CardUserLoginServices loginServices = new Services.CardUserLoginServices();
                var Login = loginServices.CardUserLogin(model.EmailAddress);
                var IsDeleted = loginServices.IsDeleted(model.EmailAddress);
                if (Login != null)
                {

                    if (Login.IsActive == true && IsDeleted == false)
                    {
                        string decriptedPassword = Common.Common.Decrypt(Login.Password);
                        bool isValidCardUser = (decriptedPassword == model.Password);
                        if (isValidCardUser)
                        {
                            var carduserInformation = loginServices.GetCardUserInformation(Login.KiiPayPersonalUserInformationId);

                            var MFTCCardNumber = loginServices.GetMFTCCardNumber(carduserInformation.KiiPayPersonalWalletInformationId);

                            ViewModels.LoggedKiiPayUserViewModel logged = new ViewModels.LoggedKiiPayUserViewModel()
                            {
                                id = carduserInformation.Id,
                                FirstName = carduserInformation.KiiPayPersonalWalletInformation.FirstName,
                                LastName = carduserInformation.KiiPayPersonalWalletInformation.LastName,
                                FullName = carduserInformation.KiiPayPersonalWalletInformation.FirstName + " " + carduserInformation.KiiPayPersonalWalletInformation.LastName,
                                Email = carduserInformation.EmailAddress,
                                KiiPayPersonalId = carduserInformation.KiiPayPersonalWalletInformationId,
                                Country = carduserInformation.KiiPayPersonalWalletInformation.CardUserCountry,
                                BalanceOnCard = cardUserCommonServices.getCurrentBalanceOnCard(carduserInformation.KiiPayPersonalWalletInformationId),
                                CardUserCurrency = Common.Common.GetCountryCurrency(carduserInformation.KiiPayPersonalWalletInformation.CardUserCountry),
                                CardUserCurrencySymbol = Common.Common.GetCurrencySymbol(carduserInformation.KiiPayPersonalWalletInformation.CardUserCountry),
                                MobileNumber = MFTCCardNumber
                            };
                            Common.CardUserSession.LoggedCardUserViewModel = logged;
                            Common.CardUserSession.FaxingCountry = logged.Country;
                            Common.CardUserSession.FaxingCurrency = Common.Common.GetCountryCurrency(logged.Country);
                            Common.CardUserSession.FaxingCurrencySymbol = Common.Common.GetCurrencySymbol(logged.Country);
                            var logincount = loginServices.UpdateLoginFailureCount(model.EmailAddress, 0, true);
                            return RedirectToAction("Index", "CardUserHome");
                        }
                        else
                        {

                            if (Login.LoginFailedCount == 0)
                            {
                                if (Login.IsActive)
                                {
                                    var loginCount = loginServices.UpdateLoginFailureCount(Login.Email, 1, true);
                                }
                                ModelState.AddModelError("Password", "Login Credential did not match , You have two more attempt");
                                return View(model);

                            }
                            else if (Login.LoginFailedCount == 1)
                            {
                                if (Login.IsActive)
                                {

                                    var loginCount = loginServices.UpdateLoginFailureCount(Login.Email, 2, true);
                                }

                                ModelState.AddModelError("Password", "Login Credential did not match , You have one more attempt");
                                return View(model);
                            }
                            else if (Login.LoginFailedCount == 2)
                            {
                                if (Login.IsActive)
                                {
                                    var loginCount = loginServices.UpdateLoginFailureCount(Login.Email, 2, false);
                                }
                                ModelState.AddModelError("Password", "Your account has been deactivated ,please contact moneyfax support team");
                                return View(model);
                            }


                        }

                    
                    }
                    else if (IsDeleted == true)
                    {
                        ModelState.AddModelError("Password", "Your card has been deleted ,please contact moneyfax support team");
                        return View(model);

                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Your Account has been deactivated , please contact moneyfax support team");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Incorrect Username or password ");
                }

            }

            return View(model);
        }
        [HttpGet]
        public ActionResult Logout()
        {

            if (Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                Session.Clear();
                Session.Abandon();
            }
            return RedirectToAction("Home" ,"CardUserHome");
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            Services.CardUserLoginServices services = new Services.CardUserLoginServices();
            var ValidEmail = services.CardUserLogin(Email);
            if (ValidEmail != null)
            {
                MailCommon mail = new MailCommon();
                string passwordSecurityCode = Common.Common.GenerateRandomDigit(8);
                Common.CardUserSession.PasswordSecurityCode = passwordSecurityCode;
                Common.CardUserSession.EmailAddress = Email;
                //mail.SendMail(Email, "Business Password Security Code.", "Your Password Security Code is " + passwordSecurityCode);

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/PasswordSecirityCodeEmail?UserName=" +
                    ValidEmail.KiiPayPersonalUserInformation.KiiPayPersonalWalletInformation.FirstName + " " + ValidEmail.KiiPayPersonalUserInformation.KiiPayPersonalWalletInformation.MiddleName + " " +
                    ValidEmail.KiiPayPersonalUserInformation.KiiPayPersonalWalletInformation.LastName +
                    "&SecurityCode=" + passwordSecurityCode);


                mail.SendMail(Email, "Money Faxer Password Reset Key", body);
                return RedirectToAction("SecurityCode");
            }
            else
            {
                TempData["Invalid"] = "Please Enter a Valid Email Address";
            }
            return View();
        }
        [HttpGet]
        public ActionResult SecurityCode()
        {

            return View();
        }
        [HttpPost]
        public ActionResult SecurityCode(string securityCode)
        {
            var passwordSecuritycode = Common.CardUserSession.PasswordSecurityCode;
            if (securityCode == passwordSecuritycode)
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
        public ActionResult PasswordReset([Bind(Include = ViewModels.PasswordResetViewModel.BindProperty)]ViewModels.PasswordResetViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password didn't match");
                    return View(model);
                }
                bool validPassword = Common.Common.ValidatePassword(model.NewPassword);
                if (validPassword == true)
                {
                    Services.CardUserLoginServices services = new Services.CardUserLoginServices();
                    var result = services.ResetPassword(model);
                    if (result == true)
                    {

                        return RedirectToAction("SuccessReset");
                    }
                    else
                    {
                        ModelState.AddModelError("ConfirmPassword", "Sorry Password reset fail please try again");
                    }

                }
                else
                {
                    ModelState.AddModelError("NewPassword", "Password Should Contain Atleast 1 UpperCase, 1 number and 1 Special Character");
                }

            }
            return View();
        }


        public ActionResult SuccessReset()
        {

            Session.Clear();
            Session.Abandon();
            return View();
        }
    }
}