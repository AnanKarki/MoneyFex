using FAXER.PORTAL.Areas.Staff.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Staff.Controllers
{
    public class StaffLoginController : Controller
    {
        // GET: Staff/StaffLogin
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult StaffMainLogin()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult StaffMainLogin([Bind(Include = StaffLoginViewModel.BindProperty)]ViewModels.StaffLoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                Services.StaffLoginServices services = new Services.StaffLoginServices();
                var LoginAccess = services.GetLogin(model);
                if (LoginAccess != null)
                {
                    if (LoginAccess.IsActive == true)
                    {
                        string decriptedPassword = Common.Common.Decrypt(LoginAccess.Password);
                        bool isValidAgent = (decriptedPassword == model.StaffPassword && LoginAccess.LoginCode == model.LoginCode);
                        if (isValidAgent)
                        {

                            var logincount = services.UpdateLoginFailureCount(model.StaffEmail, 0, true);
                            Services.StaffSignUpServices staffSignUpServices = new Services.StaffSignUpServices();
                            var staffInformation = staffSignUpServices.GetStaffByEmail(LoginAccess.UserName);
                            ViewModels.LoggedStaff loggedStaff = new ViewModels.LoggedStaff()
                            {
                                FirstName = staffInformation.FirstName,
                                LastName = staffInformation.LastName,
                                LoginCode = LoginAccess.LoginCode,
                                MiddleName = staffInformation.MiddleName,
                                StaffId = staffInformation.Id,
                                StaffMFSCode = staffInformation.StaffMFSCode,
                                StaffCurrentLocation = model.StaffCurrentLocation,
                                Country= staffInformation.Country,
                                SystemLoginLevel = staffInformation.SytemLoginLevelOfStaff
                            };
                            Common.StaffSession.LoggedStaff = loggedStaff;

                            services.SaveStaffLoginHistory();

                            return RedirectToAction("Index", "StaffHome");
                        }
                        else
                        {
                            if (LoginAccess.LoginFailedCount == 0)
                            {
                                if (LoginAccess.IsActive)
                                {
                                    var loginCount = services.UpdateLoginFailureCount(LoginAccess.UserName, 1, true);
                                }
                                ModelState.AddModelError("StaffPassword", "You have two more attempt");
                                return View(model);

                            }
                            else if (LoginAccess.LoginFailedCount == 1)
                            {
                                if (LoginAccess.IsActive)
                                {

                                    var loginCount = services.UpdateLoginFailureCount(LoginAccess.UserName, 2, true);
                                }

                                ModelState.AddModelError("StaffPassword", "You have one more attempt");
                                return View(model);
                            }
                            else if (LoginAccess.LoginFailedCount == 2)
                            {
                                if (LoginAccess.IsActive)
                                {
                                    var loginCount = services.UpdateLoginFailureCount(LoginAccess.UserName, 2, false);
                                }
                                ModelState.AddModelError("StaffPassword", "Your account has been deactivated ,contact the admin");
                                return View(model);
                            }
                        }
                    }
                    else
                    {

                        ModelState.AddModelError("StaffPassword", "Your Account has been deactivated , contact the admin");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("StaffPassword", "Your Login Credential Did Not Match");
                    return View(model);
                }
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult RequestSecurityCode()
        {
            ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        public ActionResult RequestSecurityCode(string EmailAddress)
        {

            string msg = "";
            Services.StaffSignUpServices services = new Services.StaffSignUpServices();
            var data = services.GetStaffByEmail(EmailAddress);
            if (data != null)
            {
                MailCommon mail = new MailCommon();
                string passwordSecurityCode = Common.Common.GenerateRandomDigit(8);
                Common.StaffSession.StaffPasswordSecurityCode = passwordSecurityCode;
                Common.StaffSession.StaffEmailAddress = EmailAddress;


                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/PasswordSecirityCodeEmail?UserName=" + data.FirstName + " " + data.MiddleName + " " +data.LastName + 
                    "&SecurityCode=" + passwordSecurityCode);


                mail.SendMail(EmailAddress, "MoneyFex Password Reset Key", body);
                //mail.SendMail(EmailAddress, "Staff Password Security Code.", "Your Password Security Code is " + passwordSecurityCode);
                return RedirectToAction("PasswordResetSeurityCode");
            }
            else
            {
                msg = "Request Security Code Fail Due To Invalid Email Address";
            }
            ViewBag.Message = msg;
            return View();
        }
        [HttpGet]
        public ActionResult PasswordResetSeurityCode()
        {

            ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        public ActionResult PasswordResetSeurityCode(string PasswordRequestCode)
        {
            string msg = "";
            var passwordSecuritycode = Common.StaffSession.StaffPasswordSecurityCode;
            if (PasswordRequestCode == passwordSecuritycode)
            {
                return RedirectToAction("ProceedToPasswordReset");
            }
            else
            {
                msg = "Sorry The security code is inValid";
            }

            ViewBag.Message = msg;
            return View();
        }
        [HttpGet]
        public ActionResult ProceedToPasswordReset()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ProceedToPasswordReset([Bind(Include = ConfirmPasswordResetViewModel.BindProperty)]ViewModels.ConfirmPasswordResetViewModel model)
        {
            var email = Common.StaffSession.StaffEmailAddress;
            Services.StaffLoginServices services = new Services.StaffLoginServices();

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
                        mail.SendMail(passwordReset.UserName, "Password Changed.", mailMessage);

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    return RedirectToAction("ConfirmPasswordReset");
                }
            }
            else
            {
                ModelState.AddModelError("NewPassword", "Password Should Contain Atleast 1 UpperCase, 1 number and 1 Special Character");
            }
            return View(model);
        }
        public ActionResult ConfirmPasswordReset()
        {
            Session.Clear();
            return View();
        }
        public ActionResult AdminPortalStaffLogin()
        {
            if (Common.StaffSession.LoggedStaff == null) {
                return RedirectToAction("StaffMainLogin");
            }
            return View();
        }
        public ActionResult AdminPortalStaffLogin1()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminPortalStaffLogin([Bind(Include = AdminPortalStaffLoginViewModel.BindProperty)]AdminPortalStaffLoginViewModel model)
        {
            Common.StaffSession.StaffTimeZone = model.TimeZone;
            Services.StaffLoginServices Service = new Services.StaffLoginServices();
            if (model != null)
            {
                string result = Service.CheckStaffName(model.StaffNameAndSurname, model.StaffMFSCode);
                if (result == "noStaff")
                {
                    ModelState.AddModelError("StaffMFSCode", "Staff must be logged in first !");
                    return View(model);
                }
                else if (result == "invalidUser")
                {
                    ModelState.AddModelError("StaffMFSCode", "Only logged in staff can log in to admin portal !");
                    return View(model);
                }
                else if (result == "noData")
                {
                    ModelState.AddModelError("StaffMFSCode", "You are not authorized to login. Please contact Administrator !");
                    return View(model);
                }
                else if (result == "dayMismatch")
                {
                    ModelState.AddModelError("StaffMFSCode", "You are not authorized to login today !");
                    return View(model);
                }
                else if (result == "logInTime")
                {
                    ModelState.AddModelError("StaffMFSCode", "Sorry, You can't log in at this time !");
                    return View(model);
                }
                else if (result == "mfsCode")
                {
                    ModelState.AddModelError("StaffMFSCode", "MFS Code doesn't exist. Please try again !");
                    return View(model);
                }
                else if (result == "nameMismatch")
                {
                    ModelState.AddModelError("StaffNameAndSurname", "Name mismatch. Please try again !");
                    return View(model);
                }
                else if (result == "success")
                {
                    AdminSession.StaffId = StaffSession.LoggedStaff.StaffId;
                    SystemLoginLevel level = Service.GetLoginLevel(model.StaffMFSCode);
                    AdminSession.StaffLoginLevel = level;
                    Service.SaveStaffLoginHistory();
                    return Redirect("/Admin/admindashboard");
                }
            }
            return View(model);

        }
        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("StaffMainLogin");
        }

    }
}