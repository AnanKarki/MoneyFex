using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentLoginController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();

        AgentStaffRegistrationServices agentStaffRegistrationServices = null;
        public AgentLoginController()
        {
            agentStaffRegistrationServices = new AgentStaffRegistrationServices();
        }

        // GET: Agent/AgentLogin
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()

        {
            DemoLoginModel model = new DemoLoginModel();
            model.UserName = "Demo";
            model.Password = "Demo123@";
            Common.FaxerSession.DemoLoginModel = model;

            ViewBag.Message = "";
            ViewBag.TransactionStaffFirstLogin = 0;
            AgentLoginViewModel vm = new AgentLoginViewModel();
            vm.IsFirstLogin = 0;

            return View(vm);
        }
        [HttpPost]
        public ActionResult Login([Bind(Include = AgentLoginViewModel.BindProperty)]AgentLoginViewModel vm)
        {
            ViewBag.TransactionStaffFirstLogin = false;
            Common.AgentSession.AgentTimeZone = vm.TimeZone;
            ViewBag.TransactionStaffFirstLogin = 0;
            // Check user Attempt
            var userAttemptIsValid = LockUser();

            if (userAttemptIsValid.Data == false)
            {

                ModelState.AddModelError("Invalid", userAttemptIsValid.Message);
                return View(vm);
            }
            ViewBag.IsActive = true;
            if (ModelState.IsValid)
            {

                vm.Email = vm.Email.Trim();
                vm.AgentCode = vm.AgentCode.Trim();
                vm.StaffCode = vm.StaffCode.Trim();
                var agentLoginQuery = dbContext.AgentStaffLogin;

                var agentLogin = agentLoginQuery.Where(x => x.Username == vm.Email.Trim()).FirstOrDefault();

                if (agentLogin == null)
                {

                    ModelState.AddModelError("Invalid", "Invalid login credentials !");

                    return View(vm);
                }
                var AgenInformation = dbContext.AgentStaffInformation.Where(x => x.EmailAddress == vm.Email).FirstOrDefault();
                #region Transaction Staff First Login case 

                if (vm.IsFirstLogin == 1)
                {

                    bool IsValidModel = true;
                    if (string.IsNullOrEmpty(vm.NewPassword))
                    {

                        ModelState.AddModelError("InCorrectPassword", "Please enter the password");

                        IsValidModel = false;
                    }
                    else if (Common.Common.ValidatePassword(vm.NewPassword) == false)
                    {


                        ModelState.AddModelError("InCorrectPassword", "password policy didn't match.");

                        IsValidModel = false;

                    }
                    else if (string.IsNullOrEmpty(vm.ConfirmPassword))
                    {


                        ModelState.AddModelError("InvalidConfirmPassword", "Please enter the confirm password");

                        IsValidModel = false;

                    }
                    else if (vm.NewPassword != vm.ConfirmPassword)
                    {

                        ModelState.AddModelError("InvalidConfirmPassword", "Password didn't match");

                        IsValidModel = false;
                    }
                    if (IsValidModel == false)
                    {
                        ViewBag.TransactionStaffFirstLogin = 1;
                        vm.IsFirstLogin = 1;
                        return View(vm);
                    }

                    else
                    {
                        // Create password 
                        agentLogin.Password = vm.NewPassword.Encrypt();
                        agentLogin.IsFirstLogin = false;

                        dbContext.Entry(agentLogin).State = EntityState.Modified;
                        dbContext.SaveChanges();

                        return RedirectToAction("TransactionAgentFirstLoginSuccessFul");
                    }

                }
                else
                {
                    string decryptPassword = Common.Common.Decrypt(agentLogin.Password);
                    bool isValidAgentStaff = (decryptPassword == vm.Password && agentLogin.StaffLoginCode == vm.StaffCode && agentLogin.AgencyLoginCode == vm.AgentCode);

                    if (isValidAgentStaff == true)
                    {
                        if (agentLogin.IsFirstLogin == true)
                        {

                            ViewBag.TransactionStaffFirstLogin = 1;
                            vm.IsFirstLogin = 1;
                            return View(vm);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Invalid", "Invalid login credentials !");

                        return View(vm);

                    }
                }
                #endregion


                if ((agentLogin != null) && AgenInformation.IsDeleted == false && agentLogin.IsActive)
                {

                    string decriptedPassword = Common.Common.Decrypt(agentLogin.Password);
                    bool isValidAgent = (decriptedPassword == vm.Password && agentLogin.StaffLoginCode == vm.StaffCode && agentLogin.AgencyLoginCode == vm.AgentCode);
                    if (isValidAgent)
                    {
                        if (agentLogin.AgentStaff.AgentStaffType == StaffType.Transaction)
                        {
                            if (string.IsNullOrEmpty(agentLogin.StartTime) || string.IsNullOrEmpty(agentLogin.EndTime))
                            {

                                ModelState.AddModelError("Invalid", "Your login access time hasn't been setup ,please contact your agent !");


                                return View(vm);
                            }

                            var validDayTime = agentStaffRegistrationServices.CheckStaffName(agentLogin.Id);

                            if (validDayTime != "success")
                            {
                                ModelState.AddModelError("Invalid", "You are not allowed to login during this time. Please contact your agent to know more about this");

                                return View(vm);
                            }
                        }
                        agentLogin.LoginFailedCount = 0;
                        dbContext.Entry(agentLogin).State = EntityState.Modified;
                        dbContext.SaveChanges();

                        LoggedUserVm loggedUser = new LoggedUserVm()
                        {
                            PayingAgentStaffId = agentLogin.AgentStaffId,
                            PayingAgentAccountNumber = agentLogin.AgentStaff.AgentMFSCode,
                            PayingAgentStaffName = agentLogin.AgentStaff.FirstName + " " + agentLogin.AgentStaff.MiddleName + " " + agentLogin.AgentStaff.LastName,
                            Id = agentLogin.AgentStaff.AgentId,
                            AgentType = agentLogin.AgentStaff.AgentStaffType,
                            IsAUXAgent = agentLogin.AgentStaff.Agent.IsAUXAgent
                        };


                        Common.AgentSession.LoggedUser = loggedUser;

                        Common.AgentSession.AgentStaffLogin = agentLogin;
                        var agentInformation = dbContext.AgentInformation.Where(x => x.Id == agentLogin.AgentStaff.AgentId).FirstOrDefault();
                        Common.AgentSession.AgentInformation = agentInformation;
                        Common.AgentSession.FirstLogin = "FirstLogin";
                        if (string.IsNullOrEmpty(Common.AgentSession.FormURL))
                        {
                            HttpCookie authenticationCookies = new HttpCookie("ALoginAttempt");
                            Response.Cookies.Add(authenticationCookies);
                            HttpContext.Request.Cookies.Clear();
                            return RedirectToAction("GoToDashboard", "AgentDashboard", new { area = "Agent" });
                        }
                        else
                        {
                            HttpCookie authenticationCookies = new HttpCookie("ALoginAttempt");
                            Response.Cookies.Add(authenticationCookies);
                            HttpContext.Request.Cookies.Clear();
                            return Redirect(Common.AgentSession.FormURL);
                        }
                    }



                    else
                    {
                        if (agentLogin.LoginFailedCount == 0)
                        {
                            if (agentLogin.IsActive)
                            {
                                agentLogin.LoginFailedCount = 1;
                                dbContext.Entry(agentLogin).State = EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            ModelState.AddModelError("Invalid", "You have two more attempts to login to the account before it get deactivated");

                        }
                        else if (agentLogin.LoginFailedCount == 1)
                        {
                            if (agentLogin.IsActive)
                            {
                                agentLogin.LoginFailedCount = 2;
                                dbContext.Entry(agentLogin).State = EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            ModelState.AddModelError("Invalid", "You have one more attempt to login to the account before it get deactivated");

                        }
                        else if (agentLogin.LoginFailedCount == 2)
                        {
                            if (agentLogin.IsActive)
                            {
                                agentLogin.IsActive = false;
                                dbContext.Entry(agentLogin).State = EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            ModelState.AddModelError("Invalid", "Your account has been deactivated following three failed login attempts, contact customer service");

                        }
                    }

                }
                else
                {
                    ModelState.AddModelError("Invalid", "Your account has been deactivated, contact customer service");

                }
            }
            else
            {
                ModelState.AddModelError("Invalid", "The agent does not exist for this user account");

            }


            return View(vm);
        }


        public ServiceResult<bool> LockUser()
        {

            string ErrorMsg = "";
            bool IsValid = true;
            HttpCookie authenticationCookies = Request.Cookies["ALoginAttempt"];
            string endTime = "";
            try
            {
                endTime = authenticationCookies.Values["EndDate"];
            }
            catch (Exception)
            {

            }
            FormsAuthenticationTicket ticket2 = new FormsAuthenticationTicket(2, endTime, DateTime.Now, DateTime.Now.AddMinutes(5), true, String.Empty);
            string encryptedTicket2 = FormsAuthentication.Encrypt(ticket2);

            if (authenticationCookies == null)
            {
                authenticationCookies = new HttpCookie("ALoginAttempt", encryptedTicket2);
            }
            int count = authenticationCookies.Values["Count"].ToInt();
            if (count > 5)
            {
                if (string.IsNullOrEmpty(authenticationCookies.Values["EndDate"]))
                {
                    authenticationCookies.Values["EndDate"] = DateTime.Now.AddMinutes(5).TimeOfDay.ToString();

                }
                var EndDate = TimeSpan.Parse(authenticationCookies.Values["EndDate"]);
                Response.Cookies.Add(authenticationCookies);
                if (DateTime.Now.TimeOfDay < EndDate)
                {
                    ErrorMsg = "Something went wrong. Please try again in 5  minutes ";
                    IsValid = false;
                }

            }
            count++;
            authenticationCookies.Values["Count"] = count.ToString();
            Response.Cookies.Add(authenticationCookies);
            HttpContext.Request.Cookies.Clear();

            return new ServiceResult<bool>()
            {
                Data = IsValid,
                Message = ErrorMsg,
                Status = ResultStatus.OK

            };

        }
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {

            ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string EmailAddress)
        {

            string msg = "";
            var agentLogin = dbContext.AgentStaffLogin.Where(x => x.IsActive && x.Username == EmailAddress).FirstOrDefault();
            if (agentLogin != null)
            {
                MailCommon mail = new MailCommon();
                string passwordRequestCode = Common.Common.GenerateRandomDigit(8);
                Common.AgentSession.AgentPasswordRequestCode = passwordRequestCode;
                Common.AgentSession.AgentValidEmailAddress = EmailAddress;
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/PasswordSecirityCodeEmail?UserName="
                    + agentLogin.AgentStaff.FirstName + " " + agentLogin.AgentStaff.MiddleName + " " + agentLogin.AgentStaff.LastName +
                    "&SecurityCode=" + passwordRequestCode);


                mail.SendMail(EmailAddress, "Password reset code", body);

                return RedirectToAction("RequestSecurityCode", new { area = "agent" });

            }
            else
            {
                ModelState.AddModelError("Invalid", "Invalid email address");
                //msg = "Request for Security code is failed due to invalid email address";

            }
            ViewBag.Message = msg;
            return View();
        }
        [HttpGet]
        public ActionResult RequestSecurityCode()
        {
            if (Common.AgentSession.AgentPasswordRequestCode == null)
            {
                return RedirectToAction("ForgotPassword", new { area = "agent" });
            }
            ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        public ActionResult RequestSecurityCode(string PasswordRequestCode)
        {
            string msg = "";
            string validPasswordRequestCode = Common.AgentSession.AgentPasswordRequestCode;
            if (validPasswordRequestCode == PasswordRequestCode)
            {
                return RedirectToAction("ProceedToPasswordReset", new { area = "agent" });
            }
            else
            {
                msg = "Invalid security Code";
            }
            ViewBag.Message = msg;
            return View();
        }
        [HttpGet]
        public ActionResult ProceedToPasswordReset()
        {
            if (Common.AgentSession.AgentValidEmailAddress == null)
            {
                return RedirectToAction("ForgotPassword", new { area = "agent" });
            }
            return View();
        }
        [HttpPost]
        public ActionResult ProceedToPasswordReset([Bind(Include = AgentConfirmResetPasswordViewModel.BindProperty)]AgentConfirmResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string agentEmailAddress = Session["AgentValidEmailAddress"].ToString();
                var agentLogin = dbContext.AgentStaffLogin.Where(x => x.Username == agentEmailAddress).FirstOrDefault();
                if (agentLogin != null)
                {
                    agentLogin.IsActive = false;
                    agentLogin.Password = Common.Common.Encrypt(model.NewPassword);
                    dbContext.Entry(agentLogin).State = EntityState.Modified;
                    int result = dbContext.SaveChanges();
                    if (result == 1)
                    {
                        //MailCommon mail = new MailCommon();
                        //var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                        //var link = string.Format("{0}/BecomeAnAgent/ActivateAgent?ActivationCode={1}", baseUrl, agentLogin.ActivationCode);
                        //try
                        //{
                        //    string mailMessage = string.Format("The following agent {0} with telephone {1} and email {2} has attempted to reset password, please verify and activate account by clicking the link {3}", 
                        //        agentLogin.AgentStaff.FirstName + " " + agentLogin.AgentStaff.MiddleName + " " + agentLogin.AgentStaff.LastName , agentLogin.AgentStaff.PhoneNumber, agentLogin.Username, link);
                        //    mail.SendMail(agentLogin.Username, "About Password Reset.", mailMessage);

                        //}
                        //catch (Exception)
                        //{

                        //}
                    }
                }
                return RedirectToAction("ConfirmPasswordReset", new { area = "agent" });
            }
            return View();
        }
        [HttpGet]
        public ActionResult ConfirmPasswordReset()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
        }

        [HttpGet]
        public ActionResult TransactionAgentFirstLoginSuccessFul()
        {

            return View();

        }

        [HttpGet]
        public JsonResult IsAgentCodeValid(string agentCode)
        {
            bool isvalidAgentcode = false;
            string Messsage = "";
            if (!string.IsNullOrEmpty(agentCode))
            {
                isvalidAgentcode = agentStaffRegistrationServices.IsAgentCodeValid(agentCode);
                if (!isvalidAgentcode)
                {
                    Messsage = "Invalid AgentCode";
                }
            }
            return Json(new { Data = isvalidAgentcode, Message = Messsage }, JsonRequestBehavior.AllowGet);
        }
    }
}