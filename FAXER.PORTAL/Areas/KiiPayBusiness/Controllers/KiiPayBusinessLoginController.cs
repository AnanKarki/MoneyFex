using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessLoginController : Controller
    {
        KiiPayBusinessLoginServices _kiiPayBusinessLoginServices = null;
        public KiiPayBusinessLoginController()
        {
            _kiiPayBusinessLoginServices = new KiiPayBusinessLoginServices();
        }
        // GET: KiiPayBusiness/KiiPayBusinessLogin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login() {

            return View();

        }
        [HttpPost]
        public ActionResult Login([Bind(Include = ViewModels.KiiPayBusinessLoginVM.BindProperty)]ViewModels.KiiPayBusinessLoginVM vm)
        {

            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            if (ModelState.IsValid) {

                
            
                var LoginAccess = _kiiPayBusinessLoginServices.Login(vm);
                if (LoginAccess != null)
                {
                    if (LoginAccess.IsActive == true)
                    {
                        string decriptedPassword = Common.Common.Decrypt(LoginAccess.Password);
                        bool isValidBusinessMerchant = (decriptedPassword == vm.Password);
                        if (isValidBusinessMerchant)
                        {

                            var logincount = _kiiPayBusinessLoginServices.UpdateLoginFailureCount(vm.UserName, 0, true);

                          
                            var KiiPayBusinessInformation = LoginAccess.KiiPayBusinessInformation;
                            
                            _kiiPayBusinessLoginServices.SetLoggedKiiPayBusinessUserInfo(KiiPayBusinessInformation);
                         
                            return RedirectToAction("DashBoard", "KiiPayBusinessHome");
                        }
                        else
                        {
                            if (LoginAccess.LoginFailCount == 0)
                            {
                                if (LoginAccess.IsActive)
                                {
                                    var loginCount = _kiiPayBusinessLoginServices.UpdateLoginFailureCount(LoginAccess.Username, 1, true);
                                }
                                ModelState.AddModelError("password", "Login Credential did not match , You have two more attempts");
                                return View(vm);

                            }
                            else if (LoginAccess.LoginFailCount == 1)
                            {
                                if (LoginAccess.IsActive)
                                {

                                    var loginCount = _kiiPayBusinessLoginServices.UpdateLoginFailureCount(LoginAccess.Username, 2, true);
                                }

                                ModelState.AddModelError("password", "Login Credential did not match , You have one more attempt");
                                return View(vm);
                            }
                            else if (LoginAccess.LoginFailCount == 2)
                            {
                                if (LoginAccess.IsActive)
                                {
                                    var loginCount = _kiiPayBusinessLoginServices.UpdateLoginFailureCount(LoginAccess.Username, 2, false);
                                }
                                ModelState.AddModelError("password", "Your account has been deactivated ,contact the admin");
                                return View(vm);
                            }
                        }
                    }
                    else
                    {

                        ModelState.AddModelError("password", "Your Account has been deactivated ,Please contact the moneyfax support team");
                        return View(vm);
                    }
                }
                else
                {
                    ModelState.AddModelError("password", "Your Login Credential Did Not Match");
                    return View(vm);
                }
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult ForgotPassword() {

            return View();
        }

        [HttpPost]
        public ActionResult RequestSecurityCode(string emailAddress) {

            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();

            var result = _kiiPayBusinessCommonServices.IsValidEmailAddress(emailAddress);
            if (!result)
            {

                ModelState.AddModelError("emailAddressError", "please enter the valid email address");
                return View("ForgotPassword");
            }
            else
            {

                _kiiPayBusinessCommonServices.SendSecurityCode(emailAddress);


            }
           
            return RedirectToAction("EnterSecurityCode");
        }

        [HttpGet]
        public ActionResult EnterSecurityCode() {


            return View();
        }



        [HttpPost]
        public ActionResult EnterSecurityCode(string securityCode)
        {
            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();

            var result = _kiiPayBusinessCommonServices.IsValidSecurityCode(securityCode);
            if (!result) {

                ModelState.AddModelError("securityCodeValidation", "please enter the valid security code");
                return View();
            }

            return View("ResetPassword");
        }


        [HttpGet]
        public ActionResult ResetPassword() {

            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword([Bind(Include = KiiPayBusinessResetPasswordVM.BindProperty)]KiiPayBusinessResetPasswordVM vm) {

            if (ModelState.IsValid) {

                _kiiPayBusinessLoginServices.PasswordReset(vm);
                return View("PasswordResetSuccessful");
            }
            return View(vm);

        }
        [HttpGet]
        public ActionResult PasswordResetSuccessful() {

            return View();
        }


        public ActionResult Logout() {

            Session.Abandon();
            Session.Clear();

            return RedirectToAction("Login");
        }

    }
}