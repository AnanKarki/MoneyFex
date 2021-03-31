using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Controllers
{
    public class LogInController : Controller
    {
        LogInServices _service = null;
        CommonAllServices _commonServices = null;
        public LogInController()
        {
            _commonServices = new CommonAllServices();
            _service = new LogInServices();
        }
        // GET: KiiPayPersonal/LogIn
        public ActionResult Index()
        {
            if (Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = LogInViewModel.BindProperty)]LogInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userLoginData = _service.userLoginList().Where(x => x.Email.ToLower().Trim() == model.UserName.ToLower().Trim()).FirstOrDefault();
                if (userLoginData == null)
                {
                    ModelState.AddModelError("Password", "Invalid Login !");
                    return View(model);
                }
                if (userLoginData.IsActive == false)
                {
                    ModelState.AddModelError("Password", "Your account has been deactivated. Please contact administrator !");
                    return View(model);
                }
                if (userLoginData.Password.Decrypt().Trim() == model.Password.Trim())
                {
                    if (userLoginData.IsActive == true)
                    {
                        userLoginData.LoginFailedCount = 0;
                        _service.updateUserLogin(userLoginData);
                        _service.SetKiiPayPersonalUserLoginSession(userLoginData);
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Your account has been deactivated. Please contact administrator !");
                        return RedirectToAction("Index");
                    }
                }
                if (userLoginData.Password.Decrypt().Trim() != model.Password.Trim())
                {

                    if (userLoginData.LoginFailedCount >= 3)
                    {
                        userLoginData.IsActive = false;
                        _service.updateUserLogin(userLoginData);
                        ModelState.AddModelError("Password", "Your account has been deactivated. Please contact administrator !");
                        return View(model);
                    }
                    userLoginData.LoginFailedCount = userLoginData.LoginFailedCount + 1;
                    _service.updateUserLogin(userLoginData);
                    ModelState.AddModelError("Password", "Invalid Login !");
                    return View(model);
                }


            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword([Bind(Include = ForgotPasswordViewModel.BindProperty)]ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userLoginData = _service.userLoginList().Where(x => x.Email.ToLower().Trim() == model.EmailAddress.ToLower().Trim()).FirstOrDefault();
                if (userLoginData == null)
                {
                    ModelState.AddModelError("EmailAddress", "Sorry, we couldn't find user with this email Address in our database !");
                    return View(model);
                }
                string userName = userLoginData.KiiPayPersonalUserInformation.FirstName + " " + userLoginData.KiiPayPersonalUserInformation.MiddleName + " " + userLoginData.KiiPayPersonalUserInformation.Lastname;
                _commonServices.SendPasswordResetCodeEmail(model.EmailAddress, userName);

                return RedirectToAction("ResetPassword");

            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword([Bind(Include = ResetPasswordViewModel.BindProperty)]ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SecurityCode.Trim() != Common.MiscSession.PasswordSecurityCode.Trim())
                {
                    ModelState.AddModelError("SecurityCode", "Invalid Code !");
                    return View(model);
                }
                else
                {
                    return RedirectToAction("EnterNewPassword");
                }


            }
            return View();
        }


        [HttpGet]
        public ActionResult EnterNewPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EnterNewPassword([Bind(Include = EnterNewPasswordViewModel.BindProperty)]EnterNewPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords don't match !");
                    return View(model);
                }
                else
                {
                    bool updatePassword = _service.changeKiiPayPersonalUserPassword(model.Password);
                    if (updatePassword == true)
                    {
                        return RedirectToAction("PasswordResetSuccess");
                    }
                    else {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }

        public ActionResult PasswordResetSuccess()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Remove("LoggedCardUserViewModel");
            return RedirectToAction("Index");

        }

    }
}